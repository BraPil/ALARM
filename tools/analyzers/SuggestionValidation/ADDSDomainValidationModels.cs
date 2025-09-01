using System;
using System.Collections.Generic;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Data models for ADDS Domain Validator
    /// Supporting CAD integration and database optimization scoring for ADDS migration
    /// </summary>
    
    /// <summary>
    /// Result of ADDS domain-specific validation analysis
    /// </summary>
    public class ADDSDomainValidationResult
    {
        public double OverallDomainScore { get; set; }
        public CADIntegrationAnalysis CADIntegrationAnalysis { get; set; } = new();
        public DatabaseOptimizationAnalysis DatabaseOptimizationAnalysis { get; set; } = new();
        public FrameworkMigrationAnalysis FrameworkMigrationAnalysis { get; set; } = new();
        public DomainExpertiseAssessment DomainExpertiseAssessment { get; set; } = new();
        public MigrationComplexityAssessment MigrationComplexityAssessment { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
        public double Confidence { get; set; }
        public DateTime ValidationTimestamp { get; set; }
    }

    /// <summary>
    /// Analysis of CAD integration aspects for AutoCAD Map3D 2025 migration
    /// </summary>
    public class CADIntegrationAnalysis
    {
        // Detection flags
        public bool HasMap3DReferences { get; set; }
        public bool HasAPIIntegration { get; set; }
        public bool HasSpatialDataHandling { get; set; }
        public bool HasCoordinateSystemHandling { get; set; }
        
        // Analysis scores
        public double Map3DVersionCompatibility { get; set; }
        public double APICompatibilityScore { get; set; }
        public double SpatialDataMigrationScore { get; set; }
        public double RenderingOptimizationScore { get; set; }
        public double DrawingFileCompatibility { get; set; }
        
        // Risk assessment
        public List<string> IntegrationRisks { get; set; } = new();
        public List<string> MigrationChallenges { get; set; } = new();
        
        // Overall score
        public double OverallCADScore { get; set; }
        
        // Detailed analysis
        public Dictionary<string, double> CADMetrics { get; set; } = new();
        public Dictionary<string, object> CADMetadata { get; set; } = new();
    }

    /// <summary>
    /// Analysis of database optimization aspects for Oracle 19c migration
    /// </summary>
    public class DatabaseOptimizationAnalysis
    {
        // Detection flags
        public bool HasOracleReferences { get; set; }
        public bool HasSpatialDataOptimization { get; set; }
        public bool HasQueryOptimization { get; set; }
        public bool HasIndexingStrategy { get; set; }
        
        // Analysis scores
        public double OracleVersionCompatibility { get; set; }
        public double SpatialDataOptimizationScore { get; set; }
        public double QueryPerformanceScore { get; set; }
        public double ConnectionManagementScore { get; set; }
        public double DataMigrationComplexity { get; set; }
        
        // Risk assessment
        public List<string> OptimizationRisks { get; set; } = new();
        public double PerformanceImpact { get; set; }
        
        // Overall score
        public double OverallDatabaseScore { get; set; }
        
        // Detailed analysis
        public Dictionary<string, double> DatabaseMetrics { get; set; } = new();
        public Dictionary<string, object> DatabaseMetadata { get; set; } = new();
    }

    /// <summary>
    /// Analysis of .NET Core 8 framework migration aspects
    /// </summary>
    public class FrameworkMigrationAnalysis
    {
        // Detection flags
        public bool HasDotNetCoreReferences { get; set; }
        public bool HasFrameworkMigrationStrategy { get; set; }
        public bool HasCompatibilityConsiderations { get; set; }
        
        // Analysis scores
        public double DotNetCoreCompatibilityScore { get; set; }
        public double FrameworkMigrationComplexity { get; set; }
        public double PerformanceImprovementPotential { get; set; }
        public double ModernizationScore { get; set; }
        
        // Overall score
        public double OverallFrameworkScore { get; set; }
        
        // Detailed analysis
        public Dictionary<string, double> FrameworkMetrics { get; set; } = new();
        public Dictionary<string, object> FrameworkMetadata { get; set; } = new();
    }

    /// <summary>
    /// Assessment of domain-specific expertise and knowledge depth
    /// </summary>
    public class DomainExpertiseAssessment
    {
        // Expertise levels by domain
        public double CADExpertiseLevel { get; set; }
        public double DatabaseExpertiseLevel { get; set; }
        public double FrameworkExpertiseLevel { get; set; }
        public double ADDSSpecificKnowledge { get; set; }
        public double MigrationExpertise { get; set; }
        
        // Overall expertise score
        public double OverallExpertiseScore { get; set; }
        
        // Expertise breakdown
        public Dictionary<string, double> ExpertiseMetrics { get; set; } = new();
        public List<string> IdentifiedExpertiseAreas { get; set; } = new();
        public List<string> KnowledgeGaps { get; set; } = new();
    }

    /// <summary>
    /// Assessment of migration complexity and risk factors
    /// </summary>
    public class MigrationComplexityAssessment
    {
        public double OverallComplexityScore { get; set; }
        public double TechnicalComplexity { get; set; }
        public double IntegrationComplexity { get; set; }
        public double DataMigrationComplexity { get; set; }
        public double TestingComplexity { get; set; }
        public double DeploymentComplexity { get; set; }
        
        // Risk factors
        public List<string> HighRiskAreas { get; set; } = new();
        public List<string> MediumRiskAreas { get; set; } = new();
        public List<string> LowRiskAreas { get; set; } = new();
        
        // Mitigation strategies
        public List<string> RecommendedMitigations { get; set; } = new();
        
        // Complexity breakdown
        public Dictionary<string, double> ComplexityMetrics { get; set; } = new();
    }

    /// <summary>
    /// Domain expertise rule definition
    /// </summary>
    public class DomainExpertiseRule
    {
        public string Name { get; set; } = string.Empty;
        public string[] Keywords { get; set; } = Array.Empty<string>();
        public double Weight { get; set; } = 1.0;
        public double MinimumScore { get; set; } = 0.5;
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// CAD integration pattern definition
    /// </summary>
    public class CADIntegrationPattern
    {
        public string Name { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public double Weight { get; set; } = 1.0;
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Database optimization rule definition
    /// </summary>
    public class DatabaseOptimizationRule
    {
        public string Name { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public double Weight { get; set; } = 1.0;
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// Domain expertise context for validation
    /// </summary>
    public class DomainExpertiseContext
    {
        public Dictionary<string, double> ExpertiseLevels { get; set; } = new();
        public List<string> PrimaryDomains { get; set; } = new();
        public List<string> SecondaryDomains { get; set; } = new();
        public Dictionary<string, object> ContextMetadata { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Configuration for ADDS domain validation
    /// </summary>
    public class ADDSDomainValidationConfig
    {
        public double MinimumDomainScore { get; set; } = 0.6;
        public double TargetDomainScore { get; set; } = 0.85;
        public double CADIntegrationWeight { get; set; } = 0.3;
        public double DatabaseOptimizationWeight { get; set; } = 0.3;
        public double FrameworkMigrationWeight { get; set; } = 0.25;
        public double DomainExpertiseWeight { get; set; } = 0.15;
        public bool EnableCADAnalysis { get; set; } = true;
        public bool EnableDatabaseAnalysis { get; set; } = true;
        public bool EnableFrameworkAnalysis { get; set; } = true;
        public Dictionary<string, object> AdvancedSettings { get; set; } = new();
    }

    /// <summary>
    /// CAD integration categories
    /// </summary>
    public enum CADIntegrationCategory
    {
        APIIntegration,
        SpatialDataProcessing,
        RenderingOptimization,
        DrawingFileHandling,
        CoordinateSystemManagement,
        EntityManipulation
    }

    /// <summary>
    /// Database optimization categories
    /// </summary>
    public enum DatabaseOptimizationCategory
    {
        SpatialOptimization,
        QueryPerformance,
        IndexingStrategy,
        ConnectionManagement,
        DataMigration,
        SchemaDesign
    }

    /// <summary>
    /// Framework migration categories
    /// </summary>
    public enum FrameworkMigrationCategory
    {
        CompatibilityAssessment,
        PerformanceImprovement,
        ModernizationStrategy,
        APIUpdates,
        DeploymentStrategy,
        TestingStrategy
    }

    /// <summary>
    /// Domain validation statistics for monitoring
    /// </summary>
    public class ADDSDomainValidationStatistics
    {
        public int TotalSuggestionsAnalyzed { get; set; }
        public int CADIntegrationsIdentified { get; set; }
        public int DatabaseOptimizationsIdentified { get; set; }
        public int FrameworkMigrationsIdentified { get; set; }
        public double AverageDomainScore { get; set; }
        public Dictionary<CADIntegrationCategory, int> CADCategoryDistribution { get; set; } = new();
        public Dictionary<DatabaseOptimizationCategory, int> DatabaseCategoryDistribution { get; set; } = new();
        public Dictionary<FrameworkMigrationCategory, int> FrameworkCategoryDistribution { get; set; } = new();
        public List<string> MostCommonRecommendations { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// ADDS domain training data for ML model improvement
    /// </summary>
    public class ADDSDomainTrainingData
    {
        public string SuggestionText { get; set; } = string.Empty;
        public CADIntegrationAnalysis ExpectedCADAnalysis { get; set; } = new();
        public DatabaseOptimizationAnalysis ExpectedDatabaseAnalysis { get; set; } = new();
        public FrameworkMigrationAnalysis ExpectedFrameworkAnalysis { get; set; } = new();
        public double ExpectedDomainScore { get; set; }
        public List<string> KnownDomainAreas { get; set; } = new();
        public Dictionary<string, double> ActualMetrics { get; set; } = new();
        public string ExpertAnnotation { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// ADDS domain validation report for comprehensive analysis
    /// </summary>
    public class ADDSDomainValidationReport
    {
        public string ReportId { get; set; } = Guid.NewGuid().ToString();
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
        public string SuggestionText { get; set; } = string.Empty;
        public ADDSDomainValidationResult ValidationResult { get; set; } = new();
        public List<string> DetailedAnalysis { get; set; } = new();
        public Dictionary<string, object> DomainMetrics { get; set; } = new();
        public List<string> DomainRecommendations { get; set; } = new();
        public string ValidationSummary { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}

