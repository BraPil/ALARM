using System;
using System.Collections.Generic;
using Microsoft.ML.Data;

namespace ALARM.Analyzers.PatternDetection
{
    #region Core Data Models

    /// <summary>
    /// Input data for pattern detection
    /// </summary>
    public class PatternData
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public List<double> Features { get; set; } = new List<double>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public string Category { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// Configuration for pattern detection algorithms
    /// </summary>
    public class PatternDetectionConfig
    {
        // Clustering parameters
        public int MaxClusterCount { get; set; } = 10;
        public int MinClusterSizeForPattern { get; set; } = 5;
        public double MinCohesionForPattern { get; set; } = 0.6;
        public double AnomalyThreshold { get; set; } = 0.3;

        // Sequential pattern parameters
        public double MinSupportForSequentialPattern { get; set; } = 0.1;
        public double MinConfidenceForSequentialPattern { get; set; } = 0.7;
        public double HighConfidenceThreshold { get; set; } = 0.9;
        public int MaxSequenceLength { get; set; } = 10;

        // Feature engineering parameters
        public double FeatureImportanceThreshold { get; set; } = 0.1;
        public double FeatureCorrelationThreshold { get; set; } = 0.8;
        public double SignificantChangeThreshold { get; set; } = 0.1;

        // Streaming parameters
        public int StreamingWindowSize { get; set; } = 100;
        public int MinWindowSizeForDetection { get; set; } = 10;

        // Relationship analysis parameters
        public double MinRelationshipStrength { get; set; } = 0.5;

        // Forecasting parameters
        public int ForecastWindowSize { get; set; } = 24;
        public int ForecastSeriesLength { get; set; } = 100;
        public int PredictionHorizon { get; set; } = 12;
    }

    #endregion

    #region Result Models

    /// <summary>
    /// Complete result from advanced pattern detection
    /// </summary>
    public class AdvancedPatternResult
    {
        public DateTime DetectionTimestamp { get; set; }
        public int DataSampleCount { get; set; }
        public FeatureEngineeringResult FeatureEngineering { get; set; }
        public ClusteringResult ClusteringResults { get; set; }
        public SequentialPatternResult SequentialPatterns { get; set; }
        public PatternAnalysisResult PatternAnalysis { get; set; }
        public PatternValidationResult ValidationResults { get; set; }
        public double OverallConfidence { get; set; }
        public double StatisticalSignificance { get; set; }
        public List<PatternRecommendation> Recommendations { get; set; } = new List<PatternRecommendation>();
    }

    /// <summary>
    /// Feature engineering results
    /// </summary>
    public class FeatureEngineeringResult
    {
        public DateTime ExtractionTimestamp { get; set; }
        public int OriginalDataCount { get; set; }
        public int FeatureCount { get; set; }
        public List<string> FeatureNames { get; set; } = new List<string>();
        public double[][] FeatureMatrix { get; set; }
        public Dictionary<string, FeatureStatistics> FeatureStatistics { get; set; } = new Dictionary<string, FeatureStatistics>();
        public Dictionary<string, Dictionary<string, double>> FeatureCorrelations { get; set; } = new Dictionary<string, Dictionary<string, double>>();
        public Dictionary<string, double> FeatureImportanceScores { get; set; } = new Dictionary<string, double>();
        public double FeatureSelectionRatio { get; set; }
    }

    /// <summary>
    /// Clustering analysis results
    /// </summary>
    public class ClusteringResult
    {
        public DateTime ClusteringTimestamp { get; set; }
        public int ClusterCount { get; set; }
        public int TotalDataPoints { get; set; }
        public List<Cluster> Clusters { get; set; } = new List<Cluster>();
        public double OverallQuality { get; set; }
        public string Algorithm { get; set; }
        public Dictionary<string, double> AlgorithmParameters { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Individual cluster information
    /// </summary>
    public class Cluster
    {
        public string Id { get; set; }
        public int Size { get; set; }
        public double[] Centroid { get; set; }
        public double Cohesion { get; set; }
        public double AnomalyScore { get; set; }
        public double MaxDistanceFromCentroid { get; set; }
        public List<int> DataPointIndices { get; set; } = new List<int>();
        public Dictionary<string, double> CharacteristicFeatures { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Sequential pattern mining results
    /// </summary>
    public class SequentialPatternResult
    {
        public DateTime MiningTimestamp { get; set; }
        public int PatternCount { get; set; }
        public double AverageFrequency { get; set; }
        public List<SequentialPattern> Patterns { get; set; } = new List<SequentialPattern>();
        public double TemporalComplexity { get; set; }
        public string Algorithm { get; set; }
    }

    /// <summary>
    /// Individual sequential pattern
    /// </summary>
    public class SequentialPattern
    {
        public string PatternId { get; set; }
        public List<string> Sequence { get; set; } = new List<string>();
        public double Support { get; set; }
        public double Confidence { get; set; }
        public List<PatternOccurrence> Occurrences { get; set; } = new List<PatternOccurrence>();
        public TimeSpan AverageInterval { get; set; }
        public Dictionary<string, double> SequenceMetrics { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Pattern occurrence in data
    /// </summary>
    public class PatternOccurrence
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public int DataIndex { get; set; }
        public double MatchConfidence { get; set; }
    }

    /// <summary>
    /// Pattern analysis results
    /// </summary>
    public class PatternAnalysisResult
    {
        public DateTime AnalysisTimestamp { get; set; }
        public List<IdentifiedPattern> IdentifiedPatterns { get; set; } = new List<IdentifiedPattern>();
        public List<IdentifiedPattern> AnomalousPatterns { get; set; } = new List<IdentifiedPattern>();
        public List<PatternRelationship> PatternRelationships { get; set; } = new List<PatternRelationship>();
        public Dictionary<string, double> FeatureImportance { get; set; } = new Dictionary<string, double>();
        public PatternMetrics PatternMetrics { get; set; }
        public List<PatternHierarchy> PatternHierarchies { get; set; } = new List<PatternHierarchy>();
        public List<PatternInsight> PatternInsights { get; set; } = new List<PatternInsight>();
    }

    /// <summary>
    /// Individual identified pattern
    /// </summary>
    public class IdentifiedPattern
    {
        public string Id { get; set; }
        public PatternType Type { get; set; }
        public string Description { get; set; }
        public double Confidence { get; set; }
        public double Frequency { get; set; }
        public List<string> Features { get; set; } = new List<string>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public DateTime FirstOccurrence { get; set; }
        public DateTime LastOccurrence { get; set; }
    }

    /// <summary>
    /// Pattern validation results
    /// </summary>
    public class PatternValidationResult
    {
        public DateTime ValidationTimestamp { get; set; }
        public double StatisticalSignificance { get; set; }
        public double ClusterValidityScore { get; set; }
        public double SequentialPatternConfidence { get; set; }
        public double FeatureImportanceScore { get; set; }
        public List<ValidationTest> ValidationTests { get; set; } = new List<ValidationTest>();
        public Dictionary<string, double> QualityMetrics { get; set; } = new Dictionary<string, double>();
    }

    #endregion

    #region Supporting Models

    /// <summary>
    /// Feature statistics
    /// </summary>
    public class FeatureStatistics
    {
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Median { get; set; }
        public double Skewness { get; set; }
        public double Kurtosis { get; set; }
        public double ClusterValue { get; set; }
        public double GlobalMean { get; set; }
        public double ZScore { get; set; }
        public double Importance { get; set; }
    }

    /// <summary>
    /// Pattern relationship information
    /// </summary>
    public class PatternRelationship
    {
        public string Pattern1Id { get; set; }
        public string Pattern2Id { get; set; }
        public RelationshipType RelationshipType { get; set; }
        public double Strength { get; set; }
        public string Description { get; set; }
        public Dictionary<string, double> RelationshipMetrics { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Pattern metrics summary
    /// </summary>
    public class PatternMetrics
    {
        public int TotalPatterns { get; set; }
        public int TotalAnomalies { get; set; }
        public double AverageConfidence { get; set; }
        public double AverageFrequency { get; set; }
        public double PatternDiversity { get; set; }
        public double RelationshipDensity { get; set; }
    }

    /// <summary>
    /// Pattern hierarchy structure
    /// </summary>
    public class PatternHierarchy
    {
        public PatternType RootType { get; set; }
        public List<HierarchyLevel> Levels { get; set; } = new List<HierarchyLevel>();
    }

    /// <summary>
    /// Hierarchy level
    /// </summary>
    public class HierarchyLevel
    {
        public int Level { get; set; }
        public string Description { get; set; }
        public List<IdentifiedPattern> Patterns { get; set; } = new List<IdentifiedPattern>();
    }

    /// <summary>
    /// Pattern insights
    /// </summary>
    public class PatternInsight
    {
        public InsightType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Importance { get; set; }
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    /// <summary>
    /// Pattern recommendation
    /// </summary>
    public class PatternRecommendation
    {
        public RecommendationType Type { get; set; }
        public RecommendationPriority Priority { get; set; }
        public IdentifiedPattern Pattern { get; set; }
        public string Description { get; set; }
        public List<string> SuggestedActions { get; set; } = new List<string>();
        public double ExpectedImpact { get; set; }
    }

    /// <summary>
    /// Validation test result
    /// </summary>
    public class ValidationTest
    {
        public string TestName { get; set; }
        public bool Passed { get; set; }
        public double Score { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> TestParameters { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Cluster characteristics analysis
    /// </summary>
    public class ClusterCharacteristics
    {
        public List<string> DominantFeatures { get; set; } = new List<string>();
        public List<string> OutlierFeatures { get; set; } = new List<string>();
        public Dictionary<string, FeatureStatistics> FeatureStatistics { get; set; } = new Dictionary<string, FeatureStatistics>();
    }

    /// <summary>
    /// Cluster pattern analysis result
    /// </summary>
    public class ClusterPatternAnalysis
    {
        public List<IdentifiedPattern> Patterns { get; set; } = new List<IdentifiedPattern>();
        public List<IdentifiedPattern> Anomalies { get; set; } = new List<IdentifiedPattern>();
        public double ClusterQuality { get; set; }
        public int OptimalClusterCount { get; set; }
    }

    /// <summary>
    /// Sequential pattern analysis result
    /// </summary>
    public class SequentialPatternAnalysis
    {
        public List<IdentifiedPattern> Patterns { get; set; } = new List<IdentifiedPattern>();
        public List<IdentifiedPattern> Anomalies { get; set; } = new List<IdentifiedPattern>();
        public double TemporalComplexity { get; set; }
        public double AverageSequenceLength { get; set; }
    }

    #endregion

    #region Streaming and Comparison Models

    /// <summary>
    /// Streaming pattern detection result
    /// </summary>
    public class StreamingPatternResult
    {
        public List<PatternDetectionWindow> WindowResults { get; set; } = new List<PatternDetectionWindow>();
        public int TotalWindows { get; set; }
        public int PatternChanges { get; set; }
        public List<TrendAnalysis> OverallTrends { get; set; } = new List<TrendAnalysis>();
    }

    /// <summary>
    /// Pattern detection window for streaming analysis
    /// </summary>
    public class PatternDetectionWindow
    {
        public DateTime WindowTimestamp { get; set; }
        public int WindowSize { get; set; }
        public AdvancedPatternResult Patterns { get; set; }
        public bool ChangeDetected { get; set; }
    }

    /// <summary>
    /// Pattern comparison result
    /// </summary>
    public class PatternComparisonResult
    {
        public AdvancedPatternResult BaselinePatterns { get; set; }
        public AdvancedPatternResult ComparisonPatterns { get; set; }
        public double PatternSimilarity { get; set; }
        public List<SignificantDifference> SignificantDifferences { get; set; } = new List<SignificantDifference>();
        public List<TrendAnalysis> TrendAnalysis { get; set; } = new List<TrendAnalysis>();
        public List<PatternRecommendation> Recommendations { get; set; } = new List<PatternRecommendation>();
    }

    /// <summary>
    /// Significant difference between patterns
    /// </summary>
    public class SignificantDifference
    {
        public DifferenceType Type { get; set; }
        public double BaselineValue { get; set; }
        public double ComparisonValue { get; set; }
        public double Significance { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Trend analysis result
    /// </summary>
    public class TrendAnalysis
    {
        public string MetricName { get; set; }
        public TrendDirection TrendDirection { get; set; }
        public double TrendStrength { get; set; }
        public double[] Values { get; set; }
        public double Correlation { get; set; }
    }

    #endregion

    #region Prediction Models

    /// <summary>
    /// Pattern prediction result
    /// </summary>
    public class PatternPredictionResult
    {
        public AdvancedPatternResult HistoricalPatterns { get; set; }
        public PredictionModel PredictionModel { get; set; }
        public List<PredictionPoint> Predictions { get; set; } = new List<PredictionPoint>();
        public int PredictionHorizon { get; set; }
        public double ModelAccuracy { get; set; }
        public List<ConfidenceInterval> ConfidenceIntervals { get; set; } = new List<ConfidenceInterval>();
    }

    /// <summary>
    /// Prediction model
    /// </summary>
    public class PredictionModel
    {
        public object Model { get; set; } // ITransformer in ML.NET
        public int TrainingDataCount { get; set; }
        public int WindowSize { get; set; }
        public int SeriesLength { get; set; }
        public Dictionary<string, double> ModelMetrics { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Individual prediction point
    /// </summary>
    public class PredictionPoint
    {
        public int PredictionIndex { get; set; }
        public float PredictedValue { get; set; }
        public float ConfidenceLowerBound { get; set; }
        public float ConfidenceUpperBound { get; set; }
        public DateTime Timestamp { get; set; }
        public double Confidence { get; set; }
    }

    /// <summary>
    /// Confidence interval for predictions
    /// </summary>
    public class ConfidenceInterval
    {
        public int PredictionIndex { get; set; }
        public float LowerBound { get; set; }
        public float UpperBound { get; set; }
        public double ConfidenceLevel { get; set; }
    }

    #endregion

    #region Enumerations

    /// <summary>
    /// Types of patterns that can be detected
    /// </summary>
    public enum PatternType
    {
        Cluster,
        Sequential,
        Temporal,
        Frequency,
        Anomaly,
        RareButSignificant,
        Cyclic,
        Trend
    }

    /// <summary>
    /// Types of relationships between patterns
    /// </summary>
    public enum RelationshipType
    {
        Correlation,
        Sequential,
        Hierarchical,
        Complementary,
        Causal,
        Inverse
    }

    /// <summary>
    /// Types of insights
    /// </summary>
    public enum InsightType
    {
        Distribution,
        FeatureImportance,
        Anomaly,
        Trend,
        Relationship,
        Performance
    }

    /// <summary>
    /// Types of recommendations
    /// </summary>
    public enum RecommendationType
    {
        Optimization,
        Investigation,
        Improvement,
        Prevention,
        Enhancement
    }

    /// <summary>
    /// Priority levels for recommendations
    /// </summary>
    public enum RecommendationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Types of differences in comparisons
    /// </summary>
    public enum DifferenceType
    {
        ClusterCount,
        PatternFrequency,
        Confidence,
        FeatureImportance,
        SequenceLength,
        AnomalyRate
    }

    /// <summary>
    /// Trend directions
    /// </summary>
    public enum TrendDirection
    {
        Increasing,
        Decreasing,
        Stable,
        Volatile,
        Cyclic
    }

    #endregion
}
