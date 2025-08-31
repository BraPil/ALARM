using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.CausalAnalysis
{
    #region Core Data Models

    /// <summary>
    /// Input data for causal analysis
    /// </summary>
    public class CausalData
    {
        public DateTime Timestamp { get; set; }
        public Dictionary<string, double> Variables { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        public string Source { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration for causal analysis
    /// </summary>
    public class CausalAnalysisConfig
    {
        // PC Algorithm parameters
        public double PCAlgorithmAlpha { get; set; } = 0.05; // Significance level
        
        // Granger Causality parameters
        public int MinDataPointsForGranger { get; set; } = 30;
        public int MaxLagForGranger { get; set; } = 5;
        public double GrangerSignificanceLevel { get; set; } = 0.05;
        
        // Information-theoretic parameters
        public double TransferEntropyThreshold { get; set; } = 0.1;
        
        // General parameters
        public double MinCausalStrength { get; set; } = 0.3;
        public double CausalValidationThreshold { get; set; } = 0.6;
        
        // Temporal analysis parameters
        public int TemporalWindowSize { get; set; } = 50;
        public double CausalStabilityThreshold { get; set; } = 0.7;
        
        // Structural equation modeling parameters
        public double SEMConvergenceThreshold { get; set; } = 0.001;
        public int MaxSEMIterations { get; set; } = 100;
        
        // Intervention analysis parameters
        public double InterventionEffectThreshold { get; set; } = 0.2;
        public int MinInterventionSamples { get; set; } = 10;
        
        // Confounding detection parameters
        public double ConfoundingThreshold { get; set; } = 0.4;
        public int MaxConfoundingVariables { get; set; } = 10;
    }

    #endregion

    #region Result Models

    /// <summary>
    /// Complete result from causal analysis
    /// </summary>
    public class CausalAnalysisResult
    {
        public DateTime AnalysisTimestamp { get; set; }
        public int DataSampleCount { get; set; }
        public List<CausalRelationship> CausalRelationships { get; set; } = new List<CausalRelationship>();
        public CausalGraph CausalGraph { get; set; } = new CausalGraph();
        public List<StructuralEquation> StructuralEquations { get; set; } = new List<StructuralEquation>();
        public List<InterventionEffect> InterventionEffects { get; set; } = new List<InterventionEffect>();
        public List<ConfoundingFactor> ConfoundingFactors { get; set; } = new List<ConfoundingFactor>();
        public Dictionary<string, double> CausalStrengths { get; set; } = new Dictionary<string, double>();
        public CausalValidationResult? ValidationResults { get; set; }
        public Dictionary<string, double>? ModelFitStatistics { get; set; }
        public List<CausalInsight> CausalInsights { get; set; } = new List<CausalInsight>();
        public List<CausalRecommendation> Recommendations { get; set; } = new List<CausalRecommendation>();
        public double OverallConfidence { get; set; }
    }

    /// <summary>
    /// Individual causal relationship
    /// </summary>
    public class CausalRelationship
    {
        public string Id { get; set; } = string.Empty;
        public string CauseVariable { get; set; } = string.Empty;
        public string EffectVariable { get; set; } = string.Empty;
        public double Strength { get; set; }
        public double Confidence { get; set; }
        public string Method { get; set; } = string.Empty;
        public CausalDirection Direction { get; set; }
        public List<string> Evidence { get; set; } = new List<string>();
        public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
        public Dictionary<string, double> Statistics { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Causal graph representation
    /// </summary>
    public class CausalGraph
    {
        public List<CausalNode> Nodes { get; set; } = new List<CausalNode>();
        public List<CausalEdge> Edges { get; set; } = new List<CausalEdge>();
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Node in causal graph
    /// </summary>
    public class CausalNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Variable";
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public double Centrality { get; set; }
        public int InDegree { get; set; }
        public int OutDegree { get; set; }
    }

    /// <summary>
    /// Edge in causal graph
    /// </summary>
    public class CausalEdge
    {
        public string Id { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public double Strength { get; set; }
        public double Confidence { get; set; }
        public string Method { get; set; } = string.Empty;
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Structural equation model
    /// </summary>
    public class StructuralEquation
    {
        public string Id { get; set; } = string.Empty;
        public string DependentVariable { get; set; } = string.Empty;
        public List<StructuralTerm> Terms { get; set; } = new List<StructuralTerm>();
        public double RSquared { get; set; }
        public double AdjustedRSquared { get; set; }
        public double StandardError { get; set; }
        public Dictionary<string, double> GoodnessOfFit { get; set; } = new Dictionary<string, double>();
        public List<string> Assumptions { get; set; } = new List<string>();
    }

    /// <summary>
    /// Term in structural equation
    /// </summary>
    public class StructuralTerm
    {
        public string Variable { get; set; } = string.Empty;
        public double Coefficient { get; set; }
        public double StandardError { get; set; }
        public double TStatistic { get; set; }
        public double PValue { get; set; }
        public double ConfidenceIntervalLower { get; set; }
        public double ConfidenceIntervalUpper { get; set; }
    }

    /// <summary>
    /// Intervention effect analysis
    /// </summary>
    public class InterventionEffect
    {
        public string Id { get; set; } = string.Empty;
        public string InterventionVariable { get; set; } = string.Empty;
        public string TargetVariable { get; set; } = string.Empty;
        public double InterventionValue { get; set; }
        public double ExpectedEffect { get; set; }
        public double ConfidenceIntervalLower { get; set; }
        public double ConfidenceIntervalUpper { get; set; }
        public double Probability { get; set; }
        public string InterventionType { get; set; } = string.Empty;
        public List<string> Assumptions { get; set; } = new List<string>();
        public Dictionary<string, double> SensitivityAnalysis { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Confounding factor detection
    /// </summary>
    public class ConfoundingFactor
    {
        public string Id { get; set; } = string.Empty;
        public string VariableName { get; set; } = string.Empty;
        public List<string> AffectedRelationships { get; set; } = new List<string>();
        public double Impact { get; set; }
        public double Confidence { get; set; }
        public string DetectionMethod { get; set; } = string.Empty;
        public List<string> Evidence { get; set; } = new List<string>();
        public Dictionary<string, double> Statistics { get; set; } = new Dictionary<string, double>();
    }

    #endregion

    #region Validation Models

    /// <summary>
    /// Causal validation results
    /// </summary>
    public class CausalValidationResult
    {
        public List<CausalValidationTest> ValidationTests { get; set; } = new List<CausalValidationTest>();
        public double OverallValidationScore { get; set; }
        public Dictionary<string, double> ValidationMetrics { get; set; } = new Dictionary<string, double>();
        public DateTime ValidationTimestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Individual causal validation test
    /// </summary>
    public class CausalValidationTest
    {
        public string RelationshipId { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;
        public double ValidationScore { get; set; }
        public bool Passed { get; set; }
        public Dictionary<string, double> TestResults { get; set; } = new Dictionary<string, double>();
        public List<string> Warnings { get; set; } = new List<string>();
        public string Description { get; set; } = string.Empty;
    }

    #endregion

    #region Discovery Models

    /// <summary>
    /// Causal discovery result
    /// </summary>
    public class CausalDiscoveryResult
    {
        public List<CausalRelationship> CausalRelationships { get; set; } = new List<CausalRelationship>();
        public CausalGraph CausalGraph { get; set; } = new CausalGraph();
        public Dictionary<string, double> DiscoveryMetrics { get; set; } = new Dictionary<string, double>();
        public DateTime DiscoveryTimestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Structural equation modeling result
    /// </summary>
    public class StructuralModelingResult
    {
        public List<StructuralEquation> StructuralEquations { get; set; } = new List<StructuralEquation>();
        public Dictionary<string, double> ModelFitStatistics { get; set; } = new Dictionary<string, double>();
        public List<string> ModelAssumptions { get; set; } = new List<string>();
        public bool ConvergedSuccessfully { get; set; }
        public int IterationsUsed { get; set; }
        public DateTime ModelingTimestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Intervention analysis result
    /// </summary>
    public class InterventionAnalysisResult
    {
        public List<InterventionEffect> InterventionEffects { get; set; } = new List<InterventionEffect>();
        public Dictionary<string, double> InterventionMetrics { get; set; } = new Dictionary<string, double>();
        public DateTime AnalysisTimestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Confounding detection result
    /// </summary>
    public class ConfoundingDetectionResult
    {
        public List<ConfoundingFactor> ConfoundingFactors { get; set; } = new List<ConfoundingFactor>();
        public Dictionary<string, double> DetectionMetrics { get; set; } = new Dictionary<string, double>();
        public DateTime DetectionTimestamp { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region Temporal Analysis Models

    /// <summary>
    /// Temporal causal analysis result
    /// </summary>
    public class TemporalCausalAnalysisResult
    {
        public DateTime AnalysisTimestamp { get; set; }
        public List<CausalTimeWindow> TimeWindows { get; set; } = new List<CausalTimeWindow>();
        public Dictionary<string, double> CausalStabilityMetrics { get; set; } = new Dictionary<string, double>();
        public List<CausalChangePoint> CausalChangePoints { get; set; } = new List<CausalChangePoint>();
        public Dictionary<string, object> TemporalProperties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Time window for temporal causal analysis
    /// </summary>
    public class CausalTimeWindow
    {
        public int WindowIndex { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public CausalAnalysisResult CausalAnalysis { get; set; } = new CausalAnalysisResult();
        public double StabilityScore { get; set; }
        public Dictionary<string, double> WindowMetrics { get; set; } = new Dictionary<string, double>();
    }

    /// <summary>
    /// Causal change point detection
    /// </summary>
    public class CausalChangePoint
    {
        public DateTime Timestamp { get; set; }
        public int WindowIndex { get; set; }
        public double StabilityDrop { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> AffectedRelationships { get; set; } = new List<string>();
        public double Significance { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    #endregion

    #region Comparison Models

    /// <summary>
    /// Causal comparison result
    /// </summary>
    public class CausalComparisonResult
    {
        public CausalAnalysisResult BaselineAnalysis { get; set; } = new CausalAnalysisResult();
        public CausalAnalysisResult ComparisonAnalysis { get; set; } = new CausalAnalysisResult();
        public double CausalSimilarity { get; set; }
        public List<CausalDifference> SignificantDifferences { get; set; } = new List<CausalDifference>();
        public CausalEvolution CausalEvolution { get; set; } = new CausalEvolution();
        public List<CausalRecommendation> Recommendations { get; set; } = new List<CausalRecommendation>();
        public DateTime ComparisonTimestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Significant difference between causal analyses
    /// </summary>
    public class CausalDifference
    {
        public CausalDifferenceType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public CausalRelationship? Relationship { get; set; }
        public double Significance { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Causal evolution analysis
    /// </summary>
    public class CausalEvolution
    {
        public double OverallSimilarity { get; set; }
        public object? RelationshipCount { get; set; }
        public Dictionary<string, double> StrengthEvolution { get; set; } = new Dictionary<string, double>();
        public object? ConfidenceEvolution { get; set; }
        public Dictionary<string, object> EvolutionMetrics { get; set; } = new Dictionary<string, object>();
    }

    #endregion

    #region Insight and Recommendation Models

    /// <summary>
    /// Causal insight
    /// </summary>
    public class CausalInsight
    {
        public CausalInsightType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Importance { get; set; }
        public List<string> RelatedRelationships { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Causal recommendation
    /// </summary>
    public class CausalRecommendation
    {
        public CausalRecommendationType Type { get; set; }
        public CausalRecommendationPriority Priority { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ExpectedImpact { get; set; }
        public List<string> RelatedRelationships { get; set; } = new List<string>();
        public List<string> ActionItems { get; set; } = new List<string>();
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    #endregion

    #region Enumerations

    /// <summary>
    /// Direction of causal relationship
    /// </summary>
    public enum CausalDirection
    {
        Forward,
        Backward,
        Bidirectional,
        Unknown
    }

    /// <summary>
    /// Type of causal insight
    /// </summary>
    public enum CausalInsightType
    {
        StrongCausality,
        WeakCausality,
        ConfoundingDetected,
        InterventionOpportunity,
        CausalLoop,
        MediationEffect,
        ModeratingFactor
    }

    /// <summary>
    /// Type of causal recommendation
    /// </summary>
    public enum CausalRecommendationType
    {
        Optimization,
        Investigation,
        Improvement,
        Prevention,
        Enhancement,
        Intervention
    }

    /// <summary>
    /// Priority of causal recommendation
    /// </summary>
    public enum CausalRecommendationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    /// <summary>
    /// Type of causal difference
    /// </summary>
    public enum CausalDifferenceType
    {
        RelationshipLost,
        NewRelationship,
        StrengthChanged,
        DirectionChanged,
        ConfidenceChanged
    }

    #endregion
}
