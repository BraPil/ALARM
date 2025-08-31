using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ALARM.FeedbackUI.Models;
using ALARM.FeedbackUI.Services;

namespace FeedbackUI.Tests.Services
{
    public class FeedbackServiceTests : IDisposable
    {
        private readonly FeedbackDataContext _context;
        private readonly Mock<ILogger<FeedbackService>> _mockLogger;
        private readonly FeedbackService _service;

        public FeedbackServiceTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<FeedbackDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FeedbackDataContext(options);
            _mockLogger = new Mock<ILogger<FeedbackService>>();
            _service = new FeedbackService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task SubmitFeedbackAsync_ValidAnalysisFeedback_ReturnsId()
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                Accuracy = 4,
                Usefulness = 5,
                Comments = "Great analysis!"
            };

            // Act
            var result = await _service.SubmitFeedbackAsync(feedback, "TestAgent", "127.0.0.1");

            // Assert
            Assert.True(result > 0);
            
            var savedFeedback = await _context.FeedbackEntries.FindAsync(result);
            Assert.NotNull(savedFeedback);
            Assert.Equal("analysis", savedFeedback.FeedbackType);
            Assert.Equal("pattern-detection", savedFeedback.AnalysisType);
            Assert.Equal(4, savedFeedback.Accuracy);
            Assert.Equal(5, savedFeedback.Usefulness);
            Assert.Equal("Great analysis!", savedFeedback.Comments);
            Assert.Equal("TestAgent", savedFeedback.UserAgent);
            Assert.Equal("127.0.0.1", savedFeedback.IpAddress);
        }

        [Fact]
        public async Task SubmitFeedbackAsync_ValidRecommendationFeedback_ReturnsId()
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "recommendation",
                RecommendationType = "refactoring",
                Impact = 4,
                Implementability = "medium",
                Implemented = "yes",
                Comments = "Excellent suggestion!"
            };

            // Act
            var result = await _service.SubmitFeedbackAsync(feedback, "TestAgent", "127.0.0.1");

            // Assert
            Assert.True(result > 0);
            
            var savedFeedback = await _context.FeedbackEntries.FindAsync(result);
            Assert.NotNull(savedFeedback);
            Assert.Equal("recommendation", savedFeedback.FeedbackType);
            Assert.Equal("refactoring", savedFeedback.RecommendationType);
            Assert.Equal(4, savedFeedback.Impact);
            Assert.Equal("medium", savedFeedback.Implementability);
            Assert.Equal("yes", savedFeedback.Implemented);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task SubmitFeedbackAsync_InvalidFeedbackType_ThrowsArgumentException(string feedbackType)
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = feedbackType,
                AnalysisType = "pattern-detection"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.SubmitFeedbackAsync(feedback));
        }

        [Fact]
        public async Task SubmitFeedbackAsync_InvalidAnalysisType_ThrowsArgumentException()
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "invalid-type"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.SubmitFeedbackAsync(feedback));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task SubmitFeedbackAsync_InvalidAccuracyRating_ThrowsArgumentException(int accuracy)
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                Accuracy = accuracy
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.SubmitFeedbackAsync(feedback));
        }

        [Fact]
        public async Task GetFeedbackAsync_WithLimit_ReturnsLimitedResults()
        {
            // Arrange
            var feedback1 = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                Comments = "First feedback"
            };
            var feedback2 = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis", 
                AnalysisType = "causal-analysis",
                Comments = "Second feedback"
            };

            await _service.SubmitFeedbackAsync(feedback1);
            await _service.SubmitFeedbackAsync(feedback2);

            // Act
            var results = await _service.GetFeedbackAsync(1);

            // Assert
            Assert.Single(results);
            Assert.Equal("Second feedback", results[0].Comments); // Most recent first
        }

        [Fact]
        public async Task GetFeedbackAsync_WithFeedbackTypeFilter_ReturnsFilteredResults()
        {
            // Arrange
            var analysisFeedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection"
            };
            var recommendationFeedback = new FeedbackSubmissionDto
            {
                FeedbackType = "recommendation",
                RecommendationType = "refactoring"
            };

            await _service.SubmitFeedbackAsync(analysisFeedback);
            await _service.SubmitFeedbackAsync(recommendationFeedback);

            // Act
            var results = await _service.GetFeedbackAsync(10, "analysis");

            // Assert
            Assert.Single(results);
            Assert.Equal("analysis", results[0].FeedbackType);
        }

        [Fact]
        public async Task GetFeedbackByIdAsync_ExistingId_ReturnsFeedback()
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                Comments = "Test feedback"
            };
            var id = await _service.SubmitFeedbackAsync(feedback);

            // Act
            var result = await _service.GetFeedbackByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Test feedback", result.Comments);
        }

        [Fact]
        public async Task GetFeedbackByIdAsync_NonExistentId_ReturnsNull()
        {
            // Act
            var result = await _service.GetFeedbackByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateFeedbackAsync_ExistingFeedback_ReturnsTrue()
        {
            // Arrange
            var originalFeedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                Comments = "Original comment"
            };
            var id = await _service.SubmitFeedbackAsync(originalFeedback);

            var updatedFeedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "causal-analysis",
                Comments = "Updated comment"
            };

            // Act
            var result = await _service.UpdateFeedbackAsync(id, updatedFeedback);

            // Assert
            Assert.True(result);
            
            var savedFeedback = await _context.FeedbackEntries.FindAsync(id);
            Assert.Equal("causal-analysis", savedFeedback!.AnalysisType);
            Assert.Equal("Updated comment", savedFeedback.Comments);
        }

        [Fact]
        public async Task UpdateFeedbackAsync_NonExistentFeedback_ReturnsFalse()
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection"
            };

            // Act
            var result = await _service.UpdateFeedbackAsync(999, feedback);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteFeedbackAsync_ExistingFeedback_ReturnsTrue()
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection"
            };
            var id = await _service.SubmitFeedbackAsync(feedback);

            // Act
            var result = await _service.DeleteFeedbackAsync(id);

            // Assert
            Assert.True(result);
            
            var deletedFeedback = await _context.FeedbackEntries.FindAsync(id);
            Assert.Null(deletedFeedback);
        }

        [Fact]
        public async Task DeleteFeedbackAsync_NonExistentFeedback_ReturnsFalse()
        {
            // Act
            var result = await _service.DeleteFeedbackAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetFeedbackByAnalysisRunAsync_ExistingRunId_ReturnsFeedback()
        {
            // Arrange
            var feedback = new FeedbackSubmissionDto
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                AnalysisRunId = "test-run-123"
            };
            await _service.SubmitFeedbackAsync(feedback);

            // Act
            var results = await _service.GetFeedbackByAnalysisRunAsync("test-run-123");

            // Assert
            Assert.Single(results);
            Assert.Equal("test-run-123", results[0].AnalysisRunId);
        }

        [Fact]
        public void Constructor_NullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new FeedbackService(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new FeedbackService(_context, null!));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
