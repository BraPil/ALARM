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
    /// Confounding detection for causal analysis
    /// </summary>
    public class ConfoundingDetection
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<ConfoundingDetection> _logger;

        public ConfoundingDetection(MLContext mlContext, ILogger<ConfoundingDetection> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Detect confounding factors
        /// </summary>
        public async Task<ConfoundingDetectionResult> DetectConfoundingFactorsAsync(
            List<CausalData> data, List<CausalRelationship> relationships, CausalAnalysisConfig config)
        {
            _logger.LogInformation("Detecting confounding factors for {RelationshipCount} relationships", relationships.Count);

            var result = new ConfoundingDetectionResult
            {
                ConfoundingFactors = new List<ConfoundingFactor>(),
                DetectionMetrics = new Dictionary<string, double>()
            };

            // Get all variables in the dataset
            var allVariables = ExtractAllVariables(data);

            foreach (var relationship in relationships)
            {
                var confounders = await DetectConfoundersForRelationshipAsync(
                    data, relationship, allVariables, config);
                result.ConfoundingFactors.AddRange(confounders);
            }

            // Remove duplicate confounders and merge similar ones
            result.ConfoundingFactors = MergeConfoundingFactors(result.ConfoundingFactors);
            
            result.DetectionMetrics = CalculateDetectionMetrics(result.ConfoundingFactors, relationships);

            _logger.LogInformation("Detected {ConfoundingCount} confounding factors", result.ConfoundingFactors.Count);

            return result;
        }

        /// <summary>
        /// Extract all variables from the data
        /// </summary>
        private List<string> ExtractAllVariables(List<CausalData> data)
        {
            var variables = new HashSet<string>();
            
            foreach (var dataPoint in data)
            {
                foreach (var variable in dataPoint.Variables.Keys)
                {
                    variables.Add(variable);
                }
            }
            
            return variables.ToList();
        }

        /// <summary>
        /// Detect confounders for a specific relationship
        /// </summary>
        private async Task<List<ConfoundingFactor>> DetectConfoundersForRelationshipAsync(
            List<CausalData> data, CausalRelationship relationship, List<string> allVariables, CausalAnalysisConfig config)
        {
            var confounders = new List<ConfoundingFactor>();
            
            // Test each variable as a potential confounder
            var potentialConfounders = allVariables
                .Where(v => v != relationship.CauseVariable && v != relationship.EffectVariable)
                .Take(config.MaxConfoundingVariables)
                .ToList();

            foreach (var variable in potentialConfounders)
            {
                var confounder = await TestConfoundingVariable(data, relationship, variable, config);
                if (confounder != null && confounder.Impact > config.ConfoundingThreshold)
                {
                    confounders.Add(confounder);
                }
            }

            return confounders.OrderByDescending(c => c.Impact).ToList();
        }

        /// <summary>
        /// Test if a variable is a confounder
        /// </summary>
        private async Task<ConfoundingFactor?> TestConfoundingVariable(
            List<CausalData> data, CausalRelationship relationship, string potentialConfounder, CausalAnalysisConfig config)
        {
            try
            {
                // Test 1: Association with cause
                var causeAssociation = TestAssociationWithCause(data, relationship, potentialConfounder);
                
                // Test 2: Association with effect
                var effectAssociation = TestAssociationWithEffect(data, relationship, potentialConfounder);
                
                // Test 3: Impact on relationship when controlled
                var controlImpact = TestControlImpact(data, relationship, potentialConfounder);
                
                // A variable is a confounder if it's associated with both cause and effect,
                // and controlling for it changes the relationship
                var isConfounder = causeAssociation.IsSignificant && 
                                 effectAssociation.IsSignificant && 
                                 controlImpact.Impact > config.ConfoundingThreshold;

                if (!isConfounder) return null;

                return new ConfoundingFactor
                {
                    Id = Guid.NewGuid().ToString(),
                    VariableName = potentialConfounder,
                    AffectedRelationships = new List<string> { relationship.Id },
                    Impact = controlImpact.Impact,
                    Confidence = CalculateConfoundingConfidence(causeAssociation, effectAssociation, controlImpact),
                    DetectionMethod = "Statistical Association Test",
                    Evidence = new List<string>
                    {
                        $"Cause association: {causeAssociation.Strength:F3}",
                        $"Effect association: {effectAssociation.Strength:F3}",
                        $"Control impact: {controlImpact.Impact:F3}"
                    },
                    Statistics = new Dictionary<string, double>
                    {
                        ["CauseAssociation"] = causeAssociation.Strength,
                        ["EffectAssociation"] = effectAssociation.Strength,
                        ["ControlImpact"] = controlImpact.Impact,
                        ["CausePValue"] = causeAssociation.PValue,
                        ["EffectPValue"] = effectAssociation.PValue
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to test confounding for variable {Variable}", potentialConfounder);
                return null;
            }
        }

        /// <summary>
        /// Test association between potential confounder and cause variable
        /// </summary>
        private AssociationResult TestAssociationWithCause(List<CausalData> data, CausalRelationship relationship, string potentialConfounder)
        {
            var causeValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.CauseVariable, 0.0)).ToArray();
            var confounterValues = data.Select(d => d.Variables.GetValueOrDefault(potentialConfounder, 0.0)).ToArray();

            var correlation = Correlation.Pearson(causeValues, confounterValues);
            var n = data.Count;
            
            // Calculate t-statistic and p-value
            var tStat = correlation * Math.Sqrt((n - 2) / Math.Max(1 - correlation * correlation, 1e-10));
            var pValue = 2 * (1 - NormalCDF(Math.Abs(tStat)));

            return new AssociationResult
            {
                Strength = Math.Abs(correlation),
                IsSignificant = pValue < 0.05,
                PValue = pValue,
                TStatistic = tStat
            };
        }

        /// <summary>
        /// Test association between potential confounder and effect variable
        /// </summary>
        private AssociationResult TestAssociationWithEffect(List<CausalData> data, CausalRelationship relationship, string potentialConfounder)
        {
            var effectValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.EffectVariable, 0.0)).ToArray();
            var confounterValues = data.Select(d => d.Variables.GetValueOrDefault(potentialConfounder, 0.0)).ToArray();

            var correlation = Correlation.Pearson(effectValues, confounterValues);
            var n = data.Count;
            
            // Calculate t-statistic and p-value
            var tStat = correlation * Math.Sqrt((n - 2) / Math.Max(1 - correlation * correlation, 1e-10));
            var pValue = 2 * (1 - NormalCDF(Math.Abs(tStat)));

            return new AssociationResult
            {
                Strength = Math.Abs(correlation),
                IsSignificant = pValue < 0.05,
                PValue = pValue,
                TStatistic = tStat
            };
        }

        /// <summary>
        /// Test impact of controlling for potential confounder
        /// </summary>
        private ControlImpactResult TestControlImpact(List<CausalData> data, CausalRelationship relationship, string potentialConfounder)
        {
            // Calculate original correlation between cause and effect
            var causeValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.CauseVariable, 0.0)).ToArray();
            var effectValues = data.Select(d => d.Variables.GetValueOrDefault(relationship.EffectVariable, 0.0)).ToArray();
            var confounterValues = data.Select(d => d.Variables.GetValueOrDefault(potentialConfounder, 0.0)).ToArray();

            var originalCorrelation = Correlation.Pearson(causeValues, effectValues);

            // Calculate partial correlation controlling for the potential confounder
            var partialCorrelation = CalculatePartialCorrelation(causeValues, effectValues, confounterValues);

            // Impact is the change in correlation when controlling for the confounder
            var impact = Math.Abs(originalCorrelation - partialCorrelation);

            return new ControlImpactResult
            {
                Impact = impact,
                OriginalCorrelation = originalCorrelation,
                PartialCorrelation = partialCorrelation
            };
        }

        /// <summary>
        /// Calculate partial correlation
        /// </summary>
        private double CalculatePartialCorrelation(double[] x, double[] y, double[] z)
        {
            var rXY = Correlation.Pearson(x, y);
            var rXZ = Correlation.Pearson(x, z);
            var rYZ = Correlation.Pearson(y, z);

            var denominator = Math.Sqrt((1 - rXZ * rXZ) * (1 - rYZ * rYZ));
            if (Math.Abs(denominator) < 1e-10) return 0.0;

            return (rXY - rXZ * rYZ) / denominator;
        }

        /// <summary>
        /// Calculate confounding confidence
        /// </summary>
        private double CalculateConfoundingConfidence(
            AssociationResult causeAssociation, 
            AssociationResult effectAssociation, 
            ControlImpactResult controlImpact)
        {
            // Combine evidence from all tests
            var causeEvidence = causeAssociation.IsSignificant ? (1 - causeAssociation.PValue) : 0.0;
            var effectEvidence = effectAssociation.IsSignificant ? (1 - effectAssociation.PValue) : 0.0;
            var controlEvidence = Math.Min(1.0, controlImpact.Impact * 2.0); // Scale impact to 0-1

            // Weighted average of evidence
            return (causeEvidence * 0.3 + effectEvidence * 0.3 + controlEvidence * 0.4);
        }

        /// <summary>
        /// Merge similar confounding factors
        /// </summary>
        private List<ConfoundingFactor> MergeConfoundingFactors(List<ConfoundingFactor> confounders)
        {
            var merged = new List<ConfoundingFactor>();
            var processed = new HashSet<string>();

            foreach (var confounder in confounders.OrderByDescending(c => c.Impact))
            {
                if (processed.Contains(confounder.VariableName)) continue;

                // Find similar confounders (same variable affecting different relationships)
                var similar = confounders
                    .Where(c => c.VariableName == confounder.VariableName && c.Id != confounder.Id)
                    .ToList();

                if (similar.Any())
                {
                    // Merge into single confounder
                    var mergedConfounder = new ConfoundingFactor
                    {
                        Id = confounder.Id,
                        VariableName = confounder.VariableName,
                        AffectedRelationships = new List<string>(confounder.AffectedRelationships),
                        Impact = confounder.Impact,
                        Confidence = confounder.Confidence,
                        DetectionMethod = confounder.DetectionMethod,
                        Evidence = new List<string>(confounder.Evidence),
                        Statistics = new Dictionary<string, double>(confounder.Statistics)
                    };

                    foreach (var sim in similar)
                    {
                        mergedConfounder.AffectedRelationships.AddRange(sim.AffectedRelationships);
                        mergedConfounder.Evidence.AddRange(sim.Evidence);
                        
                        // Average the impact and confidence
                        mergedConfounder.Impact = (mergedConfounder.Impact + sim.Impact) / 2.0;
                        mergedConfounder.Confidence = (mergedConfounder.Confidence + sim.Confidence) / 2.0;
                        
                        processed.Add(sim.VariableName);
                    }

                    merged.Add(mergedConfounder);
                }
                else
                {
                    merged.Add(confounder);
                }

                processed.Add(confounder.VariableName);
            }

            return merged;
        }

        /// <summary>
        /// Calculate detection metrics
        /// </summary>
        private Dictionary<string, double> CalculateDetectionMetrics(
            List<ConfoundingFactor> confounders, List<CausalRelationship> relationships)
        {
            var metrics = new Dictionary<string, double>();

            metrics["TotalConfounders"] = confounders.Count;
            metrics["AverageImpact"] = confounders.Any() ? confounders.Average(c => c.Impact) : 0.0;
            metrics["MaxImpact"] = confounders.Any() ? confounders.Max(c => c.Impact) : 0.0;
            metrics["AverageConfidence"] = confounders.Any() ? confounders.Average(c => c.Confidence) : 0.0;
            
            // Calculate percentage of relationships affected by confounding
            var affectedRelationships = confounders.SelectMany(c => c.AffectedRelationships).Distinct().Count();
            metrics["PercentageAffected"] = relationships.Count > 0 ? (double)affectedRelationships / relationships.Count : 0.0;
            
            // High-impact confounders
            metrics["HighImpactConfounders"] = confounders.Count(c => c.Impact > 0.7);

            return metrics;
        }

        /// <summary>
        /// Normal CDF approximation
        /// </summary>
        private double NormalCDF(double x)
        {
            return 0.5 * (1 + Math.Sign(x) * Math.Sqrt(1 - Math.Exp(-2 * x * x / Math.PI)));
        }
    }

    #region Helper Classes

    internal class AssociationResult
    {
        public double Strength { get; set; }
        public bool IsSignificant { get; set; }
        public double PValue { get; set; }
        public double TStatistic { get; set; }
    }

    internal class ControlImpactResult
    {
        public double Impact { get; set; }
        public double OriginalCorrelation { get; set; }
        public double PartialCorrelation { get; set; }
    }

    #endregion
}
