using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Configuration for suggestion validation system
    /// </summary>
    public class SuggestionValidationConfig
    {
        public QualityThresholds QualityThresholds { get; set; } = new QualityThresholds();
        public ValidationSettings ValidationSettings { get; set; } = new ValidationSettings();
        public LearningConfig LearningConfig { get; set; } = new LearningConfig();
        public IntegrationConfig IntegrationConfig { get; set; } = new IntegrationConfig();
    }

    /// <summary>
    /// Quality thresholds for validation scoring
    /// </summary>
    public class QualityThresholds
    {
        public double MinAcceptableScore { get; set; } = 0.6;
        public double GoodQualityThreshold { get; set; } = 0.8;
        public double ExcellentQualityThreshold { get; set; } = 0.9;
        public double CriticalThreshold { get; set; } = 0.4;
    }

    /// <summary>
    /// General validation settings
    /// </summary>
    public class ValidationSettings
    {
        public bool EnableAutomaticValidation { get; set; } = true;
        public bool EnableCrossAnalysisValidation { get; set; } = true;
        public bool EnableTrendAnalysis { get; set; } = true;
        public int MaxSuggestionsPerValidation { get; set; } = 50;
        public TimeSpan ValidationCacheExpiration { get; set; } = TimeSpan.FromHours(24);
    }

    /// <summary>
    /// Machine learning configuration for adaptive improvement
    /// </summary>
    public class LearningConfig
    {
        public bool EnableAdaptiveLearning { get; set; } = true;
        public double LearningRate { get; set; } = 0.01;
        public int MinSamplesForLearning { get; set; } = 100;
        public int ModelRetrainingInterval { get; set; } = 1000; // suggestions
    }

    /// <summary>
    /// Integration configuration with other systems
    /// </summary>
    public class IntegrationConfig
    {
        public bool EnableFeedbackUIIntegration { get; set; } = true;
        public bool EnableRealTimeValidation { get; set; } = true;
        public bool EnableHistoricalAnalysis { get; set; } = true;
        public string FeedbackDatabasePath { get; set; } = "suggestion_validation.db";
    }

    /// <summary>
    /// Types of analysis that can be validated
    /// </summary>
    public enum AnalysisType
    {
        PatternDetection,
        CausalAnalysis,
        PerformanceOptimization,
        RiskAssessment,
        SecurityAnalysis,
        ComprehensiveAnalysis
    }

    /// <summary>
    /// Context information for validation
    /// </summary>
    public class ValidationContext
    {
        public string UserId { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string SystemType { get; set; } = string.Empty;
        public Dictionary<string, object> EnvironmentInfo { get; set; } = new();
        public DateTime RequestTimestamp { get; set; } = DateTime.UtcNow;
        public string ValidationPurpose { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalContext { get; set; } = new();
        
        // ENHANCED: Detailed system information for better quality assessment
        public SystemComplexityInfo ComplexityInfo { get; set; } = new();
        public DomainSpecificContext DomainContext { get; set; } = new();
        public PerformanceConstraints PerformanceConstraints { get; set; } = new();
        public QualityExpectations QualityExpectations { get; set; } = new();
        public ValidationHistory ValidationHistory { get; set; } = new();
    }

    /// <summary>
    /// System complexity information for enhanced validation context
    /// </summary>
    public class SystemComplexityInfo
    {
        public int TotalLinesOfCode { get; set; } = 50000; // Default realistic size
        public int NumberOfModules { get; set; } = 25;
        public int NumberOfDatabases { get; set; } = 3;
        public int NumberOfIntegrations { get; set; } = 8;
        public string ArchitecturePattern { get; set; } = "Layered"; // Layered, Microservices, Monolithic
        public List<string> TechnicalDebt { get; set; } = new() { "Legacy components", "Outdated dependencies" };
        public double ComplexityScore { get; set; } = 0.7; // 0-1 scale
        public List<string> CriticalComponents { get; set; } = new() { "Authentication", "Data processing", "API layer" };
    }

    /// <summary>
    /// Domain-specific context for targeted validation
    /// </summary>
    public class DomainSpecificContext
    {
        public List<string> PrimaryDomains { get; set; } = new() { "AutoCAD", "Oracle", "Enterprise" };
        public Dictionary<string, double> DomainExpertise { get; set; } = new() 
        { 
            ["AutoCAD"] = 0.8, 
            ["Oracle"] = 0.7, 
            ["DotNetCore"] = 0.9,
            ["ADDS"] = 0.6 
        };
        public List<string> BusinessCriticalAreas { get; set; } = new() { "Data integrity", "User authentication", "Performance" };
        public Dictionary<string, string> ComplianceRequirements { get; set; } = new()
        {
            ["Security"] = "Enterprise-grade",
            ["Performance"] = "Sub-second response",
            ["Reliability"] = "99.9% uptime"
        };
        public List<string> KnownPatterns { get; set; } = new() { "Repository pattern", "Factory pattern", "Observer pattern" };
    }

    /// <summary>
    /// Performance constraints and expectations
    /// </summary>
    public class PerformanceConstraints
    {
        public int MaxResponseTimeMs { get; set; } = 1000;
        public int MaxMemoryUsageMB { get; set; } = 512;
        public int MaxConcurrentUsers { get; set; } = 100;
        public double MinThroughputRPS { get; set; } = 50.0;
        public List<string> PerformanceCriticalOperations { get; set; } = new() 
        { 
            "Database queries", 
            "File processing", 
            "API calls",
            "Authentication"
        };
        public Dictionary<string, double> ResourceLimits { get; set; } = new()
        {
            ["CPU"] = 0.8,
            ["Memory"] = 0.7,
            ["Disk"] = 0.6,
            ["Network"] = 0.5
        };
    }

    /// <summary>
    /// Quality expectations for validation scoring
    /// </summary>
    public class QualityExpectations
    {
        // IMPROVED: Progressive quality thresholds starting at 60%
        public double MinimumQualityThreshold { get; set; } = 0.60; // 60% minimum (Phase 1)
        public double TargetQualityScore { get; set; } = 0.75; // 75% target (Phase 1)
        public double AdvancedTargetScore { get; set; } = 0.85; // 85% advanced target (Phase 2)
        public double ExcellenceTargetScore { get; set; } = 0.90; // 90% excellence target (Phase 3)
        
        // ENHANCED: Configurable progressive thresholds
        public Dictionary<string, double> ProgressiveThresholds { get; set; } = new()
        {
            ["Phase1_Minimum"] = 0.60,  // Initial realistic baseline
            ["Phase1_Target"] = 0.75,   // Phase 1 completion target
            ["Phase2_Target"] = 0.85,   // Phase 2 ML-enhanced target
            ["Phase3_Target"] = 0.90,   // Phase 3 production excellence
            ["Critical_Minimum"] = 0.50, // Absolute minimum for critical issues
            ["Experimental_Threshold"] = 0.40 // For experimental/research suggestions
        };
        
        public Dictionary<string, double> MetricWeights { get; set; } = new()
        {
            ["Relevance"] = 0.25,
            ["Actionability"] = 0.25,
            ["Feasibility"] = 0.20,
            ["Impact"] = 0.20,
            ["Clarity"] = 0.10
        };
        
        // ENHANCED: Context-aware quality expectations
        public Dictionary<string, double> ContextualThresholds { get; set; } = new()
        {
            ["LegacySystem"] = 0.60,      // Lower expectations for legacy
            ["ModernSystem"] = 0.75,      // Higher for modern systems
            ["CriticalSystem"] = 0.80,    // Highest for critical systems
            ["ExperimentalFeature"] = 0.45, // Lower for experimental work
            ["SecurityRelated"] = 0.85,   // Higher for security suggestions
            ["PerformanceCritical"] = 0.70 // Moderate for performance work
        };
        
        public List<string> PriorityAreas { get; set; } = new() 
        { 
            "Security improvements", 
            "Performance optimization", 
            "Code maintainability" 
        };
        public bool RequireEvidence { get; set; } = true;
        public bool RequireImplementationPlan { get; set; } = true;
        
        // ENHANCED: Adaptive threshold calculation
        public double GetContextualThreshold(string systemType, string suggestionCategory, int phaseLevel = 1)
        {
            var baseThreshold = phaseLevel switch
            {
                1 => MinimumQualityThreshold,  // 60%
                2 => AdvancedTargetScore,      // 85%
                3 => ExcellenceTargetScore,    // 90%
                _ => MinimumQualityThreshold
            };
            
            // Adjust based on system context
            if (ContextualThresholds.TryGetValue(systemType, out var contextAdjustment))
            {
                baseThreshold = Math.Max(baseThreshold, contextAdjustment);
            }
            
            // Adjust based on suggestion category
            if (ContextualThresholds.TryGetValue(suggestionCategory, out var categoryAdjustment))
            {
                baseThreshold = Math.Max(baseThreshold, categoryAdjustment);
            }
            
            return Math.Min(1.0, baseThreshold);
        }
    }

    /// <summary>
    /// Historical validation data for trend analysis
    /// </summary>
    public class ValidationHistory
    {
        public List<double> RecentQualityScores { get; set; } = new() { 0.65, 0.68, 0.72 }; // Improving trend
        public Dictionary<string, int> CommonIssueTypes { get; set; } = new()
        {
            ["Vague suggestions"] = 15,
            ["Missing implementation details"] = 12,
            ["Unrealistic expectations"] = 8,
            ["Poor evidence"] = 10
        };
        public List<string> SuccessfulPatterns { get; set; } = new() 
        { 
            "Specific code examples", 
            "Step-by-step implementation", 
            "Performance metrics" 
        };
        public DateTime LastValidation { get; set; } = DateTime.UtcNow.AddDays(-1);
        public int TotalValidationsPerformed { get; set; } = 47;
    }

    /// <summary>
    /// Result of suggestion validation for a single analysis type
    /// </summary>
    public class SuggestionValidationResult
    {
        public string ValidationId { get; set; } = Guid.NewGuid().ToString();
        public AnalysisType AnalysisType { get; set; }
        public DateTime ValidationTimestamp { get; set; }
        public string SourceAnalysisId { get; set; } = string.Empty;
        public List<IndividualSuggestionValidation> SuggestionValidations { get; set; } = new();
        public double OverallQualityScore { get; set; }
        public Dictionary<string, double> QualityMetrics { get; set; } = new();
        public List<string> ImprovementRecommendations { get; set; } = new();
        public ValidationContext ValidationContext { get; set; } = new();
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }

    /// <summary>
    /// Validation result for an individual suggestion
    /// </summary>
    public class IndividualSuggestionValidation
    {
        public string SuggestionId { get; set; } = string.Empty;
        public string SuggestionText { get; set; } = string.Empty;
        public double OverallScore { get; set; }
        public Dictionary<string, double> QualityScores { get; set; } = new();
        public Dictionary<string, object> ValidationDetails { get; set; } = new();
        public List<ValidationIssue> Issues { get; set; } = new();
        public List<string> Improvements { get; set; } = new();
        public DateTime ValidationTimestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Validation issue found in a suggestion
    /// </summary>
    public class ValidationIssue
    {
        public string IssueId { get; set; } = Guid.NewGuid().ToString();
        public string IssueType { get; set; } = string.Empty;
        public ValidationIssueSeverity Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SuggestedFix { get; set; } = string.Empty;
        public Dictionary<string, object> Details { get; set; } = new();
    }

    /// <summary>
    /// Severity levels for validation issues
    /// </summary>
    public enum ValidationIssueSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Comprehensive validation result across multiple analysis types
    /// </summary>
    public class ComprehensiveSuggestionValidationResult
    {
        public string ValidationId { get; set; } = Guid.NewGuid().ToString();
        public DateTime ValidationTimestamp { get; set; }
        public Dictionary<AnalysisType, SuggestionValidationResult> ValidationResults { get; set; } = new();
        public Dictionary<string, double> CrossAnalysisConsistency { get; set; } = new();
        public double OverallSystemQuality { get; set; }
        public List<string> SystemWideImprovements { get; set; } = new();
        public ValidationContext ValidationContext { get; set; } = new();
        public Dictionary<string, object> ComprehensiveMetrics { get; set; } = new();
    }

    /// <summary>
    /// Input for comprehensive analysis validation
    /// </summary>
    public class ComprehensiveAnalysisResult
    {
        public string AnalysisId { get; set; } = Guid.NewGuid().ToString();
        public DateTime AnalysisTimestamp { get; set; } = DateTime.UtcNow;
        
        // Pattern Detection Results
        public object? PatternResult { get; set; }
        public IEnumerable<object>? PatternSuggestions { get; set; }
        
        // Causal Analysis Results
        public object? CausalResult { get; set; }
        public IEnumerable<object>? CausalSuggestions { get; set; }
        
        // Performance Results
        public object? PerformanceResult { get; set; }
        public IEnumerable<string>? PerformanceSuggestions { get; set; }
        
        // Additional analysis results
        public Dictionary<string, object> AdditionalResults { get; set; } = new();
    }

    /// <summary>
    /// Validation trends and historical analysis
    /// </summary>
    public class ValidationTrendsResult
    {
        public TimeSpan TimeWindow { get; set; }
        public AnalysisType? AnalysisType { get; set; }
        public Dictionary<string, List<double>> QualityTrends { get; set; } = new();
        public ValidationVolumeStats VolumeStats { get; set; } = new();
        public List<TopValidationIssue> TopIssues { get; set; } = new();
        public List<string> ImprovementOpportunities { get; set; } = new();
        public List<string> SuccessPatterns { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    /// <summary>
    /// Volume statistics for validation trends
    /// </summary>
    public class ValidationVolumeStats
    {
        public int TotalValidations { get; set; }
        public int TotalSuggestions { get; set; }
        public double AverageQualityScore { get; set; }
        public int HighQualitySuggestions { get; set; }
        public int LowQualitySuggestions { get; set; }
        public Dictionary<AnalysisType, int> ValidationsByType { get; set; } = new();
    }

    /// <summary>
    /// Top validation issue for trend analysis
    /// </summary>
    public class TopValidationIssue
    {
        public string IssueType { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public double AverageImpact { get; set; }
        public ValidationIssueSeverity TypicalSeverity { get; set; }
        public List<string> CommonSuggestions { get; set; } = new();
    }

    /// <summary>
    /// Feedback trends data from integration service
    /// </summary>
    public class FeedbackTrendsData
    {
        public Dictionary<string, List<double>> QualityTrends { get; set; } = new();
        public ValidationVolumeStats VolumeStats { get; set; } = new();
        public List<TopValidationIssue> TopIssues { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// Insights generated from trend analysis
    /// </summary>
    public class TrendInsights
    {
        public List<string> ImprovementOpportunities { get; set; } = new();
        public List<string> SuccessPatterns { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public Dictionary<string, double> KeyMetrics { get; set; } = new();
    }

    /// <summary>
    /// Quality assessment metrics for different suggestion types
    /// </summary>
    public class QualityAssessmentMetrics
    {
        // Pattern Detection Metrics
        public double PatternRelevance { get; set; }
        public double PatternActionability { get; set; }
        public double PatternSpecificity { get; set; }
        public double PatternFeasibility { get; set; }
        public double PatternImpactPotential { get; set; }

        // Causal Analysis Metrics
        public double CausalValidity { get; set; }
        public double EvidenceStrength { get; set; }
        public double PracticalApplicability { get; set; }
        public double RiskLevel { get; set; }
        public double ExpectedImpact { get; set; }

        // Performance Metrics
        public double TechnicalAccuracy { get; set; }
        public double ImplementationComplexity { get; set; }
        public double PerformanceImpact { get; set; }
        public double ResourceRequirements { get; set; }

        // General Metrics
        public double OverallClarity { get; set; }
        public double Completeness { get; set; }
        public double Consistency { get; set; }
    }

    /// <summary>
    /// Learning model performance metrics
    /// </summary>
    public class LearningModelMetrics
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public int TrainingSamples { get; set; }
        public int ValidationSamples { get; set; }
        public DateTime LastTrainingDate { get; set; }
        public Dictionary<string, double> FeatureImportances { get; set; } = new();
    }

    /// <summary>
    /// Validation model prediction result
    /// </summary>
    public class ValidationPrediction
    {
        public double PredictedQualityScore { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> QualityBreakdown { get; set; } = new();
        public List<string> PredictedIssues { get; set; } = new();
        public List<string> SuggestedImprovements { get; set; } = new();
    }
}
