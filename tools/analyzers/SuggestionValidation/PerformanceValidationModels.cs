using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Data models for Performance Validator
    /// Supporting impact prediction and resource assessment for ADDS migration
    /// </summary>
    
    /// <summary>
    /// Result of performance validation analysis
    /// </summary>
    public class PerformanceValidationResult
    {
        public double OverallPerformanceScore { get; set; }
        public PerformanceImpactAnalysis ImpactAnalysis { get; set; } = new();
        public ResourceRequirementAssessment ResourceAssessment { get; set; } = new();
        public ScalabilityAnalysis ScalabilityAnalysis { get; set; } = new();
        public BottleneckPrediction BottleneckPrediction { get; set; } = new();
        public List<OptimizationOpportunity> OptimizationOpportunities { get; set; } = new();
        public BaselineComparison BaselineComparison { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public double Confidence { get; set; }
        public DateTime ValidationTimestamp { get; set; }
    }

    /// <summary>
    /// Analysis of performance impact across different system areas
    /// </summary>
    public class PerformanceImpactAnalysis
    {
        public bool HasPerformanceMetrics { get; set; }
        public bool HasBenchmarkData { get; set; }
        public bool HasLoadTesting { get; set; }
        
        // Core performance impact areas
        public double CPUImpact { get; set; }
        public double MemoryImpact { get; set; }
        public double IOImpact { get; set; }
        public double NetworkImpact { get; set; }
        public double DatabasePerformanceImpact { get; set; }
        
        // ADDS-specific performance areas
        public double SpatialDataPerformance { get; set; }
        public double CADRenderingPerformance { get; set; }
        public double LauncherPerformance { get; set; }
        
        // Overall impact assessment
        public double OverallImpactSeverity { get; set; }
        public double ImpactConfidence { get; set; }
        public List<string> RiskFactors { get; set; } = new();
        
        // Performance metrics breakdown
        public Dictionary<string, double> MetricImpacts { get; set; } = new();
        public Dictionary<string, object> ImpactMetadata { get; set; } = new();
    }

    /// <summary>
    /// Assessment of resource requirements and constraints
    /// </summary>
    public class ResourceRequirementAssessment
    {
        // Resource requirement categories
        public double CPURequirement { get; set; }
        public double MemoryRequirement { get; set; }
        public double StorageRequirement { get; set; }
        public double NetworkRequirement { get; set; }
        public double DatabaseRequirement { get; set; }
        
        // Development and deployment resources
        public double DevelopmentEffort { get; set; }
        public double TestingRequirement { get; set; }
        public double DeploymentComplexity { get; set; }
        
        // Resource adequacy and constraints
        public double ResourceAdequacy { get; set; }
        public double ScalabilityHeadroom { get; set; }
        public List<string> ConstraintAnalysis { get; set; } = new();
        
        // Resource utilization predictions
        public Dictionary<string, ResourceUtilizationPrediction> UtilizationPredictions { get; set; } = new();
        public List<ResourceBottleneck> IdentifiedBottlenecks { get; set; } = new();
    }

    /// <summary>
    /// Analysis of scalability implications
    /// </summary>
    public class ScalabilityAnalysis
    {
        public double HorizontalScalability { get; set; }
        public double VerticalScalability { get; set; }
        public double LoadHandlingCapability { get; set; }
        public double ConcurrencySupport { get; set; }
        public string ResourceScalingPattern { get; set; } = string.Empty;
        public double BottleneckPotential { get; set; }
        public double OverallScalabilityScore { get; set; }
        
        // Scaling characteristics
        public List<ScalingCharacteristic> ScalingCharacteristics { get; set; } = new();
        public Dictionary<string, double> ScalingMetrics { get; set; } = new();
        public List<string> ScalabilityRecommendations { get; set; } = new();
    }

    /// <summary>
    /// Prediction of potential performance bottlenecks
    /// </summary>
    public class BottleneckPrediction
    {
        public List<string> CPUBottlenecks { get; set; } = new();
        public List<string> MemoryBottlenecks { get; set; } = new();
        public List<string> IOBottlenecks { get; set; } = new();
        public List<string> NetworkBottlenecks { get; set; } = new();
        public List<string> DatabaseBottlenecks { get; set; } = new();
        
        public double BottleneckLikelihood { get; set; }
        public double ImpactSeverity { get; set; }
        public List<string> MitigationStrategies { get; set; } = new();
        
        // Bottleneck details
        public Dictionary<string, BottleneckDetails> BottleneckDetails { get; set; } = new();
        public List<string> CriticalPath { get; set; } = new();
    }

    /// <summary>
    /// Performance optimization opportunity
    /// </summary>
    public class OptimizationOpportunity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double PotentialImpact { get; set; }
        public double ImplementationEffort { get; set; }
        public double Priority { get; set; }
        public List<string> Prerequisites { get; set; } = new();
        public List<string> Benefits { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Comparison against performance baseline
    /// </summary>
    public class BaselineComparison
    {
        public PerformanceBaseline Baseline { get; set; } = new();
        public Dictionary<string, double> MetricComparisons { get; set; } = new();
        public double OverallImprovement { get; set; }
        public List<string> ImprovementAreas { get; set; } = new();
        public List<string> RegressionRisks { get; set; } = new();
        public double ComparisonConfidence { get; set; }
    }

    /// <summary>
    /// Performance baseline for comparison
    /// </summary>
    public class PerformanceBaseline
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public DateTime CaptureDate { get; set; }
        public Dictionary<string, double> Metrics { get; set; } = new();
        public Dictionary<string, object> Environment { get; set; } = new();
        public List<string> TestScenarios { get; set; } = new();
    }

    /// <summary>
    /// Performance metric definition
    /// </summary>
    public class PerformanceMetricDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double TargetValue { get; set; }
        public double[] AcceptableRange { get; set; } = Array.Empty<double>();
        public double Weight { get; set; } = 1.0;
        public string Category { get; set; } = string.Empty;
        public bool IsCritical { get; set; } = false;
    }

    /// <summary>
    /// Resource constraint definition
    /// </summary>
    public class ResourceConstraint
    {
        public string Name { get; set; } = string.Empty;
        public double MaxUtilization { get; set; } = 1.0;
        public double CurrentUtilization { get; set; } = 0.0;
        public double WarningThreshold { get; set; } = 0.8;
        public double CriticalThreshold { get; set; } = 0.95;
        public string Unit { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Performance optimization rule
    /// </summary>
    public class PerformanceOptimizationRule
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Impact { get; set; } = 0.5;
        public double Confidence { get; set; } = 0.7;
        public List<string> Conditions { get; set; } = new();
        public List<string> Actions { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Resource utilization prediction
    /// </summary>
    public class ResourceUtilizationPrediction
    {
        public string ResourceType { get; set; } = string.Empty;
        public double CurrentUtilization { get; set; }
        public double PredictedUtilization { get; set; }
        public double UtilizationChange { get; set; }
        public double Confidence { get; set; }
        public List<string> Factors { get; set; } = new();
        public DateTime PredictionDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Resource bottleneck identification
    /// </summary>
    public class ResourceBottleneck
    {
        public string ResourceType { get; set; } = string.Empty;
        public string BottleneckType { get; set; } = string.Empty;
        public double Severity { get; set; }
        public double Likelihood { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Causes { get; set; } = new();
        public List<string> MitigationOptions { get; set; } = new();
        public double EstimatedImpact { get; set; }
    }

    /// <summary>
    /// Scaling characteristic definition
    /// </summary>
    public class ScalingCharacteristic
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Linear, Exponential, Logarithmic, etc.
        public double ScalingFactor { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
        public List<string> Constraints { get; set; } = new();
    }

    /// <summary>
    /// Detailed bottleneck information
    /// </summary>
    public class BottleneckDetails
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double Severity { get; set; }
        public double Probability { get; set; }
        public string Root_Cause { get; set; } = string.Empty;
        public List<string> Contributing_Factors { get; set; } = new();
        public double Estimated_Impact { get; set; }
        public List<string> Detection_Methods { get; set; } = new();
        public List<string> Mitigation_Strategies { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Configuration for performance validation
    /// </summary>
    public class PerformanceValidationConfig
    {
        public double MinimumPerformanceScore { get; set; } = 0.6;
        public double TargetPerformanceScore { get; set; } = 0.85;
        public double BottleneckThreshold { get; set; } = 0.7;
        public double ResourceUtilizationThreshold { get; set; } = 0.8;
        public bool EnableBottleneckPrediction { get; set; } = true;
        public bool EnableOptimizationSuggestions { get; set; } = true;
        public bool EnableBaselineComparison { get; set; } = true;
        public Dictionary<string, object> AdvancedSettings { get; set; } = new();
    }

    /// <summary>
    /// Performance impact categories
    /// </summary>
    public enum PerformanceImpactCategory
    {
        Positive,
        Neutral,
        Negative,
        Critical,
        Unknown
    }

    /// <summary>
    /// Resource types for performance analysis
    /// </summary>
    public enum ResourceType
    {
        CPU,
        Memory,
        Storage,
        Network,
        Database,
        GPU,
        Cache,
        ThreadPool
    }

    /// <summary>
    /// Bottleneck severity levels
    /// </summary>
    public enum BottleneckSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Performance validation statistics for monitoring
    /// </summary>
    public class PerformanceValidationStatistics
    {
        public int TotalSuggestionsAnalyzed { get; set; }
        public int BottlenecksIdentified { get; set; }
        public double AveragePerformanceScore { get; set; }
        public double AverageResourceUtilization { get; set; }
        public Dictionary<string, int> OptimizationOpportunities { get; set; } = new();
        public Dictionary<PerformanceImpactCategory, int> ImpactDistribution { get; set; } = new();
        public List<string> MostCommonBottlenecks { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Performance training data for ML model improvement
    /// </summary>
    public class PerformanceTrainingData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public PerformanceImpactAnalysis ExpectedImpact { get; set; } = new();
        public double ExpectedPerformanceScore { get; set; }
        public List<string> KnownBottlenecks { get; set; } = new();
        public Dictionary<string, double> ActualMetrics { get; set; } = new();
        public string ExpertAnnotation { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Performance validation report for comprehensive analysis
    /// </summary>
    public class PerformanceValidationReport
    {
        public string ReportId { get; set; } = Guid.NewGuid().ToString();
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public string SuggestionText { get; set; } = string.Empty;
        public PerformanceValidationResult ValidationResult { get; set; } = new();
        public List<string> DetailedAnalysis { get; set; } = new();
        public Dictionary<string, object> PerformanceMetrics { get; set; } = new();
        public List<string> OptimizationRecommendations { get; set; } = new();
        public string ValidationSummary { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}

