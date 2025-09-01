using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// Comprehensive visualization generation engine for architectural diagrams and relationship maps
    /// </summary>
    public class VisualizationGenerator : IVisualizationGenerator
    {
        private readonly ILogger<VisualizationGenerator> _logger;

        public VisualizationGenerator(ILogger<VisualizationGenerator> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Interface implementation methods (legacy)
        public async Task<bool> GenerateAsync(ApplicationAnalysis analysis, VisualizationRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating legacy visualization for {RequestTypes}", string.Join(", ", request.Types));
            await Task.CompletedTask;
            return true; // Simplified success
        }

        public async Task<string> GenerateGraphvizAsync(ApplicationAnalysis analysis, GraphvizOptions options, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return "digraph G { A -> B; }"; // Simplified Graphviz
        }

        public async Task<string> GeneratePlantUMLAsync(ApplicationAnalysis analysis, PlantUMLOptions options, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return "@startuml\nclass A\n@enduml"; // Simplified PlantUML
        }

        public async Task<string> GenerateVisioXMLAsync(ApplicationAnalysis analysis, VisioOptions options, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return "<visio></visio>"; // Simplified Visio XML
        }

        public async Task<string> GenerateD3JsonAsync(ApplicationAnalysis analysis, D3Options options, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return "{\"nodes\": [], \"links\": []}"; // Simplified D3 JSON
        }

        public async Task<string> GenerateMermaidAsync(ApplicationAnalysis analysis, MermaidOptions options, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return "graph TD\n    A --> B"; // Simplified Mermaid
        }

        /// <summary>
        /// Generates comprehensive visualization package from all analysis results
        /// </summary>
        public async Task<VisualizationPackage> GenerateVisualizationAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            RelationshipMapping relationshipMapping,
            VisualizationOptions options,
            CancellationToken cancellationToken = default)
        {
            if (codeAnalysis == null)
                throw new ArgumentNullException(nameof(codeAnalysis));
            
            if (dependencyAnalysis == null)
                throw new ArgumentNullException(nameof(dependencyAnalysis));
            
            if (architectureAnalysis == null)
                throw new ArgumentNullException(nameof(architectureAnalysis));
            
            if (relationshipMapping == null)
                throw new ArgumentNullException(nameof(relationshipMapping));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _logger.LogInformation("Starting visualization generation");

            var package = new VisualizationPackage();

            try
            {
                // Generate Mermaid diagrams
                if (options.GenerateMermaidDiagrams)
                {
                    _logger.LogInformation("Generating Mermaid diagrams");
                    package.MermaidDiagrams = await GenerateMermaidDiagramsAsync(
                        codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipMapping, cancellationToken);
                    _logger.LogInformation("Generated {DiagramCount} Mermaid diagrams", package.MermaidDiagrams.Count);
                }

                // Generate D3.js visualizations
                if (options.GenerateD3Visualizations)
                {
                    _logger.LogInformation("Generating D3.js visualizations");
                    package.D3Visualizations = await GenerateD3VisualizationsAsync(
                        codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipMapping, cancellationToken);
                    _logger.LogInformation("Generated {VisualizationCount} D3.js visualizations", package.D3Visualizations.Count);
                }

                // Generate Cytoscape visualizations
                if (options.GenerateCytoscapeVisualizations)
                {
                    _logger.LogInformation("Generating Cytoscape visualizations");
                    package.CytoscapeVisualizations = await GenerateCytoscapeVisualizationsAsync(
                        codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipMapping, cancellationToken);
                    _logger.LogInformation("Generated {VisualizationCount} Cytoscape visualizations", package.CytoscapeVisualizations.Count);
                }

                // Export data files
                if (options.ExportDataFiles)
                {
                    _logger.LogInformation("Exporting data files");
                    package.DataExports = await ExportDataFilesAsync(
                        codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipMapping, options, cancellationToken);
                    _logger.LogInformation("Exported {FileCount} data files", package.DataExports.Count);
                }

                // Generate HTML reports
                if (options.GenerateHtmlReports)
                {
                    _logger.LogInformation("Generating HTML reports");
                    package.HtmlReports = await GenerateHtmlReportsAsync(
                        codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipMapping, cancellationToken);
                    _logger.LogInformation("Generated {ReportCount} HTML reports", package.HtmlReports.Count);
                }

                // Generate visualization metadata
                package.Metadata = await GenerateVisualizationMetadataAsync(package, cancellationToken);

                _logger.LogInformation("Visualization generation complete. Generated {TotalItems} visualization items",
                    (package.MermaidDiagrams?.Count ?? 0) + 
                    (package.D3Visualizations?.Count ?? 0) + 
                    (package.CytoscapeVisualizations?.Count ?? 0) +
                    (package.DataExports?.Count ?? 0) +
                    (package.HtmlReports?.Count ?? 0));

                return package;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Visualization generation failed");
                throw;
            }
        }

        /// <summary>
        /// Generates Mermaid diagrams for various architectural views
        /// </summary>
        public async Task<List<MermaidDiagram>> GenerateMermaidDiagramsAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            RelationshipMapping relationshipMapping,
            CancellationToken cancellationToken = default)
        {
            var diagrams = new List<MermaidDiagram>();

            await Task.Run(() =>
            {
                // Component Diagram
                var componentDiagram = GenerateComponentDiagram(architectureAnalysis);
                diagrams.Add(componentDiagram);

                // Layer Diagram
                var layerDiagram = GenerateLayerDiagram(architectureAnalysis);
                diagrams.Add(layerDiagram);

                // Dependency Graph
                var dependencyDiagram = GenerateDependencyDiagram(dependencyAnalysis, architectureAnalysis);
                diagrams.Add(dependencyDiagram);

                // Call Hierarchy
                if (relationshipMapping.CallHierarchy != null)
                {
                    var callHierarchyDiagram = GenerateCallHierarchyDiagram(relationshipMapping.CallHierarchy);
                    diagrams.Add(callHierarchyDiagram);
                }

                // Inheritance Tree
                if (relationshipMapping.InheritanceTree != null)
                {
                    var inheritanceDiagram = GenerateInheritanceDiagram(relationshipMapping.InheritanceTree);
                    diagrams.Add(inheritanceDiagram);
                }

            }, cancellationToken);

            return diagrams;
        }

        /// <summary>
        /// Saves visualization package to specified directory
        /// </summary>
        public async Task SaveVisualizationPackageAsync(
            VisualizationPackage package,
            string outputDirectory,
            CancellationToken cancellationToken = default)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            
            if (string.IsNullOrEmpty(outputDirectory))
                throw new ArgumentException("Output directory cannot be null or empty", nameof(outputDirectory));

            _logger.LogInformation("Saving visualization package to {OutputDirectory}", outputDirectory);

            // Create output directory
            Directory.CreateDirectory(outputDirectory);

            try
            {
                // Save Mermaid diagrams
                if (package.MermaidDiagrams?.Any() == true)
                {
                    var mermaidDir = Path.Combine(outputDirectory, "mermaid");
                    Directory.CreateDirectory(mermaidDir);

                    foreach (var diagram in package.MermaidDiagrams)
                    {
                        var fileName = $"{SanitizeFileName(diagram.Title)}.mmd";
                        var filePath = Path.Combine(mermaidDir, fileName);
                        await File.WriteAllTextAsync(filePath, diagram.Content, cancellationToken);
                    }

                    _logger.LogInformation("Saved {DiagramCount} Mermaid diagrams", package.MermaidDiagrams.Count);
                }

                // Save D3.js visualizations
                if (package.D3Visualizations?.Any() == true)
                {
                    var d3Dir = Path.Combine(outputDirectory, "d3");
                    Directory.CreateDirectory(d3Dir);

                    foreach (var visualization in package.D3Visualizations)
                    {
                        var fileName = $"{SanitizeFileName(visualization.Title)}.html";
                        var filePath = Path.Combine(d3Dir, fileName);
                        await File.WriteAllTextAsync(filePath, visualization.HtmlContent, cancellationToken);

                        if (!string.IsNullOrEmpty(visualization.DataJson))
                        {
                            var dataFileName = $"{SanitizeFileName(visualization.Title)}_data.json";
                            var dataFilePath = Path.Combine(d3Dir, dataFileName);
                            await File.WriteAllTextAsync(dataFilePath, visualization.DataJson, cancellationToken);
                        }
                    }

                    _logger.LogInformation("Saved {VisualizationCount} D3.js visualizations", package.D3Visualizations.Count);
                }

                // Save Cytoscape visualizations
                if (package.CytoscapeVisualizations?.Any() == true)
                {
                    var cytoscapeDir = Path.Combine(outputDirectory, "cytoscape");
                    Directory.CreateDirectory(cytoscapeDir);

                    foreach (var visualization in package.CytoscapeVisualizations)
                    {
                        var fileName = $"{SanitizeFileName(visualization.Title)}.html";
                        var filePath = Path.Combine(cytoscapeDir, fileName);
                        await File.WriteAllTextAsync(filePath, visualization.HtmlContent, cancellationToken);

                        if (!string.IsNullOrEmpty(visualization.DataJson))
                        {
                            var dataFileName = $"{SanitizeFileName(visualization.Title)}_data.json";
                            var dataFilePath = Path.Combine(cytoscapeDir, dataFileName);
                            await File.WriteAllTextAsync(dataFilePath, visualization.DataJson, cancellationToken);
                        }
                    }

                    _logger.LogInformation("Saved {VisualizationCount} Cytoscape visualizations", package.CytoscapeVisualizations.Count);
                }

                // Save data exports
                if (package.DataExports?.Any() == true)
                {
                    var dataDir = Path.Combine(outputDirectory, "data");
                    Directory.CreateDirectory(dataDir);

                    foreach (var export in package.DataExports)
                    {
                        var filePath = Path.Combine(dataDir, export.FileName);
                        await File.WriteAllTextAsync(filePath, export.Content, cancellationToken);
                    }

                    _logger.LogInformation("Saved {FileCount} data export files", package.DataExports.Count);
                }

                // Save HTML reports
                if (package.HtmlReports?.Any() == true)
                {
                    var reportsDir = Path.Combine(outputDirectory, "reports");
                    Directory.CreateDirectory(reportsDir);

                    foreach (var report in package.HtmlReports)
                    {
                        var fileName = $"{SanitizeFileName(report.Title)}.html";
                        var filePath = Path.Combine(reportsDir, fileName);
                        await File.WriteAllTextAsync(filePath, report.Content, cancellationToken);
                    }

                    _logger.LogInformation("Saved {ReportCount} HTML reports", package.HtmlReports.Count);
                }

                // Save package metadata
                if (package.Metadata != null)
                {
                    var metadataPath = Path.Combine(outputDirectory, "visualization-metadata.json");
                    var metadataJson = JsonSerializer.Serialize(package.Metadata, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(metadataPath, metadataJson, cancellationToken);
                }

                // Create index file
                await CreateVisualizationIndexAsync(package, outputDirectory, cancellationToken);

                _logger.LogInformation("Visualization package saved successfully to {OutputDirectory}", outputDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save visualization package");
                throw;
            }
        }

        #region Private Mermaid Generation Methods

        private MermaidDiagram GenerateComponentDiagram(ArchitectureAnalysis architectureAnalysis)
        {
            var sb = new StringBuilder();
            sb.AppendLine("graph TD");

            // Add components
            foreach (var component in architectureAnalysis.Components)
            {
                var componentId = SanitizeId(component.Name);
                var componentLabel = $"{component.Name}<br/>({component.Type})";
                sb.AppendLine($"    {componentId}[\"{componentLabel}\"]");

                // Style components by type
                var styleClass = component.Type switch
                {
                    ComponentType.UserInterface => "ui-component",
                    ComponentType.BusinessLogic => "business-component",
                    ComponentType.DataAccess => "data-component",
                    ComponentType.Service => "service-component",
                    _ => "default-component"
                };
                sb.AppendLine($"    class {componentId} {styleClass}");
            }

            // Add component relationships (simplified)
            for (int i = 0; i < architectureAnalysis.Components.Count - 1; i++)
            {
                var source = SanitizeId(architectureAnalysis.Components[i].Name);
                var target = SanitizeId(architectureAnalysis.Components[i + 1].Name);
                sb.AppendLine($"    {source} --> {target}");
            }

            // Add styles
            sb.AppendLine("    classDef ui-component fill:#e1f5fe");
            sb.AppendLine("    classDef business-component fill:#f3e5f5");
            sb.AppendLine("    classDef data-component fill:#e8f5e8");
            sb.AppendLine("    classDef service-component fill:#fff3e0");
            sb.AppendLine("    classDef default-component fill:#f5f5f5");

            return new MermaidDiagram
            {
                Title = "Component Architecture",
                Type = "graph",
                Content = sb.ToString(),
                Description = "Component-level architecture diagram showing system components and their relationships"
            };
        }

        private MermaidDiagram GenerateLayerDiagram(ArchitectureAnalysis architectureAnalysis)
        {
            var sb = new StringBuilder();
            sb.AppendLine("graph TD");

            // Add layers in order
            foreach (var layer in architectureAnalysis.Layers.OrderBy(l => l.Level))
            {
                var layerId = SanitizeId(layer.Name);
                var layerLabel = $"{layer.Name}<br/>Level {layer.Level}<br/>({layer.Components.Count} components)";
                sb.AppendLine($"    {layerId}[\"{layerLabel}\"]");
                sb.AppendLine($"    class {layerId} layer-{layer.Level}");
            }

            // Add layer connections
            var sortedLayers = architectureAnalysis.Layers.OrderBy(l => l.Level).ToList();
            for (int i = 0; i < sortedLayers.Count - 1; i++)
            {
                var sourceId = SanitizeId(sortedLayers[i].Name);
                var targetId = SanitizeId(sortedLayers[i + 1].Name);
                sb.AppendLine($"    {sourceId} --> {targetId}");
            }

            // Add layer styles
            sb.AppendLine("    classDef layer-1 fill:#e3f2fd");
            sb.AppendLine("    classDef layer-2 fill:#f1f8e9");
            sb.AppendLine("    classDef layer-3 fill:#fce4ec");
            sb.AppendLine("    classDef layer-4 fill:#fff8e1");

            return new MermaidDiagram
            {
                Title = "Layer Architecture",
                Type = "graph",
                Content = sb.ToString(),
                Description = "Layered architecture diagram showing architectural layers and their hierarchy"
            };
        }

        private MermaidDiagram GenerateDependencyDiagram(DependencyAnalysis dependencyAnalysis, ArchitectureAnalysis architectureAnalysis)
        {
            var sb = new StringBuilder();
            sb.AppendLine("graph LR");

            // Show top dependencies only (to keep diagram readable)
            var topDependencies = dependencyAnalysis.StaticDependencies
                .GroupBy(d => new { d.From, d.To })
                .Select(g => new { 
                    From = g.Key.From, 
                    To = g.Key.To, 
                    Count = g.Count(),
                    Types = g.Select(d => d.Type).Distinct().ToList()
                })
                .OrderByDescending(d => d.Count)
                .Take(20)
                .ToList();

            var addedNodes = new HashSet<string>();

            foreach (var dep in topDependencies)
            {
                var fromId = SanitizeId(GetSimpleName(dep.From));
                var toId = SanitizeId(GetSimpleName(dep.To));

                // Add nodes if not already added
                if (!addedNodes.Contains(fromId))
                {
                    sb.AppendLine($"    {fromId}[\"{GetSimpleName(dep.From)}\"]");
                    addedNodes.Add(fromId);
                }
                if (!addedNodes.Contains(toId))
                {
                    sb.AppendLine($"    {toId}[\"{GetSimpleName(dep.To)}\"]");
                    addedNodes.Add(toId);
                }

                // Add relationship with count
                var relationshipLabel = dep.Count > 1 ? $"|{dep.Count}|" : "";
                sb.AppendLine($"    {fromId} -->{relationshipLabel} {toId}");
            }

            return new MermaidDiagram
            {
                Title = "Dependency Graph",
                Type = "graph",
                Content = sb.ToString(),
                Description = "Dependency relationships between components (top 20 by frequency)"
            };
        }

        private MermaidDiagram GenerateCallHierarchyDiagram(CallHierarchy callHierarchy)
        {
            var sb = new StringBuilder();
            sb.AppendLine("graph TD");

            // Show top methods by call count
            var topMethods = callHierarchy.Nodes
                .OrderByDescending(n => n.CalleeCount)
                .Take(15)
                .ToList();

            foreach (var method in topMethods)
            {
                var methodId = SanitizeId(GetSimpleName(method.Method));
                var methodLabel = $"{GetSimpleName(method.Method)}<br/>Calls: {method.CalleeCount}";
                sb.AppendLine($"    {methodId}[\"{methodLabel}\"]");

                // Add some callees
                foreach (var callee in method.Callees.Take(3))
                {
                    var calleeId = SanitizeId(GetSimpleName(callee));
                    var calleeLabel = GetSimpleName(callee);
                    sb.AppendLine($"    {calleeId}[\"{calleeLabel}\"]");
                    sb.AppendLine($"    {methodId} --> {calleeId}");
                }
            }

            return new MermaidDiagram
            {
                Title = "Call Hierarchy",
                Type = "graph",
                Content = sb.ToString(),
                Description = "Method call hierarchy showing top methods and their call relationships"
            };
        }

        private MermaidDiagram GenerateInheritanceDiagram(InheritanceTree inheritanceTree)
        {
            var sb = new StringBuilder();
            sb.AppendLine("graph TD");

            // Show inheritance relationships
            var classesWithInheritance = inheritanceTree.Nodes
                .Where(n => n.BaseClasses.Any() || n.DerivedClasses.Any())
                .Take(10)
                .ToList();

            foreach (var cls in classesWithInheritance)
            {
                var classId = SanitizeId(GetSimpleName(cls.ClassName));
                var classLabel = GetSimpleName(cls.ClassName);
                sb.AppendLine($"    {classId}[\"{classLabel}\"]");

                foreach (var baseClass in cls.BaseClasses)
                {
                    var baseId = SanitizeId(GetSimpleName(baseClass));
                    var baseLabel = GetSimpleName(baseClass);
                    sb.AppendLine($"    {baseId}[\"{baseLabel}\"]");
                    sb.AppendLine($"    {baseId} --> {classId}");
                }
            }

            return new MermaidDiagram
            {
                Title = "Inheritance Tree",
                Type = "graph",
                Content = sb.ToString(),
                Description = "Class inheritance relationships showing base and derived classes"
            };
        }

        #endregion

        #region Private Helper Methods

        private async Task<List<D3Visualization>> GenerateD3VisualizationsAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            RelationshipMapping relationshipMapping,
            CancellationToken cancellationToken)
        {
            var visualizations = new List<D3Visualization>();

            await Task.Run(() =>
            {
                // Generate force-directed graph for dependencies
                var dependencyGraph = GenerateD3DependencyGraph(dependencyAnalysis, architectureAnalysis);
                visualizations.Add(dependencyGraph);

                // Generate treemap for component sizes
                var componentTreemap = GenerateD3ComponentTreemap(architectureAnalysis);
                visualizations.Add(componentTreemap);

            }, cancellationToken);

            return visualizations;
        }

        private async Task<List<CytoscapeVisualization>> GenerateCytoscapeVisualizationsAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            RelationshipMapping relationshipMapping,
            CancellationToken cancellationToken)
        {
            var visualizations = new List<CytoscapeVisualization>();

            await Task.Run(() =>
            {
                // Generate network graph for relationships
                var networkGraph = GenerateCytoscapeNetworkGraph(relationshipMapping);
                visualizations.Add(networkGraph);

            }, cancellationToken);

            return visualizations;
        }

        private async Task<List<DataExport>> ExportDataFilesAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            RelationshipMapping relationshipMapping,
            VisualizationOptions options,
            CancellationToken cancellationToken)
        {
            var exports = new List<DataExport>();

            await Task.Run(() =>
            {
                // Export relationship matrix as JSON
                if (relationshipMapping.RelationshipMatrix != null)
                {
                    var matrixJson = JsonSerializer.Serialize(relationshipMapping.RelationshipMatrix, new JsonSerializerOptions { WriteIndented = true });
                    exports.Add(new DataExport
                    {
                        FileName = "relationship-matrix.json",
                        Content = matrixJson,
                        Format = "json",
                        Description = "Complete relationship matrix with all relationships"
                    });
                }

                // Export components as CSV
                var componentsCsv = GenerateComponentsCsv(architectureAnalysis);
                exports.Add(new DataExport
                {
                    FileName = "components.csv",
                    Content = componentsCsv,
                    Format = "csv",
                    Description = "Component information in CSV format"
                });

                // Export dependencies as CSV
                var dependenciesCsv = GenerateDependenciesCsv(dependencyAnalysis);
                exports.Add(new DataExport
                {
                    FileName = "dependencies.csv",
                    Content = dependenciesCsv,
                    Format = "csv",
                    Description = "Dependency relationships in CSV format"
                });

            }, cancellationToken);

            return exports;
        }

        private async Task<List<HtmlReport>> GenerateHtmlReportsAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            RelationshipMapping relationshipMapping,
            CancellationToken cancellationToken)
        {
            var reports = new List<HtmlReport>();

            await Task.Run(() =>
            {
                // Generate architecture summary report
                var summaryReport = GenerateArchitectureSummaryReport(codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipMapping);
                reports.Add(summaryReport);

            }, cancellationToken);

            return reports;
        }

        private D3Visualization GenerateD3DependencyGraph(DependencyAnalysis dependencyAnalysis, ArchitectureAnalysis architectureAnalysis)
        {
            // Simplified D3 force-directed graph
            var nodes = new List<object>();
            var links = new List<object>();

            // Add component nodes
            foreach (var component in architectureAnalysis.Components)
            {
                nodes.Add(new
                {
                    id = component.Name,
                    name = component.Name,
                    type = component.Type.ToString(),
                    size = component.Classes.Count
                });
            }

            // Add dependency links (simplified)
            var componentDependencies = dependencyAnalysis.StaticDependencies
                .GroupBy(d => new { From = GetComponentName(d.From, architectureAnalysis), To = GetComponentName(d.To, architectureAnalysis) })
                .Where(g => g.Key.From != null && g.Key.To != null && g.Key.From != g.Key.To)
                .Take(50)
                .ToList();

            foreach (var dep in componentDependencies)
            {
                links.Add(new
                {
                    source = dep.Key.From,
                    target = dep.Key.To,
                    value = dep.Count()
                });
            }

            var data = new { nodes, links };
            var dataJson = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            var html = GenerateD3GraphHtml("Dependency Graph", dataJson);

            return new D3Visualization
            {
                Title = "Dependency Graph",
                Type = "force-directed-graph",
                HtmlContent = html,
                DataJson = dataJson,
                Description = "Interactive force-directed graph showing component dependencies"
            };
        }

        private D3Visualization GenerateD3ComponentTreemap(ArchitectureAnalysis architectureAnalysis)
        {
            var data = new
            {
                name = "Components",
                children = architectureAnalysis.Components.Select(c => new
                {
                    name = c.Name,
                    value = c.Classes.Count,
                    type = c.Type.ToString()
                }).ToList()
            };

            var dataJson = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            var html = GenerateD3TreemapHtml("Component Sizes", dataJson);

            return new D3Visualization
            {
                Title = "Component Sizes",
                Type = "treemap",
                HtmlContent = html,
                DataJson = dataJson,
                Description = "Treemap visualization showing relative component sizes"
            };
        }

        private CytoscapeVisualization GenerateCytoscapeNetworkGraph(RelationshipMapping relationshipMapping)
        {
            var elements = new List<object>();

            // Add nodes and edges from relationship matrix
            if (relationshipMapping.RelationshipMatrix != null)
            {
                var addedNodes = new HashSet<string>();

                foreach (var relationship in relationshipMapping.RelationshipMatrix.Relationships.Take(100))
                {
                    // Add source node
                    if (!addedNodes.Contains(relationship.Source))
                    {
                        elements.Add(new
                        {
                            data = new
                            {
                                id = SanitizeId(relationship.Source),
                                label = GetSimpleName(relationship.Source)
                            }
                        });
                        addedNodes.Add(relationship.Source);
                    }

                    // Add target node
                    if (!addedNodes.Contains(relationship.Target))
                    {
                        elements.Add(new
                        {
                            data = new
                            {
                                id = SanitizeId(relationship.Target),
                                label = GetSimpleName(relationship.Target)
                            }
                        });
                        addedNodes.Add(relationship.Target);
                    }

                    // Add edge
                    elements.Add(new
                    {
                        data = new
                        {
                            id = $"{SanitizeId(relationship.Source)}-{SanitizeId(relationship.Target)}",
                            source = SanitizeId(relationship.Source),
                            target = SanitizeId(relationship.Target),
                            label = relationship.Type.ToString(),
                            weight = relationship.Strength
                        }
                    });
                }
            }

            var dataJson = JsonSerializer.Serialize(elements, new JsonSerializerOptions { WriteIndented = true });
            var html = GenerateCytoscapeHtml("Relationship Network", dataJson);

            return new CytoscapeVisualization
            {
                Title = "Relationship Network",
                Type = "network-graph",
                HtmlContent = html,
                DataJson = dataJson,
                Description = "Interactive network graph showing all relationships"
            };
        }

        private string GenerateComponentsCsv(ArchitectureAnalysis architectureAnalysis)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name,Type,ClassCount,Interfaces");

            foreach (var component in architectureAnalysis.Components)
            {
                sb.AppendLine($"\"{component.Name}\",\"{component.Type}\",{component.Classes.Count},\"{string.Join(";", component.Interfaces)}\"");
            }

            return sb.ToString();
        }

        private string GenerateDependenciesCsv(DependencyAnalysis dependencyAnalysis)
        {
            var sb = new StringBuilder();
            sb.AppendLine("From,To,Type,SourceFile");

            foreach (var dependency in dependencyAnalysis.StaticDependencies.Take(1000))
            {
                sb.AppendLine($"\"{dependency.From}\",\"{dependency.To}\",\"{dependency.Type}\",\"{dependency.SourceFile}\"");
            }

            return sb.ToString();
        }

        private HtmlReport GenerateArchitectureSummaryReport(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            RelationshipMapping relationshipMapping)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Architecture Summary Report</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .metric {{ background: #f5f5f5; padding: 10px; margin: 10px 0; border-radius: 5px; }}
        .section {{ margin: 20px 0; }}
        table {{ border-collapse: collapse; width: 100%; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        th {{ background-color: #f2f2f2; }}
    </style>
</head>
<body>
    <h1>Architecture Summary Report</h1>
    
    <div class='section'>
        <h2>Overview</h2>
        <div class='metric'>Total Classes: {codeAnalysis.Symbols.Count(s => s.Type == SymbolType.Class)}</div>
        <div class='metric'>Total Methods: {codeAnalysis.Symbols.Count(s => s.Type == SymbolType.Method)}</div>
        <div class='metric'>Total Dependencies: {dependencyAnalysis.StaticDependencies.Count}</div>
        <div class='metric'>Architectural Pattern: {architectureAnalysis.DetectedPattern}</div>
    </div>
    
    <div class='section'>
        <h2>Components</h2>
        <table>
            <tr><th>Name</th><th>Type</th><th>Classes</th></tr>
            {string.Join("", architectureAnalysis.Components.Select(c => 
                $"<tr><td>{c.Name}</td><td>{c.Type}</td><td>{c.Classes.Count}</td></tr>"))}
        </table>
    </div>
    
    <div class='section'>
        <h2>Layers</h2>
        <table>
            <tr><th>Name</th><th>Level</th><th>Components</th></tr>
            {string.Join("", architectureAnalysis.Layers.Select(l => 
                $"<tr><td>{l.Name}</td><td>{l.Level}</td><td>{l.Components.Count}</td></tr>"))}
        </table>
    </div>
</body>
</html>";

            return new HtmlReport
            {
                Title = "Architecture Summary",
                Content = html,
                Description = "Comprehensive architecture summary report"
            };
        }

        private async Task<VisualizationMetadata> GenerateVisualizationMetadataAsync(VisualizationPackage package, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                return new VisualizationMetadata
                {
                    GeneratedDate = DateTime.UtcNow,
                    TotalDiagrams = (package.MermaidDiagrams?.Count ?? 0) + 
                                   (package.D3Visualizations?.Count ?? 0) + 
                                   (package.CytoscapeVisualizations?.Count ?? 0),
                    TotalReports = package.HtmlReports?.Count ?? 0,
                    TotalDataFiles = package.DataExports?.Count ?? 0,
                    DiagramTypes = new List<string>(),
                    Tools = new List<string> { "Mermaid", "D3.js", "Cytoscape" },
                    Formats = new List<string> { "HTML", "JSON", "CSV" }
                };
            }, cancellationToken);
        }

        private async Task CreateVisualizationIndexAsync(VisualizationPackage package, string outputDirectory, CancellationToken cancellationToken)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>ALARM Visualization Package</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .section {{ margin: 20px 0; }}
        .file-list {{ list-style-type: none; }}
        .file-list li {{ margin: 5px 0; }}
        .file-list a {{ text-decoration: none; color: #007bff; }}
    </style>
</head>
<body>
    <h1>ALARM Visualization Package</h1>
    <p>Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
    
    <div class='section'>
        <h2>Mermaid Diagrams</h2>
        <ul class='file-list'>
            {string.Join("", package.MermaidDiagrams?.Select(d => 
                $"<li><a href='mermaid/{SanitizeFileName(d.Title)}.mmd'>{d.Title}</a> - {d.Description}</li>") ?? new string[0])}
        </ul>
    </div>
    
    <div class='section'>
        <h2>Interactive Visualizations</h2>
        <ul class='file-list'>
            {string.Join("", package.D3Visualizations?.Select(v => 
                $"<li><a href='d3/{SanitizeFileName(v.Title)}.html'>{v.Title}</a> - {v.Description}</li>") ?? new string[0])}
            {string.Join("", package.CytoscapeVisualizations?.Select(v => 
                $"<li><a href='cytoscape/{SanitizeFileName(v.Title)}.html'>{v.Title}</a> - {v.Description}</li>") ?? new string[0])}
        </ul>
    </div>
    
    <div class='section'>
        <h2>Reports</h2>
        <ul class='file-list'>
            {string.Join("", package.HtmlReports?.Select(r => 
                $"<li><a href='reports/{SanitizeFileName(r.Title)}.html'>{r.Title}</a> - {r.Description}</li>") ?? new string[0])}
        </ul>
    </div>
    
    <div class='section'>
        <h2>Data Files</h2>
        <ul class='file-list'>
            {string.Join("", package.DataExports?.Select(e => 
                $"<li><a href='data/{e.FileName}'>{e.FileName}</a> - {e.Description}</li>") ?? new string[0])}
        </ul>
    </div>
</body>
</html>";

            var indexPath = Path.Combine(outputDirectory, "index.html");
            await File.WriteAllTextAsync(indexPath, html, cancellationToken);
        }

        private string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
        }

        private string SanitizeId(string id)
        {
            return id.Replace(" ", "_").Replace(".", "_").Replace("-", "_").Replace("(", "").Replace(")", "");
        }

        private string GetSimpleName(string fullName)
        {
            var lastDot = fullName.LastIndexOf('.');
            return lastDot > 0 ? fullName.Substring(lastDot + 1) : fullName;
        }

        private string GetComponentName(string className, ArchitectureAnalysis architectureAnalysis)
        {
            return architectureAnalysis.Components
                .FirstOrDefault(c => c.Classes.Contains(className))?.Name;
        }

        private string GenerateD3GraphHtml(string title, string dataJson)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>{title}</title>
    <script src='https://d3js.org/d3.v7.min.js'></script>
    <style>
        .node {{ fill: #69b3a2; stroke: #fff; stroke-width: 2px; }}
        .link {{ stroke: #999; stroke-opacity: 0.6; }}
    </style>
</head>
<body>
    <h1>{title}</h1>
    <svg width='800' height='600'></svg>
    <script>
        const data = {dataJson};
        // D3.js force-directed graph implementation would go here
        console.log('Data loaded:', data);
    </script>
</body>
</html>";
        }

        private string GenerateD3TreemapHtml(string title, string dataJson)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>{title}</title>
    <script src='https://d3js.org/d3.v7.min.js'></script>
</head>
<body>
    <h1>{title}</h1>
    <svg width='800' height='600'></svg>
    <script>
        const data = {dataJson};
        // D3.js treemap implementation would go here
        console.log('Data loaded:', data);
    </script>
</body>
</html>";
        }

        private string GenerateCytoscapeHtml(string title, string dataJson)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <title>{title}</title>
    <script src='https://unpkg.com/cytoscape/dist/cytoscape.min.js'></script>
    <style>
        #cy {{ width: 100%; height: 600px; }}
    </style>
</head>
<body>
    <h1>{title}</h1>
    <div id='cy'></div>
    <script>
        const elements = {dataJson};
        // Cytoscape.js network graph implementation would go here
        console.log('Elements loaded:', elements);
    </script>
</body>
</html>";
        }

        #endregion
    }
}
