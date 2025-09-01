using ALARM.Mapping.Core.Interfaces;
using ALARM.Mapping.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ALARM.Mapping.Core.Services
{
    /// <summary>
    /// Multi-language code analysis engine with Roslyn integration
    /// </summary>
    public class CodeAnalysisEngine : ICodeAnalysisEngine
    {
        private readonly ILogger<CodeAnalysisEngine> _logger;

        private static readonly Dictionary<string, string> LanguageMapping = new()
        {
            [".cs"] = "csharp",
            [".vb"] = "vb",
            [".sql"] = "sql",
            [".xml"] = "xml",
            [".json"] = "json",
            [".ps1"] = "powershell",
            [".psm1"] = "powershell",
            [".lsp"] = "autolisp",
            [".lisp"] = "autolisp",
            [".dcl"] = "autolisp", // AutoCAD Dialog Control Language
            [".config"] = "xml",
            [".resx"] = "xml",
            [".xaml"] = "xml"
        };

        public CodeAnalysisEngine(ILogger<CodeAnalysisEngine> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Analyzes code across all files in the file system analysis
        /// </summary>
        public async Task<CodeAnalysis> AnalyzeAsync(
            FileSystemAnalysis fileSystem, 
            CodeAnalysisOptions options, 
            CancellationToken cancellationToken = default)
        {
            return await AnalyzeAsync(fileSystem, options, null, cancellationToken);
        }

        /// <summary>
        /// Analyzes code with progress reporting
        /// </summary>
        public async Task<CodeAnalysis> AnalyzeAsync(
            FileSystemAnalysis fileSystem, 
            CodeAnalysisOptions options, 
            IProgress<CodeAnalysisProgress>? progress, 
            CancellationToken cancellationToken = default)
        {
            if (fileSystem == null)
                throw new ArgumentNullException(nameof(fileSystem));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _logger.LogInformation("Starting code analysis for {TotalFiles} files", fileSystem.TotalFiles);

            var analysis = new CodeAnalysis();
            var analysisProgress = new CodeAnalysisProgress();
            var allFiles = fileSystem.SourceFiles.Concat(fileSystem.ConfigurationFiles).ToList();

            try
            {
                // Filter files based on supported languages and size limits
                var analyzeableFiles = allFiles.Where(f => 
                    CanAnalyzeFile(f) && 
                    f.SizeBytes <= options.MaxFileSize &&
                    options.SupportedLanguages.Contains(GetLanguage(f.Extension))
                ).ToList();

                _logger.LogInformation("Analyzing {AnalyzeableFiles} out of {TotalFiles} files", 
                    analyzeableFiles.Count, allFiles.Count);

                // Process files by language
                var languageGroups = analyzeableFiles.GroupBy(f => GetLanguage(f.Extension));

                foreach (var languageGroup in languageGroups)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var language = languageGroup.Key;
                    var files = languageGroup.ToList();

                    _logger.LogInformation("Processing {FileCount} {Language} files", files.Count, language);

                    var languageAnalysis = new LanguageAnalysis
                    {
                        Language = language,
                        FileCount = files.Count
                    };

                    foreach (var file in files)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        try
                        {
                            var symbols = await ExtractSymbolsAsync(file, cancellationToken);
                            languageAnalysis.Symbols.AddRange(symbols);
                            languageAnalysis.LineCount += file.LineCount;

                            analysis.Symbols.AddRange(symbols);

                            analysisProgress.FilesProcessed++;
                            analysisProgress.SymbolsExtracted = analysis.Symbols.Count;
                            analysisProgress.CurrentFile = file.RelativePath;
                            analysisProgress.PercentComplete = (double)analysisProgress.FilesProcessed / analyzeableFiles.Count * 100;

                            progress?.Report(analysisProgress);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to analyze file {FilePath}", file.FullPath);
                        }
                    }

                    analysis.LanguageAnalysis[language] = languageAnalysis;
                }

                // Calculate totals and metrics
                await CalculateAnalysisMetricsAsync(analysis, options, cancellationToken);

                _logger.LogInformation("Code analysis complete. Extracted {TotalSymbols} symbols from {ProcessedFiles} files",
                    analysis.Symbols.Count, analysisProgress.FilesProcessed);

                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Code analysis failed");
                throw;
            }
        }

        /// <summary>
        /// Extracts symbols from a single file
        /// </summary>
        public async Task<List<CodeSymbol>> ExtractSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken = default)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var language = GetLanguage(file.Extension);
            
            return language switch
            {
                "csharp" => await ExtractCSharpSymbolsAsync(file, cancellationToken),
                "vb" => await ExtractVBSymbolsAsync(file, cancellationToken),
                "sql" => await ExtractSqlSymbolsAsync(file, cancellationToken),
                "xml" => await ExtractXmlSymbolsAsync(file, cancellationToken),
                "json" => await ExtractJsonSymbolsAsync(file, cancellationToken),
                "powershell" => await ExtractPowerShellSymbolsAsync(file, cancellationToken),
                "autolisp" => await ExtractAutoLispSymbolsAsync(file, cancellationToken),
                _ => new List<CodeSymbol>()
            };
        }

        /// <summary>
        /// Parses a file and returns syntax tree
        /// </summary>
        public async Task<Interfaces.SyntaxTree> ParseFileAsync(Models.FileInfo file, CancellationToken cancellationToken = default)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var language = GetLanguage(file.Extension);
            var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);

            var syntaxTree = new Interfaces.SyntaxTree
            {
                FilePath = file.FullPath,
                Language = language
            };

            try
            {
                switch (language)
                {
                    case "csharp":
                        syntaxTree.Root = CSharpSyntaxTree.ParseText(content, path: file.FullPath);
                        break;
                    case "vb":
                        syntaxTree.Root = VisualBasicSyntaxTree.ParseText(content, path: file.FullPath);
                        break;
                    default:
                        syntaxTree.Root = content; // Store raw content for non-Roslyn languages
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse file {FilePath}", file.FullPath);
                syntaxTree.Diagnostics.Add(new AnalysisWarning
                {
                    Level = WarningLevel.Error,
                    Category = "Parsing",
                    Message = ex.Message,
                    SourceFile = file.FullPath
                });
            }

            return syntaxTree;
        }

        /// <summary>
        /// Determines if a file can be analyzed
        /// </summary>
        public bool CanAnalyzeFile(Models.FileInfo file)
        {
            if (file == null) return false;
            
            var language = GetLanguage(file.Extension);
            return !string.IsNullOrEmpty(language) && LanguageMapping.ContainsKey(file.Extension.ToLowerInvariant());
        }

        #region Private Methods

        private string GetLanguage(string extension)
        {
            return LanguageMapping.GetValueOrDefault(extension.ToLowerInvariant(), string.Empty);
        }

        private async Task<List<CodeSymbol>> ExtractCSharpSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);
                var syntaxTree = CSharpSyntaxTree.ParseText(content, path: file.FullPath);
                var root = await syntaxTree.GetRootAsync(cancellationToken);

                var symbols = new List<CodeSymbol>();
                var walker = new CSharpSymbolWalker(file, symbols);
                walker.Visit(root);

                return symbols;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract C# symbols from {FilePath}", file.FullPath);
                return new List<CodeSymbol>();
            }
        }

        private async Task<List<CodeSymbol>> ExtractVBSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);
                var syntaxTree = VisualBasicSyntaxTree.ParseText(content, path: file.FullPath);
                var root = await syntaxTree.GetRootAsync(cancellationToken);

                var symbols = new List<CodeSymbol>();
                var walker = new VBSymbolWalker(file, symbols);
                walker.Visit(root);

                return symbols;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract VB symbols from {FilePath}", file.FullPath);
                return new List<CodeSymbol>();
            }
        }

        private async Task<List<CodeSymbol>> ExtractSqlSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);
                var symbols = new List<CodeSymbol>();

                // Simple SQL parsing - extract common objects
                var patterns = new Dictionary<SymbolType, Regex>
                {
                    [SymbolType.Class] = new Regex(@"CREATE\s+TABLE\s+(\w+)", RegexOptions.IgnoreCase),
                    [SymbolType.Method] = new Regex(@"CREATE\s+PROCEDURE\s+(\w+)", RegexOptions.IgnoreCase),
                    [SymbolType.Method] = new Regex(@"CREATE\s+FUNCTION\s+(\w+)", RegexOptions.IgnoreCase),
                    [SymbolType.Class] = new Regex(@"CREATE\s+VIEW\s+(\w+)", RegexOptions.IgnoreCase)
                };

                var lineNumber = 1;
                foreach (var line in content.Split('\n'))
                {
                    foreach (var (symbolType, pattern) in patterns)
                    {
                        var match = pattern.Match(line);
                        if (match.Success)
                        {
                            symbols.Add(new CodeSymbol
                            {
                                Name = match.Groups[1].Value,
                                FullName = match.Groups[1].Value,
                                Type = symbolType,
                                SourceFile = file.FullPath,
                                LineNumber = lineNumber,
                                AccessModifier = AccessModifier.Public
                            });
                        }
                    }
                    lineNumber++;
                }

                return symbols;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract SQL symbols from {FilePath}", file.FullPath);
                return new List<CodeSymbol>();
            }
        }

        private async Task<List<CodeSymbol>> ExtractXmlSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);
                var symbols = new List<CodeSymbol>();

                var doc = new XmlDocument();
                doc.LoadXml(content);

                // Extract root element and major nodes
                if (doc.DocumentElement != null)
                {
                    symbols.Add(new CodeSymbol
                    {
                        Name = doc.DocumentElement.Name,
                        FullName = doc.DocumentElement.Name,
                        Type = SymbolType.Class,
                        SourceFile = file.FullPath,
                        LineNumber = 1,
                        AccessModifier = AccessModifier.Public
                    });

                    // Extract child elements as properties
                    ExtractXmlElements(doc.DocumentElement, symbols, file.FullPath);
                }

                return symbols;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract XML symbols from {FilePath}", file.FullPath);
                return new List<CodeSymbol>();
            }
        }

        private void ExtractXmlElements(XmlNode node, List<CodeSymbol> symbols, string filePath)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    symbols.Add(new CodeSymbol
                    {
                        Name = child.Name,
                        FullName = child.Name,
                        Type = SymbolType.Property,
                        SourceFile = filePath,
                        AccessModifier = AccessModifier.Public
                    });

                    ExtractXmlElements(child, symbols, filePath);
                }
            }
        }

        private async Task<List<CodeSymbol>> ExtractJsonSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);
                var symbols = new List<CodeSymbol>();

                using var document = JsonDocument.Parse(content);
                ExtractJsonProperties(document.RootElement, symbols, file.FullPath, "Root");

                return symbols;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract JSON symbols from {FilePath}", file.FullPath);
                return new List<CodeSymbol>();
            }
        }

        private void ExtractJsonProperties(JsonElement element, List<CodeSymbol> symbols, string filePath, string parentName)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        symbols.Add(new CodeSymbol
                        {
                            Name = property.Name,
                            FullName = $"{parentName}.{property.Name}",
                            Type = SymbolType.Property,
                            SourceFile = filePath,
                            AccessModifier = AccessModifier.Public
                        });

                        ExtractJsonProperties(property.Value, symbols, filePath, $"{parentName}.{property.Name}");
                    }
                    break;
                case JsonValueKind.Array:
                    // Arrays don't add symbols but we process their elements
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        ExtractJsonProperties(item, symbols, filePath, $"{parentName}[{index}]");
                        index++;
                    }
                    break;
            }
        }

        private async Task<List<CodeSymbol>> ExtractPowerShellSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);
                var symbols = new List<CodeSymbol>();

                // Simple PowerShell parsing - extract functions and variables
                var functionPattern = new Regex(@"function\s+(\w+)", RegexOptions.IgnoreCase);
                var variablePattern = new Regex(@"\$(\w+)", RegexOptions.IgnoreCase);

                var lineNumber = 1;
                foreach (var line in content.Split('\n'))
                {
                    // Extract functions
                    var functionMatch = functionPattern.Match(line);
                    if (functionMatch.Success)
                    {
                        symbols.Add(new CodeSymbol
                        {
                            Name = functionMatch.Groups[1].Value,
                            FullName = functionMatch.Groups[1].Value,
                            Type = SymbolType.Method,
                            SourceFile = file.FullPath,
                            LineNumber = lineNumber,
                            AccessModifier = AccessModifier.Public
                        });
                    }

                    // Extract variables (limit to avoid too many)
                    var variableMatches = variablePattern.Matches(line);
                    foreach (Match match in variableMatches.Take(5)) // Limit to 5 per line
                    {
                        var variableName = match.Groups[1].Value;
                        if (!symbols.Any(s => s.Name == variableName && s.Type == SymbolType.Field))
                        {
                            symbols.Add(new CodeSymbol
                            {
                                Name = variableName,
                                FullName = $"${variableName}",
                                Type = SymbolType.Field,
                                SourceFile = file.FullPath,
                                LineNumber = lineNumber,
                                AccessModifier = AccessModifier.Public
                            });
                        }
                    }

                    lineNumber++;
                }

                return symbols;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract PowerShell symbols from {FilePath}", file.FullPath);
                return new List<CodeSymbol>();
            }
        }

        private async Task<List<CodeSymbol>> ExtractAutoLispSymbolsAsync(Models.FileInfo file, CancellationToken cancellationToken)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file.FullPath, cancellationToken);
                var symbols = new List<CodeSymbol>();

                // AutoLISP parsing - extract functions, variables, and commands
                var functionPattern = new Regex(@"\(defun\s+(\w+)", RegexOptions.IgnoreCase);
                var variablePattern = new Regex(@"\(setq\s+(\w+)", RegexOptions.IgnoreCase);
                var commandPattern = new Regex(@"\(command\s+""([^""]+)""", RegexOptions.IgnoreCase);
                var globalVarPattern = new Regex(@"\(\*(\w+)\*", RegexOptions.IgnoreCase);

                var lineNumber = 1;
                var lines = content.Split('\n');

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    
                    // Skip comments
                    if (trimmedLine.StartsWith(";"))
                    {
                        lineNumber++;
                        continue;
                    }

                    // Extract function definitions
                    var functionMatch = functionPattern.Match(line);
                    if (functionMatch.Success)
                    {
                        var functionName = functionMatch.Groups[1].Value;
                        symbols.Add(new CodeSymbol
                        {
                            Name = functionName,
                            FullName = functionName,
                            Type = SymbolType.Method,
                            SourceFile = file.FullPath,
                            LineNumber = lineNumber,
                            AccessModifier = AccessModifier.Public,
                            Metadata = new Dictionary<string, object> { ["Language"] = "AutoLISP" }
                        });
                    }

                    // Extract variable assignments (setq)
                    var variableMatches = variablePattern.Matches(line);
                    foreach (Match match in variableMatches.Take(3)) // Limit per line
                    {
                        var variableName = match.Groups[1].Value;
                        if (!symbols.Any(s => s.Name == variableName && s.Type == SymbolType.Field))
                        {
                            symbols.Add(new CodeSymbol
                            {
                                Name = variableName,
                                FullName = variableName,
                                Type = SymbolType.Field,
                                SourceFile = file.FullPath,
                                LineNumber = lineNumber,
                                AccessModifier = AccessModifier.Public,
                                Metadata = new Dictionary<string, object> { ["Language"] = "AutoLISP", ["Type"] = "Variable" }
                            });
                        }
                    }

                    // Extract AutoCAD commands
                    var commandMatches = commandPattern.Matches(line);
                    foreach (Match match in commandMatches.Take(2)) // Limit per line
                    {
                        var commandName = match.Groups[1].Value;
                        if (!symbols.Any(s => s.Name == commandName && s.Type == SymbolType.Method))
                        {
                            symbols.Add(new CodeSymbol
                            {
                                Name = commandName,
                                FullName = $"(command \"{commandName}\")",
                                Type = SymbolType.Method,
                                SourceFile = file.FullPath,
                                LineNumber = lineNumber,
                                AccessModifier = AccessModifier.Public,
                                Metadata = new Dictionary<string, object> { ["Language"] = "AutoLISP", ["Type"] = "AutoCAD Command" }
                            });
                        }
                    }

                    // Extract global variables (*variable*)
                    var globalVarMatches = globalVarPattern.Matches(line);
                    foreach (Match match in globalVarMatches.Take(2)) // Limit per line
                    {
                        var globalVarName = match.Groups[1].Value;
                        if (!symbols.Any(s => s.Name == globalVarName && s.Type == SymbolType.Field))
                        {
                            symbols.Add(new CodeSymbol
                            {
                                Name = globalVarName,
                                FullName = $"*{globalVarName}*",
                                Type = SymbolType.Field,
                                SourceFile = file.FullPath,
                                LineNumber = lineNumber,
                                AccessModifier = AccessModifier.Public,
                                Metadata = new Dictionary<string, object> { ["Language"] = "AutoLISP", ["Type"] = "Global Variable" }
                            });
                        }
                    }

                    lineNumber++;
                }

                return symbols;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract AutoLISP symbols from {FilePath}", file.FullPath);
                return new List<CodeSymbol>();
            }
        }

        private async Task CalculateAnalysisMetricsAsync(CodeAnalysis analysis, CodeAnalysisOptions options, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                // Calculate basic counts
                analysis.TotalClasses = analysis.Symbols.Count(s => s.Type == SymbolType.Class);
                analysis.TotalMethods = analysis.Symbols.Count(s => s.Type == SymbolType.Method);
                analysis.TotalProperties = analysis.Symbols.Count(s => s.Type == SymbolType.Property);
                analysis.TotalInterfaces = analysis.Symbols.Count(s => s.Type == SymbolType.Interface);
                analysis.TotalLinesOfCode = analysis.LanguageAnalysis.Values.Sum(la => la.LineCount);

                // Build namespaces
                var namespaces = analysis.Symbols
                    .Where(s => !string.IsNullOrEmpty(s.Namespace))
                    .GroupBy(s => s.Namespace)
                    .Select(g => new Namespace 
                    { 
                        Name = g.Key, 
                        Types = g.ToList() 
                    })
                    .ToList();

                analysis.Namespaces.AddRange(namespaces);

                // Basic complexity metrics
                if (options.CalculateMetrics)
                {
                    analysis.Complexity = new ComplexityMetrics
                    {
                        CyclomaticComplexity = CalculateAverageComplexity(analysis.Symbols),
                        CognitiveComplexity = CalculateAverageComplexity(analysis.Symbols) * 1.2, // Approximation
                        NestingDepth = 3, // Default approximation
                        Halstead = analysis.Symbols.Count * 2 // Simple approximation
                    };

                    analysis.Quality = new QualityMetrics
                    {
                        Maintainability = Math.Max(0, 100 - analysis.Complexity.CyclomaticComplexity * 2),
                        Testability = analysis.TotalMethods > 0 ? Math.Min(100, analysis.TotalMethods * 10.0 / analysis.TotalClasses) : 0,
                        Readability = analysis.Symbols.Count(s => !string.IsNullOrEmpty(s.Name) && s.Name.Length > 3) * 100.0 / Math.Max(1, analysis.Symbols.Count),
                        Documentation = 50 // Default approximation
                    };
                }
            }, cancellationToken);
        }

        private double CalculateAverageComplexity(List<CodeSymbol> symbols)
        {
            var methods = symbols.Where(s => s.Type == SymbolType.Method).ToList();
            if (!methods.Any()) return 1.0;

            // Simple complexity approximation based on method count and nesting
            return Math.Min(10, 1 + methods.Count * 0.1);
        }

        #endregion
    }
}
