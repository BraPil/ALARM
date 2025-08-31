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
    /// Comprehensive suggestion validation engine that provides quality assessment
    /// and feedback loops for all ALARM analysis components
    /// </summary>
    public class SuggestionValidationEngine
    {
        private readonly ILogger<SuggestionValidationEngine> _logger;
        private readonly MLContext _mlContext;
        private readonly QualityMetricsCalculator _qualityMetrics;
        private readonly ValidationModelManager _validationModels;
        private readonly FeedbackIntegrationService _feedbackIntegration;
        private readonly RecommendationImprovementEngine _improvementEngine;
        private readonly SuggestionValidationConfig _config;
        private readonly MLFlowExperimentTracker _mlflowTracker;

        public SuggestionValidationEngine(ILogger<SuggestionValidationEngine> logger, SuggestionValidationConfig? config = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mlContext = new MLContext(seed: 42);
            _config = config ?? new SuggestionValidationConfig();
            
            _qualityMetrics = new QualityMetricsCalculator(_mlContext, _logger);
            _validationModels = new ValidationModelManager(_mlContext, _logger);
            _feedbackIntegration = new FeedbackIntegrationService(_logger);
            _improvementEngine = new RecommendationImprovementEngine(_mlContext, _logger);
            
            // Initialize MLFlow tracking
            var mlflowLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => 
                builder.AddConsole().SetMinimumLevel(LogLevel.Information))
                .CreateLogger<MLFlowExperimentTracker>();
            _mlflowTracker = new MLFlowExperimentTracker(mlflowLogger);
        }

        /// <summary>
        /// Validate suggestions from pattern detection analysis
        /// </summary>
        public async Task<SuggestionValidationResult> ValidatePatternSuggestionsAsync(
            AdvancedPatternResult patternResult,
            IEnumerable<PatternRecommendation> suggestions,
            ValidationContext context)
        {
            _logger.LogInformation("Validating {SuggestionCount} pattern detection suggestions", suggestions.Count());

            // Start MLFlow tracking for this validation
            var runId = await _mlflowTracker.StartRunAsync($"PatternValidation_{DateTime.UtcNow:yyyyMMdd_HHmmss}", 
                new Dictionary<string, string>
                {
                    ["analysis_type"] = "PatternDetection",
                    ["suggestion_count"] = suggestions.Count().ToString(),
                    ["validation_phase"] = "Phase1_Improvements"
                });

            var validationResult = new SuggestionValidationResult
            {
                AnalysisType = AnalysisType.PatternDetection,
                ValidationTimestamp = DateTime.UtcNow,
                SourceAnalysisId = patternResult.AnalysisId ?? Guid.NewGuid().ToString(),
                SuggestionValidations = new List<IndividualSuggestionValidation>(),
                OverallQualityScore = 0.0,
                QualityMetrics = new Dictionary<string, double>(),
                ImprovementRecommendations = new List<string>(),
                ValidationContext = context
            };

            // Validate each suggestion individually
            foreach (var suggestion in suggestions)
            {
                var individualValidation = await ValidateIndividualSuggestionAsync(suggestion, patternResult, context);
                validationResult.SuggestionValidations.Add(individualValidation);
            }

            // Calculate overall quality metrics
            validationResult.QualityMetrics = await _qualityMetrics.CalculatePatternSuggestionQualityAsync(
                suggestions, patternResult, validationResult.SuggestionValidations);

            // Calculate overall quality score
            validationResult.OverallQualityScore = validationResult.QualityMetrics.Values.Average();

            // Generate improvement recommendations
            validationResult.ImprovementRecommendations = await _improvementEngine
                .GeneratePatternSuggestionImprovementsAsync(validationResult);

            // Integrate with feedback system
            await _feedbackIntegration.RecordValidationResultAsync(validationResult);

            // Log quality metrics to MLFlow
            await _mlflowTracker.LogQualityMetricsAsync(validationResult.QualityMetrics);
            await _mlflowTracker.LogQualityComparisonAsync("PatternDetection", 0.6913, validationResult.OverallQualityScore, 
                new Dictionary<string, object>
                {
                    ["suggestions_validated"] = suggestions.Count(),
                    ["improvement_recommendations"] = validationResult.ImprovementRecommendations.Count,
                    ["validation_timestamp"] = validationResult.ValidationTimestamp
                });

            // End MLFlow run
            await _mlflowTracker.EndRunAsync();

            _logger.LogInformation("Pattern suggestion validation completed with {QualityScore:P2} overall quality", 
                validationResult.OverallQualityScore);

            return validationResult;
        }

        /// <summary>
        /// Validate suggestions from causal analysis
        /// </summary>
        public async Task<SuggestionValidationResult> ValidateCausalSuggestionsAsync(
            CausalAnalysisResult causalResult,
            IEnumerable<CausalRecommendation> suggestions,
            ValidationContext context)
        {
            _logger.LogInformation("Validating {SuggestionCount} causal analysis suggestions", suggestions.Count());

            var validationResult = new SuggestionValidationResult
            {
                AnalysisType = AnalysisType.CausalAnalysis,
                ValidationTimestamp = DateTime.UtcNow,
                SourceAnalysisId = causalResult.AnalysisId ?? Guid.NewGuid().ToString(),
                SuggestionValidations = new List<IndividualSuggestionValidation>(),
                OverallQualityScore = 0.0,
                QualityMetrics = new Dictionary<string, double>(),
                ImprovementRecommendations = new List<string>(),
                ValidationContext = context
            };

            // Validate each causal suggestion
            foreach (var suggestion in suggestions)
            {
                var individualValidation = await ValidateCausalSuggestionAsync(suggestion, causalResult, context);
                validationResult.SuggestionValidations.Add(individualValidation);
            }

            // Calculate causal-specific quality metrics
            validationResult.QualityMetrics = await _qualityMetrics.CalculateCausalSuggestionQualityAsync(
                suggestions, causalResult, validationResult.SuggestionValidations);

            validationResult.OverallQualityScore = validationResult.QualityMetrics.Values.Average();

            // Generate causal-specific improvements
            validationResult.ImprovementRecommendations = await _improvementEngine
                .GenerateCausalSuggestionImprovementsAsync(validationResult);

            await _feedbackIntegration.RecordValidationResultAsync(validationResult);

            _logger.LogInformation("Causal suggestion validation completed with {QualityScore:P2} overall quality", 
                validationResult.OverallQualityScore);

            return validationResult;
        }

        /// <summary>
        /// Validate performance optimization suggestions
        /// </summary>
        public async Task<SuggestionValidationResult> ValidatePerformanceSuggestionsAsync(
            PerformanceAdjustmentResult performanceResult,
            IEnumerable<string> suggestions,
            ValidationContext context)
        {
            _logger.LogInformation("Validating {SuggestionCount} performance suggestions", suggestions.Count());

            var validationResult = new SuggestionValidationResult
            {
                AnalysisType = AnalysisType.PerformanceOptimization,
                ValidationTimestamp = DateTime.UtcNow,
                SourceAnalysisId = performanceResult.OperationName,
                SuggestionValidations = new List<IndividualSuggestionValidation>(),
                OverallQualityScore = 0.0,
                QualityMetrics = new Dictionary<string, double>(),
                ImprovementRecommendations = new List<string>(),
                ValidationContext = context
            };

            // Validate each performance suggestion
            foreach (var suggestion in suggestions)
            {
                var individualValidation = await ValidatePerformanceSuggestionAsync(suggestion, performanceResult, context);
                validationResult.SuggestionValidations.Add(individualValidation);
            }

            // Calculate performance-specific quality metrics
            validationResult.QualityMetrics = await _qualityMetrics.CalculatePerformanceSuggestionQualityAsync(
                suggestions, performanceResult, validationResult.SuggestionValidations);

            validationResult.OverallQualityScore = validationResult.QualityMetrics.Values.Average();

            validationResult.ImprovementRecommendations = await _improvementEngine
                .GeneratePerformanceSuggestionImprovementsAsync(validationResult);

            await _feedbackIntegration.RecordValidationResultAsync(validationResult);

            _logger.LogInformation("Performance suggestion validation completed with {QualityScore:P2} overall quality", 
                validationResult.OverallQualityScore);

            return validationResult;
        }

        /// <summary>
        /// Validate suggestions across multiple analysis types
        /// </summary>
        public async Task<ComprehensiveSuggestionValidationResult> ValidateComprehensiveSuggestionsAsync(
            ComprehensiveAnalysisResult analysisResult,
            ValidationContext context)
        {
            _logger.LogInformation("Starting comprehensive suggestion validation across multiple analysis types");

            var comprehensiveResult = new ComprehensiveSuggestionValidationResult
            {
                ValidationTimestamp = DateTime.UtcNow,
                ValidationResults = new Dictionary<AnalysisType, SuggestionValidationResult>(),
                CrossAnalysisConsistency = new Dictionary<string, double>(),
                OverallSystemQuality = 0.0,
                SystemWideImprovements = new List<string>(),
                ValidationContext = context
            };

            // Validate pattern detection suggestions if present
            if (analysisResult.PatternResult is AdvancedPatternResult patternResult && 
                analysisResult.PatternSuggestions is IEnumerable<PatternRecommendation> patternSuggestions && 
                patternSuggestions.Any())
            {
                var patternValidation = await ValidatePatternSuggestionsAsync(
                    patternResult, patternSuggestions, context);
                comprehensiveResult.ValidationResults[AnalysisType.PatternDetection] = patternValidation;
            }

            // Validate causal analysis suggestions if present
            if (analysisResult.CausalResult is CausalAnalysisResult causalResult && 
                analysisResult.CausalSuggestions is IEnumerable<CausalRecommendation> causalSuggestions && 
                causalSuggestions.Any())
            {
                var causalValidation = await ValidateCausalSuggestionsAsync(
                    causalResult, causalSuggestions, context);
                comprehensiveResult.ValidationResults[AnalysisType.CausalAnalysis] = causalValidation;
            }

            // Validate performance suggestions if present
            if (analysisResult.PerformanceResult is PerformanceAdjustmentResult performanceResult && 
                analysisResult.PerformanceSuggestions?.Any() == true)
            {
                var performanceValidation = await ValidatePerformanceSuggestionsAsync(
                    performanceResult, analysisResult.PerformanceSuggestions, context);
                comprehensiveResult.ValidationResults[AnalysisType.PerformanceOptimization] = performanceValidation;
            }

            // Analyze cross-analysis consistency
            comprehensiveResult.CrossAnalysisConsistency = await AnalyzeCrossAnalysisConsistencyAsync(
                comprehensiveResult.ValidationResults);

            // Calculate overall system quality
            comprehensiveResult.OverallSystemQuality = CalculateOverallSystemQuality(comprehensiveResult);

            // Generate system-wide improvement recommendations
            comprehensiveResult.SystemWideImprovements = await _improvementEngine
                .GenerateSystemWideImprovementsAsync(comprehensiveResult);

            // Record comprehensive validation
            await _feedbackIntegration.RecordComprehensiveValidationAsync(comprehensiveResult);

            _logger.LogInformation("Comprehensive suggestion validation completed with {SystemQuality:P2} overall system quality", 
                comprehensiveResult.OverallSystemQuality);

            return comprehensiveResult;
        }

        /// <summary>
        /// Get historical validation trends and insights
        /// </summary>
        public async Task<ValidationTrendsResult> GetValidationTrendsAsync(
            TimeSpan timeWindow, 
            AnalysisType? analysisType = null)
        {
            _logger.LogInformation("Analyzing validation trends over {TimeWindow} for {AnalysisType}", 
                timeWindow, analysisType?.ToString() ?? "all analysis types");

            var trends = await _feedbackIntegration.GetValidationTrendsAsync(timeWindow, analysisType);
            var insights = await _improvementEngine.GenerateTrendInsightsAsync(trends);

            return new ValidationTrendsResult
            {
                TimeWindow = timeWindow,
                AnalysisType = analysisType,
                QualityTrends = trends.QualityTrends,
                VolumeStats = trends.VolumeStats,
                TopIssues = trends.TopIssues,
                ImprovementOpportunities = insights.ImprovementOpportunities,
                SuccessPatterns = insights.SuccessPatterns,
                Recommendations = insights.Recommendations
            };
        }

        #region Private Helper Methods

        /// <summary>
        /// Validate individual suggestion from pattern detection
        /// </summary>
        private async Task<IndividualSuggestionValidation> ValidateIndividualSuggestionAsync(
            PatternRecommendation suggestion,
            AdvancedPatternResult patternResult,
            ValidationContext context)
        {
            var validation = new IndividualSuggestionValidation
            {
                SuggestionId = suggestion.Id ?? Guid.NewGuid().ToString(),
                SuggestionText = suggestion.Description ?? string.Empty,
                QualityScores = new Dictionary<string, double>(),
                ValidationDetails = new Dictionary<string, object>(),
                Issues = new List<ValidationIssue>(),
                Improvements = new List<string>()
            };

            // Relevance assessment
            validation.QualityScores["Relevance"] = await _qualityMetrics.AssessRelevanceAsync(suggestion, patternResult);

            // Actionability assessment
            validation.QualityScores["Actionability"] = await _qualityMetrics.AssessActionabilityAsync(suggestion);

            // Specificity assessment
            validation.QualityScores["Specificity"] = await _qualityMetrics.AssessSpecificityAsync(suggestion);

            // Feasibility assessment
            validation.QualityScores["Feasibility"] = await _qualityMetrics.AssessFeasibilityAsync(suggestion, context);

            // Impact potential assessment
            validation.QualityScores["ImpactPotential"] = await _qualityMetrics.AssessImpactPotentialAsync(suggestion, patternResult);

            // Overall score
            validation.OverallScore = validation.QualityScores.Values.Average();

            // Identify issues
            validation.Issues = await IdentifySuggestionIssuesAsync(suggestion, validation.QualityScores);

            // Generate improvements
            validation.Improvements = await GenerateSuggestionImprovementsAsync(suggestion, validation.Issues);

            return validation;
        }

        /// <summary>
        /// Validate individual causal suggestion
        /// </summary>
        private async Task<IndividualSuggestionValidation> ValidateCausalSuggestionAsync(
            CausalRecommendation suggestion,
            CausalAnalysisResult causalResult,
            ValidationContext context)
        {
            var validation = new IndividualSuggestionValidation
            {
                SuggestionId = suggestion.Id ?? Guid.NewGuid().ToString(),
                SuggestionText = suggestion.Description ?? string.Empty,
                QualityScores = new Dictionary<string, double>(),
                ValidationDetails = new Dictionary<string, object>(),
                Issues = new List<ValidationIssue>(),
                Improvements = new List<string>()
            };

            // Causal validity assessment
            validation.QualityScores["CausalValidity"] = await _qualityMetrics.AssessCausalValidityAsync(suggestion, causalResult);

            // Evidence strength assessment
            validation.QualityScores["EvidenceStrength"] = await _qualityMetrics.AssessEvidenceStrengthAsync(suggestion, causalResult);

            // Practical applicability
            validation.QualityScores["PracticalApplicability"] = await _qualityMetrics.AssessPracticalApplicabilityAsync(suggestion, context);

            // Risk assessment
            validation.QualityScores["RiskLevel"] = await _qualityMetrics.AssessRiskLevelAsync(suggestion, causalResult);

            // Expected impact
            validation.QualityScores["ExpectedImpact"] = suggestion.ExpectedImpact;

            validation.OverallScore = validation.QualityScores.Values.Average();
            validation.Issues = await IdentifyCausalSuggestionIssuesAsync(suggestion, validation.QualityScores);
            validation.Improvements = await GenerateCausalSuggestionImprovementsAsync(suggestion, validation.Issues);

            return validation;
        }

        /// <summary>
        /// Validate individual performance suggestion
        /// </summary>
        private async Task<IndividualSuggestionValidation> ValidatePerformanceSuggestionAsync(
            string suggestion,
            PerformanceAdjustmentResult performanceResult,
            ValidationContext context)
        {
            var validation = new IndividualSuggestionValidation
            {
                SuggestionId = Guid.NewGuid().ToString(),
                SuggestionText = suggestion,
                QualityScores = new Dictionary<string, double>(),
                ValidationDetails = new Dictionary<string, object>(),
                Issues = new List<ValidationIssue>(),
                Improvements = new List<string>()
            };

            // Technical accuracy
            validation.QualityScores["TechnicalAccuracy"] = await _qualityMetrics.AssessTechnicalAccuracyAsync(suggestion, performanceResult);

            // Implementation complexity
            validation.QualityScores["ImplementationComplexity"] = await _qualityMetrics.AssessImplementationComplexityAsync(suggestion);

            // Performance impact potential
            validation.QualityScores["PerformanceImpact"] = await _qualityMetrics.AssessPerformanceImpactAsync(suggestion, performanceResult);

            // Resource requirements
            validation.QualityScores["ResourceRequirements"] = await _qualityMetrics.AssessResourceRequirementsAsync(suggestion, context);

            validation.OverallScore = validation.QualityScores.Values.Average();
            validation.Issues = await IdentifyPerformanceSuggestionIssuesAsync(suggestion, validation.QualityScores);
            validation.Improvements = await GeneratePerformanceSuggestionImprovementsAsync(suggestion, validation.Issues);

            return validation;
        }

        /// <summary>
        /// Analyze consistency across different analysis types
        /// </summary>
        private async Task<Dictionary<string, double>> AnalyzeCrossAnalysisConsistencyAsync(
            Dictionary<AnalysisType, SuggestionValidationResult> validationResults)
        {
            var consistency = new Dictionary<string, double>();

            if (validationResults.Count < 2)
            {
                consistency["InsufficientData"] = 1.0;
                return consistency;
            }

            // Quality score consistency
            var qualityScores = validationResults.Values.Select(v => v.OverallQualityScore).ToArray();
            consistency["QualityScoreConsistency"] = 1.0 - CalculateVariationCoefficient(qualityScores);

            // Suggestion theme consistency
            consistency["ThemeConsistency"] = await AnalyzeSuggestionThemeConsistencyAsync(validationResults);

            // Priority alignment
            consistency["PriorityAlignment"] = await AnalyzePriorityAlignmentAsync(validationResults);

            return consistency;
        }

        /// <summary>
        /// Calculate overall system quality score
        /// </summary>
        private double CalculateOverallSystemQuality(ComprehensiveSuggestionValidationResult result)
        {
            var scores = new List<double>();

            // Individual analysis quality scores
            foreach (var validationResult in result.ValidationResults.Values)
            {
                scores.Add(validationResult.OverallQualityScore);
            }

            // Cross-analysis consistency scores
            foreach (var consistencyScore in result.CrossAnalysisConsistency.Values)
            {
                scores.Add(consistencyScore);
            }

            return scores.Any() ? scores.Average() : 0.0;
        }

        /// <summary>
        /// Calculate variation coefficient for consistency analysis
        /// </summary>
        private double CalculateVariationCoefficient(double[] values)
        {
            if (values.Length < 2) return 0.0;

            var mean = values.Average();
            if (Math.Abs(mean) < 1e-10) return 0.0;

            var variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);

            return standardDeviation / mean;
        }

        /// <summary>
        /// Analyze suggestion theme consistency across analysis types
        /// </summary>
        private async Task<double> AnalyzeSuggestionThemeConsistencyAsync(
            Dictionary<AnalysisType, SuggestionValidationResult> validationResults)
        {
            // Extract themes from all suggestions
            var allThemes = new List<string>();
            
            foreach (var result in validationResults.Values)
            {
                foreach (var suggestion in result.SuggestionValidations)
                {
                    var themes = await ExtractSuggestionThemesAsync(suggestion.SuggestionText);
                    allThemes.AddRange(themes);
                }
            }

            // Calculate theme overlap
            var uniqueThemes = allThemes.Distinct().ToList();
            var themeFrequency = allThemes.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
            
            // Higher consistency when themes appear across multiple analysis types
            var consistentThemes = themeFrequency.Where(kvp => kvp.Value > 1).Count();
            
            return uniqueThemes.Any() ? (double)consistentThemes / uniqueThemes.Count : 1.0;
        }

        /// <summary>
        /// Analyze priority alignment across different analysis types
        /// </summary>
        private async Task<double> AnalyzePriorityAlignmentAsync(
            Dictionary<AnalysisType, SuggestionValidationResult> validationResults)
        {
            var highPriorityThemes = new List<string>();
            
            foreach (var result in validationResults.Values)
            {
                var highQualitySuggestions = result.SuggestionValidations
                    .Where(s => s.OverallScore > 0.7)
                    .ToList();
                
                foreach (var suggestion in highQualitySuggestions)
                {
                    var themes = await ExtractSuggestionThemesAsync(suggestion.SuggestionText);
                    highPriorityThemes.AddRange(themes);
                }
            }

            // Calculate alignment based on theme overlap in high-priority suggestions
            var themeFrequency = highPriorityThemes.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
            var alignedThemes = themeFrequency.Where(kvp => kvp.Value > 1).Count();
            var totalThemes = themeFrequency.Count;

            return totalThemes > 0 ? (double)alignedThemes / totalThemes : 1.0;
        }

        /// <summary>
        /// Extract themes from suggestion text using simple keyword analysis
        /// </summary>
        private async Task<List<string>> ExtractSuggestionThemesAsync(string suggestionText)
        {
            var themes = new List<string>();
            var text = suggestionText.ToLower();

            // Performance themes
            if (text.Contains("performance") || text.Contains("optimize") || text.Contains("speed") || text.Contains("memory"))
                themes.Add("Performance");

            // Security themes
            if (text.Contains("security") || text.Contains("vulnerability") || text.Contains("risk"))
                themes.Add("Security");

            // Maintenance themes
            if (text.Contains("maintain") || text.Contains("refactor") || text.Contains("clean") || text.Contains("documentation"))
                themes.Add("Maintenance");

            // Architecture themes
            if (text.Contains("architecture") || text.Contains("design") || text.Contains("pattern") || text.Contains("structure"))
                themes.Add("Architecture");

            // Data themes
            if (text.Contains("data") || text.Contains("database") || text.Contains("storage"))
                themes.Add("Data");

            // Integration themes
            if (text.Contains("integration") || text.Contains("api") || text.Contains("interface"))
                themes.Add("Integration");

            return themes;
        }

        /// <summary>
        /// Identify issues with individual suggestions
        /// </summary>
        private async Task<List<ValidationIssue>> IdentifySuggestionIssuesAsync(
            PatternRecommendation suggestion,
            Dictionary<string, double> qualityScores)
        {
            var issues = new List<ValidationIssue>();

            foreach (var score in qualityScores)
            {
                if (score.Value < _config.QualityThresholds.MinAcceptableScore)
                {
                    issues.Add(new ValidationIssue
                    {
                        IssueType = score.Key,
                        Severity = score.Value < _config.QualityThresholds.CriticalThreshold ? 
                            ValidationIssueSeverity.Critical : ValidationIssueSeverity.Warning,
                        Description = $"Low {score.Key} score: {score.Value:P2}",
                        SuggestedFix = GenerateQualityImprovementSuggestion(score.Key, score.Value)
                    });
                }
            }

            return issues;
        }

        /// <summary>
        /// Generate improvement suggestions for individual suggestions
        /// </summary>
        private async Task<List<string>> GenerateSuggestionImprovementsAsync(
            PatternRecommendation suggestion,
            List<ValidationIssue> issues)
        {
            var improvements = new List<string>();

            foreach (var issue in issues)
            {
                improvements.Add(issue.SuggestedFix);
            }

            // Add general improvements
            if (!improvements.Any())
            {
                improvements.Add("Consider adding more specific implementation details");
                improvements.Add("Include estimated effort and timeline information");
                improvements.Add("Provide clear success criteria");
            }

            return improvements;
        }

        /// <summary>
        /// Identify issues with causal suggestions
        /// </summary>
        private async Task<List<ValidationIssue>> IdentifyCausalSuggestionIssuesAsync(
            CausalRecommendation suggestion,
            Dictionary<string, double> qualityScores)
        {
            var issues = new List<ValidationIssue>();

            foreach (var score in qualityScores)
            {
                if (score.Value < _config.QualityThresholds.MinAcceptableScore)
                {
                    issues.Add(new ValidationIssue
                    {
                        IssueType = score.Key,
                        Severity = score.Value < _config.QualityThresholds.CriticalThreshold ? 
                            ValidationIssueSeverity.Critical : ValidationIssueSeverity.Warning,
                        Description = $"Causal suggestion has low {score.Key}: {score.Value:P2}",
                        SuggestedFix = GenerateCausalImprovementSuggestion(score.Key, score.Value)
                    });
                }
            }

            return issues;
        }

        /// <summary>
        /// Generate improvements for causal suggestions
        /// </summary>
        private async Task<List<string>> GenerateCausalSuggestionImprovementsAsync(
            CausalRecommendation suggestion,
            List<ValidationIssue> issues)
        {
            var improvements = new List<string>();

            foreach (var issue in issues)
            {
                improvements.Add(issue.SuggestedFix);
            }

            if (!improvements.Any())
            {
                improvements.Add("Strengthen causal evidence with additional validation");
                improvements.Add("Consider potential confounding factors");
                improvements.Add("Provide intervention testing recommendations");
            }

            return improvements;
        }

        /// <summary>
        /// Identify issues with performance suggestions
        /// </summary>
        private async Task<List<ValidationIssue>> IdentifyPerformanceSuggestionIssuesAsync(
            string suggestion,
            Dictionary<string, double> qualityScores)
        {
            var issues = new List<ValidationIssue>();

            foreach (var score in qualityScores)
            {
                if (score.Value < _config.QualityThresholds.MinAcceptableScore)
                {
                    issues.Add(new ValidationIssue
                    {
                        IssueType = score.Key,
                        Severity = score.Value < _config.QualityThresholds.CriticalThreshold ? 
                            ValidationIssueSeverity.Critical : ValidationIssueSeverity.Warning,
                        Description = $"Performance suggestion has low {score.Key}: {score.Value:P2}",
                        SuggestedFix = GeneratePerformanceImprovementSuggestion(score.Key, score.Value)
                    });
                }
            }

            return issues;
        }

        /// <summary>
        /// Generate improvements for performance suggestions
        /// </summary>
        private async Task<List<string>> GeneratePerformanceSuggestionImprovementsAsync(
            string suggestion,
            List<ValidationIssue> issues)
        {
            var improvements = new List<string>();

            foreach (var issue in issues)
            {
                improvements.Add(issue.SuggestedFix);
            }

            if (!improvements.Any())
            {
                improvements.Add("Include specific performance metrics and targets");
                improvements.Add("Provide implementation difficulty assessment");
                improvements.Add("Consider resource and time requirements");
            }

            return improvements;
        }

        /// <summary>
        /// Generate quality improvement suggestion based on issue type
        /// </summary>
        private string GenerateQualityImprovementSuggestion(string issueType, double score)
        {
            return issueType.ToLower() switch
            {
                "relevance" => "Make the suggestion more directly related to the identified patterns",
                "actionability" => "Provide more specific, actionable steps for implementation",
                "specificity" => "Include more detailed and specific implementation guidance",
                "feasibility" => "Consider practical constraints and provide realistic alternatives",
                "impactpotential" => "Better quantify the expected impact and benefits",
                _ => "Improve overall suggestion quality and clarity"
            };
        }

        /// <summary>
        /// Generate causal improvement suggestion
        /// </summary>
        private string GenerateCausalImprovementSuggestion(string issueType, double score)
        {
            return issueType.ToLower() switch
            {
                "causalvalidity" => "Strengthen causal reasoning with additional evidence",
                "evidencestrength" => "Provide more robust statistical evidence for causal claims",
                "practicalapplicability" => "Make the suggestion more practically applicable",
                "risklevel" => "Better assess and communicate potential risks",
                "expectedimpact" => "Provide more accurate impact estimates",
                _ => "Improve overall causal suggestion quality"
            };
        }

        /// <summary>
        /// Generate performance improvement suggestion
        /// </summary>
        private string GeneratePerformanceImprovementSuggestion(string issueType, double score)
        {
            return issueType.ToLower() switch
            {
                "technicalaccuracy" => "Ensure technical recommendations are accurate and up-to-date",
                "implementationcomplexity" => "Provide clearer guidance on implementation complexity",
                "performanceimpact" => "Better quantify expected performance improvements",
                "resourcerequirements" => "Clearly specify resource requirements and constraints",
                _ => "Improve overall performance suggestion quality"
            };
        }

        #endregion
    }
}
