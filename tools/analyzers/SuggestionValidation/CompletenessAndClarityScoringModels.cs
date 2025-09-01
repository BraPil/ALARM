using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Configuration for Completeness & Clarity Scoring System
    /// </summary>
    public class CompletenessAndClarityScoringConfig
    {
        /// <summary>
        /// Weight for completeness in combined score calculation
        /// </summary>
        public double CompletenessWeight { get; set; } = 0.6;

        /// <summary>
        /// Weight for clarity in combined score calculation
        /// </summary>
        public double ClarityWeight { get; set; } = 0.4;

        /// <summary>
        /// Minimum threshold for acceptable completeness
        /// </summary>
        public double MinCompletenessThreshold { get; set; } = 0.7;

        /// <summary>
        /// Minimum threshold for acceptable clarity
        /// </summary>
        public double MinClarityThreshold { get; set; } = 0.7;

        /// <summary>
        /// Target threshold for excellent quality
        /// </summary>
        public double ExcellenceThreshold { get; set; } = 0.9;

        /// <summary>
        /// Enable advanced linguistic analysis
        /// </summary>
        public bool EnableAdvancedLinguisticAnalysis { get; set; } = true;

        /// <summary>
        /// Enable domain-specific completeness checks
        /// </summary>
        public bool EnableDomainSpecificChecks { get; set; } = true;
    }

    /// <summary>
    /// Result of completeness and clarity assessment
    /// </summary>
    public class CompletenessAndClarityResult
    {
        /// <summary>
        /// Original suggestion text
        /// </summary>
        public string SuggestionText { get; set; } = string.Empty;

        /// <summary>
        /// Type of analysis performed
        /// </summary>
        public AnalysisType AnalysisType { get; set; }

        /// <summary>
        /// Timestamp of assessment
        /// </summary>
        public DateTime AssessmentTimestamp { get; set; }

        /// <summary>
        /// Detailed completeness assessment
        /// </summary>
        public CompletenessAssessment CompletenessAssessment { get; set; } = new();

        /// <summary>
        /// Detailed clarity assessment
        /// </summary>
        public ClarityAssessment ClarityAssessment { get; set; } = new();

        /// <summary>
        /// Overall completeness score (0.0 to 1.0)
        /// </summary>
        public double OverallCompletenessScore { get; set; }

        /// <summary>
        /// Overall clarity score (0.0 to 1.0)
        /// </summary>
        public double OverallClarityScore { get; set; }

        /// <summary>
        /// Combined completeness and clarity score
        /// </summary>
        public double CombinedScore { get; set; }

        /// <summary>
        /// Quality level determination
        /// </summary>
        public QualityLevel QualityLevel { get; set; }

        /// <summary>
        /// Specific improvement recommendations
        /// </summary>
        public List<string> ImprovementRecommendations { get; set; } = new();

        /// <summary>
        /// Error message if assessment failed
        /// </summary>
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Detailed completeness assessment
    /// </summary>
    public class CompletenessAssessment
    {
        /// <summary>
        /// Score for requirements coverage (0.0 to 1.0)
        /// </summary>
        public double RequirementsCoverage { get; set; }

        /// <summary>
        /// Score for implementation details (0.0 to 1.0)
        /// </summary>
        public double ImplementationDetails { get; set; }

        /// <summary>
        /// Score for contextual information (0.0 to 1.0)
        /// </summary>
        public double ContextualInformation { get; set; }

        /// <summary>
        /// Score for examples and evidence (0.0 to 1.0)
        /// </summary>
        public double ExamplesAndEvidence { get; set; }

        /// <summary>
        /// Score for edge cases and constraints (0.0 to 1.0)
        /// </summary>
        public double EdgeCasesAndConstraints { get; set; }

        /// <summary>
        /// List of identified missing elements
        /// </summary>
        public List<string> MissingElements { get; set; } = new();

        /// <summary>
        /// Detailed completeness metrics
        /// </summary>
        public Dictionary<string, double> CompletenessMetrics { get; set; } = new();
    }

    /// <summary>
    /// Detailed clarity assessment
    /// </summary>
    public class ClarityAssessment
    {
        /// <summary>
        /// Score for sentence structure (0.0 to 1.0)
        /// </summary>
        public double SentenceStructure { get; set; }

        /// <summary>
        /// Score for vocabulary clarity (0.0 to 1.0)
        /// </summary>
        public double VocabularyClarity { get; set; }

        /// <summary>
        /// Score for technical precision (0.0 to 1.0)
        /// </summary>
        public double TechnicalPrecision { get; set; }

        /// <summary>
        /// Score for logical flow (0.0 to 1.0)
        /// </summary>
        public double LogicalFlow { get; set; }

        /// <summary>
        /// Score for conciseness (0.0 to 1.0)
        /// </summary>
        public double Conciseness { get; set; }

        /// <summary>
        /// Score for grammar and style (0.0 to 1.0)
        /// </summary>
        public double GrammarAndStyle { get; set; }

        /// <summary>
        /// Detailed readability metrics
        /// </summary>
        public Dictionary<string, double> ReadabilityMetrics { get; set; } = new();

        /// <summary>
        /// List of identified clarity issues
        /// </summary>
        public List<string> ClarityIssues { get; set; } = new();
    }

    /// <summary>
    /// Quality level enumeration
    /// </summary>
    public enum QualityLevel
    {
        Poor = 0,
        NeedsImprovement = 1,
        Acceptable = 2,
        Good = 3,
        Excellent = 4
    }

    /// <summary>
    /// Comprehensive completeness and clarity report
    /// </summary>
    public class CompletenessAndClarityReport
    {
        /// <summary>
        /// Report generation timestamp
        /// </summary>
        public DateTime GenerationTimestamp { get; set; }

        /// <summary>
        /// Total number of assessments included
        /// </summary>
        public int TotalAssessments { get; set; }

        /// <summary>
        /// Overall statistics across all assessments
        /// </summary>
        public CompletenessAndClarityStatistics OverallStatistics { get; set; } = new();

        /// <summary>
        /// Statistics broken down by analysis type
        /// </summary>
        public Dictionary<AnalysisType, CompletenessAndClarityStatistics> AnalysisTypeBreakdown { get; set; } = new();

        /// <summary>
        /// Quality trends over time
        /// </summary>
        public QualityTrends QualityTrends { get; set; } = new();

        /// <summary>
        /// System-wide recommendations
        /// </summary>
        public List<string> SystemRecommendations { get; set; } = new();

        /// <summary>
        /// Identified improvement opportunities
        /// </summary>
        public List<ImprovementOpportunity> ImprovementOpportunities { get; set; } = new();

        /// <summary>
        /// Error message if report generation failed
        /// </summary>
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Statistical summary of completeness and clarity assessments
    /// </summary>
    public class CompletenessAndClarityStatistics
    {
        /// <summary>
        /// Average completeness score
        /// </summary>
        public double AverageCompletenessScore { get; set; }

        /// <summary>
        /// Average clarity score
        /// </summary>
        public double AverageClarityScore { get; set; }

        /// <summary>
        /// Average combined score
        /// </summary>
        public double AverageCombinedScore { get; set; }

        /// <summary>
        /// Percentage of excellent quality suggestions
        /// </summary>
        public double ExcellentQualityPercentage { get; set; }

        /// <summary>
        /// Percentage of good quality suggestions
        /// </summary>
        public double GoodQualityPercentage { get; set; }

        /// <summary>
        /// Percentage of acceptable quality suggestions
        /// </summary>
        public double AcceptableQualityPercentage { get; set; }

        /// <summary>
        /// Percentage of suggestions needing improvement
        /// </summary>
        public double NeedsImprovementPercentage { get; set; }

        /// <summary>
        /// Percentage of poor quality suggestions
        /// </summary>
        public double PoorQualityPercentage { get; set; }
    }

    /// <summary>
    /// Quality trends analysis
    /// </summary>
    public class QualityTrends
    {
        /// <summary>
        /// Trend in completeness scores over time
        /// </summary>
        public double CompletenessScoreTrend { get; set; }

        /// <summary>
        /// Trend in clarity scores over time
        /// </summary>
        public double ClarityScoreTrend { get; set; }

        /// <summary>
        /// Trend in combined scores over time
        /// </summary>
        public double CombinedScoreTrend { get; set; }

        /// <summary>
        /// Overall quality improvement measure
        /// </summary>
        public double QualityImprovement { get; set; }
    }

    /// <summary>
    /// Improvement opportunity identification
    /// </summary>
    public class ImprovementOpportunity
    {
        /// <summary>
        /// Area of improvement (Completeness or Clarity)
        /// </summary>
        public string Area { get; set; } = string.Empty;

        /// <summary>
        /// Specific issue identified
        /// </summary>
        public string Issue { get; set; } = string.Empty;

        /// <summary>
        /// Frequency of occurrence
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Impact level (High, Medium, Low)
        /// </summary>
        public string Impact { get; set; } = string.Empty;

        /// <summary>
        /// Recommended action
        /// </summary>
        public string Recommendation { get; set; } = string.Empty;
    }
}

