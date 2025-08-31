using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ALARM.Analyzers.PatternDetection;

namespace ALARM.DomainLibraries
{
    /// <summary>
    /// Interface for domain-specific pattern detection and rule libraries
    /// Provides pluggable architecture for AutoCAD, Oracle, and other domain expertise
    /// </summary>
    public interface IDomainLibrary
    {
        /// <summary>
        /// Domain identifier (e.g., "AutoCAD", "Oracle", "SQLServer")
        /// </summary>
        string DomainName { get; }

        /// <summary>
        /// Version of the domain library
        /// </summary>
        string Version { get; }

        /// <summary>
        /// List of supported pattern categories
        /// </summary>
        IReadOnlyList<string> SupportedPatternCategories { get; }

        /// <summary>
        /// Detect domain-specific patterns in the provided data
        /// </summary>
        Task<DomainPatternResult> DetectPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config);

        /// <summary>
        /// Validate code/configuration against domain-specific rules
        /// </summary>
        Task<DomainValidationResult> ValidateAsync(
            string content,
            string contentType,
            DomainValidationConfig config);

        /// <summary>
        /// Extract domain-specific features from pattern data
        /// </summary>
        Task<DomainFeatureResult> ExtractFeaturesAsync(
            IEnumerable<PatternData> data,
            DomainFeatureConfig config);

        /// <summary>
        /// Get optimization suggestions based on detected patterns
        /// </summary>
        Task<DomainOptimizationResult> GetOptimizationSuggestionsAsync(
            DomainPatternResult patternResult,
            DomainOptimizationConfig config);

        /// <summary>
        /// Get migration recommendations for legacy patterns
        /// </summary>
        Task<DomainMigrationResult> GetMigrationRecommendationsAsync(
            string legacyPattern,
            string targetVersion,
            DomainMigrationConfig config);
    }

    #region Domain-Specific Result Models

    /// <summary>
    /// Result of domain-specific pattern detection
    /// </summary>
    public class DomainPatternResult
    {
        public string DomainName { get; set; } = string.Empty;
        public DateTime DetectionTimestamp { get; set; }
        public List<DomainPattern> DetectedPatterns { get; set; } = new();
        public List<DomainAnomaly> DetectedAnomalies { get; set; } = new();
        public Dictionary<string, double> PatternConfidenceScores { get; set; } = new();
        public Dictionary<string, object> DomainSpecificMetrics { get; set; } = new();
    }

    /// <summary>
    /// Domain-specific pattern with classification and metadata
    /// </summary>
    public class DomainPattern
    {
        public string PatternId { get; set; } = string.Empty;
        public string PatternName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public PatternSeverity Severity { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
        public List<string> AffectedComponents { get; set; } = new();
        public string RecommendedAction { get; set; } = string.Empty;
    }

    /// <summary>
    /// Domain-specific anomaly detection
    /// </summary>
    public class DomainAnomaly
    {
        public string AnomalyId { get; set; } = string.Empty;
        public string AnomalyType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double AnomalyScore { get; set; }
        public AnomalySeverity Severity { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
        public string RecommendedInvestigation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Result of domain-specific validation
    /// </summary>
    public class DomainValidationResult
    {
        public bool IsValid { get; set; }
        public List<DomainValidationIssue> Issues { get; set; } = new();
        public List<DomainValidationWarning> Warnings { get; set; } = new();
        public Dictionary<string, double> QualityMetrics { get; set; } = new();
        public string ValidationSummary { get; set; } = string.Empty;
    }

    /// <summary>
    /// Domain-specific validation issue
    /// </summary>
    public class DomainValidationIssue
    {
        public string IssueId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IssueSeverity Severity { get; set; }
        public string Location { get; set; } = string.Empty;
        public string SuggestedFix { get; set; } = string.Empty;
        public Dictionary<string, object> Context { get; set; } = new();
    }

    /// <summary>
    /// Domain-specific validation warning
    /// </summary>
    public class DomainValidationWarning
    {
        public string WarningId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public Dictionary<string, object> Context { get; set; } = new();
    }

    /// <summary>
    /// Result of domain-specific feature extraction
    /// </summary>
    public class DomainFeatureResult
    {
        public List<DomainFeature> ExtractedFeatures { get; set; } = new();
        public Dictionary<string, double> FeatureImportanceScores { get; set; } = new();
        public Dictionary<string, object> DomainMetrics { get; set; } = new();
    }

    /// <summary>
    /// Domain-specific feature
    /// </summary>
    public class DomainFeature
    {
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureType { get; set; } = string.Empty;
        public double FeatureValue { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Result of domain-specific optimization analysis
    /// </summary>
    public class DomainOptimizationResult
    {
        public List<DomainOptimizationSuggestion> Suggestions { get; set; } = new();
        public Dictionary<string, double> ExpectedImprovements { get; set; } = new();
        public Dictionary<string, object> OptimizationMetrics { get; set; } = new();
    }

    /// <summary>
    /// Domain-specific optimization suggestion
    /// </summary>
    public class DomainOptimizationSuggestion
    {
        public string SuggestionId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public OptimizationCategory Category { get; set; }
        public OptimizationPriority Priority { get; set; }
        public double ExpectedImprovement { get; set; }
        public string Implementation { get; set; } = string.Empty;
        public List<string> Prerequisites { get; set; } = new();
        public Dictionary<string, object> Context { get; set; } = new();
    }

    /// <summary>
    /// Result of domain-specific migration analysis
    /// </summary>
    public class DomainMigrationResult
    {
        public List<DomainMigrationRecommendation> Recommendations { get; set; } = new();
        public Dictionary<string, double> MigrationComplexityScores { get; set; } = new();
        public Dictionary<string, object> MigrationMetrics { get; set; } = new();
    }

    /// <summary>
    /// Domain-specific migration recommendation
    /// </summary>
    public class DomainMigrationRecommendation
    {
        public string RecommendationId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MigrationCategory Category { get; set; }
        public MigrationComplexity Complexity { get; set; }
        public string LegacyPattern { get; set; } = string.Empty;
        public string ModernPattern { get; set; } = string.Empty;
        public List<string> MigrationSteps { get; set; } = new();
        public Dictionary<string, object> Context { get; set; } = new();
    }

    #endregion

    #region Configuration Models

    /// <summary>
    /// Configuration for domain-specific validation
    /// </summary>
    public class DomainValidationConfig
    {
        public List<string> EnabledRules { get; set; } = new();
        public Dictionary<string, object> RuleParameters { get; set; } = new();
        public bool StrictMode { get; set; } = false;
        public string TargetVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration for domain-specific feature extraction
    /// </summary>
    public class DomainFeatureConfig
    {
        public List<string> FeatureCategories { get; set; } = new();
        public Dictionary<string, double> FeatureWeights { get; set; } = new();
        public bool IncludeAdvancedFeatures { get; set; } = true;
    }

    /// <summary>
    /// Configuration for domain-specific optimization
    /// </summary>
    public class DomainOptimizationConfig
    {
        public List<OptimizationCategory> TargetCategories { get; set; } = new();
        public OptimizationPriority MinimumPriority { get; set; } = OptimizationPriority.Low;
        public double MinimumExpectedImprovement { get; set; } = 0.1;
    }

    /// <summary>
    /// Configuration for domain-specific migration analysis
    /// </summary>
    public class DomainMigrationConfig
    {
        public string SourceVersion { get; set; } = string.Empty;
        public string TargetVersion { get; set; } = string.Empty;
        public List<MigrationCategory> TargetCategories { get; set; } = new();
        public MigrationComplexity MaxComplexity { get; set; } = MigrationComplexity.High;
    }

    #endregion

    #region Enumerations

    public enum PatternSeverity
    {
        Info,
        Low,
        Medium,
        High,
        Critical
    }

    public enum AnomalySeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum IssueSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public enum OptimizationCategory
    {
        Performance,
        Memory,
        Security,
        Maintainability,
        Scalability,
        Reliability
    }

    public enum OptimizationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum MigrationCategory
    {
        API,
        Configuration,
        Performance,
        Security,
        Architecture,
        Dependencies
    }

    public enum MigrationComplexity
    {
        Low,
        Medium,
        High,
        VeryHigh
    }

    #endregion
}
