using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace ALARM.ProtocolEngine;

public class Program
{
    private static ILogger<Program>? _logger;
    
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("ALARM Protocol Modification Engine - Applies protocol updates automatically");
        
        var protocolPathOption = new Option<string>(
            name: "--protocol-path",
            description: "Path to the protocol files directory",
            getDefaultValue: () => Path.Combine(Directory.GetCurrentDirectory(), "mcp", "protocols"));

        var updatesPathOption = new Option<string>(
            name: "--updates-path", 
            description: "Path to protocol update files",
            getDefaultValue: () => Path.Combine(Directory.GetCurrentDirectory(), "mcp_runs", "protocol_updates"));

        var outputPathOption = new Option<string>(
            name: "--output-path",
            description: "Path to output modified protocols",
            getDefaultValue: () => Path.Combine(Directory.GetCurrentDirectory(), "mcp_runs", DateTime.Now.ToString("yyyyMMdd-HHmm"), "updated_protocols"));

        var dryRunOption = new Option<bool>(
            name: "--dry-run",
            description: "Show what would be changed without applying");

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Enable verbose logging");

        var backupOption = new Option<bool>(
            name: "--backup",
            description: "Create backup before modifying protocols",
            getDefaultValue: () => true);

        rootCommand.AddOption(protocolPathOption);
        rootCommand.AddOption(updatesPathOption);
        rootCommand.AddOption(outputPathOption);
        rootCommand.AddOption(dryRunOption);
        rootCommand.AddOption(verboseOption);
        rootCommand.AddOption(backupOption);

        rootCommand.SetHandler(async (protocolPath, updatesPath, outputPath, dryRun, verbose, backup) =>
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(verbose ? LogLevel.Debug : LogLevel.Information);
            });
            
            _logger = loggerFactory.CreateLogger<Program>();
            
            var engine = new ProtocolModificationEngine(loggerFactory.CreateLogger<ProtocolModificationEngine>());
            await engine.ProcessUpdatesAsync(protocolPath, updatesPath, outputPath, dryRun, backup);
            
        }, protocolPathOption, updatesPathOption, outputPathOption, dryRunOption, verboseOption, backupOption);

        return await rootCommand.InvokeAsync(args);
    }
}

public class ProtocolModificationEngine
{
    private readonly ILogger<ProtocolModificationEngine> _logger;
    private readonly IDiffer _differ;
    private readonly ISerializer _yamlSerializer;
    private readonly IDeserializer _yamlDeserializer;

    public ProtocolModificationEngine(ILogger<ProtocolModificationEngine> logger)
    {
        _logger = logger;
        _differ = new Differ();
        _yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public async Task ProcessUpdatesAsync(string protocolPath, string updatesPath, string outputPath, bool dryRun, bool backup)
    {
        _logger.LogInformation("Starting protocol modification process...");
        _logger.LogInformation("Protocol Path: {ProtocolPath}", protocolPath);
        _logger.LogInformation("Updates Path: {UpdatesPath}", updatesPath);
        _logger.LogInformation("Output Path: {OutputPath}", outputPath);
        _logger.LogInformation("Dry Run: {DryRun}", dryRun);

        try
        {
            // 1. Load protocol update files
            var updateFiles = await LoadProtocolUpdatesAsync(updatesPath);
            if (!updateFiles.Any())
            {
                _logger.LogWarning("No protocol update files found in {UpdatesPath}", updatesPath);
                return;
            }

            // 2. Create output directory
            if (!dryRun)
            {
                Directory.CreateDirectory(outputPath);
            }

            // 3. Process each protocol update
            var modificationResults = new List<ProtocolModificationResult>();
            
            foreach (var updateFile in updateFiles)
            {
                var result = await ProcessProtocolUpdateAsync(protocolPath, updateFile, outputPath, dryRun, backup);
                modificationResults.Add(result);
            }

            // 4. Generate comprehensive report
            await GenerateModificationReportAsync(outputPath, modificationResults, dryRun);

            // 5. Validate all changes if not dry run
            if (!dryRun)
            {
                await ValidateModificationsAsync(outputPath, modificationResults);
            }

            _logger.LogInformation("Protocol modification process completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Protocol modification process failed");
            throw;
        }
    }

    private async Task<List<ProtocolUpdateFile>> LoadProtocolUpdatesAsync(string updatesPath)
    {
        _logger.LogInformation("Loading protocol update files from {UpdatesPath}...", updatesPath);
        
        var updateFiles = new List<ProtocolUpdateFile>();

        if (!Directory.Exists(updatesPath))
        {
            _logger.LogWarning("Updates path does not exist: {UpdatesPath}", updatesPath);
            return updateFiles;
        }

        var jsonFiles = Directory.GetFiles(updatesPath, "*protocol_updates*.json", SearchOption.AllDirectories);
        var yamlFiles = Directory.GetFiles(updatesPath, "*protocol_updates*.yml", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(updatesPath, "*protocol_updates*.yaml", SearchOption.AllDirectories));

        // Load JSON update files
        foreach (var jsonFile in jsonFiles)
        {
            try
            {
                var content = await File.ReadAllTextAsync(jsonFile);
                var updates = JsonSerializer.Deserialize<List<ProtocolUpdate>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (updates != null)
                {
                    updateFiles.Add(new ProtocolUpdateFile
                    {
                        FilePath = jsonFile,
                        Format = "json",
                        Updates = updates
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load JSON update file: {JsonFile}", jsonFile);
            }
        }

        // Load YAML update files
        foreach (var yamlFile in yamlFiles)
        {
            try
            {
                var content = await File.ReadAllTextAsync(yamlFile);
                var updates = _yamlDeserializer.Deserialize<List<ProtocolUpdate>>(content);

                if (updates != null)
                {
                    updateFiles.Add(new ProtocolUpdateFile
                    {
                        FilePath = yamlFile,
                        Format = "yaml",
                        Updates = updates
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load YAML update file: {YamlFile}", yamlFile);
            }
        }

        _logger.LogInformation("Loaded {UpdateCount} protocol update files with {TotalUpdates} total updates",
            updateFiles.Count, updateFiles.Sum(f => f.Updates.Count));

        return updateFiles;
    }

    private async Task<ProtocolModificationResult> ProcessProtocolUpdateAsync(string protocolPath, ProtocolUpdateFile updateFile, string outputPath, bool dryRun, bool backup)
    {
        _logger.LogInformation("Processing protocol update file: {UpdateFile}", Path.GetFileName(updateFile.FilePath));

        var result = new ProtocolModificationResult
        {
            UpdateFile = updateFile.FilePath,
            Timestamp = DateTime.UtcNow,
            DryRun = dryRun
        };

        foreach (var update in updateFile.Updates)
        {
            try
            {
                var protocolFile = FindProtocolFile(protocolPath, update.ProtocolName);
                if (protocolFile == null)
                {
                    var error = $"Protocol file not found for: {update.ProtocolName}";
                    _logger.LogError(error);
                    result.Errors.Add(error);
                    continue;
                }

                var modification = await ApplyProtocolUpdateAsync(protocolFile, update, outputPath, dryRun, backup);
                result.Modifications.Add(modification);
            }
            catch (Exception ex)
            {
                var error = $"Failed to process update for {update.ProtocolName}: {ex.Message}";
                _logger.LogError(ex, error);
                result.Errors.Add(error);
            }
        }

        return result;
    }

    private string? FindProtocolFile(string protocolPath, string protocolName)
    {
        // Try different naming patterns
        var possibleNames = new[]
        {
            $"{protocolName.ToLower()}_protocol.md",
            $"{protocolName.ToLower()}.protocol.md",
            $"{protocolName.Replace(" ", "_").ToLower()}.md",
            $"{protocolName.ToLower()}.md"
        };

        foreach (var name in possibleNames)
        {
            var fullPath = Path.Combine(protocolPath, name);
            if (File.Exists(fullPath))
            {
                return fullPath;
            }
        }

        // Search recursively
        var allMdFiles = Directory.GetFiles(protocolPath, "*.md", SearchOption.AllDirectories);
        return allMdFiles.FirstOrDefault(f => 
            Path.GetFileNameWithoutExtension(f).Contains(protocolName.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
    }

    private async Task<ProtocolModification> ApplyProtocolUpdateAsync(string protocolFile, ProtocolUpdate update, string outputPath, bool dryRun, bool backup)
    {
        _logger.LogDebug("Applying update to protocol: {ProtocolFile}", Path.GetFileName(protocolFile));

        var originalContent = await File.ReadAllTextAsync(protocolFile);
        var modifiedContent = originalContent;

        var modification = new ProtocolModification
        {
            ProtocolFile = protocolFile,
            UpdateType = update.UpdateType,
            Description = update.Description,
            OriginalContent = originalContent
        };

        // Apply the modification based on update type
        switch (update.UpdateType.ToLower())
        {
            case "enhancement":
                modifiedContent = ApplyEnhancement(originalContent, update);
                break;
            case "replacement":
                modifiedContent = ApplyReplacement(originalContent, update);
                break;
            case "insertion":
                modifiedContent = ApplyInsertion(originalContent, update);
                break;
            case "deletion":
                modifiedContent = ApplyDeletion(originalContent, update);
                break;
            default:
                throw new NotSupportedException($"Update type '{update.UpdateType}' is not supported");
        }

        modification.ModifiedContent = modifiedContent;

        // Generate diff
        var diffResult = _differ.CreateLineDiffs(originalContent, modifiedContent, false);
        var diffBuilder = new InlineDiffBuilder(_differ);
        var diffPane = diffBuilder.BuildDiffModel(originalContent, modifiedContent, false);
        modification.Diff = GenerateDiffSummary(diffPane);
        modification.HasChanges = !string.Equals(originalContent, modifiedContent, StringComparison.Ordinal);

        if (!dryRun && modification.HasChanges)
        {
            // Create backup if requested
            if (backup)
            {
                var backupPath = protocolFile + $".backup.{DateTime.Now:yyyyMMddHHmm}";
                await File.WriteAllTextAsync(backupPath, originalContent);
                modification.BackupPath = backupPath;
                _logger.LogDebug("Created backup: {BackupPath}", backupPath);
            }

            // Write modified content
            var outputFile = Path.Combine(outputPath, Path.GetFileName(protocolFile));
            await File.WriteAllTextAsync(outputFile, modifiedContent);
            modification.OutputPath = outputFile;
            _logger.LogInformation("Applied modification to: {OutputFile}", outputFile);
        }

        return modification;
    }

    private string ApplyEnhancement(string content, ProtocolUpdate update)
    {
        // Enhancement: Add new content to existing sections
        if (!string.IsNullOrEmpty(update.TargetSection))
        {
            var sectionPattern = $"## **{update.TargetSection}**";
            var sectionIndex = content.IndexOf(sectionPattern, StringComparison.OrdinalIgnoreCase);
            
            if (sectionIndex >= 0)
            {
                // Find the end of the section (next ## or end of file)
                var nextSectionIndex = content.IndexOf("\n## ", sectionIndex + sectionPattern.Length);
                var insertIndex = nextSectionIndex >= 0 ? nextSectionIndex : content.Length;
                
                return content.Insert(insertIndex, $"\n\n{update.ProposedChange}\n");
            }
        }
        
        // If no target section, append to end
        return content + $"\n\n{update.ProposedChange}\n";
    }

    private string ApplyReplacement(string content, ProtocolUpdate update)
    {
        // Replacement: Replace existing text with new text
        if (!string.IsNullOrEmpty(update.TargetText))
        {
            return content.Replace(update.TargetText, update.ProposedChange);
        }
        
        return content;
    }

    private string ApplyInsertion(string content, ProtocolUpdate update)
    {
        // Insertion: Insert new content at specific location
        if (!string.IsNullOrEmpty(update.TargetText))
        {
            var targetIndex = content.IndexOf(update.TargetText, StringComparison.OrdinalIgnoreCase);
            if (targetIndex >= 0)
            {
                return content.Insert(targetIndex + update.TargetText.Length, $"\n{update.ProposedChange}");
            }
        }
        
        return content + $"\n{update.ProposedChange}";
    }

    private string ApplyDeletion(string content, ProtocolUpdate update)
    {
        // Deletion: Remove specific content
        if (!string.IsNullOrEmpty(update.TargetText))
        {
            return content.Replace(update.TargetText, "");
        }
        
        return content;
    }

    private string GenerateDiffSummary(DiffPaneModel diffResult)
    {
        var summary = new List<string>();
        var linesAdded = diffResult.Lines.Count(l => l.Type == ChangeType.Inserted);
        var linesDeleted = diffResult.Lines.Count(l => l.Type == ChangeType.Deleted);
        var linesModified = diffResult.Lines.Count(l => l.Type == ChangeType.Modified);

        summary.Add($"Lines Added: {linesAdded}");
        summary.Add($"Lines Deleted: {linesDeleted}");
        summary.Add($"Lines Modified: {linesModified}");

        return string.Join(", ", summary);
    }

    private async Task GenerateModificationReportAsync(string outputPath, List<ProtocolModificationResult> results, bool dryRun)
    {
        _logger.LogInformation("Generating modification report...");

        var report = new ProtocolModificationReport
        {
            Timestamp = DateTime.UtcNow,
            DryRun = dryRun,
            Results = results,
            Summary = new ModificationSummary
            {
                TotalUpdateFiles = results.Count,
                TotalModifications = results.Sum(r => r.Modifications.Count),
                TotalErrors = results.Sum(r => r.Errors.Count),
                SuccessfulModifications = results.Sum(r => r.Modifications.Count(m => m.HasChanges)),
                ProtocolsModified = results.SelectMany(r => r.Modifications)
                    .Where(m => m.HasChanges)
                    .Select(m => Path.GetFileName(m.ProtocolFile))
                    .Distinct()
                    .ToList()
            }
        };

        // Generate JSON report
        var jsonOptions = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var jsonReport = JsonSerializer.Serialize(report, jsonOptions);
        var jsonPath = Path.Combine(outputPath, "protocol_modification_report.json");
        
        if (!dryRun)
        {
            await File.WriteAllTextAsync(jsonPath, jsonReport);
        }

        // Generate Markdown report
        var markdownReport = GenerateMarkdownReport(report);
        var markdownPath = Path.Combine(outputPath, "protocol_modification_report.md");
        
        if (!dryRun)
        {
            await File.WriteAllTextAsync(markdownPath, markdownReport);
        }

        _logger.LogInformation("Modification report generated: {JsonPath}, {MarkdownPath}", jsonPath, markdownPath);
    }

    private string GenerateMarkdownReport(ProtocolModificationReport report)
    {
        var md = new System.Text.StringBuilder();

        md.AppendLine("# Protocol Modification Report");
        md.AppendLine();
        md.AppendLine($"**Generated:** {report.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
        md.AppendLine($"**Dry Run:** {report.DryRun}");
        md.AppendLine();

        md.AppendLine("## Summary");
        md.AppendLine();
        md.AppendLine($"- **Update Files Processed:** {report.Summary.TotalUpdateFiles}");
        md.AppendLine($"- **Total Modifications:** {report.Summary.TotalModifications}");
        md.AppendLine($"- **Successful Modifications:** {report.Summary.SuccessfulModifications}");
        md.AppendLine($"- **Errors:** {report.Summary.TotalErrors}");
        md.AppendLine($"- **Protocols Modified:** {report.Summary.ProtocolsModified.Count}");
        md.AppendLine();

        if (report.Summary.ProtocolsModified.Any())
        {
            md.AppendLine("### Modified Protocols");
            md.AppendLine();
            foreach (var protocol in report.Summary.ProtocolsModified)
            {
                md.AppendLine($"- {protocol}");
            }
            md.AppendLine();
        }

        md.AppendLine("## Detailed Results");
        md.AppendLine();

        foreach (var result in report.Results)
        {
            md.AppendLine($"### {Path.GetFileName(result.UpdateFile)}");
            md.AppendLine();

            if (result.Errors.Any())
            {
                md.AppendLine("**Errors:**");
                foreach (var error in result.Errors)
                {
                    md.AppendLine($"- {error}");
                }
                md.AppendLine();
            }

            if (result.Modifications.Any())
            {
                md.AppendLine("**Modifications:**");
                foreach (var mod in result.Modifications)
                {
                    md.AppendLine($"- **{Path.GetFileName(mod.ProtocolFile)}** ({mod.UpdateType})");
                    md.AppendLine($"  - Description: {mod.Description}");
                    md.AppendLine($"  - Changes: {mod.Diff}");
                    md.AppendLine($"  - Modified: {mod.HasChanges}");
                    if (!string.IsNullOrEmpty(mod.BackupPath))
                    {
                        md.AppendLine($"  - Backup: {Path.GetFileName(mod.BackupPath)}");
                    }
                }
                md.AppendLine();
            }
        }

        md.AppendLine("---");
        md.AppendLine("*Generated by ALARM Protocol Modification Engine*");

        return md.ToString();
    }

    private async Task ValidateModificationsAsync(string outputPath, List<ProtocolModificationResult> results)
    {
        _logger.LogInformation("Validating protocol modifications...");

        var validationResults = new List<ValidationResult>();

        foreach (var result in results)
        {
            foreach (var modification in result.Modifications.Where(m => m.HasChanges))
            {
                var validation = await ValidateProtocolAsync(modification);
                validationResults.Add(validation);
            }
        }

        // Generate validation report
        var validationReport = new
        {
            Timestamp = DateTime.UtcNow,
            TotalValidations = validationResults.Count,
            PassedValidations = validationResults.Count(v => v.IsValid),
            FailedValidations = validationResults.Count(v => !v.IsValid),
            Results = validationResults
        };

        var jsonOptions = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var validationJson = JsonSerializer.Serialize(validationReport, jsonOptions);
        var validationPath = Path.Combine(outputPath, "protocol_validation_report.json");
        await File.WriteAllTextAsync(validationPath, validationJson);

        _logger.LogInformation("Validation completed: {Passed}/{Total} protocols passed validation", 
            validationReport.PassedValidations, validationReport.TotalValidations);
    }

    private async Task<ValidationResult> ValidateProtocolAsync(ProtocolModification modification)
    {
        var result = new ValidationResult
        {
            ProtocolFile = modification.ProtocolFile,
            Timestamp = DateTime.UtcNow
        };

        await Task.Delay(1); // Minimal async operation to satisfy async signature

        try
        {
            // Basic validation checks
            var content = modification.ModifiedContent;

            // Check for required sections
            var requiredSections = new[] { "Purpose", "Steps", "Completion Checklist" };
            foreach (var section in requiredSections)
            {
                if (!content.Contains(section, StringComparison.OrdinalIgnoreCase))
                {
                    result.Warnings.Add($"Missing recommended section: {section}");
                }
            }

            // Check for proper markdown formatting
            if (!content.Contains("##"))
            {
                result.Errors.Add("No markdown headers found");
            }

            // Check for anti-sampling compliance
            if (content.Contains("sample", StringComparison.OrdinalIgnoreCase) && 
                !content.Contains("no sampling", StringComparison.OrdinalIgnoreCase))
            {
                result.Warnings.Add("Potential sampling reference without anti-sampling directive");
            }

            // Check for quality gates references
            if (!content.Contains("quality gate", StringComparison.OrdinalIgnoreCase))
            {
                result.Warnings.Add("No quality gate references found");
            }

            result.IsValid = !result.Errors.Any();
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Validation failed: {ex.Message}");
            result.IsValid = false;
        }

        return result;
    }
}

// Data Models
public class ProtocolUpdate
{
    public string ProtocolName { get; set; } = "";
    public string UpdateType { get; set; } = ""; // enhancement, replacement, insertion, deletion
    public string Description { get; set; } = "";
    public string ProposedChange { get; set; } = "";
    public string Justification { get; set; } = "";
    public string EstimatedImpact { get; set; } = "";
    public string TargetSection { get; set; } = ""; // For targeted updates
    public string TargetText { get; set; } = ""; // For replacements/deletions
    public double Confidence { get; set; } = 1.0;
    public List<string> Prerequisites { get; set; } = new();
}

public class ProtocolUpdateFile
{
    public string FilePath { get; set; } = "";
    public string Format { get; set; } = ""; // json or yaml
    public List<ProtocolUpdate> Updates { get; set; } = new();
}

public class ProtocolModificationResult
{
    public string UpdateFile { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public bool DryRun { get; set; }
    public List<ProtocolModification> Modifications { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class ProtocolModification
{
    public string ProtocolFile { get; set; } = "";
    public string UpdateType { get; set; } = "";
    public string Description { get; set; } = "";
    public string OriginalContent { get; set; } = "";
    public string ModifiedContent { get; set; } = "";
    public string Diff { get; set; } = "";
    public bool HasChanges { get; set; }
    public string? BackupPath { get; set; }
    public string? OutputPath { get; set; }
}

public class ProtocolModificationReport
{
    public DateTime Timestamp { get; set; }
    public bool DryRun { get; set; }
    public List<ProtocolModificationResult> Results { get; set; } = new();
    public ModificationSummary Summary { get; set; } = new();
}

public class ModificationSummary
{
    public int TotalUpdateFiles { get; set; }
    public int TotalModifications { get; set; }
    public int TotalErrors { get; set; }
    public int SuccessfulModifications { get; set; }
    public List<string> ProtocolsModified { get; set; } = new();
}

public class ValidationResult
{
    public string ProtocolFile { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
