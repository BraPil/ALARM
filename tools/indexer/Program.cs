using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Build.Locator;
using System.Text.Json.Serialization;

namespace ALARM.Indexer;

public class Program
{
    private static ILogger<Program>? _logger;
    
    public static async Task<int> Main(string[] args)
    {
        // Register MSBuild
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }

        var rootCommand = new RootCommand("ALARM Legacy App Indexer - Catalogs codebase symbols and dependencies");
        
        var legacyPathOption = new Option<string>(
            name: "--legacy-path",
            description: "Path to the legacy application codebase",
            getDefaultValue: () => Path.Combine(Directory.GetCurrentDirectory(), "app-legacy"));

        var outputPathOption = new Option<string>(
            name: "--output-path", 
            description: "Path to output the index files",
            getDefaultValue: () => Path.Combine(Directory.GetCurrentDirectory(), "mcp_runs", DateTime.Now.ToString("yyyyMMdd-HHmm")));

        var manifestPathOption = new Option<string>(
            name: "--manifest-path",
            description: "Path to the manifest files",
            getDefaultValue: () => Path.Combine(Directory.GetCurrentDirectory(), "mcp", "manifests"));

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Enable verbose logging");

        rootCommand.AddOption(legacyPathOption);
        rootCommand.AddOption(outputPathOption);
        rootCommand.AddOption(manifestPathOption);
        rootCommand.AddOption(verboseOption);

        rootCommand.SetHandler(async (legacyPath, outputPath, manifestPath, verbose) =>
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(verbose ? LogLevel.Debug : LogLevel.Information);
            });
            
            _logger = loggerFactory.CreateLogger<Program>();
            
            var indexer = new CodebaseIndexer(_logger);
            await indexer.IndexAsync(legacyPath, outputPath, manifestPath);
            
        }, legacyPathOption, outputPathOption, manifestPathOption, verboseOption);

        return await rootCommand.InvokeAsync(args);
    }
}

public class CodebaseIndexer
{
    private readonly ILogger<CodebaseIndexer> _logger;

    public CodebaseIndexer(ILogger<CodebaseIndexer> logger)
    {
        _logger = logger;
    }

    public async Task IndexAsync(string legacyPath, string outputPath, string manifestPath)
    {
        _logger.LogInformation("Starting codebase indexing...");
        _logger.LogInformation("Legacy Path: {LegacyPath}", legacyPath);
        _logger.LogInformation("Output Path: {OutputPath}", outputPath);
        _logger.LogInformation("Manifest Path: {ManifestPath}", manifestPath);

        // Ensure output directory exists
        Directory.CreateDirectory(outputPath);

        // Load manifests
        var fileManifest = await LoadFileManifestAsync(Path.Combine(manifestPath, "files.manifest.json"));
        var apiManifest = await LoadApiManifestAsync(Path.Combine(manifestPath, "apis.manifest.json"));

        // Discover files
        var discoveredFiles = await DiscoverFilesAsync(legacyPath, fileManifest);
        
        // Index solutions and projects
        var codeIndex = await IndexCodeAsync(legacyPath, discoveredFiles);
        
        // Analyze external API usage
        var externalApiUsage = await AnalyzeExternalApiUsageAsync(discoveredFiles, apiManifest);
        
        // Perform risk assessment
        var riskAssessment = await PerformRiskAssessmentAsync(codeIndex, externalApiUsage);

        // Generate outputs
        await GenerateIndexOutputsAsync(outputPath, codeIndex, externalApiUsage, riskAssessment, discoveredFiles);
        
        _logger.LogInformation("Indexing completed successfully!");
    }

    private async Task<FileManifest> LoadFileManifestAsync(string path)
    {
        if (!File.Exists(path))
        {
            _logger.LogWarning("File manifest not found at {Path}, using defaults", path);
            return new FileManifest();
        }

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<FileManifest>(json) ?? new FileManifest();
    }

    private async Task<ApiManifest> LoadApiManifestAsync(string path)
    {
        if (!File.Exists(path))
        {
            _logger.LogWarning("API manifest not found at {Path}, using defaults", path);
            return new ApiManifest();
        }

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<ApiManifest>(json) ?? new ApiManifest();
    }

    private async Task<DiscoveredFiles> DiscoverFilesAsync(string legacyPath, FileManifest manifest)
    {
        _logger.LogInformation("Discovering files in {LegacyPath}...", legacyPath);

        var discoveredFiles = new DiscoveredFiles();

        if (!Directory.Exists(legacyPath))
        {
            _logger.LogWarning("Legacy path does not exist: {LegacyPath}", legacyPath);
            return discoveredFiles;
        }

        // Discover all files
        var allFiles = Directory.GetFiles(legacyPath, "*.*", SearchOption.AllDirectories)
            .Where(f => !ShouldIgnoreFile(f, manifest.Ignore))
            .ToList();

        _logger.LogInformation("Found {FileCount} total files", allFiles.Count);

        // Categorize files by role
        foreach (var (role, patterns) in manifest.FileRoles)
        {
            var matchingFiles = new List<string>();
            
            foreach (var pattern in patterns)
            {
                var globPattern = pattern.Replace("**", "*");
                var regex = new System.Text.RegularExpressions.Regex(
                    "^" + System.Text.RegularExpressions.Regex.Escape(globPattern).Replace("\\*", ".*") + "$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                var matches = allFiles.Where(f => 
                {
                    var relativePath = Path.GetRelativePath(legacyPath, f).Replace('\\', '/');
                    return regex.IsMatch(relativePath);
                }).ToList();

                matchingFiles.AddRange(matches);
            }

            discoveredFiles.FilesByRole[role] = matchingFiles.Distinct().ToList();
            _logger.LogInformation("Found {Count} files for role '{Role}'", matchingFiles.Count, role);
        }

        // Categorize by file type
        discoveredFiles.CSharpFiles = allFiles.Where(f => f.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)).ToList();
        discoveredFiles.CppFiles = allFiles.Where(f => f.EndsWith(".cpp", StringComparison.OrdinalIgnoreCase) || 
                                                        f.EndsWith(".h", StringComparison.OrdinalIgnoreCase)).ToList();
        discoveredFiles.ConfigFiles = allFiles.Where(f => f.EndsWith(".config", StringComparison.OrdinalIgnoreCase) || 
                                                          f.EndsWith(".json", StringComparison.OrdinalIgnoreCase)).ToList();
        discoveredFiles.ProjectFiles = allFiles.Where(f => f.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || 
                                                           f.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)).ToList();

        return discoveredFiles;
    }

    private bool ShouldIgnoreFile(string filePath, List<string> ignorePatterns)
    {
        var fileName = Path.GetFileName(filePath);
        var relativePath = filePath;

        return ignorePatterns.Any(pattern => 
            fileName.Contains(pattern, StringComparison.OrdinalIgnoreCase) ||
            relativePath.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<CodeIndex> IndexCodeAsync(string legacyPath, DiscoveredFiles discoveredFiles)
    {
        _logger.LogInformation("Indexing C# code...");

        var codeIndex = new CodeIndex();

        // Find solution files
        var solutionFiles = discoveredFiles.ProjectFiles.Where(f => f.EndsWith(".sln")).ToList();
        
        if (solutionFiles.Any())
        {
            foreach (var solutionFile in solutionFiles)
            {
                try
                {
                    await IndexSolutionAsync(solutionFile, codeIndex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to index solution {SolutionFile}", solutionFile);
                }
            }
        }
        else
        {
            // Index individual project files
            var projectFiles = discoveredFiles.ProjectFiles.Where(f => f.EndsWith(".csproj")).ToList();
            foreach (var projectFile in projectFiles)
            {
                try
                {
                    await IndexProjectAsync(projectFile, codeIndex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to index project {ProjectFile}", projectFile);
                }
            }
        }

        _logger.LogInformation("Indexed {SymbolCount} symbols across {FileCount} files", 
            codeIndex.Symbols.Count, codeIndex.Files.Count);

        return codeIndex;
    }

    private async Task IndexSolutionAsync(string solutionPath, CodeIndex codeIndex)
    {
        _logger.LogDebug("Indexing solution: {SolutionPath}", solutionPath);

        try
        {
            using var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            foreach (var project in solution.Projects)
            {
                await IndexProjectAsync(project, codeIndex);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open solution {SolutionPath}", solutionPath);
        }
    }

    private async Task IndexProjectAsync(string projectPath, CodeIndex codeIndex)
    {
        _logger.LogDebug("Indexing project file: {ProjectPath}", projectPath);

        try
        {
            using var workspace = MSBuildWorkspace.Create();
            var project = await workspace.OpenProjectAsync(projectPath);
            await IndexProjectAsync(project, codeIndex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open project {ProjectPath}", projectPath);
        }
    }

    private async Task IndexProjectAsync(Project project, CodeIndex codeIndex)
    {
        _logger.LogDebug("Indexing project: {ProjectName}", project.Name);

        foreach (var document in project.Documents)
        {
            if (document.SourceCodeKind == SourceCodeKind.Regular)
            {
                await IndexDocumentAsync(document, codeIndex);
            }
        }
    }

    private async Task IndexDocumentAsync(Document document, CodeIndex codeIndex)
    {
        try
        {
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var semanticModel = await document.GetSemanticModelAsync();
            
            if (syntaxTree == null || semanticModel == null)
                return;

            var root = await syntaxTree.GetRootAsync();
            var filePath = document.FilePath ?? document.Name;

            var fileInfo = new FileInfo
            {
                Path = filePath,
                ProjectName = document.Project.Name,
                Language = "C#",
                LineCount = syntaxTree.GetText().Lines.Count
            };

            codeIndex.Files.Add(fileInfo);

            // Index symbols using a syntax walker
            var walker = new SymbolWalker(semanticModel, codeIndex, filePath);
            walker.Visit(root);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to index document {DocumentName}", document.Name);
        }
    }

    private async Task<ExternalApiUsage> AnalyzeExternalApiUsageAsync(DiscoveredFiles discoveredFiles, ApiManifest apiManifest)
    {
        _logger.LogInformation("Analyzing external API usage...");

        var usage = new ExternalApiUsage();

        foreach (var csFile in discoveredFiles.CSharpFiles)
        {
            try
            {
                var content = await File.ReadAllTextAsync(csFile);
                
                // Check for AutoCAD APIs
                foreach (var autocadApi in apiManifest.ExternalDependencies.GetValueOrDefault("autocad", new List<string>()))
                {
                    if (content.Contains(autocadApi, StringComparison.OrdinalIgnoreCase))
                    {
                        usage.AutoCadUsages.Add(new ApiUsage
                        {
                            FilePath = csFile,
                            ApiName = autocadApi,
                            LineNumber = GetLineNumber(content, autocadApi)
                        });
                    }
                }

                // Check for Oracle APIs
                foreach (var oracleApi in apiManifest.ExternalDependencies.GetValueOrDefault("oracle", new List<string>()))
                {
                    if (content.Contains(oracleApi, StringComparison.OrdinalIgnoreCase))
                    {
                        usage.OracleUsages.Add(new ApiUsage
                        {
                            FilePath = csFile,
                            ApiName = oracleApi,
                            LineNumber = GetLineNumber(content, oracleApi)
                        });
                    }
                }

                // Check for deprecated Framework APIs
                foreach (var frameworkApi in apiManifest.ExternalDependencies.GetValueOrDefault("framework", new List<string>()))
                {
                    if (content.Contains(frameworkApi, StringComparison.OrdinalIgnoreCase))
                    {
                        usage.FrameworkUsages.Add(new ApiUsage
                        {
                            FilePath = csFile,
                            ApiName = frameworkApi,
                            LineNumber = GetLineNumber(content, frameworkApi)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to analyze file {FilePath}", csFile);
            }
        }

        _logger.LogInformation("Found {AutoCadCount} AutoCAD, {OracleCount} Oracle, {FrameworkCount} Framework API usages",
            usage.AutoCadUsages.Count, usage.OracleUsages.Count, usage.FrameworkUsages.Count);

        return usage;
    }

    private int GetLineNumber(string content, string searchText)
    {
        var lines = content.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                return i + 1;
            }
        }
        return 1;
    }

    private async Task<RiskAssessment> PerformRiskAssessmentAsync(CodeIndex codeIndex, ExternalApiUsage externalApiUsage)
    {
        _logger.LogInformation("Performing risk assessment...");

        var risk = new RiskAssessment();

        // High risk: Files with many external API dependencies
        var filesWithManyExternalApis = externalApiUsage.AutoCadUsages
            .Concat(externalApiUsage.OracleUsages)
            .Concat(externalApiUsage.FrameworkUsages)
            .GroupBy(u => u.FilePath)
            .Where(g => g.Count() > 5)
            .Select(g => new RiskItem
            {
                FilePath = g.Key,
                RiskLevel = "High",
                Description = $"File has {g.Count()} external API dependencies",
                Category = "External Dependencies"
            });

        risk.HighRiskItems.AddRange(filesWithManyExternalApis);

        // Medium risk: Large files (>1000 lines)
        var largeFiles = codeIndex.Files
            .Where(f => f.LineCount > 1000)
            .Select(f => new RiskItem
            {
                FilePath = f.Path,
                RiskLevel = "Medium", 
                Description = $"Large file with {f.LineCount} lines",
                Category = "Code Complexity"
            });

        risk.MediumRiskItems.AddRange(largeFiles);

        // Low risk: Files with only framework API usage
        var frameworkOnlyFiles = externalApiUsage.FrameworkUsages
            .Select(u => u.FilePath)
            .Distinct()
            .Except(externalApiUsage.AutoCadUsages.Select(u => u.FilePath))
            .Except(externalApiUsage.OracleUsages.Select(u => u.FilePath))
            .Select(filePath => new RiskItem
            {
                FilePath = filePath,
                RiskLevel = "Low",
                Description = "Only framework API migration needed",
                Category = "Framework Migration"
            });

        risk.LowRiskItems.AddRange(frameworkOnlyFiles);

        _logger.LogInformation("Risk assessment complete: {HighRisk} high, {MediumRisk} medium, {LowRisk} low risk items",
            risk.HighRiskItems.Count, risk.MediumRiskItems.Count, risk.LowRiskItems.Count);

        return risk;
    }

    private async Task GenerateIndexOutputsAsync(string outputPath, CodeIndex codeIndex, ExternalApiUsage externalApiUsage, 
        RiskAssessment riskAssessment, DiscoveredFiles discoveredFiles)
    {
        _logger.LogInformation("Generating index outputs...");

        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Generate index.json
        var indexOutput = new
        {
            Timestamp = DateTime.UtcNow,
            Summary = new
            {
                TotalFiles = discoveredFiles.CSharpFiles.Count + discoveredFiles.CppFiles.Count + discoveredFiles.ConfigFiles.Count,
                CSharpFiles = discoveredFiles.CSharpFiles.Count,
                CppFiles = discoveredFiles.CppFiles.Count,
                ConfigFiles = discoveredFiles.ConfigFiles.Count,
                ProjectFiles = discoveredFiles.ProjectFiles.Count,
                TotalSymbols = codeIndex.Symbols.Count,
                ExternalApiUsages = externalApiUsage.AutoCadUsages.Count + externalApiUsage.OracleUsages.Count + externalApiUsage.FrameworkUsages.Count
            },
            Files = codeIndex.Files,
            Symbols = codeIndex.Symbols,
            FilesByRole = discoveredFiles.FilesByRole
        };

        await File.WriteAllTextAsync(Path.Combine(outputPath, "index.json"), 
            JsonSerializer.Serialize(indexOutput, options));

        // Generate external_apis.json
        await File.WriteAllTextAsync(Path.Combine(outputPath, "external_apis.json"),
            JsonSerializer.Serialize(externalApiUsage, options));

        // Generate risk_assessment.json
        await File.WriteAllTextAsync(Path.Combine(outputPath, "risk_assessment.json"),
            JsonSerializer.Serialize(riskAssessment, options));

        // Generate index.md
        var markdown = GenerateMarkdownSummary(codeIndex, externalApiUsage, riskAssessment, discoveredFiles);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "index.md"), markdown);

        _logger.LogInformation("Generated index outputs in {OutputPath}", outputPath);
    }

    private string GenerateMarkdownSummary(CodeIndex codeIndex, ExternalApiUsage externalApiUsage, 
        RiskAssessment riskAssessment, DiscoveredFiles discoveredFiles)
    {
        var totalFiles = discoveredFiles.CSharpFiles.Count + discoveredFiles.CppFiles.Count + discoveredFiles.ConfigFiles.Count;
        var totalApiUsages = externalApiUsage.AutoCadUsages.Count + externalApiUsage.OracleUsages.Count + externalApiUsage.FrameworkUsages.Count;

        return $@"# ALARM Codebase Index Report

Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC

## Summary

- **Total Files**: {totalFiles:N0}
  - C# Files: {discoveredFiles.CSharpFiles.Count:N0}
  - C++ Files: {discoveredFiles.CppFiles.Count:N0}
  - Config Files: {discoveredFiles.ConfigFiles.Count:N0}
  - Project Files: {discoveredFiles.ProjectFiles.Count:N0}

- **Code Analysis**:
  - Total Symbols: {codeIndex.Symbols.Count:N0}
  - External API Usages: {totalApiUsages:N0}

## File Distribution by Role

{string.Join("\n", discoveredFiles.FilesByRole.Select(kvp => $"- **{kvp.Key}**: {kvp.Value.Count} files"))}

## External API Usage

- **AutoCAD APIs**: {externalApiUsage.AutoCadUsages.Count:N0} usages
- **Oracle APIs**: {externalApiUsage.OracleUsages.Count:N0} usages  
- **Framework APIs**: {externalApiUsage.FrameworkUsages.Count:N0} usages

## Risk Assessment

- **High Risk Items**: {riskAssessment.HighRiskItems.Count:N0}
- **Medium Risk Items**: {riskAssessment.MediumRiskItems.Count:N0}
- **Low Risk Items**: {riskAssessment.LowRiskItems.Count:N0}

### High Risk Files
{string.Join("\n", riskAssessment.HighRiskItems.Take(10).Select(r => $"- `{Path.GetFileName(r.FilePath)}`: {r.Description}"))}

## Migration Recommendations

1. **Start with Low Risk Items**: {riskAssessment.LowRiskItems.Count} files need only framework API migration
2. **Address Medium Risk Items**: Focus on large files and complex components
3. **Careful Planning for High Risk**: Files with heavy external API usage need adapter patterns

## Next Steps

1. Run `[PLAN]` directive to generate detailed migration sequence
2. Create adapters for AutoCAD and Oracle integration
3. Begin with framework API migration using Roslyn analyzers
4. Implement comprehensive testing strategy

---
*Generated by ALARM MCP Indexer*";
    }
}
