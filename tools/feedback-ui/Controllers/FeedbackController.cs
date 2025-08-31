using Microsoft.AspNetCore.Mvc;
using ALARM.FeedbackUI.Models;
using ALARM.FeedbackUI.Services;

namespace ALARM.FeedbackUI.Controllers
{
    /// <summary>
    /// API controller for feedback collection and management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IFeedbackAnalyticsService _analyticsService;
        private readonly ILearningIntegrationService _learningService;
        private readonly FeedbackBackgroundService _backgroundService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            IFeedbackService feedbackService,
            IFeedbackAnalyticsService analyticsService,
            ILearningIntegrationService learningService,
            FeedbackBackgroundService backgroundService,
            ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
            _learningService = learningService ?? throw new ArgumentNullException(nameof(learningService));
            _backgroundService = backgroundService ?? throw new ArgumentNullException(nameof(backgroundService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Submit new feedback
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> SubmitFeedback([FromBody] FeedbackSubmissionDto feedback)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userAgent = Request.Headers.UserAgent.ToString();
                var ipAddress = GetClientIpAddress();

                var feedbackId = await _feedbackService.SubmitFeedbackAsync(feedback, userAgent, ipAddress);

                // Queue feedback for background processing with proper DI scoping
                await _backgroundService.QueueFeedbackForProcessingAsync(feedbackId);

                return Ok(new { Id = feedbackId, Message = "Feedback submitted successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid feedback submission");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get feedback entries
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<FeedbackEntry>>> GetFeedback(
            [FromQuery] int limit = 50,
            [FromQuery] string? feedbackType = null)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackAsync(limit, feedbackType);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get feedback by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackEntry>> GetFeedbackById(int id)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
                if (feedback == null)
                {
                    return NotFound();
                }

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback {FeedbackId}", id);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get feedback for specific analysis run
        /// </summary>
        [HttpGet("analysis-run/{analysisRunId}")]
        public async Task<ActionResult<List<FeedbackEntry>>> GetFeedbackByAnalysisRun(string analysisRunId)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByAnalysisRunAsync(analysisRunId);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving feedback for analysis run {AnalysisRunId}", analysisRunId);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update existing feedback
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFeedback(int id, [FromBody] FeedbackSubmissionDto feedback)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _feedbackService.UpdateFeedbackAsync(id, feedback);
                if (!success)
                {
                    return NotFound();
                }

                return Ok(new { Message = "Feedback updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid feedback update for {FeedbackId}", id);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback {FeedbackId}", id);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Delete feedback
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFeedback(int id)
        {
            try
            {
                var success = await _feedbackService.DeleteFeedbackAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return Ok(new { Message = "Feedback deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback {FeedbackId}", id);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get analytics summary
        /// </summary>
        [HttpGet("analytics")]
        public async Task<ActionResult<FeedbackAnalyticsDto>> GetAnalytics([FromQuery] int days = 30)
        {
            try
            {
                var analytics = await _analyticsService.GetAnalyticsSummaryAsync(days);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving analytics");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get detailed analytics for specific category
        /// </summary>
        [HttpGet("analytics/{category}")]
        public async Task<ActionResult<Dictionary<string, object>>> GetDetailedAnalytics(
            string category, 
            [FromQuery] int days = 30)
        {
            try
            {
                var analytics = await _analyticsService.GetDetailedAnalyticsAsync(category, days);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving detailed analytics for category {Category}", category);
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get accuracy trend data
        /// </summary>
        [HttpGet("analytics/trends/accuracy")]
        public async Task<ActionResult<List<FeedbackTrendDto>>> GetAccuracyTrend([FromQuery] int days = 30)
        {
            try
            {
                var trend = await _analyticsService.GetAccuracyTrendAsync(days);
                return Ok(trend);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving accuracy trend");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get usage trend data
        /// </summary>
        [HttpGet("analytics/trends/usage")]
        public async Task<ActionResult<List<FeedbackTrendDto>>> GetUsageTrend([FromQuery] int days = 30)
        {
            try
            {
                var trend = await _analyticsService.GetUsageTrendAsync(days);
                return Ok(trend);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving usage trend");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get learning insights
        /// </summary>
        [HttpGet("insights")]
        public async Task<ActionResult<List<LearningInsightDto>>> GetLearningInsights()
        {
            try
            {
                var insights = await _learningService.GenerateActionableInsightsAsync();
                return Ok(insights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving learning insights");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get learning metrics
        /// </summary>
        [HttpGet("learning/metrics")]
        public async Task<ActionResult<Dictionary<string, double>>> GetLearningMetrics()
        {
            try
            {
                var metrics = await _learningService.GetLearningMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving learning metrics");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Trigger ML model update based on feedback
        /// </summary>
        [HttpPost("learning/update-model")]
        public async Task<ActionResult> UpdateMLModel()
        {
            try
            {
                _ = Task.Run(async () => await _learningService.UpdateMLModelBasedOnFeedbackAsync());
                return Ok(new { Message = "ML model update triggered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering ML model update");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            return Ok(new 
            { 
                Status = "Healthy", 
                Timestamp = DateTime.UtcNow,
                Service = "ALARM Feedback Collection API",
                Version = "1.0.0"
            });
        }

        #region Private Methods

        /// <summary>
        /// Get client IP address from request
        /// </summary>
        private string? GetClientIpAddress()
        {
            try
            {
                // Check for forwarded IP first
                var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                // Check for real IP
                var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp;
                }

                // Fall back to connection remote IP
                return HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting client IP address");
                return null;
            }
        }

        #endregion
    }
}
