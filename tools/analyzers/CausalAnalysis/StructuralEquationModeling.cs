using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace ALARM.Analyzers.CausalAnalysis
{
    /// <summary>
    /// Structural Equation Modeling for causal analysis
    /// </summary>
    public class StructuralEquationModeling
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<StructuralEquationModeling> _logger;

        public StructuralEquationModeling(MLContext mlContext, ILogger<StructuralEquationModeling> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Build structural models from causal relationships
        /// </summary>
        public async Task<StructuralModelingResult> BuildStructuralModelsAsync(
            List<CausalData> data, List<CausalRelationship> relationships, CausalAnalysisConfig config)
        {
            _logger.LogInformation("Building structural equation models for {RelationshipCount} relationships", relationships.Count);

            var result = new StructuralModelingResult
            {
                StructuralEquations = new List<StructuralEquation>(),
                ModelFitStatistics = new Dictionary<string, double>(),
                ModelAssumptions = new List<string>(),
                ConvergedSuccessfully = true
            };

            // Group relationships by effect variable to create equations
            var equationGroups = relationships.GroupBy(r => r.EffectVariable).ToList();

            foreach (var group in equationGroups)
            {
                var equation = await BuildStructuralEquationAsync(data, group.ToList(), config);
                if (equation != null)
                {
                    result.StructuralEquations.Add(equation);
                }
            }

            // Calculate overall model fit statistics
            result.ModelFitStatistics = CalculateModelFitStatistics(result.StructuralEquations, data);
            result.ModelAssumptions = GenerateModelAssumptions();

            _logger.LogInformation("Built {EquationCount} structural equations", result.StructuralEquations.Count);

            return result;
        }

        /// <summary>
        /// Build individual structural equation
        /// </summary>
        private async Task<StructuralEquation?> BuildStructuralEquationAsync(
            List<CausalData> data, List<CausalRelationship> relationships, CausalAnalysisConfig config)
        {
            if (!relationships.Any()) return null;

            var dependentVar = relationships.First().EffectVariable;
            var independentVars = relationships.Select(r => r.CauseVariable).Distinct().ToList();

            _logger.LogInformation("Building equation for {DependentVar} with {IndependentCount} predictors", 
                dependentVar, independentVars.Count);

            // Extract data matrices
            var y = data.Select(d => d.Variables.GetValueOrDefault(dependentVar, 0.0)).ToArray();
            var X = BuildDesignMatrix(data, independentVars);

            if (X.RowCount != y.Length || X.ColumnCount == 0)
            {
                _logger.LogWarning("Invalid data dimensions for equation {DependentVar}", dependentVar);
                return null;
            }

            // Perform multiple linear regression
            var regression = PerformMultipleRegression(X, Vector<double>.Build.DenseOfArray(y));

            if (regression == null) return null;

            var equation = new StructuralEquation
            {
                Id = Guid.NewGuid().ToString(),
                DependentVariable = dependentVar,
                Terms = new List<StructuralTerm>(),
                RSquared = regression.RSquared,
                AdjustedRSquared = regression.AdjustedRSquared,
                StandardError = regression.StandardError,
                GoodnessOfFit = new Dictionary<string, double>()
            };

            // Add intercept term
            if (regression.Coefficients.Count > 0)
            {
                equation.Terms.Add(new StructuralTerm
                {
                    Variable = "Intercept",
                    Coefficient = regression.Intercept,
                    StandardError = regression.InterceptStandardError,
                    TStatistic = regression.InterceptTStatistic,
                    PValue = regression.InterceptPValue,
                    ConfidenceIntervalLower = regression.Intercept - 1.96 * regression.InterceptStandardError,
                    ConfidenceIntervalUpper = regression.Intercept + 1.96 * regression.InterceptStandardError
                });
            }

            // Add variable terms
            for (int i = 0; i < Math.Min(independentVars.Count, regression.Coefficients.Count); i++)
            {
                var coeff = regression.Coefficients[i];
                var stdErr = regression.StandardErrors[i];
                var tStat = Math.Abs(stdErr) > 1e-10 ? coeff / stdErr : 0.0;
                var pValue = 2 * (1 - NormalCDF(Math.Abs(tStat)));

                equation.Terms.Add(new StructuralTerm
                {
                    Variable = independentVars[i],
                    Coefficient = coeff,
                    StandardError = stdErr,
                    TStatistic = tStat,
                    PValue = pValue,
                    ConfidenceIntervalLower = coeff - 1.96 * stdErr,
                    ConfidenceIntervalUpper = coeff + 1.96 * stdErr
                });
            }

            // Calculate goodness of fit measures
            equation.GoodnessOfFit["AIC"] = CalculateAIC(regression, data.Count);
            equation.GoodnessOfFit["BIC"] = CalculateBIC(regression, data.Count);
            equation.GoodnessOfFit["RMSE"] = Math.Sqrt(regression.MeanSquaredError);

            return equation;
        }

        /// <summary>
        /// Build design matrix for regression
        /// </summary>
        private Matrix<double> BuildDesignMatrix(List<CausalData> data, List<string> variables)
        {
            var matrix = Matrix<double>.Build.Dense(data.Count, variables.Count);

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < variables.Count; j++)
                {
                    matrix[i, j] = data[i].Variables.GetValueOrDefault(variables[j], 0.0);
                }
            }

            return matrix;
        }

        /// <summary>
        /// Perform multiple linear regression
        /// </summary>
        private RegressionResult? PerformMultipleRegression(Matrix<double> X, Vector<double> y)
        {
            try
            {
                // Add intercept column
                var XWithIntercept = Matrix<double>.Build.Dense(X.RowCount, X.ColumnCount + 1);
                XWithIntercept.SetSubMatrix(0, 0, X);
                XWithIntercept.SetColumn(X.ColumnCount, Vector<double>.Build.Dense(X.RowCount, 1.0));

                // Calculate coefficients: beta = (X'X)^-1 X'y
                var XTX = XWithIntercept.TransposeThisAndMultiply(XWithIntercept);
                var XTy = XWithIntercept.TransposeThisAndMultiply(y);

                // Check for singularity
                if (XTX.Determinant() == 0)
                {
                    // Use pseudo-inverse for singular matrices
                    var svd = XWithIntercept.Svd();
                    var coefficients = svd.Solve(y);
                    return CreateRegressionResult(coefficients, X, y, XWithIntercept);
                }

                var beta = XTX.Inverse() * XTy;
                return CreateRegressionResult(beta, X, y, XWithIntercept);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to perform multiple regression");
                return null;
            }
        }

        /// <summary>
        /// Create regression result from coefficients
        /// </summary>
        private RegressionResult CreateRegressionResult(Vector<double> beta, Matrix<double> X, Vector<double> y, Matrix<double> XWithIntercept)
        {
            var n = y.Count;
            var k = beta.Count - 1; // Excluding intercept

            // Calculate fitted values and residuals
            var yHat = XWithIntercept * beta;
            var residuals = y - yHat;

            // Calculate R-squared
            var yMean = y.Average();
            var totalSumSquares = y.Sum(yi => Math.Pow(yi - yMean, 2));
            var residualSumSquares = residuals.Sum(r => r * r);
            var rSquared = totalSumSquares > 0 ? 1 - (residualSumSquares / totalSumSquares) : 0.0;

            // Calculate adjusted R-squared
            var adjustedRSquared = n > k + 1 ? 1 - ((1 - rSquared) * (n - 1) / (n - k - 1)) : rSquared;

            // Calculate standard errors
            var mse = residualSumSquares / Math.Max(n - k - 1, 1);
            var covarianceMatrix = mse * (XWithIntercept.TransposeThisAndMultiply(XWithIntercept)).Inverse();
            var standardErrors = new List<double>();

            for (int i = 0; i < beta.Count; i++)
            {
                standardErrors.Add(Math.Sqrt(Math.Max(0, covarianceMatrix[i, i])));
            }

            return new RegressionResult
            {
                Intercept = beta[beta.Count - 1], // Last coefficient is intercept
                Coefficients = beta.Take(beta.Count - 1).ToList(),
                InterceptStandardError = standardErrors.LastOrDefault(),
                StandardErrors = standardErrors.Take(standardErrors.Count - 1).ToList(),
                RSquared = rSquared,
                AdjustedRSquared = adjustedRSquared,
                StandardError = Math.Sqrt(mse),
                MeanSquaredError = mse,
                InterceptTStatistic = standardErrors.LastOrDefault() > 1e-10 ? beta[beta.Count - 1] / standardErrors.LastOrDefault() : 0.0,
                InterceptPValue = CalculatePValue(standardErrors.LastOrDefault() > 1e-10 ? beta[beta.Count - 1] / standardErrors.LastOrDefault() : 0.0)
            };
        }

        /// <summary>
        /// Calculate model fit statistics
        /// </summary>
        private Dictionary<string, double> CalculateModelFitStatistics(List<StructuralEquation> equations, List<CausalData> data)
        {
            var stats = new Dictionary<string, double>();

            if (!equations.Any())
            {
                stats["OverallFit"] = 0.0;
                return stats;
            }

            // Average R-squared across equations
            stats["AverageRSquared"] = equations.Average(e => e.RSquared);
            stats["AverageAdjustedRSquared"] = equations.Average(e => e.AdjustedRSquared);

            // Overall model fit (simplified)
            stats["OverallFit"] = stats["AverageAdjustedRSquared"];

            // Number of equations and parameters
            stats["EquationCount"] = equations.Count;
            stats["TotalParameters"] = equations.Sum(e => e.Terms.Count);

            // Average standard error
            stats["AverageStandardError"] = equations.Average(e => e.StandardError);

            return stats;
        }

        /// <summary>
        /// Generate model assumptions
        /// </summary>
        private List<string> GenerateModelAssumptions()
        {
            return new List<string>
            {
                "Linear relationship between variables",
                "Independence of residuals",
                "Homoscedasticity (constant variance)",
                "Normality of residuals",
                "No perfect multicollinearity",
                "Correct model specification",
                "No measurement error in predictors"
            };
        }

        /// <summary>
        /// Calculate Akaike Information Criterion
        /// </summary>
        private double CalculateAIC(RegressionResult regression, int n)
        {
            var k = regression.Coefficients.Count + 1; // Including intercept
            var logLikelihood = -0.5 * n * Math.Log(2 * Math.PI * regression.MeanSquaredError) - 0.5 * n;
            return 2 * k - 2 * logLikelihood;
        }

        /// <summary>
        /// Calculate Bayesian Information Criterion
        /// </summary>
        private double CalculateBIC(RegressionResult regression, int n)
        {
            var k = regression.Coefficients.Count + 1; // Including intercept
            var logLikelihood = -0.5 * n * Math.Log(2 * Math.PI * regression.MeanSquaredError) - 0.5 * n;
            return Math.Log(n) * k - 2 * logLikelihood;
        }

        /// <summary>
        /// Calculate p-value from t-statistic
        /// </summary>
        private double CalculatePValue(double tStatistic)
        {
            return 2 * (1 - NormalCDF(Math.Abs(tStatistic)));
        }

        /// <summary>
        /// Normal CDF approximation
        /// </summary>
        private double NormalCDF(double x)
        {
            return 0.5 * (1 + Math.Sign(x) * Math.Sqrt(1 - Math.Exp(-2 * x * x / Math.PI)));
        }
    }

    /// <summary>
    /// Regression result helper class
    /// </summary>
    internal class RegressionResult
    {
        public double Intercept { get; set; }
        public List<double> Coefficients { get; set; } = new List<double>();
        public double InterceptStandardError { get; set; }
        public List<double> StandardErrors { get; set; } = new List<double>();
        public double RSquared { get; set; }
        public double AdjustedRSquared { get; set; }
        public double StandardError { get; set; }
        public double MeanSquaredError { get; set; }
        public double InterceptTStatistic { get; set; }
        public double InterceptPValue { get; set; }
    }
}
