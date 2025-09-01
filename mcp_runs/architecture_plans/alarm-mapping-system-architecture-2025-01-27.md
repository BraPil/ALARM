# ALARM MAPPING SYSTEM ARCHITECTURE

**Date**: January 27, 2025  
**Objective**: Design comprehensive architectural and dependency mapping system for 100% relationship visualization  
**Target**: ADDS 2019 → ADDS25 migration with complete functionality preservation  

## **🎯 SYSTEM OVERVIEW**

The ALARM Mapping Function provides **100% comprehensive architectural visualization** of legacy applications through multi-layered analysis and automated diagram generation. This system supports ALARM's prime directive of universal legacy app reverse engineering with complete functionality preservation.

## **🏗️ CORE ARCHITECTURE COMPONENTS**

### **1. MULTI-LAYER ANALYSIS ENGINE**

```
┌─────────────────────────────────────────────────────────────┐
│                    ALARM MAPPING CORE                      │
├─────────────────────────────────────────────────────────────┤
│  📁 File System Crawler     │  🔍 Code Analysis Engine    │
│  🌐 Dependency Resolver     │  📊 Relationship Mapper     │
│  🏛️ Architecture Extractor  │  📈 Metrics Calculator      │
│  🔗 Cross-Reference Builder │  🎯 Pattern Detector        │
└─────────────────────────────────────────────────────────────┘
```

#### **A. File System Crawler**
- **Recursive Directory Traversal**: Complete file system enumeration
- **File Type Classification**: Source code, configuration, resources, documentation
- **Metadata Extraction**: Size, dates, permissions, encoding
- **Exclusion Management**: .gitignore, build artifacts, temporary files

#### **B. Code Analysis Engine**
- **Multi-Language Parser**: C#, VB.NET, SQL, XML, JSON, PowerShell
- **Syntax Tree Generation**: Complete AST for each source file
- **Symbol Resolution**: Classes, methods, properties, variables, constants
- **Scope Analysis**: Namespace, assembly, project boundaries

#### **C. Dependency Resolver**
- **Static Dependencies**: Using statements, imports, references
- **Dynamic Dependencies**: Reflection, late binding, configuration-driven
- **External Dependencies**: NuGet packages, COM components, system libraries
- **Database Dependencies**: Tables, views, procedures, functions

### **2. RELATIONSHIP MAPPING MATRIX**

```
┌─────────────────────────────────────────────────────────────┐
│                 RELATIONSHIP TYPES                          │
├─────────────────────────────────────────────────────────────┤
│  🔄 INHERITANCE      │  📞 METHOD CALLS    │  📦 COMPOSITION │
│  🎯 IMPLEMENTATION   │  🏷️  PROPERTY ACCESS │  🔗 AGGREGATION │
│  📋 INTERFACE USAGE  │  🎛️  EVENT HANDLING  │  ⚡ DEPENDENCY   │
│  📊 DATA FLOW        │  🔧 CONFIGURATION    │  🌐 NETWORK     │
└─────────────────────────────────────────────────────────────┘
```

#### **A. Code-Level Relationships**
- **Inheritance Hierarchies**: Base classes, derived classes, interface implementations
- **Method Call Graphs**: Direct calls, indirect calls, virtual dispatch
- **Data Dependencies**: Field access, property usage, parameter passing
- **Event Relationships**: Publishers, subscribers, event chains

#### **B. Architectural Relationships**
- **Layer Dependencies**: Presentation → Business → Data access patterns
- **Module Coupling**: Tight coupling, loose coupling, circular dependencies
- **Component Interfaces**: Public APIs, internal contracts, service boundaries
- **Configuration Dependencies**: App.config, web.config, environment variables

#### **C. Database Relationships**
- **Schema Dependencies**: Table relationships, foreign keys, constraints
- **Procedure Dependencies**: Stored procedure calls, function usage
- **Data Flow**: Insert/Update/Delete patterns, transaction boundaries
- **Performance Relationships**: Indexes, query optimization, caching

### **3. VISUALIZATION ENGINE ARCHITECTURE**

```
┌─────────────────────────────────────────────────────────────┐
│              MULTI-FORMAT OUTPUT SYSTEM                    │
├─────────────────────────────────────────────────────────────┤
│  🎨 PRIMARY: Graphviz DOT  │  📊 SECONDARY: PlantUML      │
│  🏢 TERTIARY: Visio XML    │  🌐 WEB: D3.js Interactive   │
│  📱 MOBILE: SVG Responsive │  📄 DOCS: Mermaid Markdown   │
└─────────────────────────────────────────────────────────────┘
```

#### **A. Primary Engine: Graphviz DOT Integration**
**Advantages**: 
- Mature, stable, handles complex graphs
- Excellent automatic layout algorithms
- High-quality output formats (SVG, PNG, PDF)
- Programmatic control via .NET wrapper

**Implementation**:
```csharp
public class GraphvizRenderer : IVisualizationRenderer
{
    public async Task<RenderResult> GenerateAsync(MappingData data, RenderOptions options)
    {
        var dotGraph = new DotGraphBuilder()
            .WithNodes(data.Components)
            .WithEdges(data.Relationships)
            .WithStyling(options.Theme)
            .Build();
            
        return await _graphvizEngine.RenderAsync(dotGraph, options.Format);
    }
}
```

#### **B. Secondary Engine: PlantUML Integration**
**Advantages**:
- Text-based, version control friendly
- Excellent for UML diagrams
- Good documentation integration
- Active development community

**Use Cases**:
- Class diagrams for OOP analysis
- Sequence diagrams for method flows
- Component diagrams for architecture
- Deployment diagrams for infrastructure

#### **C. Tertiary Engine: Visio XML Export**
**Advantages**:
- Native Microsoft ecosystem integration
- Professional presentation quality
- Advanced formatting capabilities
- Familiar tool for stakeholders

**Implementation Strategy**:
- Generate Visio XML format programmatically
- Use Visio stencils for consistent styling
- Support custom templates for different diagram types
- Enable manual refinement in Visio application

### **4. INTERACTIVE WEB VISUALIZATION**

```
┌─────────────────────────────────────────────────────────────┐
│                WEB-BASED EXPLORATION                       │
├─────────────────────────────────────────────────────────────┤
│  🔍 ZOOM & PAN       │  🎯 SEARCH & FILTER  │  📊 METRICS   │
│  🎨 DYNAMIC STYLING  │  🔗 DRILL-DOWN       │  📋 EXPORT    │
│  ⚡ REAL-TIME UPDATE │  🎛️  LAYER TOGGLE    │  📱 RESPONSIVE │
└─────────────────────────────────────────────────────────────┘
```

#### **A. D3.js Force-Directed Graphs**
- **Interactive Navigation**: Zoom, pan, drag nodes
- **Dynamic Filtering**: Show/hide by type, layer, complexity
- **Real-time Search**: Highlight matching components
- **Contextual Information**: Hover details, click for drill-down

#### **B. Hierarchical Visualization**
- **Tree Views**: File system structure, inheritance hierarchies
- **Sunburst Diagrams**: Nested component relationships
- **Treemaps**: Size-based visualization (lines of code, complexity)
- **Network Graphs**: Cross-cutting concerns, dependencies

## **🔧 IMPLEMENTATION ARCHITECTURE**

### **1. Core Mapping Engine**

```csharp
public interface IAlarmMappingEngine
{
    Task<MappingResult> AnalyzeAsync(AnalysisRequest request);
    Task<VisualizationResult> GenerateVisualizationAsync(MappingResult mapping, VisualizationOptions options);
    Task<ExportResult> ExportAsync(VisualizationResult visualization, ExportFormat format);
}

public class AlarmMappingEngine : IAlarmMappingEngine
{
    private readonly IFileSystemCrawler _crawler;
    private readonly ICodeAnalyzer _analyzer;
    private readonly IDependencyResolver _resolver;
    private readonly IRelationshipMapper _mapper;
    private readonly IVisualizationEngine _visualizer;
    
    public async Task<MappingResult> AnalyzeAsync(AnalysisRequest request)
    {
        // Phase 1: File System Analysis
        var fileSystem = await _crawler.CrawlAsync(request.SourcePath);
        
        // Phase 2: Code Analysis
        var codeAnalysis = await _analyzer.AnalyzeAsync(fileSystem.SourceFiles);
        
        // Phase 3: Dependency Resolution
        var dependencies = await _resolver.ResolveAsync(codeAnalysis);
        
        // Phase 4: Relationship Mapping
        var relationships = await _mapper.MapAsync(dependencies);
        
        return new MappingResult
        {
            FileSystem = fileSystem,
            CodeAnalysis = codeAnalysis,
            Dependencies = dependencies,
            Relationships = relationships,
            Metrics = CalculateMetrics(relationships)
        };
    }
}
```

### **2. Visualization Pipeline**

```csharp
public class VisualizationPipeline
{
    private readonly Dictionary<VisualizationType, IVisualizationRenderer> _renderers;
    
    public VisualizationPipeline()
    {
        _renderers = new Dictionary<VisualizationType, IVisualizationRenderer>
        {
            [VisualizationType.Architecture] = new GraphvizRenderer(),
            [VisualizationType.ClassDiagram] = new PlantUMLRenderer(),
            [VisualizationType.Interactive] = new D3Renderer(),
            [VisualizationType.Visio] = new VisioXmlRenderer()
        };
    }
    
    public async Task<VisualizationResult> GenerateAsync(MappingResult mapping, VisualizationOptions options)
    {
        var renderer = _renderers[options.Type];
        var result = await renderer.GenerateAsync(mapping, options);
        
        return new VisualizationResult
        {
            Type = options.Type,
            Format = options.Format,
            Content = result.Content,
            Metadata = result.Metadata,
            InteractiveUrl = result.InteractiveUrl
        };
    }
}
```

### **3. Data Models**

```csharp
public class MappingResult
{
    public FileSystemAnalysis FileSystem { get; set; }
    public CodeAnalysisResult CodeAnalysis { get; set; }
    public DependencyGraph Dependencies { get; set; }
    public RelationshipMatrix Relationships { get; set; }
    public ArchitectureMetrics Metrics { get; set; }
    public DateTime AnalysisTimestamp { get; set; }
    public string SourceVersion { get; set; }
}

public class RelationshipMatrix
{
    public List<Component> Components { get; set; }
    public List<Relationship> Relationships { get; set; }
    public Dictionary<string, LayerInfo> Layers { get; set; }
    public List<ArchitecturalPattern> Patterns { get; set; }
}

public class Relationship
{
    public string Id { get; set; }
    public string SourceComponentId { get; set; }
    public string TargetComponentId { get; set; }
    public RelationshipType Type { get; set; }
    public RelationshipStrength Strength { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
    public List<string> SourceLocations { get; set; }
}
```

## **📊 VISUALIZATION TYPES & USE CASES**

### **1. System Architecture Overview**
```
┌─────────────────────────────────────────────────────────────┐
│                 ADDS 2019 ARCHITECTURE                     │
├─────────────────────────────────────────────────────────────┤
│  🖥️  Presentation Layer    │  📊 Data Access Layer        │
│  ├─ WinForms UI            │  ├─ Oracle Connections       │
│  ├─ AutoCAD Integration    │  ├─ Spatial Data Handlers    │
│  └─ User Controls          │  └─ Transaction Management    │
│                            │                              │
│  🏢 Business Logic Layer   │  🔧 Infrastructure Layer     │
│  ├─ Domain Services        │  ├─ Configuration Management │
│  ├─ Workflow Engines       │  ├─ Logging & Monitoring     │
│  └─ Validation Rules       │  └─ Security & Authentication│
└─────────────────────────────────────────────────────────────┘
```

### **2. Dependency Flow Diagrams**
- **Layered Dependencies**: Top-down architectural flow
- **Circular Dependencies**: Identify and resolve cycles
- **External Dependencies**: Third-party components, system libraries
- **Database Dependencies**: Schema relationships, data flow

### **3. Component Interaction Maps**
- **Method Call Graphs**: Runtime execution paths
- **Data Flow Diagrams**: Information movement through system
- **Event Flow Charts**: Asynchronous communication patterns
- **Configuration Dependencies**: Settings and their impacts

### **4. Migration Impact Analysis**
```
┌─────────────────────────────────────────────────────────────┐
│              MIGRATION IMPACT VISUALIZATION                 │
├─────────────────────────────────────────────────────────────┤
│  🔴 HIGH IMPACT        │  🟡 MEDIUM IMPACT   │  🟢 LOW IMPACT │
│  ├─ Breaking Changes   │  ├─ API Updates     │  ├─ Compatible │
│  ├─ Major Refactoring │  ├─ Configuration   │  ├─ Direct Port │
│  └─ New Dependencies   │  └─ Minor Updates   │  └─ No Changes  │
└─────────────────────────────────────────────────────────────┘
```

## **🎯 ADDS-SPECIFIC MAPPING FEATURES**

### **1. AutoCAD Map3D Integration Analysis**
- **COM Component Dependencies**: AutoCAD object model usage
- **Drawing Database Interactions**: DWG file operations
- **Spatial Data Workflows**: Coordinate systems, projections
- **Custom Commands**: AutoLISP, .NET customizations

### **2. Oracle Database Schema Mapping**
- **Table Relationships**: Foreign keys, constraints, indexes
- **Stored Procedure Dependencies**: Call graphs, data dependencies
- **Spatial Data Types**: SDO_GEOMETRY usage patterns
- **Performance Bottlenecks**: Query analysis, optimization opportunities

### **3. .NET Framework → .NET Core Migration Paths**
- **API Compatibility**: Framework-specific dependencies
- **Configuration Changes**: app.config → appsettings.json
- **Deployment Models**: Framework-dependent vs self-contained
- **Performance Implications**: Runtime differences, optimization opportunities

## **🚀 IMPLEMENTATION ROADMAP**

### **Phase 1: Core Analysis Engine (Week 1-2)**
1. **File System Crawler**: Complete directory traversal with filtering
2. **Basic Code Parser**: C# syntax analysis, symbol extraction
3. **Dependency Resolver**: Static dependency detection
4. **Data Models**: Core mapping data structures

### **Phase 2: Relationship Mapping (Week 3-4)**
1. **Method Call Analysis**: Direct and indirect call detection
2. **Data Flow Tracking**: Variable usage, parameter passing
3. **Configuration Dependencies**: Settings file analysis
4. **Database Schema Analysis**: Oracle-specific features

### **Phase 3: Visualization Engine (Week 5-6)**
1. **Graphviz Integration**: DOT generation, rendering pipeline
2. **Basic Web Interface**: D3.js interactive visualization
3. **Export Capabilities**: Multiple format support
4. **Styling System**: Customizable themes and layouts

### **Phase 4: ADDS-Specific Features (Week 7-8)**
1. **AutoCAD Analysis**: COM component detection, API usage
2. **Oracle Optimization**: Spatial data analysis, performance mapping
3. **Migration Planning**: Impact analysis, risk assessment
4. **Documentation Generation**: Automated report creation

## **📈 SUCCESS METRICS**

### **Completeness Metrics**
- **File Coverage**: 100% of source files analyzed
- **Relationship Detection**: 95%+ of dependencies identified
- **Cross-Reference Accuracy**: 98%+ precision in relationship mapping
- **Performance**: Analysis of 100K+ LOC within 10 minutes

### **Quality Metrics**
- **Visualization Clarity**: Stakeholder comprehension testing
- **Interactive Performance**: <2s response time for filtering/navigation
- **Export Quality**: Professional-grade diagram output
- **Documentation Completeness**: 100% component coverage

### **ADDS Migration Metrics**
- **Compatibility Analysis**: 100% API compatibility assessment
- **Risk Identification**: All breaking changes identified
- **Migration Path Clarity**: Step-by-step migration plan
- **Validation Coverage**: 100% functionality preservation verification

## **🔧 TECHNOLOGY STACK**

### **Core Technologies**
- **.NET 8**: Primary development platform
- **Roslyn**: C# code analysis and compilation
- **Entity Framework Core**: Database analysis and modeling
- **Graphviz**: Primary visualization engine
- **PlantUML**: UML diagram generation

### **Web Technologies**
- **ASP.NET Core**: Web API and hosting
- **D3.js**: Interactive web visualizations
- **SignalR**: Real-time updates and collaboration
- **Bootstrap**: Responsive UI framework

### **Integration Technologies**
- **Microsoft.CodeAnalysis**: Roslyn compiler platform
- **Oracle.ManagedDataAccess**: Oracle database connectivity
- **Autodesk.AutoCAD.Interop**: AutoCAD integration
- **System.Reflection**: Dynamic code analysis

This comprehensive architecture provides ALARM with the capability to generate **100% complete architectural and dependency maps** for any legacy application, with specialized features for the ADDS 2019 → ADDS25 migration project. The multi-layered approach ensures both broad coverage and deep analysis, while the flexible visualization system supports various stakeholder needs and use cases.
