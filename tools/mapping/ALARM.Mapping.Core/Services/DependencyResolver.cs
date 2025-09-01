using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// Comprehensive dependency resolution engine for static and dynamic dependency analysis
    /// </summary>
    public class DependencyResolver : IDependencyResolver
    {
        private readonly ILogger<DependencyResolver> _logger;

        public DependencyResolver(ILogger<DependencyResolver> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Resolves all dependencies for the given code analysis
        /// </summary>
        public async Task<DependencyAnalysis> ResolveAsync(
            CodeAnalysis codeAnalysis, 
            DependencyOptions options, 
            CancellationToken cancellationToken = default)
        {
            if (codeAnalysis == null)
                throw new ArgumentNullException(nameof(codeAnalysis));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _logger.LogInformation("Starting dependency resolution for {TotalSymbols} symbols", codeAnalysis.Symbols.Count);

            var analysis = new DependencyAnalysis();

            try
            {
                // Resolve static dependencies
                if (options.ResolveStaticDependencies)
                {
                    _logger.LogInformation("Resolving static dependencies");
                    analysis.StaticDependencies = await ResolveStaticDependenciesAsync(codeAnalysis, cancellationToken);
                    _logger.LogInformation("Found {StaticDependencies} static dependencies", analysis.StaticDependencies.Count);
                }

                // Resolve dynamic dependencies
                if (options.ResolveDynamicDependencies)
                {
                    _logger.LogInformation("Resolving dynamic dependencies");
                    analysis.DynamicDependencies = await ResolveDynamicDependenciesAsync(codeAnalysis, cancellationToken);
                    _logger.LogInformation("Found {DynamicDependencies} dynamic dependencies", analysis.DynamicDependencies.Count);
                }

                // Resolve external dependencies
                if (options.ResolveExternalDependencies)
                {
                    _logger.LogInformation("Resolving external dependencies");
                    // Note: This would typically require FileSystemAnalysis, but we'll work with what we have
                    analysis.ExternalDependencies = await ResolveExternalDependenciesFromSymbolsAsync(codeAnalysis, cancellationToken);
                    _logger.LogInformation("Found {ExternalDependencies} external dependencies", analysis.ExternalDependencies.Count);
                }

                // Build dependency graph
                analysis.DependencyGraph = await BuildDependencyGraphAsync(analysis, cancellationToken);
                _logger.LogInformation("Built dependency graph with {Nodes} nodes and {Edges} edges", 
                    analysis.DependencyGraph.Nodes.Count, analysis.DependencyGraph.Edges.Count);

                // Detect circular dependencies
                if (options.DetectCircularDependencies)
                {
                    _logger.LogInformation("Detecting circular dependencies");
                    analysis.CircularDependencies = await DetectCircularDependenciesAsync(analysis.DependencyGraph, cancellationToken);
                    _logger.LogInformation("Found {CircularDependencies} circular dependencies", analysis.CircularDependencies.Count);
                }

                _logger.LogInformation("Dependency resolution complete. Total: {StaticCount} static, {DynamicCount} dynamic, {ExternalCount} external",
                    analysis.StaticDependencies.Count, analysis.DynamicDependencies.Count, analysis.ExternalDependencies.Count);

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dependency resolution failed");
                throw;
            }
        }

        /// <summary>
        /// Resolves static dependencies from using statements and imports
        /// </summary>
        public async Task<List<StaticDependency>> ResolveStaticDependenciesAsync(
            CodeAnalysis codeAnalysis, 
            CancellationToken cancellationToken = default)
        {
            var dependencies = new List<StaticDependency>();

            await Task.Run(() =>
            {
                // Group symbols by source file for efficient processing
                var symbolsByFile = codeAnalysis.Symbols.GroupBy(s => s.SourceFile).ToList();

                foreach (var fileGroup in symbolsByFile)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var filePath = fileGroup.Key;
                    var fileSymbols = fileGroup.ToList();

                    // Extract using statements and imports from file
                    var usingDependencies = ExtractUsingDependencies(filePath, fileSymbols);
                    dependencies.AddRange(usingDependencies);

                    // Extract inheritance relationships
                    var inheritanceDependencies = ExtractInheritanceDependencies(fileSymbols);
                    dependencies.AddRange(inheritanceDependencies);

                    // Extract method call dependencies
                    var methodCallDependencies = ExtractMethodCallDependencies(fileSymbols);
                    dependencies.AddRange(methodCallDependencies);

                    // Extract property access dependencies
                    var propertyDependencies = ExtractPropertyDependencies(fileSymbols);
                    dependencies.AddRange(propertyDependencies);
                }
            }, cancellationToken);

            return dependencies.Distinct().ToList();
        }

        /// <summary>
        /// Resolves dynamic dependencies from reflection and late binding
        /// </summary>
        public async Task<List<DynamicDependency>> ResolveDynamicDependenciesAsync(
            CodeAnalysis codeAnalysis, 
            CancellationToken cancellationToken = default)
        {
            var dependencies = new List<DynamicDependency>();

            await Task.Run(() =>
            {
                // Look for reflection patterns in method bodies
                var reflectionPatterns = new Dictionary<string, DependencyType>
                {
                    [@"Type\.GetType\(""([^""]+)"""] = DependencyType.Using,
                    [@"Assembly\.LoadFrom\(""([^""]+)"""] = DependencyType.Using,
                    [@"Activator\.CreateInstance\(typeof\(([^)]+)\)"] = DependencyType.Using,
                    [@"GetMethod\(""([^""]+)"""] = DependencyType.MethodCall,
                    [@"GetProperty\(""([^""]+)"""] = DependencyType.PropertyAccess
                };

                foreach (var symbol in codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Method))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // This would typically require reading the method body from source
                    // For now, we'll create placeholder dynamic dependencies based on symbol metadata
                    if (symbol.Metadata.ContainsKey("ReflectionTarget"))
                    {
                        dependencies.Add(new DynamicDependency
                        {
                            From = symbol.FullName,
                            To = symbol.Metadata["ReflectionTarget"].ToString() ?? string.Empty,
                            Type = DependencyType.Using,
                            SourceFile = symbol.SourceFile,
                            LineNumber = symbol.LineNumber,
                            ReflectionTarget = symbol.Metadata["ReflectionTarget"].ToString() ?? string.Empty,
                            IsConditional = true
                        });
                    }
                }
            }, cancellationToken);

            return dependencies;
        }

        /// <summary>
        /// Resolves external dependencies from symbols (approximation without FileSystemAnalysis)
        /// </summary>
        public async Task<List<ExternalDependency>> ResolveExternalDependenciesFromSymbolsAsync(
            CodeAnalysis codeAnalysis, 
            CancellationToken cancellationToken = default)
        {
            var dependencies = new List<ExternalDependency>();

            await Task.Run(() =>
            {
                // Extract common external dependencies from namespaces
                var externalNamespaces = new Dictionary<string, string>
                {
                    ["System"] = "System",
                    ["Microsoft"] = "Microsoft",
                    ["Newtonsoft"] = "Newtonsoft.Json",
                    ["AutoCAD"] = "AutoCAD.NET",
                    ["Oracle"] = "Oracle.DataAccess"
                };

                var foundNamespaces = codeAnalysis.Symbols
                    .Where(s => !string.IsNullOrEmpty(s.Namespace))
                    .Select(s => s.Namespace.Split('.')[0])
                    .Distinct()
                    .Where(ns => externalNamespaces.ContainsKey(ns))
                    .ToList();

                foreach (var ns in foundNamespaces)
                {
                    if (externalNamespaces.TryGetValue(ns, out var packageName))
                    {
                        dependencies.Add(new ExternalDependency
                        {
                            Name = packageName,
                            Version = "Unknown",
                            Source = "NuGet",
                            ReferencedBy = codeAnalysis.Symbols
                                .Where(s => s.Namespace?.StartsWith(ns) == true)
                                .Select(s => s.SourceFile)
                                .Distinct()
                                .ToList()
                        });
                    }
                }
            }, cancellationToken);

            return dependencies;
        }

        /// <summary>
        /// Resolves external dependencies from file system analysis
        /// </summary>
        public async Task<List<ExternalDependency>> ResolveExternalDependenciesAsync(
            FileSystemAnalysis fileSystem, 
            CancellationToken cancellationToken = default)
        {
            var dependencies = new List<ExternalDependency>();

            await Task.Run(() =>
            {
                // Look for project files and package references
                var projectFiles = fileSystem.ConfigurationFiles
                    .Where(f => f.Extension.ToLowerInvariant() == ".csproj")
                    .ToList();

                foreach (var projectFile in projectFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        // Parse project file for package references
                        var projectDependencies = ParseProjectFile(projectFile.FullPath);
                        dependencies.AddRange(projectDependencies);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to parse project file {ProjectFile}", projectFile.FullPath);
                    }
                }

                // Look for packages.config files
                var packageConfigFiles = fileSystem.ConfigurationFiles
                    .Where(f => f.FileName.ToLowerInvariant() == "packages.config")
                    .ToList();

                foreach (var packageFile in packageConfigFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var packageDependencies = ParsePackagesConfig(packageFile.FullPath);
                        dependencies.AddRange(packageDependencies);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to parse packages.config {PackageFile}", packageFile.FullPath);
                    }
                }
            }, cancellationToken);

            return dependencies;
        }

        /// <summary>
        /// Resolves database dependencies from connection strings and SQL references
        /// </summary>
        public async Task<List<DatabaseDependency>> ResolveDatabaseDependenciesAsync(
            FileSystemAnalysis fileSystem, 
            CancellationToken cancellationToken = default)
        {
            var dependencies = new List<DatabaseDependency>();

            await Task.Run(() =>
            {
                // Look for connection strings in configuration files
                var configFiles = fileSystem.ConfigurationFiles
                    .Where(f => f.Extension.ToLowerInvariant() == ".config" || 
                               f.FileName.ToLowerInvariant().Contains("appsettings"))
                    .ToList();

                foreach (var configFile in configFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var dbDependencies = ExtractDatabaseDependencies(configFile.FullPath);
                        dependencies.AddRange(dbDependencies);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to extract database dependencies from {ConfigFile}", configFile.FullPath);
                    }
                }

                // Look for SQL files
                var sqlFiles = fileSystem.SourceFiles
                    .Where(f => f.Extension.ToLowerInvariant() == ".sql")
                    .ToList();

                foreach (var sqlFile in sqlFiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var sqlDependencies = ExtractSqlDependencies(sqlFile.FullPath);
                        dependencies.AddRange(sqlDependencies);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to extract SQL dependencies from {SqlFile}", sqlFile.FullPath);
                    }
                }
            }, cancellationToken);

            return dependencies;
        }

        /// <summary>
        /// Detects circular dependencies in the dependency graph
        /// </summary>
        public async Task<List<CircularDependency>> DetectCircularDependenciesAsync(
            DependencyGraph graph, 
            CancellationToken cancellationToken = default)
        {
            var circularDependencies = new List<CircularDependency>();

            await Task.Run(() =>
            {
                var visited = new HashSet<string>();
                var recursionStack = new HashSet<string>();

                foreach (var node in graph.Nodes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!visited.Contains(node.Id))
                    {
                        var cycles = DetectCyclesFromNode(node.Id, graph, visited, recursionStack, new List<string>());
                        circularDependencies.AddRange(cycles);
                    }
                }
            }, cancellationToken);

            return circularDependencies.Distinct().ToList();
        }

        #region Private Methods

        private List<StaticDependency> ExtractUsingDependencies(string filePath, List<CodeSymbol> fileSymbols)
        {
            var dependencies = new List<StaticDependency>();

            // Extract namespace dependencies from symbols
            var namespaces = fileSymbols
                .Where(s => !string.IsNullOrEmpty(s.Namespace))
                .Select(s => s.Namespace)
                .Distinct()
                .ToList();

            foreach (var ns in namespaces)
            {
                dependencies.Add(new StaticDependency
                {
                    From = filePath,
                    To = ns,
                    Type = DependencyType.Using,
                    SourceFile = filePath,
                    LineNumber = 1
                });
            }

            return dependencies;
        }

        private List<StaticDependency> ExtractInheritanceDependencies(List<CodeSymbol> fileSymbols)
        {
            var dependencies = new List<StaticDependency>();

            foreach (var symbol in fileSymbols.Where(s => s.Type == SymbolType.Class))
            {
                if (symbol.Metadata.ContainsKey("BaseTypes"))
                {
                    var baseTypes = symbol.Metadata["BaseTypes"] as List<string>;
                    if (baseTypes != null)
                    {
                        foreach (var baseType in baseTypes)
                        {
                            dependencies.Add(new StaticDependency
                            {
                                From = symbol.FullName,
                                To = baseType,
                                Type = DependencyType.Inheritance,
                                SourceFile = symbol.SourceFile,
                                LineNumber = symbol.LineNumber
                            });
                        }
                    }
                }
            }

            return dependencies;
        }

        private List<StaticDependency> ExtractMethodCallDependencies(List<CodeSymbol> fileSymbols)
        {
            var dependencies = new List<StaticDependency>();

            // This would typically require analyzing method bodies
            // For now, we'll create dependencies based on symbol relationships
            var classes = fileSymbols.Where(s => s.Type == SymbolType.Class).ToList();
            var methods = fileSymbols.Where(s => s.Type == SymbolType.Method).ToList();

            foreach (var method in methods)
            {
                var containingClass = classes.FirstOrDefault(c => method.FullName.StartsWith(c.FullName + "."));
                if (containingClass != null)
                {
                    dependencies.Add(new StaticDependency
                    {
                        From = containingClass.FullName,
                        To = method.FullName,
                        Type = DependencyType.MethodCall,
                        SourceFile = method.SourceFile,
                        LineNumber = method.LineNumber
                    });
                }
            }

            return dependencies;
        }

        private List<StaticDependency> ExtractPropertyDependencies(List<CodeSymbol> fileSymbols)
        {
            var dependencies = new List<StaticDependency>();

            var classes = fileSymbols.Where(s => s.Type == SymbolType.Class).ToList();
            var properties = fileSymbols.Where(s => s.Type == SymbolType.Property).ToList();

            foreach (var property in properties)
            {
                var containingClass = classes.FirstOrDefault(c => property.FullName.StartsWith(c.FullName + "."));
                if (containingClass != null)
                {
                    dependencies.Add(new StaticDependency
                    {
                        From = containingClass.FullName,
                        To = property.FullName,
                        Type = DependencyType.PropertyAccess,
                        SourceFile = property.SourceFile,
                        LineNumber = property.LineNumber
                    });
                }
            }

            return dependencies;
        }

        private async Task<DependencyGraph> BuildDependencyGraphAsync(DependencyAnalysis analysis, CancellationToken cancellationToken)
        {
            var graph = new DependencyGraph();

            await Task.Run(() =>
            {
                var nodeIds = new HashSet<string>();

                // Add nodes from all dependencies
                foreach (var dep in analysis.StaticDependencies)
                {
                    nodeIds.Add(dep.From);
                    nodeIds.Add(dep.To);
                }

                foreach (var dep in analysis.DynamicDependencies)
                {
                    nodeIds.Add(dep.From);
                    nodeIds.Add(dep.To);
                }

                // Create nodes
                foreach (var nodeId in nodeIds)
                {
                    graph.Nodes.Add(new GraphNode
                    {
                        Id = nodeId,
                        Label = nodeId.Split('.').LastOrDefault() ?? nodeId,
                        Type = DetermineNodeType(nodeId),
                        Attributes = new Dictionary<string, object>()
                    });
                }

                // Create edges from static dependencies
                foreach (var dep in analysis.StaticDependencies)
                {
                    graph.Edges.Add(new GraphEdge
                    {
                        From = dep.From,
                        To = dep.To,
                        Type = MapDependencyTypeToEdgeType(dep.Type),
                        Attributes = new Dictionary<string, object>
                        {
                            ["SourceFile"] = dep.SourceFile,
                            ["LineNumber"] = dep.LineNumber
                        }
                    });
                }

                // Create edges from dynamic dependencies
                foreach (var dep in analysis.DynamicDependencies)
                {
                    graph.Edges.Add(new GraphEdge
                    {
                        From = dep.From,
                        To = dep.To,
                        Type = EdgeType.Dependency,
                        Attributes = new Dictionary<string, object>
                        {
                            ["SourceFile"] = dep.SourceFile,
                            ["LineNumber"] = dep.LineNumber,
                            ["IsDynamic"] = true,
                            ["IsConditional"] = dep.IsConditional
                        }
                    });
                }
            }, cancellationToken);

            return graph;
        }

        private NodeType DetermineNodeType(string nodeId)
        {
            if (nodeId.Contains(".") && char.IsUpper(nodeId.Split('.').LastOrDefault()?[0] ?? ' '))
                return NodeType.Class;
            if (nodeId.Contains("()"))
                return NodeType.Method;
            if (nodeId.StartsWith("System.") || nodeId.StartsWith("Microsoft."))
                return NodeType.Assembly;
            
            return NodeType.Unknown;
        }

        private EdgeType MapDependencyTypeToEdgeType(DependencyType dependencyType)
        {
            return dependencyType switch
            {
                DependencyType.Inheritance => EdgeType.Inheritance,
                DependencyType.MethodCall => EdgeType.MethodCall,
                DependencyType.PropertyAccess => EdgeType.Association,
                DependencyType.Using => EdgeType.Dependency,
                _ => EdgeType.Unknown
            };
        }

        private List<CircularDependency> DetectCyclesFromNode(
            string nodeId, 
            DependencyGraph graph, 
            HashSet<string> visited, 
            HashSet<string> recursionStack, 
            List<string> currentPath)
        {
            var cycles = new List<CircularDependency>();

            visited.Add(nodeId);
            recursionStack.Add(nodeId);
            currentPath.Add(nodeId);

            var outgoingEdges = graph.Edges.Where(e => e.From == nodeId).ToList();

            foreach (var edge in outgoingEdges)
            {
                if (!visited.Contains(edge.To))
                {
                    var childCycles = DetectCyclesFromNode(edge.To, graph, visited, recursionStack, new List<string>(currentPath));
                    cycles.AddRange(childCycles);
                }
                else if (recursionStack.Contains(edge.To))
                {
                    // Found a cycle
                    var cycleStartIndex = currentPath.IndexOf(edge.To);
                    var cycle = currentPath.Skip(cycleStartIndex).Concat(new[] { edge.To }).ToList();

                    cycles.Add(new CircularDependency
                    {
                        Cycle = cycle,
                        Type = DependencyType.Unknown
                    });
                }
            }

            recursionStack.Remove(nodeId);
            return cycles;
        }

        private List<ExternalDependency> ParseProjectFile(string projectFilePath)
        {
            var dependencies = new List<ExternalDependency>();

            try
            {
                var doc = new XmlDocument();
                doc.Load(projectFilePath);

                var packageReferences = doc.SelectNodes("//PackageReference");
                if (packageReferences != null)
                {
                    foreach (XmlNode packageRef in packageReferences)
                    {
                        var include = packageRef.Attributes?["Include"]?.Value;
                        var version = packageRef.Attributes?["Version"]?.Value;

                        if (!string.IsNullOrEmpty(include))
                        {
                            dependencies.Add(new ExternalDependency
                            {
                                Name = include,
                                Version = version ?? "Unknown",
                                Source = "NuGet",
                                ReferencedBy = new List<string> { projectFilePath }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse project file {ProjectFile}", projectFilePath);
            }

            return dependencies;
        }

        private List<ExternalDependency> ParsePackagesConfig(string packageConfigPath)
        {
            var dependencies = new List<ExternalDependency>();

            try
            {
                var doc = new XmlDocument();
                doc.Load(packageConfigPath);

                var packages = doc.SelectNodes("//package");
                if (packages != null)
                {
                    foreach (XmlNode package in packages)
                    {
                        var id = package.Attributes?["id"]?.Value;
                        var version = package.Attributes?["version"]?.Value;

                        if (!string.IsNullOrEmpty(id))
                        {
                            dependencies.Add(new ExternalDependency
                            {
                                Name = id,
                                Version = version ?? "Unknown",
                                Source = "NuGet",
                                ReferencedBy = new List<string> { packageConfigPath }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse packages.config {PackageConfig}", packageConfigPath);
            }

            return dependencies;
        }

        private List<DatabaseDependency> ExtractDatabaseDependencies(string configFilePath)
        {
            var dependencies = new List<DatabaseDependency>();

            try
            {
                var content = File.ReadAllText(configFilePath);
                
                // Extract connection strings using regex
                var connectionStringPattern = new Regex(@"connectionString\s*=\s*""([^""]+)""", RegexOptions.IgnoreCase);
                var matches = connectionStringPattern.Matches(content);

                foreach (Match match in matches)
                {
                    var connectionString = match.Groups[1].Value;
                    var dbName = ExtractDatabaseName(connectionString);

                    if (!string.IsNullOrEmpty(dbName))
                    {
                        dependencies.Add(new DatabaseDependency
                        {
                            DatabaseName = dbName,
                            ConnectionString = connectionString,
                            Tables = new List<string>(),
                            Views = new List<string>(),
                            Procedures = new List<string>()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract database dependencies from {ConfigFile}", configFilePath);
            }

            return dependencies;
        }

        private List<DatabaseDependency> ExtractSqlDependencies(string sqlFilePath)
        {
            var dependencies = new List<DatabaseDependency>();

            try
            {
                var content = File.ReadAllText(sqlFilePath);
                
                // Extract table references
                var tablePattern = new Regex(@"FROM\s+(\w+)", RegexOptions.IgnoreCase);
                var tableMatches = tablePattern.Matches(content);

                var tables = tableMatches.Cast<Match>()
                    .Select(m => m.Groups[1].Value)
                    .Distinct()
                    .ToList();

                if (tables.Any())
                {
                    dependencies.Add(new DatabaseDependency
                    {
                        DatabaseName = "Unknown",
                        ConnectionString = string.Empty,
                        Tables = tables,
                        Views = new List<string>(),
                        Procedures = new List<string>()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract SQL dependencies from {SqlFile}", sqlFilePath);
            }

            return dependencies;
        }

        private string ExtractDatabaseName(string connectionString)
        {
            var patterns = new[]
            {
                @"Database\s*=\s*([^;]+)",
                @"Initial Catalog\s*=\s*([^;]+)",
                @"Data Source\s*=\s*([^;]+)"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(connectionString, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            return string.Empty;
        }

        #endregion
    }
}

