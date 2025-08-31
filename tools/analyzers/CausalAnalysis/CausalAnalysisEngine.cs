using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra;

namespace ALARM.Analyzers.CausalAnalysis
{
    /// <summary>
    /// Advanced causal analysis engine for discovering cause-and-effect relationships
    /// Goes beyond correlation to identify actual causal relationships in legacy applications
    /// </summary>
    public class CausalAnalysisEngine
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<CausalAnalysisEngine> _logger;
        private readonly CausalDiscovery _causalDiscovery;
        private readonly StructuralEquationModeling _structuralModeling;
        private readonly InterventionAnalysis _interventionAnalysis;
        private readonly ConfoundingDetection _confoundingDetection;

        public CausalAnalysisEngine(ILogger<CausalAnalysisEngine> logger)
        {
            _mlContext = new MLContext(seed: 42);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _causalDiscovery = new CausalDiscovery(_mlContext, new SimpleLogger<CausalDiscovery>());
            _structuralModeling = new StructuralEquationModeling(_mlContext, new SimpleLogger<StructuralEquationModeling>());
            _interventionAnalysis = new InterventionAnalysis(_mlContext, new SimpleLogger<InterventionAnalysis>());
            _confoundingDetection = new ConfoundingDetection(_mlContext, new SimpleLogger<ConfoundingDetection>());
        }

        /// <summary>
        /// Perform comprehensive causal analysis on legacy application data
        /// </summary>
        public async Task<CausalAnalysisResult> AnalyzeCausalRelationshipsAsync(
            IEnumerable<CausalData> data,
            CausalAnalysisConfig config = null)
        {
            config = config ?? new CausalAnalysisConfig();
            var dataList = data.ToList();
            
            _logger.LogInformation("Starting causal analysis with {DataCount} samples", dataList.Count);
            
            try
            {
                var result = new CausalAnalysisResult
                {
                    AnalysisTimestamp = DateTime.UtcNow,
                    DataSampleCount = dataList.Count,
                    CausalRelationships = new List<CausalRelationship>(),
                    CausalGraph = new CausalGraph(),
                    StructuralEquations = new List<StructuralEquation>(),
                    InterventionEffects = new List<InterventionEffect>(),
                    ConfoundingFactors = new List<ConfoundingFactor>()
                };

                // Phase 1: Causal Discovery
                var discoveryResults = await _causalDiscovery.DiscoverCausalRelationshipsAsync(dataList, config);
                result.CausalRelationships = discoveryResults.CausalRelationships;
                result.CausalGraph = discoveryResults.CausalGraph;

                _logger.LogInformation("Discovered {RelationshipCount} causal relationships", 
                    result.CausalRelationships.Count);

                // Phase 2: Structural Equation Modeling
                var structuralResults = await _structuralModeling.BuildStructuralModelsAsync(
                    dataList, result.CausalRelationships, config);
                result.StructuralEquations = structuralResults.StructuralEquations;
                result.ModelFitStatistics = structuralResults.ModelFitStatistics;

                _logger.LogInformation("Built {EquationCount} structural equations", 
                    result.StructuralEquations.Count);

                // Phase 3: Intervention Analysis
                var interventionResults = await _interventionAnalysis.AnalyzeInterventionEffectsAsync(
                    dataList, result.CausalRelationships, config);
                result.InterventionEffects = interventionResults.InterventionEffects;

                _logger.LogInformation("Identified {InterventionCount} intervention effects", 
                    result.InterventionEffects.Count);

                // Phase 4: Confounding Detection
                var confoundingResults = await _confoundingDetection.DetectConfoundingFactorsAsync(
                    dataList, result.CausalRelationships, config);
                result.ConfoundingFactors = confoundingResults.ConfoundingFactors;

                _logger.LogInformation("Detected {ConfoundingCount} confounding factors", 
                    result.ConfoundingFactors.Count);

                // Phase 5: Causal Strength Calculation
                result.CausalStrengths = CalculateCausalStrengths(result.CausalRelationships, dataList);

                // Phase 6: Statistical Validation
                result.ValidationResults = await ValidateCausalRelationshipsAsync(result, dataList, config);

                // Phase 7: Generate Insights and Recommendations
                result.CausalInsights = GenerateCausalInsights(result);
                result.Recommendations = GenerateCausalRecommendations(result);

                // Calculate overall confidence
                result.OverallConfidence = CalculateOverallCausalConfidence(result);

                _logger.LogInformation("Causal analysis completed with {Confidence:P2} overall confidence", 
                    result.OverallConfidence);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during causal analysis");
                throw;
            }
        }

        /// <summary>
        /// Analyze causal relationships over time to detect changes
        /// </summary>
        public async Task<TemporalCausalAnalysisResult> AnalyzeTemporalCausalRelationshipsAsync(
            IEnumerable<CausalData> timeSeriesData,
            CausalAnalysisConfig config = null)
        {
            config = config ?? new CausalAnalysisConfig();
            var dataList = timeSeriesData.OrderBy(d => d.Timestamp).ToList();
            
            var result = new TemporalCausalAnalysisResult
            {
                AnalysisTimestamp = DateTime.UtcNow,
                TimeWindows = new List<CausalTimeWindow>(),
                CausalStabilityMetrics = new Dictionary<string, double>(),
                CausalChangePoints = new List<CausalChangePoint>()
            };

            // Create sliding time windows
            var windowSize = config.TemporalWindowSize;
            var stepSize = Math.Max(1, windowSize / 4); // 75% overlap

            for (int i = 0; i <= dataList.Count - windowSize; i += stepSize)
            {
                var windowData = dataList.Skip(i).Take(windowSize).ToList();
                var windowAnalysis = await AnalyzeCausalRelationshipsAsync(windowData, config);
                
                var timeWindow = new CausalTimeWindow
                {
                    WindowIndex = i / stepSize,
                    StartTime = windowData.First().Timestamp,
                    EndTime = windowData.Last().Timestamp,
                    CausalAnalysis = windowAnalysis,
                    StabilityScore = CalculateWindowStability(windowAnalysis, result.TimeWindows.LastOrDefault())
                };
                
                result.TimeWindows.Add(timeWindow);
            }

            // Detect causal change points
            result.CausalChangePoints = DetectCausalChangePoints(result.TimeWindows, config);
            
            // Calculate stability metrics
            result.CausalStabilityMetrics = CalculateCausalStabilityMetrics(result.TimeWindows);

            return result;
        }

        /// <summary>
        /// Compare causal relationships between different datasets or conditions
        /// </summary>
        public async Task<CausalComparisonResult> CompareCausalRelationshipsAsync(
            IEnumerable<CausalData> baselineData,
            IEnumerable<CausalData> comparisonData,
            CausalAnalysisConfig config = null)
        {
            config = config ?? new CausalAnalysisConfig();

            var baselineAnalysis = await AnalyzeCausalRelationshipsAsync(baselineData, config);
            var comparisonAnalysis = await AnalyzeCausalRelationshipsAsync(comparisonData, config);

            var result = new CausalComparisonResult
            {
                BaselineAnalysis = baselineAnalysis,
                ComparisonAnalysis = comparisonAnalysis,
                CausalSimilarity = CalculateCausalSimilarity(baselineAnalysis, comparisonAnalysis),
                SignificantDifferences = IdentifySignificantCausalDifferences(baselineAnalysis, comparisonAnalysis),
                CausalEvolution = AnalyzeCausalEvolution(baselineAnalysis, comparisonAnalysis),
                Recommendations = GenerateComparisonRecommendations(baselineAnalysis, comparisonAnalysis)
            };

            return result;
        }

        #region Private Helper Methods

        /// <summary>
        /// Calculate causal strengths for discovered relationships
        /// </summary>
        private Dictionary<string, double> CalculateCausalStrengths(
            List<CausalRelationship> relationships, List<CausalData> data)
        {
            var strengths = new Dictionary<string, double>();

            foreach (var relationship in relationships)
            {
                var strength = CalculateIndividualCausalStrength(relationship, data);
                strengths[relationship.Id] = strength;
            }

            return strengths;
        }

        /// <summary>
        /// Calculate causal strength for individual relationship
        /// </summary>
        private double CalculateIndividualCausalStrength(CausalRelationship relationship, List<CausalData> data)
        {
            // Use multiple methods to estimate causal strength
            var correlationStrength = CalculateCorrelationStrength(relationship, data);
            var temporalStrength = CalculateTemporalStrength(relationship, data);
            var interventionalStrength = CalculateInterventionalStrength(relationship, data);
            
            // Combine strengths with weights
            var combinedStrength = (correlationStrength * 0.3 + temporalStrength * 0.4 + interventionalStrength * 0.3);
            
            return Math.Min(1.0, Math.Max(0.0, combinedStrength));
        }

        /// <summary>
        /// Calculate correlation-based strength
        /// </summary>
        private double CalculateCorrelationStrength(CausalRelationship relationship, List<CausalData> data)
        {
            var causeValues = data.Select(d => GetVariableValue(d, relationship.CauseVariable)).ToArray();
            var effectValues = data.Select(d => GetVariableValue(d, relationship.EffectVariable)).ToArray();
            
            var correlation = Correlation.Pearson(causeValues, effectValues);
            return Math.Abs(correlation);
        }

        /// <summary>
        /// Calculate temporal-based strength (Granger causality)
        /// </summary>
        private double CalculateTemporalStrength(CausalRelationship relationship, List<CausalData> data)
        {
            // Simplified Granger causality test
            var sortedData = data.OrderBy(d => d.Timestamp).ToList();
            
            if (sortedData.Count < 10) return 0.0; // Need sufficient data
            
            var causeValues = sortedData.Select(d => GetVariableValue(d, relationship.CauseVariable)).ToArray();
            var effectValues = sortedData.Select(d => GetVariableValue(d, relationship.EffectVariable)).ToArray();
            
            // Calculate lagged correlation (simplified Granger test)
            var maxLag = Math.Min(5, sortedData.Count / 4);
            var maxGrangerScore = 0.0;
            
            for (int lag = 1; lag <= maxLag; lag++)
            {
                if (causeValues.Length > lag && effectValues.Length > lag)
                {
                    var laggedCause = causeValues.Take(causeValues.Length - lag).ToArray();
                    var laggedEffect = effectValues.Skip(lag).ToArray();
                    
                    if (laggedCause.Length == laggedEffect.Length && laggedCause.Length > 0)
                    {
                        var grangerScore = Math.Abs(Correlation.Pearson(laggedCause, laggedEffect));
                        maxGrangerScore = Math.Max(maxGrangerScore, grangerScore);
                    }
                }
            }
            
            return maxGrangerScore;
        }

        /// <summary>
        /// Calculate intervention-based strength
        /// </summary>
        private double CalculateInterventionalStrength(CausalRelationship relationship, List<CausalData> data)
        {
            // Look for natural experiments or interventions in the data
            var causeValues = data.Select(d => GetVariableValue(d, relationship.CauseVariable)).ToArray();
            var effectValues = data.Select(d => GetVariableValue(d, relationship.EffectVariable)).ToArray();
            
            // Identify potential intervention points (large changes in cause variable)
            var causeChanges = new List<double>();
            var effectChanges = new List<double>();
            
            for (int i = 1; i < causeValues.Length; i++)
            {
                var causeChange = Math.Abs(causeValues[i] - causeValues[i - 1]);
                var effectChange = Math.Abs(effectValues[i] - effectValues[i - 1]);
                
                // If cause change is above threshold, consider it an intervention
                var threshold = causeValues.StandardDeviation() * 1.5;
                if (causeChange > threshold)
                {
                    causeChanges.Add(causeChange);
                    effectChanges.Add(effectChange);
                }
            }
            
            if (causeChanges.Count < 3) return 0.0; // Need sufficient interventions
            
            // Calculate correlation between cause changes and effect changes
            return Math.Abs(Correlation.Pearson(causeChanges.ToArray(), effectChanges.ToArray()));
        }

        /// <summary>
        /// Get variable value from causal data
        /// </summary>
        private double GetVariableValue(CausalData data, string variableName)
        {
            if (data.Variables.ContainsKey(variableName))
            {
                return data.Variables[variableName];
            }
            return 0.0;
        }

        /// <summary>
        /// Validate causal relationships using statistical tests
        /// </summary>
        private async Task<CausalValidationResult> ValidateCausalRelationshipsAsync(
            CausalAnalysisResult result, List<CausalData> data, CausalAnalysisConfig config)
        {
            var validation = new CausalValidationResult
            {
                ValidationTests = new List<CausalValidationTest>(),
                OverallValidationScore = 0.0
            };

            foreach (var relationship in result.CausalRelationships)
            {
                var test = await ValidateIndividualRelationshipAsync(relationship, data, config);
                validation.ValidationTests.Add(test);
            }

            validation.OverallValidationScore = validation.ValidationTests.Any() ? 
                validation.ValidationTests.Average(t => t.ValidationScore) : 0.0;

            return validation;
        }

        /// <summary>
        /// Validate individual causal relationship
        /// </summary>
        private async Task<CausalValidationTest> ValidateIndividualRelationshipAsync(
            CausalRelationship relationship, List<CausalData> data, CausalAnalysisConfig config)
        {
            var test = new CausalValidationTest
            {
                RelationshipId = relationship.Id,
                TestName = $"Validation for {relationship.CauseVariable} → {relationship.EffectVariable}",
                ValidationScore = 0.0,
                TestResults = new Dictionary<string, double>()
            };

            // Test 1: Statistical significance
            var significanceScore = TestStatisticalSignificance(relationship, data);
            test.TestResults["StatisticalSignificance"] = significanceScore;

            // Test 2: Temporal precedence
            var temporalScore = TestTemporalPrecedence(relationship, data);
            test.TestResults["TemporalPrecedence"] = temporalScore;

            // Test 3: Confounding control
            var confoundingScore = TestConfoundingControl(relationship, data);
            test.TestResults["ConfoundingControl"] = confoundingScore;

            // Test 4: Dose-response relationship
            var doseResponseScore = TestDoseResponse(relationship, data);
            test.TestResults["DoseResponse"] = doseResponseScore;

            // Calculate overall validation score
            test.ValidationScore = test.TestResults.Values.Average();
            test.Passed = test.ValidationScore > config.CausalValidationThreshold;

            return test;
        }

        /// <summary>
        /// Test statistical significance of causal relationship
        /// </summary>
        private double TestStatisticalSignificance(CausalRelationship relationship, List<CausalData> data)
        {
            var causeValues = data.Select(d => GetVariableValue(d, relationship.CauseVariable)).ToArray();
            var effectValues = data.Select(d => GetVariableValue(d, relationship.EffectVariable)).ToArray();
            
            var correlation = Correlation.Pearson(causeValues, effectValues);
            var n = data.Count;
            
            // Calculate t-statistic for correlation
            var tStat = correlation * Math.Sqrt((n - 2) / (1 - correlation * correlation));
            
            // Convert to p-value approximation (simplified)
            var pValue = 2 * (1 - NormalCDF(Math.Abs(tStat)));
            
            // Return 1 - p-value (higher is more significant)
            return Math.Max(0, 1 - pValue);
        }

        /// <summary>
        /// Test temporal precedence (cause before effect)
        /// </summary>
        private double TestTemporalPrecedence(CausalRelationship relationship, List<CausalData> data)
        {
            // This is a simplified test - in practice would need more sophisticated temporal analysis
            var temporalStrength = CalculateTemporalStrength(relationship, data);
            return temporalStrength;
        }

        /// <summary>
        /// Test confounding control
        /// </summary>
        private double TestConfoundingControl(CausalRelationship relationship, List<CausalData> data)
        {
            // Simplified confounding test - check if relationship holds when controlling for other variables
            var baseCorrelation = CalculateCorrelationStrength(relationship, data);
            
            // Test robustness by removing potential confounders
            var robustnessScores = new List<double>();
            
            // Get all other variables as potential confounders
            var allVariables = data.SelectMany(d => d.Variables.Keys).Distinct().ToList();
            var potentialConfounders = allVariables.Where(v => 
                v != relationship.CauseVariable && v != relationship.EffectVariable).ToList();
            
            foreach (var confounder in potentialConfounders.Take(5)) // Test top 5 potential confounders
            {
                var partialCorrelation = CalculatePartialCorrelation(relationship, confounder, data);
                robustnessScores.Add(Math.Abs(partialCorrelation - baseCorrelation));
            }
            
            // Higher score means more robust to confounding
            return robustnessScores.Any() ? 1.0 - robustnessScores.Average() : 1.0;
        }

        /// <summary>
        /// Test dose-response relationship
        /// </summary>
        private double TestDoseResponse(CausalRelationship relationship, List<CausalData> data)
        {
            var causeValues = data.Select(d => GetVariableValue(d, relationship.CauseVariable)).ToArray();
            var effectValues = data.Select(d => GetVariableValue(d, relationship.EffectVariable)).ToArray();
            
            // Test if higher "doses" of cause lead to higher effects
            var sortedPairs = causeValues.Zip(effectValues, (c, e) => new { Cause = c, Effect = e })
                                        .OrderBy(p => p.Cause)
                                        .ToArray();
            
            if (sortedPairs.Length < 5) return 0.5; // Default score for insufficient data
            
            // Check for monotonic relationship
            var increases = 0;
            var decreases = 0;
            
            for (int i = 1; i < sortedPairs.Length; i++)
            {
                if (sortedPairs[i].Effect > sortedPairs[i - 1].Effect) increases++;
                else if (sortedPairs[i].Effect < sortedPairs[i - 1].Effect) decreases++;
            }
            
            var totalChanges = increases + decreases;
            return totalChanges > 0 ? (double)increases / totalChanges : 0.5;
        }

        /// <summary>
        /// Calculate partial correlation controlling for confounding variable
        /// </summary>
        private double CalculatePartialCorrelation(CausalRelationship relationship, string confoundingVariable, List<CausalData> data)
        {
            var causeValues = data.Select(d => GetVariableValue(d, relationship.CauseVariable)).ToArray();
            var effectValues = data.Select(d => GetVariableValue(d, relationship.EffectVariable)).ToArray();
            var confoundingValues = data.Select(d => GetVariableValue(d, confoundingVariable)).ToArray();
            
            // Calculate correlations
            var rXY = Correlation.Pearson(causeValues, effectValues);
            var rXZ = Correlation.Pearson(causeValues, confoundingValues);
            var rYZ = Correlation.Pearson(effectValues, confoundingValues);
            
            // Calculate partial correlation
            var denominator = Math.Sqrt((1 - rXZ * rXZ) * (1 - rYZ * rYZ));
            if (Math.Abs(denominator) < 1e-10) return 0.0;
            
            var partialCorrelation = (rXY - rXZ * rYZ) / denominator;
            return partialCorrelation;
        }

        /// <summary>
        /// Normal cumulative distribution function approximation
        /// </summary>
        private double NormalCDF(double x)
        {
            // Approximation of normal CDF
            return 0.5 * (1 + Math.Sign(x) * Math.Sqrt(1 - Math.Exp(-2 * x * x / Math.PI)));
        }

        /// <summary>
        /// Generate causal insights from analysis results
        /// </summary>
        private List<CausalInsight> GenerateCausalInsights(CausalAnalysisResult result)
        {
            var insights = new List<CausalInsight>();

            // Insight 1: Strong causal relationships
            var strongRelationships = result.CausalRelationships
                .Where(r => result.CausalStrengths.GetValueOrDefault(r.Id, 0) > 0.7)
                .ToList();

            if (strongRelationships.Any())
            {
                insights.Add(new CausalInsight
                {
                    Type = CausalInsightType.StrongCausality,
                    Title = "Strong Causal Relationships Identified",
                    Description = $"Found {strongRelationships.Count} strong causal relationships that can guide optimization efforts.",
                    Importance = strongRelationships.Average(r => result.CausalStrengths[r.Id]),
                    RelatedRelationships = strongRelationships.Select(r => r.Id).ToList(),
                    Recommendations = new List<string>
                    {
                        "Focus optimization efforts on these high-impact causal relationships",
                        "Monitor these relationships for changes during system modifications",
                        "Consider these as primary intervention points for improvements"
                    }
                });
            }

            // Insight 2: Confounding factors
            if (result.ConfoundingFactors.Any())
            {
                var significantConfounders = result.ConfoundingFactors.Where(c => c.Impact > 0.5).ToList();
                if (significantConfounders.Any())
                {
                    insights.Add(new CausalInsight
                    {
                        Type = CausalInsightType.ConfoundingDetected,
                        Title = "Significant Confounding Factors Detected",
                        Description = $"Identified {significantConfounders.Count} confounding factors that may affect causal interpretations.",
                        Importance = significantConfounders.Average(c => c.Impact),
                        Recommendations = new List<string>
                        {
                            "Control for these confounding factors in future analyses",
                            "Consider stratified analysis to isolate true causal effects",
                            "Investigate these factors as potential optimization targets"
                        }
                    });
                }
            }

            // Insight 3: Intervention opportunities
            var highImpactInterventions = result.InterventionEffects
                .Where(i => i.ExpectedEffect > 0.6)
                .ToList();

            if (highImpactInterventions.Any())
            {
                insights.Add(new CausalInsight
                {
                    Type = CausalInsightType.InterventionOpportunity,
                    Title = "High-Impact Intervention Opportunities",
                    Description = $"Identified {highImpactInterventions.Count} intervention opportunities with significant expected effects.",
                    Importance = highImpactInterventions.Average(i => i.ExpectedEffect),
                    Recommendations = new List<string>
                    {
                        "Prioritize these interventions for maximum system improvement",
                        "Implement controlled testing of these intervention strategies",
                        "Monitor intervention effects to validate causal models"
                    }
                });
            }

            return insights;
        }

        /// <summary>
        /// Generate causal recommendations
        /// </summary>
        private List<CausalRecommendation> GenerateCausalRecommendations(CausalAnalysisResult result)
        {
            var recommendations = new List<CausalRecommendation>();

            // Recommendation 1: Target strong causal relationships
            var strongestRelationship = result.CausalRelationships
                .OrderByDescending(r => result.CausalStrengths.GetValueOrDefault(r.Id, 0))
                .FirstOrDefault();

            if (strongestRelationship != null)
            {
                recommendations.Add(new CausalRecommendation
                {
                    Type = CausalRecommendationType.Optimization,
                    Priority = CausalRecommendationPriority.High,
                    Title = $"Optimize {strongestRelationship.CauseVariable} to improve {strongestRelationship.EffectVariable}",
                    Description = $"The strongest causal relationship shows that {strongestRelationship.CauseVariable} has significant impact on {strongestRelationship.EffectVariable}.",
                    ExpectedImpact = result.CausalStrengths.GetValueOrDefault(strongestRelationship.Id, 0),
                    RelatedRelationships = new List<string> { strongestRelationship.Id },
                    ActionItems = new List<string>
                    {
                        $"Monitor {strongestRelationship.CauseVariable} closely",
                        $"Implement controls to optimize {strongestRelationship.CauseVariable}",
                        $"Measure impact on {strongestRelationship.EffectVariable}"
                    }
                });
            }

            // Recommendation 2: Address confounding
            var topConfounder = result.ConfoundingFactors
                .OrderByDescending(c => c.Impact)
                .FirstOrDefault();

            if (topConfounder != null && topConfounder.Impact > 0.5)
            {
                recommendations.Add(new CausalRecommendation
                {
                    Type = CausalRecommendationType.Investigation,
                    Priority = CausalRecommendationPriority.Medium,
                    Title = $"Address confounding factor: {topConfounder.VariableName}",
                    Description = $"The variable {topConfounder.VariableName} is confounding multiple causal relationships.",
                    ExpectedImpact = topConfounder.Impact,
                    ActionItems = new List<string>
                    {
                        $"Investigate the role of {topConfounder.VariableName} in the system",
                        "Consider controlling for this factor in future analyses",
                        "Explore whether this factor can be directly optimized"
                    }
                });
            }

            return recommendations;
        }

        /// <summary>
        /// Calculate overall causal confidence
        /// </summary>
        private double CalculateOverallCausalConfidence(CausalAnalysisResult result)
        {
            var scores = new List<double>();

            // Average causal strength
            if (result.CausalStrengths.Any())
            {
                scores.Add(result.CausalStrengths.Values.Average());
            }

            // Validation score
            if (result.ValidationResults != null)
            {
                scores.Add(result.ValidationResults.OverallValidationScore);
            }

            // Model fit (if available)
            if (result.ModelFitStatistics?.ContainsKey("OverallFit") == true)
            {
                scores.Add(result.ModelFitStatistics["OverallFit"]);
            }

            return scores.Any() ? scores.Average() : 0.0;
        }

        /// <summary>
        /// Calculate window stability for temporal analysis
        /// </summary>
        private double CalculateWindowStability(CausalAnalysisResult current, CausalTimeWindow? previous)
        {
            if (previous == null) return 1.0;

            var currentRelationships = current.CausalRelationships.Select(r => $"{r.CauseVariable}->{r.EffectVariable}").ToHashSet();
            var previousRelationships = previous.CausalAnalysis.CausalRelationships.Select(r => $"{r.CauseVariable}->{r.EffectVariable}").ToHashSet();

            var intersection = currentRelationships.Intersect(previousRelationships).Count();
            var union = currentRelationships.Union(previousRelationships).Count();

            return union > 0 ? (double)intersection / union : 1.0;
        }

        /// <summary>
        /// Detect causal change points in temporal analysis
        /// </summary>
        private List<CausalChangePoint> DetectCausalChangePoints(List<CausalTimeWindow> windows, CausalAnalysisConfig config)
        {
            var changePoints = new List<CausalChangePoint>();

            for (int i = 1; i < windows.Count; i++)
            {
                if (windows[i].StabilityScore < config.CausalStabilityThreshold)
                {
                    changePoints.Add(new CausalChangePoint
                    {
                        Timestamp = windows[i].StartTime,
                        WindowIndex = i,
                        StabilityDrop = windows[i - 1].StabilityScore - windows[i].StabilityScore,
                        Description = $"Significant change in causal relationships detected at window {i}"
                    });
                }
            }

            return changePoints;
        }

        /// <summary>
        /// Calculate causal stability metrics
        /// </summary>
        private Dictionary<string, double> CalculateCausalStabilityMetrics(List<CausalTimeWindow> windows)
        {
            var metrics = new Dictionary<string, double>();

            if (windows.Count < 2)
            {
                metrics["AverageStability"] = 1.0;
                metrics["StabilityVariance"] = 0.0;
                metrics["MinStability"] = 1.0;
                metrics["MaxStability"] = 1.0;
                return metrics;
            }

            var stabilityScores = windows.Select(w => w.StabilityScore).ToArray();

            metrics["AverageStability"] = stabilityScores.Average();
            metrics["StabilityVariance"] = stabilityScores.Variance();
            metrics["MinStability"] = stabilityScores.Min();
            metrics["MaxStability"] = stabilityScores.Max();

            return metrics;
        }

        /// <summary>
        /// Calculate causal similarity between two analyses
        /// </summary>
        private double CalculateCausalSimilarity(CausalAnalysisResult baseline, CausalAnalysisResult comparison)
        {
            var baselineRelationships = baseline.CausalRelationships.Select(r => $"{r.CauseVariable}->{r.EffectVariable}").ToHashSet();
            var comparisonRelationships = comparison.CausalRelationships.Select(r => $"{r.CauseVariable}->{r.EffectVariable}").ToHashSet();

            var intersection = baselineRelationships.Intersect(comparisonRelationships).Count();
            var union = baselineRelationships.Union(comparisonRelationships).Count();

            return union > 0 ? (double)intersection / union : 1.0;
        }

        /// <summary>
        /// Identify significant causal differences
        /// </summary>
        private List<CausalDifference> IdentifySignificantCausalDifferences(CausalAnalysisResult baseline, CausalAnalysisResult comparison)
        {
            var differences = new List<CausalDifference>();

            // Find relationships present in baseline but not in comparison
            var baselineRelationships = baseline.CausalRelationships.ToDictionary(r => $"{r.CauseVariable}->{r.EffectVariable}", r => r);
            var comparisonRelationships = comparison.CausalRelationships.ToDictionary(r => $"{r.CauseVariable}->{r.EffectVariable}", r => r);

            foreach (var baselineRel in baselineRelationships)
            {
                if (!comparisonRelationships.ContainsKey(baselineRel.Key))
                {
                    differences.Add(new CausalDifference
                    {
                        Type = CausalDifferenceType.RelationshipLost,
                        Description = $"Causal relationship {baselineRel.Key} no longer present",
                        Relationship = baselineRel.Value,
                        Significance = baseline.CausalStrengths.GetValueOrDefault(baselineRel.Value.Id, 0)
                    });
                }
            }

            // Find relationships present in comparison but not in baseline
            foreach (var comparisonRel in comparisonRelationships)
            {
                if (!baselineRelationships.ContainsKey(comparisonRel.Key))
                {
                    differences.Add(new CausalDifference
                    {
                        Type = CausalDifferenceType.NewRelationship,
                        Description = $"New causal relationship {comparisonRel.Key} discovered",
                        Relationship = comparisonRel.Value,
                        Significance = comparison.CausalStrengths.GetValueOrDefault(comparisonRel.Value.Id, 0)
                    });
                }
            }

            return differences.OrderByDescending(d => d.Significance).ToList();
        }

        /// <summary>
        /// Analyze causal evolution between analyses
        /// </summary>
        private CausalEvolution AnalyzeCausalEvolution(CausalAnalysisResult baseline, CausalAnalysisResult comparison)
        {
            return new CausalEvolution
            {
                OverallSimilarity = CalculateCausalSimilarity(baseline, comparison),
                RelationshipCount = new { Baseline = baseline.CausalRelationships.Count, Comparison = comparison.CausalRelationships.Count },
                StrengthEvolution = AnalyzeStrengthEvolution(baseline, comparison),
                ConfidenceEvolution = new { Baseline = baseline.OverallConfidence, Comparison = comparison.OverallConfidence }
            };
        }

        /// <summary>
        /// Analyze strength evolution
        /// </summary>
        private Dictionary<string, double> AnalyzeStrengthEvolution(CausalAnalysisResult baseline, CausalAnalysisResult comparison)
        {
            var evolution = new Dictionary<string, double>();

            var commonRelationships = baseline.CausalRelationships
                .Where(br => comparison.CausalRelationships.Any(cr => 
                    cr.CauseVariable == br.CauseVariable && cr.EffectVariable == br.EffectVariable))
                .ToList();

            foreach (var relationship in commonRelationships)
            {
                var baselineStrength = baseline.CausalStrengths.GetValueOrDefault(relationship.Id, 0);
                var comparisonRelationship = comparison.CausalRelationships.First(cr => 
                    cr.CauseVariable == relationship.CauseVariable && cr.EffectVariable == relationship.EffectVariable);
                var comparisonStrength = comparison.CausalStrengths.GetValueOrDefault(comparisonRelationship.Id, 0);
                
                evolution[$"{relationship.CauseVariable}->{relationship.EffectVariable}"] = comparisonStrength - baselineStrength;
            }

            return evolution;
        }

        /// <summary>
        /// Generate comparison recommendations
        /// </summary>
        private List<CausalRecommendation> GenerateComparisonRecommendations(CausalAnalysisResult baseline, CausalAnalysisResult comparison)
        {
            var recommendations = new List<CausalRecommendation>();

            // Analyze overall changes
            if (comparison.OverallConfidence > baseline.OverallConfidence)
            {
                recommendations.Add(new CausalRecommendation
                {
                    Type = CausalRecommendationType.Improvement,
                    Priority = CausalRecommendationPriority.High,
                    Title = "Causal Understanding Improved",
                    Description = $"Overall causal confidence increased from {baseline.OverallConfidence:P2} to {comparison.OverallConfidence:P2}",
                    ExpectedImpact = comparison.OverallConfidence - baseline.OverallConfidence,
                    ActionItems = new List<string>
                    {
                        "Continue current optimization approach",
                        "Document successful changes for future reference",
                        "Monitor to ensure improvements are sustained"
                    }
                });
            }
            else if (comparison.OverallConfidence < baseline.OverallConfidence)
            {
                recommendations.Add(new CausalRecommendation
                {
                    Type = CausalRecommendationType.Investigation,
                    Priority = CausalRecommendationPriority.High,
                    Title = "Causal Understanding Degraded",
                    Description = $"Overall causal confidence decreased from {baseline.OverallConfidence:P2} to {comparison.OverallConfidence:P2}",
                    ExpectedImpact = baseline.OverallConfidence - comparison.OverallConfidence,
                    ActionItems = new List<string>
                    {
                        "Investigate causes of degradation",
                        "Consider reverting recent changes",
                        "Analyze new confounding factors"
                    }
                });
            }

            return recommendations;
        }

        #endregion
    }

    /// <summary>
    /// Simple logger implementation for causal analysis components
    /// </summary>
    internal class SimpleLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Console.WriteLine($"[{logLevel}] {typeof(T).Name}: {formatter(state, exception)}");
        }
    }
}
