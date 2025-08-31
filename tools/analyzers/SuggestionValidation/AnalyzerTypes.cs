using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.PatternDetection
{
    // Mock types for pattern detection - these would normally come from the main analyzers
    public class AdvancedPatternResult
    {
        public string? AnalysisId { get; set; }
        public PatternAnalysisResult? PatternAnalysis { get; set; }
        public ClusteringResult ClusteringResults { get; set; } = new();
        public SequentialPatternResult SequentialPatterns { get; set; } = new();
        public FeatureEngineeringResult FeatureEngineering { get; set; } = new();
        public PatternValidationResult ValidationResults { get; set; } = new();
        public double OverallConfidence { get; set; }
        public double StatisticalSignificance { get; set; }
        public List<PatternRecommendation> Recommendations { get; set; } = new();
    }

    public class PatternAnalysisResult
    {
        public List<IdentifiedPattern> IdentifiedPatterns { get; set; } = new();
        public List<IdentifiedPattern> AnomalousPatterns { get; set; } = new();
    }

    public class IdentifiedPattern
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    public class ClusteringResult
    {
        public int ClusterCount { get; set; }
        public List<Cluster> Clusters { get; set; } = new();
    }

    public class Cluster
    {
        public double[] Centroid { get; set; } = Array.Empty<double>();
        public double MaxDistanceFromCentroid { get; set; }
    }

    public class SequentialPatternResult
    {
        public List<SequentialPattern> Patterns { get; set; } = new();
        public int PatternCount { get; set; }
        public double AverageFrequency { get; set; }
    }

    public class SequentialPattern
    {
        public string? PatternId { get; set; }
        public List<string> Sequence { get; set; } = new();
        public List<PatternOccurrence> Occurrences { get; set; } = new();
    }

    public class PatternOccurrence
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

    public class FeatureEngineeringResult
    {
        public List<string> FeatureNames { get; set; } = new();
        public int FeatureCount { get; set; }
    }

    public class PatternValidationResult
    {
        public double StatisticalSignificance { get; set; }
        public double ClusterValidityScore { get; set; }
        public double SequentialPatternConfidence { get; set; }
        public double FeatureImportanceScore { get; set; }
    }

    public class PatternRecommendation
    {
        public string? Id { get; set; }
        public RecommendationType Type { get; set; }
        public RecommendationPriority Priority { get; set; }
        public string Description { get; set; } = string.Empty;
        public IdentifiedPattern? Pattern { get; set; }
        public List<string>? SuggestedActions { get; set; }
    }

    public enum RecommendationType
    {
        Optimization,
        Investigation,
        Monitoring,
        Improvement
    }

    public enum RecommendationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class PatternDetectionConfig
    {
        public double MinCohesionForPattern { get; set; } = 0.7;
        public double AnomalyThreshold { get; set; } = 0.8;
        public double MinSupportForSequentialPattern { get; set; } = 0.3;
        public double MinConfidenceForSequentialPattern { get; set; } = 0.6;
        public double HighConfidenceThreshold { get; set; } = 0.8;
        public double FeatureImportanceThreshold { get; set; } = 0.1;
        public double FeatureCorrelationThreshold { get; set; } = 0.7;
        public double SignificantChangeThreshold { get; set; } = 0.2;
        public double MinRelationshipStrength { get; set; } = 0.5;
        public int MaxClusterCount { get; set; } = 10;
        public int MinClusterSizeForPattern { get; set; } = 5;
        public int StreamingWindowSize { get; set; } = 100;
        public int MinWindowSizeForDetection { get; set; } = 20;
        public int MaxSequenceLength { get; set; } = 10;
        public int ForecastWindowSize { get; set; } = 24;
        public int ForecastSeriesLength { get; set; } = 100;
        public int PredictionHorizon { get; set; } = 12;
    }

    public class PatternData
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public double Value { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}

namespace ALARM.Analyzers.CausalAnalysis
{
    // Mock types for causal analysis
    public class CausalAnalysisResult
    {
        public string? AnalysisId { get; set; }
        public DateTime AnalysisTimestamp { get; set; }
        public int DataSampleCount { get; set; }
        public List<CausalRelationship> CausalRelationships { get; set; } = new();
        public CausalGraph CausalGraph { get; set; } = new();
        public List<StructuralEquation> StructuralEquations { get; set; } = new();
        public List<InterventionEffect> InterventionEffects { get; set; } = new();
        public List<ConfoundingFactor> ConfoundingFactors { get; set; } = new();
        public Dictionary<string, double>? CausalStrengths { get; set; }
        public CausalValidationResult? ValidationResults { get; set; }
        public List<CausalInsight> CausalInsights { get; set; } = new();
        public List<CausalRecommendation> Recommendations { get; set; } = new();
        public double OverallConfidence { get; set; }
        public Dictionary<string, double>? ModelFitStatistics { get; set; }
    }

    public class CausalRelationship
    {
        public string Id { get; set; } = string.Empty;
        public string CauseVariable { get; set; } = string.Empty;
        public string EffectVariable { get; set; } = string.Empty;
        public double Strength { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class CausalGraph
    {
        public List<string> Nodes { get; set; } = new();
        public List<CausalEdge> Edges { get; set; } = new();
    }

    public class CausalEdge
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public double Weight { get; set; }
    }

    public class StructuralEquation
    {
        public string DependentVariable { get; set; } = string.Empty;
        public List<string> IndependentVariables { get; set; } = new();
        public Dictionary<string, double> Coefficients { get; set; } = new();
    }

    public class InterventionEffect
    {
        public string InterventionVariable { get; set; } = string.Empty;
        public string TargetVariable { get; set; } = string.Empty;
        public double ExpectedEffect { get; set; }
    }

    public class ConfoundingFactor
    {
        public string VariableName { get; set; } = string.Empty;
        public double Impact { get; set; }
        public List<string> AffectedRelationships { get; set; } = new();
    }

    public class CausalValidationResult
    {
        public List<CausalValidationTest> ValidationTests { get; set; } = new();
        public double OverallValidationScore { get; set; }
    }

    public class CausalValidationTest
    {
        public string RelationshipId { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;
        public double ValidationScore { get; set; }
        public bool Passed { get; set; }
        public Dictionary<string, double> TestResults { get; set; } = new();
    }

    public class CausalInsight
    {
        public CausalInsightType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Importance { get; set; }
        public List<string>? RelatedRelationships { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public enum CausalInsightType
    {
        StrongCausality,
        ConfoundingDetected,
        InterventionOpportunity
    }

    public class CausalRecommendation
    {
        public string? Id { get; set; }
        public CausalRecommendationType Type { get; set; }
        public CausalRecommendationPriority Priority { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ExpectedImpact { get; set; }
        public List<string>? RelatedRelationships { get; set; }
        public List<string> ActionItems { get; set; } = new();
    }

    public enum CausalRecommendationType
    {
        Optimization,
        Investigation,
        Intervention,
        Improvement
    }

    public enum CausalRecommendationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class CausalAnalysisConfig
    {
        public double PCAlgorithmAlpha { get; set; } = 0.05;
        public double GrangerSignificanceLevel { get; set; } = 0.05;
        public double TransferEntropyThreshold { get; set; } = 0.1;
        public double MinCausalStrength { get; set; } = 0.3;
        public double CausalValidationThreshold { get; set; } = 0.6;
        public double CausalStabilityThreshold { get; set; } = 0.7;
        public double InterventionEffectThreshold { get; set; } = 0.2;
        public int MinInterventionSamples { get; set; } = 50;
        public int MaxLagForGranger { get; set; } = 5;
        public int MinDataPointsForGranger { get; set; } = 100;
        public int MaxSEMIterations { get; set; } = 1000;
        public double SEMConvergenceThreshold { get; set; } = 1e-6;
        public int TemporalWindowSize { get; set; } = 50;
    }

    public class CausalData
    {
        public DateTime Timestamp { get; set; }
        public Dictionary<string, double> Variables { get; set; } = new();
        public string Source { get; set; } = string.Empty;
    }
}

namespace ALARM.Analyzers.Performance
{
    // Mock types for performance optimization
    public class PerformanceAdjustmentResult
    {
        public string OperationName { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
        public double MemoryUsedMB { get; set; }
        public bool Success { get; set; }
        public object? Result { get; set; }
        public string? Error { get; set; }
        public Dictionary<string, double>? PerformanceMetrics { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public class PerformanceTuningContext
    {
        public int DataPointCount { get; set; }
        public int VariableCount { get; set; }
        public OptimizationStrategy Strategy { get; set; }
        public double AvailableMemoryMB { get; set; }
    }

    public enum OptimizationStrategy
    {
        Speed,
        Accuracy,
        MemoryOptimized,
        Balanced
    }
}
