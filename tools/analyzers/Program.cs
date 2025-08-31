using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Build.Locator;

namespace ALARM.Analyzers;

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

        var rootCommand = new RootCommand("ALARM Analyzers - Continuous learning and improvement system");
        
        var targetPathOption = new Option<string>(
            name: "--target",
            description: "Path to the target codebase to analyze",
            getDefaultValue: () => "app-core");

        var outputPathOption = new Option<string>(
            name: "--output", 
            description: "Path to output analysis results",
            getDefaultValue: () => Path.Combine("mcp_runs", DateTime.Now.ToString("yyyyMMdd-HHmm"), "analysis"));

        var historyPathOption = new Option<string>(
            name: "--history-path",
            description: "Path to historical run data for learning",
            getDefaultValue: () => "mcp_runs");

        var learnModeOption = new Option<bool>(
            name: "--learn",
            description: "Enable machine learning analysis of patterns");

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Enable verbose logging");

        rootCommand.AddOption(targetPathOption);
        rootCommand.AddOption(outputPathOption);
        rootCommand.AddOption(historyPathOption);
        rootCommand.AddOption(learnModeOption);
        rootCommand.AddOption(verboseOption);

        rootCommand.SetHandler(async (targetPath, outputPath, historyPath, learnMode, verbose) =>
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(verbose ? LogLevel.Debug : LogLevel.Information);
            });
            
            _logger = loggerFactory.CreateLogger<Program>();
            
            var analyzer = new LearningAnalyzer(loggerFactory.CreateLogger<LearningAnalyzer>());
            await analyzer.AnalyzeAsync(targetPath, outputPath, historyPath, learnMode);
            
        }, targetPathOption, outputPathOption, historyPathOption, learnModeOption, verboseOption);

        return await rootCommand.InvokeAsync(args);
    }
}

public class LearningAnalyzer
{
    private readonly ILogger<LearningAnalyzer> _logger;

    public LearningAnalyzer(ILogger<LearningAnalyzer> logger)
    {
        _logger = logger;
    }

    public async Task AnalyzeAsync(string targetPath, string outputPath, string historyPath, bool enableLearning)
    {
        _logger.LogInformation("Starting ALARM learning analysis...");
        _logger.LogInformation("Target: {TargetPath}", targetPath);
        _logger.LogInformation("Output: {OutputPath}", outputPath);
        _logger.LogInformation("Learning Mode: {LearnMode}", enableLearning);

        Directory.CreateDirectory(outputPath);

        // 1. Analyze current codebase patterns
        var codePatterns = await AnalyzeCodePatternsAsync(targetPath);
        
        // 2. Load historical data for learning
        var historicalData = await LoadHistoricalDataAsync(historyPath);
        
        // 3. Perform success/failure pattern analysis
        var patternAnalysis = await AnalyzePatternsAsync(historicalData);
        
        // 4. Generate improvement suggestions
        var improvements = await GenerateImprovementsAsync(codePatterns, patternAnalysis, enableLearning);
        
        // 5. Update protocols based on learnings
        var protocolUpdates = await GenerateProtocolUpdatesAsync(improvements);
        
        // 6. Create performance baselines and trends
        var performanceAnalysis = await AnalyzePerformanceTrendsAsync(historicalData);
        
        // 7. Generate comprehensive report
        await GenerateAnalysisReportAsync(outputPath, codePatterns, patternAnalysis, improvements, protocolUpdates, performanceAnalysis);
        
        _logger.LogInformation("Analysis completed successfully!");
    }

    private async Task<CodePatternAnalysis> AnalyzeCodePatternsAsync(string targetPath)
    {
        _logger.LogInformation("Analyzing code patterns...");
        
        var analysis = new CodePatternAnalysis
        {
            Timestamp = DateTime.UtcNow,
            TargetPath = targetPath
        };

        if (!Directory.Exists(targetPath))
        {
            _logger.LogWarning("Target path does not exist: {TargetPath}", targetPath);
            return analysis;
        }

        // Analyze common patterns
        var csFiles = Directory.GetFiles(targetPath, "*.cs", SearchOption.AllDirectories);
        
        foreach (var file in csFiles)
        {
            try
            {
                var content = await File.ReadAllTextAsync(file);
                
                // Detect anti-patterns
                var antiPatterns = DetectAntiPatterns(content, file);
                analysis.AntiPatterns.AddRange(antiPatterns);
                
                // Detect good patterns
                var goodPatterns = DetectGoodPatterns(content, file);
                analysis.GoodPatterns.AddRange(goodPatterns);
                
                // Analyze complexity
                var complexity = AnalyzeComplexity(content, file);
                analysis.ComplexityMetrics.Add(file, complexity);
                
                // Detect external API usage
                var apiUsage = DetectExternalApiUsage(content, file);
                analysis.ExternalApiUsage.AddRange(apiUsage);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to analyze file: {File}", file);
            }
        }

        _logger.LogInformation("Found {AntiPatterns} anti-patterns and {GoodPatterns} good patterns", 
            analysis.AntiPatterns.Count, analysis.GoodPatterns.Count);

        return analysis;
    }

    private async Task<List<HistoricalRunData>> LoadHistoricalDataAsync(string historyPath)
    {
        _logger.LogInformation("Loading historical run data...");
        
        var historicalData = new List<HistoricalRunData>();

        if (!Directory.Exists(historyPath))
        {
            _logger.LogWarning("History path does not exist: {HistoryPath}", historyPath);
            return historicalData;
        }

        var runDirectories = Directory.GetDirectories(historyPath)
            .Where(d => Directory.GetFiles(d, "*.json").Any())
            .OrderBy(d => d)
            .ToList();

        foreach (var runDir in runDirectories)
        {
            try
            {
                var runData = await LoadRunDataAsync(runDir);
                if (runData != null)
                {
                    historicalData.Add(runData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load run data from: {RunDir}", runDir);
            }
        }

        _logger.LogInformation("Loaded {Count} historical runs", historicalData.Count);
        return historicalData;
    }

    private async Task<HistoricalRunData?> LoadRunDataAsync(string runDirectory)
    {
        var runData = new HistoricalRunData
        {
            RunId = Path.GetFileName(runDirectory),
            Timestamp = ExtractTimestampFromPath(runDirectory)
        };

        // Load various data files
        var indexFile = Path.Combine(runDirectory, "index.json");
        if (File.Exists(indexFile))
        {
            var indexContent = await File.ReadAllTextAsync(indexFile);
            runData.IndexData = JsonSerializer.Deserialize<Dictionary<string, object>>(indexContent) ?? new();
        }

        var riskFile = Path.Combine(runDirectory, "risk_assessment.json");
        if (File.Exists(riskFile))
        {
            var riskContent = await File.ReadAllTextAsync(riskFile);
            runData.RiskAssessment = JsonSerializer.Deserialize<Dictionary<string, object>>(riskContent) ?? new();
        }

        // Load test results if available
        var testResultsPattern = Path.Combine(runDirectory, "*test*.json");
        var testFiles = Directory.GetFiles(Path.GetDirectoryName(testResultsPattern) ?? "", Path.GetFileName(testResultsPattern));
        foreach (var testFile in testFiles)
        {
            var testContent = await File.ReadAllTextAsync(testFile);
            var testData = JsonSerializer.Deserialize<Dictionary<string, object>>(testContent) ?? new();
            runData.TestResults[Path.GetFileName(testFile)] = testData;
        }

        return runData.IndexData.Any() || runData.RiskAssessment.Any() || runData.TestResults.Any() ? runData : null;
    }

    private DateTime ExtractTimestampFromPath(string path)
    {
        var dirName = Path.GetFileName(path);
        if (DateTime.TryParseExact(dirName, "yyyyMMdd-HHmm", null, System.Globalization.DateTimeStyles.None, out var timestamp))
        {
            return timestamp;
        }
        return Directory.GetCreationTime(path);
    }

    private async Task<PatternAnalysisResult> AnalyzePatternsAsync(List<HistoricalRunData> historicalData)
    {
        _logger.LogInformation("Analyzing success/failure patterns...");
        
        var result = new PatternAnalysisResult
        {
            AnalysisTimestamp = DateTime.UtcNow,
            TotalRuns = historicalData.Count
        };

        if (!historicalData.Any())
        {
            _logger.LogWarning("No historical data available for pattern analysis");
            return result;
        }

        // Analyze success patterns
        var successfulRuns = historicalData.Where(IsSuccessfulRun).ToList();
        var failedRuns = historicalData.Where(r => !IsSuccessfulRun(r)).ToList();

        result.SuccessRate = historicalData.Count > 0 ? (double)successfulRuns.Count / historicalData.Count : 0;

        // Find common patterns in successful runs
        result.SuccessPatterns = FindCommonPatterns(successfulRuns, "Success patterns");
        
        // Find common patterns in failed runs
        result.FailurePatterns = FindCommonPatterns(failedRuns, "Failure patterns");
        
        // Analyze trends over time
        result.Trends = AnalyzeTrends(historicalData);
        
        // Generate insights
        result.Insights = GenerateInsights(result);

        _logger.LogInformation("Pattern analysis complete: {SuccessRate:P1} success rate over {TotalRuns} runs", 
            result.SuccessRate, result.TotalRuns);

        return result;
    }

    private bool IsSuccessfulRun(HistoricalRunData runData)
    {
        // Determine success based on available metrics
        if (runData.TestResults.Any())
        {
            // Check if tests passed
            foreach (var testResult in runData.TestResults.Values)
            {
                if (testResult.TryGetValue("success", out var successValue) && 
                    successValue is bool success && !success)
                {
                    return false;
                }
            }
        }

        // Check risk assessment - high number of high-risk items indicates problems
        if (runData.RiskAssessment.TryGetValue("HighRiskItems", out var highRiskValue) &&
            highRiskValue is JsonElement highRiskElement && 
            highRiskElement.ValueKind == JsonValueKind.Array &&
            highRiskElement.GetArrayLength() > 10)
        {
            return false;
        }

        return true; // Default to success if no clear failure indicators
    }

    private List<string> FindCommonPatterns(List<HistoricalRunData> runs, string patternType)
    {
        var patterns = new List<string>();
        
        if (!runs.Any()) return patterns;

        // Analyze common characteristics
        var commonFeatures = new Dictionary<string, int>();

        foreach (var run in runs)
        {
            // Extract features from run data
            var features = ExtractFeatures(run);
            foreach (var feature in features)
            {
                commonFeatures[feature] = commonFeatures.GetValueOrDefault(feature, 0) + 1;
            }
        }

        // Find patterns that appear in majority of runs
        var threshold = runs.Count * 0.6; // 60% threshold
        patterns.AddRange(commonFeatures
            .Where(kv => kv.Value >= threshold)
            .Select(kv => $"{patternType}: {kv.Key} (appears in {kv.Value}/{runs.Count} runs)")
            .ToList());

        return patterns;
    }

    private List<string> ExtractFeatures(HistoricalRunData runData)
    {
        var features = new List<string>();

        // Extract features from index data
        if (runData.IndexData.TryGetValue("Summary", out var summaryValue) && summaryValue is JsonElement summary)
        {
            if (summary.TryGetProperty("TotalFiles", out var totalFiles))
            {
                var fileCount = totalFiles.GetInt32();
                features.Add($"FileCount_{GetSizeCategory(fileCount)}");
            }
            
            if (summary.TryGetProperty("ExternalApiUsages", out var apiUsages))
            {
                var usageCount = apiUsages.GetInt32();
                features.Add($"ApiUsage_{GetSizeCategory(usageCount)}");
            }
        }

        // Extract features from risk assessment
        if (runData.RiskAssessment.TryGetValue("HighRiskItems", out var highRiskValue) && 
            highRiskValue is JsonElement highRisk && highRisk.ValueKind == JsonValueKind.Array)
        {
            features.Add($"HighRisk_{GetSizeCategory(highRisk.GetArrayLength())}");
        }

        return features;
    }

    private string GetSizeCategory(int count)
    {
        return count switch
        {
            < 10 => "Small",
            < 100 => "Medium",
            < 1000 => "Large",
            _ => "XLarge"
        };
    }

    private Dictionary<string, object> AnalyzeTrends(List<HistoricalRunData> historicalData)
    {
        var trends = new Dictionary<string, object>();

        if (historicalData.Count < 2) return trends;

        var sortedData = historicalData.OrderBy(r => r.Timestamp).ToList();

        // Analyze success rate trend
        var recentRuns = sortedData.TakeLast(10).ToList();
        var olderRuns = sortedData.Take(Math.Max(1, sortedData.Count - 10)).ToList();

        var recentSuccessRate = recentRuns.Count(IsSuccessfulRun) / (double)recentRuns.Count;
        var olderSuccessRate = olderRuns.Any() ? olderRuns.Count(IsSuccessfulRun) / (double)olderRuns.Count : recentSuccessRate;

        trends["SuccessRateTrend"] = recentSuccessRate - olderSuccessRate;
        trends["RecentSuccessRate"] = recentSuccessRate;
        trends["HistoricalSuccessRate"] = olderSuccessRate;

        // Analyze complexity trends
        var complexityTrend = AnalyzeComplexityTrend(sortedData);
        trends["ComplexityTrend"] = complexityTrend;

        return trends;
    }

    private string AnalyzeComplexityTrend(List<HistoricalRunData> sortedData)
    {
        // Simplified complexity trend analysis
        var recentComplexity = GetAverageComplexity(sortedData.TakeLast(5));
        var olderComplexity = GetAverageComplexity(sortedData.Take(5));

        if (recentComplexity > olderComplexity * 1.1)
            return "Increasing";
        else if (recentComplexity < olderComplexity * 0.9)
            return "Decreasing";
        else
            return "Stable";
    }

    private double GetAverageComplexity(IEnumerable<HistoricalRunData> runs)
    {
        var complexities = new List<double>();
        
        foreach (var run in runs)
        {
            if (run.IndexData.TryGetValue("Summary", out var summaryValue) && summaryValue is JsonElement summary)
            {
                if (summary.TryGetProperty("TotalSymbols", out var symbols))
                {
                    complexities.Add(symbols.GetDouble());
                }
            }
        }

        return complexities.Any() ? complexities.Average() : 0;
    }

    private List<string> GenerateInsights(PatternAnalysisResult result)
    {
        var insights = new List<string>();

        if (result.SuccessRate < 0.8)
        {
            insights.Add($"Success rate is below 80% ({result.SuccessRate:P1}). Consider reviewing failure patterns.");
        }

        if (result.FailurePatterns.Any())
        {
            insights.Add($"Identified {result.FailurePatterns.Count} failure patterns. Focus on addressing these issues.");
        }

        if (result.Trends.TryGetValue("SuccessRateTrend", out var trendValue) && trendValue is double trend)
        {
            if (trend < -0.1)
            {
                insights.Add("Success rate is declining. Immediate attention required.");
            }
            else if (trend > 0.1)
            {
                insights.Add("Success rate is improving. Current practices are effective.");
            }
        }

        return insights;
    }

    private async Task<ImprovementSuggestions> GenerateImprovementsAsync(CodePatternAnalysis codePatterns, PatternAnalysisResult patternAnalysis, bool enableLearning)
    {
        _logger.LogInformation("Generating improvement suggestions...");
        
        var suggestions = new ImprovementSuggestions
        {
            GeneratedAt = DateTime.UtcNow,
            LearningEnabled = enableLearning
        };

        // Generate suggestions based on anti-patterns
        foreach (var antiPattern in codePatterns.AntiPatterns)
        {
            suggestions.CodeImprovements.Add(new ImprovementSuggestion
            {
                Type = "AntiPattern",
                Priority = GetPriorityForAntiPattern(antiPattern.Pattern),
                Description = $"Address {antiPattern.Pattern} in {antiPattern.File}",
                Recommendation = GetRecommendationForAntiPattern(antiPattern.Pattern),
                EstimatedEffort = EstimateEffort(antiPattern.Pattern)
            });
        }

        // Generate suggestions based on failure patterns
        foreach (var failurePattern in patternAnalysis.FailurePatterns)
        {
            suggestions.ProcessImprovements.Add(new ImprovementSuggestion
            {
                Type = "ProcessImprovement",
                Priority = "High",
                Description = $"Address recurring failure: {failurePattern}",
                Recommendation = GenerateProcessRecommendation(failurePattern),
                EstimatedEffort = "Medium"
            });
        }

        // Generate ML-based suggestions if enabled
        if (enableLearning)
        {
            var mlSuggestions = await GenerateMLSuggestionsAsync(codePatterns, patternAnalysis, _logger);
            suggestions.MLSuggestions.AddRange(mlSuggestions);
        }

        _logger.LogInformation("Generated {CodeImprovements} code improvements and {ProcessImprovements} process improvements",
            suggestions.CodeImprovements.Count, suggestions.ProcessImprovements.Count);

        return suggestions;
    }

    private async Task<List<ImprovementSuggestion>> GenerateMLSuggestionsAsync(CodePatternAnalysis codePatterns, PatternAnalysisResult patternAnalysis, ILogger<LearningAnalyzer> logger)
    {
        var suggestions = new List<ImprovementSuggestion>();

        try
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var mlEngine = new MLEngine(loggerFactory.CreateLogger<MLEngine>());
            
            // Create project context from current analysis
            var context = new ProjectContext
            {
                FileCount = codePatterns.ComplexityMetrics.Count,
                ComplexityScore = (float)codePatterns.ComplexityMetrics.Values.Average(m => m.CyclomaticComplexity),
                ApiUsageCount = codePatterns.ExternalApiUsage.Count,
                TestCoverage = 0.6f, // Would extract from actual test data
                PreviousSuccessRate = (float)patternAnalysis.SuccessRate,
                TeamExperience = 0.7f, // Would come from team metadata
                TechnicalDebt = codePatterns.AntiPatterns.Count * 0.5f
            };

            // Get historical data (would normally load from persistence layer)
            var historicalData = new List<HistoricalRunData>(); // Placeholder

            // Predict success probability
            var successPrediction = await mlEngine.PredictSuccessProbabilityAsync(context, historicalData);
            
            if (successPrediction.SuccessProbability < 0.7)
            {
                suggestions.Add(new ImprovementSuggestion
                {
                    Type = "MLRecommendation",
                    Priority = "Critical",
                    Description = $"ML model predicts {successPrediction.SuccessProbability:P1} success probability",
                    Recommendation = $"Key risk factors: {string.Join(", ", successPrediction.Factors)}",
                    EstimatedEffort = "High",
                    Confidence = successPrediction.Confidence
                });
            }

            // Analyze patterns with ML
            var mlPatternAnalysis = await mlEngine.AnalyzePatternsWithMLAsync(historicalData);
            
            foreach (var insight in mlPatternAnalysis.Insights)
            {
                suggestions.Add(new ImprovementSuggestion
                {
                    Type = "MLPatternInsight",
                    Priority = "Medium",
                    Description = insight,
                    Recommendation = "Consider implementing pattern-based improvements",
                    EstimatedEffort = "Medium",
                    Confidence = 0.8
                });
            }

            // Performance optimization suggestions
            var optimizationResult = await mlEngine.OptimizeParametersAsync("indexer", "large-codebase", historicalData);
            
            if (optimizationResult.ExpectedImprovement > 0.1)
            {
                suggestions.Add(new ImprovementSuggestion
                {
                    Type = "MLOptimization",
                    Priority = "Medium",
                    Description = $"ML suggests {optimizationResult.ExpectedImprovement:P1} performance improvement",
                    Recommendation = $"Apply optimized parameters: {string.Join(", ", optimizationResult.OptimizedParameters.Keys)}",
                    EstimatedEffort = "Low",
                    Confidence = optimizationResult.Confidence
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ML suggestions generation failed");
            suggestions.Add(new ImprovementSuggestion
            {
                Type = "MLError",
                Priority = "Low",
                Description = "ML analysis unavailable",
                Recommendation = "Continue with heuristic-based analysis",
                EstimatedEffort = "None",
                Confidence = 1.0
            });
        }

        return suggestions;
    }

    private async Task<List<ProtocolUpdate>> GenerateProtocolUpdatesAsync(ImprovementSuggestions improvements)
    {
        _logger.LogInformation("Generating protocol updates...");
        
        var updates = new List<ProtocolUpdate>();

        // Generate updates based on high-priority improvements
        var highPriorityImprovements = improvements.CodeImprovements
            .Concat(improvements.ProcessImprovements)
            .Where(i => i.Priority == "Critical" || i.Priority == "High")
            .ToList();

        foreach (var improvement in highPriorityImprovements)
        {
            var update = new ProtocolUpdate
            {
                ProtocolName = DetermineAffectedProtocol(improvement.Type),
                UpdateType = "Enhancement",
                Description = $"Update based on improvement: {improvement.Description}",
                ProposedChange = improvement.Recommendation,
                Justification = $"Addresses {improvement.Type} with {improvement.Priority} priority",
                EstimatedImpact = improvement.EstimatedEffort
            };

            updates.Add(update);
        }

        await Task.Delay(10); // Simulate processing
        return updates;
    }

    private async Task<PerformanceAnalysis> AnalyzePerformanceTrendsAsync(List<HistoricalRunData> historicalData)
    {
        _logger.LogInformation("Analyzing performance trends...");
        
        var analysis = new PerformanceAnalysis
        {
            AnalysisTimestamp = DateTime.UtcNow
        };

        if (!historicalData.Any())
        {
            _logger.LogWarning("No historical data for performance analysis");
            return analysis;
        }

        // Analyze performance metrics over time
        var sortedData = historicalData.OrderBy(r => r.Timestamp).ToList();
        
        // Extract performance metrics
        foreach (var run in sortedData)
        {
            var metrics = ExtractPerformanceMetrics(run);
            analysis.HistoricalMetrics.Add(run.Timestamp, metrics);
        }

        // Calculate trends
        analysis.Trends = CalculatePerformanceTrends(analysis.HistoricalMetrics);
        
        // Generate baselines
        analysis.Baselines = GeneratePerformanceBaselines(analysis.HistoricalMetrics);
        
        // Identify regressions
        analysis.Regressions = IdentifyPerformanceRegressions(analysis.HistoricalMetrics);

        await Task.Delay(10); // Simulate processing
        return analysis;
    }

    private Dictionary<string, double> ExtractPerformanceMetrics(HistoricalRunData runData)
    {
        var metrics = new Dictionary<string, double>();

        // Extract metrics from test results
        foreach (var testResult in runData.TestResults.Values)
        {
            if (testResult.TryGetValue("Duration", out var durationValue) && durationValue is JsonElement duration)
            {
                if (duration.ValueKind == JsonValueKind.Number)
                {
                    metrics["TestDuration"] = duration.GetDouble();
                }
            }

            if (testResult.TryGetValue("Metrics", out var metricsValue) && metricsValue is JsonElement metricsElement)
            {
                foreach (var metric in metricsElement.EnumerateObject())
                {
                    if (metric.Value.ValueKind == JsonValueKind.Number)
                    {
                        metrics[metric.Name] = metric.Value.GetDouble();
                    }
                }
            }
        }

        return metrics;
    }

    private Dictionary<string, string> CalculatePerformanceTrends(Dictionary<DateTime, Dictionary<string, double>> historicalMetrics)
    {
        var trends = new Dictionary<string, string>();

        if (historicalMetrics.Count < 2) return trends;

        var sortedMetrics = historicalMetrics.OrderBy(kv => kv.Key).ToList();
        var recentMetrics = sortedMetrics.TakeLast(5).ToList();
        var olderMetrics = sortedMetrics.Take(5).ToList();

        // Calculate trends for each metric
        var allMetricNames = historicalMetrics.Values.SelectMany(m => m.Keys).Distinct();

        foreach (var metricName in allMetricNames)
        {
            var recentAvg = recentMetrics
                .Where(m => m.Value.ContainsKey(metricName))
                .Average(m => m.Value[metricName]);

            var olderAvg = olderMetrics
                .Where(m => m.Value.ContainsKey(metricName))
                .Average(m => m.Value[metricName]);

            if (olderAvg > 0)
            {
                var change = (recentAvg - olderAvg) / olderAvg;
                trends[metricName] = change switch
                {
                    > 0.1 => "Degrading",
                    < -0.1 => "Improving", 
                    _ => "Stable"
                };
            }
        }

        return trends;
    }

    private Dictionary<string, double> GeneratePerformanceBaselines(Dictionary<DateTime, Dictionary<string, double>> historicalMetrics)
    {
        var baselines = new Dictionary<string, double>();

        if (!historicalMetrics.Any()) return baselines;

        var allMetricNames = historicalMetrics.Values.SelectMany(m => m.Keys).Distinct();

        foreach (var metricName in allMetricNames)
        {
            var values = historicalMetrics.Values
                .Where(m => m.ContainsKey(metricName))
                .Select(m => m[metricName])
                .ToList();

            if (values.Any())
            {
                // Use 95th percentile as baseline
                values.Sort();
                var percentileIndex = (int)(values.Count * 0.95);
                baselines[metricName] = values[Math.Min(percentileIndex, values.Count - 1)];
            }
        }

        return baselines;
    }

    private List<PerformanceRegression> IdentifyPerformanceRegressions(Dictionary<DateTime, Dictionary<string, double>> historicalMetrics)
    {
        var regressions = new List<PerformanceRegression>();

        if (historicalMetrics.Count < 2) return regressions;

        var sortedMetrics = historicalMetrics.OrderBy(kv => kv.Key).ToList();
        var latest = sortedMetrics.Last();
        var baseline = CalculateBaseline(sortedMetrics.Take(sortedMetrics.Count - 1));

        foreach (var metric in latest.Value)
        {
            if (baseline.TryGetValue(metric.Key, out var baselineValue))
            {
                var regression = (metric.Value - baselineValue) / baselineValue;
                if (regression > 0.2) // 20% regression threshold
                {
                    regressions.Add(new PerformanceRegression
                    {
                        MetricName = metric.Key,
                        BaselineValue = baselineValue,
                        CurrentValue = metric.Value,
                        RegressionPercentage = regression * 100,
                        DetectedAt = latest.Key
                    });
                }
            }
        }

        return regressions;
    }

    private Dictionary<string, double> CalculateBaseline(IEnumerable<KeyValuePair<DateTime, Dictionary<string, double>>> historicalData)
    {
        var baseline = new Dictionary<string, double>();
        var dataList = historicalData.ToList();

        if (!dataList.Any()) return baseline;

        var allMetricNames = dataList.SelectMany(kv => kv.Value.Keys).Distinct();

        foreach (var metricName in allMetricNames)
        {
            var values = dataList
                .Where(kv => kv.Value.ContainsKey(metricName))
                .Select(kv => kv.Value[metricName])
                .ToList();

            if (values.Any())
            {
                baseline[metricName] = values.Average();
            }
        }

        return baseline;
    }

    private async Task GenerateAnalysisReportAsync(string outputPath, CodePatternAnalysis codePatterns, 
        PatternAnalysisResult patternAnalysis, ImprovementSuggestions improvements, 
        List<ProtocolUpdate> protocolUpdates, PerformanceAnalysis performanceAnalysis)
    {
        _logger.LogInformation("Generating comprehensive analysis report...");

        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Generate detailed JSON report
        var report = new
        {
            GeneratedAt = DateTime.UtcNow,
            CodePatterns = codePatterns,
            PatternAnalysis = patternAnalysis,
            Improvements = improvements,
            ProtocolUpdates = protocolUpdates,
            PerformanceAnalysis = performanceAnalysis
        };

        await File.WriteAllTextAsync(Path.Combine(outputPath, "learning_analysis.json"), 
            JsonSerializer.Serialize(report, options));

        // Generate executive summary
        var summary = GenerateExecutiveSummary(patternAnalysis, improvements, performanceAnalysis);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "executive_summary.md"), summary);

        // Generate actionable recommendations
        var recommendations = GenerateActionableRecommendations(improvements, protocolUpdates);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "recommendations.md"), recommendations);

        _logger.LogInformation("Analysis report generated in {OutputPath}", outputPath);
    }

    // Helper methods for pattern detection
    private List<AntiPattern> DetectAntiPatterns(string content, string filePath)
    {
        var patterns = new List<AntiPattern>();

        // Detect common anti-patterns
        if (content.Contains("catch (Exception ex)") && !content.Contains("_logger"))
        {
            patterns.Add(new AntiPattern { Pattern = "UnloggedExceptions", File = filePath, Severity = "Medium" });
        }

        if (content.Contains("Thread.Sleep"))
        {
            patterns.Add(new AntiPattern { Pattern = "ThreadSleep", File = filePath, Severity = "High" });
        }

        if (content.Contains("ConfigurationManager.AppSettings"))
        {
            patterns.Add(new AntiPattern { Pattern = "LegacyConfiguration", File = filePath, Severity = "Medium" });
        }

        return patterns;
    }

    private List<GoodPattern> DetectGoodPatterns(string content, string filePath)
    {
        var patterns = new List<GoodPattern>();

        if (content.Contains("async Task") && content.Contains("await"))
        {
            patterns.Add(new GoodPattern { Pattern = "AsyncAwait", File = filePath });
        }

        if (content.Contains("ILogger") && content.Contains("LogError"))
        {
            patterns.Add(new GoodPattern { Pattern = "StructuredLogging", File = filePath });
        }

        if (content.Contains("IOptions<"))
        {
            patterns.Add(new GoodPattern { Pattern = "OptionsPattern", File = filePath });
        }

        return patterns;
    }

    private ComplexityMetric AnalyzeComplexity(string content, string filePath)
    {
        var lines = content.Split('\n');
        var cyclomaticComplexity = 1; // Base complexity

        foreach (var line in lines)
        {
            if (line.Contains("if ") || line.Contains("while ") || line.Contains("for ") || 
                line.Contains("foreach ") || line.Contains("case ") || line.Contains("catch "))
            {
                cyclomaticComplexity++;
            }
        }

        return new ComplexityMetric
        {
            FilePath = filePath,
            LinesOfCode = lines.Length,
            CyclomaticComplexity = cyclomaticComplexity,
            MethodCount = content.Split("public ").Length + content.Split("private ").Length - 2
        };
    }

    private List<ExternalApiUsage> DetectExternalApiUsage(string content, string filePath)
    {
        var usages = new List<ExternalApiUsage>();

        var externalApis = new[]
        {
            "Autodesk.", "Oracle.", "System.Data.OracleClient", "AcDb", "AcGe", "AcBr"
        };

        foreach (var api in externalApis)
        {
            if (content.Contains(api))
            {
                usages.Add(new ExternalApiUsage
                {
                    ApiName = api,
                    FilePath = filePath,
                    UsageType = DetermineUsageType(content, api)
                });
            }
        }

        return usages;
    }

    private string DetermineUsageType(string content, string api)
    {
        if (content.Contains($"using {api}")) return "Import";
        if (content.Contains($"new {api}")) return "Instantiation";
        if (content.Contains($"{api}.")) return "StaticCall";
        return "Reference";
    }

    // Helper methods for improvement suggestions
    private string GetPriorityForAntiPattern(string pattern)
    {
        return pattern switch
        {
            "ThreadSleep" => "Critical",
            "UnloggedExceptions" => "High",
            "LegacyConfiguration" => "Medium",
            _ => "Low"
        };
    }

    private string GetRecommendationForAntiPattern(string pattern)
    {
        return pattern switch
        {
            "ThreadSleep" => "Replace Thread.Sleep with Task.Delay in async contexts",
            "UnloggedExceptions" => "Add structured logging to exception handlers",
            "LegacyConfiguration" => "Migrate to IOptions<T> pattern with appsettings.json",
            _ => "Review and refactor according to best practices"
        };
    }

    private string EstimateEffort(string pattern)
    {
        return pattern switch
        {
            "ThreadSleep" => "Low",
            "UnloggedExceptions" => "Low",
            "LegacyConfiguration" => "High",
            _ => "Medium"
        };
    }

    private string GenerateProcessRecommendation(string failurePattern)
    {
        if (failurePattern.Contains("test"))
            return "Enhance test coverage and add integration tests";
        if (failurePattern.Contains("build"))
            return "Improve build reliability with better dependency management";
        if (failurePattern.Contains("deployment"))
            return "Implement staged deployment with rollback procedures";
        return "Review and strengthen the affected process";
    }

    private string DetermineAffectedProtocol(string improvementType)
    {
        return improvementType switch
        {
            "AntiPattern" => "Refactor Protocol",
            "ProcessImprovement" => "Testing Protocol",
            "MLRecommendation" => "General Protocol",
            _ => "Unknown Protocol"
        };
    }

    private string GenerateExecutiveSummary(PatternAnalysisResult patternAnalysis, 
        ImprovementSuggestions improvements, PerformanceAnalysis performanceAnalysis)
    {
        return $@"# ALARM Learning Analysis - Executive Summary

Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC

## Key Metrics
- **Success Rate**: {patternAnalysis.SuccessRate:P1} over {patternAnalysis.TotalRuns} runs
- **Code Improvements Identified**: {improvements.CodeImprovements.Count}
- **Process Improvements Identified**: {improvements.ProcessImprovements.Count}
- **Performance Regressions**: {performanceAnalysis.Regressions.Count}

## Success Patterns
{string.Join("\n", patternAnalysis.SuccessPatterns.Select(p => $"- {p}"))}

## Failure Patterns  
{string.Join("\n", patternAnalysis.FailurePatterns.Select(p => $"- {p}"))}

## Critical Actions Required
{string.Join("\n", improvements.CodeImprovements.Where(i => i.Priority == "Critical").Select(i => $"- {i.Description}"))}

## Performance Trends
{string.Join("\n", performanceAnalysis.Trends.Select(kv => $"- {kv.Key}: {kv.Value}"))}

## Recommendations
1. Address critical priority items immediately
2. Implement suggested protocol updates  
3. Monitor performance regression trends
4. Continue learning-based analysis for continuous improvement

---
*Generated by ALARM Learning Analyzer*";
    }

    private string GenerateActionableRecommendations(ImprovementSuggestions improvements, 
        List<ProtocolUpdate> protocolUpdates)
    {
        return $@"# ALARM Actionable Recommendations

Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC

## Immediate Actions (Critical Priority)

{string.Join("\n", improvements.CodeImprovements.Where(i => i.Priority == "Critical").Select(i => 
$@"### {i.Description}
- **Type**: {i.Type}
- **Recommendation**: {i.Recommendation}
- **Effort**: {i.EstimatedEffort}
"))}

## Short-term Actions (High Priority)

{string.Join("\n", improvements.CodeImprovements.Where(i => i.Priority == "High").Select(i => 
$@"### {i.Description}
- **Type**: {i.Type}
- **Recommendation**: {i.Recommendation}
- **Effort**: {i.EstimatedEffort}
"))}

## Protocol Updates Required

{string.Join("\n", protocolUpdates.Select(u => 
$@"### {u.ProtocolName}
- **Update Type**: {u.UpdateType}
- **Description**: {u.Description}
- **Proposed Change**: {u.ProposedChange}
- **Justification**: {u.Justification}
"))}

## Implementation Priority

1. **Week 1**: Address all Critical priority items
2. **Week 2-3**: Implement High priority improvements
3. **Week 4**: Update protocols based on recommendations
4. **Ongoing**: Monitor and iterate based on results

---
*Use these recommendations to guide your next sprint planning*";
    }
}

// Data models for the learning system
public class CodePatternAnalysis
{
    public DateTime Timestamp { get; set; }
    public string TargetPath { get; set; } = "";
    public List<AntiPattern> AntiPatterns { get; set; } = new();
    public List<GoodPattern> GoodPatterns { get; set; } = new();
    public Dictionary<string, ComplexityMetric> ComplexityMetrics { get; set; } = new();
    public List<ExternalApiUsage> ExternalApiUsage { get; set; } = new();
}

public class AntiPattern
{
    public string Pattern { get; set; } = "";
    public string File { get; set; } = "";
    public string Severity { get; set; } = "";
}

public class GoodPattern
{
    public string Pattern { get; set; } = "";
    public string File { get; set; } = "";
}

public class ComplexityMetric
{
    public string FilePath { get; set; } = "";
    public int LinesOfCode { get; set; }
    public int CyclomaticComplexity { get; set; }
    public int MethodCount { get; set; }
}

public class ExternalApiUsage
{
    public string ApiName { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string UsageType { get; set; } = "";
}

public class HistoricalRunData
{
    public string RunId { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> IndexData { get; set; } = new();
    public Dictionary<string, object> RiskAssessment { get; set; } = new();
    public Dictionary<string, Dictionary<string, object>> TestResults { get; set; } = new();
}



public class ImprovementSuggestions
{
    public DateTime GeneratedAt { get; set; }
    public bool LearningEnabled { get; set; }
    public List<ImprovementSuggestion> CodeImprovements { get; set; } = new();
    public List<ImprovementSuggestion> ProcessImprovements { get; set; } = new();
    public List<ImprovementSuggestion> MLSuggestions { get; set; } = new();
}

public class ImprovementSuggestion
{
    public string Type { get; set; } = "";
    public string Priority { get; set; } = "";
    public string Description { get; set; } = "";
    public string Recommendation { get; set; } = "";
    public string EstimatedEffort { get; set; } = "";
    public double Confidence { get; set; } = 1.0;
}

public class ProtocolUpdate
{
    public string ProtocolName { get; set; } = "";
    public string UpdateType { get; set; } = "";
    public string Description { get; set; } = "";
    public string ProposedChange { get; set; } = "";
    public string Justification { get; set; } = "";
    public string EstimatedImpact { get; set; } = "";
}

public class PerformanceAnalysis
{
    public DateTime AnalysisTimestamp { get; set; }
    public Dictionary<DateTime, Dictionary<string, double>> HistoricalMetrics { get; set; } = new();
    public Dictionary<string, string> Trends { get; set; } = new();
    public Dictionary<string, double> Baselines { get; set; } = new();
    public List<PerformanceRegression> Regressions { get; set; } = new();
}

public class PerformanceRegression
{
    public string MetricName { get; set; } = "";
    public double BaselineValue { get; set; }
    public double CurrentValue { get; set; }
    public double RegressionPercentage { get; set; }
    public DateTime DetectedAt { get; set; }
}
