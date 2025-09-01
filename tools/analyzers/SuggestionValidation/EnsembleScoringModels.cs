using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    #region Core Ensemble Models

    /// <summary>
    /// Configuration for ensemble scoring with dynamic optimization
    /// </summary>
    public class EnsembleConfiguration
    {
        public AnalysisType AnalysisType { get; set; }
        public Dictionary<string, double> BaseWeights { get; set; } = new();
        public Dictionary<string, (double Min, double Max)> WeightBounds { get; set; } = new();
        public OptimizationStrategy OptimizationStrategy { get; set; } = OptimizationStrategy.GradientBased;
        public double OptimizationThreshold { get; set; } = 0.02; // 2% improvement threshold
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public int OptimizationFrequency { get; set; } = 100; // Optimize every 100 predictions
        public bool EnableDynamicWeighting { get; set; } = true;
        public double ConfidenceWeightingFactor { get; set; } = 1.2;
        public double ComplexityWeightingFactor { get; set; } = 1.1;
    }

    /// <summary>
    /// Result from individual validator within ensemble
    /// </summary>
    public class ValidatorResult
    {
        public double Score { get; set; }
        public double Confidence { get; set; }
        public List<string> Recommendations { get; set; } = new();
        public string ValidatorType { get; set; } = string.Empty;
        public Dictionary<string, double>? FeatureImportance { get; set; }
        public string? ErrorMessage { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
    }

    /// <summary>
    /// Comprehensive result from ensemble scoring engine
    /// </summary>
    public class EnsembleValidationResult
    {
        public AnalysisType AnalysisType { get; set; }
        public string SuggestionText { get; set; } = string.Empty;
        public DateTime ValidationStartTime { get; set; }
        public DateTime ValidationEndTime { get; set; }
        public TimeSpan ValidationDuration { get; set; }

        // Core ensemble results
        public double EnsembleScore { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> WeightDistribution { get; set; } = new();
        public Dictionary<string, ValidatorResult> IndividualResults { get; set; } = new();

        // Model interpretability
        public Dictionary<string, string> ModelExplanations { get; set; } = new();
        public Dictionary<string, double> FeatureImportance { get; set; } = new();
        public double PredictionUncertainty { get; set; }
        public (double Lower, double Upper) ConfidenceInterval { get; set; }

        // Recommendations and insights
        public List<string> EnsembleRecommendations { get; set; } = new();
        public string QualityAssessment { get; set; } = string.Empty;
        public List<string> ImprovementSuggestions { get; set; } = new();

        // Performance metrics
        public double ValidatorAgreement { get; set; }
        public Dictionary<string, double> ValidatorContributions { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Training data for ensemble optimization
    /// </summary>
    public class EnsembleTrainingData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public double ActualQualityScore { get; set; }
        public ValidationContext Context { get; set; } = new();
        public AnalysisType AnalysisType { get; set; }
        public Dictionary<string, double> ValidatorScores { get; set; } = new();
        public Dictionary<string, double> ValidatorConfidences { get; set; } = new();
        public DateTime TrainingDate { get; set; } = DateTime.UtcNow;
        public string ExpertAnnotation { get; set; } = string.Empty;
        public double QualityVariance { get; set; }
    }

    #endregion

    #region Optimization Models

    /// <summary>
    /// Optimization strategy for ensemble weight adjustment
    /// </summary>
    public enum OptimizationStrategy
    {
        GridSearch,
        GradientBased,
        BayesianOptimization,
        GeneticAlgorithm,
        SimulatedAnnealing,
        ParticleSwarmOptimization
    }

    /// <summary>
    /// Result from weight optimization process
    /// </summary>
    public class WeightOptimizationResult
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime OptimizationStartTime { get; set; }
        public DateTime OptimizationEndTime { get; set; }
        public TimeSpan OptimizationDuration { get; set; }

        // Optimization results
        public Dictionary<string, double> InitialWeights { get; set; } = new();
        public Dictionary<string, double> OptimalWeights { get; set; } = new();
        public double PerformanceImprovement { get; set; }
        public string OptimizationMethod { get; set; } = string.Empty;

        // Validation results
        public double ValidationAccuracy { get; set; }
        public double ValidationConfidence { get; set; }
        public bool ConfigurationUpdated { get; set; }

        // Optimization process details
        public int IterationsPerformed { get; set; }
        public double ConvergenceThreshold { get; set; }
        public List<OptimizationStep> OptimizationSteps { get; set; } = new();
        public Dictionary<string, double> PerformanceMetrics { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Individual step in optimization process
    /// </summary>
    public class OptimizationStep
    {
        public int StepNumber { get; set; }
        public Dictionary<string, double> Weights { get; set; } = new();
        public double PerformanceScore { get; set; }
        public double ImprovementFromPrevious { get; set; }
        public TimeSpan StepDuration { get; set; }
        public string OptimizationNotes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Generic optimization result for internal use
    /// </summary>
    public class OptimizationResult
    {
        public string Method { get; set; } = string.Empty;
        public Dictionary<string, double> OptimalWeights { get; set; } = new();
        public double AccuracyImprovement { get; set; }
        public int IterationsUsed { get; set; }
        public double ConvergenceScore { get; set; }
    }

    /// <summary>
    /// Validation result for optimized weights
    /// </summary>
    public class ValidationResult
    {
        public double Accuracy { get; set; }
        public double Confidence { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public Dictionary<string, double> ClassificationMetrics { get; set; } = new();
    }

    #endregion

    #region Performance Tracking Models

    /// <summary>
    /// Performance metrics for ensemble tracking
    /// </summary>
    public class EnsemblePerformanceMetrics
    {
        public AnalysisType AnalysisType { get; set; }
        public double OverallAccuracy { get; set; }
        public double AverageConfidence { get; set; }
        public int TotalPredictions { get; set; }
        public DateTime LastUpdateTime { get; set; } = DateTime.UtcNow;
        public Dictionary<string, double> ValidatorPerformance { get; set; } = new();
        public List<PerformanceDataPoint> PerformanceHistory { get; set; } = new();
        public Dictionary<string, double> WeightStability { get; set; } = new();
        public double PredictionVariance { get; set; }
    }

    /// <summary>
    /// Individual performance data point
    /// </summary>
    public class PerformanceDataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Accuracy { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> ValidatorScores { get; set; } = new();
        public Dictionary<string, double> WeightsUsed { get; set; } = new();
    }

    /// <summary>
    /// Weight optimization history tracking
    /// </summary>
    public class ModelWeightHistory
    {
        public string ValidatorName { get; set; } = string.Empty;
        public AnalysisType AnalysisType { get; set; }
        public List<WeightOptimizationRecord> OptimizationHistory { get; set; } = new();
        public double CurrentWeight { get; set; }
        public double AverageWeight { get; set; }
        public double WeightVariance { get; set; }
        public DateTime LastOptimization { get; set; }
    }

    /// <summary>
    /// Individual weight optimization record
    /// </summary>
    public class WeightOptimizationRecord
    {
        public DateTime OptimizationDate { get; set; }
        public double PreviousWeight { get; set; }
        public double NewWeight { get; set; }
        public double WeightChange { get; set; }
        public double PerformanceImprovement { get; set; }
        public string OptimizationReason { get; set; } = string.Empty;
        public int SampleSize { get; set; }
    }

    #endregion

    #region Reporting Models

    /// <summary>
    /// Comprehensive ensemble performance report
    /// </summary>
    public class EnsemblePerformanceReport
    {
        public DateTime GenerationTime { get; set; } = DateTime.UtcNow;
        public Dictionary<AnalysisType, AnalysisTypePerformanceReport> AnalysisTypeReports { get; set; } = new();
        public EnsembleOverallStatistics OverallStatistics { get; set; } = new();
        public List<string> SystemRecommendations { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Performance report for specific analysis type
    /// </summary>
    public class AnalysisTypePerformanceReport
    {
        public AnalysisType AnalysisType { get; set; }
        public double CurrentAccuracy { get; set; }
        public double AverageConfidence { get; set; }
        public int PredictionCount { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public Dictionary<string, double> CurrentWeights { get; set; } = new();
        public List<WeightOptimizationRecord> WeightOptimizationHistory { get; set; } = new();
        public double AccuracyTrend { get; set; }
        public double ConfidenceTrend { get; set; }
        public List<string> ImprovementRecommendations { get; set; } = new();
        public Dictionary<string, double> ValidatorContributions { get; set; } = new();
    }

    /// <summary>
    /// Overall ensemble statistics
    /// </summary>
    public class EnsembleOverallStatistics
    {
        public double AverageAccuracy { get; set; }
        public double AverageConfidence { get; set; }
        public int TotalPredictions { get; set; }
        public AnalysisType BestPerformingAnalysisType { get; set; }
        public AnalysisType WorstPerformingAnalysisType { get; set; }
        public double AccuracyVariance { get; set; }
        public double ConfidenceVariance { get; set; }
        public DateTime LastOptimization { get; set; }
        public int OptimizationsPerformed { get; set; }
        public double AverageOptimizationImprovement { get; set; }
    }

    #endregion

    #region Advanced Ensemble Features

    /// <summary>
    /// Dynamic weight adjustment configuration
    /// </summary>
    public class DynamicWeightConfig
    {
        public bool EnableConfidenceWeighting { get; set; } = true;
        public bool EnableComplexityWeighting { get; set; } = true;
        public bool EnablePerformanceWeighting { get; set; } = true;
        public bool EnableContextualWeighting { get; set; } = true;
        public double ConfidenceWeightFactor { get; set; } = 1.2;
        public double ComplexityWeightFactor { get; set; } = 1.1;
        public double PerformanceWeightFactor { get; set; } = 1.3;
        public double ContextualWeightFactor { get; set; } = 1.05;
        public double MinWeightThreshold { get; set; } = 0.05;
        public double MaxWeightThreshold { get; set; } = 0.6;
    }

    /// <summary>
    /// Ensemble model interpretability features
    /// </summary>
    public class EnsembleInterpretability
    {
        public Dictionary<string, double> GlobalFeatureImportance { get; set; } = new();
        public Dictionary<string, Dictionary<string, double>> ValidatorFeatureImportance { get; set; } = new();
        public Dictionary<string, string> ModelExplanations { get; set; } = new();
        public List<string> KeyFactors { get; set; } = new();
        public Dictionary<string, double> UncertaintyContributions { get; set; } = new();
        public string OverallExplanation { get; set; } = string.Empty;
        public double ExplanationConfidence { get; set; }
    }

    /// <summary>
    /// Ensemble quality assessment framework
    /// </summary>
    public class EnsembleQualityAssessment
    {
        public double OverallQualityScore { get; set; }
        public Dictionary<string, double> QualityDimensions { get; set; } = new();
        public List<string> QualityStrengths { get; set; } = new();
        public List<string> QualityWeaknesses { get; set; } = new();
        public List<string> ImprovementRecommendations { get; set; } = new();
        public double QualityTrend { get; set; }
        public string QualityClassification { get; set; } = string.Empty; // Excellent, Good, Fair, Poor
        public Dictionary<string, double> ValidatorQualityContributions { get; set; } = new();
    }

    /// <summary>
    /// Ensemble monitoring and alerting configuration
    /// </summary>
    public class EnsembleMonitoringConfig
    {
        public bool EnablePerformanceMonitoring { get; set; } = true;
        public bool EnableDriftDetection { get; set; } = true;
        public bool EnableAnomalyDetection { get; set; } = true;
        public double AccuracyThreshold { get; set; } = 0.85;
        public double ConfidenceThreshold { get; set; } = 0.8;
        public double DriftThreshold { get; set; } = 0.1;
        public TimeSpan MonitoringInterval { get; set; } = TimeSpan.FromHours(1);
        public int AlertCooldownMinutes { get; set; } = 30;
        public List<string> AlertChannels { get; set; } = new();
        public Dictionary<string, double> CustomThresholds { get; set; } = new();
    }

    /// <summary>
    /// Ensemble A/B testing configuration
    /// </summary>
    public class EnsembleABTestConfig
    {
        public string TestName { get; set; } = string.Empty;
        public Dictionary<string, double> ControlWeights { get; set; } = new();
        public Dictionary<string, double> TreatmentWeights { get; set; } = new();
        public double TrafficSplit { get; set; } = 0.5;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(14);
        public List<string> SuccessMetrics { get; set; } = new();
        public double StatisticalSignificance { get; set; } = 0.05;
        public int MinSampleSize { get; set; } = 100;
        public bool AutoPromoteWinner { get; set; } = false;
    }

    /// <summary>
    /// Ensemble feature selection and engineering
    /// </summary>
    public class EnsembleFeatureConfig
    {
        public bool EnableFeatureSelection { get; set; } = true;
        public bool EnableFeatureEngineering { get; set; } = true;
        public List<string> RequiredFeatures { get; set; } = new();
        public List<string> OptionalFeatures { get; set; } = new();
        public Dictionary<string, double> FeatureWeights { get; set; } = new();
        public int MaxFeatures { get; set; } = 50;
        public double FeatureImportanceThreshold { get; set; } = 0.01;
        public string FeatureSelectionMethod { get; set; } = "Mutual Information";
        public Dictionary<string, object> FeatureEngineeringParams { get; set; } = new();
    }

    #endregion

    #region Utility Models

    /// <summary>
    /// Ensemble configuration validation result
    /// </summary>
    public class EnsembleConfigValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
        public List<string> ValidationWarnings { get; set; } = new();
        public Dictionary<string, object> SuggestedCorrections { get; set; } = new();
        public double ConfigurationScore { get; set; }
    }

    /// <summary>
    /// Ensemble deployment readiness assessment
    /// </summary>
    public class EnsembleDeploymentReadiness
    {
        public bool IsReadyForDeployment { get; set; }
        public double ReadinessScore { get; set; }
        public List<string> ReadinessCriteria { get; set; } = new();
        public List<string> BlockingIssues { get; set; } = new();
        public List<string> RecommendedActions { get; set; } = new();
        public Dictionary<string, double> ComponentReadiness { get; set; } = new();
        public DateTime AssessmentDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Ensemble versioning and rollback configuration
    /// </summary>
    public class EnsembleVersionConfig
    {
        public string CurrentVersion { get; set; } = "1.0";
        public Dictionary<string, Dictionary<string, double>> VersionWeights { get; set; } = new();
        public Dictionary<string, EnsemblePerformanceMetrics> VersionPerformance { get; set; } = new();
        public bool EnableAutoRollback { get; set; } = true;
        public double RollbackThreshold { get; set; } = 0.05; // 5% performance degradation
        public int MaxVersionHistory { get; set; } = 10;
        public TimeSpan RollbackCooldown { get; set; } = TimeSpan.FromHours(1);
    }

    #endregion
}

