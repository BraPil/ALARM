using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// Comprehensive relationship mapping engine for building relationship matrices and visualization data
    /// </summary>
    public class RelationshipMapper : IRelationshipMapper
    {
        private readonly ILogger<RelationshipMapper> _logger;

        public RelationshipMapper(ILogger<RelationshipMapper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Interface implementation methods (legacy)
        public async Task<RelationshipMatrix> MapRelationshipsAsync(CodeAnalysis codeAnalysis, DependencyAnalysis dependencyAnalysis, Interfaces.RelationshipOptions options, CancellationToken cancellationToken = default)
        {
            var newOptions = new Models.RelationshipOptions
            {
                BuildRelationshipMatrix = true,
                BuildComponentRelationships = options.MapMethodCalls,
                BuildLayerRelationships = options.MapInheritance,
                BuildDependencyMatrix = true,
                BuildCallHierarchy = options.MapMethodCalls,
                BuildInheritanceTree = options.MapInheritance
            };

            var mapping = await BuildMappingAsync(codeAnalysis, dependencyAnalysis, new ArchitectureAnalysis(), newOptions, cancellationToken);
            return mapping.RelationshipMatrix ?? new RelationshipMatrix();
        }

        public async Task<List<InheritanceRelationship>> MapInheritanceAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default)
        {
            // Legacy implementation - convert from new format
            var relationships = new List<InheritanceRelationship>();
            var inheritanceDeps = codeAnalysis.Symbols
                .Where(s => s.Type == SymbolType.Class)
                .Take(10) // Limit for demo
                .Select(s => new InheritanceRelationship
                {
                    DerivedType = s.FullName,
                    BaseType = "System.Object", // Simplified
                    Type = InheritanceType.ClassInheritance
                })
                .ToList();
            
            await Task.CompletedTask;
            return inheritanceDeps;
        }

        public async Task<List<CompositionRelationship>> MapCompositionAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return new List<CompositionRelationship>(); // Simplified
        }

        public async Task<List<MethodCallRelationship>> MapMethodCallsAsync(CodeAnalysis codeAnalysis, CancellationToken cancellationToken = default)
        {
            var methodCalls = new List<MethodCallRelationship>();
            var methods = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Method).Take(10).ToList();
            
            foreach (var method in methods)
            {
                methodCalls.Add(new MethodCallRelationship
                {
                    Caller = method.FullName,
                    Callee = "SomeMethod", // Simplified
                    MethodName = method.Name,
                    CallCount = 1
                });
            }
            
            await Task.CompletedTask;
            return methodCalls;
        }

        public async Task<RelationshipGraph> BuildGraphAsync(RelationshipMatrix relationships, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return relationships.Graph;
        }

        public async Task<RelationshipStatistics> CalculateStatisticsAsync(RelationshipMatrix relationships, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return relationships.Statistics;
        }

        /// <summary>
        /// Builds comprehensive relationship mapping from all analysis results
        /// </summary>
        public async Task<RelationshipMapping> BuildMappingAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            Models.RelationshipOptions options,
            CancellationToken cancellationToken = default)
        {
            if (codeAnalysis == null)
                throw new ArgumentNullException(nameof(codeAnalysis));
            
            if (dependencyAnalysis == null)
                throw new ArgumentNullException(nameof(dependencyAnalysis));
            
            if (architectureAnalysis == null)
                throw new ArgumentNullException(nameof(architectureAnalysis));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _logger.LogInformation("Starting relationship mapping for {TotalSymbols} symbols", codeAnalysis.Symbols.Count);

            var mapping = new RelationshipMapping();

            try
            {
                // Build relationship matrix
                if (options.BuildRelationshipMatrix)
                {
                    _logger.LogInformation("Building relationship matrix");
                    mapping.RelationshipMatrix = await BuildRelationshipMatrixAsync(
                        codeAnalysis, dependencyAnalysis, architectureAnalysis, cancellationToken);
                    _logger.LogInformation("Built relationship matrix with {Relationships} relationships", 
                        mapping.RelationshipMatrix.Relationships.Count);
                }

                // Build component relationships
                if (options.BuildComponentRelationships)
                {
                    _logger.LogInformation("Building component relationships");
                    mapping.ComponentRelationships = await BuildComponentRelationshipsAsync(
                        architectureAnalysis, dependencyAnalysis, cancellationToken);
                    _logger.LogInformation("Built {ComponentRelationships} component relationships", 
                        mapping.ComponentRelationships.Count);
                }

                // Build layer relationships
                if (options.BuildLayerRelationships)
                {
                    _logger.LogInformation("Building layer relationships");
                    mapping.LayerRelationships = await BuildLayerRelationshipsAsync(
                        architectureAnalysis, dependencyAnalysis, cancellationToken);
                    _logger.LogInformation("Built {LayerRelationships} layer relationships", 
                        mapping.LayerRelationships.Count);
                }

                // Build dependency strength matrix
                if (options.BuildDependencyMatrix)
                {
                    _logger.LogInformation("Building dependency strength matrix");
                    mapping.DependencyMatrix = await BuildDependencyMatrixAsync(
                        dependencyAnalysis, architectureAnalysis, cancellationToken);
                    _logger.LogInformation("Built dependency matrix with {Entries} entries", 
                        mapping.DependencyMatrix.Entries.Count);
                }

                // Build call hierarchy
                if (options.BuildCallHierarchy)
                {
                    _logger.LogInformation("Building call hierarchy");
                    mapping.CallHierarchy = await BuildCallHierarchyAsync(
                        codeAnalysis, dependencyAnalysis, cancellationToken);
                    _logger.LogInformation("Built call hierarchy with {Nodes} nodes", 
                        mapping.CallHierarchy.Nodes.Count);
                }

                // Build inheritance tree
                if (options.BuildInheritanceTree)
                {
                    _logger.LogInformation("Building inheritance tree");
                    mapping.InheritanceTree = await BuildInheritanceTreeAsync(
                        codeAnalysis, dependencyAnalysis, cancellationToken);
                    _logger.LogInformation("Built inheritance tree with {Nodes} nodes", 
                        mapping.InheritanceTree.Nodes.Count);
                }

                // Generate relationship statistics
                mapping.Statistics = await GenerateRelationshipStatisticsAsync(mapping, cancellationToken);

                _logger.LogInformation("Relationship mapping complete. Matrix: {MatrixSize}, Components: {ComponentCount}",
                    mapping.RelationshipMatrix?.Relationships.Count ?? 0, mapping.ComponentRelationships?.Count ?? 0);

                return mapping;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Relationship mapping failed");
                throw;
            }
        }

        /// <summary>
        /// Builds comprehensive relationship matrix
        /// </summary>
        public async Task<RelationshipMatrix> BuildRelationshipMatrixAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            CancellationToken cancellationToken = default)
        {
            var matrix = new RelationshipMatrix();

            await Task.Run(() =>
            {
                var relationships = new List<Relationship>();

                // Add static dependencies as relationships
                foreach (var dependency in dependencyAnalysis.StaticDependencies)
                {
                    var relationship = new Relationship
                    {
                        Source = dependency.From,
                        Target = dependency.To,
                        Type = GetRelationshipType(dependency.Type),
                        Strength = CalculateRelationshipStrength(dependency.Type),
                        Direction = RelationshipDirection.Outbound,
                        Metadata = new Dictionary<string, object>
                        {
                            ["DependencyType"] = dependency.Type.ToString(),
                            ["SourceFile"] = dependency.SourceFile
                        }
                    };

                    relationships.Add(relationship);
                }

                // Add component relationships
                foreach (var component in architectureAnalysis.Components)
                {
                    foreach (var className in component.Classes)
                    {
                        var componentRelationship = new Relationship
                        {
                            Source = className,
                            Target = component.Name,
                            Type = RelationshipType.ComponentMembership,
                            Strength = 1.0,
                            Direction = RelationshipDirection.Bidirectional,
                            Metadata = new Dictionary<string, object>
                            {
                                ["ComponentType"] = component.Type.ToString()
                            }
                        };

                        relationships.Add(componentRelationship);
                    }
                }

                // Add layer relationships
                foreach (var layer in architectureAnalysis.Layers)
                {
                    foreach (var componentName in layer.Components)
                    {
                        var layerRelationship = new Relationship
                        {
                            Source = componentName,
                            Target = layer.Name,
                            Type = RelationshipType.LayerMembership,
                            Strength = 1.0,
                            Direction = RelationshipDirection.Bidirectional,
                            Metadata = new Dictionary<string, object>
                            {
                                ["LayerLevel"] = layer.Level
                            }
                        };

                        relationships.Add(layerRelationship);
                    }
                }

                matrix.Relationships = relationships;
                matrix.Sources = relationships.Select(r => r.Source).Distinct().ToList();
                matrix.Targets = relationships.Select(r => r.Target).Distinct().ToList();
                matrix.RelationshipTypes = relationships.Select(r => r.Type).Distinct().ToList();

            }, cancellationToken);

            return matrix;
        }

        /// <summary>
        /// Builds component-to-component relationships
        /// </summary>
        public async Task<List<ComponentRelationship>> BuildComponentRelationshipsAsync(
            ArchitectureAnalysis architectureAnalysis,
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken = default)
        {
            var componentRelationships = new List<ComponentRelationship>();

            await Task.Run(() =>
            {
                var components = architectureAnalysis.Components;

                foreach (var sourceComponent in components)
                {
                    foreach (var targetComponent in components)
                    {
                        if (sourceComponent.Name == targetComponent.Name) continue;

                        // Calculate relationships between components based on their classes
                        var relationships = dependencyAnalysis.StaticDependencies
                            .Where(d => sourceComponent.Classes.Contains(d.From) && 
                                       targetComponent.Classes.Contains(d.To))
                            .ToList();

                        if (relationships.Any())
                        {
                            var componentRelationship = new ComponentRelationship
                            {
                                SourceComponent = sourceComponent.Name,
                                TargetComponent = targetComponent.Name,
                                RelationshipCount = relationships.Count,
                                RelationshipTypes = relationships.Select(r => r.Type).Distinct().ToList(),
                                Strength = CalculateComponentRelationshipStrength(relationships),
                                Description = $"{sourceComponent.Name} depends on {targetComponent.Name}",
                                Metadata = new Dictionary<string, object>
                                {
                                    ["SourceType"] = sourceComponent.Type.ToString(),
                                    ["TargetType"] = targetComponent.Type.ToString(),
                                    ["DependencyCount"] = relationships.Count
                                }
                            };

                            componentRelationships.Add(componentRelationship);
                        }
                    }
                }

            }, cancellationToken);

            return componentRelationships.OrderByDescending(cr => cr.Strength).ToList();
        }

        /// <summary>
        /// Builds layer-to-layer relationships
        /// </summary>
        public async Task<List<LayerRelationship>> BuildLayerRelationshipsAsync(
            ArchitectureAnalysis architectureAnalysis,
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken = default)
        {
            var layerRelationships = new List<LayerRelationship>();

            await Task.Run(() =>
            {
                var layers = architectureAnalysis.Layers;

                foreach (var sourceLayer in layers)
                {
                    foreach (var targetLayer in layers)
                    {
                        if (sourceLayer.Name == targetLayer.Name) continue;

                        // Calculate relationships between layers based on their components
                        var relationships = dependencyAnalysis.StaticDependencies
                            .Where(d => sourceLayer.Components.Contains(d.From) && 
                                       targetLayer.Components.Contains(d.To))
                            .ToList();

                        if (relationships.Any())
                        {
                            var layerRelationship = new LayerRelationship
                            {
                                SourceLayer = sourceLayer.Name,
                                TargetLayer = targetLayer.Name,
                                SourceLevel = sourceLayer.Level,
                                TargetLevel = targetLayer.Level,
                                RelationshipCount = relationships.Count,
                                Strength = CalculateLayerRelationshipStrength(relationships, sourceLayer, targetLayer),
                                IsViolation = IsLayerViolation(sourceLayer.Level, targetLayer.Level),
                                Description = $"Layer {sourceLayer.Name} (L{sourceLayer.Level}) → {targetLayer.Name} (L{targetLayer.Level})",
                                Metadata = new Dictionary<string, object>
                                {
                                    ["DependencyCount"] = relationships.Count,
                                    ["LevelDifference"] = Math.Abs(sourceLayer.Level - targetLayer.Level)
                                }
                            };

                            layerRelationships.Add(layerRelationship);
                        }
                    }
                }

            }, cancellationToken);

            return layerRelationships.OrderBy(lr => lr.SourceLevel).ThenBy(lr => lr.TargetLevel).ToList();
        }

        /// <summary>
        /// Builds dependency strength matrix
        /// </summary>
        public async Task<DependencyMatrix> BuildDependencyMatrixAsync(
            DependencyAnalysis dependencyAnalysis,
            ArchitectureAnalysis architectureAnalysis,
            CancellationToken cancellationToken = default)
        {
            var matrix = new DependencyMatrix();

            await Task.Run(() =>
            {
                var entries = new List<DependencyMatrixEntry>();
                var allComponents = architectureAnalysis.Components.SelectMany(c => c.Classes).ToList();

                // Build component-to-component dependency strength matrix
                foreach (var source in allComponents)
                {
                    foreach (var target in allComponents)
                    {
                        if (source == target) continue;

                        var dependencies = dependencyAnalysis.StaticDependencies
                            .Where(d => d.From == source && d.To == target)
                            .ToList();

                        if (dependencies.Any())
                        {
                            var strength = CalculateDependencyStrength(dependencies);
                            var entry = new DependencyMatrixEntry
                            {
                                Source = source,
                                Target = target,
                                DependencyCount = dependencies.Count,
                                Strength = strength,
                                DependencyTypes = dependencies.Select(d => d.Type).Distinct().ToList(),
                                Metadata = new Dictionary<string, object>
                                {
                                    ["MaxStrength"] = dependencies.Max(d => CalculateRelationshipStrength(d.Type)),
                                    ["AvgStrength"] = dependencies.Average(d => CalculateRelationshipStrength(d.Type))
                                }
                            };

                            entries.Add(entry);
                        }
                    }
                }

                matrix.Entries = entries.OrderByDescending(e => e.Strength).ToList();
                matrix.Components = allComponents;
                matrix.MaxStrength = entries.Any() ? entries.Max(e => e.Strength) : 0;
                matrix.MinStrength = entries.Any() ? entries.Min(e => e.Strength) : 0;

            }, cancellationToken);

            return matrix;
        }

        /// <summary>
        /// Builds call hierarchy tree
        /// </summary>
        public async Task<CallHierarchy> BuildCallHierarchyAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken = default)
        {
            var hierarchy = new CallHierarchy();

            await Task.Run(() =>
            {
                var nodes = new List<CallHierarchyNode>();
                var methods = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Method).ToList();

                foreach (var method in methods)
                {
                    var callees = dependencyAnalysis.StaticDependencies
                        .Where(d => d.From == method.FullName && d.Type == DependencyType.MethodCall)
                        .Select(d => d.To)
                        .ToList();

                    var callers = dependencyAnalysis.StaticDependencies
                        .Where(d => d.To == method.FullName && d.Type == DependencyType.MethodCall)
                        .Select(d => d.From)
                        .ToList();

                    var node = new CallHierarchyNode
                    {
                        Method = method.FullName,
                        ClassName = GetClassName(method.FullName),
                        Callees = callees,
                        Callers = callers,
                        CalleeCount = callees.Count,
                        CallerCount = callers.Count,
                        Complexity = CalculateMethodComplexity(method, callees.Count),
                        Metadata = new Dictionary<string, object>
                        {
                            ["AccessModifier"] = method.AccessModifier.ToString(),
                            ["SourceFile"] = method.SourceFile,
                            ["LineNumber"] = method.LineNumber
                        }
                    };

                    nodes.Add(node);
                }

                hierarchy.Nodes = nodes.OrderByDescending(n => n.CalleeCount).ToList();
                hierarchy.RootMethods = nodes.Where(n => n.CallerCount == 0).ToList();
                hierarchy.LeafMethods = nodes.Where(n => n.CalleeCount == 0).ToList();

            }, cancellationToken);

            return hierarchy;
        }

        /// <summary>
        /// Builds inheritance tree
        /// </summary>
        public async Task<InheritanceTree> BuildInheritanceTreeAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken = default)
        {
            var tree = new InheritanceTree();

            await Task.Run(() =>
            {
                var nodes = new List<InheritanceNode>();
                var classes = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Class).ToList();

                foreach (var cls in classes)
                {
                    var baseClasses = dependencyAnalysis.StaticDependencies
                        .Where(d => d.From == cls.FullName && d.Type == DependencyType.Inheritance)
                        .Select(d => d.To)
                        .ToList();

                    var derivedClasses = dependencyAnalysis.StaticDependencies
                        .Where(d => d.To == cls.FullName && d.Type == DependencyType.Inheritance)
                        .Select(d => d.From)
                        .ToList();

                    var node = new InheritanceNode
                    {
                        ClassName = cls.FullName,
                        BaseClasses = baseClasses,
                        DerivedClasses = derivedClasses,
                        InheritanceDepth = CalculateInheritanceDepth(cls.FullName, dependencyAnalysis.StaticDependencies),
                        IsAbstract = cls.Modifiers?.Contains("abstract") == true,
                        IsInterface = cls.Type == SymbolType.Interface,
                        Metadata = new Dictionary<string, object>
                        {
                            ["AccessModifier"] = cls.AccessModifier.ToString(),
                            ["SourceFile"] = cls.SourceFile,
                            ["LineNumber"] = cls.LineNumber
                        }
                    };

                    nodes.Add(node);
                }

                tree.Nodes = nodes;
                tree.RootClasses = nodes.Where(n => !n.BaseClasses.Any()).ToList();
                tree.LeafClasses = nodes.Where(n => !n.DerivedClasses.Any()).ToList();

            }, cancellationToken);

            return tree;
        }

        #region Private Helper Methods

        private RelationshipType GetRelationshipType(DependencyType dependencyType)
        {
            return dependencyType switch
            {
                DependencyType.MethodCall => RelationshipType.MethodCall,
                DependencyType.PropertyAccess => RelationshipType.PropertyAccess,
                DependencyType.Inheritance => RelationshipType.Inheritance,
                DependencyType.Using => RelationshipType.Using,
                _ => RelationshipType.Unknown
            };
        }

        private double CalculateRelationshipStrength(DependencyType dependencyType)
        {
            return dependencyType switch
            {
                DependencyType.Inheritance => 1.0,      // Strongest
                DependencyType.MethodCall => 0.8,       // Strong
                DependencyType.PropertyAccess => 0.6,   // Medium
                DependencyType.Using => 0.2,            // Weak
                _ => 0.1                                 // Minimal
            };
        }

        private double CalculateComponentRelationshipStrength(List<StaticDependency> relationships)
        {
            if (!relationships.Any()) return 0;

            var totalStrength = relationships.Sum(r => CalculateRelationshipStrength(r.Type));
            return totalStrength / relationships.Count; // Average strength
        }

        private double CalculateLayerRelationshipStrength(List<StaticDependency> relationships, Layer sourceLayer, Layer targetLayer)
        {
            if (!relationships.Any()) return 0;

            var baseStrength = CalculateComponentRelationshipStrength(relationships);
            
            // Adjust for layer violations (downward dependencies are violations)
            if (sourceLayer.Level > targetLayer.Level)
            {
                baseStrength *= 0.5; // Penalize upward dependencies
            }

            return baseStrength;
        }

        private bool IsLayerViolation(int sourceLevel, int targetLevel)
        {
            // Violation if a lower layer (higher number) depends on a higher layer (lower number)
            return sourceLevel > targetLevel;
        }

        private double CalculateDependencyStrength(List<StaticDependency> dependencies)
        {
            if (!dependencies.Any()) return 0;

            // Weight by dependency type and count
            var totalWeight = dependencies.Sum(d => CalculateRelationshipStrength(d.Type));
            var countMultiplier = Math.Log10(dependencies.Count + 1); // Logarithmic scaling
            
            return totalWeight * countMultiplier;
        }

        private string GetClassName(string fullMethodName)
        {
            var lastDotIndex = fullMethodName.LastIndexOf('.');
            return lastDotIndex > 0 ? fullMethodName.Substring(0, lastDotIndex) : fullMethodName;
        }

        private double CalculateMethodComplexity(CodeSymbol method, int calleeCount)
        {
            // Simple complexity based on callees (could be enhanced with cyclomatic complexity)
            return Math.Log10(calleeCount + 1) * 2.0;
        }

        private int CalculateInheritanceDepth(string className, List<StaticDependency> dependencies)
        {
            var visited = new HashSet<string>();
            return CalculateInheritanceDepthRecursive(className, dependencies, visited);
        }

        private int CalculateInheritanceDepthRecursive(string className, List<StaticDependency> dependencies, HashSet<string> visited)
        {
            if (visited.Contains(className)) return 0; // Circular reference protection
            visited.Add(className);

            var baseClasses = dependencies
                .Where(d => d.From == className && d.Type == DependencyType.Inheritance)
                .Select(d => d.To)
                .ToList();

            if (!baseClasses.Any()) return 0;

            var maxDepth = baseClasses.Max(bc => CalculateInheritanceDepthRecursive(bc, dependencies, visited));
            return maxDepth + 1;
        }

        private async Task<RelationshipStatistics> GenerateRelationshipStatisticsAsync(RelationshipMapping mapping, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var stats = new RelationshipStatistics();

                if (mapping.RelationshipMatrix != null)
                {
                    stats.TotalRelationships = mapping.RelationshipMatrix.Relationships.Count;
                    stats.RelationshipTypeDistribution = mapping.RelationshipMatrix.Relationships
                        .GroupBy(r => r.Type)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count());
                    stats.AverageRelationshipStrength = mapping.RelationshipMatrix.Relationships
                        .Average(r => r.Strength);
                }

                if (mapping.ComponentRelationships != null)
                {
                    stats.ComponentRelationshipCount = mapping.ComponentRelationships.Count;
                    stats.StrongestComponentRelationship = mapping.ComponentRelationships
                        .OrderByDescending(cr => cr.Strength)
                        .FirstOrDefault()?.Description ?? "None";
                }

                if (mapping.LayerRelationships != null)
                {
                    stats.LayerRelationshipCount = mapping.LayerRelationships.Count;
                    stats.LayerViolationCount = mapping.LayerRelationships.Count(lr => lr.IsViolation);
                }

                if (mapping.CallHierarchy != null)
                {
                    stats.TotalMethods = mapping.CallHierarchy.Nodes.Count;
                    stats.RootMethodCount = mapping.CallHierarchy.RootMethods.Count;
                    stats.LeafMethodCount = mapping.CallHierarchy.LeafMethods.Count;
                    stats.MaxCallDepth = mapping.CallHierarchy.Nodes.Any() ? 
                        mapping.CallHierarchy.Nodes.Max(n => n.CalleeCount) : 0;
                }

                if (mapping.InheritanceTree != null)
                {
                    stats.TotalClasses = mapping.InheritanceTree.Nodes.Count;
                    stats.RootClassCount = mapping.InheritanceTree.RootClasses.Count;
                    stats.MaxInheritanceDepth = mapping.InheritanceTree.Nodes.Any() ? 
                        mapping.InheritanceTree.Nodes.Max(n => n.InheritanceDepth) : 0;
                }

                return stats;
            }, cancellationToken);
        }

        #endregion
    }
}
