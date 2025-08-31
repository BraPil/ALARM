using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ML;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Engine for generating improvements to suggestions based on validation feedback
    /// </summary>
    public class RecommendationImprovementEngine
    {
        private readonly MLContext _mlContext;
        private readonly ILogger _logger;
        private readonly Dictionary<string, List<string>> _improvementTemplates;

        public RecommendationImprovementEngine(MLContext mlContext, ILogger logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _improvementTemplates = InitializeImprovementTemplates();
        }

        /// <summary>
        /// Generate improvements for pattern detection suggestions
        /// </summary>
        public async Task<List<string>> GeneratePatternSuggestionImprovementsAsync(SuggestionValidationResult validationResult)
        {
            _logger.LogDebug("Generating pattern suggestion improvements for validation {ValidationId}", 
                validationResult.ValidationId);

            var improvements = new List<string>();

            // Analyze overall quality issues
            if (validationResult.OverallQualityScore < 0.6)
            {
                improvements.Add("📊 Overall suggestion quality is below threshold - consider comprehensive revision");
            }

            // Analyze specific quality metrics
            if (validationResult.QualityMetrics.TryGetValue("AverageRelevance", out var relevance) && relevance < 0.7)
            {
                improvements.AddRange(_improvementTemplates["LowRelevance"]);
            }

            if (validationResult.QualityMetrics.TryGetValue("AverageActionability", out var actionability) && actionability < 0.7)
            {
                improvements.AddRange(_improvementTemplates["LowActionability"]);
            }

            if (validationResult.QualityMetrics.TryGetValue("SuggestionDiversity", out var diversity) && diversity < 0.5)
            {
                improvements.Add("🎯 Increase suggestion diversity by addressing different pattern types and priorities");
            }

            // Analyze individual suggestion issues
            var commonIssues = AnalyzeCommonIssues(validationResult.SuggestionValidations);
            foreach (var issue in commonIssues.Take(3)) // Top 3 most common issues
            {
                if (_improvementTemplates.ContainsKey(issue.Key))
                {
                    improvements.Add($"🔧 Common issue ({issue.Value} occurrences): {_improvementTemplates[issue.Key].First()}");
                }
            }

            // Generate adaptive improvements based on patterns
            var adaptiveImprovements = await GenerateAdaptiveImprovementsAsync(validationResult, AnalysisType.PatternDetection);
            improvements.AddRange(adaptiveImprovements);

            return improvements.Distinct().Take(10).ToList(); // Limit to top 10 unique improvements
        }

        /// <summary>
        /// Generate improvements for causal analysis suggestions
        /// </summary>
        public async Task<List<string>> GenerateCausalSuggestionImprovementsAsync(SuggestionValidationResult validationResult)
        {
            _logger.LogDebug("Generating causal suggestion improvements for validation {ValidationId}", 
                validationResult.ValidationId);

            var improvements = new List<string>();

            // Causal-specific quality analysis
            if (validationResult.QualityMetrics.TryGetValue("AverageCausalValidity", out var validity) && validity < 0.7)
            {
                improvements.AddRange(_improvementTemplates["LowCausalValidity"]);
            }

            if (validationResult.QualityMetrics.TryGetValue("AverageEvidenceStrength", out var evidence) && evidence < 0.7)
            {
                improvements.AddRange(_improvementTemplates["WeakEvidence"]);
            }

            if (validationResult.QualityMetrics.TryGetValue("AverageRiskAssessment", out var risk) && risk < 0.6)
            {
                improvements.Add("⚠️ Improve risk assessment by considering potential negative consequences and mitigation strategies");
            }

            // Impact realism check
            if (validationResult.QualityMetrics.TryGetValue("ImpactRealism", out var realism) && realism < 0.6)
            {
                improvements.Add("📈 Align expected impact estimates with actual causal strength evidence");
            }

            var adaptiveImprovements = await GenerateAdaptiveImprovementsAsync(validationResult, AnalysisType.CausalAnalysis);
            improvements.AddRange(adaptiveImprovements);

            return improvements.Distinct().Take(10).ToList();
        }

        /// <summary>
        /// Generate improvements for performance suggestions
        /// </summary>
        public async Task<List<string>> GeneratePerformanceSuggestionImprovementsAsync(SuggestionValidationResult validationResult)
        {
            _logger.LogDebug("Generating performance suggestion improvements for validation {ValidationId}", 
                validationResult.ValidationId);

            var improvements = new List<string>();

            // Performance-specific quality analysis
            if (validationResult.QualityMetrics.TryGetValue("AverageTechnicalAccuracy", out var accuracy) && accuracy < 0.7)
            {
                improvements.AddRange(_improvementTemplates["LowTechnicalAccuracy"]);
            }

            if (validationResult.QualityMetrics.TryGetValue("ComplexityBalance", out var balance) && balance < 0.6)
            {
                improvements.Add("⚖️ Better balance implementation complexity with expected performance gains");
            }

            if (validationResult.QualityMetrics.TryGetValue("ResourceEfficiency", out var efficiency) && efficiency < 0.6)
            {
                improvements.Add("💡 Focus on high-impact optimizations that require minimal resources");
            }

            if (validationResult.QualityMetrics.TryGetValue("PerformanceContextAlignment", out var alignment) && alignment < 0.6)
            {
                improvements.Add("🎯 Ensure suggestions directly address the identified performance bottlenecks");
            }

            var adaptiveImprovements = await GenerateAdaptiveImprovementsAsync(validationResult, AnalysisType.PerformanceOptimization);
            improvements.AddRange(adaptiveImprovements);

            return improvements.Distinct().Take(10).ToList();
        }

        /// <summary>
        /// Generate system-wide improvements for comprehensive validation
        /// </summary>
        public async Task<List<string>> GenerateSystemWideImprovementsAsync(ComprehensiveSuggestionValidationResult comprehensiveResult)
        {
            _logger.LogInformation("Generating system-wide improvements for comprehensive validation {ValidationId}", 
                comprehensiveResult.ValidationId);

            var improvements = new List<string>();

            // Overall system quality assessment
            if (comprehensiveResult.OverallSystemQuality < 0.7)
            {
                improvements.Add("🏗️ Overall system suggestion quality needs improvement - consider comprehensive review of analysis engines");
            }

            // Cross-analysis consistency analysis
            if (comprehensiveResult.CrossAnalysisConsistency.TryGetValue("QualityScoreConsistency", out var consistency) && consistency < 0.7)
            {
                improvements.Add("🔄 Improve consistency between different analysis types - ensure similar quality standards");
            }

            if (comprehensiveResult.CrossAnalysisConsistency.TryGetValue("ThemeConsistency", out var themeConsistency) && themeConsistency < 0.6)
            {
                improvements.Add("🎨 Enhance thematic consistency across analysis types - ensure complementary recommendations");
            }

            if (comprehensiveResult.CrossAnalysisConsistency.TryGetValue("PriorityAlignment", out var priorityAlignment) && priorityAlignment < 0.6)
            {
                improvements.Add("📋 Align priority levels across different analysis types for coherent action planning");
            }

            // Analysis type specific improvements
            foreach (var validationResult in comprehensiveResult.ValidationResults.Values)
            {
                if (validationResult.OverallQualityScore < 0.6)
                {
                    improvements.Add($"⚠️ {validationResult.AnalysisType} suggestions need significant improvement - focus on this analysis type");
                }
            }

            // Generate strategic improvements
            var strategicImprovements = await GenerateStrategicImprovementsAsync(comprehensiveResult);
            improvements.AddRange(strategicImprovements);

            return improvements.Distinct().Take(15).ToList(); // More improvements for system-wide
        }

        /// <summary>
        /// Generate insights from validation trends
        /// </summary>
        public async Task<TrendInsights> GenerateTrendInsightsAsync(FeedbackTrendsData trendsData)
        {
            _logger.LogInformation("Generating trend insights for period {StartDate} to {EndDate}", 
                trendsData.StartDate, trendsData.EndDate);

            var insights = new TrendInsights();

            // Quality trend analysis
            if (trendsData.QualityTrends.TryGetValue("OverallQuality", out var qualityTrend) && qualityTrend.Count > 1)
            {
                var trendDirection = AnalyzeTrendDirection(qualityTrend);
                
                if (trendDirection == TrendDirection.Decreasing)
                {
                    insights.ImprovementOpportunities.Add("📉 Quality trend is declining - immediate intervention needed");
                    insights.Recommendations.Add("Implement quality gate reviews for all suggestions");
                }
                else if (trendDirection == TrendDirection.Increasing)
                {
                    insights.SuccessPatterns.Add("📈 Quality trend is improving - continue current practices");
                }

                insights.KeyMetrics["QualityTrend"] = CalculateTrendStrength(qualityTrend);
            }

            // Volume analysis
            var volumeStats = trendsData.VolumeStats;
            if (volumeStats.LowQualitySuggestions > volumeStats.HighQualitySuggestions)
            {
                insights.ImprovementOpportunities.Add("⚠️ More low-quality than high-quality suggestions - focus on quality improvement");
            }

            if (volumeStats.TotalValidations > 0)
            {
                var highQualityRatio = (double)volumeStats.HighQualitySuggestions / volumeStats.TotalValidations;
                insights.KeyMetrics["HighQualityRatio"] = highQualityRatio;
                
                if (highQualityRatio > 0.7)
                {
                    insights.SuccessPatterns.Add("✅ High proportion of high-quality suggestions");
                }
            }

            // Top issues analysis
            foreach (var issue in trendsData.TopIssues.Take(5))
            {
                if (issue.Frequency > volumeStats.TotalValidations * 0.2) // More than 20% of validations
                {
                    insights.ImprovementOpportunities.Add($"🔧 Address frequent issue: {issue.IssueType} ({issue.Frequency} occurrences)");
                }
            }

            // Generate recommendations based on patterns
            insights.Recommendations.AddRange(await GenerateTrendBasedRecommendationsAsync(trendsData));

            return insights;
        }

        #region Private Helper Methods

        /// <summary>
        /// Initialize improvement templates for common issues
        /// </summary>
        private Dictionary<string, List<string>> InitializeImprovementTemplates()
        {
            return new Dictionary<string, List<string>>
            {
                ["LowRelevance"] = new List<string>
                {
                    "🎯 Ensure suggestions directly address the identified patterns and their implications",
                    "📊 Include specific references to pattern confidence scores and characteristics",
                    "🔍 Focus on the most significant patterns rather than minor anomalies"
                },
                ["LowActionability"] = new List<string>
                {
                    "⚡ Add specific implementation steps with clear action items",
                    "📝 Include configuration examples, code snippets, or specific tools to use",
                    "🎯 Break down complex suggestions into smaller, manageable tasks"
                },
                ["LowCausalValidity"] = new List<string>
                {
                    "🔬 Strengthen causal claims with additional statistical evidence",
                    "📈 Reference specific causal strength scores and validation results",
                    "⚖️ Consider alternative explanations and confounding factors"
                },
                ["WeakEvidence"] = new List<string>
                {
                    "📊 Include more robust statistical analysis and validation metrics",
                    "🔍 Provide larger sample sizes or longer observation periods",
                    "📈 Reference specific p-values, confidence intervals, or effect sizes"
                },
                ["LowTechnicalAccuracy"] = new List<string>
                {
                    "🔧 Ensure technical recommendations are current and follow best practices",
                    "📚 Reference authoritative sources and proven optimization techniques",
                    "⚡ Validate suggestions against actual system architecture and constraints"
                },
                ["LowSpecificity"] = new List<string>
                {
                    "📋 Add quantifiable metrics and success criteria",
                    "🎯 Specify exact components, methods, or configurations to modify",
                    "📊 Include expected outcomes with measurable improvements"
                },
                ["LowFeasibility"] = new List<string>
                {
                    "⚖️ Consider resource constraints and implementation complexity",
                    "🕒 Provide realistic timelines and effort estimates",
                    "🛠️ Suggest incremental approaches for complex changes"
                }
            };
        }

        /// <summary>
        /// Analyze common issues across suggestion validations
        /// </summary>
        private Dictionary<string, int> AnalyzeCommonIssues(List<IndividualSuggestionValidation> validations)
        {
            var issueFrequency = new Dictionary<string, int>();

            foreach (var validation in validations)
            {
                foreach (var issue in validation.Issues)
                {
                    if (issueFrequency.ContainsKey(issue.IssueType))
                    {
                        issueFrequency[issue.IssueType]++;
                    }
                    else
                    {
                        issueFrequency[issue.IssueType] = 1;
                    }
                }
            }

            return issueFrequency.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Generate adaptive improvements using ML insights
        /// </summary>
        private async Task<List<string>> GenerateAdaptiveImprovementsAsync(SuggestionValidationResult validationResult, AnalysisType analysisType)
        {
            var improvements = new List<string>();

            // Analyze quality score distribution
            var qualityScores = validationResult.SuggestionValidations.Select(sv => sv.OverallScore).ToArray();
            if (qualityScores.Any())
            {
                var variance = CalculateVariance(qualityScores);
                if (variance > 0.1) // High variance indicates inconsistent quality
                {
                    improvements.Add("📊 Reduce quality variance by standardizing suggestion generation criteria");
                }

                var lowQualityCount = qualityScores.Count(q => q < 0.6);
                if (lowQualityCount > qualityScores.Length * 0.3) // More than 30% low quality
                {
                    improvements.Add($"⚠️ {lowQualityCount} suggestions below quality threshold - review generation algorithms");
                }
            }

            // Analysis type specific adaptive improvements
            switch (analysisType)
            {
                case AnalysisType.PatternDetection:
                    if (validationResult.QualityMetrics.TryGetValue("PatternAlignmentScore", out var alignment) && alignment < 0.6)
                    {
                        improvements.Add("🎯 Improve pattern-suggestion alignment by using higher confidence patterns");
                    }
                    break;

                case AnalysisType.CausalAnalysis:
                    if (validationResult.QualityMetrics.TryGetValue("CausalStrengthAlignment", out var causalAlignment) && causalAlignment < 0.6)
                    {
                        improvements.Add("🔗 Better align suggestions with actual causal strength measurements");
                    }
                    break;

                case AnalysisType.PerformanceOptimization:
                    if (validationResult.QualityMetrics.TryGetValue("ImpactPredictability", out var predictability) && predictability < 0.6)
                    {
                        improvements.Add("📈 Improve performance impact predictions using historical optimization data");
                    }
                    break;
            }

            return improvements;
        }

        /// <summary>
        /// Generate strategic improvements for system-wide quality
        /// </summary>
        private async Task<List<string>> GenerateStrategicImprovementsAsync(ComprehensiveSuggestionValidationResult comprehensiveResult)
        {
            var improvements = new List<string>();

            // Identify best-performing analysis type
            var bestPerforming = comprehensiveResult.ValidationResults
                .OrderByDescending(kvp => kvp.Value.OverallQualityScore)
                .FirstOrDefault();

            if (bestPerforming.Value != null)
            {
                improvements.Add($"🏆 Learn from {bestPerforming.Key} analysis - highest quality at {bestPerforming.Value.OverallQualityScore:P1}");
            }

            // Identify worst-performing analysis type
            var worstPerforming = comprehensiveResult.ValidationResults
                .OrderBy(kvp => kvp.Value.OverallQualityScore)
                .FirstOrDefault();

            if (worstPerforming.Value != null && worstPerforming.Value.OverallQualityScore < 0.6)
            {
                improvements.Add($"🎯 Prioritize improvement of {worstPerforming.Key} analysis - lowest quality at {worstPerforming.Value.OverallQualityScore:P1}");
            }

            // Cross-analysis learning opportunities
            if (comprehensiveResult.ValidationResults.Count > 1)
            {
                improvements.Add("🔄 Implement cross-analysis learning - share quality improvement techniques between analysis types");
            }

            // System integration improvements
            improvements.Add("🏗️ Develop unified quality standards across all analysis types");
            improvements.Add("📊 Implement real-time quality monitoring dashboard");
            improvements.Add("🤖 Consider automated suggestion enhancement based on validation feedback");

            return improvements;
        }

        /// <summary>
        /// Generate recommendations based on trend analysis
        /// </summary>
        private async Task<List<string>> GenerateTrendBasedRecommendationsAsync(FeedbackTrendsData trendsData)
        {
            var recommendations = new List<string>();

            // Quality trend recommendations
            if (trendsData.QualityTrends.TryGetValue("DailyAverageQuality", out var dailyQuality) && dailyQuality.Count > 7)
            {
                var recentWeekAvg = dailyQuality.TakeLast(7).Average();
                var previousWeekAvg = dailyQuality.Skip(Math.Max(0, dailyQuality.Count - 14)).Take(7).Average();

                if (recentWeekAvg < previousWeekAvg)
                {
                    recommendations.Add("📉 Recent quality decline detected - implement immediate quality review process");
                }
                else if (recentWeekAvg > previousWeekAvg)
                {
                    recommendations.Add("📈 Quality improvement trend - document and replicate successful practices");
                }
            }

            // Volume-based recommendations
            var volumeStats = trendsData.VolumeStats;
            if (volumeStats.TotalValidations > 100) // Sufficient data
            {
                var qualityRatio = (double)volumeStats.HighQualitySuggestions / volumeStats.TotalValidations;
                
                if (qualityRatio < 0.3)
                {
                    recommendations.Add("⚠️ Low high-quality ratio - implement suggestion quality gates");
                }
                else if (qualityRatio > 0.8)
                {
                    recommendations.Add("✅ Excellent quality ratio - consider sharing best practices");
                }
            }

            // Issue-based recommendations
            if (trendsData.TopIssues.Any())
            {
                var topIssue = trendsData.TopIssues.First();
                recommendations.Add($"🔧 Address most frequent issue: {topIssue.IssueType} - implement targeted improvement");
            }

            return recommendations;
        }

        /// <summary>
        /// Analyze trend direction from time series data
        /// </summary>
        private TrendDirection AnalyzeTrendDirection(List<double> values)
        {
            if (values.Count < 2) return TrendDirection.Stable;

            var firstHalf = values.Take(values.Count / 2).Average();
            var secondHalf = values.Skip(values.Count / 2).Average();

            var difference = secondHalf - firstHalf;
            var threshold = 0.05; // 5% change threshold

            if (difference > threshold) return TrendDirection.Increasing;
            if (difference < -threshold) return TrendDirection.Decreasing;
            return TrendDirection.Stable;
        }

        /// <summary>
        /// Calculate trend strength (correlation with time)
        /// </summary>
        private double CalculateTrendStrength(List<double> values)
        {
            if (values.Count < 2) return 0.0;

            var xValues = Enumerable.Range(0, values.Count).Select(i => (double)i).ToArray();
            var yValues = values.ToArray();

            return Math.Abs(CalculateCorrelation(xValues, yValues));
        }

        /// <summary>
        /// Calculate correlation coefficient between two arrays
        /// </summary>
        private double CalculateCorrelation(double[] x, double[] y)
        {
            if (x.Length != y.Length || x.Length < 2) return 0.0;

            var meanX = x.Average();
            var meanY = y.Average();

            var numerator = x.Zip(y, (xi, yi) => (xi - meanX) * (yi - meanY)).Sum();
            var denomX = Math.Sqrt(x.Select(xi => Math.Pow(xi - meanX, 2)).Sum());
            var denomY = Math.Sqrt(y.Select(yi => Math.Pow(yi - meanY, 2)).Sum());

            if (denomX == 0 || denomY == 0) return 0.0;

            return numerator / (denomX * denomY);
        }

        /// <summary>
        /// Calculate variance of values
        /// </summary>
        private double CalculateVariance(double[] values)
        {
            if (values.Length < 2) return 0.0;
            var mean = values.Average();
            return values.Select(v => Math.Pow(v - mean, 2)).Average();
        }

        #endregion
    }

    /// <summary>
    /// Trend direction enumeration
    /// </summary>
    public enum TrendDirection
    {
        Increasing,
        Decreasing,
        Stable
    }
}
