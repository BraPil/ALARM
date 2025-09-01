using ALARM.Mapping.Core;
using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using ALARM.Mapping.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ALARM.Mapping.Core
{
    /// <summary>
    /// ALARM Mapping Function demonstration program
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🎯 ALARM Universal Mapping Function");
            Console.WriteLine("====================================");

            // Setup dependency injection
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
                    services.AddSingleton<IFileSystemCrawler, FileSystemCrawler>();
                    services.AddSingleton<ICodeAnalysisEngine, CodeAnalysisEngine>();
                    services.AddSingleton<IDependencyResolver, DependencyResolver>();
                    services.AddSingleton<IArchitectureAnalyzer, ArchitectureAnalyzer>();
                    services.AddSingleton<IRelationshipMapper, RelationshipMapper>();
                    services.AddSingleton<IVisualizationGenerator, VisualizationGenerator>();
                    // Note: Other services would be added here when implemented
                })
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var fileSystemCrawler = host.Services.GetRequiredService<IFileSystemCrawler>();
            var codeAnalysisEngine = host.Services.GetRequiredService<ICodeAnalysisEngine>();
            var dependencyResolver = host.Services.GetRequiredService<IDependencyResolver>();
            var architectureAnalyzer = host.Services.GetRequiredService<IArchitectureAnalyzer>();
            var relationshipMapper = host.Services.GetRequiredService<IRelationshipMapper>();
            var visualizationGenerator = host.Services.GetRequiredService<IVisualizationGenerator>();

            try
            {
                // Test basic file system crawling functionality
                var targetPath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
                
                if (!Directory.Exists(targetPath))
                {
                    Console.WriteLine($"❌ Target path does not exist: {targetPath}");
                    return;
                }

                Console.WriteLine($"🔍 Analyzing target application: {targetPath}");
                Console.WriteLine();

                // Configure crawl options
                var crawlOptions = new CrawlOptions
                {
                    IncludePatterns = new List<string> { "*.*" },
                    ExcludePatterns = new List<string> { "bin/*", "obj/*", "*.tmp", "*.log", ".git/*", "node_modules/*" },
                    CalculateHashes = false, // Disable for faster demo
                    ExtractMetadata = true,
                    MaxFileSize = 10 * 1024 * 1024, // 10MB
                    MaxDepth = 20
                };

                // Create progress reporter
                var progress = new Progress<CrawlProgress>(p =>
                {
                    Console.Write($"\r📁 Processing: {p.FilesProcessed} files, {p.DirectoriesProcessed} directories - {p.CurrentPath}");
                });

                // Perform file system analysis
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var fileSystemAnalysis = await fileSystemCrawler.CrawlAsync(targetPath, crawlOptions, progress);
                stopwatch.Stop();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("📊 **FILE SYSTEM ANALYSIS RESULTS**");
                Console.WriteLine("=====================================");
                Console.WriteLine($"📁 Total Directories: {fileSystemAnalysis.TotalDirectories:N0}");
                Console.WriteLine($"📄 Total Files: {fileSystemAnalysis.TotalFiles:N0}");
                Console.WriteLine($"💾 Total Size: {FormatBytes(fileSystemAnalysis.TotalSizeBytes)}");
                Console.WriteLine($"⏱️ Analysis Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
                Console.WriteLine();

                // File type distribution
                Console.WriteLine("📈 **FILE TYPE DISTRIBUTION**");
                Console.WriteLine("=============================");
                foreach (var (extension, count) in fileSystemAnalysis.FileTypeDistribution.OrderByDescending(x => x.Value).Take(10))
                {
                    var percentage = (double)count / fileSystemAnalysis.TotalFiles * 100;
                    Console.WriteLine($"{extension.PadRight(10)} {count,6:N0} files ({percentage,5:F1}%)");
                }

                if (fileSystemAnalysis.FileTypeDistribution.Count > 10)
                {
                    var remaining = fileSystemAnalysis.FileTypeDistribution.Skip(10).Sum(x => x.Value);
                    var percentage = (double)remaining / fileSystemAnalysis.TotalFiles * 100;
                    Console.WriteLine($"{"Others".PadRight(10)} {remaining,6:N0} files ({percentage,5:F1}%)");
                }

                Console.WriteLine();

                // Categorized files
                Console.WriteLine("🗂️ **FILE CATEGORIES**");
                Console.WriteLine("======================");
                Console.WriteLine($"💻 Source Code Files: {fileSystemAnalysis.SourceFiles.Count:N0}");
                Console.WriteLine($"⚙️ Configuration Files: {fileSystemAnalysis.ConfigurationFiles.Count:N0}");
                Console.WriteLine($"🎨 Resource Files: {fileSystemAnalysis.ResourceFiles.Count:N0}");
                Console.WriteLine($"📚 Documentation Files: {fileSystemAnalysis.DocumentationFiles.Count:N0}");
                Console.WriteLine();

                // Largest files
                var largestFiles = fileSystemAnalysis.SourceFiles
                    .Concat(fileSystemAnalysis.ConfigurationFiles)
                    .Concat(fileSystemAnalysis.ResourceFiles)
                    .Concat(fileSystemAnalysis.DocumentationFiles)
                    .OrderByDescending(f => f.SizeBytes)
                    .Take(5)
                    .ToList();

                if (largestFiles.Any())
                {
                    Console.WriteLine("📏 **LARGEST FILES**");
                    Console.WriteLine("===================");
                    foreach (var file in largestFiles)
                    {
                        Console.WriteLine($"{FormatBytes(file.SizeBytes).PadRight(10)} {file.RelativePath}");
                    }
                    Console.WriteLine();
                }

                // Show some source code files
                if (fileSystemAnalysis.SourceFiles.Any())
                {
                    Console.WriteLine("💻 **SOURCE CODE FILES (Sample)**");
                    Console.WriteLine("=================================");
                    var sourceFiles = fileSystemAnalysis.SourceFiles.Take(10);
                    foreach (var file in sourceFiles)
                    {
                        var lines = file.LineCount > 0 ? $" ({file.LineCount:N0} lines)" : "";
                        Console.WriteLine($"   {file.RelativePath}{lines}");
                    }
                    if (fileSystemAnalysis.SourceFiles.Count > 10)
                    {
                        Console.WriteLine($"   ... and {fileSystemAnalysis.SourceFiles.Count - 10:N0} more source files");
                    }
                    Console.WriteLine();
                }

                // **PHASE 2: CODE ANALYSIS**
                CodeAnalysis? codeAnalysis = null;
                if (fileSystemAnalysis.SourceFiles.Any())
                {
                    Console.WriteLine("🔍 **PHASE 2: CODE ANALYSIS**");
                    Console.WriteLine("=============================");

                    var codeAnalysisOptions = new CodeAnalysisOptions
                    {
                        SupportedLanguages = new List<string> { "csharp", "vb", "sql", "xml", "json", "powershell", "autolisp" },
                        ExtractDocumentation = true,
                        CalculateMetrics = true,
                        DetectPatterns = true,
                        IncludePrivateMembers = true,
                        MaxFileSize = 10 * 1024 * 1024 // 10MB
                    };

                    var codeProgress = new Progress<CodeAnalysisProgress>(p =>
                    {
                        Console.Write($"\r🔍 Analyzing: {p.FilesProcessed} files, {p.SymbolsExtracted} symbols - {Path.GetFileName(p.CurrentFile)}");
                    });

                    var codeStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    codeAnalysis = await codeAnalysisEngine.AnalyzeAsync(fileSystemAnalysis, codeAnalysisOptions, codeProgress);
                    codeStopwatch.Stop();

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("📊 **CODE ANALYSIS RESULTS**");
                    Console.WriteLine("============================");
                    Console.WriteLine($"🏗️ Total Classes: {codeAnalysis.TotalClasses:N0}");
                    Console.WriteLine($"🔧 Total Methods: {codeAnalysis.TotalMethods:N0}");
                    Console.WriteLine($"📋 Total Properties: {codeAnalysis.TotalProperties:N0}");
                    Console.WriteLine($"🎯 Total Interfaces: {codeAnalysis.TotalInterfaces:N0}");
                    Console.WriteLine($"📝 Total Lines of Code: {codeAnalysis.TotalLinesOfCode:N0}");
                    Console.WriteLine($"🔍 Total Symbols: {codeAnalysis.Symbols.Count:N0}");
                    Console.WriteLine($"⏱️ Analysis Time: {codeStopwatch.Elapsed.TotalSeconds:F2} seconds");
                    Console.WriteLine();

                    // Language breakdown
                    if (codeAnalysis.LanguageAnalysis.Any())
                    {
                        Console.WriteLine("🌐 **LANGUAGE BREAKDOWN**");
                        Console.WriteLine("========================");
                        foreach (var (language, analysis) in codeAnalysis.LanguageAnalysis.OrderByDescending(x => x.Value.Symbols.Count))
                        {
                            Console.WriteLine($"{language.PadRight(12)} {analysis.FileCount,3} files, {analysis.Symbols.Count,6} symbols, {analysis.LineCount,8:N0} lines");
                        }
                        Console.WriteLine();
                    }

                    // Top classes by member count
                    var topClasses = codeAnalysis.Symbols
                        .Where(s => s.Type == SymbolType.Class)
                        .OrderByDescending(s => s.Members.Count)
                        .Take(5)
                        .ToList();

                    if (topClasses.Any())
                    {
                        Console.WriteLine("🏆 **LARGEST CLASSES**");
                        Console.WriteLine("=====================");
                        foreach (var cls in topClasses)
                        {
                            var memberCount = codeAnalysis.Symbols.Count(s => s.FullName.StartsWith(cls.FullName + "."));
                            Console.WriteLine($"{memberCount.ToString().PadRight(3)} members  {cls.Name} ({Path.GetFileName(cls.SourceFile)})");
                        }
                        Console.WriteLine();
                    }

                    // Complexity metrics
                    if (codeAnalysis.Complexity != null)
                    {
                        Console.WriteLine("📈 **COMPLEXITY METRICS**");
                        Console.WriteLine("=========================");
                        Console.WriteLine($"🔄 Cyclomatic Complexity: {codeAnalysis.Complexity.CyclomaticComplexity:F2}");
                        Console.WriteLine($"🧠 Cognitive Complexity: {codeAnalysis.Complexity.CognitiveComplexity:F2}");
                        Console.WriteLine($"📊 Halstead Metric: {codeAnalysis.Complexity.Halstead}");
                        Console.WriteLine($"🏗️ Nesting Depth: {codeAnalysis.Complexity.NestingDepth}");
                        Console.WriteLine();
                    }

                    // Quality metrics
                    if (codeAnalysis.Quality != null)
                    {
                        Console.WriteLine("✨ **QUALITY METRICS**");
                        Console.WriteLine("======================");
                        Console.WriteLine($"🔧 Maintainability: {codeAnalysis.Quality.Maintainability:F1}%");
                        Console.WriteLine($"🧪 Testability: {codeAnalysis.Quality.Testability:F1}%");
                        Console.WriteLine($"📖 Readability: {codeAnalysis.Quality.Readability:F1}%");
                        Console.WriteLine($"📚 Documentation: {codeAnalysis.Quality.Documentation:F1}%");
                        Console.WriteLine();
                    }
                }

                // **PHASE 3: DEPENDENCY ANALYSIS**
                if (codeAnalysis?.Symbols.Any() == true)
                {
                    Console.WriteLine("🔗 **PHASE 3: DEPENDENCY ANALYSIS**");
                    Console.WriteLine("===================================");

                    var dependencyOptions = new DependencyOptions
                    {
                        ResolveStaticDependencies = true,
                        ResolveDynamicDependencies = true,
                        ResolveExternalDependencies = true,
                        ResolveDatabaseDependencies = false, // Would need FileSystemAnalysis
                        DetectCircularDependencies = true,
                        ExternalSources = new List<string> { "NuGet", "COM", "GAC" }
                    };

                    var dependencyStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var dependencyAnalysis = await dependencyResolver.ResolveAsync(codeAnalysis, dependencyOptions);
                    dependencyStopwatch.Stop();

                    Console.WriteLine("📊 **DEPENDENCY ANALYSIS RESULTS**");
                    Console.WriteLine("==================================");
                    Console.WriteLine($"🔗 Static Dependencies: {dependencyAnalysis.StaticDependencies.Count:N0}");
                    Console.WriteLine($"⚡ Dynamic Dependencies: {dependencyAnalysis.DynamicDependencies.Count:N0}");
                    Console.WriteLine($"📦 External Dependencies: {dependencyAnalysis.ExternalDependencies.Count:N0}");
                    Console.WriteLine($"🔄 Circular Dependencies: {dependencyAnalysis.CircularDependencies.Count:N0}");
                    Console.WriteLine($"📈 Graph Nodes: {dependencyAnalysis.DependencyGraph.Nodes.Count:N0}");
                    Console.WriteLine($"🔀 Graph Edges: {dependencyAnalysis.DependencyGraph.Edges.Count:N0}");
                    Console.WriteLine($"⏱️ Analysis Time: {dependencyStopwatch.Elapsed.TotalSeconds:F2} seconds");
                    Console.WriteLine();

                    // Dependency type breakdown
                    if (dependencyAnalysis.StaticDependencies.Any())
                    {
                        Console.WriteLine("📋 **DEPENDENCY TYPES**");
                        Console.WriteLine("=======================");
                        var dependencyTypes = dependencyAnalysis.StaticDependencies
                            .GroupBy(d => d.Type)
                            .OrderByDescending(g => g.Count())
                            .ToList();

                        foreach (var group in dependencyTypes)
                        {
                            Console.WriteLine($"{group.Key.ToString().PadRight(15)} {group.Count(),6:N0} dependencies");
                        }
                        Console.WriteLine();
                    }

                    // External dependencies
                    if (dependencyAnalysis.ExternalDependencies.Any())
                    {
                        Console.WriteLine("📦 **EXTERNAL PACKAGES**");
                        Console.WriteLine("========================");
                        foreach (var extDep in dependencyAnalysis.ExternalDependencies.Take(10))
                        {
                            Console.WriteLine($"{extDep.Name.PadRight(25)} {extDep.Version.PadRight(10)} ({extDep.Source})");
                        }
                        if (dependencyAnalysis.ExternalDependencies.Count > 10)
                        {
                            Console.WriteLine($"... and {dependencyAnalysis.ExternalDependencies.Count - 10:N0} more packages");
                        }
                        Console.WriteLine();
                    }

                    // Circular dependencies warning
                    if (dependencyAnalysis.CircularDependencies.Any())
                    {
                        Console.WriteLine("⚠️ **CIRCULAR DEPENDENCIES DETECTED**");
                        Console.WriteLine("=====================================");
                        foreach (var circular in dependencyAnalysis.CircularDependencies.Take(5))
                        {
                            Console.WriteLine($"🔄 {string.Join(" → ", circular.Cycle)}");
                        }
                        if (dependencyAnalysis.CircularDependencies.Count > 5)
                        {
                            Console.WriteLine($"... and {dependencyAnalysis.CircularDependencies.Count - 5:N0} more cycles");
                        }
                        Console.WriteLine();
                    }

                    // Graph statistics
                    Console.WriteLine("📊 **DEPENDENCY GRAPH METRICS**");
                    Console.WriteLine("===============================");
                    var avgEdgesPerNode = dependencyAnalysis.DependencyGraph.Nodes.Count > 0 
                        ? (double)dependencyAnalysis.DependencyGraph.Edges.Count / dependencyAnalysis.DependencyGraph.Nodes.Count 
                        : 0;
                    Console.WriteLine($"📈 Average Dependencies per Component: {avgEdgesPerNode:F1}");
                    Console.WriteLine($"🔗 Graph Density: {CalculateGraphDensity(dependencyAnalysis.DependencyGraph):F3}");
                    Console.WriteLine($"🌐 Graph Complexity: {(dependencyAnalysis.DependencyGraph.Edges.Count > 100 ? "High" : dependencyAnalysis.DependencyGraph.Edges.Count > 50 ? "Medium" : "Low")}");
                    Console.WriteLine();

                    // **PHASE 4: ARCHITECTURE ANALYSIS**
                    Console.WriteLine("🏗️ **PHASE 4: ARCHITECTURE ANALYSIS**");
                    Console.WriteLine("====================================");

                    var architectureOptions = new ArchitectureOptions
                    {
                        DetectPatterns = true,
                        DetectLayers = true,
                        DetectComponents = true,
                        DetectDesignPatterns = true,
                        DetectViolations = true,
                        LayerConventions = new List<string>()
                    };

                    var architectureStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var architectureAnalysis = await architectureAnalyzer.AnalyzeAsync(codeAnalysis, dependencyAnalysis, architectureOptions);
                    architectureStopwatch.Stop();

                    Console.WriteLine("📊 **ARCHITECTURE ANALYSIS RESULTS**");
                    Console.WriteLine("====================================");
                    Console.WriteLine($"🏛️ Architectural Pattern: {architectureAnalysis.DetectedPattern}");
                    Console.WriteLine($"🏗️ Architectural Layers: {architectureAnalysis.Layers.Count:N0}");
                    Console.WriteLine($"📦 Components: {architectureAnalysis.Components.Count:N0}");
                    Console.WriteLine($"🎨 Design Patterns: {architectureAnalysis.DesignPatterns.Count:N0}");
                    Console.WriteLine($"⚠️ Violations: {architectureAnalysis.Violations.Count:N0}");
                    Console.WriteLine($"📚 Modules: {architectureAnalysis.Modules.Count:N0}");
                    Console.WriteLine($"⏱️ Analysis Time: {architectureStopwatch.Elapsed.TotalSeconds:F2} seconds");
                    Console.WriteLine();

                    // Architectural layers
                    if (architectureAnalysis.Layers.Any())
                    {
                        Console.WriteLine("🏗️ **ARCHITECTURAL LAYERS**");
                        Console.WriteLine("===========================");
                        foreach (var layer in architectureAnalysis.Layers.OrderBy(l => l.Level))
                        {
                            Console.WriteLine($"Layer {layer.Level}: {layer.Name.PadRight(15)} {layer.Components.Count,3} components");
                        }
                        Console.WriteLine();
                    }

                    // Components breakdown
                    if (architectureAnalysis.Components.Any())
                    {
                        Console.WriteLine("📦 **COMPONENT ANALYSIS**");
                        Console.WriteLine("=========================");
                        var componentTypes = architectureAnalysis.Components
                            .GroupBy(c => c.Type)
                            .OrderByDescending(g => g.Count())
                            .ToList();

                        foreach (var group in componentTypes)
                        {
                            Console.WriteLine($"{group.Key.ToString().PadRight(15)} {group.Count(),3} components");
                        }
                        Console.WriteLine();
                    }

                    // Design patterns found
                    if (architectureAnalysis.DesignPatterns.Any())
                    {
                        Console.WriteLine("🎨 **DESIGN PATTERNS DETECTED**");
                        Console.WriteLine("===============================");
                        foreach (var pattern in architectureAnalysis.DesignPatterns.Take(5))
                        {
                            Console.WriteLine($"{pattern.Name.PadRight(12)} {pattern.Confidence,5:P1} confidence, {pattern.ParticipatingClasses.Count} classes");
                        }
                        if (architectureAnalysis.DesignPatterns.Count > 5)
                        {
                            Console.WriteLine($"... and {architectureAnalysis.DesignPatterns.Count - 5:N0} more patterns");
                        }
                        Console.WriteLine();
                    }

                    // Architectural violations
                    if (architectureAnalysis.Violations.Any())
                    {
                        Console.WriteLine("⚠️ **ARCHITECTURAL VIOLATIONS**");
                        Console.WriteLine("===============================");
                        var violationsByType = architectureAnalysis.Violations
                            .GroupBy(v => v.Type)
                            .OrderByDescending(g => g.Count())
                            .ToList();

                        foreach (var group in violationsByType)
                        {
                            Console.WriteLine($"{group.Key.ToString().PadRight(15)} {group.Count(),3} violations");
                        }

                        var highSeverityViolations = architectureAnalysis.Violations
                            .Where(v => v.Severity == ViolationSeverity.High || v.Severity == ViolationSeverity.Critical)
                            .Take(3)
                            .ToList();

                        if (highSeverityViolations.Any())
                        {
                            Console.WriteLine("\n🚨 **HIGH PRIORITY VIOLATIONS**:");
                            foreach (var violation in highSeverityViolations)
                            {
                                Console.WriteLine($"   • {violation.Description}");
                            }
                        }
                        Console.WriteLine();
                    }

                    // Quality metrics
                    Console.WriteLine("📈 **ARCHITECTURE QUALITY METRICS**");
                    Console.WriteLine("===================================");
                    Console.WriteLine($"🔗 Cohesion (LCOM): {architectureAnalysis.Cohesion.LCOM:F3} (lower is better)");
                    Console.WriteLine($"🎯 Cohesion (TCC): {architectureAnalysis.Cohesion.TCC:F3} (higher is better)");
                    Console.WriteLine($"📥 Afferent Coupling: {architectureAnalysis.Coupling.AfferentCoupling:F1} (incoming dependencies)");
                    Console.WriteLine($"📤 Efferent Coupling: {architectureAnalysis.Coupling.EfferentCoupling:F1} (outgoing dependencies)");
                    Console.WriteLine($"⚖️ Instability: {architectureAnalysis.Coupling.Instability:F3} (0=stable, 1=unstable)");
                    Console.WriteLine();

                    // **PHASE 5: RELATIONSHIP MAPPING**
                    Console.WriteLine("🗺️ **PHASE 5: RELATIONSHIP MAPPING**");
                    Console.WriteLine("===================================");

                    var relationshipOptions = new Models.RelationshipOptions
                    {
                        BuildRelationshipMatrix = true,
                        BuildComponentRelationships = true,
                        BuildLayerRelationships = true,
                        BuildDependencyMatrix = true,
                        BuildCallHierarchy = true,
                        BuildInheritanceTree = true
                    };

                    var relationshipStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var concreteMapper = (RelationshipMapper)relationshipMapper;
                    var relationshipMapping = await concreteMapper.BuildMappingAsync(codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipOptions);
                    relationshipStopwatch.Stop();

                    Console.WriteLine("📊 **RELATIONSHIP MAPPING RESULTS**");
                    Console.WriteLine("===================================");
                    Console.WriteLine($"🗺️ Relationship Matrix: {relationshipMapping.RelationshipMatrix?.Relationships.Count ?? 0:N0} relationships");
                    Console.WriteLine($"🔗 Component Relationships: {relationshipMapping.ComponentRelationships?.Count ?? 0:N0}");
                    Console.WriteLine($"🏗️ Layer Relationships: {relationshipMapping.LayerRelationships?.Count ?? 0:N0}");
                    Console.WriteLine($"📈 Dependency Matrix: {relationshipMapping.DependencyMatrix?.Entries.Count ?? 0:N0} entries");
                    Console.WriteLine($"📞 Call Hierarchy: {relationshipMapping.CallHierarchy?.Nodes.Count ?? 0:N0} methods");
                    Console.WriteLine($"🌳 Inheritance Tree: {relationshipMapping.InheritanceTree?.Nodes.Count ?? 0:N0} classes");
                    Console.WriteLine($"⏱️ Mapping Time: {relationshipStopwatch.Elapsed.TotalSeconds:F2} seconds");
                    Console.WriteLine();

                    // Relationship statistics
                    if (relationshipMapping.Statistics != null)
                    {
                        Console.WriteLine("📊 **RELATIONSHIP STATISTICS**");
                        Console.WriteLine("==============================");
                        Console.WriteLine($"📈 Total Relationships: {relationshipMapping.Statistics.TotalRelationships:N0}");
                        Console.WriteLine($"🔗 Average Relationship Strength: {relationshipMapping.Statistics.AverageRelationshipStrength:F3}");
                        Console.WriteLine($"🏗️ Component Relationships: {relationshipMapping.Statistics.ComponentRelationshipCount:N0}");
                        Console.WriteLine($"📚 Layer Relationships: {relationshipMapping.Statistics.LayerRelationshipCount:N0}");
                        Console.WriteLine($"⚠️ Layer Violations: {relationshipMapping.Statistics.LayerViolationCount:N0}");
                        Console.WriteLine($"📞 Total Methods: {relationshipMapping.Statistics.TotalMethods:N0}");
                        Console.WriteLine($"🌳 Total Classes: {relationshipMapping.Statistics.TotalClasses:N0}");
                        Console.WriteLine();
                    }

                    // **PHASE 6: VISUALIZATION GENERATION**
                    Console.WriteLine("🎨 **PHASE 6: VISUALIZATION GENERATION**");
                    Console.WriteLine("========================================");

                    var visualizationOptions = new VisualizationOptions
                    {
                        GenerateMermaidDiagrams = true,
                        GenerateD3Visualizations = true,
                        GenerateCytoscapeVisualizations = true,
                        ExportDataFiles = true,
                        GenerateHtmlReports = true,
                        OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "visualizations")
                    };

                    var visualizationStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var concreteGenerator = (VisualizationGenerator)visualizationGenerator;
                    var visualizationPackage = await concreteGenerator.GenerateVisualizationAsync(
                        codeAnalysis, dependencyAnalysis, architectureAnalysis, relationshipMapping, visualizationOptions);
                    visualizationStopwatch.Stop();

                    Console.WriteLine("📊 **VISUALIZATION GENERATION RESULTS**");
                    Console.WriteLine("=======================================");
                    Console.WriteLine($"🎯 Mermaid Diagrams: {visualizationPackage.MermaidDiagrams?.Count ?? 0:N0}");
                    Console.WriteLine($"🌐 D3.js Visualizations: {visualizationPackage.D3Visualizations?.Count ?? 0:N0}");
                    Console.WriteLine($"🔗 Cytoscape Visualizations: {visualizationPackage.CytoscapeVisualizations?.Count ?? 0:N0}");
                    Console.WriteLine($"📄 HTML Reports: {visualizationPackage.HtmlReports?.Count ?? 0:N0}");
                    Console.WriteLine($"💾 Data Exports: {visualizationPackage.DataExports?.Count ?? 0:N0}");
                    Console.WriteLine($"⏱️ Generation Time: {visualizationStopwatch.Elapsed.TotalSeconds:F2} seconds");
                    Console.WriteLine();

                    // Save visualization package
                    Console.WriteLine("💾 **SAVING VISUALIZATION PACKAGE**");
                    Console.WriteLine("===================================");
                    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "visualizations", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                    await concreteGenerator.SaveVisualizationPackageAsync(visualizationPackage, outputDir);
                    Console.WriteLine($"📁 Saved to: {outputDir}");
                    Console.WriteLine($"🌐 Open: {Path.Combine(outputDir, "index.html")}");
                    Console.WriteLine();
                }

                Console.WriteLine("✅ **ANALYSIS COMPLETE**");
                Console.WriteLine("========================");
                Console.WriteLine($"The ALARM Mapping Function successfully analyzed {fileSystemAnalysis.TotalFiles:N0} files");
                Console.WriteLine($"across {fileSystemAnalysis.TotalDirectories:N0} directories in {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
                Console.WriteLine();
                Console.WriteLine("🚀 **NEXT STEPS**");
                Console.WriteLine("=================");
                Console.WriteLine("1. ✅ File System Analysis - COMPLETE");
                Console.WriteLine("2. ✅ Code Analysis Engine - COMPLETE (with AutoLISP support)");
                Console.WriteLine("3. ✅ Dependency Resolution - COMPLETE");
                Console.WriteLine("4. ✅ Architecture Analysis - COMPLETE");
                Console.WriteLine("5. ✅ Relationship Mapping - COMPLETE");
                Console.WriteLine("6. ✅ Visualization Generation - COMPLETE");
                Console.WriteLine();
                Console.WriteLine("This demonstrates Phase 1-6 (Complete ALARM Universal Mapping Function) of the ALARM system.");
                Console.WriteLine("The system now provides comprehensive application reverse engineering with full visualization capabilities.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Analysis failed");
                Console.WriteLine($"❌ Error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            double number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:N1} {suffixes[counter]}";
        }

        private static double CalculateGraphDensity(DependencyGraph graph)
        {
            if (graph.Nodes.Count < 2) return 0.0;
            
            var maxPossibleEdges = graph.Nodes.Count * (graph.Nodes.Count - 1);
            return maxPossibleEdges > 0 ? (double)graph.Edges.Count / maxPossibleEdges : 0.0;
        }
    }
}
