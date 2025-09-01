using System;
using System.Collections.Generic;
using Microsoft.ML.Data;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Enhanced training data structure with comprehensive features for advanced ML models
    /// </summary>
    public class EnhancedTrainingData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public double ActualQualityScore { get; set; }
        public ValidationContext Context { get; set; } = new();
        public AnalysisType AnalysisType { get; set; }
        public DateTime TrainingDate { get; set; } = DateTime.UtcNow;
        public string ExpertAnnotation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Enhanced ML.NET data structure with all features from EnhancedFeatureExtractor
    /// </summary>
    public class EnhancedSuggestionMLData
    {
        [LoadColumn(0)]
        public string SuggestionText { get; set; } = string.Empty;

        [LoadColumn(1)]
        public float QualityScore { get; set; }

        // Basic features
        [LoadColumn(2)]
        public int WordCount { get; set; }

        [LoadColumn(3)]
        public int CharacterCount { get; set; }

        [LoadColumn(4)]
        public int SentenceCount { get; set; }

        // Enhanced features
        [LoadColumn(5)]
        public float SemanticComplexity { get; set; }

        [LoadColumn(6)]
        public float TechnicalComplexity { get; set; }

        [LoadColumn(7)]
        public float ContextualRelevance { get; set; }

        // Advanced linguistic features
        [LoadColumn(8)]
        public int ActionVerbCount { get; set; }

        [LoadColumn(9)]
        public int TechnicalTermCount { get; set; }

        [LoadColumn(10)]
        public int QuantifiableElementCount { get; set; }

        [LoadColumn(11)]
        public float SpecificityScore { get; set; }

        // Domain-specific features
        [LoadColumn(12)]
        public float CADIntegrationScore { get; set; }

        [LoadColumn(13)]
        public float DatabaseOperationScore { get; set; }

        [LoadColumn(14)]
        public float LegacyMigrationScore { get; set; }

        [LoadColumn(15)]
        public float PerformanceImpactScore { get; set; }
    }

    /// <summary>
    /// Advanced validation prediction with model interpretability
    /// </summary>
    public class AdvancedValidationPrediction : ValidationPrediction
    {
        /// <summary>
        /// Contribution of each model type to the final prediction
        /// </summary>
        public Dictionary<string, double> ModelContributions { get; set; } = new();

        /// <summary>
        /// Feature importance scores for interpretability
        /// </summary>
        public Dictionary<string, double> FeatureImportance { get; set; } = new();

        /// <summary>
        /// Uncertainty quantification for the prediction
        /// </summary>
        public double PredictionUncertainty { get; set; }

        /// <summary>
        /// Confidence intervals for the quality score
        /// </summary>
        public (double Lower, double Upper) ConfidenceInterval { get; set; }

        /// <summary>
        /// Model explanation for human understanding
        /// </summary>
        public string ModelExplanation { get; set; } = string.Empty;
    }

    /// <summary>
    /// Ensemble model combining multiple ML algorithms
    /// </summary>
    public class EnsembleModel
    {
        public Dictionary<string, Microsoft.ML.ITransformer> BaseModels { get; set; } = new();
        public Dictionary<string, double> Weights { get; set; } = new();
        public AnalysisType AnalysisType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string ModelVersion { get; set; } = "1.0";
    }

    /// <summary>
    /// Transfer learning model with domain adaptation
    /// </summary>
    public class TransferLearningModel
    {
        public Microsoft.ML.ITransformer? BaseModel { get; set; }
        public Microsoft.ML.IEstimator<Microsoft.ML.ITransformer>? DomainSpecificLayers { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string SourceDomain { get; set; } = "Generic";
        public string TargetDomain { get; set; } = "ADDS";
    }

    /// <summary>
    /// Advanced model performance metrics
    /// </summary>
    public class AdvancedModelMetrics
    {
        public double Accuracy { get; set; }
        public double MeanAbsoluteError { get; set; }
        public double RootMeanSquaredError { get; set; }
        public double RSquared { get; set; }
        public string ModelType { get; set; } = string.Empty;
        public AnalysisType AnalysisType { get; set; }
        public DateTime TrainingDate { get; set; } = DateTime.UtcNow;
        
        // Advanced metrics
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public double AUC { get; set; }
        
        // Bias and fairness metrics
        public Dictionary<string, double> BiasMetrics { get; set; } = new();
        public Dictionary<string, double> FairnessMetrics { get; set; } = new();
        
        // Model interpretability
        public Dictionary<string, double> GlobalFeatureImportance { get; set; } = new();
        public double ModelComplexity { get; set; }
        public double TrainingTime { get; set; }
        public double InferenceTime { get; set; }
    }

    /// <summary>
    /// Neural network architecture configuration
    /// </summary>
    public class NeuralNetworkConfig
    {
        public int InputDimension { get; set; } = 20; // Based on enhanced features
        public List<int> HiddenLayers { get; set; } = new() { 64, 32, 16 };
        public int OutputDimension { get; set; } = 1;
        public string ActivationFunction { get; set; } = "ReLU";
        public double LearningRate { get; set; } = 0.001;
        public double Dropout { get; set; } = 0.2;
        public int BatchSize { get; set; } = 32;
        public int Epochs { get; set; } = 100;
        public string Optimizer { get; set; } = "Adam";
    }

    /// <summary>
    /// Hyperparameter tuning configuration
    /// </summary>
    public class HyperparameterConfig
    {
        public Dictionary<string, (double Min, double Max)> ContinuousParameters { get; set; } = new();
        public Dictionary<string, List<object>> DiscreteParameters { get; set; } = new();
        public int MaxTrials { get; set; } = 50;
        public string OptimizationMetric { get; set; } = "Accuracy";
        public string OptimizationDirection { get; set; } = "Maximize";
    }

    /// <summary>
    /// Cross-validation configuration
    /// </summary>
    public class CrossValidationConfig
    {
        public int Folds { get; set; } = 5;
        public bool Stratified { get; set; } = true;
        public int RandomSeed { get; set; } = 42;
        public double ValidationSplit { get; set; } = 0.2;
        public bool ShuffleSplit { get; set; } = true;
    }

    /// <summary>
    /// Model deployment configuration
    /// </summary>
    public class ModelDeploymentConfig
    {
        public string ModelName { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0";
        public string Environment { get; set; } = "Development";
        public Dictionary<string, object> DeploymentParameters { get; set; } = new();
        public bool EnableMonitoring { get; set; } = true;
        public double PerformanceThreshold { get; set; } = 0.85;
        public TimeSpan ModelRefreshInterval { get; set; } = TimeSpan.FromDays(7);
    }

    /// <summary>
    /// Model monitoring metrics
    /// </summary>
    public class ModelMonitoringMetrics
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public double CurrentAccuracy { get; set; }
        public double DataDrift { get; set; }
        public double ConceptDrift { get; set; }
        public double PredictionLatency { get; set; }
        public double ThroughputRPS { get; set; }
        public Dictionary<string, double> FeatureDrift { get; set; } = new();
        public List<string> Alerts { get; set; } = new();
    }

    /// <summary>
    /// A/B testing configuration for model comparison
    /// </summary>
    public class ABTestConfig
    {
        public string TestName { get; set; } = string.Empty;
        public string ControlModelVersion { get; set; } = string.Empty;
        public string TreatmentModelVersion { get; set; } = string.Empty;
        public double TrafficSplit { get; set; } = 0.5;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(14);
        public List<string> SuccessMetrics { get; set; } = new();
        public double StatisticalSignificance { get; set; } = 0.05;
    }

    /// <summary>
    /// Feature store configuration for ML pipeline
    /// </summary>
    public class FeatureStoreConfig
    {
        public string StoreName { get; set; } = "SuggestionValidationFeatures";
        public Dictionary<string, Type> FeatureSchema { get; set; } = new();
        public TimeSpan FeatureRetention { get; set; } = TimeSpan.FromDays(90);
        public bool EnableVersioning { get; set; } = true;
        public bool EnableLineage { get; set; } = true;
        public string ComputeEngine { get; set; } = "Local";
    }
}
