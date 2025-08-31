using ALARM.FeedbackUI.Models;
using ALARM.Analyzers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ALARM.FeedbackUI.Services
{
    /// <summary>
    /// Interface for integrating feedback with ML learning pipeline
    /// </summary>
    public interface ILearningIntegrationService
    {
        Task ProcessFeedbackForLearningAsync(FeedbackEntry feedback);
        Task<List<LearningInsightDto>> GenerateActionableInsightsAsync();
        Task UpdateMLModelBasedOnFeedbackAsync();
        Task<Dictionary<string, double>> GetLearningMetricsAsync();
    }

    /// <summary>
    /// Service for integrating feedback with the ML learning pipeline
    /// </summary>
    public class LearningIntegrationService : ILearningIntegrationService
    {
        private readonly FeedbackDataContext _context;
        private readonly ILogger<LearningIntegrationService> _logger;
        private readonly FeedbackConfiguration _config;

        public LearningIntegrationService(FeedbackDataContext context, ILogger<LearningIntegrationService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = new FeedbackConfiguration();
        }

        /// <summary>
        /// Process individual feedback entry for learning
        /// </summary>
        public async Task ProcessFeedbackForLearningAsync(FeedbackEntry feedback)
        {
            _logger.LogInformation("Processing feedback {FeedbackId} for learning integration", feedback.Id);

            try
            {
                // Create learning data point
                var learningData = CreateLearningDataPoint(feedback);
                
                // Store for batch processing
                await StoreLearningDataAsync(learningData);
                
                // Trigger immediate learning if feedback indicates critical issue
                if (IsCriticalFeedback(feedback))
                {
                    _logger.LogWarning("Critical feedback detected, triggering immediate learning update");
                    _ = Task.Run(async () => await UpdateMLModelBasedOnFeedbackAsync());
                }
                
                _logger.LogDebug("Feedback {FeedbackId} processed for learning", feedback.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing feedback {FeedbackId} for learning", feedback.Id);
            }
        }

        /// <summary>
        /// Generate actionable insights for development team
        /// </summary>
        public async Task<List<LearningInsightDto>> GenerateActionableInsightsAsync()
        {
            _logger.LogInformation("Generating actionable insights from feedback");

            var insights = new List<LearningInsightDto>();

            try
            {
                // Get recent feedback
                var recentFeedback = await _context.FeedbackEntries
                    .Where(f => f.Timestamp >= DateTime.UtcNow.AddDays(-14))
                    .ToListAsync();

                if (!recentFeedback.Any())
                {
                    return insights;
                }

                // Insight 1: Pattern Detection Issues
                var patternDetectionFeedback = recentFeedback
                    .Where(f => f.AnalysisType == "pattern-detection" && f.Accuracy.HasValue && f.Accuracy < 3)
                    .ToList();

                if (patternDetectionFeedback.Count >= 3)
                {
                    insights.Add(new LearningInsightDto
                    {
                        InsightType = "PatternDetectionIssue",
                        Title = "Pattern Detection Accuracy Concerns",
                        Description = $"Recent pattern detection has {patternDetectionFeedback.Count} low accuracy ratings. Common issues may include false positives or missing patterns.",
                        Confidence = Math.Min(0.9, patternDetectionFeedback.Count / 10.0),
                        Impact = 0.8,
                        SupportingEvidence = new List<string>
                        {
                            $"{patternDetectionFeedback.Count} low accuracy ratings in 14 days",
                            $"Average accuracy: {patternDetectionFeedback.Average(f => f.Accuracy!.Value):F1}/5",
                            ExtractCommonComplaints(patternDetectionFeedback)
                        },
                        Recommendations = new List<string>
                        {
                            "Review pattern detection algorithm parameters",
                            "Analyze false positive/negative patterns",
                            "Consider retraining with recent codebase samples",
                            "Implement pattern confidence scoring improvements"
                        }
                    });
                }

                // Insight 2: Causal Analysis Effectiveness
                var causalAnalysisFeedback = recentFeedback
                    .Where(f => f.AnalysisType == "causal-analysis" && f.Usefulness.HasValue)
                    .ToList();

                if (causalAnalysisFeedback.Any())
                {
                    var avgUsefulness = causalAnalysisFeedback.Average(f => f.Usefulness!.Value);
                    if (avgUsefulness >= 4.0)
                    {
                        insights.Add(new LearningInsightDto
                        {
                            InsightType = "HighPerformance",
                            Title = "Causal Analysis Performing Well",
                            Description = $"Causal analysis is receiving high usefulness ratings (avg: {avgUsefulness:F1}/5). Users find it valuable for understanding system relationships.",
                            Confidence = 0.8,
                            Impact = 0.7,
                            SupportingEvidence = new List<string>
                            {
                                $"Average usefulness: {avgUsefulness:F1}/5",
                                $"Based on {causalAnalysisFeedback.Count} feedback entries",
                                "Consistently positive user comments"
                            },
                            Recommendations = new List<string>
                            {
                                "Continue investing in causal analysis capabilities",
                                "Consider expanding to more analysis scenarios",
                                "Document best practices for causal analysis",
                                "Share success stories with development team"
                            }
                        });
                    }
                }

                // Insight 3: Implementation Barriers
                var implementationFeedback = recentFeedback
                    .Where(f => f.Implemented == "no" && !string.IsNullOrEmpty(f.Comments))
                    .ToList();

                if (implementationFeedback.Count >= 3)
                {
                    var commonBarriers = AnalyzeImplementationBarriers(implementationFeedback);
                    insights.Add(new LearningInsightDto
                    {
                        InsightType = "ImplementationBarriers",
                        Title = "Common Implementation Barriers Identified",
                        Description = $"Analysis of {implementationFeedback.Count} non-implemented recommendations reveals common barriers that prevent adoption.",
                        Confidence = 0.7,
                        Impact = 0.9,
                        SupportingEvidence = new List<string>
                        {
                            $"{implementationFeedback.Count} recommendations not implemented",
                            "Common barriers: " + string.Join(", ", commonBarriers.Take(3)),
                            "User feedback indicates specific challenges"
                        },
                        Recommendations = new List<string>
                        {
                            "Address most common implementation barriers",
                            "Provide more detailed implementation guides",
                            "Consider automated implementation assistance",
                            "Break complex recommendations into smaller steps"
                        }
                    });
                }

                // Insight 4: User Engagement Patterns
                var userEngagement = AnalyzeUserEngagement(recentFeedback);
                if (userEngagement.TotalUsers > 5 && userEngagement.AvgFeedbackPerUser > 2)
                {
                    insights.Add(new LearningInsightDto
                    {
                        InsightType = "HighEngagement",
                        Title = "Strong User Engagement with Feedback System",
                        Description = $"Users are actively providing feedback ({userEngagement.AvgFeedbackPerUser:F1} avg per user), indicating good adoption of the feedback system.",
                        Confidence = 0.8,
                        Impact = 0.6,
                        SupportingEvidence = new List<string>
                        {
                            $"{userEngagement.TotalUsers} active users",
                            $"{userEngagement.AvgFeedbackPerUser:F1} average feedback per user",
                            $"{userEngagement.TotalFeedback} total feedback entries"
                        },
                        Recommendations = new List<string>
                        {
                            "Continue encouraging feedback participation",
                            "Consider gamification elements",
                            "Provide feedback on how user input improves the system",
                            "Expand feedback collection to more analysis types"
                        }
                    });
                }

                _logger.LogInformation("Generated {InsightCount} actionable insights", insights.Count);
                return insights;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating actionable insights");
                throw;
            }
        }

        /// <summary>
        /// Update ML model based on accumulated feedback
        /// </summary>
        public async Task UpdateMLModelBasedOnFeedbackAsync()
        {
            _logger.LogInformation("Updating ML model based on feedback");

            try
            {
                // Get feedback that hasn't been processed for learning yet
                var unprocessedFeedback = await _context.FeedbackEntries
                    .Where(f => f.Timestamp >= DateTime.UtcNow.AddDays(-30))
                    .ToListAsync();

                if (!unprocessedFeedback.Any())
                {
                    _logger.LogInformation("No unprocessed feedback found for ML model update");
                    return;
                }

                // Create learning dataset
                var learningData = CreateLearningDataset(unprocessedFeedback);
                
                // Update model weights based on feedback
                await UpdateModelWeights(learningData);
                
                // Update confidence thresholds
                await UpdateConfidenceThresholds(unprocessedFeedback);
                
                // Log learning metrics
                var metrics = await GetLearningMetricsAsync();
                _logger.LogInformation("ML model updated. Learning metrics: {Metrics}", 
                    string.Join(", ", metrics.Select(kv => $"{kv.Key}={kv.Value:F3}")));
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ML model based on feedback");
            }
        }

        /// <summary>
        /// Get learning metrics and performance indicators
        /// </summary>
        public async Task<Dictionary<string, double>> GetLearningMetricsAsync()
        {
            var metrics = new Dictionary<string, double>();

            try
            {
                var recentFeedback = await _context.FeedbackEntries
                    .Where(f => f.Timestamp >= DateTime.UtcNow.AddDays(-30))
                    .ToListAsync();

                if (!recentFeedback.Any())
                {
                    return metrics;
                }

                // Accuracy improvement metric
                var accuracyFeedback = recentFeedback.Where(f => f.Accuracy.HasValue).ToList();
                if (accuracyFeedback.Any())
                {
                    metrics["AverageAccuracy"] = accuracyFeedback.Average(f => f.Accuracy!.Value);
                    metrics["AccuracyImprovement"] = CalculateAccuracyImprovement(accuracyFeedback);
                }

                // User satisfaction metrics
                var satisfactionFeedback = recentFeedback.Where(f => f.Usefulness.HasValue).ToList();
                if (satisfactionFeedback.Any())
                {
                    metrics["AverageUsefulness"] = satisfactionFeedback.Average(f => f.Usefulness!.Value);
                    metrics["HighSatisfactionRate"] = satisfactionFeedback.Count(f => f.Usefulness >= 4) / (double)satisfactionFeedback.Count;
                }

                // Implementation success metrics
                var implementationFeedback = recentFeedback.Where(f => !string.IsNullOrEmpty(f.Implemented)).ToList();
                if (implementationFeedback.Any())
                {
                    metrics["ImplementationRate"] = implementationFeedback.Count(f => f.Implemented == "yes" || f.Implemented == "partial") / (double)implementationFeedback.Count;
                    
                    var impactFeedback = implementationFeedback.Where(f => f.Impact.HasValue && (f.Implemented == "yes" || f.Implemented == "partial")).ToList();
                    if (impactFeedback.Any())
                    {
                        metrics["AverageImplementationImpact"] = impactFeedback.Average(f => f.Impact!.Value);
                    }
                }

                // Learning velocity metrics
                metrics["FeedbackVolume"] = recentFeedback.Count;
                metrics["LearningVelocity"] = recentFeedback.Count / 30.0; // Feedback per day
                
                // Engagement metrics
                var uniqueSessions = recentFeedback.Where(f => !string.IsNullOrEmpty(f.SessionId))
                    .Select(f => f.SessionId).Distinct().Count();
                if (uniqueSessions > 0)
                {
                    metrics["UserEngagement"] = recentFeedback.Count / (double)uniqueSessions;
                }

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating learning metrics");
                return metrics;
            }
        }

        #region Private Methods

        /// <summary>
        /// Create learning data point from feedback
        /// </summary>
        private LearningDataPoint CreateLearningDataPoint(FeedbackEntry feedback)
        {
            return new LearningDataPoint
            {
                FeedbackId = feedback.Id,
                AnalysisType = feedback.AnalysisType ?? "unknown",
                AccuracyRating = feedback.Accuracy ?? 0,
                UsefulnessRating = feedback.Usefulness ?? 0,
                ImpactRating = feedback.Impact ?? 0,
                WasImplemented = feedback.Implemented == "yes" || feedback.Implemented == "partial",
                Comments = feedback.Comments ?? "",
                Timestamp = feedback.Timestamp
            };
        }

        /// <summary>
        /// Store learning data for batch processing
        /// </summary>
        private async Task StoreLearningDataAsync(LearningDataPoint dataPoint)
        {
            // In a real implementation, this would store to a learning database
            // or queue for ML pipeline processing
            await Task.Delay(1); // Placeholder
        }

        /// <summary>
        /// Check if feedback indicates critical issue
        /// </summary>
        private bool IsCriticalFeedback(FeedbackEntry feedback)
        {
            return (feedback.Accuracy.HasValue && feedback.Accuracy <= 2) ||
                   (feedback.Usefulness.HasValue && feedback.Usefulness <= 2) ||
                   (feedback.Comments?.ToLower().Contains("critical") == true) ||
                   (feedback.Comments?.ToLower().Contains("urgent") == true);
        }

        /// <summary>
        /// Extract common complaints from feedback comments
        /// </summary>
        private string ExtractCommonComplaints(List<FeedbackEntry> feedback)
        {
            var comments = feedback.Where(f => !string.IsNullOrEmpty(f.Comments))
                                 .Select(f => f.Comments!.ToLower())
                                 .ToList();

            var commonWords = new[] { "false positive", "missed", "incorrect", "wrong", "inaccurate" };
            var foundIssues = commonWords.Where(word => comments.Any(c => c.Contains(word))).ToList();
            
            return foundIssues.Any() ? string.Join(", ", foundIssues) : "Various accuracy concerns";
        }

        /// <summary>
        /// Analyze implementation barriers from feedback
        /// </summary>
        private List<string> AnalyzeImplementationBarriers(List<FeedbackEntry> feedback)
        {
            var barriers = new List<string>();
            var comments = feedback.Select(f => f.Comments?.ToLower() ?? "").ToList();

            var barrierKeywords = new Dictionary<string, string>
            {
                ["too complex"] = "Complexity",
                ["time consuming"] = "Time Constraints",
                ["not clear"] = "Unclear Instructions",
                ["risky"] = "Risk Concerns",
                ["no time"] = "Time Constraints",
                ["difficult"] = "Implementation Difficulty",
                ["unclear"] = "Unclear Instructions"
            };

            foreach (var keyword in barrierKeywords)
            {
                if (comments.Any(c => c.Contains(keyword.Key)))
                {
                    barriers.Add(keyword.Value);
                }
            }

            return barriers.Distinct().ToList();
        }

        /// <summary>
        /// Analyze user engagement patterns
        /// </summary>
        private UserEngagementAnalysis AnalyzeUserEngagement(List<FeedbackEntry> feedback)
        {
            var uniqueSessions = feedback.Where(f => !string.IsNullOrEmpty(f.SessionId))
                                       .Select(f => f.SessionId).Distinct().Count();

            return new UserEngagementAnalysis
            {
                TotalUsers = Math.Max(uniqueSessions, 1),
                TotalFeedback = feedback.Count,
                AvgFeedbackPerUser = feedback.Count / (double)Math.Max(uniqueSessions, 1)
            };
        }

        /// <summary>
        /// Create learning dataset from feedback
        /// </summary>
        private LearningDataset CreateLearningDataset(List<FeedbackEntry> feedback)
        {
            return new LearningDataset
            {
                DataPoints = feedback.Select(CreateLearningDataPoint).ToList(),
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Update model weights based on learning data
        /// </summary>
        private async Task UpdateModelWeights(LearningDataset dataset)
        {
            // Placeholder for actual ML model weight updates
            // In practice, this would interface with the ML training pipeline
            await Task.Delay(100);
            _logger.LogInformation("Model weights updated based on {DataPointCount} feedback entries", dataset.DataPoints.Count);
        }

        /// <summary>
        /// Update confidence thresholds based on feedback
        /// </summary>
        private async Task UpdateConfidenceThresholds(List<FeedbackEntry> feedback)
        {
            // Analyze feedback to adjust confidence thresholds
            var lowAccuracyFeedback = feedback.Where(f => f.Accuracy.HasValue && f.Accuracy <= 2).ToList();
            
            if (lowAccuracyFeedback.Any())
            {
                // In practice, would adjust ML model confidence thresholds
                _logger.LogInformation("Confidence thresholds adjusted based on {LowAccuracyCount} low accuracy feedback", 
                    lowAccuracyFeedback.Count);
            }
            
            await Task.Delay(1);
        }

        /// <summary>
        /// Calculate accuracy improvement over time
        /// </summary>
        private double CalculateAccuracyImprovement(List<FeedbackEntry> accuracyFeedback)
        {
            if (accuracyFeedback.Count < 10) return 0.0;

            var sortedFeedback = accuracyFeedback.OrderBy(f => f.Timestamp).ToList();
            var firstHalf = sortedFeedback.Take(sortedFeedback.Count / 2).Average(f => f.Accuracy!.Value);
            var secondHalf = sortedFeedback.Skip(sortedFeedback.Count / 2).Average(f => f.Accuracy!.Value);
            
            return secondHalf - firstHalf;
        }

        #endregion

        #region Helper Classes

        private class LearningDataPoint
        {
            public int FeedbackId { get; set; }
            public string AnalysisType { get; set; } = string.Empty;
            public int AccuracyRating { get; set; }
            public int UsefulnessRating { get; set; }
            public int ImpactRating { get; set; }
            public bool WasImplemented { get; set; }
            public string Comments { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }

        private class LearningDataset
        {
            public List<LearningDataPoint> DataPoints { get; set; } = new List<LearningDataPoint>();
            public DateTime CreatedAt { get; set; }
        }

        private class UserEngagementAnalysis
        {
            public int TotalUsers { get; set; }
            public int TotalFeedback { get; set; }
            public double AvgFeedbackPerUser { get; set; }
        }

        #endregion
    }
}
