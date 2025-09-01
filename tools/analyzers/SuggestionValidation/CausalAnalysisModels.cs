using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Data models for Causal Analysis Validator
    /// Supporting statistical significance and evidence quality assessment
    /// </summary>
    
    /// <summary>
    /// Result of causal analysis validation
    /// </summary>
    public class CausalValidationResult
    {
        public double OverallCausalScore { get; set; }
        public List<CausalClaim> CausalClaims { get; set; } = new();
        public StatisticalSignificanceAssessment StatisticalAssessment { get; set; } = new();
        public EvidenceQualityAssessment EvidenceQuality { get; set; } = new();
        public CausalReasoningValidation ReasoningValidation { get; set; } = new();
        public FallacyDetectionResult FallacyDetection { get; set; } = new();
        public ADDSCausalAnalysis ADDSSpecificAnalysis { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public double Confidence { get; set; }
        public DateTime ValidationTimestamp { get; set; }
    }

    /// <summary>
    /// Individual causal claim identified in suggestion text
    /// </summary>
    public class CausalClaim
    {
        public string Cause { get; set; } = string.Empty;
        public string Effect { get; set; } = string.Empty;
        public string CausalIndicator { get; set; } = string.Empty;
        public CausalStrength Strength { get; set; }
        public string Context { get; set; } = string.Empty;
        public int Position { get; set; }
        public double Confidence { get; set; }
        public bool IsADDSSpecific { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Statistical significance assessment for causal claims
    /// </summary>
    public class StatisticalSignificanceAssessment
    {
        public bool HasQuantitativeEvidence { get; set; }
        public bool HasComparativeEvidence { get; set; }
        public bool HasControlGroupMention { get; set; }
        public bool HasSampleSizeInformation { get; set; }
        public bool HasConfidenceIntervals { get; set; }
        public bool HasSignificanceTesting { get; set; }
        public double StatisticalRigor { get; set; }
        public double OverallStatisticalStrength { get; set; }
        public List<string> StatisticalRecommendations { get; set; } = new();
        public Dictionary<string, double> StatisticalMetrics { get; set; } = new();
    }

    /// <summary>
    /// Evidence quality assessment for supporting evidence
    /// </summary>
    public class EvidenceQualityAssessment
    {
        public double SourceCredibility { get; set; }
        public double EvidenceRelevance { get; set; }
        public double EvidenceRecency { get; set; }
        public double EvidenceCompleteness { get; set; }
        public bool HasExperimentalEvidence { get; set; }
        public bool HasObservationalEvidence { get; set; }
        public bool HasExpertOpinion { get; set; }
        public bool HasIndustryBenchmarks { get; set; }
        public double BiasRisk { get; set; }
        public bool ConflictOfInterest { get; set; }
        public double OverallEvidenceStrength { get; set; }
        public List<string> EvidenceRecommendations { get; set; } = new();
        public Dictionary<string, object> EvidenceMetadata { get; set; } = new();
    }

    /// <summary>
    /// Causal reasoning validation assessment
    /// </summary>
    public class CausalReasoningValidation
    {
        public double LogicalConsistency { get; set; }
        public double CausalChainCoherence { get; set; }
        public List<string> NecessaryConditions { get; set; } = new();
        public List<string> SufficientConditions { get; set; } = new();
        public double TemporalOrdering { get; set; }
        public List<string> ConfoundingVariables { get; set; } = new();
        public double OverallReasoningQuality { get; set; }
        public List<string> ReasoningRecommendations { get; set; } = new();
    }

    /// <summary>
    /// Fallacy detection results for causal reasoning
    /// </summary>
    public class FallacyDetectionResult
    {
        public double PostHocFallacy { get; set; }
        public double CorrelationCausationFallacy { get; set; }
        public double FalseCauseFallacy { get; set; }
        public double OversimplificationFallacy { get; set; }
        public double SingleCauseFallacy { get; set; }
        public double CircularReasoning { get; set; }
        public double OverallFallacyRisk { get; set; }
        public List<string> FallacyRecommendations { get; set; } = new();
        public Dictionary<string, double> FallacyScores { get; set; } = new();
    }

    /// <summary>
    /// ADDS-specific causal analysis for migration validation
    /// </summary>
    public class ADDSCausalAnalysis
    {
        public double FrameworkMigrationCausality { get; set; }
        public double DatabaseMigrationCausality { get; set; }
        public double SpatialDataCausality { get; set; }
        public double PerformanceImpactCausality { get; set; }
        public double DependencyChainValidation { get; set; }
        public double OverallADDSCausalityScore { get; set; }
        public List<string> ADDSSpecificRecommendations { get; set; } = new();
        public Dictionary<string, object> MigrationMetrics { get; set; } = new();
    }

    /// <summary>
    /// Causal relationship definition for domain-specific validation
    /// </summary>
    public class CausalRelationshipDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string[] ExpectedCauses { get; set; } = Array.Empty<string>();
        public string[] ExpectedEffects { get; set; } = Array.Empty<string>();
        public double StrengthThreshold { get; set; } = 0.5;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Statistical test definition for significance assessment
    /// </summary>
    public class StatisticalTest
    {
        public string Name { get; set; } = string.Empty;
        public int MinSampleSize { get; set; }
        public double PowerThreshold { get; set; }
        public double SignificanceLevel { get; set; } = 0.05;
        public Dictionary<string, object> TestParameters { get; set; } = new();
    }

    /// <summary>
    /// Evidence quality rule for assessment
    /// </summary>
    public class EvidenceQualityRule
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Weight { get; set; } = 1.0;
        public Func<string, List<string>, double> Evaluator { get; set; } = (text, evidence) => 0.5;
        public bool IsCritical { get; set; } = false;
    }

    /// <summary>
    /// Configuration for causal analysis validation
    /// </summary>
    public class CausalAnalysisConfig
    {
        public double MinimumCausalScore { get; set; } = 0.6;
        public double TargetCausalScore { get; set; } = 0.85;
        public double StatisticalSignificanceThreshold { get; set; } = 0.05;
        public double EvidenceQualityThreshold { get; set; } = 0.7;
        public double FallacyRiskThreshold { get; set; } = 0.3;
        public bool EnableAdvancedStatistics { get; set; } = true;
        public bool EnableFallacyDetection { get; set; } = true;
        public bool EnableADDSSpecificAnalysis { get; set; } = true;
        public Dictionary<string, object> AdvancedSettings { get; set; } = new();
    }

    /// <summary>
    /// Strength levels for causal claims
    /// </summary>
    public enum CausalStrength
    {
        Weak,
        Medium,
        Strong,
        VeryStrong
    }

    /// <summary>
    /// Types of causal evidence
    /// </summary>
    public enum EvidenceType
    {
        Experimental,
        Observational,
        ExpertOpinion,
        IndustryBenchmark,
        CaseStudy,
        MetaAnalysis,
        SystematicReview
    }

    /// <summary>
    /// Statistical test types for causal analysis
    /// </summary>
    public enum StatisticalTestType
    {
        TTest,
        ChiSquare,
        ANOVA,
        Regression,
        Correlation,
        TimeSeriesAnalysis
    }

    /// <summary>
    /// Causal analysis statistics for monitoring and improvement
    /// </summary>
    public class CausalAnalysisStatistics
    {
        public int TotalSuggestionsAnalyzed { get; set; }
        public int CausalClaimsIdentified { get; set; }
        public double AverageCausalScore { get; set; }
        public double AverageStatisticalStrength { get; set; }
        public double AverageEvidenceQuality { get; set; }
        public double AverageFallacyRisk { get; set; }
        public Dictionary<CausalStrength, int> CausalStrengthDistribution { get; set; } = new();
        public Dictionary<string, int> CommonFallacies { get; set; } = new();
        public List<string> MostCommonRecommendations { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Causal training data for ML model improvement
    /// </summary>
    public class CausalTrainingData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public List<CausalClaim> ExpectedClaims { get; set; } = new();
        public double ExpectedCausalScore { get; set; }
        public List<string> SupportingEvidence { get; set; } = new();
        public string ExpertAnnotation { get; set; } = string.Empty;
        public StatisticalSignificanceAssessment ExpectedStatistics { get; set; } = new();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// Causal validation report for comprehensive analysis
    /// </summary>
    public class CausalValidationReport
    {
        public string ReportId { get; set; } = Guid.NewGuid().ToString();
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public string SuggestionText { get; set; } = string.Empty;
        public CausalValidationResult ValidationResult { get; set; } = new();
        public List<string> DetailedAnalysis { get; set; } = new();
        public Dictionary<string, object> StatisticalMetrics { get; set; } = new();
        public List<string> ImprovementSuggestions { get; set; } = new();
        public string ValidationSummary { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}

