using ALARM.Mapping.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ALARM.Mapping.Core.Interfaces
{
    /// <summary>
    /// Main interface for the ALARM mapping system
    /// </summary>
    public interface IApplicationMapper
    {
        Task<ApplicationAnalysis> AnalyzeApplicationAsync(string rootPath, AnalysisConfiguration configuration, CancellationToken cancellationToken = default);
        Task<bool> GenerateVisualizationsAsync(ApplicationAnalysis analysis, string outputPath, CancellationToken cancellationToken = default);
        Task<ApplicationAnalysis> LoadAnalysisAsync(string filePath, CancellationToken cancellationToken = default);
        Task SaveAnalysisAsync(ApplicationAnalysis analysis, string filePath, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// File system crawling and analysis
    /// </summary>
    public interface IFileSystemCrawler
    {
        Task<FileSystemAnalysis> CrawlAsync(string rootPath, CrawlOptions options, CancellationToken cancellationToken = default);
        Task<FileSystemAnalysis> CrawlAsync(string rootPath, CrawlOptions options, IProgress<CrawlProgress> progress, CancellationToken cancellationToken = default);
        IAsyncEnumerable<Models.FileInfo> EnumerateFilesAsync(string rootPath, CrawlOptions options, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Code parsing and symbol extraction
    /// </summary>
    public interface ICodeAnalysisEngine
    {
        Task<CodeAnalysis> AnalyzeAsync(FileSystemAnalysis fileSystem, CodeAnalysisOptions options, CancellationToken cancellationToken = default);
        Task<CodeAnalysis> AnalyzeAsync(FileSystemAnalysis fileSystem, CodeAnalysisOptions options, IProgress<CodeAnalysisProgress> progress, CancellationToken cancellationToken = default);
        Task<List<CodeSymbol>> ExtractSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken = default);
        Task<SyntaxTree> ParseFileAsync(Models.FileInfo file, CancellationToken cancellationToken = default);
        bool CanAnalyzeFile(Models.FileInfo file);
    }

    /// <summary>
    /// Dependency resolution and mapping
    /// </summary>
    public interface IDependencyResolver
    {
        Task<DependencyAnalysis> ResolveAsync(CodeAnalysis codeAnalysis, DependencyOptions options, CancellationToken cancellationToken = default);
        Task<List<StaticDependency>> ResolveStaticDependenciesAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<List<DynamicDependency>> ResolveDynamicDependenciesAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<List<ExternalDependency>> ResolveExternalDependenciesAsync(FileSystemAnalysis fileSystem, CancellationToken cancellationToken = default);
        Task<List<DatabaseDependency>> ResolveDatabaseDependenciesAsync(FileSystemAnalysis fileSystem, CancellationToken cancellationToken = default);
        Task<List<CircularDependency>> DetectCircularDependenciesAsync(DependencyGraph graph, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Architectural pattern detection and analysis
    /// </summary>
    public interface IArchitectureAnalyzer
    {
        Task<ArchitectureAnalysis> AnalyzeAsync(CodeAnalysis codeAnalysis, DependencyAnalysis dependencyAnalysis, ArchitectureOptions options, CancellationToken cancellationToken = default);
        Task<ArchitecturalPattern> DetectPatternAsync(CodeAnalysis codeAnalysis, DependencyAnalysis dependencyAnalysis, CancellationToken cancellationToken = default);
        Task<List<Layer>> DetectLayersAsync(CodeAnalysis codeAnalysis, DependencyAnalysis dependencyAnalysis, CancellationToken cancellationToken = default);
        Task<List<Component>> DetectComponentsAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<List<DesignPattern>> DetectDesignPatternsAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<List<ArchitecturalViolation>> DetectViolationsAsync(ArchitectureAnalysis architecture, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Relationship mapping and graph construction
    /// </summary>
    public interface IRelationshipMapper
    {
        Task<RelationshipMatrix> MapRelationshipsAsync(CodeAnalysis codeAnalysis, DependencyAnalysis dependencyAnalysis, RelationshipOptions options, CancellationToken cancellationToken = default);
        Task<List<InheritanceRelationship>> MapInheritanceAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<List<CompositionRelationship>> MapCompositionAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<List<MethodCallRelationship>> MapMethodCallsAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<RelationshipGraph> BuildGraphAsync(RelationshipMatrix relationships, CancellationToken cancellationToken = default);
        Task<RelationshipStatistics> CalculateStatisticsAsync(RelationshipMatrix relationships, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Visualization generation for multiple formats
    /// </summary>
    public interface IVisualizationGenerator
    {
        Task<bool> GenerateAsync(ApplicationAnalysis analysis, VisualizationRequest request, CancellationToken cancellationToken = default);
        Task<string> GenerateGraphvizAsync(ApplicationAnalysis analysis, GraphvizOptions options, CancellationToken cancellationToken = default);
        Task<string> GeneratePlantUMLAsync(ApplicationAnalysis analysis, PlantUMLOptions options, CancellationToken cancellationToken = default);
        Task<string> GenerateVisioXMLAsync(ApplicationAnalysis analysis, VisioOptions options, CancellationToken cancellationToken = default);
        Task<string> GenerateD3JsonAsync(ApplicationAnalysis analysis, D3Options options, CancellationToken cancellationToken = default);
        Task<string> GenerateMermaidAsync(ApplicationAnalysis analysis, MermaidOptions options, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Specialized analyzer for specific technologies
    /// </summary>
    public interface ISpecializedAnalyzer
    {
        string Name { get; }
        string[] SupportedExtensions { get; }
        bool CanAnalyze(Models.FileInfo file);
        Task<SpecializedAnalysis> AnalyzeAsync(Models.FileInfo file, SpecializedAnalysisOptions options, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// ADDS-specific analyzer for AutoCAD and Oracle integration
    /// </summary>
    public interface IADDSAnalyzer : ISpecializedAnalyzer
    {
        Task<AutoCADAnalysis> AnalyzeAutoCADIntegrationAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<OracleAnalysis> AnalyzeOracleIntegrationAsync(CodeAnalysis codeAnalysis, FileSystemAnalysis fileSystem, CancellationToken cancellationToken = default);
        Task<ADDSArchitecture> AnalyzeADDSArchitectureAsync(ApplicationAnalysis analysis, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Pattern detection for specific design patterns
    /// </summary>
    public interface IPatternDetector
    {
        PatternType PatternType { get; }
        Task<List<PatternMatch>> DetectAsync(CodeAnalysis codeAnalysis, PatternDetectionOptions options, CancellationToken cancellationToken = default);
        double CalculateConfidence(PatternMatch match);
    }

    /// <summary>
    /// Metrics calculation for various quality and complexity measures
    /// </summary>
    public interface IMetricsCalculator
    {
        Task<AnalysisMetrics> CalculateAsync(ApplicationAnalysis analysis, MetricsOptions options, CancellationToken cancellationToken = default);
        Task<ComplexityMetrics> CalculateComplexityAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<QualityMetrics> CalculateQualityAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<CohesionMetrics> CalculateCohesionAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default);
        Task<CouplingMetrics> CalculateCouplingAsync(DependencyAnalysis dependencyAnalysis, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Export functionality for analysis results
    /// </summary>
    public interface IAnalysisExporter
    {
        Task<bool> ExportAsync(ApplicationAnalysis analysis, ExportRequest request, CancellationToken cancellationToken = default);
        Task<string> ExportToJsonAsync(ApplicationAnalysis analysis, CancellationToken cancellationToken = default);
        Task<string> ExportToXmlAsync(ApplicationAnalysis analysis, CancellationToken cancellationToken = default);
        Task<byte[]> ExportToExcelAsync(ApplicationAnalysis analysis, CancellationToken cancellationToken = default);
        Task<string> ExportToReportAsync(ApplicationAnalysis analysis, ReportOptions options, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Configuration management for analysis options
    /// </summary>
    public interface IConfigurationManager
    {
        Task<AnalysisConfiguration> LoadConfigurationAsync(string filePath, CancellationToken cancellationToken = default);
        Task SaveConfigurationAsync(AnalysisConfiguration configuration, string filePath, CancellationToken cancellationToken = default);
        AnalysisConfiguration CreateDefaultConfiguration(string targetPath);
        Task<bool> ValidateConfigurationAsync(AnalysisConfiguration configuration, CancellationToken cancellationToken = default);
    }

    // Supporting classes and options
    public class CrawlOptions
    {
        public List<string> IncludePatterns { get; set; } = new() { "*.*" };
        public List<string> ExcludePatterns { get; set; } = new() { "bin/*", "obj/*", "*.tmp" };
        public bool FollowSymlinks { get; set; } = false;
        public int MaxDepth { get; set; } = 50;
        public long MaxFileSize { get; set; } = 100 * 1024 * 1024; // 100MB
        public bool CalculateHashes { get; set; } = false;
        public bool ExtractMetadata { get; set; } = true;
    }

    public class CrawlProgress
    {
        public int FilesProcessed { get; set; }
        public int DirectoriesProcessed { get; set; }
        public long BytesProcessed { get; set; }
        public string CurrentPath { get; set; } = string.Empty;
        public double PercentComplete { get; set; }
    }

    public class CodeAnalysisOptions
    {
        public List<string> SupportedLanguages { get; set; } = new() { "csharp", "vb", "sql", "xml", "json" };
        public bool ExtractDocumentation { get; set; } = true;
        public bool CalculateMetrics { get; set; } = true;
        public bool DetectPatterns { get; set; } = true;
        public int MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
        public bool IncludePrivateMembers { get; set; } = true;
    }

    public class CodeAnalysisProgress
    {
        public int FilesProcessed { get; set; }
        public int SymbolsExtracted { get; set; }
        public string CurrentFile { get; set; } = string.Empty;
        public double PercentComplete { get; set; }
    }

    public class DependencyOptions
    {
        public bool ResolveStaticDependencies { get; set; } = true;
        public bool ResolveDynamicDependencies { get; set; } = true;
        public bool ResolveExternalDependencies { get; set; } = true;
        public bool ResolveDatabaseDependencies { get; set; } = true;
        public bool DetectCircularDependencies { get; set; } = true;
        public List<string> ExternalSources { get; set; } = new() { "NuGet", "COM", "GAC" };
    }

    public class ArchitectureOptions
    {
        public bool DetectPatterns { get; set; } = true;
        public bool DetectLayers { get; set; } = true;
        public bool DetectComponents { get; set; } = true;
        public bool DetectDesignPatterns { get; set; } = true;
        public bool DetectViolations { get; set; } = true;
        public List<string> LayerConventions { get; set; } = new();
    }

    public class RelationshipOptions
    {
        public bool MapInheritance { get; set; } = true;
        public bool MapComposition { get; set; } = true;
        public bool MapAggregation { get; set; } = true;
        public bool MapAssociation { get; set; } = true;
        public bool MapMethodCalls { get; set; } = true;
        public bool MapPropertyAccess { get; set; } = true;
        public bool MapEvents { get; set; } = true;
        public int MaxCallDepth { get; set; } = 10;
    }

    public class VisualizationRequest
    {
        public string OutputPath { get; set; } = string.Empty;
        public List<VisualizationType> Types { get; set; } = new();
        public VisualizationOptions Options { get; set; } = new();
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    public class SpecializedAnalysisOptions
    {
        public Dictionary<string, object> Options { get; set; } = new();
    }

    public class SpecializedAnalysis
    {
        public string AnalyzerName { get; set; } = string.Empty;
        public Dictionary<string, object> Results { get; set; } = new();
        public List<AnalysisWarning> Warnings { get; set; } = new();
    }

    public class AutoCADAnalysis
    {
        public List<string> AutoCADAPIs { get; set; } = new();
        public List<string> CustomCommands { get; set; } = new();
        public List<string> BlockDefinitions { get; set; } = new();
        public List<string> LayerDefinitions { get; set; } = new();
    }

    public class OracleAnalysis
    {
        public List<string> ConnectionStrings { get; set; } = new();
        public List<string> Tables { get; set; } = new();
        public List<string> Views { get; set; } = new();
        public List<string> Procedures { get; set; } = new();
        public List<string> Functions { get; set; } = new();
    }

    public class ADDSArchitecture
    {
        public AutoCADAnalysis AutoCAD { get; set; } = new();
        public OracleAnalysis Oracle { get; set; } = new();
        public List<string> ADDSModules { get; set; } = new();
        public Dictionary<string, object> Configuration { get; set; } = new();
    }

    public class PatternMatch
    {
        public PatternType PatternType { get; set; } = PatternType.Unknown;
        public List<string> ParticipatingClasses { get; set; } = new();
        public double Confidence { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class PatternDetectionOptions
    {
        public double MinimumConfidence { get; set; } = 0.7;
        public List<PatternType> PatternsToDetect { get; set; } = new();
    }

    public class MetricsOptions
    {
        public bool CalculateComplexity { get; set; } = true;
        public bool CalculateQuality { get; set; } = true;
        public bool CalculateCohesion { get; set; } = true;
        public bool CalculateCoupling { get; set; } = true;
        public Dictionary<string, object> CustomMetrics { get; set; } = new();
    }

    public class ExportRequest
    {
        public string OutputPath { get; set; } = string.Empty;
        public ExportFormat Format { get; set; } = ExportFormat.Json;
        public ExportOptions Options { get; set; } = new();
    }

    public class ExportOptions
    {
        public bool IncludeFullDetails { get; set; } = true;
        public bool CompressOutput { get; set; } = false;
        public List<string> SectionsToInclude { get; set; } = new();
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    public class ReportOptions
    {
        public ReportFormat Format { get; set; } = ReportFormat.HTML;
        public string TemplatePath { get; set; } = string.Empty;
        public bool IncludeCharts { get; set; } = true;
        public bool IncludeDiagrams { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    public class SyntaxTree
    {
        public string FilePath { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public object Root { get; set; } = new(); // Language-specific syntax tree root
        public List<AnalysisWarning> Diagnostics { get; set; } = new();
    }

    public class MermaidOptions
    {
        public MermaidDiagramType DiagramType { get; set; } = MermaidDiagramType.Flowchart;
        public string Direction { get; set; } = "TD"; // TD, LR, RL, BT
        public bool IncludeLabels { get; set; } = true;
        public Dictionary<string, string> Styling { get; set; } = new();
    }

    // Enums
    public enum VisualizationType
    {
        Graphviz,
        PlantUML,
        VisioXML,
        D3Json,
        Mermaid
    }

    public enum ExportFormat
    {
        Json,
        Xml,
        Excel,
        CSV,
        Report
    }

    public enum ReportFormat
    {
        HTML,
        PDF,
        Word,
        Markdown
    }

    public enum MermaidDiagramType
    {
        Flowchart,
        Sequence,
        Class,
        State,
        EntityRelationship,
        UserJourney,
        Gantt,
        Pie,
        GitGraph
    }
}
