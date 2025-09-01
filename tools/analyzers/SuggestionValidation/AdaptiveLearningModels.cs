using System;
using System.Collections.Generic;
using Microsoft.ML;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Configuration for Adaptive Learning System
    /// </summary>
    public class AdaptiveLearningConfig
    {
        /// <summary>
        /// Base learning rate for weight adjustments
        /// </summary>
        public double BaseLearningRate { get; set; } = 0.01;

        /// <summary>
        /// Error threshold that triggers adaptive weight adjustment
        /// </summary>
        public double ErrorThresholdForAdaptation { get; set; } = 0.1;

        /// <summary>
        /// Accuracy threshold below which model retraining is triggered
        /// </summary>
        public double AccuracyThresholdForRetraining { get; set; } = 0.75;

        /// <summary>
        /// Number of adaptations after which retraining is triggered
        /// </summary>
        public int AdaptationCountForRetraining { get; set; } = 50;

        /// <summary>
        /// Time interval for automatic retraining
        /// </summary>
        public TimeSpan RetrainingInterval { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// Maximum size of learning history to maintain
        /// </summary>
        public int MaxHistorySize { get; set; } = 1000;

        /// <summary>
        /// Maximum size of training buffer for each model
        /// </summary>
        public int MaxTrainingBufferSize { get; set; } = 500;

        /// <summary>
        /// Minimum samples required for model retraining
        /// </summary>
        public int MinSamplesForRetraining { get; set; } = 50;

        /// <summary>
        /// Window size for performance trend analysis
        /// </summary>
        public int TrendAnalysisWindow { get; set; } = 20;

        /// <summary>
        /// Error threshold for optimization trigger
        /// </summary>
        public double ErrorThresholdForOptimization { get; set; } = 0.15;

        /// <summary>
        /// Trend threshold for optimization trigger
        /// </summary>
        public double TrendThresholdForOptimization { get; set; } = 0.05;

        /// <summary>
        /// Minimum samples required for concept drift detection
        /// </summary>
        public int MinSamplesForDriftDetection { get; set; } = 100;

        /// <summary>
        /// Window size for drift detection analysis
        /// </summary>
        public int DriftDetectionWindow { get; set; } = 50;

        /// <summary>
        /// Threshold for concept drift detection
        /// </summary>
        public double DriftThreshold { get; set; } = 0.1;

        /// <summary>
        /// Minimum samples required for online learning
        /// </summary>
        public int MinSamplesForOnlineLearning { get; set; } = 10;

        /// <summary>
        /// Batch size for online learning updates
        /// </summary>
        public int OnlineLearningBatchSize { get; set; } = 25;
    }

    /// <summary>
    /// Represents an adaptive learning model for a specific analysis type
    /// </summary>
    public class AdaptiveLearningModel
    {
        public AnalysisType AnalysisType { get; set; }
        public double LearningRate { get; set; }
        public double CurrentAccuracy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastAdaptation { get; set; }
        public DateTime LastRetraining { get; set; }
        public int AdaptationCount { get; set; }
        public int SampleCount { get; set; }
        public Dictionary<string, double>? PendingWeightAdjustments { get; set; }
        public List<AdaptiveTrainingPoint>? TrainingBuffer { get; set; }
        public ITransformer? TrainedModel { get; set; }
    }

    /// <summary>
    /// Represents a training point for adaptive learning
    /// </summary>
    public class AdaptiveTrainingPoint
    {
        public string SuggestionText { get; set; } = string.Empty;
        public ValidationContext? Context { get; set; }
        public double ActualScore { get; set; }
        public Dictionary<string, double> ValidatorScores { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// ML.NET data structure for adaptive learning
    /// </summary>
    public class AdaptiveMLData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public float ActualScore { get; set; }
        public float ValidatorScore1 { get; set; }
        public float ValidatorScore2 { get; set; }
        public float ValidatorScore3 { get; set; }
        public int WordCount { get; set; }
        public int TextLength { get; set; }
    }

    /// <summary>
    /// Learning history for an analysis type
    /// </summary>
    public class LearningHistory
    {
        public AnalysisType AnalysisType { get; set; }
        public List<LearningCycle> LearningCycles { get; set; } = new();
    }

    /// <summary>
    /// Individual learning cycle data
    /// </summary>
    public class LearningCycle
    {
        public DateTime Timestamp { get; set; }
        public string SuggestionText { get; set; } = string.Empty;
        public double ActualScore { get; set; }
        public double PredictedScore { get; set; }
        public double PredictionError { get; set; }
        public Dictionary<string, double> ValidatorScores { get; set; } = new();
    }

    /// <summary>
    /// Result of adaptive learning processing
    /// </summary>
    public class AdaptiveLearningResult
    {
        public AnalysisType AnalysisType { get; set; }
        public string SuggestionText { get; set; } = string.Empty;
        public DateTime ProcessingStartTime { get; set; }
        public DateTime ProcessingEndTime { get; set; }
        public TimeSpan ProcessingDuration { get; set; }
        public double PredictionError { get; set; }
        public double ActualScore { get; set; }
        public double PredictedScore { get; set; }
        public Dictionary<string, double>? WeightAdjustments { get; set; }
        public bool RetrainingPerformed { get; set; }
        public double NewModelAccuracy { get; set; }
        public List<string> LearningInsights { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Result of continuous learning optimization
    /// </summary>
    public class ContinuousLearningResult
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime OptimizationStartTime { get; set; }
        public DateTime OptimizationEndTime { get; set; }
        public TimeSpan OptimizationDuration { get; set; }
        public PerformanceTrends? PerformanceTrends { get; set; }
        public bool ConceptDriftDetected { get; set; }
        public double DriftSeverity { get; set; }
        public bool OnlineLearningApplied { get; set; }
        public double PerformanceImprovement { get; set; }
        public List<string> OptimizationRecommendations { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Performance trends analysis
    /// </summary>
    public class PerformanceTrends
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime AnalysisTimestamp { get; set; }
        public double AverageError { get; set; }
        public double ErrorTrend { get; set; }
        public double AccuracyStability { get; set; }
        public bool RequiresOptimization { get; set; }
    }

    /// <summary>
    /// Concept drift detection result
    /// </summary>
    public class ConceptDriftDetection
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime DetectionTimestamp { get; set; }
        public bool DriftDetected { get; set; }
        public double Severity { get; set; }
        public double AccuracyDrift { get; set; }
    }

    /// <summary>
    /// Online learning operation result
    /// </summary>
    public class OnlineLearningResult
    {
        public bool Success { get; set; }
        public double Improvement { get; set; }
        public int SamplesProcessed { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Model retraining result
    /// </summary>
    public class RetrainingResult
    {
        public bool Success { get; set; }
        public double Accuracy { get; set; }
        public double MeanAbsoluteError { get; set; }
        public double RootMeanSquaredError { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Comprehensive adaptive learning report
    /// </summary>
    public class AdaptiveLearningReport
    {
        public DateTime GenerationTime { get; set; }
        public Dictionary<AnalysisType, AnalysisTypeLearningReport> AnalysisTypeReports { get; set; } = new();
        public LearningOverallStatistics OverallStatistics { get; set; } = new();
        public List<string> SystemRecommendations { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Learning report for specific analysis type
    /// </summary>
    public class AnalysisTypeLearningReport
    {
        public AnalysisType AnalysisType { get; set; }
        public int TotalLearningCycles { get; set; }
        public double AveragePredictionError { get; set; }
        public double LearningTrend { get; set; }
        public DateTime LastLearningCycle { get; set; }
        public double CurrentModelAccuracy { get; set; }
        public int AdaptationCount { get; set; }
        public DateTime LastAdaptation { get; set; }
        public double LearningEffectiveness { get; set; }
        public List<string> ImprovementRecommendations { get; set; } = new();
    }

    /// <summary>
    /// Overall learning statistics across all analysis types
    /// </summary>
    public class LearningOverallStatistics
    {
        public double AverageAccuracy { get; set; }
        public double AverageLearningEffectiveness { get; set; }
        public int TotalAdaptations { get; set; }
        public AnalysisType BestPerformingAnalysisType { get; set; }
        public double SystemLearningTrend { get; set; }
        public DateTime LastSystemOptimization { get; set; }
    }

    /// <summary>
    /// Real-time learning metrics for monitoring
    /// </summary>
    public class RealTimeLearningMetrics
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime Timestamp { get; set; }
        public double CurrentAccuracy { get; set; }
        public double RecentErrorRate { get; set; }
        public int AdaptationsInLastHour { get; set; }
        public double LearningVelocity { get; set; }
        public bool IsOptimal { get; set; }
        public List<string> AlertMessages { get; set; } = new();
    }

    /// <summary>
    /// Adaptive threshold configuration
    /// </summary>
    public class AdaptiveThresholds
    {
        public AnalysisType AnalysisType { get; set; }
        public double QualityThreshold { get; set; }
        public double ConfidenceThreshold { get; set; }
        public double ErrorThreshold { get; set; }
        public DateTime LastUpdate { get; set; }
        public int UpdateCount { get; set; }
        public double AdaptationRate { get; set; }
    }
}

