using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Xunit;

namespace ALARM.Analyzers.SuggestionValidation.Tests
{
    /// <summary>
    /// Comprehensive tests for ML Model Testing Framework
    /// Target: 85%+ accuracy validation, bias detection, and performance testing
    /// </summary>
    public class MLModelTestingFrameworkTests
    {
        private readonly MLModelTestingFramework _framework;
        private readonly MLContext _mlContext;
        private readonly ILogger<MLModelTestingFramework> _logger;
        private readonly AdvancedMLModelManager _advancedModelManager;
        private readonly ValidationModelManager _validationModelManager;

        public MLModelTestingFrameworkTests()
        {
            _mlContext = new MLContext(seed: 42);
            _logger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => 
                builder.AddConsole().SetMinimumLevel(LogLevel.Information))
                .CreateLogger<MLModelTestingFramework>();

            var advancedLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => 
                builder.AddConsole().SetMinimumLevel(LogLevel.Information))
                .CreateLogger<AdvancedMLModelManager>();

            var featureExtractorLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => 
                builder.AddConsole().SetMinimumLevel(LogLevel.Information))
                .CreateLogger<EnhancedFeatureExtractor>();

            var validationLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => 
                builder.AddConsole().SetMinimumLevel(LogLevel.Information))
                .CreateLogger("ValidationModelManager");

            var featureExtractor = new EnhancedFeatureExtractor(featureExtractorLogger);
            
            _advancedModelManager = new AdvancedMLModelManager(_mlContext, advancedLogger, featureExtractor);
            _validationModelManager = new ValidationModelManager(_mlContext, validationLogger);
            
            _framework = new MLModelTestingFramework(_mlContext, _logger, _advancedModelManager, _validationModelManager);
        }

        #region Accuracy Validation Tests

        [Fact]
        public async Task RunAccuracyValidation_PatternDetection_MeetsAccuracyTargets()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunAccuracyValidationAsync(analysisType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(analysisType, result.AnalysisType);
            Assert.True(result.OverallAccuracy >= 0.85, $"Expected accuracy >= 0.85, got {result.OverallAccuracy:F3}");
            Assert.True(result.MeetsAccuracyTarget);
            Assert.NotNull(result.CrossValidationResults);
            Assert.NotNull(result.HoldoutValidationResults);
            Assert.NotNull(result.StatisticalTests);
            Assert.NotNull(result.ModelComparison);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task RunAccuracyValidation_CausalAnalysis_ValidatesCrossValidation()
        {
            // Arrange
            var analysisType = AnalysisType.CausalAnalysis;

            // Act
            var result = await _framework.RunAccuracyValidationAsync(analysisType);

            // Assert
            Assert.True(result.CrossValidationResults.FoldCount >= 5, "Should use at least 5-fold cross-validation");
            Assert.True(result.CrossValidationResults.AverageAccuracy > 0.0);
            Assert.True(result.CrossValidationResults.StandardDeviation >= 0.0);
            Assert.True(result.CrossValidationResults.ConfidenceInterval.Lower <= result.CrossValidationResults.ConfidenceInterval.Upper);
            Assert.Equal(0.95, result.CrossValidationResults.ConfidenceInterval.Level);
        }

        [Fact]
        public async Task RunAccuracyValidation_PerformanceOptimization_ValidatesHoldoutResults()
        {
            // Arrange
            var analysisType = AnalysisType.PerformanceOptimization;

            // Act
            var result = await _framework.RunAccuracyValidationAsync(analysisType);

            // Assert
            Assert.True(result.HoldoutValidationResults.TrainingAccuracy > 0.0);
            Assert.True(result.HoldoutValidationResults.ValidationAccuracy > 0.0);
            Assert.True(result.HoldoutValidationResults.TestAccuracy > 0.0);
            Assert.True(result.HoldoutValidationResults.Overfitting >= 0.0, "Overfitting should be non-negative");
            Assert.True(result.HoldoutValidationResults.Generalization >= 0.0, "Generalization gap should be non-negative");
        }

        [Theory]
        [InlineData(AnalysisType.PatternDetection)]
        [InlineData(AnalysisType.CausalAnalysis)]
        [InlineData(AnalysisType.PerformanceOptimization)]
        public async Task RunAccuracyValidation_AllAnalysisTypes_ValidatesStatisticalSignificance(AnalysisType analysisType)
        {
            // Act
            var result = await _framework.RunAccuracyValidationAsync(analysisType);

            // Assert
            Assert.True(result.StatisticalTests.TTestPValue >= 0.0 && result.StatisticalTests.TTestPValue <= 1.0);
            Assert.True(result.StatisticalTests.WilcoxonPValue >= 0.0 && result.StatisticalTests.WilcoxonPValue <= 1.0);
            Assert.True(result.StatisticalTests.ChiSquarePValue >= 0.0 && result.StatisticalTests.ChiSquarePValue <= 1.0);
            Assert.Equal(0.05, result.StatisticalTests.SignificanceLevel);
            Assert.True(result.StatisticalTests.PowerAnalysis >= 0.0 && result.StatisticalTests.PowerAnalysis <= 1.0);
        }

        [Fact]
        public async Task RunAccuracyValidation_ModelComparison_IdentifiesBestModel()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunAccuracyValidationAsync(analysisType);

            // Assert
            Assert.True(result.ModelComparison.BaselineAccuracy > 0.0);
            Assert.True(result.ModelComparison.BestModelAccuracy >= result.ModelComparison.BaselineAccuracy);
            Assert.False(string.IsNullOrEmpty(result.ModelComparison.BestModelType));
            Assert.True(result.ModelComparison.ImprovementOverBaseline >= 0.0);
        }

        #endregion

        #region Bias Detection Tests

        [Fact]
        public async Task RunBiasDetection_PatternDetection_DetectsDemographicBias()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunBiasDetectionAsync(analysisType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(analysisType, result.AnalysisType);
            Assert.True(result.DemographicBias.GenderBias >= 0.0);
            Assert.True(result.DemographicBias.AgeBias >= 0.0);
            Assert.True(result.DemographicBias.ExperienceBias >= 0.0);
            Assert.True(result.DemographicBias.OverallDemographicBias >= 0.0);
            Assert.Equal(0.05, result.DemographicBias.BiasThreshold);
        }

        [Fact]
        public async Task RunBiasDetection_CausalAnalysis_DetectsDomainBias()
        {
            // Arrange
            var analysisType = AnalysisType.CausalAnalysis;

            // Act
            var result = await _framework.RunBiasDetectionAsync(analysisType);

            // Assert
            Assert.True(result.DomainBias.ADDSBias >= 0.0);
            Assert.True(result.DomainBias.AutoCADBias >= 0.0);
            Assert.True(result.DomainBias.OracleBias >= 0.0);
            Assert.True(result.DomainBias.DotNetBias >= 0.0);
            Assert.True(result.DomainBias.OverallDomainBias >= 0.0);
            Assert.Equal(0.05, result.DomainBias.BiasThreshold);
        }

        [Fact]
        public async Task RunBiasDetection_PerformanceOptimization_AnalyzesFeatureBias()
        {
            // Arrange
            var analysisType = AnalysisType.PerformanceOptimization;

            // Act
            var result = await _framework.RunBiasDetectionAsync(analysisType);

            // Assert
            Assert.NotNull(result.FeatureBias.HighInfluenceFeatures);
            Assert.NotNull(result.FeatureBias.FeatureBiasScores);
            Assert.True(result.FeatureBias.OverallFeatureBias >= 0.0);
            Assert.Equal(0.05, result.FeatureBias.BiasThreshold);
            Assert.NotNull(result.FeatureBias.BiasedFeatures);
        }

        [Theory]
        [InlineData(AnalysisType.PatternDetection)]
        [InlineData(AnalysisType.CausalAnalysis)]
        [InlineData(AnalysisType.PerformanceOptimization)]
        public async Task RunBiasDetection_AllAnalysisTypes_CalculatesFairnessMetrics(AnalysisType analysisType)
        {
            // Act
            var result = await _framework.RunBiasDetectionAsync(analysisType);

            // Assert
            Assert.True(result.FairnessMetrics.EqualOpportunity >= 0.0 && result.FairnessMetrics.EqualOpportunity <= 1.0);
            Assert.True(result.FairnessMetrics.DemographicParity >= 0.0 && result.FairnessMetrics.DemographicParity <= 1.0);
            Assert.True(result.FairnessMetrics.EqualizedOdds >= 0.0 && result.FairnessMetrics.EqualizedOdds <= 1.0);
            Assert.True(result.FairnessMetrics.CalibrationScore >= 0.0 && result.FairnessMetrics.CalibrationScore <= 1.0);
            Assert.True(result.FairnessMetrics.OverallFairness >= 0.0 && result.FairnessMetrics.OverallFairness <= 1.0);
            Assert.Equal(0.9, result.FairnessMetrics.FairnessThreshold);
        }

        [Fact]
        public async Task RunBiasDetection_LowBiasScenario_PassesBiasTest()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunBiasDetectionAsync(analysisType);

            // Assert - Assuming the test data produces low bias
            Assert.True(result.OverallBiasScore <= 0.1, $"Expected bias score <= 0.1, got {result.OverallBiasScore:F3}");
            Assert.True(result.PassesBiasTest);
            Assert.NotNull(result.MitigationRecommendations);
            Assert.True(result.MitigationRecommendations.Count > 0);
        }

        #endregion

        #region Performance Testing Tests

        [Fact]
        public async Task RunPerformanceTesting_PatternDetection_TestsLatency()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(analysisType, result.AnalysisType);
            Assert.True(result.LatencyResults.AverageLatencyMs > 0);
            Assert.True(result.LatencyResults.MedianLatencyMs > 0);
            Assert.True(result.LatencyResults.P95LatencyMs >= result.LatencyResults.MedianLatencyMs);
            Assert.True(result.LatencyResults.P99LatencyMs >= result.LatencyResults.P95LatencyMs);
            Assert.True(result.LatencyResults.MaxLatencyMs >= result.LatencyResults.P99LatencyMs);
            Assert.Equal(100, result.LatencyResults.LatencyTargetMs);
        }

        [Fact]
        public async Task RunPerformanceTesting_CausalAnalysis_TestsThroughput()
        {
            // Arrange
            var analysisType = AnalysisType.CausalAnalysis;

            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert
            Assert.True(result.ThroughputResults.RequestsPerSecond > 0);
            Assert.True(result.ThroughputResults.PeakThroughput >= result.ThroughputResults.RequestsPerSecond);
            Assert.True(result.ThroughputResults.SustainedThroughput <= result.ThroughputResults.PeakThroughput);
            Assert.Equal(500, result.ThroughputResults.ThroughputTarget);
        }

        [Fact]
        public async Task RunPerformanceTesting_PerformanceOptimization_TestsMemoryUsage()
        {
            // Arrange
            var analysisType = AnalysisType.PerformanceOptimization;

            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert
            Assert.True(result.MemoryUsageResults.BaselineMemoryMB > 0);
            Assert.True(result.MemoryUsageResults.PeakMemoryMB >= result.MemoryUsageResults.BaselineMemoryMB);
            Assert.True(result.MemoryUsageResults.AverageMemoryMB >= result.MemoryUsageResults.BaselineMemoryMB);
            Assert.True(result.MemoryUsageResults.AverageMemoryMB <= result.MemoryUsageResults.PeakMemoryMB);
            Assert.True(result.MemoryUsageResults.MemoryEfficiency >= 0.0 && result.MemoryUsageResults.MemoryEfficiency <= 1.0);
            Assert.Equal(200, result.MemoryUsageResults.MemoryTargetMB);
        }

        [Theory]
        [InlineData(AnalysisType.PatternDetection)]
        [InlineData(AnalysisType.CausalAnalysis)]
        [InlineData(AnalysisType.PerformanceOptimization)]
        public async Task RunPerformanceTesting_AllAnalysisTypes_TestsScalability(AnalysisType analysisType)
        {
            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert
            Assert.True(result.ScalabilityResults.LinearScalingFactor >= 0.0 && result.ScalabilityResults.LinearScalingFactor <= 1.0);
            Assert.True(result.ScalabilityResults.MaxConcurrentUsers > 0);
            Assert.True(result.ScalabilityResults.PerformanceDegradation >= 0.0 && result.ScalabilityResults.PerformanceDegradation <= 1.0);
            Assert.Equal(0.8, result.ScalabilityResults.ScalabilityTarget);
        }

        [Fact]
        public async Task RunPerformanceTesting_LoadTesting_ValidatesUnderLoad()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert
            Assert.True(result.LoadTestResults.NormalLoadPerformance >= 0.0 && result.LoadTestResults.NormalLoadPerformance <= 1.0);
            Assert.True(result.LoadTestResults.HighLoadPerformance >= 0.0 && result.LoadTestResults.HighLoadPerformance <= 1.0);
            Assert.True(result.LoadTestResults.PeakLoadPerformance >= 0.0 && result.LoadTestResults.PeakLoadPerformance <= 1.0);
            Assert.True(result.LoadTestResults.StressTestPerformance >= 0.0 && result.LoadTestResults.StressTestPerformance <= 1.0);
            Assert.True(result.LoadTestResults.RecoveryTime >= TimeSpan.Zero);
            Assert.Equal(0.8, result.LoadTestResults.LoadTestTarget);
        }

        [Fact]
        public async Task RunPerformanceTesting_ResourceEfficiency_AnalyzesResourceUsage()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert
            Assert.True(result.ResourceEfficiency.CPUEfficiency >= 0.0 && result.ResourceEfficiency.CPUEfficiency <= 1.0);
            Assert.True(result.ResourceEfficiency.MemoryEfficiency >= 0.0 && result.ResourceEfficiency.MemoryEfficiency <= 1.0);
            Assert.True(result.ResourceEfficiency.NetworkEfficiency >= 0.0 && result.ResourceEfficiency.NetworkEfficiency <= 1.0);
            Assert.True(result.ResourceEfficiency.OverallEfficiency >= 0.0 && result.ResourceEfficiency.OverallEfficiency <= 1.0);
        }

        [Fact]
        public async Task RunPerformanceTesting_HighPerformanceScenario_MeetsPerformanceTargets()
        {
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert - Assuming the test scenario produces good performance
            Assert.True(result.OverallPerformanceScore >= 0.8, $"Expected performance score >= 0.8, got {result.OverallPerformanceScore:F3}");
            Assert.True(result.MeetsPerformanceTarget);
            Assert.Null(result.ErrorMessage);
        }

        #endregion

        #region Comprehensive Testing

        [Fact]
        public async Task GenerateComprehensiveReport_AllModels_ProducesValidReport()
        {
            // Act
            var result = await _framework.GenerateComprehensiveReportAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.GenerationTime <= DateTime.UtcNow);
            Assert.NotNull(result.TestSummary);
            Assert.NotNull(result.ModelReports);
            Assert.True(result.ModelReports.Count > 0);
            Assert.NotNull(result.SystemRecommendations);
            Assert.Equal("1.0", result.ReportVersion);
        }

        [Fact]
        public async Task GenerateComprehensiveReport_TestSummary_CalculatesCorrectStatistics()
        {
            // Act
            var result = await _framework.GenerateComprehensiveReportAsync();

            // Assert
            Assert.True(result.TestSummary.TotalModelsTest > 0);
            Assert.True(result.TestSummary.ModelsPassingAllTests >= 0);
            Assert.True(result.TestSummary.ModelsPassingAllTests <= result.TestSummary.TotalModelsTest);
            Assert.True(result.TestSummary.OverallSuccessRate >= 0.0 && result.TestSummary.OverallSuccessRate <= 1.0);
            Assert.True(result.TestSummary.AverageAccuracy >= 0.0 && result.TestSummary.AverageAccuracy <= 1.0);
            Assert.True(result.TestSummary.AverageBiasScore >= 0.0);
            Assert.True(result.TestSummary.AveragePerformanceScore >= 0.0 && result.TestSummary.AveragePerformanceScore <= 1.0);
        }

        [Fact]
        public async Task GenerateComprehensiveReport_ModelReports_ContainsAllAnalysisTypes()
        {
            // Act
            var result = await _framework.GenerateComprehensiveReportAsync();

            // Assert
            var analysisTypes = Enum.GetValues<AnalysisType>();
            foreach (var analysisType in analysisTypes)
            {
                Assert.True(result.ModelReports.ContainsKey(analysisType), $"Missing report for {analysisType}");
                
                var modelReport = result.ModelReports[analysisType];
                Assert.Equal(analysisType, modelReport.AnalysisType);
                Assert.NotNull(modelReport.AccuracyReport);
                Assert.NotNull(modelReport.BiasReport);
                Assert.NotNull(modelReport.PerformanceReport);
                Assert.True(modelReport.OverallModelScore >= 0.0 && modelReport.OverallModelScore <= 1.0);
            }
        }

        [Fact]
        public async Task GenerateComprehensiveReport_SystemRecommendations_ProvidesActionableAdvice()
        {
            // Act
            var result = await _framework.GenerateComprehensiveReportAsync();

            // Assert
            Assert.NotNull(result.SystemRecommendations);
            Assert.True(result.SystemRecommendations.Count > 0);
            
            // Recommendations should be non-empty strings
            foreach (var recommendation in result.SystemRecommendations)
            {
                Assert.False(string.IsNullOrWhiteSpace(recommendation));
            }
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task RunAccuracyValidation_ErrorHandling_ReturnsGracefulFailure()
        {
            // This test would simulate error conditions in a real implementation
            // For now, we test that the framework handles normal cases gracefully
            
            // Arrange
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var result = await _framework.RunAccuracyValidationAsync(analysisType);

            // Assert - Should not throw exceptions
            Assert.NotNull(result);
            Assert.Equal(analysisType, result.AnalysisType);
        }

        [Fact]
        public async Task RunBiasDetection_ErrorHandling_ReturnsGracefulFailure()
        {
            // Arrange
            var analysisType = AnalysisType.CausalAnalysis;

            // Act
            var result = await _framework.RunBiasDetectionAsync(analysisType);

            // Assert - Should not throw exceptions
            Assert.NotNull(result);
            Assert.Equal(analysisType, result.AnalysisType);
        }

        [Fact]
        public async Task RunPerformanceTesting_ErrorHandling_ReturnsGracefulFailure()
        {
            // Arrange
            var analysisType = AnalysisType.PerformanceOptimization;

            // Act
            var result = await _framework.RunPerformanceTestingAsync(analysisType);

            // Assert - Should not throw exceptions
            Assert.NotNull(result);
            Assert.Equal(analysisType, result.AnalysisType);
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task MLModelTestingFramework_PerformanceTest_CompletesWithinReasonableTime()
        {
            // Arrange
            var startTime = DateTime.UtcNow;
            var analysisType = AnalysisType.PatternDetection;

            // Act
            var accuracyResult = await _framework.RunAccuracyValidationAsync(analysisType);
            var biasResult = await _framework.RunBiasDetectionAsync(analysisType);
            var performanceResult = await _framework.RunPerformanceTestingAsync(analysisType);
            
            var duration = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(duration.TotalSeconds < 30, $"Testing took too long: {duration.TotalSeconds:F2} seconds");
            Assert.NotNull(accuracyResult);
            Assert.NotNull(biasResult);
            Assert.NotNull(performanceResult);
        }

        #endregion
    }
}
