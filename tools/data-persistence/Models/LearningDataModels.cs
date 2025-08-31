using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ALARM.DataPersistence.Models;

// Core entities for learning data storage
[Table("runs")]
public class RunEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string RunId { get; set; } = "";
    
    [Required]
    public DateTime Timestamp { get; set; }
    
    [MaxLength(50)]
    public string ProjectName { get; set; } = "";
    
    [MaxLength(20)]
    public string Environment { get; set; } = "";
    
    public bool Success { get; set; }
    
    public double Duration { get; set; } // in minutes
    
    [Column(TypeName = "nvarchar(max)")]
    public string IndexDataJson { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string RiskAssessmentJson { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string TestResultsJson { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string MetricsJson { get; set; } = "";
    
    // Navigation properties
    public virtual ICollection<PatternEntity> Patterns { get; set; } = new List<PatternEntity>();
    public virtual ICollection<ImprovementEntity> Improvements { get; set; } = new List<ImprovementEntity>();
    public virtual ICollection<PerformanceMetricEntity> PerformanceMetrics { get; set; } = new List<PerformanceMetricEntity>();
}

[Table("patterns")]
public class PatternEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int RunId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string PatternType { get; set; } = ""; // Success, Failure, AntiPattern, GoodPattern
    
    [Required]
    [MaxLength(200)]
    public string PatternName { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string Description { get; set; } = "";
    
    [MaxLength(500)]
    public string FilePath { get; set; } = "";
    
    public double Confidence { get; set; }
    
    public int Frequency { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string ContextJson { get; set; } = "";
    
    public DateTime DetectedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("RunId")]
    public virtual RunEntity Run { get; set; } = null!;
}

[Table("improvements")]
public class ImprovementEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int RunId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = ""; // MLRecommendation, ProcessImprovement, etc.
    
    [Required]
    [MaxLength(20)]
    public string Priority { get; set; } = ""; // Critical, High, Medium, Low
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Description { get; set; } = "";
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Recommendation { get; set; } = "";
    
    [MaxLength(20)]
    public string EstimatedEffort { get; set; } = "";
    
    public double Confidence { get; set; }
    
    public bool Applied { get; set; }
    
    public DateTime? AppliedAt { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string AppliedResult { get; set; } = "";
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("RunId")]
    public virtual RunEntity Run { get; set; } = null!;
}

[Table("performance_metrics")]
public class PerformanceMetricEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int RunId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string MetricName { get; set; } = "";
    
    [Required]
    public double Value { get; set; }
    
    [MaxLength(20)]
    public string Unit { get; set; } = "";
    
    [MaxLength(50)]
    public string Category { get; set; } = ""; // Performance, Quality, Coverage, etc.
    
    public DateTime MeasuredAt { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string ContextJson { get; set; } = "";
    
    // Navigation properties
    [ForeignKey("RunId")]
    public virtual RunEntity Run { get; set; } = null!;
}

[Table("ml_models")]
public class MLModelEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ModelName { get; set; } = "";
    
    [Required]
    [MaxLength(50)]
    public string ModelType { get; set; } = ""; // Classification, Regression, Clustering, etc.
    
    [Required]
    public byte[] ModelData { get; set; } = Array.Empty<byte>();
    
    [Required]
    public DateTime TrainedAt { get; set; }
    
    public double Accuracy { get; set; }
    
    public int TrainingDataCount { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string FeatureNames { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string HyperParameters { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string ValidationMetrics { get; set; } = "";
    
    public bool IsActive { get; set; }
    
    public DateTime? DeactivatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<PredictionEntity> Predictions { get; set; } = new List<PredictionEntity>();
}

[Table("predictions")]
public class PredictionEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ModelId { get; set; }
    
    [Required]
    public int RunId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string PredictionType { get; set; } = ""; // Success, Performance, Risk, etc.
    
    [Required]
    public double PredictedValue { get; set; }
    
    public double? ActualValue { get; set; } // For validation
    
    public double Confidence { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string InputFeatures { get; set; } = "";
    
    public DateTime PredictedAt { get; set; }
    
    public DateTime? ValidatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("ModelId")]
    public virtual MLModelEntity Model { get; set; } = null!;
    
    [ForeignKey("RunId")]
    public virtual RunEntity Run { get; set; } = null!;
}

[Table("feedback")]
public class FeedbackEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int RunId { get; set; }
    
    [MaxLength(50)]
    public string FeedbackType { get; set; } = ""; // Satisfaction, Correction, Suggestion
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = "";
    
    public int Rating { get; set; } // 1-5 scale
    
    [MaxLength(100)]
    public string UserRole { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string ContextJson { get; set; } = "";
    
    public bool Processed { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("RunId")]
    public virtual RunEntity Run { get; set; } = null!;
}

[Table("protocol_versions")]
public class ProtocolVersionEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ProtocolName { get; set; } = "";
    
    [Required]
    [MaxLength(20)]
    public string Version { get; set; } = "";
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string ChangeLog { get; set; } = "";
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? ActivatedAt { get; set; }
    
    public DateTime? DeactivatedAt { get; set; }
    
    [MaxLength(200)]
    public string CreatedBy { get; set; } = "";
    
    // Navigation properties
    public virtual ICollection<ProtocolUpdateEntity> Updates { get; set; } = new List<ProtocolUpdateEntity>();
}

[Table("protocol_updates")]
public class ProtocolUpdateEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ProtocolVersionId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string UpdateType { get; set; } = ""; // Enhancement, Replacement, etc.
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string Description { get; set; } = "";
    
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string ProposedChange { get; set; } = "";
    
    [Column(TypeName = "nvarchar(max)")]
    public string Justification { get; set; } = "";
    
    public double Confidence { get; set; }
    
    [MaxLength(20)]
    public string Status { get; set; } = ""; // Pending, Applied, Rejected
    
    public bool Applied { get; set; }
    
    public DateTime? AppliedAt { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string AppliedResult { get; set; } = "";
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("ProtocolVersionId")]
    public virtual ProtocolVersionEntity ProtocolVersion { get; set; } = null!;
}

// Data Transfer Objects for API and service layer
public class RunDataDto
{
    public string RunId { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string ProjectName { get; set; } = "";
    public string Environment { get; set; } = "";
    public bool Success { get; set; }
    public double Duration { get; set; }
    public Dictionary<string, object> IndexData { get; set; } = new();
    public Dictionary<string, object> RiskAssessment { get; set; } = new();
    public Dictionary<string, Dictionary<string, object>> TestResults { get; set; } = new();
    public Dictionary<string, double> Metrics { get; set; } = new();
}

public class PatternDto
{
    public string PatternType { get; set; } = "";
    public string PatternName { get; set; } = "";
    public string Description { get; set; } = "";
    public string FilePath { get; set; } = "";
    public double Confidence { get; set; }
    public int Frequency { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

public class ImprovementDto
{
    public string Type { get; set; } = "";
    public string Priority { get; set; } = "";
    public string Description { get; set; } = "";
    public string Recommendation { get; set; } = "";
    public string EstimatedEffort { get; set; } = "";
    public double Confidence { get; set; }
    public bool Applied { get; set; }
    public DateTime? AppliedAt { get; set; }
    public string AppliedResult { get; set; } = "";
}

public class PerformanceMetricDto
{
    public string MetricName { get; set; } = "";
    public double Value { get; set; }
    public string Unit { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime MeasuredAt { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

public class FeedbackDto
{
    public string FeedbackType { get; set; } = "";
    public string Content { get; set; } = "";
    public int Rating { get; set; }
    public string UserRole { get; set; } = "";
    public Dictionary<string, object> Context { get; set; } = new();
}
