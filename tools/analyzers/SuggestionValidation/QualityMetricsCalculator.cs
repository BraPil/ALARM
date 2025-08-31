using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using ALARM.Analyzers.PatternDetection;
using ALARM.Analyzers.CausalAnalysis;
using ALARM.Analyzers.Performance;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Calculates quality metrics for suggestions across different analysis types
    /// </summary>
    public class QualityMetricsCalculator
    {
        private readonly MLContext _mlContext;
        private readonly ILogger _logger;
        private readonly Dictionary<string, double> _qualityWeights;

        public QualityMetricsCalculator(MLContext mlContext, ILogger logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Initialize quality metric weights
            _qualityWeights = new Dictionary<string, double>
            {
                ["Relevance"] = 0.25,
                ["Actionability"] = 0.20,
                ["Specificity"] = 0.15,
                ["Feasibility"] = 0.20,
                ["ImpactPotential"] = 0.20
            };
        }

        /// <summary>
        /// Calculate comprehensive quality metrics for pattern detection suggestions
        /// </summary>
        public async Task<Dictionary<string, double>> CalculatePatternSuggestionQualityAsync(
            IEnumerable<PatternRecommendation> suggestions,
            AdvancedPatternResult patternResult,
            List<IndividualSuggestionValidation> validations)
        {
            var metrics = new Dictionary<string, double>();
            var suggestionsList = suggestions.ToList();

            if (!suggestionsList.Any())
            {
                return metrics;
            }

            // Overall quality distribution
            var qualityScores = validations.Select(v => v.OverallScore).ToArray();
            metrics["AverageQuality"] = qualityScores.Average();
            metrics["QualityStandardDeviation"] = CalculateStandardDeviation(qualityScores);
            metrics["HighQualityPercentage"] = qualityScores.Count(q => q > 0.8) / (double)qualityScores.Length;
            metrics["LowQualityPercentage"] = qualityScores.Count(q => q < 0.6) / (double)qualityScores.Length;

            // Relevance metrics
            var relevanceScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "Relevance").Select(qs => qs.Value)).ToArray();
            if (relevanceScores.Any())
            {
                metrics["AverageRelevance"] = relevanceScores.Average();
                metrics["RelevanceConsistency"] = 1.0 - (CalculateStandardDeviation(relevanceScores) / Math.Max(relevanceScores.Average(), 0.1));
            }

            // Actionability metrics
            var actionabilityScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "Actionability").Select(qs => qs.Value)).ToArray();
            if (actionabilityScores.Any())
            {
                metrics["AverageActionability"] = actionabilityScores.Average();
                metrics["ActionabilityDistribution"] = CalculateDistributionMetric(actionabilityScores);
            }

            // Pattern-specific metrics
            metrics["PatternAlignmentScore"] = await CalculatePatternAlignmentScoreAsync(suggestionsList, patternResult);
            metrics["SuggestionDiversity"] = CalculateSuggestionDiversity(suggestionsList);
            metrics["ImplementationComplexity"] = await CalculateAverageImplementationComplexityAsync(suggestionsList);

            // Confidence-based metrics
            var confidenceScores = suggestionsList.Where(s => s.Pattern != null).Select(s => s.Pattern.Confidence).ToArray();
            if (confidenceScores.Any())
            {
                metrics["ConfidenceAlignment"] = CalculateConfidenceAlignment(confidenceScores, qualityScores);
            }

            _logger.LogDebug("Calculated {MetricCount} pattern suggestion quality metrics", metrics.Count);
            return metrics;
        }

        /// <summary>
        /// Calculate quality metrics for causal analysis suggestions
        /// </summary>
        public async Task<Dictionary<string, double>> CalculateCausalSuggestionQualityAsync(
            IEnumerable<CausalRecommendation> suggestions,
            CausalAnalysisResult causalResult,
            List<IndividualSuggestionValidation> validations)
        {
            var metrics = new Dictionary<string, double>();
            var suggestionsList = suggestions.ToList();

            if (!suggestionsList.Any())
            {
                return metrics;
            }

            var qualityScores = validations.Select(v => v.OverallScore).ToArray();
            metrics["AverageQuality"] = qualityScores.Average();
            metrics["QualityVariance"] = CalculateVariance(qualityScores);

            // Causal-specific metrics
            var causalValidityScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "CausalValidity").Select(qs => qs.Value)).ToArray();
            if (causalValidityScores.Any())
            {
                metrics["AverageCausalValidity"] = causalValidityScores.Average();
                metrics["CausalValidityReliability"] = CalculateReliabilityScore(causalValidityScores);
            }

            var evidenceStrengthScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "EvidenceStrength").Select(qs => qs.Value)).ToArray();
            if (evidenceStrengthScores.Any())
            {
                metrics["AverageEvidenceStrength"] = evidenceStrengthScores.Average();
                metrics["EvidenceConsistency"] = 1.0 - CalculateVariationCoefficient(evidenceStrengthScores);
            }

            // Risk assessment metrics
            var riskScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "RiskLevel").Select(qs => qs.Value)).ToArray();
            if (riskScores.Any())
            {
                metrics["AverageRiskAssessment"] = riskScores.Average();
                metrics["RiskDistribution"] = CalculateDistributionMetric(riskScores);
            }

            // Expected impact alignment
            var expectedImpacts = suggestionsList.Select(s => s.ExpectedImpact).ToArray();
            if (expectedImpacts.Any())
            {
                metrics["AverageExpectedImpact"] = expectedImpacts.Average();
                metrics["ImpactRealism"] = await CalculateImpactRealismAsync(expectedImpacts, causalResult);
            }

            // Causal strength alignment
            metrics["CausalStrengthAlignment"] = await CalculateCausalStrengthAlignmentAsync(suggestionsList, causalResult);

            _logger.LogDebug("Calculated {MetricCount} causal suggestion quality metrics", metrics.Count);
            return metrics;
        }

        /// <summary>
        /// Calculate quality metrics for performance suggestions
        /// </summary>
        public async Task<Dictionary<string, double>> CalculatePerformanceSuggestionQualityAsync(
            IEnumerable<string> suggestions,
            PerformanceAdjustmentResult performanceResult,
            List<IndividualSuggestionValidation> validations)
        {
            var metrics = new Dictionary<string, double>();
            var suggestionsList = suggestions.ToList();

            if (!suggestionsList.Any())
            {
                return metrics;
            }

            var qualityScores = validations.Select(v => v.OverallScore).ToArray();
            metrics["AverageQuality"] = qualityScores.Average();
            metrics["QualityRange"] = qualityScores.Max() - qualityScores.Min();

            // Technical accuracy metrics
            var technicalAccuracyScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "TechnicalAccuracy").Select(qs => qs.Value)).ToArray();
            if (technicalAccuracyScores.Any())
            {
                metrics["AverageTechnicalAccuracy"] = technicalAccuracyScores.Average();
                metrics["TechnicalAccuracyReliability"] = CalculateReliabilityScore(technicalAccuracyScores);
            }

            // Implementation complexity metrics
            var complexityScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "ImplementationComplexity").Select(qs => qs.Value)).ToArray();
            if (complexityScores.Any())
            {
                metrics["AverageImplementationComplexity"] = complexityScores.Average();
                metrics["ComplexityBalance"] = CalculateComplexityBalance(complexityScores, qualityScores);
            }

            // Performance impact metrics
            var performanceImpactScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "PerformanceImpact").Select(qs => qs.Value)).ToArray();
            if (performanceImpactScores.Any())
            {
                metrics["AveragePerformanceImpact"] = performanceImpactScores.Average();
                metrics["ImpactPredictability"] = CalculateImpactPredictability(performanceImpactScores, performanceResult);
            }

            // Resource requirement metrics
            var resourceScores = validations.SelectMany(v => v.QualityScores.Where(qs => qs.Key == "ResourceRequirements").Select(qs => qs.Value)).ToArray();
            if (resourceScores.Any())
            {
                metrics["AverageResourceRequirements"] = resourceScores.Average();
                metrics["ResourceEfficiency"] = CalculateResourceEfficiency(resourceScores, performanceImpactScores);
            }

            // Performance context alignment
            metrics["PerformanceContextAlignment"] = await CalculatePerformanceContextAlignmentAsync(suggestionsList, performanceResult);

            _logger.LogDebug("Calculated {MetricCount} performance suggestion quality metrics", metrics.Count);
            return metrics;
        }

        #region Individual Quality Assessment Methods

        /// <summary>
        /// Assess relevance of a pattern suggestion
        /// </summary>
        public async Task<double> AssessRelevanceAsync(PatternRecommendation suggestion, AdvancedPatternResult patternResult)
        {
            var relevanceScore = 0.0;

            // Check if suggestion addresses identified patterns
            if (suggestion.Pattern != null && patternResult.PatternAnalysis?.IdentifiedPatterns != null)
            {
                var relatedPatterns = patternResult.PatternAnalysis.IdentifiedPatterns
                    .Where(p => p.Name.Contains(suggestion.Pattern.Name, StringComparison.OrdinalIgnoreCase) ||
                               suggestion.Description.Contains(p.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (relatedPatterns.Any())
                {
                    relevanceScore += 0.6; // IMPROVED: Higher base relevance for addressing identified patterns
                    relevanceScore += Math.Min(0.25, relatedPatterns.Average(p => p.Confidence) * 0.25); // Bonus for high-confidence patterns
                }
            }

            // Check confidence alignment
            if (suggestion.Pattern != null)
            {
                var confidenceAlignment = Math.Min(1.0, suggestion.Pattern.Confidence * 1.2);
                relevanceScore += confidenceAlignment * 0.3;
            }

            return Math.Min(1.0, relevanceScore);
        }

        /// <summary>
        /// Assess actionability of a suggestion
        /// </summary>
        public async Task<double> AssessActionabilityAsync(PatternRecommendation suggestion)
        {
            var actionabilityScore = 0.0;

            // Check for specific action items
            if (suggestion.SuggestedActions?.Any() == true)
            {
                actionabilityScore += 0.5; // IMPROVED: Higher base score for having action items
                
                // Bonus for multiple specific actions
                var specificActions = suggestion.SuggestedActions.Count(a => ContainsSpecificAction(a));
                actionabilityScore += Math.Min(0.35, specificActions * 0.15); // IMPROVED: More generous bonus
            }

            // Check description specificity
            if (!string.IsNullOrEmpty(suggestion.Description))
            {
                actionabilityScore += CalculateDescriptionActionability(suggestion.Description);
            }

            // Check for implementation guidance
            if (suggestion.Type == RecommendationType.Optimization)
            {
                actionabilityScore += 0.15; // IMPROVED: Higher bonus for optimization suggestions
            }
            
            // IMPROVED: Additional bonus for having a clear description
            if (!string.IsNullOrEmpty(suggestion.Description) && suggestion.Description.Length > 20)
            {
                actionabilityScore += 0.1; // Bonus for detailed descriptions
            }

            return Math.Min(1.0, actionabilityScore);
        }

        /// <summary>
        /// Assess specificity of a suggestion
        /// </summary>
        public async Task<double> AssessSpecificityAsync(PatternRecommendation suggestion)
        {
            var specificityScore = 0.0;

            // Check description specificity
            if (!string.IsNullOrEmpty(suggestion.Description))
            {
                specificityScore += CalculateDescriptionSpecificity(suggestion.Description);
            }

            // Check for specific patterns referenced
            if (suggestion.Pattern != null)
            {
                specificityScore += 0.2;
                
                if (!string.IsNullOrEmpty(suggestion.Pattern.Description))
                {
                    specificityScore += 0.1;
                }
            }

            // Check for quantifiable elements
            if (ContainsQuantifiableElements(suggestion.Description))
            {
                specificityScore += 0.2;
            }

            return Math.Min(1.0, specificityScore);
        }

        /// <summary>
        /// Assess feasibility of a suggestion
        /// </summary>
        public async Task<double> AssessFeasibilityAsync(PatternRecommendation suggestion, ValidationContext context)
        {
            var feasibilityScore = 0.6; // IMPROVED: Higher default baseline score

            // Assess based on suggestion type
            feasibilityScore += suggestion.Type switch
            {
                RecommendationType.Optimization => 0.25, // IMPROVED: More generous for optimization
                RecommendationType.Investigation => 0.3,  // Highly feasible
                RecommendationType.Monitoring => 0.25,   // Usually feasible
                _ => 0.15 // IMPROVED: Higher default bonus
            };

            // ENHANCED: Use detailed system complexity information
            if (context.ComplexityInfo != null)
            {
                // Adjust based on system complexity
                var complexityAdjustment = (1.0 - context.ComplexityInfo.ComplexityScore) * 0.1;
                feasibilityScore += complexityAdjustment;

                // Bonus for suggestions targeting critical components
                if (suggestion.Description != null && context.ComplexityInfo.CriticalComponents.Any(c => 
                    suggestion.Description.Contains(c, StringComparison.OrdinalIgnoreCase)))
                {
                    feasibilityScore += 0.1; // Higher priority for critical components
                }
            }

            // ENHANCED: Use domain expertise information
            if (context.DomainContext != null)
            {
                var domainRelevance = context.DomainContext.PrimaryDomains
                    .Where(domain => suggestion.Description?.Contains(domain, StringComparison.OrdinalIgnoreCase) == true)
                    .Select(domain => context.DomainContext.DomainExpertise.GetValueOrDefault(domain, 0.5))
                    .DefaultIfEmpty(0.5)
                    .Average();
                
                feasibilityScore += domainRelevance * 0.15; // Bonus for domain alignment
            }

            // Assess based on priority (higher priority suggests more feasible)
            feasibilityScore += suggestion.Priority switch
            {
                RecommendationPriority.High => 0.15,
                RecommendationPriority.Medium => 0.1,
                RecommendationPriority.Low => 0.05,
                _ => 0.0
            };

            // Consider system complexity (from context)
            if (context.AdditionalContext.TryGetValue("SystemComplexity", out var complexityStr) &&
                Enum.TryParse<SystemComplexity>(complexityStr, out var complexity))
            {
                feasibilityScore += complexity switch
                {
                    SystemComplexity.Low => 0.15,
                    SystemComplexity.Medium => 0.05,
                    SystemComplexity.High => -0.1,
                    SystemComplexity.VeryHigh => -0.2,
                    _ => 0.0
                };
            }

            return Math.Max(0.0, Math.Min(1.0, feasibilityScore));
        }

        /// <summary>
        /// Assess impact potential of a suggestion
        /// </summary>
        public async Task<double> AssessImpactPotentialAsync(PatternRecommendation suggestion, AdvancedPatternResult patternResult)
        {
            var impactScore = 0.0;

            // Base impact from pattern confidence
            if (suggestion.Pattern != null)
            {
                impactScore += suggestion.Pattern.Confidence * 0.3;
            }

            // Impact from suggestion priority
            impactScore += suggestion.Priority switch
            {
                RecommendationPriority.High => 0.4,
                RecommendationPriority.Medium => 0.25,
                RecommendationPriority.Low => 0.1,
                _ => 0.0
            };

            // Impact from suggestion type
            impactScore += suggestion.Type switch
            {
                RecommendationType.Optimization => 0.3, // High impact potential
                RecommendationType.Investigation => 0.15, // Medium impact potential
                RecommendationType.Monitoring => 0.1,   // Lower direct impact
                _ => 0.1
            };

            return Math.Min(1.0, impactScore);
        }

        /// <summary>
        /// Assess causal validity of a causal suggestion
        /// </summary>
        public async Task<double> AssessCausalValidityAsync(CausalRecommendation suggestion, CausalAnalysisResult causalResult)
        {
            var validityScore = 0.0;

            // Check if suggestion is based on validated causal relationships
            if (suggestion.RelatedRelationships?.Any() == true && causalResult.ValidationResults != null)
            {
                var validatedRelationships = suggestion.RelatedRelationships
                    .Where(relId => causalResult.ValidationResults.ValidationTests
                        .Any(test => test.RelationshipId == relId && test.Passed))
                    .Count();

                if (validatedRelationships > 0)
                {
                    validityScore += Math.Min(0.5, validatedRelationships * 0.2);
                }
            }

            // Check causal strength alignment
            if (causalResult.CausalStrengths != null && suggestion.RelatedRelationships?.Any() == true)
            {
                var avgStrength = suggestion.RelatedRelationships
                    .Where(relId => causalResult.CausalStrengths.ContainsKey(relId))
                    .Select(relId => causalResult.CausalStrengths[relId])
                    .DefaultIfEmpty(0.0)
                    .Average();

                validityScore += avgStrength * 0.4;
            }

            // Overall confidence bonus
            validityScore += causalResult.OverallConfidence * 0.1;

            return Math.Min(1.0, validityScore);
        }

        /// <summary>
        /// Assess evidence strength for causal suggestion
        /// </summary>
        public async Task<double> AssessEvidenceStrengthAsync(CausalRecommendation suggestion, CausalAnalysisResult causalResult)
        {
            var evidenceScore = 0.0;

            // Statistical significance of related relationships
            if (causalResult.ValidationResults?.ValidationTests != null && suggestion.RelatedRelationships?.Any() == true)
            {
                var relevantTests = causalResult.ValidationResults.ValidationTests
                    .Where(test => suggestion.RelatedRelationships.Contains(test.RelationshipId))
                    .ToList();

                if (relevantTests.Any())
                {
                    evidenceScore += relevantTests.Average(test => test.ValidationScore) * 0.6;
                }
            }

            // Data sample size consideration
            evidenceScore += Math.Min(0.3, causalResult.DataSampleCount / 1000.0 * 0.3);

            // Model fit quality
            if (causalResult.ModelFitStatistics?.ContainsKey("OverallFit") == true)
            {
                evidenceScore += causalResult.ModelFitStatistics["OverallFit"] * 0.1;
            }

            return Math.Min(1.0, evidenceScore);
        }

        /// <summary>
        /// Assess practical applicability of causal suggestion
        /// </summary>
        public async Task<double> AssessPracticalApplicabilityAsync(CausalRecommendation suggestion, ValidationContext context)
        {
            var applicabilityScore = 0.5; // Default neutral score

            // Assess based on suggestion type
            applicabilityScore += suggestion.Type switch
            {
                CausalRecommendationType.Optimization => 0.3,
                CausalRecommendationType.Investigation => 0.2,
                CausalRecommendationType.Intervention => 0.1, // More complex to apply
                _ => 0.0
            };

            // Consider expected impact (higher impact = more worth applying)
            applicabilityScore += Math.Min(0.2, suggestion.ExpectedImpact * 0.2);

            return Math.Min(1.0, applicabilityScore);
        }

        /// <summary>
        /// Assess risk level of causal suggestion
        /// </summary>
        public async Task<double> AssessRiskLevelAsync(CausalRecommendation suggestion, CausalAnalysisResult causalResult)
        {
            var riskScore = 0.5; // Start with medium risk

            // Lower risk for investigation suggestions
            if (suggestion.Type == CausalRecommendationType.Investigation)
            {
                riskScore += 0.3; // Lower risk
            }

            // Higher risk for intervention suggestions
            if (suggestion.Type == CausalRecommendationType.Intervention)
            {
                riskScore -= 0.2; // Higher risk
            }

            // Consider causal strength (stronger causality = lower risk)
            if (causalResult.CausalStrengths != null && suggestion.RelatedRelationships?.Any() == true)
            {
                var avgStrength = suggestion.RelatedRelationships
                    .Where(relId => causalResult.CausalStrengths.ContainsKey(relId))
                    .Select(relId => causalResult.CausalStrengths[relId])
                    .DefaultIfEmpty(0.0)
                    .Average();

                riskScore += avgStrength * 0.2; // Higher strength = lower risk
            }

            return Math.Max(0.0, Math.Min(1.0, riskScore));
        }

        /// <summary>
        /// Assess technical accuracy of performance suggestion
        /// </summary>
        public async Task<double> AssessTechnicalAccuracyAsync(string suggestion, PerformanceAdjustmentResult performanceResult)
        {
            var accuracyScore = 0.5; // Default neutral score

            // Check for specific performance metrics mentioned
            if (ContainsPerformanceMetrics(suggestion))
            {
                accuracyScore += 0.2;
            }

            // Check alignment with actual performance issues
            if (performanceResult.ExecutionTimeMs > 5000 && suggestion.ToLower().Contains("time"))
            {
                accuracyScore += 0.15;
            }

            if (performanceResult.MemoryUsedMB > 100 && suggestion.ToLower().Contains("memory"))
            {
                accuracyScore += 0.15;
            }

            // Check for technical best practices
            if (ContainsTechnicalBestPractices(suggestion))
            {
                accuracyScore += 0.2;
            }

            return Math.Min(1.0, accuracyScore);
        }

        /// <summary>
        /// Assess implementation complexity of performance suggestion
        /// </summary>
        public async Task<double> AssessImplementationComplexityAsync(string suggestion)
        {
            var complexityScore = 0.5; // Default medium complexity

            // Simple optimizations
            if (ContainsSimpleOptimizations(suggestion))
            {
                complexityScore += 0.3; // Lower complexity (higher score)
            }

            // Complex architectural changes
            if (ContainsArchitecturalChanges(suggestion))
            {
                complexityScore -= 0.2; // Higher complexity (lower score)
            }

            // Configuration changes
            if (ContainsConfigurationChanges(suggestion))
            {
                complexityScore += 0.2; // Relatively simple
            }

            return Math.Max(0.0, Math.Min(1.0, complexityScore));
        }

        /// <summary>
        /// Assess performance impact of suggestion
        /// </summary>
        public async Task<double> AssessPerformanceImpactAsync(string suggestion, PerformanceAdjustmentResult performanceResult)
        {
            var impactScore = 0.0;

            // High-impact optimizations
            if (ContainsHighImpactOptimizations(suggestion))
            {
                impactScore += 0.4;
            }

            // Medium-impact optimizations
            if (ContainsMediumImpactOptimizations(suggestion))
            {
                impactScore += 0.25;
            }

            // Alignment with current performance issues
            if (performanceResult.ExecutionTimeMs > 10000 && suggestion.ToLower().Contains("parallel"))
            {
                impactScore += 0.2;
            }

            if (performanceResult.MemoryUsedMB > 500 && suggestion.ToLower().Contains("cache"))
            {
                impactScore += 0.15;
            }

            return Math.Min(1.0, impactScore);
        }

        /// <summary>
        /// Assess resource requirements for performance suggestion
        /// </summary>
        public async Task<double> AssessResourceRequirementsAsync(string suggestion, ValidationContext context)
        {
            var resourceScore = 0.5; // Default medium requirements

            // Low resource requirements
            if (ContainsLowResourceChanges(suggestion))
            {
                resourceScore += 0.3;
            }

            // High resource requirements
            if (ContainsHighResourceChanges(suggestion))
            {
                resourceScore -= 0.2;
            }

            // Consider available resources from context
            if (context.EnvironmentInfo.TryGetValue("AvailableMemoryGB", out var memoryObj) &&
                memoryObj is double memory)
            {
                if (memory > 8)
                {
                    resourceScore += 0.1; // More resources available
                }
                else if (memory < 4)
                {
                    resourceScore -= 0.1; // Limited resources
                }
            }

            return Math.Max(0.0, Math.Min(1.0, resourceScore));
        }

        #endregion

        #region Private Helper Methods

        private double CalculateStandardDeviation(double[] values)
        {
            if (values.Length < 2) return 0.0;
            var mean = values.Average();
            var variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
            return Math.Sqrt(variance);
        }

        private double CalculateVariance(double[] values)
        {
            if (values.Length < 2) return 0.0;
            var mean = values.Average();
            return values.Select(v => Math.Pow(v - mean, 2)).Average();
        }

        private double CalculateVariationCoefficient(double[] values)
        {
            if (values.Length < 2) return 0.0;
            var mean = values.Average();
            if (Math.Abs(mean) < 1e-10) return 0.0;
            return CalculateStandardDeviation(values) / mean;
        }

        private double CalculateDistributionMetric(double[] values)
        {
            if (values.Length < 2) return 1.0;
            
            // Calculate how evenly distributed the values are
            var sortedValues = values.OrderBy(v => v).ToArray();
            var gaps = new List<double>();
            
            for (int i = 1; i < sortedValues.Length; i++)
            {
                gaps.Add(sortedValues[i] - sortedValues[i - 1]);
            }
            
            var avgGap = gaps.Average();
            var gapVariance = gaps.Select(g => Math.Pow(g - avgGap, 2)).Average();
            
            return 1.0 / (1.0 + gapVariance); // Higher score for more even distribution
        }

        private double CalculateReliabilityScore(double[] values)
        {
            if (values.Length < 2) return 1.0;
            
            // Reliability based on consistency (low variance = high reliability)
            var variance = CalculateVariance(values);
            return 1.0 / (1.0 + variance);
        }

        private async Task<double> CalculatePatternAlignmentScoreAsync(List<PatternRecommendation> suggestions, AdvancedPatternResult patternResult)
        {
            if (patternResult.PatternAnalysis?.IdentifiedPatterns == null) return 0.5;
            
            var alignmentScore = 0.0;
            var identifiedPatterns = patternResult.PatternAnalysis.IdentifiedPatterns.ToList();
            
            foreach (var suggestion in suggestions)
            {
                if (suggestion.Pattern != null)
                {
                    var matchingPatterns = identifiedPatterns.Where(p => 
                        p.Name.Contains(suggestion.Pattern.Name, StringComparison.OrdinalIgnoreCase) ||
                        suggestion.Pattern.Name.Contains(p.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                    
                    if (matchingPatterns.Any())
                    {
                        alignmentScore += matchingPatterns.Average(p => p.Confidence);
                    }
                }
            }
            
            return suggestions.Any() ? alignmentScore / suggestions.Count : 0.0;
        }

        private double CalculateSuggestionDiversity(List<PatternRecommendation> suggestions)
        {
            if (suggestions.Count < 2) return 1.0;
            
            var types = suggestions.Select(s => s.Type).Distinct().Count();
            var priorities = suggestions.Select(s => s.Priority).Distinct().Count();
            
            var maxTypes = Enum.GetValues<RecommendationType>().Length;
            var maxPriorities = Enum.GetValues<RecommendationPriority>().Length;
            
            var typeDiversity = (double)types / Math.Min(maxTypes, suggestions.Count);
            var priorityDiversity = (double)priorities / Math.Min(maxPriorities, suggestions.Count);
            
            return (typeDiversity + priorityDiversity) / 2.0;
        }

        private async Task<double> CalculateAverageImplementationComplexityAsync(List<PatternRecommendation> suggestions)
        {
            var complexityScores = new List<double>();
            
            foreach (var suggestion in suggestions)
            {
                var complexity = suggestion.Type switch
                {
                    RecommendationType.Optimization => 0.7, // Medium-high complexity
                    RecommendationType.Investigation => 0.3, // Low complexity
                    RecommendationType.Monitoring => 0.4,   // Low-medium complexity
                    _ => 0.5
                };
                
                complexityScores.Add(complexity);
            }
            
            return complexityScores.Any() ? complexityScores.Average() : 0.5;
        }

        private double CalculateConfidenceAlignment(double[] confidenceScores, double[] qualityScores)
        {
            if (confidenceScores.Length != qualityScores.Length || confidenceScores.Length < 2)
                return 0.5;
            
            // Calculate correlation between confidence and quality
            var meanConfidence = confidenceScores.Average();
            var meanQuality = qualityScores.Average();
            
            var numerator = confidenceScores.Zip(qualityScores, (c, q) => (c - meanConfidence) * (q - meanQuality)).Sum();
            var denomConfidence = Math.Sqrt(confidenceScores.Select(c => Math.Pow(c - meanConfidence, 2)).Sum());
            var denomQuality = Math.Sqrt(qualityScores.Select(q => Math.Pow(q - meanQuality, 2)).Sum());
            
            if (denomConfidence == 0 || denomQuality == 0) return 0.5;
            
            var correlation = numerator / (denomConfidence * denomQuality);
            return (correlation + 1) / 2; // Normalize to 0-1 range
        }

        private async Task<double> CalculateImpactRealismAsync(double[] expectedImpacts, CausalAnalysisResult causalResult)
        {
            // Check if expected impacts are realistic based on causal strengths
            var avgExpectedImpact = expectedImpacts.Average();
            var avgCausalStrength = causalResult.CausalStrengths?.Values.Average() ?? 0.5;
            
            // Realistic if expected impact aligns with causal strength
            var alignment = 1.0 - Math.Abs(avgExpectedImpact - avgCausalStrength);
            return Math.Max(0.0, alignment);
        }

        private async Task<double> CalculateCausalStrengthAlignmentAsync(List<CausalRecommendation> suggestions, CausalAnalysisResult causalResult)
        {
            if (causalResult.CausalStrengths == null) return 0.5;
            
            var alignmentScores = new List<double>();
            
            foreach (var suggestion in suggestions)
            {
                if (suggestion.RelatedRelationships?.Any() == true)
                {
                    var relatedStrengths = suggestion.RelatedRelationships
                        .Where(relId => causalResult.CausalStrengths.ContainsKey(relId))
                        .Select(relId => causalResult.CausalStrengths[relId])
                        .ToList();
                    
                    if (relatedStrengths.Any())
                    {
                        var avgStrength = relatedStrengths.Average();
                        var impactAlignment = 1.0 - Math.Abs(suggestion.ExpectedImpact - avgStrength);
                        alignmentScores.Add(Math.Max(0.0, impactAlignment));
                    }
                }
            }
            
            return alignmentScores.Any() ? alignmentScores.Average() : 0.5;
        }

        private double CalculateComplexityBalance(double[] complexityScores, double[] qualityScores)
        {
            if (complexityScores.Length != qualityScores.Length || complexityScores.Length == 0)
                return 0.5;
            
            // Good balance: high quality with reasonable complexity
            var balanceScores = complexityScores.Zip(qualityScores, (complexity, quality) =>
            {
                // Prefer high quality with low-medium complexity
                if (quality > 0.7 && complexity < 0.6) return 1.0;
                if (quality > 0.6 && complexity < 0.7) return 0.8;
                if (quality > 0.5 && complexity < 0.8) return 0.6;
                return 0.4;
            });
            
            return balanceScores.Average();
        }

        private double CalculateImpactPredictability(double[] performanceImpactScores, PerformanceAdjustmentResult performanceResult)
        {
            // Higher predictability for suggestions aligned with current performance issues
            var avgImpact = performanceImpactScores.Average();
            var currentIssuesSeverity = CalculateCurrentIssuesSeverity(performanceResult);
            
            // Good predictability if impact aligns with issue severity
            return 1.0 - Math.Abs(avgImpact - currentIssuesSeverity);
        }

        private double CalculateResourceEfficiency(double[] resourceScores, double[] performanceImpactScores)
        {
            if (resourceScores.Length != performanceImpactScores.Length || resourceScores.Length == 0)
                return 0.5;
            
            // Efficiency = high impact with low resource requirements
            var efficiencyScores = resourceScores.Zip(performanceImpactScores, (resource, impact) =>
                impact / Math.Max(1.0 - resource, 0.1)); // Higher impact, lower resource = higher efficiency
            
            return Math.Min(1.0, efficiencyScores.Average() / 2.0); // Normalize
        }

        private async Task<double> CalculatePerformanceContextAlignmentAsync(List<string> suggestions, PerformanceAdjustmentResult performanceResult)
        {
            var alignmentScore = 0.0;
            
            foreach (var suggestion in suggestions)
            {
                var suggestionLower = suggestion.ToLower();
                var suggestionScore = 0.5; // IMPROVED: Base score for any valid suggestion
                
                // IMPROVED: More generous alignment scoring
                if (performanceResult.ExecutionTimeMs > 2000 && (suggestionLower.Contains("time") || suggestionLower.Contains("performance") || suggestionLower.Contains("optimize")))
                    suggestionScore += 0.4; // IMPROVED: Lower threshold and higher score
                
                if (performanceResult.MemoryUsedMB > 50 && (suggestionLower.Contains("memory") || suggestionLower.Contains("cache") || suggestionLower.Contains("pool")))
                    suggestionScore += 0.4; // IMPROVED: Lower threshold and higher score
                
                // IMPROVED: More flexible keyword matching
                if (suggestionLower.Contains("database") || suggestionLower.Contains("query") || suggestionLower.Contains("connection"))
                    suggestionScore += 0.3;
                
                if (suggestionLower.Contains("async") || suggestionLower.Contains("parallel") || suggestionLower.Contains("concurrent"))
                    suggestionScore += 0.3;
                
                if (performanceResult.Recommendations?.Any(r => suggestion.Contains(r, StringComparison.OrdinalIgnoreCase)) == true)
                    suggestionScore += 0.2; // IMPROVED: Additional bonus, not replacement
                
                alignmentScore += Math.Min(1.0, suggestionScore);
            }
            
            return suggestions.Any() ? alignmentScore / suggestions.Count : 0.0;
        }

        private double CalculateCurrentIssuesSeverity(PerformanceAdjustmentResult performanceResult)
        {
            var severity = 0.0;
            
            // Time-based severity
            if (performanceResult.ExecutionTimeMs > 10000) severity += 0.4;
            else if (performanceResult.ExecutionTimeMs > 5000) severity += 0.2;
            else if (performanceResult.ExecutionTimeMs > 2000) severity += 0.1;
            
            // Memory-based severity
            if (performanceResult.MemoryUsedMB > 500) severity += 0.3;
            else if (performanceResult.MemoryUsedMB > 200) severity += 0.2;
            else if (performanceResult.MemoryUsedMB > 100) severity += 0.1;
            
            // Success/failure impact
            if (!performanceResult.Success) severity += 0.3;
            
            return Math.Min(1.0, severity);
        }

        // Text analysis helper methods
        private bool ContainsSpecificAction(string action)
        {
            var actionKeywords = new[] { "implement", "configure", "optimize", "refactor", "update", "install", "enable", "disable", "modify" };
            return actionKeywords.Any(keyword => action.ToLower().Contains(keyword));
        }

        private double CalculateDescriptionActionability(string description)
        {
            var actionableWords = new[] { "should", "must", "implement", "configure", "optimize", "add", "remove", "update", "modify" };
            var wordCount = description.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            var actionableWordCount = actionableWords.Count(word => description.ToLower().Contains(word));
            
            return wordCount > 0 ? Math.Min(0.3, (double)actionableWordCount / wordCount * 2) : 0.0;
        }

        private double CalculateDescriptionSpecificity(string description)
        {
            var specificWords = new[] { "specific", "exactly", "precisely", "particular", "detailed", "explicit" };
            var technicalWords = new[] { "algorithm", "method", "function", "class", "module", "component", "service" };
            var quantifiableWords = new[] { "increase", "decrease", "reduce", "improve", "percent", "%", "times", "factor" };
            
            var allSpecificWords = specificWords.Concat(technicalWords).Concat(quantifiableWords);
            var wordCount = description.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            var specificWordCount = allSpecificWords.Count(word => description.ToLower().Contains(word));
            
            return wordCount > 0 ? Math.Min(0.5, (double)specificWordCount / wordCount * 3) : 0.0;
        }

        private bool ContainsQuantifiableElements(string text)
        {
            var quantifiablePatterns = new[] { "%", "percent", "times", "factor", "increase", "decrease", "improve", "reduce" };
            return quantifiablePatterns.Any(pattern => text.ToLower().Contains(pattern));
        }

        private bool ContainsPerformanceMetrics(string suggestion)
        {
            var metrics = new[] { "latency", "throughput", "response time", "cpu", "memory", "disk", "network", "bandwidth" };
            return metrics.Any(metric => suggestion.ToLower().Contains(metric));
        }

        private bool ContainsTechnicalBestPractices(string suggestion)
        {
            var practices = new[] { "cache", "index", "optimize", "parallel", "async", "batch", "pool", "lazy loading" };
            return practices.Any(practice => suggestion.ToLower().Contains(practice));
        }

        private bool ContainsSimpleOptimizations(string suggestion)
        {
            var simple = new[] { "cache", "index", "configuration", "setting", "parameter", "flag" };
            return simple.Any(s => suggestion.ToLower().Contains(s));
        }

        private bool ContainsArchitecturalChanges(string suggestion)
        {
            var architectural = new[] { "refactor", "redesign", "architecture", "pattern", "structure", "framework" };
            return architectural.Any(a => suggestion.ToLower().Contains(a));
        }

        private bool ContainsConfigurationChanges(string suggestion)
        {
            var config = new[] { "config", "setting", "parameter", "property", "option", "flag" };
            return config.Any(c => suggestion.ToLower().Contains(c));
        }

        private bool ContainsHighImpactOptimizations(string suggestion)
        {
            var highImpact = new[] { "parallel", "async", "cache", "index", "algorithm", "data structure" };
            return highImpact.Any(h => suggestion.ToLower().Contains(h));
        }

        private bool ContainsMediumImpactOptimizations(string suggestion)
        {
            var mediumImpact = new[] { "batch", "pool", "lazy", "optimize", "efficient" };
            return mediumImpact.Any(m => suggestion.ToLower().Contains(m));
        }

        private bool ContainsLowResourceChanges(string suggestion)
        {
            var lowResource = new[] { "config", "setting", "parameter", "flag", "enable", "disable" };
            return lowResource.Any(l => suggestion.ToLower().Contains(l));
        }

        private bool ContainsHighResourceChanges(string suggestion)
        {
            var highResource = new[] { "refactor", "redesign", "rewrite", "migrate", "upgrade", "new system" };
            return highResource.Any(h => suggestion.ToLower().Contains(h));
        }

        #endregion
    }

    /// <summary>
    /// System complexity levels for feasibility assessment
    /// </summary>
    public enum SystemComplexity
    {
        Low,
        Medium,
        High,
        VeryHigh
    }
}
