using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALARM.Mapping.Core.Models
{
    /// <summary>
    /// Comprehensive analysis result for a target application
    /// </summary>
    public class ApplicationAnalysis
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string RootPath { get; set; } = string.Empty;
        public DateTime AnalysisTimestamp { get; set; } = DateTime.UtcNow;
        public TimeSpan AnalysisDuration { get; set; }
        
        public FileSystemAnalysis FileSystemAnalysis { get; set; } = new();
        public CodeAnalysis CodeAnalysis { get; set; } = new();
        public DependencyAnalysis DependencyAnalysis { get; set; } = new();
        public ArchitectureAnalysis ArchitectureAnalysis { get; set; } = new();
        public RelationshipMatrix RelationshipMatrix { get; set; } = new();
        
        public AnalysisMetrics Metrics { get; set; } = new();
        public List<AnalysisWarning> Warnings { get; set; } = new();
        public AnalysisConfiguration Configuration { get; set; } = new();
    }

    /// <summary>
    /// File system structure analysis
    /// </summary>
    public class FileSystemAnalysis
    {
        public int TotalFiles { get; set; }
        public int TotalDirectories { get; set; }
        public long TotalSizeBytes { get; set; }
        
        public Dictionary<string, int> FileTypeDistribution { get; set; } = new();
        public List<FileInfo> SourceFiles { get; set; } = new();
        public List<FileInfo> ConfigurationFiles { get; set; } = new();
        public List<FileInfo> ResourceFiles { get; set; } = new();
        public List<FileInfo> DocumentationFiles { get; set; } = new();
        
        public List<string> ExcludedPaths { get; set; } = new();
        public DirectoryStructure RootStructure { get; set; } = new();
    }

    /// <summary>
    /// Code structure and symbol analysis
    /// </summary>
    public class CodeAnalysis
    {
        public int TotalLinesOfCode { get; set; }
        public int TotalClasses { get; set; }
        public int TotalMethods { get; set; }
        public int TotalProperties { get; set; }
        public int TotalInterfaces { get; set; }
        
        public Dictionary<string, LanguageAnalysis> LanguageAnalysis { get; set; } = new();
        public List<CodeSymbol> Symbols { get; set; } = new();
        public List<Namespace> Namespaces { get; set; } = new();
        public List<Assembly> Assemblies { get; set; } = new();
        
        public ComplexityMetrics Complexity { get; set; } = new();
        public QualityMetrics Quality { get; set; } = new();
    }

    /// <summary>
    /// Dependency relationship analysis
    /// </summary>
    public class DependencyAnalysis
    {
        public List<StaticDependency> StaticDependencies { get; set; } = new();
        public List<DynamicDependency> DynamicDependencies { get; set; } = new();
        public List<ExternalDependency> ExternalDependencies { get; set; } = new();
        public List<DatabaseDependency> DatabaseDependencies { get; set; } = new();
        
        public DependencyGraph DependencyGraph { get; set; } = new();
        public List<CircularDependency> CircularDependencies { get; set; } = new();
        public List<string> UnresolvedDependencies { get; set; } = new();
    }

    /// <summary>
    /// High-level architecture analysis
    /// </summary>
    public class ArchitectureAnalysis
    {
        public ArchitecturalPattern DetectedPattern { get; set; } = ArchitecturalPattern.Unknown;
        public List<Layer> Layers { get; set; } = new();
        public List<Component> Components { get; set; } = new();
        public List<Module> Modules { get; set; } = new();
        
        public List<DesignPattern> DesignPatterns { get; set; } = new();
        public List<ArchitecturalViolation> Violations { get; set; } = new();
        public CohesionMetrics Cohesion { get; set; } = new();
        public CouplingMetrics Coupling { get; set; } = new();
    }

    /// <summary>
    /// Comprehensive relationship mapping
    /// </summary>
    public class RelationshipMatrix
    {
        // Legacy relationship lists (for backward compatibility)
        public List<InheritanceRelationship> Inheritance { get; set; } = new();
        public List<CompositionRelationship> Composition { get; set; } = new();
        public List<AggregationRelationship> Aggregation { get; set; } = new();
        public List<AssociationRelationship> Association { get; set; } = new();
        public List<DependencyRelationship> Dependencies { get; set; } = new();
        public List<MethodCallRelationship> MethodCalls { get; set; } = new();
        public List<PropertyAccessRelationship> PropertyAccess { get; set; } = new();
        public List<EventRelationship> Events { get; set; } = new();
        
        public RelationshipGraph Graph { get; set; } = new();
        public RelationshipStatistics Statistics { get; set; } = new();
        
        // New unified relationship model (Phase 5)
        public List<Relationship> Relationships { get; set; } = new();
        public List<string> Sources { get; set; } = new();
        public List<string> Targets { get; set; } = new();
        public List<RelationshipType> RelationshipTypes { get; set; } = new();
    }

    /// <summary>
    /// File information with metadata
    /// </summary>
    public class FileInfo
    {
        public string FullPath { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public FileType Type { get; set; } = FileType.Unknown;
        
        public long SizeBytes { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
        public string Encoding { get; set; } = string.Empty;
        
        public int LineCount { get; set; }
        public string Hash { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Directory structure representation
    /// </summary>
    public class DirectoryStructure
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string RelativePath { get; set; } = string.Empty;
        
        public List<DirectoryStructure> Subdirectories { get; set; } = new();
        public List<FileInfo> Files { get; set; } = new();
        
        public int TotalFiles { get; set; }
        public int TotalDirectories { get; set; }
        public long TotalSizeBytes { get; set; }
    }

    /// <summary>
    /// Code symbol representation
    /// </summary>
    public class CodeSymbol
    {
        public string Name { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public SymbolType Type { get; set; } = SymbolType.Unknown;
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Unknown;
        
        public string Namespace { get; set; } = string.Empty;
        public string Assembly { get; set; } = string.Empty;
        public string SourceFile { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        
        public List<CodeSymbol> Members { get; set; } = new();
        public List<CodeSymbol> Parameters { get; set; } = new();
        public List<string> Attributes { get; set; } = new();
        public List<string> Modifiers { get; set; } = new();
        
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    /// <summary>
    /// Analysis configuration
    /// </summary>
    public class AnalysisConfiguration
    {
        public string TargetPath { get; set; } = string.Empty;
        public List<string> IncludePatterns { get; set; } = new();
        public List<string> ExcludePatterns { get; set; } = new();
        public List<string> LanguageExtensions { get; set; } = new();
        
        public bool AnalyzeStaticDependencies { get; set; } = true;
        public bool AnalyzeDynamicDependencies { get; set; } = true;
        public bool AnalyzeDatabaseDependencies { get; set; } = true;
        public bool GenerateVisualization { get; set; } = true;
        
        public VisualizationOptions Visualization { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Visualization generation options
    /// </summary>
    public class VisualizationOptions
    {
        // Legacy visualization options (for backward compatibility)
        public bool GenerateGraphviz { get; set; } = true;
        public bool GeneratePlantUML { get; set; } = true;
        public bool GenerateVisioXML { get; set; } = true;
        public bool GenerateD3Json { get; set; } = true;
        
        public GraphvizOptions Graphviz { get; set; } = new();
        public PlantUMLOptions PlantUML { get; set; } = new();
        public VisioOptions Visio { get; set; } = new();
        public D3Options D3 { get; set; } = new();
        
        // New visualization options (Phase 6)
        public bool GenerateMermaidDiagrams { get; set; } = true;
        public bool GenerateD3Visualizations { get; set; } = true;
        public bool GenerateCytoscapeVisualizations { get; set; } = true;
        public bool ExportDataFiles { get; set; } = true;
        public bool GenerateHtmlReports { get; set; } = true;
        public string OutputDirectory { get; set; } = string.Empty;
    }

    /// <summary>
    /// Analysis metrics and statistics
    /// </summary>
    public class AnalysisMetrics
    {
        public int TotalSymbols { get; set; }
        public int TotalRelationships { get; set; }
        public int TotalDependencies { get; set; }
        public double AverageComplexity { get; set; }
        public double CohesionScore { get; set; }
        public double CouplingScore { get; set; }
        
        public Dictionary<string, int> SymbolCounts { get; set; } = new();
        public Dictionary<string, int> RelationshipCounts { get; set; } = new();
        public Dictionary<string, double> QualityScores { get; set; } = new();
    }

    /// <summary>
    /// Analysis warning or issue
    /// </summary>
    public class AnalysisWarning
    {
        public WarningLevel Level { get; set; } = WarningLevel.Info;
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string SourceFile { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public Dictionary<string, object> Context { get; set; } = new();
    }

    // Supporting classes and enums
    public class LanguageAnalysis
    {
        public string Language { get; set; } = string.Empty;
        public int FileCount { get; set; }
        public int LineCount { get; set; }
        public List<CodeSymbol> Symbols { get; set; } = new();
    }

    public class ComplexityMetrics
    {
        public double CyclomaticComplexity { get; set; }
        public double CognitiveComplexity { get; set; }
        public int NestingDepth { get; set; }
        public int Halstead { get; set; }
    }

    public class QualityMetrics
    {
        public double Maintainability { get; set; }
        public double Testability { get; set; }
        public double Readability { get; set; }
        public double Documentation { get; set; }
    }

    public class Namespace
    {
        public string Name { get; set; } = string.Empty;
        public List<CodeSymbol> Types { get; set; } = new();
        public List<string> Usings { get; set; } = new();
    }

    public class Assembly
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<Namespace> Namespaces { get; set; } = new();
    }

    // Dependency classes
    public class StaticDependency
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public DependencyType Type { get; set; } = DependencyType.Unknown;
        public string SourceFile { get; set; } = string.Empty;
        public int LineNumber { get; set; }
    }

    public class DynamicDependency : StaticDependency
    {
        public string ReflectionTarget { get; set; } = string.Empty;
        public bool IsConditional { get; set; }
    }

    public class ExternalDependency
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty; // NuGet, COM, System
        public List<string> ReferencedBy { get; set; } = new();
    }

    public class DatabaseDependency
    {
        public string DatabaseName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public List<string> Tables { get; set; } = new();
        public List<string> Views { get; set; } = new();
        public List<string> Procedures { get; set; } = new();
    }

    public class DependencyGraph
    {
        public List<GraphNode> Nodes { get; set; } = new();
        public List<GraphEdge> Edges { get; set; } = new();
    }

    public class CircularDependency
    {
        public List<string> Cycle { get; set; } = new();
        public DependencyType Type { get; set; } = DependencyType.Unknown;
    }

    // Architecture classes
    public class Layer
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public List<string> Components { get; set; } = new();
        public List<string> Dependencies { get; set; } = new();
    }

    public class Component
    {
        public string Name { get; set; } = string.Empty;
        public ComponentType Type { get; set; } = ComponentType.Unknown;
        public List<string> Classes { get; set; } = new();
        public List<string> Interfaces { get; set; } = new();
    }

    public class Module
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Assemblies { get; set; } = new();
        public List<Component> Components { get; set; } = new();
    }

    public class DesignPattern
    {
        public PatternType Type { get; set; } = PatternType.Unknown;
        public string Name { get; set; } = string.Empty;
        public List<string> ParticipatingClasses { get; set; } = new();
        public double Confidence { get; set; }
    }

    public class ArchitecturalViolation
    {
        public ViolationType Type { get; set; } = ViolationType.Unknown;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ViolationSeverity Severity { get; set; } = ViolationSeverity.Low;
    }

    public class CohesionMetrics
    {
        public double LCOM { get; set; } // Lack of Cohesion of Methods
        public double TCC { get; set; }  // Tight Class Cohesion
    }

    public class CouplingMetrics
    {
        public double AfferentCoupling { get; set; }
        public double EfferentCoupling { get; set; }
        public double Instability { get; set; }
    }

    // Relationship classes
    public class InheritanceRelationship
    {
        public string BaseType { get; set; } = string.Empty;
        public string DerivedType { get; set; } = string.Empty;
        public InheritanceType Type { get; set; } = InheritanceType.ClassInheritance;
    }

    public class CompositionRelationship
    {
        public string Owner { get; set; } = string.Empty;
        public string Owned { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
    }

    public class AggregationRelationship
    {
        public string Whole { get; set; } = string.Empty;
        public string Part { get; set; } = string.Empty;
        public string RelationshipName { get; set; } = string.Empty;
    }

    public class AssociationRelationship
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public AssociationType Type { get; set; } = AssociationType.OneToOne;
    }

    public class DependencyRelationship
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public DependencyType Type { get; set; } = DependencyType.Unknown;
    }

    public class MethodCallRelationship
    {
        public string Caller { get; set; } = string.Empty;
        public string Callee { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public int CallCount { get; set; }
    }

    public class PropertyAccessRelationship
    {
        public string Accessor { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public AccessType AccessType { get; set; } = AccessType.Read;
    }

    public class EventRelationship
    {
        public string Publisher { get; set; } = string.Empty;
        public string Subscriber { get; set; } = string.Empty;
        public string EventName { get; set; } = string.Empty;
    }

    public class RelationshipGraph
    {
        public List<GraphNode> Nodes { get; set; } = new();
        public List<GraphEdge> Edges { get; set; } = new();
    }

    public class RelationshipStatistics
    {
        public Dictionary<string, int> RelationshipCounts { get; set; } = new();
        public double AverageRelationshipsPerClass { get; set; }
        public int MaxRelationshipsPerClass { get; set; }
        public int TotalRelationships { get; set; }
        public Dictionary<string, int> RelationshipTypeDistribution { get; set; } = new();
        public double AverageRelationshipStrength { get; set; }
        public int ComponentRelationshipCount { get; set; }
        public string StrongestComponentRelationship { get; set; } = string.Empty;
        public int LayerRelationshipCount { get; set; }
        public int LayerViolationCount { get; set; }
        public int TotalMethods { get; set; }
        public int RootMethodCount { get; set; }
        public int LeafMethodCount { get; set; }
        public int MaxCallDepth { get; set; }
        public int TotalClasses { get; set; }
        public int RootClassCount { get; set; }
        public int MaxInheritanceDepth { get; set; }
    }

    public class GraphNode
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public NodeType Type { get; set; } = NodeType.Unknown;
        public Dictionary<string, object> Attributes { get; set; } = new();
    }

    public class GraphEdge
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public EdgeType Type { get; set; } = EdgeType.Unknown;
        public Dictionary<string, object> Attributes { get; set; } = new();
    }

    // Phase 5: Relationship Mapping Models
    public class RelationshipMapping
    {
        public RelationshipMatrix? RelationshipMatrix { get; set; }
        public List<ComponentRelationship>? ComponentRelationships { get; set; }
        public List<LayerRelationship>? LayerRelationships { get; set; }
        public DependencyMatrix? DependencyMatrix { get; set; }
        public CallHierarchy? CallHierarchy { get; set; }
        public InheritanceTree? InheritanceTree { get; set; }
        public RelationshipStatistics? Statistics { get; set; }
    }



    public class Relationship
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public RelationshipType Type { get; set; }
        public double Strength { get; set; }
        public RelationshipDirection Direction { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class ComponentRelationship
    {
        public string SourceComponent { get; set; } = string.Empty;
        public string TargetComponent { get; set; } = string.Empty;
        public int RelationshipCount { get; set; }
        public List<DependencyType> RelationshipTypes { get; set; } = new();
        public double Strength { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class LayerRelationship
    {
        public string SourceLayer { get; set; } = string.Empty;
        public string TargetLayer { get; set; } = string.Empty;
        public int SourceLevel { get; set; }
        public int TargetLevel { get; set; }
        public int RelationshipCount { get; set; }
        public double Strength { get; set; }
        public bool IsViolation { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class DependencyMatrix
    {
        public List<DependencyMatrixEntry> Entries { get; set; } = new();
        public List<string> Components { get; set; } = new();
        public double MaxStrength { get; set; }
        public double MinStrength { get; set; }
    }

    public class DependencyMatrixEntry
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public int DependencyCount { get; set; }
        public double Strength { get; set; }
        public List<DependencyType> DependencyTypes { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class CallHierarchy
    {
        public List<CallHierarchyNode> Nodes { get; set; } = new();
        public List<CallHierarchyNode> RootMethods { get; set; } = new();
        public List<CallHierarchyNode> LeafMethods { get; set; } = new();
    }

    public class CallHierarchyNode
    {
        public string Method { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public List<string> Callees { get; set; } = new();
        public List<string> Callers { get; set; } = new();
        public int CalleeCount { get; set; }
        public int CallerCount { get; set; }
        public double Complexity { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class InheritanceTree
    {
        public List<InheritanceNode> Nodes { get; set; } = new();
        public List<InheritanceNode> RootClasses { get; set; } = new();
        public List<InheritanceNode> LeafClasses { get; set; } = new();
    }

    public class InheritanceNode
    {
        public string ClassName { get; set; } = string.Empty;
        public List<string> BaseClasses { get; set; } = new();
        public List<string> DerivedClasses { get; set; } = new();
        public int InheritanceDepth { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsInterface { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    // Phase 6: Visualization Models
    public class VisualizationPackage
    {
        public List<MermaidDiagram>? MermaidDiagrams { get; set; }
        public List<D3Visualization>? D3Visualizations { get; set; }
        public List<CytoscapeVisualization>? CytoscapeVisualizations { get; set; }
        public List<DataExport>? DataExports { get; set; }
        public List<HtmlReport>? HtmlReports { get; set; }
        public VisualizationMetadata? Metadata { get; set; }
    }

    public class MermaidDiagram
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class D3Visualization
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public string DataJson { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CytoscapeVisualization
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public string DataJson { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class DataExport
    {
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class HtmlReport
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class VisualizationMetadata
    {
        public DateTime GeneratedDate { get; set; }
        public int TotalDiagrams { get; set; }
        public int TotalReports { get; set; }
        public int TotalDataFiles { get; set; }
        public List<string> DiagramTypes { get; set; } = new();
        public List<string> Tools { get; set; } = new();
        public List<string> Formats { get; set; } = new();
    }

    // Options Classes
    public class RelationshipOptions
    {
        public bool BuildRelationshipMatrix { get; set; } = true;
        public bool BuildComponentRelationships { get; set; } = true;
        public bool BuildLayerRelationships { get; set; } = true;
        public bool BuildDependencyMatrix { get; set; } = true;
        public bool BuildCallHierarchy { get; set; } = true;
        public bool BuildInheritanceTree { get; set; } = true;
    }



    // Visualization options
    public class GraphvizOptions
    {
        public string Engine { get; set; } = "dot"; // dot, neato, fdp, sfdp, twopi, circo
        public string Format { get; set; } = "svg"; // svg, png, pdf, ps
        public bool GenerateMap { get; set; } = true;
        public Dictionary<string, string> GlobalAttributes { get; set; } = new();
    }

    public class PlantUMLOptions
    {
        public PlantUMLDiagramType DiagramType { get; set; } = PlantUMLDiagramType.Class;
        public bool IncludePrivateMembers { get; set; } = false;
        public bool ShowPackages { get; set; } = true;
        public string Theme { get; set; } = "default";
    }

    public class VisioOptions
    {
        public string Template { get; set; } = "UML_Model_Diagram_US.vst";
        public bool GenerateStencils { get; set; } = true;
        public Dictionary<string, string> ShapeMapping { get; set; } = new();
    }

    public class D3Options
    {
        public bool GenerateForceDirected { get; set; } = true;
        public bool GenerateHierarchical { get; set; } = true;
        public bool GenerateMatrix { get; set; } = true;
        public Dictionary<string, object> CustomOptions { get; set; } = new();
    }

    // Enums
    public enum FileType
    {
        Unknown,
        SourceCode,
        Configuration,
        Resource,
        Documentation,
        Binary,
        Archive
    }

    public enum SymbolType
    {
        Unknown,
        Class,
        Interface,
        Struct,
        Enum,
        Method,
        Property,
        Field,
        Event,
        Delegate,
        Namespace,
        Assembly
    }

    public enum AccessModifier
    {
        Unknown,
        Public,
        Private,
        Protected,
        Internal,
        ProtectedInternal,
        PrivateProtected
    }

    public enum WarningLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    public enum DependencyType
    {
        Unknown,
        Using,
        Inheritance,
        Composition,
        Aggregation,
        Association,
        MethodCall,
        PropertyAccess,
        EventSubscription
    }

    public enum ArchitecturalPattern
    {
        Unknown,
        Layered,
        MVC,
        MVP,
        MVVM,
        Microservices,
        EventDriven,
        PipeAndFilter,
        Repository,
        ServiceOriented
    }

    public enum ComponentType
    {
        Unknown,
        UserInterface,
        BusinessLogic,
        DataAccess,
        Service,
        Utility,
        Infrastructure
    }

    public enum PatternType
    {
        Unknown,
        Singleton,
        Factory,
        AbstractFactory,
        Builder,
        Prototype,
        Adapter,
        Bridge,
        Composite,
        Decorator,
        Facade,
        Flyweight,
        Proxy,
        Observer,
        Strategy,
        Command,
        State,
        TemplateMethod,
        Visitor
    }

    public enum ViolationType
    {
        Unknown,
        LayerViolation,
        CircularDependency,
        UnstableInterface,
        GodClass,
        FeatureEnvy,
        DataClass
    }

    public enum ViolationSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum InheritanceType
    {
        ClassInheritance,
        InterfaceImplementation,
        AbstractClassInheritance
    }

    public enum AssociationType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany
    }

    public enum AccessType
    {
        Read,
        Write,
        ReadWrite
    }

    public enum NodeType
    {
        Unknown,
        Class,
        Interface,
        Method,
        Property,
        Assembly,
        Namespace
    }

    public enum EdgeType
    {
        Unknown,
        Inheritance,
        Implementation,
        Composition,
        Aggregation,
        Association,
        Dependency,
        MethodCall
    }

    public enum PlantUMLDiagramType
    {
        Class,
        Component,
        Deployment,
        UseCase,
        Sequence,
        Activity,
        State
    }

    public enum RelationshipType
    {
        Unknown,
        MethodCall,
        PropertyAccess,
        Inheritance,
        Using,
        ComponentMembership,
        LayerMembership
    }

    public enum RelationshipDirection
    {
        Inbound,
        Outbound,
        Bidirectional
    }
}
