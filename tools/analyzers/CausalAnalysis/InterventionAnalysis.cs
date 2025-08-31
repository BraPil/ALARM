using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.Statistics;

namespace ALARM.Analyzers.CausalAnalysis
{
    /// <summary>
    /// Intervention analysis for causal inference
    /// </summary>
    public class InterventionAnalysis
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<InterventionAnalysis> _logger;

        public InterventionAnalysis(MLContext mlContext, ILogger<InterventionAnalysis> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Analyze intervention effects
        /// </summary>
        public async Task<InterventionAnalysisResult> AnalyzeInterventionEffectsAsync(
            List<CausalData> data, List<CausalRelationship> relationships, CausalAnalysisConfig config)
        {
            _logger.LogInformation("Analyzing intervention effects for {RelationshipCount} causal relationships", relationships.Count);

            var result = new InterventionAnalysisResult
            {
                InterventionEffects = new List<InterventionEffect>(),
                InterventionMetrics = new Dictionary<string, double>()
            };

            foreach (var relationship in relationships)
            {
                var effects = await AnalyzeInterventionForRelationshipAsync(data, relationship, config);
                result.InterventionEffects.AddRange(effects);
            }

            result.InterventionMetrics = CalculateInterventionMetrics(result.InterventionEffects);

            _logger.LogInformation("Identified {EffectCount} intervention effects", result.InterventionEffects.Count);

            return result;
        }

        /// <summary>
        /// Analyze intervention effects for a specific relationship
        /// </summary>
        private async Task<List<InterventionEffect>> AnalyzeInterventionForRelationshipAsync(
            List<CausalData> data, CausalRelationship relationship, CausalAnalysisConfig config)
        {
            var effects = new List<InterventionEffect>();

            var causeValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.CauseVariable, 0.0)).ToArray();
            var effectValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.EffectVariable, 0.0)).ToArray();

            if (causeValues.Length < config.MinInterventionSamples)
            {
                return effects;
            }

            // Generate intervention scenarios
            var interventionValues = GenerateInterventionValues(causeValues);

            foreach (var interventionValue in interventionValues)
            {
                var effect = await EstimateInterventionEffectAsync(
                    data, relationship, interventionValue, config);

                if (effect != null && Math.Abs(effect.ExpectedEffect) > config.InterventionEffectThreshold)
                {
                    effects.Add(effect);
                }
            }

            return effects;
        }

        /// <summary>
        /// Generate intervention values to test
        /// </summary>
        private List<double> GenerateInterventionValues(double[] observedValues)
        {
            var mean = observedValues.Mean();
            var stdDev = observedValues.StandardDeviation();
            var min = observedValues.Min();
            var max = observedValues.Max();

            return new List<double>
            {
                mean - 2 * stdDev,  // Low intervention
                mean - stdDev,      // Moderate low
                mean,               // Mean intervention
                mean + stdDev,      // Moderate high
                mean + 2 * stdDev,  // High intervention
                min,                // Minimum observed
                max                 // Maximum observed
            }.Where(v => v >= min && v <= max * 1.2).ToList(); // Allow slight extrapolation
        }

        /// <summary>
        /// Estimate intervention effect
        /// </summary>
        private async Task<InterventionEffect?> EstimateInterventionEffectAsync(
            List<CausalData> data, CausalRelationship relationship, double interventionValue, CausalAnalysisConfig config)
        {
            try
            {
                // Method 1: Direct effect estimation using linear relationship
                var directEffect = EstimateDirectEffect(data, relationship, interventionValue);

                // Method 2: Counterfactual estimation
                var counterfactualEffect = EstimateCounterfactualEffect(data, relationship, interventionValue);

                // Combine estimates
                var combinedEffect = (directEffect + counterfactualEffect) / 2.0;

                // Calculate confidence interval
                var (lowerBound, upperBound) = CalculateConfidenceInterval(data, relationship, interventionValue, combinedEffect);

                return new InterventionEffect
                {
                    Id = Guid.NewGuid().ToString(),
                    InterventionVariable = relationship.CauseVariable,
                    TargetVariable = relationship.EffectVariable,
                    InterventionValue = interventionValue,
                    ExpectedEffect = combinedEffect,
                    ConfidenceIntervalLower = lowerBound,
                    ConfidenceIntervalUpper = upperBound,
                    Probability = CalculateInterventionProbability(combinedEffect, lowerBound, upperBound),
                    InterventionType = ClassifyInterventionType(interventionValue, data, relationship.CauseVariable),
                    Assumptions = GetInterventionAssumptions(),
                    SensitivityAnalysis = PerformSensitivityAnalysis(data, relationship, interventionValue)
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to estimate intervention effect for {Relationship}", 
                    $"{relationship.CauseVariable} -> {relationship.EffectVariable}");
                return null;
            }
        }

        /// <summary>
        /// Estimate direct effect using linear relationship
        /// </summary>
        private double EstimateDirectEffect(List<CausalData> data, CausalRelationship relationship, double interventionValue)
        {
            var causeValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.CauseVariable, 0.0)).ToArray();
            var effectValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.EffectVariable, 0.0)).ToArray();

            // Simple linear regression to estimate effect
            var correlation = Correlation.Pearson(causeValues, effectValues);
            var effectStdDev = effectValues.StandardDeviation();
            var causeStdDev = causeValues.StandardDeviation();
            
            if (causeStdDev == 0) return 0.0;

            var slope = correlation * (effectStdDev / causeStdDev);
            var causeMean = causeValues.Mean();
            
            return slope * (interventionValue - causeMean);
        }

        /// <summary>
        /// Estimate counterfactual effect
        /// </summary>
        private double EstimateCounterfactualEffect(List<CausalData> data, CausalRelationship relationship, double interventionValue)
        {
            // Find similar observations to the intervention value
            var causeValues = data.Select((d, i) => new { Value = d.Variables.GetValueOrDefault(relationship.CauseVariable, 0.0), Index = i }).ToArray();
            var effectValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.EffectVariable, 0.0)).ToArray();

            // Find k-nearest neighbors to intervention value
            var k = Math.Min(10, data.Count / 4);
            var neighbors = causeValues
                .OrderBy(cv => Math.Abs(cv.Value - interventionValue))
                .Take(k)
                .ToArray();

            if (!neighbors.Any()) return 0.0;

            // Calculate weighted average effect
            var totalWeight = 0.0;
            var weightedEffect = 0.0;

            foreach (var neighbor in neighbors)
            {
                var distance = Math.Abs(neighbor.Value - interventionValue);
                var weight = 1.0 / (1.0 + distance); // Inverse distance weighting
                
                totalWeight += weight;
                weightedEffect += weight * effectValues[neighbor.Index];
            }

            var predictedEffect = totalWeight > 0 ? weightedEffect / totalWeight : 0.0;
            var baselineEffect = effectValues.Average();

            return predictedEffect - baselineEffect;
        }

        /// <summary>
        /// Calculate confidence interval for intervention effect
        /// </summary>
        private (double lower, double upper) CalculateConfidenceInterval(
            List<CausalData> data, CausalRelationship relationship, double interventionValue, double expectedEffect)
        {
            // Bootstrap-based confidence interval estimation
            var bootstrapEffects = new List<double>();
            var random = new Random(42);
            var bootstrapSamples = 100;

            for (int i = 0; i < bootstrapSamples; i++)
            {
                var bootstrapData = data.OrderBy(x => random.Next()).Take(data.Count).ToList();
                var bootstrapEffect = EstimateDirectEffect(bootstrapData, relationship, interventionValue);
                bootstrapEffects.Add(bootstrapEffect);
            }

            bootstrapEffects.Sort();
            var lowerIndex = (int)(bootstrapSamples * 0.025);
            var upperIndex = (int)(bootstrapSamples * 0.975);

            return (bootstrapEffects[lowerIndex], bootstrapEffects[upperIndex]);
        }

        /// <summary>
        /// Calculate probability of intervention success
        /// </summary>
        private double CalculateInterventionProbability(double expectedEffect, double lowerBound, double upperBound)
        {
            // Probability based on confidence interval and effect size
            var intervalWidth = upperBound - lowerBound;
            var effectSize = Math.Abs(expectedEffect);
            
            if (intervalWidth <= 0) return 0.5;

            // Higher probability for larger effects with narrower intervals
            var probability = Math.Min(1.0, effectSize / intervalWidth);
            
            // Adjust based on whether effect crosses zero
            if (lowerBound <= 0 && upperBound >= 0)
            {
                probability *= 0.5; // Reduce probability if effect might be zero
            }

            return probability;
        }

        /// <summary>
        /// Classify intervention type
        /// </summary>
        private string ClassifyInterventionType(double interventionValue, List<CausalData> data, string causeVariable)
        {
            var observedValues = data.Select(d => d.Variables.GetValueOrDefault(causeVariable, 0.0)).ToArray();
            var mean = observedValues.Mean();
            var stdDev = observedValues.StandardDeviation();

            if (interventionValue < mean - stdDev)
                return "Decrease";
            else if (interventionValue > mean + stdDev)
                return "Increase";
            else
                return "Moderate Adjustment";
        }

        /// <summary>
        /// Get intervention assumptions
        /// </summary>
        private List<string> GetInterventionAssumptions()
        {
            return new List<string>
            {
                "No unmeasured confounders",
                "Stable unit treatment value assumption (SUTVA)",
                "Positivity assumption (overlap)",
                "Consistency assumption",
                "No interference between units",
                "Correct model specification"
            };
        }

        /// <summary>
        /// Perform sensitivity analysis
        /// </summary>
        private Dictionary<string, double> PerformSensitivityAnalysis(
            List<CausalData> data, CausalRelationship relationship, double interventionValue)
        {
            var sensitivity = new Dictionary<string, double>();

            // Test robustness to different model specifications
            var baseEffect = EstimateDirectEffect(data, relationship, interventionValue);
            
            // Sensitivity to sample size
            var halfSampleEffect = EstimateDirectEffect(data.Take(data.Count / 2).ToList(), relationship, interventionValue);
            sensitivity["SampleSizeSensitivity"] = Math.Abs(baseEffect - halfSampleEffect) / Math.Max(Math.Abs(baseEffect), 0.1);

            // Sensitivity to outliers
            var withoutOutliers = RemoveOutliers(data, relationship.CauseVariable);
            var robustEffect = EstimateDirectEffect(withoutOutliers, relationship, interventionValue);
            sensitivity["OutlierSensitivity"] = Math.Abs(baseEffect - robustEffect) / Math.Max(Math.Abs(baseEffect), 0.1);

            // Sensitivity to intervention value
            var nearbyIntervention = interventionValue * 1.1;
            var nearbyEffect = EstimateDirectEffect(data, relationship, nearbyIntervention);
            sensitivity["InterventionValueSensitivity"] = Math.Abs(baseEffect - nearbyEffect) / Math.Max(Math.Abs(baseEffect), 0.1);

            return sensitivity;
        }

        /// <summary>
        /// Remove outliers from data
        /// </summary>
        private List<CausalData> RemoveOutliers(List<CausalData> data, string variable)
        {
            var values = data.Select(d => d.Variables.GetValueOrDefault(variable, 0.0)).ToArray();
            var q1 = values.Percentile(25);
            var q3 = values.Percentile(75);
            var iqr = q3 - q1;
            var lowerBound = q1 - 1.5 * iqr;
            var upperBound = q3 + 1.5 * iqr;

            return data.Where(d =>
            {
                var value = d.Variables.GetValueOrDefault(variable, 0.0);
                return value >= lowerBound && value <= upperBound;
            }).ToList();
        }

        /// <summary>
        /// Calculate intervention metrics
        /// </summary>
        private Dictionary<string, double> CalculateInterventionMetrics(List<InterventionEffect> effects)
        {
            var metrics = new Dictionary<string, double>();

            if (!effects.Any())
            {
                metrics["AverageEffect"] = 0.0;
                metrics["MaxEffect"] = 0.0;
                metrics["MinEffect"] = 0.0;
                metrics["EffectVariance"] = 0.0;
                return metrics;
            }

            var effectSizes = effects.Select(e => Math.Abs(e.ExpectedEffect)).ToArray();

            metrics["AverageEffect"] = effectSizes.Average();
            metrics["MaxEffect"] = effectSizes.Max();
            metrics["MinEffect"] = effectSizes.Min();
            metrics["EffectVariance"] = effectSizes.Variance();
            metrics["PositiveEffects"] = effects.Count(e => e.ExpectedEffect > 0);
            metrics["NegativeEffects"] = effects.Count(e => e.ExpectedEffect < 0);
            metrics["HighConfidenceEffects"] = effects.Count(e => e.Probability > 0.8);

            return metrics;
        }
    }
}
