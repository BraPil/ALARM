using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ALARM.Mapping.Core
{
    /// <summary>
    /// Main ALARM mapping system orchestrator
    /// Coordinates all analysis phases to provide comprehensive application mapping
    /// </summary>
    public class ApplicationMapper : IApplicationMapper
    {
        private readonly IFileSystemCrawler _fileSystemCrawler;
        private readonly ICodeAnalysisEngine _codeAnalysisEngine;
        private readonly IDependencyResolver _dependencyResolver;
        private readonly IArchitectureAnalyzer _architectureAnalyzer;
        private readonly IRelationshipMapper _relationshipMapper;
        private readonly IVisualizationGenerator _visualizationGenerator;
        private readonly IMetricsCalculator _metricsCalculator;
        private readonly ILogger<ApplicationMapper> _logger;

        public ApplicationMapper(
            IFileSystemCrawler fileSystemCrawler,
            ICodeAnalysisEngine codeAnalysisEngine,
            IDependencyResolver dependencyResolver,
            IArchitectureAnalyzer architectureAnalyzer,
            IRelationshipMapper relationshipMapper,
            IVisualizationGenerator visualizationGenerator,
            IMetricsCalculator metricsCalculator,
            ILogger<ApplicationMapper> logger)
        {
            _fileSystemCrawler = fileSystemCrawler ?? throw new ArgumentNullException(nameof(fileSystemCrawler));
            _codeAnalysisEngine = codeAnalysisEngine ?? throw new ArgumentNullException(nameof(codeAnalysisEngine));
            _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            _architectureAnalyzer = architectureAnalyzer ?? throw new ArgumentNullException(nameof(architectureAnalyzer));
            _relationshipMapper = relationshipMapper ?? throw new ArgumentNullException(nameof(relationshipMapper));
            _visualizationGenerator = visualizationGenerator ?? throw new ArgumentNullException(nameof(visualizationGenerator));
            _metricsCalculator = metricsCalculator ?? throw new ArgumentNullException(nameof(metricsCalculator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Performs comprehensive analysis of target application
        /// </summary>
        public async Task<ApplicationAnalysis> AnalyzeApplicationAsync(
            string rootPath, 
            AnalysisConfiguration configuration, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Root path cannot be null or empty", nameof(rootPath));
            
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException($"Root path does not exist: {rootPath}");

            var stopwatch = Stopwatch.StartNew();
            var analysis = new ApplicationAnalysis
            {
                ApplicationName = Path.GetFileName(rootPath),
                RootPath = Path.GetFullPath(rootPath),
                AnalysisTimestamp = DateTime.UtcNow,
                Configuration = configuration
            };

            _logger.LogInformation("Starting comprehensive analysis of application: {ApplicationName} at {RootPath}", 
                analysis.ApplicationName, analysis.RootPath);

            try
            {
                // Phase 1: File System Analysis
                _logger.LogInformation("Phase 1: Starting file system analysis");
                analysis.FileSystemAnalysis = await AnalyzeFileSystemAsync(rootPath, configuration, cancellationToken);
                _logger.LogInformation("Phase 1: File system analysis complete - {TotalFiles} files, {TotalDirectories} directories",
                    analysis.FileSystemAnalysis.TotalFiles, analysis.FileSystemAnalysis.TotalDirectories);

                // Phase 2: Code Analysis
                _logger.LogInformation("Phase 2: Starting code analysis");
                analysis.CodeAnalysis = await AnalyzeCodeAsync(analysis.FileSystemAnalysis, configuration, cancellationToken);
                _logger.LogInformation("Phase 2: Code analysis complete - {TotalSymbols} symbols, {TotalClasses} classes",
                    analysis.CodeAnalysis.Symbols.Count, analysis.CodeAnalysis.TotalClasses);

                // Phase 3: Dependency Analysis
                _logger.LogInformation("Phase 3: Starting dependency analysis");
                analysis.DependencyAnalysis = await AnalyzeDependenciesAsync(analysis.CodeAnalysis, configuration, cancellationToken);
                _logger.LogInformation("Phase 3: Dependency analysis complete - {StaticDeps} static, {DynamicDeps} dynamic dependencies",
                    analysis.DependencyAnalysis.StaticDependencies.Count, analysis.DependencyAnalysis.DynamicDependencies.Count);

                // Phase 4: Architecture Analysis
                _logger.LogInformation("Phase 4: Starting architecture analysis");
                analysis.ArchitectureAnalysis = await AnalyzeArchitectureAsync(analysis.CodeAnalysis, analysis.DependencyAnalysis, configuration, cancellationToken);
                _logger.LogInformation("Phase 4: Architecture analysis complete - Pattern: {Pattern}, {ComponentCount} components",
                    analysis.ArchitectureAnalysis.DetectedPattern, analysis.ArchitectureAnalysis.Components.Count);

                // Phase 5: Relationship Mapping
                _logger.LogInformation("Phase 5: Starting relationship mapping");
                analysis.RelationshipMatrix = await MapRelationshipsAsync(analysis.CodeAnalysis, analysis.DependencyAnalysis, configuration, cancellationToken);
                _logger.LogInformation("Phase 5: Relationship mapping complete - {RelationshipCount} total relationships",
                    analysis.RelationshipMatrix.Statistics.RelationshipCounts.Values.Sum());

                // Phase 6: Metrics Calculation
                _logger.LogInformation("Phase 6: Starting metrics calculation");
                analysis.Metrics = await CalculateMetricsAsync(analysis, configuration, cancellationToken);
                _logger.LogInformation("Phase 6: Metrics calculation complete - Complexity: {Complexity:F2}, Quality: {Quality:F2}",
                    analysis.Metrics.AverageComplexity, analysis.Metrics.QualityScores.GetValueOrDefault("Overall", 0.0));

                // Finalize analysis
                stopwatch.Stop();
                analysis.AnalysisDuration = stopwatch.Elapsed;

                _logger.LogInformation("Analysis complete in {Duration:F2} seconds. Total symbols: {TotalSymbols}, Total relationships: {TotalRelationships}",
                    analysis.AnalysisDuration.TotalSeconds, analysis.Metrics.TotalSymbols, analysis.Metrics.TotalRelationships);

                return analysis;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                analysis.AnalysisDuration = stopwatch.Elapsed;
                analysis.Warnings.Add(new AnalysisWarning
                {
                    Level = WarningLevel.Critical,
                    Category = "Analysis",
                    Message = $"Analysis failed: {ex.Message}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object> { ["Exception"] = ex.ToString() }
                });

                _logger.LogError(ex, "Analysis failed for application: {ApplicationName}", analysis.ApplicationName);
                throw;
            }
        }

        /// <summary>
        /// Generates visualizations for the analyzed application
        /// </summary>
        public async Task<bool> GenerateVisualizationsAsync(
            ApplicationAnalysis analysis, 
            string outputPath, 
            CancellationToken cancellationToken = default)
        {
            if (analysis == null)
                throw new ArgumentNullException(nameof(analysis));
            
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

            _logger.LogInformation("Starting visualization generation for {ApplicationName} to {OutputPath}",
                analysis.ApplicationName, outputPath);

            try
            {
                Directory.CreateDirectory(outputPath);

                var request = new VisualizationRequest
                {
                    OutputPath = outputPath,
                    Types = new List<VisualizationType> 
                    { 
                        VisualizationType.Graphviz, 
                        VisualizationType.PlantUML, 
                        VisualizationType.D3Json,
                        VisualizationType.Mermaid
                    },
                    Options = analysis.Configuration.Visualization
                };

                var result = await _visualizationGenerator.GenerateAsync(analysis, request, cancellationToken);

                _logger.LogInformation("Visualization generation {Result} for {ApplicationName}",
                    result ? "succeeded" : "failed", analysis.ApplicationName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Visualization generation failed for {ApplicationName}", analysis.ApplicationName);
                return false;
            }
        }

        /// <summary>
        /// Loads analysis from saved file
        /// </summary>
        public async Task<ApplicationAnalysis> LoadAnalysisAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Analysis file not found: {filePath}");

            _logger.LogInformation("Loading analysis from {FilePath}", filePath);

            try
            {
                var json = await File.ReadAllTextAsync(filePath, cancellationToken);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var analysis = JsonSerializer.Deserialize<ApplicationAnalysis>(json, options);
                if (analysis == null)
                    throw new InvalidOperationException("Failed to deserialize analysis file");

                _logger.LogInformation("Successfully loaded analysis for {ApplicationName}", analysis.ApplicationName);
                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load analysis from {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Saves analysis to file
        /// </summary>
        public async Task SaveAnalysisAsync(ApplicationAnalysis analysis, string filePath, CancellationToken cancellationToken = default)
        {
            if (analysis == null)
                throw new ArgumentNullException(nameof(analysis));
            
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            _logger.LogInformation("Saving analysis for {ApplicationName} to {FilePath}", analysis.ApplicationName, filePath);

            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(analysis, options);
                await File.WriteAllTextAsync(filePath, json, cancellationToken);

                _logger.LogInformation("Successfully saved analysis for {ApplicationName}", analysis.ApplicationName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save analysis for {ApplicationName} to {FilePath}", 
                    analysis.ApplicationName, filePath);
                throw;
            }
        }

        #region Private Analysis Methods

        private async Task<FileSystemAnalysis> AnalyzeFileSystemAsync(
            string rootPath, 
            AnalysisConfiguration configuration, 
            CancellationToken cancellationToken)
        {
            var options = new CrawlOptions
            {
                IncludePatterns = configuration.IncludePatterns.Any() ? configuration.IncludePatterns : new List<string> { "*.*" },
                ExcludePatterns = configuration.ExcludePatterns.Any() ? configuration.ExcludePatterns : new List<string> { "bin/*", "obj/*", "*.tmp" },
                CalculateHashes = true,
                ExtractMetadata = true
            };

            return await _fileSystemCrawler.CrawlAsync(rootPath, options, cancellationToken);
        }

        private async Task<CodeAnalysis> AnalyzeCodeAsync(
            FileSystemAnalysis fileSystem, 
            AnalysisConfiguration configuration, 
            CancellationToken cancellationToken)
        {
            var options = new CodeAnalysisOptions
            {
                SupportedLanguages = configuration.LanguageExtensions.Any() ? configuration.LanguageExtensions : new List<string> { "csharp", "vb", "sql", "xml", "json" },
                ExtractDocumentation = true,
                CalculateMetrics = true,
                DetectPatterns = true,
                IncludePrivateMembers = true
            };

            return await _codeAnalysisEngine.AnalyzeAsync(fileSystem, options, cancellationToken);
        }

        private async Task<DependencyAnalysis> AnalyzeDependenciesAsync(
            CodeAnalysis codeAnalysis, 
            AnalysisConfiguration configuration, 
            CancellationToken cancellationToken)
        {
            var options = new DependencyOptions
            {
                ResolveStaticDependencies = configuration.AnalyzeStaticDependencies,
                ResolveDynamicDependencies = configuration.AnalyzeDynamicDependencies,
                ResolveExternalDependencies = true,
                ResolveDatabaseDependencies = configuration.AnalyzeDatabaseDependencies,
                DetectCircularDependencies = true
            };

            return await _dependencyResolver.ResolveAsync(codeAnalysis, options, cancellationToken);
        }

        private async Task<ArchitectureAnalysis> AnalyzeArchitectureAsync(
            CodeAnalysis codeAnalysis, 
            DependencyAnalysis dependencyAnalysis, 
            AnalysisConfiguration configuration, 
            CancellationToken cancellationToken)
        {
            var options = new ArchitectureOptions
            {
                DetectPatterns = true,
                DetectLayers = true,
                DetectComponents = true,
                DetectDesignPatterns = true,
                DetectViolations = true
            };

            return await _architectureAnalyzer.AnalyzeAsync(codeAnalysis, dependencyAnalysis, options, cancellationToken);
        }

        private async Task<RelationshipMatrix> MapRelationshipsAsync(
            CodeAnalysis codeAnalysis, 
            DependencyAnalysis dependencyAnalysis, 
            AnalysisConfiguration configuration, 
            CancellationToken cancellationToken)
        {
            var options = new Interfaces.RelationshipOptions
            {
                MapInheritance = true,
                MapComposition = true,
                MapAggregation = true,
                MapAssociation = true,
                MapMethodCalls = true,
                MapPropertyAccess = true,
                MapEvents = true,
                MaxCallDepth = 10
            };

            return await _relationshipMapper.MapRelationshipsAsync(codeAnalysis, dependencyAnalysis, options, cancellationToken);
        }

        private async Task<AnalysisMetrics> CalculateMetricsAsync(
            ApplicationAnalysis analysis, 
            AnalysisConfiguration configuration, 
            CancellationToken cancellationToken)
        {
            var options = new MetricsOptions
            {
                CalculateComplexity = true,
                CalculateQuality = true,
                CalculateCohesion = true,
                CalculateCoupling = true
            };

            return await _metricsCalculator.CalculateAsync(analysis, options, cancellationToken);
        }

        #endregion
    }
}
