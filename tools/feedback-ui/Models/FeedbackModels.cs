using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ALARM.FeedbackUI.Models
{
    /// <summary>
    /// Feedback data context for Entity Framework
    /// </summary>
    public class FeedbackDataContext : DbContext
    {
        public FeedbackDataContext(DbContextOptions<FeedbackDataContext> options) : base(options) { }

        public DbSet<FeedbackEntry> FeedbackEntries { get; set; }
        public DbSet<FeedbackAnalytics> FeedbackAnalytics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FeedbackEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FeedbackType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AnalysisType).HasMaxLength(100);
                entity.Property(e => e.RecommendationType).HasMaxLength(100);
                entity.Property(e => e.Comments).HasMaxLength(2000);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.SessionId).HasMaxLength(100);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.FeedbackType);
            });

            modelBuilder.Entity<FeedbackAnalytics>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MetricName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.HasIndex(e => new { e.MetricName, e.Timestamp });
            });
        }
    }

    /// <summary>
    /// Individual feedback entry from users
    /// </summary>
    public class FeedbackEntry
    {
        public int Id { get; set; }
        
        [Required]
        public string FeedbackType { get; set; } = string.Empty; // "analysis" or "recommendation"
        
        public string? AnalysisType { get; set; } // For analysis feedback
        public string? RecommendationType { get; set; } // For recommendation feedback
        
        // Ratings (1-5 scale)
        public int? Accuracy { get; set; }
        public int? Usefulness { get; set; }
        public int? Impact { get; set; }
        
        // Implementation details
        public string? Implementability { get; set; } // "easy", "medium", "hard", "very-hard"
        public string? Implemented { get; set; } // "yes", "partial", "no", "planning"
        
        public string? Comments { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Tracking information
        public string? SessionId { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        
        // Analysis context
        public string? ProjectName { get; set; }
        public string? FilePath { get; set; }
        public string? AnalysisRunId { get; set; }
    }

    /// <summary>
    /// Aggregated analytics for feedback trends
    /// </summary>
    public class FeedbackAnalytics
    {
        public int Id { get; set; }
        
        [Required]
        public string MetricName { get; set; } = string.Empty;
        
        public string? Category { get; set; }
        public double Value { get; set; }
        public int Count { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Additional metadata as JSON string
        public string? Properties { get; set; }
    }

    /// <summary>
    /// DTO for submitting feedback via API
    /// </summary>
    public class FeedbackSubmissionDto
    {
        [Required]
        public string FeedbackType { get; set; } = string.Empty;
        
        public string? AnalysisType { get; set; }
        public string? RecommendationType { get; set; }
        
        public int? Accuracy { get; set; }
        public int? Usefulness { get; set; }
        public int? Impact { get; set; }
        
        public string? Implementability { get; set; }
        public string? Implemented { get; set; }
        
        public string? Comments { get; set; }
        
        // Context information
        public string? ProjectName { get; set; }
        public string? FilePath { get; set; }
        public string? AnalysisRunId { get; set; }
    }

    /// <summary>
    /// Analytics summary for dashboard
    /// </summary>
    public class FeedbackAnalyticsDto
    {
        public int TotalFeedback { get; set; }
        public double AverageAccuracy { get; set; }
        public double AverageUsefulness { get; set; }
        public double AverageImpact { get; set; }
        public double ImplementationRate { get; set; }
        
        public Dictionary<string, int> FeedbackByType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> AccuracyByAnalysisType { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> ImpactByRecommendationType { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, int> ImplementationByDifficulty { get; set; } = new Dictionary<string, int>();
        
        public List<FeedbackTrendDto> AccuracyTrend { get; set; } = new List<FeedbackTrendDto>();
        public List<FeedbackTrendDto> UsageTrend { get; set; } = new List<FeedbackTrendDto>();
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Trend data point for analytics charts
    /// </summary>
    public class FeedbackTrendDto
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public int Count { get; set; }
        public string? Category { get; set; }
    }

    /// <summary>
    /// Learning insights derived from feedback
    /// </summary>
    public class LearningInsightDto
    {
        public string InsightType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public double Impact { get; set; }
        
        public List<string> SupportingEvidence { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Configuration for feedback collection system
    /// </summary>
    public class FeedbackConfiguration
    {
        public bool EnableFeedbackCollection { get; set; } = true;
        public bool EnableAnalytics { get; set; } = true;
        public bool EnableLearningIntegration { get; set; } = true;
        
        public int MaxFeedbackLength { get; set; } = 2000;
        public int AnalyticsRetentionDays { get; set; } = 90;
        public int FeedbackRetentionDays { get; set; } = 365;
        
        public double MinAccuracyThreshold { get; set; } = 3.0;
        public double MinUsefulnessThreshold { get; set; } = 3.0;
        public double MinImpactThreshold { get; set; } = 3.0;
        
        public List<string> AllowedAnalysisTypes { get; set; } = new List<string>
        {
            "pattern-detection",
            "causal-analysis", 
            "risk-assessment",
            "refactoring-suggestions",
            "performance-optimization"
        };
        
        public List<string> AllowedRecommendationTypes { get; set; } = new List<string>
        {
            "refactoring",
            "performance",
            "security",
            "maintainability",
            "architecture"
        };
    }
}
