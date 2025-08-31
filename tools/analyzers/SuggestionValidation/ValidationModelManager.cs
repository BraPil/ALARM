using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ML;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Manages ML models for automated suggestion quality prediction
    /// </summary>
    public class ValidationModelManager
    {
        private readonly MLContext _mlContext;
        private readonly ILogger _logger;
        private readonly Dictionary<AnalysisType, ITransformer> _trainedModels;
        private readonly Dictionary<AnalysisType, LearningModelMetrics> _modelMetrics;

        public ValidationModelManager(MLContext mlContext, ILogger logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _trainedModels = new Dictionary<AnalysisType, ITransformer>();
            _modelMetrics = new Dictionary<AnalysisType, LearningModelMetrics>();
        }

        /// <summary>
        /// Predict quality score for a suggestion using trained ML model
        /// </summary>
        public async Task<ValidationPrediction> PredictSuggestionQualityAsync(
            string suggestionText, 
            AnalysisType analysisType,
            Dictionary<string, object> context)
        {
            _logger.LogDebug("Predicting suggestion quality for {AnalysisType}", analysisType);

            var prediction = new ValidationPrediction
            {
                PredictedQualityScore = 0.5, // Default score
                Confidence = 0.5,
                QualityBreakdown = new Dictionary<string, double>(),
                PredictedIssues = new List<string>(),
                SuggestedImprovements = new List<string>()
            };

            // Use rule-based prediction if no trained model available
            if (!_trainedModels.ContainsKey(analysisType))
            {
                prediction = await PredictUsingRulesAsync(suggestionText, analysisType, context);
            }
            else
            {
                prediction = await PredictUsingMLModelAsync(suggestionText, analysisType, context);
            }

            return prediction;
        }

        /// <summary>
        /// Train or update ML model with new validation data
        /// </summary>
        public async Task<bool> TrainModelAsync(
            AnalysisType analysisType,
            IEnumerable<SuggestionTrainingData> trainingData)
        {
            _logger.LogInformation("Training validation model for {AnalysisType}", analysisType);

            try
            {
                var trainingDataList = trainingData.ToList();
                if (trainingDataList.Count < 50) // Need minimum training data
                {
                    _logger.LogWarning("Insufficient training data for {AnalysisType}: {Count} samples", 
                        analysisType, trainingDataList.Count);
                    return false;
                }

                // Convert to ML.NET format
                var mlTrainingData = _mlContext.Data.LoadFromEnumerable(
                    trainingDataList.Select(td => new SuggestionMLData
                    {
                        SuggestionText = td.SuggestionText,
                        QualityScore = (float)td.ActualQualityScore,
                        WordCount = td.SuggestionText.Split(' ').Length,
                        HasSpecificActions = td.SuggestionText.ToLower().Contains("implement") || td.SuggestionText.ToLower().Contains("configure"),
                        HasQuantifiableElements = td.SuggestionText.Contains("%") || td.SuggestionText.ToLower().Contains("percent"),
                        SuggestionLength = td.SuggestionText.Length
                    }));

                // Build training pipeline
                var pipeline = _mlContext.Transforms.Text.FeaturizeText("SuggestionFeatures", nameof(SuggestionMLData.SuggestionText))
                    .Append(_mlContext.Transforms.Concatenate("Features", 
                        "SuggestionFeatures", 
                        nameof(SuggestionMLData.WordCount),
                        nameof(SuggestionMLData.HasSpecificActions),
                        nameof(SuggestionMLData.HasQuantifiableElements),
                        nameof(SuggestionMLData.SuggestionLength)))
                    .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(SuggestionMLData.QualityScore)));

                // Train model
                var model = pipeline.Fit(mlTrainingData);
                _trainedModels[analysisType] = model;

                // Calculate model metrics
                var metrics = await EvaluateModelAsync(model, mlTrainingData);
                _modelMetrics[analysisType] = metrics;

                _logger.LogInformation("Model training completed for {AnalysisType} with {Accuracy:P2} accuracy", 
                    analysisType, metrics.Accuracy);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error training model for {AnalysisType}", analysisType);
                return false;
            }
        }

        /// <summary>
        /// Get model performance metrics
        /// </summary>
        public LearningModelMetrics? GetModelMetrics(AnalysisType analysisType)
        {
            return _modelMetrics.TryGetValue(analysisType, out var metrics) ? metrics : null;
        }

        #region Private Methods

        /// <summary>
        /// Predict using rule-based approach when no ML model is available
        /// </summary>
        private async Task<ValidationPrediction> PredictUsingRulesAsync(
            string suggestionText, 
            AnalysisType analysisType,
            Dictionary<string, object> context)
        {
            var prediction = new ValidationPrediction();
            var qualityBreakdown = new Dictionary<string, double>();

            // Text-based quality indicators
            var wordCount = suggestionText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            var hasSpecificActions = ContainsSpecificActions(suggestionText);
            var hasQuantifiableElements = ContainsQuantifiableElements(suggestionText);
            var hasKeywords = ContainsRelevantKeywords(suggestionText, analysisType);

            // Calculate quality components
            qualityBreakdown["TextQuality"] = CalculateTextQuality(suggestionText, wordCount);
            qualityBreakdown["Specificity"] = hasSpecificActions ? 0.8 : 0.4;
            qualityBreakdown["Quantifiability"] = hasQuantifiableElements ? 0.9 : 0.5;
            qualityBreakdown["DomainRelevance"] = hasKeywords ? 0.8 : 0.3;

            // Analysis type specific adjustments
            switch (analysisType)
            {
                case AnalysisType.PatternDetection:
                    qualityBreakdown["PatternAlignment"] = ContainsPatternKeywords(suggestionText) ? 0.8 : 0.4;
                    break;
                case AnalysisType.CausalAnalysis:
                    qualityBreakdown["CausalRelevance"] = ContainsCausalKeywords(suggestionText) ? 0.8 : 0.4;
                    break;
                case AnalysisType.PerformanceOptimization:
                    qualityBreakdown["PerformanceRelevance"] = ContainsPerformanceKeywords(suggestionText) ? 0.8 : 0.4;
                    break;
            }

            prediction.QualityBreakdown = qualityBreakdown;
            prediction.PredictedQualityScore = qualityBreakdown.Values.Average();
            prediction.Confidence = CalculateRuleBasedConfidence(qualityBreakdown);

            // Generate predicted issues
            if (prediction.PredictedQualityScore < 0.6)
            {
                prediction.PredictedIssues.Add("Low overall quality score predicted");
            }
            if (!hasSpecificActions)
            {
                prediction.PredictedIssues.Add("Lacks specific actionable guidance");
            }
            if (!hasQuantifiableElements)
            {
                prediction.PredictedIssues.Add("Missing quantifiable success criteria");
            }

            // Generate improvement suggestions
            if (!hasSpecificActions)
            {
                prediction.SuggestedImprovements.Add("Add specific implementation steps");
            }
            if (!hasQuantifiableElements)
            {
                prediction.SuggestedImprovements.Add("Include measurable outcomes or metrics");
            }
            if (wordCount < 10)
            {
                prediction.SuggestedImprovements.Add("Provide more detailed explanation");
            }

            return prediction;
        }

        /// <summary>
        /// Predict using trained ML model
        /// </summary>
        private async Task<ValidationPrediction> PredictUsingMLModelAsync(
            string suggestionText, 
            AnalysisType analysisType,
            Dictionary<string, object> context)
        {
            var model = _trainedModels[analysisType];
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SuggestionMLData, SuggestionQualityPrediction>(model);

            var input = new SuggestionMLData
            {
                SuggestionText = suggestionText,
                WordCount = suggestionText.Split(' ').Length,
                HasSpecificActions = ContainsSpecificActions(suggestionText),
                HasQuantifiableElements = ContainsQuantifiableElements(suggestionText),
                SuggestionLength = suggestionText.Length
            };

            var mlPrediction = predictionEngine.Predict(input);

            var prediction = new ValidationPrediction
            {
                PredictedQualityScore = mlPrediction.QualityScore,
                Confidence = CalculateMLConfidence(mlPrediction),
                QualityBreakdown = new Dictionary<string, double>
                {
                    ["MLPrediction"] = mlPrediction.QualityScore,
                    ["ModelConfidence"] = CalculateMLConfidence(mlPrediction)
                }
            };

            // Combine with rule-based insights
            var ruleBasedPrediction = await PredictUsingRulesAsync(suggestionText, analysisType, context);
            prediction.PredictedIssues = ruleBasedPrediction.PredictedIssues;
            prediction.SuggestedImprovements = ruleBasedPrediction.SuggestedImprovements;

            return prediction;
        }

        /// <summary>
        /// Evaluate trained model performance
        /// </summary>
        private async Task<LearningModelMetrics> EvaluateModelAsync(ITransformer model, IDataView trainingData)
        {
            // Split data for evaluation
            var dataSplit = _mlContext.Data.TrainTestSplit(trainingData, testFraction: 0.2);
            
            // Make predictions on test set
            var predictions = model.Transform(dataSplit.TestSet);
            
            // Evaluate regression metrics
            var regressionMetrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(SuggestionMLData.QualityScore));

            return new LearningModelMetrics
            {
                Accuracy = 1.0 - regressionMetrics.MeanAbsoluteError, // Convert MAE to accuracy-like metric
                Precision = CalculatePrecisionFromR2(regressionMetrics.RSquared),
                Recall = CalculateRecallFromR2(regressionMetrics.RSquared),
                F1Score = CalculateF1FromR2(regressionMetrics.RSquared),
                TrainingSamples = GetDataViewRowCount(dataSplit.TrainSet),
                ValidationSamples = GetDataViewRowCount(dataSplit.TestSet),
                LastTrainingDate = DateTime.UtcNow,
                FeatureImportances = new Dictionary<string, double>
                {
                    ["TextFeatures"] = 0.4,
                    ["WordCount"] = 0.2,
                    ["SpecificActions"] = 0.2,
                    ["QuantifiableElements"] = 0.2
                }
            };
        }

        #region Helper Methods

        private double CalculateTextQuality(string text, int wordCount)
        {
            var baseScore = 0.5;
            
            // Length considerations
            if (wordCount >= 10 && wordCount <= 50) baseScore += 0.2;
            else if (wordCount > 50 && wordCount <= 100) baseScore += 0.1;
            else if (wordCount < 5) baseScore -= 0.2;
            
            // Grammar and structure (simplified)
            if (text.Contains('.') || text.Contains('!') || text.Contains('?')) baseScore += 0.1;
            if (char.IsUpper(text[0])) baseScore += 0.05;
            
            // Technical terminology
            var technicalWords = new[] { "implement", "configure", "optimize", "analyze", "monitor", "improve" };
            if (technicalWords.Any(word => text.ToLower().Contains(word))) baseScore += 0.15;
            
            return Math.Min(1.0, baseScore);
        }

        private bool ContainsSpecificActions(string text)
        {
            var actionWords = new[] { "implement", "configure", "optimize", "add", "remove", "update", "modify", "enable", "disable" };
            return actionWords.Any(word => text.ToLower().Contains(word));
        }

        private bool ContainsQuantifiableElements(string text)
        {
            var quantifiers = new[] { "%", "percent", "times", "factor", "increase", "decrease", "improve", "reduce", "by", "to" };
            return quantifiers.Any(q => text.ToLower().Contains(q));
        }

        private bool ContainsRelevantKeywords(string text, AnalysisType analysisType)
        {
            return analysisType switch
            {
                AnalysisType.PatternDetection => ContainsPatternKeywords(text),
                AnalysisType.CausalAnalysis => ContainsCausalKeywords(text),
                AnalysisType.PerformanceOptimization => ContainsPerformanceKeywords(text),
                _ => false
            };
        }

        private bool ContainsPatternKeywords(string text)
        {
            var keywords = new[] { "pattern", "cluster", "sequence", "anomaly", "trend", "behavior", "structure" };
            return keywords.Any(keyword => text.ToLower().Contains(keyword));
        }

        private bool ContainsCausalKeywords(string text)
        {
            var keywords = new[] { "cause", "effect", "relationship", "correlation", "influence", "impact", "intervention" };
            return keywords.Any(keyword => text.ToLower().Contains(keyword));
        }

        private bool ContainsPerformanceKeywords(string text)
        {
            var keywords = new[] { "performance", "speed", "memory", "cpu", "optimize", "cache", "parallel", "async" };
            return keywords.Any(keyword => text.ToLower().Contains(keyword));
        }

        private double CalculateRuleBasedConfidence(Dictionary<string, double> qualityBreakdown)
        {
            var confidence = 0.5; // Base confidence for rule-based prediction
            
            // Higher confidence when multiple quality indicators agree
            var highQualityIndicators = qualityBreakdown.Values.Count(v => v > 0.7);
            var lowQualityIndicators = qualityBreakdown.Values.Count(v => v < 0.4);
            
            if (highQualityIndicators > lowQualityIndicators)
            {
                confidence += 0.3;
            }
            else if (lowQualityIndicators > highQualityIndicators)
            {
                confidence += 0.2; // Still confident, but in low quality prediction
            }
            
            return Math.Min(1.0, confidence);
        }

        private double CalculateMLConfidence(SuggestionQualityPrediction mlPrediction)
        {
            // Confidence based on prediction certainty
            // Higher confidence when prediction is closer to extremes (0 or 1)
            var distanceFromMiddle = Math.Abs(mlPrediction.QualityScore - 0.5);
            return Math.Min(1.0, 0.5 + distanceFromMiddle);
        }

        private double CalculatePrecisionFromR2(double rSquared)
        {
            // Convert R-squared to precision-like metric
            return Math.Max(0.0, Math.Min(1.0, rSquared));
        }

        private double CalculateRecallFromR2(double rSquared)
        {
            // Convert R-squared to recall-like metric
            return Math.Max(0.0, Math.Min(1.0, rSquared * 0.9));
        }

        private double CalculateF1FromR2(double rSquared)
        {
            var precision = CalculatePrecisionFromR2(rSquared);
            var recall = CalculateRecallFromR2(rSquared);
            
            if (precision + recall == 0) return 0.0;
            return 2 * (precision * recall) / (precision + recall);
        }

        private int GetDataViewRowCount(IDataView dataView)
        {
            // Simple row count approximation
            var cursor = dataView.GetRowCursor(dataView.Schema);
            int count = 0;
            while (cursor.MoveNext())
            {
                count++;
            }
            return count;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Training data for suggestion quality models
    /// </summary>
    public class SuggestionTrainingData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public double ActualQualityScore { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public Dictionary<string, double> QualityBreakdown { get; set; } = new();
        public DateTime ValidationDate { get; set; }
        public string ValidatorId { get; set; } = string.Empty;
    }

    /// <summary>
    /// ML.NET data structure for suggestion quality prediction
    /// </summary>
    public class SuggestionMLData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public float QualityScore { get; set; }
        public int WordCount { get; set; }
        public bool HasSpecificActions { get; set; }
        public bool HasQuantifiableElements { get; set; }
        public int SuggestionLength { get; set; }
    }

    /// <summary>
    /// ML.NET prediction output for suggestion quality
    /// </summary>
    public class SuggestionQualityPrediction
    {
        public float QualityScore { get; set; }
    }
}
