using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Pattern validation result for ADDS migration pattern detection
    /// </summary>
    public class PatternValidationResult
    {
        public double OverallQualityScore { get; set; }
        public List<DetectedPattern> DetectedPatterns { get; set; } = new();
        public Dictionary<string, double> PatternQualityBreakdown { get; set; } = new();
        public PatternAccuracyMetrics AccuracyMetrics { get; set; } = new();
        public ADDSMigrationCompliance MigrationCompliance { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public double Confidence { get; set; }
        public DateTime ValidationTimestamp { get; set; }
    }

    /// <summary>
    /// Detected migration pattern with confidence and metadata
    /// </summary>
    public class DetectedPattern
    {
        public PatternType PatternType { get; set; }
        public string PatternName { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public List<string> MatchedKeywords { get; set; } = new();
        public PatternComplexityLevel ComplexityLevel { get; set; }
        public double MigrationRelevance { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Pattern definition for migration pattern detection
    /// </summary>
    public class PatternDefinition
    {
        public string Name { get; set; } = string.Empty;
        public PatternType PatternType { get; set; }
        public Dictionary<string, double> Keywords { get; set; } = new();
        public List<string> RegexPatterns { get; set; } = new();
        public double MinimumConfidence { get; set; } = 0.3;
        public string Description { get; set; } = string.Empty;
        public List<string> Examples { get; set; } = new();
    }

    /// <summary>
    /// Pattern validation rule for quality assessment
    /// </summary>
    public class PatternRule
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Func<string, List<DetectedPattern>, bool> Validator { get; set; }
        public double Weight { get; set; } = 1.0;
        public bool IsCritical { get; set; } = false;
    }

    /// <summary>
    /// Accuracy metrics for pattern detection validation
    /// </summary>
    public class PatternAccuracyMetrics
    {
        public double PatternDetectionAccuracy { get; set; }
        public double ClassificationConfidence { get; set; }
        public double FeatureAlignmentScore { get; set; }
        public double PrecisionScore { get; set; }
        public double RecallScore { get; set; }
        public double F1Score { get; set; }
        public Dictionary<PatternType, double> PatternTypeAccuracy { get; set; } = new();
    }

    /// <summary>
    /// ADDS migration compliance assessment
    /// Prime Directive: 100% functionality preservation for .NET Core 8, AutoCAD Map3D 2025, Oracle 19c
    /// </summary>
    public class ADDSMigrationCompliance
    {
        public double DotNetCore8Compliance { get; set; }
        public double Map3D2025Compliance { get; set; }
        public double Oracle19cCompliance { get; set; }
        public double FunctionalityPreservation { get; set; }
        public double BackwardCompatibility { get; set; }
        public double MigrationPathValidity { get; set; }
        public double OverallComplianceScore { get; set; }
        public List<string> ComplianceIssues { get; set; } = new();
        public List<string> ComplianceRecommendations { get; set; } = new();
    }

    /// <summary>
    /// ADDS migration context for pattern validation
    /// </summary>
    public class ADDSMigrationContext
    {
        public bool RequiresLauncherMigration { get; set; } = true;
        public bool RequiresDatabaseMigration { get; set; } = true;
        public bool RequiresFrameworkMigration { get; set; } = true;
        public bool RequiresSpatialMigration { get; set; } = true;
        public string CurrentADDSVersion { get; set; } = "2019";
        public string TargetADDSVersion { get; set; } = "2025";
        public string CurrentDotNetVersion { get; set; } = ".NET Framework 4.8";
        public string TargetDotNetVersion { get; set; } = ".NET Core 8";
        public string CurrentAutoCADVersion { get; set; } = "AutoCAD Map3D 2019";
        public string TargetAutoCADVersion { get; set; } = "AutoCAD Map3D 2025";
        public string CurrentOracleVersion { get; set; } = "Oracle 12c";
        public string TargetOracleVersion { get; set; } = "Oracle 19c";
        public Dictionary<string, object> CustomContext { get; set; } = new();
    }

    /// <summary>
    /// Pattern types for ADDS migration
    /// </summary>
    public enum PatternType
    {
        LauncherMigration,
        DatabaseMigration,
        FrameworkMigration,
        SpatialDataMigration,
        UIModernization,
        SecurityEnhancement,
        PerformanceOptimization,
        ConfigurationManagement,
        ErrorHandling,
        IntegrationTesting
    }

    /// <summary>
    /// Pattern complexity levels
    /// </summary>
    public enum PatternComplexityLevel
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Pattern validation configuration
    /// </summary>
    public class PatternValidationConfig
    {
        public double MinimumQualityThreshold { get; set; } = 0.6;
        public double TargetQualityScore { get; set; } = 0.85;
        public bool EnableAdvancedPatternDetection { get; set; } = true;
        public bool EnableMigrationCompliance { get; set; } = true;
        public bool EnableAccuracyMetrics { get; set; } = true;
        public Dictionary<PatternType, double> PatternWeights { get; set; } = new();
        public List<string> CustomKeywords { get; set; } = new();
        public Dictionary<string, object> AdvancedSettings { get; set; } = new();
    }

    /// <summary>
    /// Pattern detection statistics for monitoring and improvement
    /// </summary>
    public class PatternDetectionStatistics
    {
        public int TotalSuggestionsValidated { get; set; }
        public int PatternsDetected { get; set; }
        public double AverageQualityScore { get; set; }
        public double AverageConfidence { get; set; }
        public Dictionary<PatternType, int> PatternTypeDistribution { get; set; } = new();
        public Dictionary<PatternType, double> PatternTypeAccuracy { get; set; } = new();
        public List<string> MostCommonRecommendations { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Pattern training data for ML model improvement
    /// </summary>
    public class PatternTrainingData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public List<PatternType> ExpectedPatterns { get; set; } = new();
        public double ExpectedQualityScore { get; set; }
        public ADDSMigrationContext MigrationContext { get; set; } = new();
        public string ExpertAnnotation { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Pattern validation report for comprehensive analysis
    /// </summary>
    public class PatternValidationReport
    {
        public string ReportId { get; set; } = Guid.NewGuid().ToString();
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public string SuggestionText { get; set; } = string.Empty;
        public PatternValidationResult ValidationResult { get; set; } = new();
        public List<string> DetailedAnalysis { get; set; } = new();
        public Dictionary<string, object> TechnicalMetrics { get; set; } = new();
        public List<string> ImprovementSuggestions { get; set; } = new();
        public string ValidationSummary { get; set; } = string.Empty;
    }
}

