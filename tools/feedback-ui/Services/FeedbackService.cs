using ALARM.FeedbackUI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ALARM.FeedbackUI.Services
{
    /// <summary>
    /// Interface for feedback collection service
    /// </summary>
    public interface IFeedbackService
    {
        Task<int> SubmitFeedbackAsync(FeedbackSubmissionDto feedback, string? userAgent = null, string? ipAddress = null);
        Task<List<FeedbackEntry>> GetFeedbackAsync(int limit = 100, string? feedbackType = null);
        Task<List<FeedbackEntry>> GetFeedbackByAnalysisRunAsync(string analysisRunId);
        Task<FeedbackEntry?> GetFeedbackByIdAsync(int id);
        Task<bool> UpdateFeedbackAsync(int id, FeedbackSubmissionDto feedback);
        Task<bool> DeleteFeedbackAsync(int id);
    }

    /// <summary>
    /// Service for managing feedback collection and storage
    /// </summary>
    public class FeedbackService : IFeedbackService
    {
        private readonly FeedbackDataContext _context;
        private readonly ILogger<FeedbackService> _logger;
        private readonly FeedbackConfiguration _config;

        public FeedbackService(FeedbackDataContext context, ILogger<FeedbackService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = new FeedbackConfiguration(); // In production, inject from configuration
        }

        /// <summary>
        /// Submit new feedback entry
        /// </summary>
        public async Task<int> SubmitFeedbackAsync(FeedbackSubmissionDto feedback, string? userAgent = null, string? ipAddress = null)
        {
            _logger.LogInformation("Submitting feedback of type {FeedbackType}", feedback.FeedbackType);

            try
            {
                // Validate feedback
                ValidateFeedback(feedback);

                // Create feedback entry
                var entry = new FeedbackEntry
                {
                    FeedbackType = feedback.FeedbackType,
                    AnalysisType = feedback.AnalysisType,
                    RecommendationType = feedback.RecommendationType,
                    Accuracy = feedback.Accuracy,
                    Usefulness = feedback.Usefulness,
                    Impact = feedback.Impact,
                    Implementability = feedback.Implementability,
                    Implemented = feedback.Implemented,
                    Comments = TruncateText(feedback.Comments, _config.MaxFeedbackLength),
                    ProjectName = feedback.ProjectName,
                    FilePath = feedback.FilePath,
                    AnalysisRunId = feedback.AnalysisRunId,
                    UserAgent = TruncateText(userAgent, 500),
                    IpAddress = ipAddress,
                    SessionId = GenerateSessionId(),
                    Timestamp = DateTime.UtcNow
                };

                _context.FeedbackEntries.Add(entry);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Feedback submitted successfully with ID {FeedbackId}", entry.Id);

                // Note: Analytics update moved to be handled properly with DI scoping
                // Background task would need proper service scope management

                return entry.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback");
                throw;
            }
        }

        /// <summary>
        /// Get feedback entries with optional filtering
        /// </summary>
        public async Task<List<FeedbackEntry>> GetFeedbackAsync(int limit = 100, string? feedbackType = null)
        {
            var query = _context.FeedbackEntries.AsQueryable();

            if (!string.IsNullOrEmpty(feedbackType))
            {
                query = query.Where(f => f.FeedbackType == feedbackType);
            }

            return await query
                .OrderByDescending(f => f.Timestamp)
                .Take(Math.Min(limit, 1000)) // Cap at 1000 for performance
                .ToListAsync();
        }

        /// <summary>
        /// Get feedback for specific analysis run
        /// </summary>
        public async Task<List<FeedbackEntry>> GetFeedbackByAnalysisRunAsync(string analysisRunId)
        {
            return await _context.FeedbackEntries
                .Where(f => f.AnalysisRunId == analysisRunId)
                .OrderByDescending(f => f.Timestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Get specific feedback entry by ID
        /// </summary>
        public async Task<FeedbackEntry?> GetFeedbackByIdAsync(int id)
        {
            return await _context.FeedbackEntries.FindAsync(id);
        }

        /// <summary>
        /// Update existing feedback entry
        /// </summary>
        public async Task<bool> UpdateFeedbackAsync(int id, FeedbackSubmissionDto feedback)
        {
            try
            {
                var existingEntry = await _context.FeedbackEntries.FindAsync(id);
                if (existingEntry == null)
                {
                    return false;
                }

                // Update fields
                existingEntry.AnalysisType = feedback.AnalysisType;
                existingEntry.RecommendationType = feedback.RecommendationType;
                existingEntry.Accuracy = feedback.Accuracy;
                existingEntry.Usefulness = feedback.Usefulness;
                existingEntry.Impact = feedback.Impact;
                existingEntry.Implementability = feedback.Implementability;
                existingEntry.Implemented = feedback.Implemented;
                existingEntry.Comments = TruncateText(feedback.Comments, _config.MaxFeedbackLength);
                existingEntry.ProjectName = feedback.ProjectName;
                existingEntry.FilePath = feedback.FilePath;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Feedback {FeedbackId} updated successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback {FeedbackId}", id);
                return false;
            }
        }

        /// <summary>
        /// Delete feedback entry
        /// </summary>
        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            try
            {
                var entry = await _context.FeedbackEntries.FindAsync(id);
                if (entry == null)
                {
                    return false;
                }

                _context.FeedbackEntries.Remove(entry);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Feedback {FeedbackId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback {FeedbackId}", id);
                return false;
            }
        }

        #region Private Methods

        /// <summary>
        /// Validate feedback submission
        /// </summary>
        private void ValidateFeedback(FeedbackSubmissionDto feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback.FeedbackType))
            {
                throw new ArgumentException("Feedback type is required");
            }

            if (feedback.FeedbackType == "analysis")
            {
                if (string.IsNullOrWhiteSpace(feedback.AnalysisType) || 
                    !_config.AllowedAnalysisTypes.Contains(feedback.AnalysisType))
                {
                    throw new ArgumentException("Valid analysis type is required for analysis feedback");
                }
            }
            else if (feedback.FeedbackType == "recommendation")
            {
                if (string.IsNullOrWhiteSpace(feedback.RecommendationType) || 
                    !_config.AllowedRecommendationTypes.Contains(feedback.RecommendationType))
                {
                    throw new ArgumentException("Valid recommendation type is required for recommendation feedback");
                }
            }

            // Validate ratings are in range
            if (feedback.Accuracy.HasValue && (feedback.Accuracy < 1 || feedback.Accuracy > 5))
            {
                throw new ArgumentException("Accuracy rating must be between 1 and 5");
            }

            if (feedback.Usefulness.HasValue && (feedback.Usefulness < 1 || feedback.Usefulness > 5))
            {
                throw new ArgumentException("Usefulness rating must be between 1 and 5");
            }

            if (feedback.Impact.HasValue && (feedback.Impact < 1 || feedback.Impact > 5))
            {
                throw new ArgumentException("Impact rating must be between 1 and 5");
            }
        }

        /// <summary>
        /// Update analytics based on new feedback
        /// </summary>
        private async Task UpdateAnalyticsAsync(FeedbackEntry feedback)
        {
            try
            {
                var analytics = new List<FeedbackAnalytics>();
                var timestamp = DateTime.UtcNow.Date; // Daily aggregation

                // Overall feedback count
                analytics.Add(new FeedbackAnalytics
                {
                    MetricName = "TotalFeedback",
                    Category = "Overall",
                    Value = 1,
                    Count = 1,
                    Timestamp = timestamp
                });

                // Feedback by type
                analytics.Add(new FeedbackAnalytics
                {
                    MetricName = "FeedbackByType",
                    Category = feedback.FeedbackType,
                    Value = 1,
                    Count = 1,
                    Timestamp = timestamp
                });

                // Accuracy metrics
                if (feedback.Accuracy.HasValue)
                {
                    analytics.Add(new FeedbackAnalytics
                    {
                        MetricName = "AccuracyRating",
                        Category = feedback.AnalysisType ?? "Unknown",
                        Value = feedback.Accuracy.Value,
                        Count = 1,
                        Timestamp = timestamp
                    });
                }

                // Usefulness metrics
                if (feedback.Usefulness.HasValue)
                {
                    analytics.Add(new FeedbackAnalytics
                    {
                        MetricName = "UsefulnessRating",
                        Category = feedback.AnalysisType ?? "Unknown",
                        Value = feedback.Usefulness.Value,
                        Count = 1,
                        Timestamp = timestamp
                    });
                }

                // Impact metrics
                if (feedback.Impact.HasValue)
                {
                    analytics.Add(new FeedbackAnalytics
                    {
                        MetricName = "ImpactRating",
                        Category = feedback.RecommendationType ?? "Unknown",
                        Value = feedback.Impact.Value,
                        Count = 1,
                        Timestamp = timestamp
                    });
                }

                // Implementation metrics
                if (!string.IsNullOrEmpty(feedback.Implemented))
                {
                    var implementedValue = feedback.Implemented == "yes" ? 1.0 : 
                                         feedback.Implemented == "partial" ? 0.5 : 0.0;
                    
                    analytics.Add(new FeedbackAnalytics
                    {
                        MetricName = "ImplementationRate",
                        Category = feedback.RecommendationType ?? "Unknown",
                        Value = implementedValue,
                        Count = 1,
                        Timestamp = timestamp
                    });
                }

                // Store analytics
                _context.FeedbackAnalytics.AddRange(analytics);
                await _context.SaveChangesAsync();

                _logger.LogDebug("Analytics updated for feedback {FeedbackId}", feedback.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error updating analytics for feedback {FeedbackId}", feedback.Id);
            }
        }

        /// <summary>
        /// Truncate text to maximum length
        /// </summary>
        private static string? TruncateText(string? text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            {
                return text;
            }

            return text.Substring(0, maxLength - 3) + "...";
        }

        /// <summary>
        /// Generate session ID for tracking
        /// </summary>
        private static string GenerateSessionId()
        {
            return Guid.NewGuid().ToString("N")[..16]; // Short session ID
        }

        #endregion
    }
}
