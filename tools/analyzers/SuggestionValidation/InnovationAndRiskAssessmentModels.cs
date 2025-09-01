using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation.Models
{
    /// <summary>
    /// Result of innovation and risk assessment
    /// </summary>
    public class InnovationAndRiskAssessmentResult
    {
        public string SuggestionText { get; set; } = "";
        public AnalysisType AnalysisType { get; set; }
        public DateTime AssessmentTimestamp { get; set; }
        public InnovationAssessment InnovationAssessment { get; set; } = new();
        public RiskAssessment RiskAssessment { get; set; } = new();
        public double RiskAdjustedScore { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    /// <summary>
    /// Innovation assessment dimensions
    /// </summary>
    public class InnovationAssessment
    {
        public double NoveltyScore { get; set; }
        public double CreativityScore { get; set; }
        public double TechnicalAdvancementScore { get; set; }
        public double ApproachUniquenessScore { get; set; }
        public double ProblemSolvingInnovationScore { get; set; }
        public double OverallInnovationScore { get; set; }
        public InnovationLevel InnovationLevel { get; set; }
    }

    /// <summary>
    /// Risk assessment dimensions
    /// </summary>
    public class RiskAssessment
    {
        public double TechnicalComplexityRisk { get; set; }
        public double CompatibilityRisk { get; set; }
        public double PerformanceImpactRisk { get; set; }
        public double MaintenanceRisk { get; set; }
        public double SecurityRisk { get; set; }
        public double BusinessContinuityRisk { get; set; }
        public double OverallRiskScore { get; set; }
        public RiskLevel RiskLevel { get; set; }
    }

    /// <summary>
    /// Innovation level classification
    /// </summary>
    public enum InnovationLevel
    {
        Conventional,
        Somewhat_Innovative,
        Moderately_Innovative,
        Highly_Innovative,
        Revolutionary
    }

    /// <summary>
    /// Risk level classification
    /// </summary>
    public enum RiskLevel
    {
        Minimal,
        Low,
        Medium,
        High,
        Critical
    }
}

