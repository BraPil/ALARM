using ALARM.FeedbackUI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ALARM.FeedbackUI.Services
{
    /// <summary>
    /// Interface for feedback analytics service
    /// </summary>
    public interface IFeedbackAnalyticsService
    {
        Task<FeedbackAnalyticsDto> GetAnalyticsSummaryAsync(int days = 30);
        Task<List<FeedbackTrendDto>> GetAccuracyTrendAsync(int days = 30);
        Task<List<FeedbackTrendDto>> GetUsageTrendAsync(int days = 30);
        Task<List<LearningInsightDto>> GenerateLearningInsightsAsync();
        Task<Dictionary<string, object>> GetDetailedAnalyticsAsync(string category, int days = 30);
        Task CleanupOldAnalyticsAsync();
    }

    /// <summary>
    /// Service for analyzing feedback data and generating insights
    /// </summary>
    public class FeedbackAnalyticsService : IFeedbackAnalyticsService
    {
        private readonly FeedbackDataContext _context;
        private readonly ILogger<FeedbackAnalyticsService> _logger;
        private readonly FeedbackConfiguration _config;

        public FeedbackAnalyticsService(FeedbackDataContext context, ILogger<FeedbackAnalyticsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = new FeedbackConfiguration();
        }

        /// <summary>
        /// Get comprehensive analytics summary
        /// </summary>
        public async Task<FeedbackAnalyticsDto> GetAnalyticsSummaryAsync(int days = 30)
        {
            _logger.LogInformation("Generating analytics summary for {Days} days", days);

            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                
                var feedback = await _context.FeedbackEntries
                    .Where(f => f.Timestamp >= cutoffDate)
                    .ToListAsync();

                var analytics = new FeedbackAnalyticsDto
                {
                    TotalFeedback = feedback.Count,
                    LastUpdated = DateTime.UtcNow
                };

                if (feedback.Any())
                {
                    // Calculate averages
                    var accuracyRatings = feedback.Where(f => f.Accuracy.HasValue).Select(f => f.Accuracy!.Value).ToList();
                    var usefulnessRatings = feedback.Where(f => f.Usefulness.HasValue).Select(f => f.Usefulness!.Value).ToList();
                    var impactRatings = feedback.Where(f => f.Impact.HasValue).Select(f => f.Impact!.Value).ToList();

                    analytics.AverageAccuracy = accuracyRatings.Any() ? accuracyRatings.Average() : 0;
                    analytics.AverageUsefulness = usefulnessRatings.Any() ? usefulnessRatings.Average() : 0;
                    analytics.AverageImpact = impactRatings.Any() ? impactRatings.Average() : 0;

                    // Calculate implementation rate
                    var implementationFeedback = feedback.Where(f => !string.IsNullOrEmpty(f.Implemented)).ToList();
                    if (implementationFeedback.Any())
                    {
                        var implementedCount = implementationFeedback.Count(f => f.Implemented == "yes" || f.Implemented == "partial");
                        analytics.ImplementationRate = (double)implementedCount / implementationFeedback.Count * 100;
                    }

                    // Feedback by type
                    analytics.FeedbackByType = feedback
                        .GroupBy(f => f.FeedbackType)
                        .ToDictionary(g => g.Key, g => g.Count());

                    // Accuracy by analysis type
                    analytics.AccuracyByAnalysisType = feedback
                        .Where(f => f.Accuracy.HasValue && !string.IsNullOrEmpty(f.AnalysisType))
                        .GroupBy(f => f.AnalysisType!)
                        .ToDictionary(g => g.Key, g => g.Average(f => f.Accuracy!.Value));

                    // Impact by recommendation type
                    analytics.ImpactByRecommendationType = feedback
                        .Where(f => f.Impact.HasValue && !string.IsNullOrEmpty(f.RecommendationType))
                        .GroupBy(f => f.RecommendationType!)
                        .ToDictionary(g => g.Key, g => g.Average(f => f.Impact!.Value));

                    // Implementation by difficulty
                    analytics.ImplementationByDifficulty = feedback
                        .Where(f => !string.IsNullOrEmpty(f.Implementability))
                        .GroupBy(f => f.Implementability!)
                        .ToDictionary(g => g.Key, g => g.Count());

                    // Get trends
                    analytics.AccuracyTrend = await GetAccuracyTrendAsync(days);
                    analytics.UsageTrend = await GetUsageTrendAsync(days);
                }

                _logger.LogInformation("Analytics summary generated: {TotalFeedback} feedback entries", analytics.TotalFeedback);
                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating analytics summary");
                throw;
            }
        }

        /// <summary>
        /// Get accuracy trend over time
        /// </summary>
        public async Task<List<FeedbackTrendDto>> GetAccuracyTrendAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            var accuracyData = await _context.FeedbackEntries
                .Where(f => f.Timestamp >= cutoffDate && f.Accuracy.HasValue)
                .GroupBy(f => f.Timestamp.Date)
                .Select(g => new FeedbackTrendDto
                {
                    Date = g.Key,
                    Value = g.Average(f => f.Accuracy!.Value),
                    Count = g.Count(),
                    Category = "Accuracy"
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            return accuracyData;
        }

        /// <summary>
        /// Get usage trend over time
        /// </summary>
        public async Task<List<FeedbackTrendDto>> GetUsageTrendAsync(int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            
            var usageData = await _context.FeedbackEntries
                .Where(f => f.Timestamp >= cutoffDate)
                .GroupBy(f => f.Timestamp.Date)
                .Select(g => new FeedbackTrendDto
                {
                    Date = g.Key,
                    Value = g.Count(),
                    Count = g.Count(),
                    Category = "Usage"
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            return usageData;
        }

        /// <summary>
        /// Generate learning insights from feedback patterns
        /// </summary>
        public async Task<List<LearningInsightDto>> GenerateLearningInsightsAsync()
        {
            _logger.LogInformation("Generating learning insights from feedback data");

            try
            {
                var insights = new List<LearningInsightDto>();
                var recentFeedback = await _context.FeedbackEntries
                    .Where(f => f.Timestamp >= DateTime.UtcNow.AddDays(-30))
                    .ToListAsync();

                if (!recentFeedback.Any())
                {
                    return insights;
                }

                // Insight 1: Low accuracy analysis types
                var accuracyByType = recentFeedback
                    .Where(f => f.Accuracy.HasValue && !string.IsNullOrEmpty(f.AnalysisType))
                    .GroupBy(f => f.AnalysisType!)
                    .Where(g => g.Count() >= 3) // Minimum sample size
                    .Select(g => new { Type = g.Key, AvgAccuracy = g.Average(f => f.Accuracy!.Value), Count = g.Count() })
                    .Where(x => x.AvgAccuracy < _config.MinAccuracyThreshold)
                    .OrderBy(x => x.AvgAccuracy)
                    .ToList();

                foreach (var lowAccuracy in accuracyByType)
                {
                    insights.Add(new LearningInsightDto
                    {
                        InsightType = "LowAccuracy",
                        Title = $"Low Accuracy in {lowAccuracy.Type}",
                        Description = $"Analysis type '{lowAccuracy.Type}' has low accuracy rating of {lowAccuracy.AvgAccuracy:F1}/5 based on {lowAccuracy.Count} feedback entries.",
                        Confidence = Math.Min(0.9, lowAccuracy.Count / 10.0), // Higher confidence with more data
                        Impact = (5.0 - lowAccuracy.AvgAccuracy) / 5.0, // Impact based on accuracy gap
                        SupportingEvidence = new List<string>
                        {
                            $"Average accuracy: {lowAccuracy.AvgAccuracy:F1}/5",
                            $"Sample size: {lowAccuracy.Count} feedback entries",
                            $"Below threshold of {_config.MinAccuracyThreshold}"
                        },
                        Recommendations = new List<string>
                        {
                            $"Review and improve {lowAccuracy.Type} algorithm",
                            "Collect more training data for this analysis type",
                            "Consider adjusting confidence thresholds",
                            "Investigate common failure patterns"
                        },
                        Metadata = new Dictionary<string, object>
                        {
                            ["AnalysisType"] = lowAccuracy.Type,
                            ["AverageAccuracy"] = lowAccuracy.AvgAccuracy,
                            ["SampleSize"] = lowAccuracy.Count
                        }
                    });
                }

                // Insight 2: High-impact recommendations not being implemented
                var implementationByType = recentFeedback
                    .Where(f => f.Impact.HasValue && f.Impact >= 4 && !string.IsNullOrEmpty(f.RecommendationType) && !string.IsNullOrEmpty(f.Implemented))
                    .GroupBy(f => f.RecommendationType!)
                    .Where(g => g.Count() >= 3)
                    .Select(g => new 
                    { 
                        Type = g.Key, 
                        AvgImpact = g.Average(f => f.Impact!.Value),
                        ImplementationRate = g.Count(f => f.Implemented == "yes" || f.Implemented == "partial") / (double)g.Count(),
                        Count = g.Count()
                    })
                    .Where(x => x.ImplementationRate < 0.5) // Less than 50% implementation
                    .OrderByDescending(x => x.AvgImpact)
                    .ToList();

                foreach (var lowImplementation in implementationByType)
                {
                    insights.Add(new LearningInsightDto
                    {
                        InsightType = "LowImplementation",
                        Title = $"High-Impact {lowImplementation.Type} Recommendations Not Implemented",
                        Description = $"Recommendations of type '{lowImplementation.Type}' have high impact ({lowImplementation.AvgImpact:F1}/5) but low implementation rate ({lowImplementation.ImplementationRate:P0}).",
                        Confidence = Math.Min(0.8, lowImplementation.Count / 10.0),
                        Impact = lowImplementation.AvgImpact / 5.0,
                        SupportingEvidence = new List<string>
                        {
                            $"Average impact rating: {lowImplementation.AvgImpact:F1}/5",
                            $"Implementation rate: {lowImplementation.ImplementationRate:P0}",
                            $"Sample size: {lowImplementation.Count} feedback entries"
                        },
                        Recommendations = new List<string>
                        {
                            "Investigate barriers to implementation",
                            "Provide more detailed implementation guidance",
                            "Consider breaking recommendations into smaller steps",
                            "Offer implementation support or automation"
                        },
                        Metadata = new Dictionary<string, object>
                        {
                            ["RecommendationType"] = lowImplementation.Type,
                            ["AverageImpact"] = lowImplementation.AvgImpact,
                            ["ImplementationRate"] = lowImplementation.ImplementationRate,
                            ["SampleSize"] = lowImplementation.Count
                        }
                    });
                }

                // Insight 3: Trending analysis types
                var recentUsage = recentFeedback
                    .Where(f => !string.IsNullOrEmpty(f.AnalysisType))
                    .GroupBy(f => f.AnalysisType!)
                    .Select(g => new { Type = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(3)
                    .ToList();

                if (recentUsage.Any())
                {
                    var topType = recentUsage.First();
                    insights.Add(new LearningInsightDto
                    {
                        InsightType = "TrendingAnalysis",
                        Title = $"High Usage of {topType.Type} Analysis",
                        Description = $"Analysis type '{topType.Type}' is being used frequently ({topType.Count} times in the last 30 days), indicating high user interest.",
                        Confidence = 0.7,
                        Impact = 0.6,
                        SupportingEvidence = new List<string>
                        {
                            $"Usage count: {topType.Count} in 30 days",
                            $"Most popular analysis type",
                            "High user engagement"
                        },
                        Recommendations = new List<string>
                        {
                            "Continue investing in this analysis type",
                            "Consider expanding capabilities",
                            "Monitor for performance bottlenecks",
                            "Collect additional training data"
                        },
                        Metadata = new Dictionary<string, object>
                        {
                            ["AnalysisType"] = topType.Type,
                            ["UsageCount"] = topType.Count,
                            ["Rank"] = 1
                        }
                    });
                }

                _logger.LogInformation("Generated {InsightCount} learning insights", insights.Count);
                return insights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating learning insights");
                throw;
            }
        }

        /// <summary>
        /// Get detailed analytics for specific category
        /// </summary>
        public async Task<Dictionary<string, object>> GetDetailedAnalyticsAsync(string category, int days = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days);
            var analytics = new Dictionary<string, object>();

            switch (category.ToLower())
            {
                case "accuracy":
                    var accuracyData = await _context.FeedbackEntries
                        .Where(f => f.Timestamp >= cutoffDate && f.Accuracy.HasValue)
                        .ToListAsync();
                    
                    analytics["TotalRatings"] = accuracyData.Count;
                    analytics["AverageRating"] = accuracyData.Any() ? accuracyData.Average(f => f.Accuracy!.Value) : 0;
                    analytics["ByAnalysisType"] = accuracyData
                        .Where(f => !string.IsNullOrEmpty(f.AnalysisType))
                        .GroupBy(f => f.AnalysisType!)
                        .ToDictionary(g => g.Key, g => new 
                        { 
                            Average = g.Average(f => f.Accuracy!.Value),
                            Count = g.Count(),
                            Distribution = g.GroupBy(f => f.Accuracy!.Value).ToDictionary(x => x.Key, x => x.Count())
                        });
                    break;

                case "implementation":
                    var implementationData = await _context.FeedbackEntries
                        .Where(f => f.Timestamp >= cutoffDate && !string.IsNullOrEmpty(f.Implemented))
                        .ToListAsync();
                    
                    analytics["TotalResponses"] = implementationData.Count;
                    analytics["ImplementationRate"] = implementationData.Any() 
                        ? implementationData.Count(f => f.Implemented == "yes" || f.Implemented == "partial") / (double)implementationData.Count * 100
                        : 0;
                    analytics["ByStatus"] = implementationData
                        .GroupBy(f => f.Implemented!)
                        .ToDictionary(g => g.Key, g => g.Count());
                    analytics["ByDifficulty"] = implementationData
                        .Where(f => !string.IsNullOrEmpty(f.Implementability))
                        .GroupBy(f => f.Implementability!)
                        .ToDictionary(g => g.Key, g => new
                        {
                            Count = g.Count(),
                            ImplementationRate = g.Count(f => f.Implemented == "yes" || f.Implemented == "partial") / (double)g.Count() * 100
                        });
                    break;

                default:
                    analytics["Error"] = "Unknown category";
                    break;
            }

            return analytics;
        }

        /// <summary>
        /// Clean up old analytics data
        /// </summary>
        public async Task CleanupOldAnalyticsAsync()
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-_config.AnalyticsRetentionDays);
                
                var oldAnalytics = await _context.FeedbackAnalytics
                    .Where(a => a.Timestamp < cutoffDate)
                    .ToListAsync();

                if (oldAnalytics.Any())
                {
                    _context.FeedbackAnalytics.RemoveRange(oldAnalytics);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Cleaned up {Count} old analytics records", oldAnalytics.Count);
                }

                // Also clean up old feedback if configured
                var feedbackCutoffDate = DateTime.UtcNow.AddDays(-_config.FeedbackRetentionDays);
                var oldFeedback = await _context.FeedbackEntries
                    .Where(f => f.Timestamp < feedbackCutoffDate)
                    .ToListAsync();

                if (oldFeedback.Any())
                {
                    _context.FeedbackEntries.RemoveRange(oldFeedback);
                    await _context.SaveChangesAsync();
                    
                    _logger.LogInformation("Cleaned up {Count} old feedback records", oldFeedback.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during analytics cleanup");
                throw;
            }
        }
    }
}
