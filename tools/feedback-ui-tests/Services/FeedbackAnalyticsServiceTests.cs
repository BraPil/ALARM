using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ALARM.FeedbackUI.Models;
using ALARM.FeedbackUI.Services;

namespace FeedbackUI.Tests.Services
{
    public class FeedbackAnalyticsServiceTests : IDisposable
    {
        private readonly FeedbackDataContext _context;
        private readonly Mock<ILogger<FeedbackAnalyticsService>> _mockLogger;
        private readonly FeedbackAnalyticsService _service;

        public FeedbackAnalyticsServiceTests()
        {
            var options = new DbContextOptionsBuilder<FeedbackDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FeedbackDataContext(options);
            _mockLogger = new Mock<ILogger<FeedbackAnalyticsService>>();
            _service = new FeedbackAnalyticsService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAnalyticsSummaryAsync_NoData_ReturnsEmptyAnalytics()
        {
            // Act
            var result = await _service.GetAnalyticsSummaryAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalFeedback);
            Assert.Equal(0, result.AverageAccuracy);
            Assert.Equal(0, result.AverageUsefulness);
            Assert.Equal(0, result.AverageImpact);
            Assert.Equal(0, result.ImplementationRate);
            Assert.Empty(result.FeedbackByType);
        }

        [Fact]
        public async Task GetAnalyticsSummaryAsync_WithData_ReturnsCorrectAnalytics()
        {
            // Arrange
            var feedback1 = new FeedbackEntry
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                Accuracy = 4,
                Usefulness = 5,
                Timestamp = DateTime.UtcNow.AddDays(-1)
            };

            var feedback2 = new FeedbackEntry
            {
                FeedbackType = "recommendation",
                RecommendationType = "refactoring",
                Impact = 3,
                Implemented = "yes",
                Timestamp = DateTime.UtcNow.AddDays(-2)
            };

            _context.FeedbackEntries.AddRange(feedback1, feedback2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAnalyticsSummaryAsync();

            // Assert
            Assert.Equal(2, result.TotalFeedback);
            Assert.Equal(4.0, result.AverageAccuracy);
            Assert.Equal(5.0, result.AverageUsefulness);
            Assert.Equal(3.0, result.AverageImpact);
            Assert.Equal(100.0, result.ImplementationRate); // 1 out of 1 implemented
            Assert.Equal(2, result.FeedbackByType.Count);
            Assert.Equal(1, result.FeedbackByType["analysis"]);
            Assert.Equal(1, result.FeedbackByType["recommendation"]);
        }

        [Fact]
        public async Task GetAnalyticsSummaryAsync_WithDaysFilter_ReturnsFilteredData()
        {
            // Arrange
            var recentFeedback = new FeedbackEntry
            {
                FeedbackType = "analysis",
                Accuracy = 4,
                Timestamp = DateTime.UtcNow.AddDays(-1)
            };

            var oldFeedback = new FeedbackEntry
            {
                FeedbackType = "analysis",
                Accuracy = 2,
                Timestamp = DateTime.UtcNow.AddDays(-10)
            };

            _context.FeedbackEntries.AddRange(recentFeedback, oldFeedback);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAnalyticsSummaryAsync(5); // Last 5 days

            // Assert
            Assert.Equal(1, result.TotalFeedback);
            Assert.Equal(4.0, result.AverageAccuracy);
        }

        [Fact]
        public async Task GetAccuracyTrendAsync_ReturnsCorrectTrend()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var todayFeedback = new FeedbackEntry
            {
                FeedbackType = "analysis",
                Accuracy = 5,
                Timestamp = today.AddHours(12)
            };

            var yesterdayFeedback = new FeedbackEntry
            {
                FeedbackType = "analysis", 
                Accuracy = 3,
                Timestamp = yesterday.AddHours(12)
            };

            _context.FeedbackEntries.AddRange(todayFeedback, yesterdayFeedback);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAccuracyTrendAsync(7);

            // Assert
            Assert.Equal(2, result.Count);
            
            var todayTrend = result.First(r => r.Date == today);
            var yesterdayTrend = result.First(r => r.Date == yesterday);
            
            Assert.Equal(5.0, todayTrend.Value);
            Assert.Equal(1, todayTrend.Count);
            Assert.Equal(3.0, yesterdayTrend.Value);
            Assert.Equal(1, yesterdayTrend.Count);
        }

        [Fact]
        public async Task GetUsageTrendAsync_ReturnsCorrectTrend()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var feedback1 = new FeedbackEntry
            {
                FeedbackType = "analysis",
                Timestamp = today.AddHours(10)
            };
            var feedback2 = new FeedbackEntry
            {
                FeedbackType = "recommendation",
                Timestamp = today.AddHours(14)
            };

            _context.FeedbackEntries.AddRange(feedback1, feedback2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetUsageTrendAsync(7);

            // Assert
            Assert.Single(result);
            var todayUsage = result.First();
            Assert.Equal(today, todayUsage.Date);
            Assert.Equal(2.0, todayUsage.Value);
            Assert.Equal(2, todayUsage.Count);
        }

        [Fact]
        public async Task GenerateLearningInsightsAsync_NoData_ReturnsEmptyInsights()
        {
            // Act
            var result = await _service.GenerateLearningInsightsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GenerateLearningInsightsAsync_LowAccuracyData_ReturnsLowAccuracyInsight()
        {
            // Arrange - Create feedback with low accuracy
            var lowAccuracyFeedback = new List<FeedbackEntry>();
            for (int i = 0; i < 5; i++)
            {
                lowAccuracyFeedback.Add(new FeedbackEntry
                {
                    FeedbackType = "analysis",
                    AnalysisType = "pattern-detection",
                    Accuracy = 2, // Low accuracy
                    Timestamp = DateTime.UtcNow.AddDays(-i)
                });
            }

            _context.FeedbackEntries.AddRange(lowAccuracyFeedback);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GenerateLearningInsightsAsync();

            // Assert
            Assert.NotEmpty(result);
            var lowAccuracyInsight = result.FirstOrDefault(i => i.InsightType == "LowAccuracy");
            Assert.NotNull(lowAccuracyInsight);
            Assert.Contains("pattern-detection", lowAccuracyInsight.Title);
            Assert.True(lowAccuracyInsight.Impact > 0);
            Assert.NotEmpty(lowAccuracyInsight.Recommendations);
        }

        [Fact]
        public async Task GenerateLearningInsightsAsync_HighImpactLowImplementation_ReturnsImplementationInsight()
        {
            // Arrange - Create high-impact recommendations that aren't implemented
            var highImpactFeedback = new List<FeedbackEntry>();
            for (int i = 0; i < 5; i++)
            {
                highImpactFeedback.Add(new FeedbackEntry
                {
                    FeedbackType = "recommendation",
                    RecommendationType = "refactoring",
                    Impact = 5, // High impact
                    Implemented = "no", // Not implemented
                    Timestamp = DateTime.UtcNow.AddDays(-i)
                });
            }

            _context.FeedbackEntries.AddRange(highImpactFeedback);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GenerateLearningInsightsAsync();

            // Assert
            Assert.NotEmpty(result);
            var implementationInsight = result.FirstOrDefault(i => i.InsightType == "LowImplementation");
            Assert.NotNull(implementationInsight);
            Assert.Contains("refactoring", implementationInsight.Title);
            Assert.NotEmpty(implementationInsight.SupportingEvidence);
        }

        [Fact]
        public async Task GetDetailedAnalyticsAsync_AccuracyCategory_ReturnsAccuracyDetails()
        {
            // Arrange
            var feedback = new FeedbackEntry
            {
                FeedbackType = "analysis",
                AnalysisType = "pattern-detection",
                Accuracy = 4,
                Timestamp = DateTime.UtcNow
            };

            _context.FeedbackEntries.Add(feedback);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetDetailedAnalyticsAsync("accuracy");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("TotalRatings"));
            Assert.True(result.ContainsKey("AverageRating"));
            Assert.True(result.ContainsKey("ByAnalysisType"));
            Assert.Equal(1, result["TotalRatings"]);
            Assert.Equal(4.0, result["AverageRating"]);
        }

        [Fact]
        public async Task GetDetailedAnalyticsAsync_ImplementationCategory_ReturnsImplementationDetails()
        {
            // Arrange
            var feedback = new FeedbackEntry
            {
                FeedbackType = "recommendation",
                Implementability = "easy",
                Implemented = "yes",
                Timestamp = DateTime.UtcNow
            };

            _context.FeedbackEntries.Add(feedback);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetDetailedAnalyticsAsync("implementation");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("TotalResponses"));
            Assert.True(result.ContainsKey("ImplementationRate"));
            Assert.True(result.ContainsKey("ByStatus"));
            Assert.True(result.ContainsKey("ByDifficulty"));
        }

        [Fact]
        public async Task GetDetailedAnalyticsAsync_UnknownCategory_ReturnsError()
        {
            // Act
            var result = await _service.GetDetailedAnalyticsAsync("unknown");

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("Error"));
            Assert.Equal("Unknown category", result["Error"]);
        }

        [Fact]
        public void Constructor_NullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new FeedbackAnalyticsService(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new FeedbackAnalyticsService(_context, null!));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
