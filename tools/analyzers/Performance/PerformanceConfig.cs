using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.Performance
{
    /// <summary>
    /// Comprehensive performance configuration for ALARM system
    /// Provides adaptive thresholds and optimization settings
    /// </summary>
    public class PerformanceConfig
    {
        public AdaptiveThresholds Adaptive { get; set; } = new();
        public CachingSettings Caching { get; set; } = new();
        public ParallelProcessing Parallel { get; set; } = new();
        public ResourceLimits Resources { get; set; } = new();
        public MonitoringSettings Monitoring { get; set; } = new();
    }

    /// <summary>
    /// Adaptive threshold configuration that adjusts based on data characteristics
    /// </summary>
    public class AdaptiveThresholds
    {
        // Pattern Detection Adaptive Settings
        public bool EnableAdaptiveClustering { get; set; } = true;
        public int MinClusterCount { get; set; } = 2;
        public int MaxClusterCount { get; set; } = 20; // Increased from 10
        public double ClusterCountScalingFactor { get; set; } = 0.1; // Clusters per 100 data points
        
        public bool EnableAdaptiveWindowSizing { get; set; } = true;
        public int MinStreamingWindowSize { get; set; } = 50;
        public int MaxStreamingWindowSize { get; set; } = 500; // Increased from 100
        public double WindowSizeScalingFactor { get; set; } = 0.2;
        
        // Causal Analysis Adaptive Settings
        public bool EnableAdaptiveLagSelection { get; set; } = true;
        public int MinLagForGranger { get; set; } = 1;
        public int MaxLagForGranger { get; set; } = 10; // Increased from 5
        public double LagSelectionCriterion { get; set; } = 0.05; // AIC/BIC threshold
        
        public bool EnableAdaptiveIterations { get; set; } = true;
        public int MinSEMIterations { get; set; } = 10;
        public int MaxSEMIterations { get; set; } = 200; // Increased from 100
        public double ConvergenceThreshold { get; set; } = 0.0001; // Tightened from 0.001
        
        // Quality-based thresholds
        public double QualityBasedTimeoutMultiplier { get; set; } = 1.5;
        public double LowQualityThreshold { get; set; } = 0.3;
        public double HighQualityThreshold { get; set; } = 0.8;
    }

    /// <summary>
    /// Caching configuration for performance optimization
    /// </summary>
    public class CachingSettings
    {
        public bool EnableMLModelCaching { get; set; } = true;
        public bool EnablePatternResultCaching { get; set; } = true;
        public bool EnableFeatureExtractionCaching { get; set; } = true;
        public bool EnableCausalAnalysisCaching { get; set; } = true;
        
        // Cache size limits (number of items)
        public int MaxMLModelCacheSize { get; set; } = 50;
        public int MaxPatternResultCacheSize { get; set; } = 200;
        public int MaxFeatureCacheSize { get; set; } = 1000;
        public int MaxCausalResultCacheSize { get; set; } = 100;
        
        // Cache expiration times (minutes)
        public int MLModelCacheExpiration { get; set; } = 60;
        public int PatternResultCacheExpiration { get; set; } = 30;
        public int FeatureCacheExpiration { get; set; } = 15;
        public int CausalResultCacheExpiration { get; set; } = 45;
        
        // Cache key generation settings
        public bool UseDataHashForCacheKeys { get; set; } = true;
        public bool IncludeTimestampInCacheKeys { get; set; } = false;
    }

    /// <summary>
    /// Parallel processing configuration
    /// </summary>
    public class ParallelProcessing
    {
        public bool EnableParallelClustering { get; set; } = true;
        public bool EnableParallelCausalAnalysis { get; set; } = true;
        public bool EnableParallelFeatureExtraction { get; set; } = true;
        public bool EnableParallelValidation { get; set; } = true;
        
        // Thread management
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
        public int MinDataPointsForParallel { get; set; } = 100;
        public int BatchSizeForParallelProcessing { get; set; } = 50;
        
        // Task scheduling
        public bool UseTaskScheduler { get; set; } = true;
        public int TaskTimeoutSeconds { get; set; } = 300; // 5 minutes
        public bool EnableProgressReporting { get; set; } = true;
    }

    /// <summary>
    /// Resource limit configuration
    /// </summary>
    public class ResourceLimits
    {
        // Memory limits (MB)
        public int MaxMemoryUsageMB { get; set; } = 1024; // 1GB default
        public int MemoryWarningThresholdMB { get; set; } = 768; // 75% of max
        public bool EnableMemoryMonitoring { get; set; } = true;
        
        // Processing time limits (seconds)
        public int MaxPatternDetectionTimeSeconds { get; set; } = 120; // 2 minutes
        public int MaxCausalAnalysisTimeSeconds { get; set; } = 180; // 3 minutes
        public int MaxMLTrainingTimeSeconds { get; set; } = 300; // 5 minutes
        
        // Data size limits
        public int MaxDataPointsPerAnalysis { get; set; } = 10000;
        public int MaxVariablesForCausalAnalysis { get; set; } = 50;
        public int MaxFeaturesPerDataPoint { get; set; } = 100;
        
        // Database limits
        public int MaxDatabaseConnections { get; set; } = 20;
        public int DatabaseCommandTimeoutSeconds { get; set; } = 30;
        public int MaxQueryResultSize { get; set; } = 5000;
    }

    /// <summary>
    /// Performance monitoring configuration
    /// </summary>
    public class MonitoringSettings
    {
        public bool EnablePerformanceMonitoring { get; set; } = true;
        public bool EnableDetailedLogging { get; set; } = false;
        public bool EnableMetricsCollection { get; set; } = true;
        
        // Monitoring intervals (seconds)
        public int MemoryMonitoringInterval { get; set; } = 10;
        public int PerformanceMetricsInterval { get; set; } = 30;
        public int ResourceUsageReportingInterval { get; set; } = 60;
        
        // Alert thresholds
        public double CPUUsageAlertThreshold { get; set; } = 0.8; // 80%
        public double MemoryUsageAlertThreshold { get; set; } = 0.85; // 85%
        public double ResponseTimeAlertThreshold { get; set; } = 5.0; // 5 seconds
        
        // Performance targets
        public double TargetPatternDetectionTime { get; set; } = 2.0; // 2 seconds
        public double TargetCausalAnalysisTime { get; set; } = 3.0; // 3 seconds
        public double TargetAPIResponseTime { get; set; } = 0.5; // 500ms
    }

    /// <summary>
    /// Performance optimization strategies
    /// </summary>
    public enum OptimizationStrategy
    {
        Balanced,      // Balance between speed and accuracy
        Speed,         // Prioritize speed over accuracy
        Accuracy,      // Prioritize accuracy over speed
        MemoryOptimized, // Minimize memory usage
        Adaptive       // Dynamically adjust based on conditions
    }

    /// <summary>
    /// Performance tuning context for adaptive adjustments
    /// </summary>
    public class PerformanceTuningContext
    {
        public int DataPointCount { get; set; }
        public int VariableCount { get; set; }
        public int FeatureCount { get; set; }
        public double DataComplexity { get; set; }
        public double AvailableMemoryMB { get; set; }
        public double CPUUsage { get; set; }
        public OptimizationStrategy Strategy { get; set; } = OptimizationStrategy.Balanced;
        public Dictionary<string, object> CustomParameters { get; set; } = new();
    }
}
