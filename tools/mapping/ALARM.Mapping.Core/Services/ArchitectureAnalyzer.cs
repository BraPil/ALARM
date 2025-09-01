using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// Comprehensive architecture analysis engine for pattern detection and architectural insights
    /// </summary>
    public class ArchitectureAnalyzer : IArchitectureAnalyzer
    {
        private readonly ILogger<ArchitectureAnalyzer> _logger;

        // Common architectural patterns and their indicators
        private static readonly Dictionary<ArchitecturalPattern, List<string>> PatternIndicators = new()
        {
            [ArchitecturalPattern.MVC] = new() { "Controller", "Model", "View", "Action" },
            [ArchitecturalPattern.MVP] = new() { "Presenter", "Model", "View", "IView" },
            [ArchitecturalPattern.MVVM] = new() { "ViewModel", "Model", "View", "Command", "Binding" },
            [ArchitecturalPattern.Layered] = new() { "Business", "Data", "Presentation", "Service", "Repository" },
            [ArchitecturalPattern.Repository] = new() { "Repository", "IRepository", "UnitOfWork" },
            [ArchitecturalPattern.ServiceOriented] = new() { "Service", "IService", "ServiceContract", "WCF" }
        };

        // Layer detection patterns
        private static readonly Dictionary<string, List<string>> LayerPatterns = new()
        {
            ["Presentation"] = new() { "UI", "Web", "View", "Controller", "Page", "Form", "Window" },
            ["Business"] = new() { "Business", "Logic", "Service", "Manager", "Engine", "Processor" },
            ["Data"] = new() { "Data", "Repository", "DAO", "Entity", "Model", "Context" },
            ["Infrastructure"] = new() { "Infrastructure", "Utility", "Helper", "Common", "Shared" }
        };

        // Design pattern indicators
        private static readonly Dictionary<PatternType, List<string>> DesignPatternIndicators = new()
        {
            [PatternType.Singleton] = new() { "Instance", "GetInstance", "SingleInstance" },
            [PatternType.Factory] = new() { "Factory", "Create", "Builder" },
            [PatternType.Observer] = new() { "Observer", "Event", "Notify", "Subscribe" },
            [PatternType.Strategy] = new() { "Strategy", "Algorithm", "IStrategy" },
            [PatternType.Decorator] = new() { "Decorator", "Wrapper", "IComponent" },
            [PatternType.Adapter] = new() { "Adapter", "IAdapter", "Adaptee" }
        };

        public ArchitectureAnalyzer(ILogger<ArchitectureAnalyzer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Performs comprehensive architecture analysis
        /// </summary>
        public async Task<ArchitectureAnalysis> AnalyzeAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            ArchitectureOptions options,
            CancellationToken cancellationToken = default)
        {
            if (codeAnalysis == null)
                throw new ArgumentNullException(nameof(codeAnalysis));
            
            if (dependencyAnalysis == null)
                throw new ArgumentNullException(nameof(dependencyAnalysis));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _logger.LogInformation("Starting architecture analysis for {TotalSymbols} symbols", codeAnalysis.Symbols.Count);

            var analysis = new ArchitectureAnalysis();

            try
            {
                // Detect architectural patterns
                if (options.DetectPatterns)
                {
                    _logger.LogInformation("Detecting architectural patterns");
                    analysis.DetectedPattern = await DetectPatternAsync(codeAnalysis, dependencyAnalysis, cancellationToken);
                    _logger.LogInformation("Detected architectural pattern: {Pattern}", analysis.DetectedPattern);
                }

                // Detect layers
                if (options.DetectLayers)
                {
                    _logger.LogInformation("Detecting architectural layers");
                    analysis.Layers = await DetectLayersAsync(codeAnalysis, dependencyAnalysis, cancellationToken);
                    _logger.LogInformation("Detected {LayerCount} architectural layers", analysis.Layers.Count);
                }

                // Detect components
                if (options.DetectComponents)
                {
                    _logger.LogInformation("Detecting components");
                    analysis.Components = await DetectComponentsAsync(codeAnalysis, cancellationToken);
                    _logger.LogInformation("Detected {ComponentCount} components", analysis.Components.Count);
                }

                // Detect design patterns
                if (options.DetectDesignPatterns)
                {
                    _logger.LogInformation("Detecting design patterns");
                    analysis.DesignPatterns = await DetectDesignPatternsAsync(codeAnalysis, cancellationToken);
                    _logger.LogInformation("Detected {PatternCount} design patterns", analysis.DesignPatterns.Count);
                }

                // Calculate cohesion and coupling metrics
                analysis.Cohesion = await CalculateCohesionMetricsAsync(codeAnalysis, dependencyAnalysis, cancellationToken);
                analysis.Coupling = await CalculateCouplingMetricsAsync(dependencyAnalysis, cancellationToken);

                // Detect architectural violations
                if (options.DetectViolations)
                {
                    _logger.LogInformation("Detecting architectural violations");
                    analysis.Violations = await DetectViolationsAsync(analysis, cancellationToken);
                    _logger.LogInformation("Detected {ViolationCount} architectural violations", analysis.Violations.Count);
                }

                // Build modules from components
                analysis.Modules = await BuildModulesAsync(analysis.Components, codeAnalysis, cancellationToken);

                _logger.LogInformation("Architecture analysis complete. Pattern: {Pattern}, Layers: {LayerCount}, Components: {ComponentCount}",
                    analysis.DetectedPattern, analysis.Layers.Count, analysis.Components.Count);

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Architecture analysis failed");
                throw;
            }
        }

        /// <summary>
        /// Detects the primary architectural pattern
        /// </summary>
        public async Task<ArchitecturalPattern> DetectPatternAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken = default)
        {
            var patternScores = new Dictionary<ArchitecturalPattern, double>();

            await Task.Run(() =>
            {
                var classNames = codeAnalysis.Symbols
                    .Where(s => s.Type == SymbolType.Class)
                    .Select(s => s.Name)
                    .ToList();

                var namespaces = codeAnalysis.Symbols
                    .Where(s => !string.IsNullOrEmpty(s.Namespace))
                    .Select(s => s.Namespace)
                    .Distinct()
                    .ToList();

                // Score each pattern based on indicators
                foreach (var (pattern, indicators) in PatternIndicators)
                {
                    var score = 0.0;

                    // Check class names for pattern indicators
                    foreach (var indicator in indicators)
                    {
                        var classMatches = classNames.Count(name => 
                            name.Contains(indicator, StringComparison.OrdinalIgnoreCase));
                        score += classMatches * 2.0; // Class names are strong indicators

                        var namespaceMatches = namespaces.Count(ns => 
                            ns.Contains(indicator, StringComparison.OrdinalIgnoreCase));
                        score += namespaceMatches * 1.0; // Namespace matches are moderate indicators
                    }

                    // Normalize score by total classes
                    if (classNames.Count > 0)
                    {
                        score = score / classNames.Count * 100;
                    }

                    patternScores[pattern] = score;
                }

                // Apply pattern-specific heuristics
                ApplyPatternHeuristics(patternScores, codeAnalysis, dependencyAnalysis);

            }, cancellationToken);

            // Return the pattern with the highest score, or Unknown if no clear pattern
            var bestPattern = patternScores.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            return bestPattern.Value > 10.0 ? bestPattern.Key : ArchitecturalPattern.Unknown;
        }

        /// <summary>
        /// Detects architectural layers
        /// </summary>
        public async Task<List<Layer>> DetectLayersAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken = default)
        {
            var layers = new List<Layer>();

            await Task.Run(() =>
            {
                var classes = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Class).ToList();

                foreach (var (layerName, patterns) in LayerPatterns)
                {
                    var layerClasses = new List<string>();

                    foreach (var cls in classes)
                    {
                        var matchScore = 0;
                        
                        // Check class name
                        foreach (var pattern in patterns)
                        {
                            if (cls.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                                matchScore += 2;
                        }

                        // Check namespace
                        foreach (var pattern in patterns)
                        {
                            if (cls.Namespace?.Contains(pattern, StringComparison.OrdinalIgnoreCase) == true)
                                matchScore += 1;
                        }

                        if (matchScore > 0)
                        {
                            layerClasses.Add(cls.FullName);
                        }
                    }

                    if (layerClasses.Any())
                    {
                        var layer = new Layer
                        {
                            Name = layerName,
                            Level = GetLayerLevel(layerName),
                            Components = layerClasses,
                            Dependencies = GetLayerDependencies(layerClasses, dependencyAnalysis)
                        };

                        layers.Add(layer);
                    }
                }

                // Sort layers by level
                layers = layers.OrderBy(l => l.Level).ToList();

            }, cancellationToken);

            return layers;
        }

        /// <summary>
        /// Detects components based on functionality
        /// </summary>
        public async Task<List<Component>> DetectComponentsAsync(
            CodeAnalysis codeAnalysis,
            CancellationToken cancellationToken = default)
        {
            var components = new List<Component>();

            await Task.Run(() =>
            {
                var classes = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Class).ToList();
                var interfaces = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Interface).ToList();

                // Group by namespace for component detection
                var namespaceGroups = classes.GroupBy(c => c.Namespace ?? "Default").ToList();

                foreach (var group in namespaceGroups)
                {
                    var namespaceName = group.Key;
                    var namespaceClasses = group.ToList();

                    if (namespaceClasses.Count < 2) continue; // Skip single-class namespaces

                    var component = new Component
                    {
                        Name = GetComponentName(namespaceName),
                        Type = DetermineComponentType(namespaceName, namespaceClasses),
                        Classes = namespaceClasses.Select(c => c.FullName).ToList(),
                        Interfaces = interfaces.Where(i => i.Namespace == namespaceName).Select(i => i.FullName).ToList()
                    };

                    components.Add(component);
                }

                // Detect cross-cutting concerns
                DetectCrossCuttingComponents(components, classes);

            }, cancellationToken);

            return components;
        }

        /// <summary>
        /// Detects design patterns in the codebase
        /// </summary>
        public async Task<List<DesignPattern>> DetectDesignPatternsAsync(
            CodeAnalysis codeAnalysis,
            CancellationToken cancellationToken = default)
        {
            var patterns = new List<DesignPattern>();

            await Task.Run(() =>
            {
                var classes = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Class).ToList();
                var methods = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Method).ToList();

                foreach (var (patternType, indicators) in DesignPatternIndicators)
                {
                    var participatingClasses = new List<string>();
                    var confidence = 0.0;

                    foreach (var cls in classes)
                    {
                        var matchScore = 0.0;

                        // Check class name for pattern indicators
                        foreach (var indicator in indicators)
                        {
                            if (cls.Name.Contains(indicator, StringComparison.OrdinalIgnoreCase))
                                matchScore += 2.0;
                        }

                        // Check method names for pattern indicators
                        var classMethods = methods.Where(m => m.FullName.StartsWith(cls.FullName + ".")).ToList();
                        foreach (var method in classMethods)
                        {
                            foreach (var indicator in indicators)
                            {
                                if (method.Name.Contains(indicator, StringComparison.OrdinalIgnoreCase))
                                    matchScore += 1.0;
                            }
                        }

                        if (matchScore > 0)
                        {
                            participatingClasses.Add(cls.FullName);
                            confidence += matchScore;
                        }
                    }

                    if (participatingClasses.Any() && confidence > 2.0)
                    {
                        // Normalize confidence
                        confidence = Math.Min(1.0, confidence / (participatingClasses.Count * 3.0));

                        patterns.Add(new DesignPattern
                        {
                            Type = patternType,
                            Name = patternType.ToString(),
                            ParticipatingClasses = participatingClasses,
                            Confidence = confidence
                        });
                    }
                }

            }, cancellationToken);

            return patterns.OrderByDescending(p => p.Confidence).ToList();
        }

        /// <summary>
        /// Detects architectural violations
        /// </summary>
        public async Task<List<ArchitecturalViolation>> DetectViolationsAsync(
            ArchitectureAnalysis architecture,
            CancellationToken cancellationToken = default)
        {
            var violations = new List<ArchitecturalViolation>();

            await Task.Run(() =>
            {
                // Detect layer violations
                DetectLayerViolations(violations, architecture.Layers);

                // Detect god classes (too many responsibilities)
                DetectGodClasses(violations, architecture.Components);

                // Detect feature envy (classes using too many external methods)
                DetectFeatureEnvy(violations, architecture.Components);

                // Detect data classes (classes with only data, no behavior)
                DetectDataClasses(violations, architecture.Components);

            }, cancellationToken);

            return violations;
        }

        #region Private Methods

        private void ApplyPatternHeuristics(
            Dictionary<ArchitecturalPattern, double> patternScores,
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis)
        {
            // MVC heuristic: Controllers should have actions, Models should be data-focused
            if (patternScores.ContainsKey(ArchitecturalPattern.MVC))
            {
                var controllers = codeAnalysis.Symbols.Where(s => 
                    s.Type == SymbolType.Class && s.Name.Contains("Controller")).ToList();
                
                if (controllers.Any())
                {
                    var hasActions = controllers.Any(c => 
                        codeAnalysis.Symbols.Any(m => 
                            m.Type == SymbolType.Method && 
                            m.FullName.StartsWith(c.FullName) && 
                            m.AccessModifier == AccessModifier.Public));
                    
                    if (hasActions)
                        patternScores[ArchitecturalPattern.MVC] += 20.0;
                }
            }

            // Repository heuristic: Should have CRUD operations
            if (patternScores.ContainsKey(ArchitecturalPattern.Repository))
            {
                var repositories = codeAnalysis.Symbols.Where(s => 
                    s.Type == SymbolType.Class && s.Name.Contains("Repository")).ToList();
                
                if (repositories.Any())
                {
                    var crudMethods = new[] { "Create", "Read", "Update", "Delete", "Get", "Add", "Remove" };
                    var hasCrud = repositories.Any(r =>
                        crudMethods.Any(crud =>
                            codeAnalysis.Symbols.Any(m =>
                                m.Type == SymbolType.Method &&
                                m.FullName.StartsWith(r.FullName) &&
                                m.Name.Contains(crud, StringComparison.OrdinalIgnoreCase))));
                    
                    if (hasCrud)
                        patternScores[ArchitecturalPattern.Repository] += 25.0;
                }
            }
        }

        private int GetLayerLevel(string layerName)
        {
            return layerName switch
            {
                "Presentation" => 1,
                "Business" => 2,
                "Data" => 3,
                "Infrastructure" => 4,
                _ => 0
            };
        }

        private List<string> GetLayerDependencies(List<string> layerClasses, DependencyAnalysis dependencyAnalysis)
        {
            var dependencies = new List<string>();

            foreach (var className in layerClasses)
            {
                var classDependencies = dependencyAnalysis.StaticDependencies
                    .Where(d => d.From == className)
                    .Select(d => d.To)
                    .Where(to => !layerClasses.Contains(to)) // External dependencies only
                    .ToList();

                dependencies.AddRange(classDependencies);
            }

            return dependencies.Distinct().ToList();
        }

        private string GetComponentName(string namespaceName)
        {
            var parts = namespaceName.Split('.');
            return parts.LastOrDefault() ?? "Unknown";
        }

        private ComponentType DetermineComponentType(string namespaceName, List<CodeSymbol> classes)
        {
            var namespaceLower = namespaceName.ToLowerInvariant();

            if (namespaceLower.Contains("ui") || namespaceLower.Contains("view") || namespaceLower.Contains("form"))
                return ComponentType.UserInterface;
            
            if (namespaceLower.Contains("business") || namespaceLower.Contains("logic") || namespaceLower.Contains("service"))
                return ComponentType.BusinessLogic;
            
            if (namespaceLower.Contains("data") || namespaceLower.Contains("repository") || namespaceLower.Contains("entity"))
                return ComponentType.DataAccess;
            
            if (namespaceLower.Contains("utility") || namespaceLower.Contains("helper") || namespaceLower.Contains("common"))
                return ComponentType.Utility;
            
            if (namespaceLower.Contains("infrastructure"))
                return ComponentType.Infrastructure;

            // Analyze class types for better classification
            var hasDataClasses = classes.Any(c => c.Name.EndsWith("Model") || c.Name.EndsWith("Entity"));
            var hasServiceClasses = classes.Any(c => c.Name.EndsWith("Service") || c.Name.EndsWith("Manager"));

            if (hasServiceClasses)
                return ComponentType.Service;
            
            if (hasDataClasses)
                return ComponentType.DataAccess;

            return ComponentType.Unknown;
        }

        private void DetectCrossCuttingComponents(List<Component> components, List<CodeSymbol> classes)
        {
            // Detect logging components
            var loggingClasses = classes.Where(c => 
                c.Name.Contains("Log", StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains("Audit", StringComparison.OrdinalIgnoreCase)).ToList();

            if (loggingClasses.Any())
            {
                components.Add(new Component
                {
                    Name = "Logging",
                    Type = ComponentType.Infrastructure,
                    Classes = loggingClasses.Select(c => c.FullName).ToList(),
                    Interfaces = new List<string>()
                });
            }

            // Detect validation components
            var validationClasses = classes.Where(c => 
                c.Name.Contains("Valid", StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains("Rule", StringComparison.OrdinalIgnoreCase)).ToList();

            if (validationClasses.Any())
            {
                components.Add(new Component
                {
                    Name = "Validation",
                    Type = ComponentType.BusinessLogic,
                    Classes = validationClasses.Select(c => c.FullName).ToList(),
                    Interfaces = new List<string>()
                });
            }
        }

        private async Task<CohesionMetrics> CalculateCohesionMetricsAsync(
            CodeAnalysis codeAnalysis,
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var classes = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Class).ToList();
                var methods = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Method).ToList();
                var properties = codeAnalysis.Symbols.Where(s => s.Type == SymbolType.Property).ToList();

                var totalLCOM = 0.0;
                var totalTCC = 0.0;
                var classCount = 0;

                foreach (var cls in classes)
                {
                    var classMethods = methods.Where(m => m.FullName.StartsWith(cls.FullName + ".")).ToList();
                    var classProperties = properties.Where(p => p.FullName.StartsWith(cls.FullName + ".")).ToList();

                    if (classMethods.Count > 1)
                    {
                        // Simple LCOM calculation (higher is worse)
                        var memberCount = classMethods.Count + classProperties.Count;
                        var lcom = memberCount > 0 ? (double)classMethods.Count / memberCount : 0;
                        totalLCOM += lcom;

                        // Simple TCC calculation (higher is better)
                        var tcc = memberCount > 1 ? 1.0 / memberCount : 1.0;
                        totalTCC += tcc;

                        classCount++;
                    }
                }

                return new CohesionMetrics
                {
                    LCOM = classCount > 0 ? totalLCOM / classCount : 0,
                    TCC = classCount > 0 ? totalTCC / classCount : 0
                };
            }, cancellationToken);
        }

        private async Task<CouplingMetrics> CalculateCouplingMetricsAsync(
            DependencyAnalysis dependencyAnalysis,
            CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var dependencies = dependencyAnalysis.StaticDependencies;
                var allComponents = dependencies.Select(d => d.From).Union(dependencies.Select(d => d.To)).Distinct().ToList();

                var totalAfferent = 0.0;
                var totalEfferent = 0.0;

                foreach (var component in allComponents)
                {
                    // Afferent coupling: dependencies coming into this component
                    var afferent = dependencies.Count(d => d.To == component);
                    totalAfferent += afferent;

                    // Efferent coupling: dependencies going out from this component
                    var efferent = dependencies.Count(d => d.From == component);
                    totalEfferent += efferent;
                }

                var componentCount = allComponents.Count;
                var avgAfferent = componentCount > 0 ? totalAfferent / componentCount : 0;
                var avgEfferent = componentCount > 0 ? totalEfferent / componentCount : 0;

                return new CouplingMetrics
                {
                    AfferentCoupling = avgAfferent,
                    EfferentCoupling = avgEfferent,
                    Instability = (avgAfferent + avgEfferent) > 0 ? avgEfferent / (avgAfferent + avgEfferent) : 0
                };
            }, cancellationToken);
        }

        private async Task<List<Module>> BuildModulesAsync(
            List<Component> components,
            CodeAnalysis codeAnalysis,
            CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var modules = new List<Module>();

                // Group components by assembly (if available)
                var assemblies = codeAnalysis.Assemblies;
                
                if (assemblies.Any())
                {
                    foreach (var assembly in assemblies)
                    {
                        var assemblyComponents = components.Where(c => 
                            c.Classes.Any(cls => assembly.Namespaces.Any(ns => cls.StartsWith(ns.Name)))).ToList();

                        if (assemblyComponents.Any())
                        {
                            modules.Add(new Module
                            {
                                Name = assembly.Name,
                                Assemblies = new List<string> { assembly.Name },
                                Components = assemblyComponents
                            });
                        }
                    }
                }
                else
                {
                    // Fallback: create a single module
                    modules.Add(new Module
                    {
                        Name = "Main",
                        Assemblies = new List<string> { "Unknown" },
                        Components = components
                    });
                }

                return modules;
            }, cancellationToken);
        }

        private void DetectLayerViolations(List<ArchitecturalViolation> violations, List<Layer> layers)
        {
            // Check for layer skipping (presentation calling data directly)
            var presentationLayer = layers.FirstOrDefault(l => l.Name == "Presentation");
            var dataLayer = layers.FirstOrDefault(l => l.Name == "Data");

            if (presentationLayer != null && dataLayer != null)
            {
                var directDataCalls = presentationLayer.Dependencies.Intersect(dataLayer.Components).ToList();
                
                foreach (var violation in directDataCalls)
                {
                    violations.Add(new ArchitecturalViolation
                    {
                        Type = ViolationType.LayerViolation,
                        Description = $"Presentation layer directly accessing data layer: {violation}",
                        Location = violation,
                        Severity = ViolationSeverity.High
                    });
                }
            }
        }

        private void DetectGodClasses(List<ArchitecturalViolation> violations, List<Component> components)
        {
            foreach (var component in components)
            {
                if (component.Classes.Count > 20) // Threshold for too many classes in component
                {
                    violations.Add(new ArchitecturalViolation
                    {
                        Type = ViolationType.GodClass,
                        Description = $"Component '{component.Name}' has too many classes ({component.Classes.Count})",
                        Location = component.Name,
                        Severity = ViolationSeverity.Medium
                    });
                }
            }
        }

        private void DetectFeatureEnvy(List<ArchitecturalViolation> violations, List<Component> components)
        {
            // This would require more detailed dependency analysis
            // For now, we'll create a placeholder
            foreach (var component in components.Where(c => c.Type == ComponentType.BusinessLogic))
            {
                if (component.Classes.Count < 3) // Too few classes might indicate feature envy
                {
                    violations.Add(new ArchitecturalViolation
                    {
                        Type = ViolationType.FeatureEnvy,
                        Description = $"Business component '{component.Name}' might have feature envy (too few classes)",
                        Location = component.Name,
                        Severity = ViolationSeverity.Low
                    });
                }
            }
        }

        private void DetectDataClasses(List<ArchitecturalViolation> violations, List<Component> components)
        {
            foreach (var component in components.Where(c => c.Type == ComponentType.DataAccess))
            {
                if (component.Classes.All(cls => cls.Contains("Model") || cls.Contains("Entity")))
                {
                    violations.Add(new ArchitecturalViolation
                    {
                        Type = ViolationType.DataClass,
                        Description = $"Component '{component.Name}' appears to contain only data classes",
                        Location = component.Name,
                        Severity = ViolationSeverity.Low
                    });
                }
            }
        }

        #endregion
    }
}

