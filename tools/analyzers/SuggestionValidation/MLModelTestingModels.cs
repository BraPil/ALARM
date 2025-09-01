using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    #region Core Testing Models

    /// <summary>
    /// Test suite configuration for ML model testing
    /// </summary>
    public class TestSuite
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<TestCase> Tests { get; set; } = new();
    }

    /// <summary>
    /// Individual test case within a test suite
    /// </summary>
    public class TestCase
    {
        public string Name { get; set; } = string.Empty;
        public double Weight { get; set; }
        public bool Passed { get; set; }
        public double Score { get; set; }
        public string Details { get; set; } = string.Empty;
    }

    /// <summary>
    /// Overall test results for a specific model
    /// </summary>
    public class ModelTestResults
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime TestDate { get; set; } = DateTime.UtcNow;
        public double OverallScore { get; set; }
        public bool PassedAllTests { get; set; }
        public Dictionary<string, double> TestScores { get; set; } = new();
        public List<string> FailedTests { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    #endregion

    #region Accuracy Validation Models

    /// <summary>
    /// Comprehensive accuracy validation report
    /// </summary>
    public class AccuracyValidationReport
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime TestStartTime { get; set; }
        public DateTime TestEndTime { get; set; }
        public TimeSpan TestDuration { get; set; }
        
        public CrossValidationResults CrossValidationResults { get; set; } = new();
        public HoldoutValidationResults HoldoutValidationResults { get; set; } = new();
        public StatisticalTestResults StatisticalTests { get; set; } = new();
        public ModelComparisonResults ModelComparison { get; set; } = new();
        
        public double OverallAccuracy { get; set; }
        public double AccuracyTarget { get; set; } = 0.85;
        public bool MeetsAccuracyTarget { get; set; }
        
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Cross-validation test results
    /// </summary>
    public class CrossValidationResults
    {
        public int FoldCount { get; set; } = 10;
        public double AverageAccuracy { get; set; }
        public double StandardDeviation { get; set; }
        public double MinAccuracy { get; set; }
        public double MaxAccuracy { get; set; }
        public ConfidenceInterval ConfidenceInterval { get; set; } = new();
        public List<double> FoldAccuracies { get; set; } = new();
    }

    /// <summary>
    /// Holdout validation results
    /// </summary>
    public class HoldoutValidationResults
    {
        public double TrainingAccuracy { get; set; }
        public double ValidationAccuracy { get; set; }
        public double TestAccuracy { get; set; }
        public double Overfitting { get; set; }
        public double Generalization { get; set; }
    }

    /// <summary>
    /// Statistical significance test results
    /// </summary>
    public class StatisticalTestResults
    {
        public double TTestPValue { get; set; }
        public double WilcoxonPValue { get; set; }
        public double ChiSquarePValue { get; set; }
        public double EffectSize { get; set; }
        public double PowerAnalysis { get; set; }
        public double SignificanceLevel { get; set; } = 0.05;
        public bool IsStatisticallySignificant { get; set; }
    }

    /// <summary>
    /// Model comparison results
    /// </summary>
    public class ModelComparisonResults
    {
        public double BaselineAccuracy { get; set; }
        public double NeuralNetworkAccuracy { get; set; }
        public double EnsembleAccuracy { get; set; }
        public double TransferLearningAccuracy { get; set; }
        public string BestModelType { get; set; } = string.Empty;
        public double BestModelAccuracy { get; set; }
        public double ImprovementOverBaseline { get; set; }
        public Dictionary<string, double> ModelRankings { get; set; } = new();
    }

    /// <summary>
    /// Confidence interval for statistical analysis
    /// </summary>
    public class ConfidenceInterval
    {
        public double Lower { get; set; }
        public double Upper { get; set; }
        public double Level { get; set; } = 0.95;
    }

    #endregion

    #region Bias Detection Models

    /// <summary>
    /// Comprehensive bias detection report
    /// </summary>
    public class BiasDetectionReport
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime TestStartTime { get; set; }
        public DateTime TestEndTime { get; set; }
        public TimeSpan TestDuration { get; set; }
        
        public DemographicBiasResults DemographicBias { get; set; } = new();
        public DomainBiasResults DomainBias { get; set; } = new();
        public FeatureBiasResults FeatureBias { get; set; } = new();
        public FairnessMetricsResults FairnessMetrics { get; set; } = new();
        
        public double OverallBiasScore { get; set; }
        public double BiasThreshold { get; set; } = 0.1;
        public bool PassesBiasTest { get; set; }
        
        public List<string> MitigationRecommendations { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Demographic bias analysis results
    /// </summary>
    public class DemographicBiasResults
    {
        public double GenderBias { get; set; }
        public double AgeBias { get; set; }
        public double ExperienceBias { get; set; }
        public double OverallDemographicBias { get; set; }
        public double BiasThreshold { get; set; } = 0.05;
        public bool PassesDemographicTest { get; set; }
        public Dictionary<string, double> DetailedBiasScores { get; set; } = new();
    }

    /// <summary>
    /// Domain-specific bias analysis results
    /// </summary>
    public class DomainBiasResults
    {
        public double ADDSBias { get; set; }
        public double AutoCADBias { get; set; }
        public double OracleBias { get; set; }
        public double DotNetBias { get; set; }
        public double OverallDomainBias { get; set; }
        public double BiasThreshold { get; set; } = 0.05;
        public bool PassesDomainTest { get; set; }
        public Dictionary<string, double> DomainBiasBreakdown { get; set; } = new();
    }

    /// <summary>
    /// Feature bias analysis results
    /// </summary>
    public class FeatureBiasResults
    {
        public Dictionary<string, double> HighInfluenceFeatures { get; set; } = new();
        public Dictionary<string, double> FeatureBiasScores { get; set; } = new();
        public double OverallFeatureBias { get; set; }
        public double BiasThreshold { get; set; } = 0.05;
        public bool PassesFeatureTest { get; set; }
        public List<string> BiasedFeatures { get; set; } = new();
    }

    /// <summary>
    /// Fairness metrics calculation results
    /// </summary>
    public class FairnessMetricsResults
    {
        public double EqualOpportunity { get; set; }
        public double DemographicParity { get; set; }
        public double EqualizedOdds { get; set; }
        public double CalibrationScore { get; set; }
        public double OverallFairness { get; set; }
        public double FairnessThreshold { get; set; } = 0.9;
        public bool PassesFairnessTest { get; set; }
        public Dictionary<string, double> DetailedFairnessMetrics { get; set; } = new();
    }

    #endregion

    #region Performance Testing Models

    /// <summary>
    /// Comprehensive performance testing report
    /// </summary>
    public class PerformanceTestReport
    {
        public AnalysisType AnalysisType { get; set; }
        public DateTime TestStartTime { get; set; }
        public DateTime TestEndTime { get; set; }
        public TimeSpan TestDuration { get; set; }
        
        public LatencyTestResults LatencyResults { get; set; } = new();
        public ThroughputTestResults ThroughputResults { get; set; } = new();
        public MemoryUsageResults MemoryUsageResults { get; set; } = new();
        public ScalabilityTestResults ScalabilityResults { get; set; } = new();
        public LoadTestResults LoadTestResults { get; set; } = new();
        public ResourceEfficiencyAnalysis ResourceEfficiency { get; set; } = new();
        
        public double OverallPerformanceScore { get; set; }
        public double PerformanceTarget { get; set; } = 0.8;
        public bool MeetsPerformanceTarget { get; set; }
        
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Latency testing results
    /// </summary>
    public class LatencyTestResults
    {
        public double AverageLatencyMs { get; set; }
        public double MedianLatencyMs { get; set; }
        public double P95LatencyMs { get; set; }
        public double P99LatencyMs { get; set; }
        public double MaxLatencyMs { get; set; }
        public double LatencyTargetMs { get; set; } = 100;
        public bool MeetsLatencyTarget { get; set; }
        public List<double> LatencyDistribution { get; set; } = new();
    }

    /// <summary>
    /// Throughput testing results
    /// </summary>
    public class ThroughputTestResults
    {
        public double RequestsPerSecond { get; set; }
        public double PeakThroughput { get; set; }
        public double SustainedThroughput { get; set; }
        public double ThroughputTarget { get; set; } = 500;
        public bool MeetsThroughputTarget { get; set; }
        public List<double> ThroughputOverTime { get; set; } = new();
    }

    /// <summary>
    /// Memory usage testing results
    /// </summary>
    public class MemoryUsageResults
    {
        public double BaselineMemoryMB { get; set; }
        public double PeakMemoryMB { get; set; }
        public double AverageMemoryMB { get; set; }
        public double MemoryTargetMB { get; set; } = 200;
        public bool MeetsMemoryTarget { get; set; }
        public double MemoryEfficiency { get; set; }
        public List<double> MemoryUsageOverTime { get; set; } = new();
    }

    /// <summary>
    /// Scalability testing results
    /// </summary>
    public class ScalabilityTestResults
    {
        public double LinearScalingFactor { get; set; }
        public int MaxConcurrentUsers { get; set; }
        public double PerformanceDegradation { get; set; }
        public double ScalabilityTarget { get; set; } = 0.8;
        public bool MeetsScalabilityTarget { get; set; }
        public Dictionary<int, double> ScalingPerformance { get; set; } = new();
    }

    /// <summary>
    /// Load testing results
    /// </summary>
    public class LoadTestResults
    {
        public double NormalLoadPerformance { get; set; }
        public double HighLoadPerformance { get; set; }
        public double PeakLoadPerformance { get; set; }
        public double StressTestPerformance { get; set; }
        public TimeSpan RecoveryTime { get; set; }
        public double LoadTestTarget { get; set; } = 0.8;
        public bool PassesLoadTest { get; set; }
    }

    /// <summary>
    /// Resource efficiency analysis
    /// </summary>
    public class ResourceEfficiencyAnalysis
    {
        public double CPUEfficiency { get; set; }
        public double MemoryEfficiency { get; set; }
        public double NetworkEfficiency { get; set; }
        public double OverallEfficiency { get; set; }
        public Dictionary<string, double> ResourceBreakdown { get; set; } = new();
    }

    #endregion

    #region Comprehensive Reporting Models

    /// <summary>
    /// Comprehensive test report for all models
    /// </summary>
    public class ComprehensiveTestReport
    {
        public DateTime GenerationTime { get; set; } = DateTime.UtcNow;
        public TestSummary TestSummary { get; set; } = new();
        public Dictionary<AnalysisType, ModelTestReport> ModelReports { get; set; } = new();
        public List<string> SystemRecommendations { get; set; } = new();
        public string ReportVersion { get; set; } = "1.0";
    }

    /// <summary>
    /// Individual model test report
    /// </summary>
    public class ModelTestReport
    {
        public AnalysisType AnalysisType { get; set; }
        public AccuracyValidationReport AccuracyReport { get; set; } = new();
        public BiasDetectionReport BiasReport { get; set; } = new();
        public PerformanceTestReport PerformanceReport { get; set; } = new();
        
        public double OverallModelScore { get; set; }
        public bool PassesAllTests { get; set; }
        public List<string> CriticalIssues { get; set; } = new();
        public List<string> ImprovementRecommendations { get; set; } = new();
    }

    /// <summary>
    /// Summary statistics for all model tests
    /// </summary>
    public class TestSummary
    {
        public int TotalModelsTest { get; set; }
        public int ModelsPassingAllTests { get; set; }
        public double OverallSuccessRate { get; set; }
        
        public double AverageAccuracy { get; set; }
        public double AverageBiasScore { get; set; }
        public double AveragePerformanceScore { get; set; }
        
        public int AccuracyTestsPassed { get; set; }
        public int BiasTestsPassed { get; set; }
        public int PerformanceTestsPassed { get; set; }
        
        public DateTime LastTestRun { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region Configuration Models

    /// <summary>
    /// Configuration for ML model testing framework
    /// </summary>
    public class MLModelTestingConfig
    {
        public double AccuracyTarget { get; set; } = 0.85;
        public double BiasThreshold { get; set; } = 0.1;
        public double PerformanceTarget { get; set; } = 0.8;
        
        public int CrossValidationFolds { get; set; } = 10;
        public double TestDataSplit { get; set; } = 0.2;
        public double ValidationDataSplit { get; set; } = 0.2;
        
        public bool EnableBiasDetection { get; set; } = true;
        public bool EnablePerformanceTesting { get; set; } = true;
        public bool EnableStatisticalTests { get; set; } = true;
        
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Test execution context
    /// </summary>
    public class TestExecutionContext
    {
        public string TestEnvironment { get; set; } = "Development";
        public string TestRunner { get; set; } = "MLModelTestingFramework";
        public Dictionary<string, string> EnvironmentVariables { get; set; } = new();
        public List<string> TestTags { get; set; } = new();
        public DateTime ExecutionStartTime { get; set; } = DateTime.UtcNow;
    }

    #endregion
}

