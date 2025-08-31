using ALARM.FeedbackUI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace ALARM.FeedbackUI.Services
{
    /// <summary>
    /// Background service for processing feedback analytics with proper DI scoping
    /// </summary>
    public class FeedbackBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FeedbackBackgroundService> _logger;
        private readonly Channel<int> _feedbackQueue;

        public FeedbackBackgroundService(IServiceProvider serviceProvider, ILogger<FeedbackBackgroundService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            // Create bounded channel for feedback processing queue
            var options = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };
            _feedbackQueue = Channel.CreateBounded<int>(options);
        }

        /// <summary>
        /// Queue feedback for background processing
        /// </summary>
        public async Task QueueFeedbackForProcessingAsync(int feedbackId)
        {
            try
            {
                await _feedbackQueue.Writer.WriteAsync(feedbackId);
                _logger.LogDebug("Queued feedback {FeedbackId} for background processing", feedbackId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to queue feedback {FeedbackId} for processing", feedbackId);
            }
        }

        /// <summary>
        /// Background processing loop
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Feedback background service started");

            try
            {
                await foreach (var feedbackId in _feedbackQueue.Reader.ReadAllAsync(stoppingToken))
                {
                    await ProcessFeedbackAsync(feedbackId, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Feedback background service stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in feedback background service");
            }
        }

        /// <summary>
        /// Process individual feedback with proper scoping
        /// </summary>
        private async Task ProcessFeedbackAsync(int feedbackId, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var feedbackService = scope.ServiceProvider.GetRequiredService<IFeedbackService>();
                var learningService = scope.ServiceProvider.GetRequiredService<ILearningIntegrationService>();

                _logger.LogDebug("Processing feedback {FeedbackId} in background", feedbackId);

                // Get feedback entry
                var feedbackEntry = await feedbackService.GetFeedbackByIdAsync(feedbackId);
                if (feedbackEntry == null)
                {
                    _logger.LogWarning("Feedback {FeedbackId} not found for background processing", feedbackId);
                    return;
                }

                // Process analytics (this would need to be moved to a separate method with proper context)
                await ProcessFeedbackAnalyticsAsync(feedbackEntry, scope);

                // Process learning integration
                await learningService.ProcessFeedbackForLearningAsync(feedbackEntry);

                _logger.LogDebug("Successfully processed feedback {FeedbackId} in background", feedbackId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing feedback {FeedbackId} in background", feedbackId);
            }
        }

        /// <summary>
        /// Process feedback analytics with proper context scoping
        /// </summary>
        private async Task ProcessFeedbackAnalyticsAsync(FeedbackEntry feedback, IServiceScope scope)
        {
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<FeedbackDataContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<FeedbackBackgroundService>>();

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

                // Store analytics with proper context
                context.FeedbackAnalytics.AddRange(analytics);
                await context.SaveChangesAsync();

                logger.LogDebug("Analytics processed for feedback {FeedbackId}", feedback.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error processing analytics for feedback {FeedbackId}", feedback.Id);
            }
        }

        /// <summary>
        /// Cleanup on disposal
        /// </summary>
        public override void Dispose()
        {
            _feedbackQueue.Writer.Complete();
            base.Dispose();
        }
    }

    /// <summary>
    /// Extension methods for registering background service
    /// </summary>
    public static class FeedbackBackgroundServiceExtensions
    {
        /// <summary>
        /// Add feedback background service to DI container
        /// </summary>
        public static IServiceCollection AddFeedbackBackgroundService(this IServiceCollection services)
        {
            services.AddSingleton<FeedbackBackgroundService>();
            services.AddHostedService<FeedbackBackgroundService>(provider => provider.GetRequiredService<FeedbackBackgroundService>());
            return services;
        }
    }
}
