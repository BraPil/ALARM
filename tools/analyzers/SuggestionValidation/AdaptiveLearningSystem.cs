using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Text.Json;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Adaptive Learning System for Phase 2 Week 4
    /// Provides real-time weight adjustment and feedback loops for ML model ensemble optimization
    /// Target: 95%+ accuracy through continuous learning and dynamic adaptation
    /// </summary>
    public class AdaptiveLearningSystem
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<AdaptiveLearningSystem> _logger;
        private readonly EnsembleScoringEngine _ensembleEngine;
        private readonly FeedbackIntegrationService _feedbackService;
        private readonly ValidationModelManager _validationModelManager;
        private readonly Dictionary<AnalysisType, AdaptiveLearningModel> _adaptiveModels;
        private readonly Dictionary<AnalysisType, LearningHistory> _learningHistory;
        private readonly AdaptiveLearningConfig _config;

        public AdaptiveLearningSystem(
            MLContext mlContext,
            ILogger<AdaptiveLearningSystem> logger,
            EnsembleScoringEngine ensembleEngine,
            FeedbackIntegrationService feedbackService,
            ValidationModelManager validationModelManager,
            AdaptiveLearningConfig? config = null)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ensembleEngine = ensembleEngine ?? throw new ArgumentNullException(nameof(ensembleEngine));
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _validationModelManager = validationModelManager ?? throw new ArgumentNullException(nameof(validationModelManager));
            
            _adaptiveModels = new Dictionary<AnalysisType, AdaptiveLearningModel>();
            _learningHistory = new Dictionary<AnalysisType, LearningHistory>();
            _config = config ?? new AdaptiveLearningConfig();

            InitializeAdaptiveLearningModels();
        }

        #region Public API

        /// <summary>
        /// Process feedback and adapt model weights in real-time
        /// </summary>
        public async Task<AdaptiveLearningResult> ProcessFeedbackAsync(
            string suggestionText,
            ValidationContext context,
            AnalysisType analysisType,
            double actualQualityScore,
            double predictedQualityScore,
            Dictionary<string, double> validatorScores)
        {
            _logger.LogInformation("Processing adaptive learning feedback for {AnalysisType}", analysisType);

            var result = new AdaptiveLearningResult
            {
                AnalysisType = analysisType,
                SuggestionText = suggestionText,
                ProcessingStartTime = DateTime.UtcNow
            };

            try
            {
                // Calculate prediction error
                var predictionError = Math.Abs(actualQualityScore - predictedQualityScore);
                result.PredictionError = predictionError;
                result.ActualScore = actualQualityScore;
                result.PredictedScore = predictedQualityScore;

                // Update learning history
                await UpdateLearningHistoryAsync(analysisType, suggestionText, actualQualityScore, 
                    predictedQualityScore, validatorScores);

                // Perform adaptive weight adjustment if error exceeds threshold
                if (predictionError > _config.ErrorThresholdForAdaptation)
                {
                    var weightAdjustment = await PerformAdaptiveWeightAdjustmentAsync(
                        analysisType, predictionError, validatorScores, actualQualityScore);
                    result.WeightAdjustments = weightAdjustment;
                }

                // Update adaptive model with new data point
                await UpdateAdaptiveModelAsync(analysisType, suggestionText, context, 
                    actualQualityScore, validatorScores);

                // Check if model retraining is needed
                if (ShouldRetrain(analysisType))
                {
                    var retrainingResult = await RetrainAdaptiveModelAsync(analysisType);
                    result.RetrainingPerformed = retrainingResult.Success;
                    result.NewModelAccuracy = retrainingResult.Accuracy;
                }

                // Generate learning insights
                result.LearningInsights = GenerateLearningInsights(analysisType, predictionError);

                // Update real-time learning metrics
                await UpdateRealTimeLearningMetricsAsync(analysisType, result);

                _logger.LogInformation("Adaptive learning completed for {AnalysisType}. Error: {Error:F3}, Adjustments: {Adjustments}",
                    analysisType, predictionError, result.WeightAdjustments?.Count ?? 0);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing adaptive learning feedback for {AnalysisType}", analysisType);
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                result.ProcessingEndTime = DateTime.UtcNow;
                result.ProcessingDuration = result.ProcessingEndTime - result.ProcessingStartTime;
            }
        }

        /// <summary>
        /// Continuously monitor and adjust ensemble weights based on performance
        /// </summary>
        public async Task<ContinuousLearningResult> PerformContinuousLearningAsync(AnalysisType analysisType)
        {
            _logger.LogInformation("Performing continuous learning optimization for {AnalysisType}", analysisType);

            var result = new ContinuousLearningResult
            {
                AnalysisType = analysisType,
                OptimizationStartTime = DateTime.UtcNow
            };

            try
            {
                // Analyze recent performance trends
                var performanceTrends = await AnalyzePerformanceTrendsAsync(analysisType);
                result.PerformanceTrends = performanceTrends;

                // Detect concept drift
                var driftDetection = await DetectConceptDriftAsync(analysisType);
                result.ConceptDriftDetected = driftDetection.DriftDetected;
                result.DriftSeverity = driftDetection.Severity;

                // Perform online learning updates
                if (performanceTrends.RequiresOptimization || driftDetection.DriftDetected)
                {
                    var onlineLearningResult = await PerformOnlineLearningAsync(analysisType);
                    result.OnlineLearningApplied = onlineLearningResult.Success;
                    result.PerformanceImprovement = onlineLearningResult.Improvement;
                }

                // Update adaptive thresholds
                await UpdateAdaptiveThresholdsAsync(analysisType, performanceTrends);

                // Generate optimization recommendations
                result.OptimizationRecommendations = GenerateOptimizationRecommendations(
                    analysisType, performanceTrends, driftDetection);

                _logger.LogInformation("Continuous learning completed for {AnalysisType}. Drift: {Drift}, Improvement: {Improvement:P2}",
                    analysisType, driftDetection.DriftDetected, result.PerformanceImprovement);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing continuous learning for {AnalysisType}", analysisType);
                result.ErrorMessage = ex.Message;
                return result;
            }
            finally
            {
                result.OptimizationEndTime = DateTime.UtcNow;
                result.OptimizationDuration = result.OptimizationEndTime - result.OptimizationStartTime;
            }
        }

        /// <summary>
        /// Generate comprehensive adaptive learning report
        /// </summary>
        public async Task<AdaptiveLearningReport> GenerateAdaptiveLearningReportAsync()
        {
            _logger.LogInformation("Generating comprehensive adaptive learning report");

            var report = new AdaptiveLearningReport
            {
                GenerationTime = DateTime.UtcNow
            };

            try
            {
                var analysisTypes = Enum.GetValues<AnalysisType>();

                foreach (var analysisType in analysisTypes)
                {
                    var typeReport = new AnalysisTypeLearningReport
                    {
                        AnalysisType = analysisType
                    };

                    if (_learningHistory.ContainsKey(analysisType))
                    {
                        var history = _learningHistory[analysisType];
                        typeReport.TotalLearningCycles = history.LearningCycles.Count;
                        typeReport.AveragePredictionError = history.LearningCycles.Average(c => c.PredictionError);
                        typeReport.LearningTrend = CalculateLearningTrend(history);
                        typeReport.LastLearningCycle = history.LearningCycles.LastOrDefault()?.Timestamp ?? DateTime.MinValue;
                    }

                    if (_adaptiveModels.ContainsKey(analysisType))
                    {
                        var model = _adaptiveModels[analysisType];
                        typeReport.CurrentModelAccuracy = model.CurrentAccuracy;
                        typeReport.AdaptationCount = model.AdaptationCount;
                        typeReport.LastAdaptation = model.LastAdaptation;
                    }

                    // Calculate learning effectiveness
                    typeReport.LearningEffectiveness = CalculateLearningEffectiveness(analysisType);

                    // Generate improvement recommendations
                    typeReport.ImprovementRecommendations = GenerateLearningImprovementRecommendations(analysisType, typeReport);

                    report.AnalysisTypeReports[analysisType] = typeReport;
                }

                // Calculate overall learning statistics
                report.OverallStatistics = CalculateOverallLearningStatistics(report.AnalysisTypeReports.Values);

                // Generate system-wide recommendations
                report.SystemRecommendations = GenerateSystemWideLearningRecommendations(report);

                _logger.LogInformation("Adaptive learning report generated successfully");

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating adaptive learning report");
                report.ErrorMessage = ex.Message;
                return report;
            }
        }

        #endregion

        #region Private Implementation Methods

        private void InitializeAdaptiveLearningModels()
        {
            var analysisTypes = Enum.GetValues<AnalysisType>();
            
            foreach (var analysisType in analysisTypes)
            {
                _adaptiveModels[analysisType] = new AdaptiveLearningModel
                {
                    AnalysisType = analysisType,
                    LearningRate = _config.BaseLearningRate,
                    CreatedAt = DateTime.UtcNow,
                    CurrentAccuracy = 0.5, // Start with baseline
                    AdaptationCount = 0
                };

                _learningHistory[analysisType] = new LearningHistory
                {
                    AnalysisType = analysisType,
                    LearningCycles = new List<LearningCycle>()
                };
            }
        }

        private async Task UpdateLearningHistoryAsync(
            AnalysisType analysisType,
            string suggestionText,
            double actualScore,
            double predictedScore,
            Dictionary<string, double> validatorScores)
        {
            if (!_learningHistory.ContainsKey(analysisType))
            {
                _learningHistory[analysisType] = new LearningHistory
                {
                    AnalysisType = analysisType,
                    LearningCycles = new List<LearningCycle>()
                };
            }

            var cycle = new LearningCycle
            {
                Timestamp = DateTime.UtcNow,
                SuggestionText = suggestionText,
                ActualScore = actualScore,
                PredictedScore = predictedScore,
                PredictionError = Math.Abs(actualScore - predictedScore),
                ValidatorScores = new Dictionary<string, double>(validatorScores)
            };

            _learningHistory[analysisType].LearningCycles.Add(cycle);

            // Keep only recent history to manage memory
            if (_learningHistory[analysisType].LearningCycles.Count > _config.MaxHistorySize)
            {
                _learningHistory[analysisType].LearningCycles.RemoveAt(0);
            }
        }

        private async Task<Dictionary<string, double>> PerformAdaptiveWeightAdjustmentAsync(
            AnalysisType analysisType,
            double predictionError,
            Dictionary<string, double> validatorScores,
            double actualScore)
        {
            var adjustments = new Dictionary<string, double>();

            // Calculate error gradient for each validator
            foreach (var validator in validatorScores.Keys)
            {
                var validatorScore = validatorScores[validator];
                var validatorError = Math.Abs(actualScore - validatorScore);
                
                // Calculate adjustment based on relative performance
                var relativePerformance = 1.0 - (validatorError / Math.Max(predictionError, 0.001));
                var adjustment = _config.BaseLearningRate * relativePerformance * Math.Sign(actualScore - validatorScore);
                
                adjustments[validator] = adjustment;
            }

            // Apply adjustments to ensemble weights
            await ApplyWeightAdjustmentsAsync(analysisType, adjustments);

            return adjustments;
        }

        private async Task ApplyWeightAdjustmentsAsync(AnalysisType analysisType, Dictionary<string, double> adjustments)
        {
            // This would integrate with the EnsembleScoringEngine to update weights
            // For now, we'll log the adjustments and store them for future application
            _logger.LogInformation("Applying weight adjustments for {AnalysisType}: {Adjustments}",
                analysisType, string.Join(", ", adjustments.Select(a => $"{a.Key}:{a.Value:F3}")));

            // Store adjustments for application in next ensemble scoring cycle
            if (_adaptiveModels.ContainsKey(analysisType))
            {
                _adaptiveModels[analysisType].PendingWeightAdjustments = adjustments;
                _adaptiveModels[analysisType].LastAdaptation = DateTime.UtcNow;
                _adaptiveModels[analysisType].AdaptationCount++;
            }
        }

        private async Task UpdateAdaptiveModelAsync(
            AnalysisType analysisType,
            string suggestionText,
            ValidationContext context,
            double actualScore,
            Dictionary<string, double> validatorScores)
        {
            if (!_adaptiveModels.ContainsKey(analysisType))
                return;

            var model = _adaptiveModels[analysisType];

            // Update running accuracy estimate
            var currentError = Math.Abs(actualScore - (validatorScores.Values.Average()));
            var newAccuracy = 1.0 - currentError;
            
            model.CurrentAccuracy = (model.CurrentAccuracy * 0.9) + (newAccuracy * 0.1); // Exponential moving average
            model.LastUpdate = DateTime.UtcNow;
            model.SampleCount++;

            // Add to training buffer for potential retraining
            if (model.TrainingBuffer == null)
                model.TrainingBuffer = new List<AdaptiveTrainingPoint>();

            model.TrainingBuffer.Add(new AdaptiveTrainingPoint
            {
                SuggestionText = suggestionText,
                Context = context,
                ActualScore = actualScore,
                ValidatorScores = validatorScores,
                Timestamp = DateTime.UtcNow
            });

            // Keep buffer size manageable
            if (model.TrainingBuffer.Count > _config.MaxTrainingBufferSize)
            {
                model.TrainingBuffer.RemoveAt(0);
            }
        }

        private bool ShouldRetrain(AnalysisType analysisType)
        {
            if (!_adaptiveModels.ContainsKey(analysisType))
                return false;

            var model = _adaptiveModels[analysisType];

            // Retrain if accuracy has dropped significantly
            if (model.CurrentAccuracy < _config.AccuracyThresholdForRetraining)
                return true;

            // Retrain after certain number of adaptations
            if (model.AdaptationCount >= _config.AdaptationCountForRetraining)
                return true;

            // Retrain after time interval
            if (DateTime.UtcNow - model.LastRetraining > _config.RetrainingInterval)
                return true;

            return false;
        }

        private async Task<RetrainingResult> RetrainAdaptiveModelAsync(AnalysisType analysisType)
        {
            _logger.LogInformation("Retraining adaptive model for {AnalysisType}", analysisType);

            try
            {
                if (!_adaptiveModels.ContainsKey(analysisType))
                    return new RetrainingResult { Success = false, ErrorMessage = "Model not found" };

                var model = _adaptiveModels[analysisType];
                if (model.TrainingBuffer == null || model.TrainingBuffer.Count < _config.MinSamplesForRetraining)
                    return new RetrainingResult { Success = false, ErrorMessage = "Insufficient training data" };

                // Convert training buffer to ML.NET format
                var trainingData = model.TrainingBuffer.Select(point => new AdaptiveMLData
                {
                    SuggestionText = point.SuggestionText,
                    ActualScore = (float)point.ActualScore,
                    ValidatorScore1 = (float)(point.ValidatorScores.Values.FirstOrDefault()),
                    ValidatorScore2 = (float)(point.ValidatorScores.Values.Skip(1).FirstOrDefault()),
                    ValidatorScore3 = (float)(point.ValidatorScores.Values.Skip(2).FirstOrDefault()),
                    WordCount = point.SuggestionText.Split(' ').Length,
                    TextLength = point.SuggestionText.Length
                });

                var mlData = _mlContext.Data.LoadFromEnumerable(trainingData);

                // Create training pipeline
                var pipeline = _mlContext.Transforms.Text.FeaturizeText("TextFeatures", nameof(AdaptiveMLData.SuggestionText))
                    .Append(_mlContext.Transforms.Concatenate("Features",
                        "TextFeatures",
                        nameof(AdaptiveMLData.ValidatorScore1),
                        nameof(AdaptiveMLData.ValidatorScore2),
                        nameof(AdaptiveMLData.ValidatorScore3),
                        nameof(AdaptiveMLData.WordCount),
                        nameof(AdaptiveMLData.TextLength)))
                    .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(AdaptiveMLData.ActualScore)));

                // Train new model
                var trainedModel = pipeline.Fit(mlData);

                // Evaluate model
                var predictions = trainedModel.Transform(mlData);
                var metrics = _mlContext.Regression.Evaluate(predictions);

                // Update model
                model.TrainedModel = trainedModel;
                model.CurrentAccuracy = 1.0 - metrics.MeanAbsoluteError;
                model.LastRetraining = DateTime.UtcNow;
                model.AdaptationCount = 0; // Reset adaptation count

                return new RetrainingResult
                {
                    Success = true,
                    Accuracy = model.CurrentAccuracy,
                    MeanAbsoluteError = metrics.MeanAbsoluteError,
                    RootMeanSquaredError = metrics.RootMeanSquaredError
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retraining adaptive model for {AnalysisType}", analysisType);
                return new RetrainingResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        private List<string> GenerateLearningInsights(AnalysisType analysisType, double predictionError)
        {
            var insights = new List<string>();

            if (predictionError > 0.3)
            {
                insights.Add("High prediction error detected - consider validator recalibration");
            }

            if (_learningHistory.ContainsKey(analysisType))
            {
                var history = _learningHistory[analysisType];
                if (history.LearningCycles.Count >= 5)
                {
                    var recentErrors = history.LearningCycles.TakeLast(5).Select(c => c.PredictionError).ToList();
                    var trend = recentErrors.Last() - recentErrors.First();
                    
                    if (trend > 0.1)
                    {
                        insights.Add("Prediction accuracy is declining - model drift detected");
                    }
                    else if (trend < -0.1)
                    {
                        insights.Add("Prediction accuracy is improving - adaptive learning is effective");
                    }
                }
            }

            return insights;
        }

        #endregion

        #region Performance Analysis Methods

        private async Task<PerformanceTrends> AnalyzePerformanceTrendsAsync(AnalysisType analysisType)
        {
            var trends = new PerformanceTrends
            {
                AnalysisType = analysisType,
                AnalysisTimestamp = DateTime.UtcNow
            };

            if (_learningHistory.ContainsKey(analysisType))
            {
                var history = _learningHistory[analysisType];
                var recentCycles = history.LearningCycles.TakeLast(_config.TrendAnalysisWindow).ToList();

                if (recentCycles.Count >= 5)
                {
                    trends.AverageError = recentCycles.Average(c => c.PredictionError);
                    trends.ErrorTrend = CalculateLinearTrend(recentCycles.Select(c => c.PredictionError).ToList());
                    trends.AccuracyStability = CalculateStability(recentCycles.Select(c => 1.0 - c.PredictionError).ToList());
                    trends.RequiresOptimization = trends.AverageError > _config.ErrorThresholdForOptimization ||
                                                 trends.ErrorTrend > _config.TrendThresholdForOptimization;
                }
            }

            return trends;
        }

        private async Task<ConceptDriftDetection> DetectConceptDriftAsync(AnalysisType analysisType)
        {
            var detection = new ConceptDriftDetection
            {
                AnalysisType = analysisType,
                DetectionTimestamp = DateTime.UtcNow
            };

            if (_learningHistory.ContainsKey(analysisType))
            {
                var history = _learningHistory[analysisType];
                var allCycles = history.LearningCycles;

                if (allCycles.Count >= _config.MinSamplesForDriftDetection)
                {
                    // Compare recent performance to historical baseline
                    var recentCycles = allCycles.TakeLast(_config.DriftDetectionWindow).ToList();
                    var historicalCycles = allCycles.Take(allCycles.Count - _config.DriftDetectionWindow).ToList();

                    if (historicalCycles.Any())
                    {
                        var recentAccuracy = recentCycles.Average(c => 1.0 - c.PredictionError);
                        var historicalAccuracy = historicalCycles.Average(c => 1.0 - c.PredictionError);
                        var accuracyDrift = Math.Abs(recentAccuracy - historicalAccuracy);

                        detection.DriftDetected = accuracyDrift > _config.DriftThreshold;
                        detection.Severity = accuracyDrift / _config.DriftThreshold;
                        detection.AccuracyDrift = accuracyDrift;
                    }
                }
            }

            return detection;
        }

        private async Task<OnlineLearningResult> PerformOnlineLearningAsync(AnalysisType analysisType)
        {
            _logger.LogInformation("Performing online learning for {AnalysisType}", analysisType);

            try
            {
                if (!_adaptiveModels.ContainsKey(analysisType))
                    return new OnlineLearningResult { Success = false };

                var model = _adaptiveModels[analysisType];
                var previousAccuracy = model.CurrentAccuracy;

                // Apply incremental learning updates
                if (model.TrainingBuffer != null && model.TrainingBuffer.Count >= _config.MinSamplesForOnlineLearning)
                {
                    var recentSamples = model.TrainingBuffer.TakeLast(_config.OnlineLearningBatchSize).ToList();
                    
                    // Update model with recent samples using online learning techniques
                    await ApplyOnlineLearningUpdatesAsync(analysisType, recentSamples);
                    
                    var newAccuracy = await EvaluateCurrentModelAccuracyAsync(analysisType);
                    model.CurrentAccuracy = newAccuracy;

                    return new OnlineLearningResult
                    {
                        Success = true,
                        Improvement = newAccuracy - previousAccuracy,
                        SamplesProcessed = recentSamples.Count
                    };
                }

                return new OnlineLearningResult { Success = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing online learning for {AnalysisType}", analysisType);
                return new OnlineLearningResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        #endregion

        #region Utility Methods

        private double CalculateLinearTrend(List<double> values)
        {
            if (values.Count < 2) return 0;
            
            var n = values.Count;
            var sumX = n * (n - 1) / 2;
            var sumY = values.Sum();
            var sumXY = values.Select((y, i) => i * y).Sum();
            var sumX2 = n * (n - 1) * (2 * n - 1) / 6;
            
            return (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        }

        private double CalculateStability(List<double> values)
        {
            if (values.Count < 2) return 1.0;
            
            var mean = values.Average();
            var variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count;
            return 1.0 / (1.0 + Math.Sqrt(variance));
        }

        private double CalculateLearningTrend(LearningHistory history)
        {
            if (history.LearningCycles.Count < 5) return 0;
            
            var recentErrors = history.LearningCycles.TakeLast(10).Select(c => c.PredictionError).ToList();
            return -CalculateLinearTrend(recentErrors); // Negative because lower error is better
        }

        private double CalculateLearningEffectiveness(AnalysisType analysisType)
        {
            if (!_learningHistory.ContainsKey(analysisType) || !_adaptiveModels.ContainsKey(analysisType))
                return 0;

            var history = _learningHistory[analysisType];
            var model = _adaptiveModels[analysisType];

            if (history.LearningCycles.Count < 10)
                return model.CurrentAccuracy;

            var initialAccuracy = 1.0 - history.LearningCycles.Take(5).Average(c => c.PredictionError);
            var currentAccuracy = model.CurrentAccuracy;

            return (currentAccuracy - initialAccuracy) / Math.Max(initialAccuracy, 0.1);
        }

        #endregion

        #region Placeholder Methods for Comprehensive Framework

        private async Task UpdateRealTimeLearningMetricsAsync(AnalysisType analysisType, AdaptiveLearningResult result)
        {
            // Placeholder - would update real-time metrics dashboard
            _logger.LogDebug("Updating real-time learning metrics for {AnalysisType}", analysisType);
        }

        private async Task UpdateAdaptiveThresholdsAsync(AnalysisType analysisType, PerformanceTrends trends)
        {
            // Placeholder - would update adaptive thresholds based on performance
            _logger.LogDebug("Updating adaptive thresholds for {AnalysisType}", analysisType);
        }

        private List<string> GenerateOptimizationRecommendations(
            AnalysisType analysisType, 
            PerformanceTrends trends, 
            ConceptDriftDetection drift)
        {
            var recommendations = new List<string>();
            
            if (trends.RequiresOptimization)
            {
                recommendations.Add("Performance optimization needed - consider weight adjustment");
            }
            
            if (drift.DriftDetected)
            {
                recommendations.Add($"Concept drift detected (severity: {drift.Severity:F2}) - model retraining recommended");
            }
            
            return recommendations;
        }

        private List<string> GenerateLearningImprovementRecommendations(AnalysisType analysisType, AnalysisTypeLearningReport report)
        {
            var recommendations = new List<string>();
            
            if (report.LearningEffectiveness < 0.1)
            {
                recommendations.Add("Low learning effectiveness - review learning rate and training data quality");
            }
            
            if (report.AveragePredictionError > 0.2)
            {
                recommendations.Add("High prediction error - consider feature engineering improvements");
            }
            
            return recommendations;
        }

        private LearningOverallStatistics CalculateOverallLearningStatistics(IEnumerable<AnalysisTypeLearningReport> reports)
        {
            var reportList = reports.ToList();
            return new LearningOverallStatistics
            {
                AverageAccuracy = reportList.Average(r => r.CurrentModelAccuracy),
                AverageLearningEffectiveness = reportList.Average(r => r.LearningEffectiveness),
                TotalAdaptations = reportList.Sum(r => r.AdaptationCount),
                BestPerformingAnalysisType = reportList.OrderByDescending(r => r.CurrentModelAccuracy).First().AnalysisType
            };
        }

        private List<string> GenerateSystemWideLearningRecommendations(AdaptiveLearningReport report)
        {
            var recommendations = new List<string>();
            
            if (report.OverallStatistics.AverageAccuracy < 0.85)
            {
                recommendations.Add("System-wide accuracy below target - comprehensive optimization needed");
            }
            
            return recommendations;
        }

        private async Task ApplyOnlineLearningUpdatesAsync(AnalysisType analysisType, List<AdaptiveTrainingPoint> samples)
        {
            // Placeholder - would apply incremental learning updates
            _logger.LogDebug("Applying online learning updates for {AnalysisType} with {SampleCount} samples", 
                analysisType, samples.Count);
        }

        private async Task<double> EvaluateCurrentModelAccuracyAsync(AnalysisType analysisType)
        {
            // Placeholder - would evaluate current model accuracy
            return _adaptiveModels.ContainsKey(analysisType) ? _adaptiveModels[analysisType].CurrentAccuracy : 0.5;
        }

        #endregion
    }
}
