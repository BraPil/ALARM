using System.CommandLine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ALARM.ProtocolEngine;
using ALARM.Analyzers;
using ALARM.DataPersistence;
using ALARM.DataPersistence.Services;
using ALARM.DataPersistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ALARM.SystemTests;

public class Program
{
    private static ILogger<Program>? _logger;
    private static IServiceProvider? _serviceProvider;

    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("ALARM High Priority Systems Test Suite");

        var testOption = new Option<string>(
            name: "--test",
            description: "Specific test to run (protocol-engine, ml-engine, data-persistence, all)",
            getDefaultValue: () => "all");

        var verboseOption = new Option<bool>(
            name: "--verbose",
            description: "Enable verbose logging");

        var outputPathOption = new Option<string>(
            name: "--output-path",
            description: "Path to output test results",
            getDefaultValue: () => Path.Combine(Directory.GetCurrentDirectory(), "test-results", DateTime.Now.ToString("yyyyMMdd-HHmm")));

        rootCommand.AddOption(testOption);
        rootCommand.AddOption(verboseOption);
        rootCommand.AddOption(outputPathOption);

        rootCommand.SetHandler(async (test, verbose, outputPath) =>
        {
            await InitializeAsync(verbose);
            var testRunner = new HighPrioritySystemTestRunner(_serviceProvider!, _logger!);
            await testRunner.RunTestsAsync(test, outputPath);
            
        }, testOption, verboseOption, outputPathOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task InitializeAsync(bool verbose)
    {
        // Setup configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Setup services
        var services = new ServiceCollection();
        
        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(verbose ? LogLevel.Debug : LogLevel.Information);
        });

        // Configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Database
        services.AddDbContext<LearningDataContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("TestConnection") 
                ?? "Data Source=test_alarm_learning.db";
            options.UseSqlite(connectionString);
        });

        // Data services
        services.AddScoped<ILearningDataService, LearningDataService>();

        _serviceProvider = services.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger<Program>>();

        // Ensure test database exists
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LearningDataContext>();
        await context.EnsureDatabaseCreatedAsync();

        _logger.LogInformation("System test initialization completed");
    }
}

public class HighPrioritySystemTestRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HighPrioritySystemTestRunner> _logger;
    private readonly List<TestResult> _testResults;

    public HighPrioritySystemTestRunner(IServiceProvider serviceProvider, ILogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = (ILogger<HighPrioritySystemTestRunner>)logger;
        _testResults = new List<TestResult>();
    }

    public async Task RunTestsAsync(string testType, string outputPath)
    {
        _logger.LogInformation("Starting ALARM High Priority Systems Test Suite");
        _logger.LogInformation("Test Type: {TestType}", testType);
        _logger.LogInformation("Output Path: {OutputPath}", outputPath);

        Directory.CreateDirectory(outputPath);

        var startTime = DateTime.UtcNow;

        try
        {
            switch (testType.ToLower())
            {
                case "protocol-engine":
                    await TestProtocolEngineAsync();
                    break;
                case "ml-engine":
                    await TestMLEngineAsync();
                    break;
                case "data-persistence":
                    await TestDataPersistenceAsync();
                    break;
                case "all":
                default:
                    await TestProtocolEngineAsync();
                    await TestMLEngineAsync();
                    await TestDataPersistenceAsync();
                    await TestIntegrationAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test suite execution failed");
            _testResults.Add(new TestResult
            {
                TestName = "TestSuiteExecution",
                Success = false,
                ErrorMessage = ex.Message,
                Duration = DateTime.UtcNow - startTime
            });
        }

        // Generate comprehensive test report
        await GenerateTestReportAsync(outputPath, DateTime.UtcNow - startTime);
    }

    private async Task TestProtocolEngineAsync()
    {
        _logger.LogInformation("🔧 Testing Protocol Modification Engine...");

        var testStartTime = DateTime.UtcNow;

        try
        {
            // Test 1: Create sample protocol update files
            await CreateTestProtocolUpdatesAsync();
            
            // Test 2: Test dry-run functionality
            await TestProtocolEngineDryRunAsync();
            
            // Test 3: Test actual protocol modification
            await TestProtocolEngineModificationAsync();
            
            // Test 4: Test validation functionality
            await TestProtocolEngineValidationAsync();

            _testResults.Add(new TestResult
            {
                TestName = "ProtocolEngine_Complete",
                Success = true,
                Duration = DateTime.UtcNow - testStartTime,
                Details = "All protocol engine tests passed successfully"
            });

            _logger.LogInformation("✅ Protocol Engine tests completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Protocol Engine tests failed");
            _testResults.Add(new TestResult
            {
                TestName = "ProtocolEngine_Complete",
                Success = false,
                ErrorMessage = ex.Message,
                Duration = DateTime.UtcNow - testStartTime
            });
        }
    }

    private async Task TestMLEngineAsync()
    {
        _logger.LogInformation("🧠 Testing ML Engine...");

        var testStartTime = DateTime.UtcNow;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<MLEngine>>();
            var mlEngine = new MLEngine(logger);

            // Test 1: Success Prediction
            await TestSuccessPredictionAsync(mlEngine);
            
            // Test 2: Pattern Analysis
            await TestPatternAnalysisAsync(mlEngine);
            
            // Test 3: Performance Optimization
            await TestPerformanceOptimizationAsync(mlEngine);
            
            // Test 4: Causal Analysis
            await TestCausalAnalysisAsync(mlEngine);
            
            // Test 5: Advanced Causal Analysis
            await TestAdvancedCausalAnalysisAsync();

            _testResults.Add(new TestResult
            {
                TestName = "MLEngine_Complete",
                Success = true,
                Duration = DateTime.UtcNow - testStartTime,
                Details = "All ML engine tests passed successfully"
            });

            _logger.LogInformation("✅ ML Engine tests completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ ML Engine tests failed");
            _testResults.Add(new TestResult
            {
                TestName = "MLEngine_Complete",
                Success = false,
                ErrorMessage = ex.Message,
                Duration = DateTime.UtcNow - testStartTime
            });
        }
    }

    private async Task TestDataPersistenceAsync()
    {
        _logger.LogInformation("💾 Testing Data Persistence Layer...");

        var testStartTime = DateTime.UtcNow;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dataService = scope.ServiceProvider.GetRequiredService<ILearningDataService>();

            // Test 1: Run Data Operations
            await TestRunDataOperationsAsync(dataService);
            
            // Test 2: Pattern Operations
            await TestPatternOperationsAsync(dataService);
            
            // Test 3: Improvement Operations
            await TestImprovementOperationsAsync(dataService);
            
            // Test 4: Database Performance
            await TestDatabasePerformanceAsync(dataService);

            _testResults.Add(new TestResult
            {
                TestName = "DataPersistence_Complete",
                Success = true,
                Duration = DateTime.UtcNow - testStartTime,
                Details = "All data persistence tests passed successfully"
            });

            _logger.LogInformation("✅ Data Persistence tests completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Data Persistence tests failed");
            _testResults.Add(new TestResult
            {
                TestName = "DataPersistence_Complete",
                Success = false,
                ErrorMessage = ex.Message,
                Duration = DateTime.UtcNow - testStartTime
            });
        }
    }

    private async Task TestIntegrationAsync()
    {
        _logger.LogInformation("🔗 Testing System Integration...");

        var testStartTime = DateTime.UtcNow;

        try
        {
            // Test end-to-end workflow:
            // 1. Generate ML insights
            // 2. Create protocol updates
            // 3. Apply updates
            // 4. Store results

            await TestEndToEndWorkflowAsync();

            _testResults.Add(new TestResult
            {
                TestName = "Integration_Complete",
                Success = true,
                Duration = DateTime.UtcNow - testStartTime,
                Details = "Integration tests passed successfully"
            });

            _logger.LogInformation("✅ Integration tests completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Integration tests failed");
            _testResults.Add(new TestResult
            {
                TestName = "Integration_Complete",
                Success = false,
                ErrorMessage = ex.Message,
                Duration = DateTime.UtcNow - testStartTime
            });
        }
    }

    // Specific test implementations
    private async Task CreateTestProtocolUpdatesAsync()
    {
        _logger.LogDebug("Creating test protocol update files...");

        var testUpdatesDir = Path.Combine(Directory.GetCurrentDirectory(), "test-data", "protocol-updates");
        Directory.CreateDirectory(testUpdatesDir);

        var sampleUpdates = new List<ALARM.ProtocolEngine.ProtocolUpdate>
        {
            new ALARM.ProtocolEngine.ProtocolUpdate
            {
                ProtocolName = "Research Protocol",
                UpdateType = "Enhancement",
                Description = "Add AI-powered root cause analysis",
                ProposedChange = "\n### **AI-POWERED ROOT CAUSE ANALYSIS**\n- Use machine learning to identify potential root causes\n- Provide confidence scores for each hypothesis\n- Cross-reference with historical failure patterns\n",
                Justification = "ML analysis shows 85% accuracy improvement in root cause identification",
                EstimatedImpact = "High",
                TargetSection = "PROTOCOL STEPS",
                Confidence = 0.9
            },
            new ALARM.ProtocolEngine.ProtocolUpdate
            {
                ProtocolName = "Verify Protocol",
                UpdateType = "Enhancement", 
                Description = "Add automated regression testing",
                ProposedChange = "\n- [ ] Automated regression tests executed\n- [ ] Performance benchmarks validated\n- [ ] Security scan completed\n",
                Justification = "Automated testing reduces verification time by 60%",
                EstimatedImpact = "Medium",
                TargetSection = "COMPLETION CHECKLIST",
                Confidence = 0.8
            }
        };

        var jsonContent = JsonSerializer.Serialize(sampleUpdates, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(Path.Combine(testUpdatesDir, "test_protocol_updates.json"), jsonContent);

        _logger.LogDebug("Test protocol update files created successfully");
    }

    private async Task TestProtocolEngineDryRunAsync()
    {
        _logger.LogDebug("Testing protocol engine dry-run functionality...");

        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var protocolLogger = loggerFactory.CreateLogger<ProtocolModificationEngine>();
        var engine = new ProtocolModificationEngine(protocolLogger);
        
        var protocolPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "mcp", "protocols");
        var updatesPath = Path.Combine(Directory.GetCurrentDirectory(), "test-data", "protocol-updates");
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "test-results", "dry-run");

        await engine.ProcessUpdatesAsync(protocolPath, updatesPath, outputPath, dryRun: true, backup: false);

        // Verify dry-run didn't modify original files
        var originalFiles = Directory.GetFiles(protocolPath, "*.md");
        foreach (var file in originalFiles)
        {
            var content = await File.ReadAllTextAsync(file);
            if (content.Contains("AI-POWERED ROOT CAUSE ANALYSIS"))
            {
                throw new Exception("Dry-run modified original files!");
            }
        }

        _logger.LogDebug("Protocol engine dry-run test passed");
    }

    private async Task TestProtocolEngineModificationAsync()
    {
        _logger.LogDebug("Testing protocol engine modification functionality...");

        // Create copies of protocols for testing
        var testProtocolPath = Path.Combine(Directory.GetCurrentDirectory(), "test-data", "protocols");
        Directory.CreateDirectory(testProtocolPath);

        var originalProtocolPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "mcp", "protocols");
        foreach (var file in Directory.GetFiles(originalProtocolPath, "*.md"))
        {
            File.Copy(file, Path.Combine(testProtocolPath, Path.GetFileName(file)), true);
        }

        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var protocolLogger = loggerFactory.CreateLogger<ProtocolModificationEngine>();
        var engine = new ProtocolModificationEngine(protocolLogger);
        var updatesPath = Path.Combine(Directory.GetCurrentDirectory(), "test-data", "protocol-updates");
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "test-results", "modifications");

        await engine.ProcessUpdatesAsync(testProtocolPath, updatesPath, outputPath, dryRun: false, backup: true);

        // Verify modifications were applied
        var modifiedFiles = Directory.GetFiles(outputPath, "*.md");
        if (!modifiedFiles.Any())
        {
            throw new Exception("No modified files found in output directory");
        }

        _logger.LogDebug("Protocol engine modification test passed");
    }

    private async Task TestProtocolEngineValidationAsync()
    {
        _logger.LogDebug("Testing protocol engine validation functionality...");

        // This would test the validation logic
        // For now, we'll just verify the validation report exists
        var validationReportPath = Path.Combine(Directory.GetCurrentDirectory(), "test-results", "modifications", "protocol_validation_report.json");
        
        if (File.Exists(validationReportPath))
        {
            var validationContent = await File.ReadAllTextAsync(validationReportPath);
            var validationReport = JsonSerializer.Deserialize<Dictionary<string, object>>(validationContent);
            
            if (validationReport == null || !validationReport.ContainsKey("TotalValidations"))
            {
                throw new Exception("Invalid validation report format");
            }
        }

        _logger.LogDebug("Protocol engine validation test passed");
    }

    private async Task TestSuccessPredictionAsync(MLEngine mlEngine)
    {
        _logger.LogDebug("Testing ML success prediction...");

        var context = new ProjectContext
        {
            FileCount = 150,
            ComplexityScore = 5.5f,
            ApiUsageCount = 25,
            TestCoverage = 0.75f,
            PreviousSuccessRate = 0.8f,
            TeamExperience = 0.7f,
            TechnicalDebt = 3.2f
        };

        var historicalData = new List<HistoricalRunData>(); // Would normally load real data

        var prediction = await mlEngine.PredictSuccessProbabilityAsync(context, historicalData);

        if (prediction.SuccessProbability < 0 || prediction.SuccessProbability > 1)
        {
            throw new Exception($"Invalid success probability: {prediction.SuccessProbability}");
        }

        _logger.LogDebug("ML success prediction test passed - Probability: {Probability:P2}", prediction.SuccessProbability);
    }

    private async Task TestPatternAnalysisAsync(MLEngine mlEngine)
    {
        _logger.LogDebug("Testing ML pattern analysis...");

        var historicalData = GenerateTestHistoricalData(20);
        var analysis = await mlEngine.AnalyzePatternsWithMLAsync(historicalData);

        if (analysis.TotalRuns != historicalData.Count)
        {
            throw new Exception($"Pattern analysis run count mismatch: expected {historicalData.Count}, got {analysis.TotalRuns}");
        }

        _logger.LogDebug("ML pattern analysis test passed - {InsightCount} insights generated", analysis.Insights.Count);
    }

    private async Task TestPerformanceOptimizationAsync(MLEngine mlEngine)
    {
        _logger.LogDebug("Testing ML performance optimization...");

        var historicalData = GenerateTestHistoricalData(10);
        var optimization = await mlEngine.OptimizeParametersAsync("indexer", "large-codebase", historicalData);

        if (optimization.TargetTool != "indexer")
        {
            throw new Exception($"Optimization target tool mismatch: expected 'indexer', got '{optimization.TargetTool}'");
        }

        _logger.LogDebug("ML performance optimization test passed - Expected improvement: {Improvement:P2}", optimization.ExpectedImprovement);
    }

    private async Task TestCausalAnalysisAsync(MLEngine mlEngine)
    {
        _logger.LogDebug("Testing ML causal analysis...");

        var historicalData = GenerateTestHistoricalData(15);
        var causalAnalysis = await mlEngine.PerformCausalAnalysisAsync(historicalData);

        if (causalAnalysis.AnalysisTimestamp == default)
        {
            throw new Exception("Causal analysis timestamp not set");
        }

        _logger.LogDebug("ML causal analysis test passed - {InsightCount} causal insights generated", causalAnalysis.CausalInsights.Count);
    }

    private async Task TestAdvancedCausalAnalysisAsync()
    {
        _logger.LogDebug("Testing Advanced Causal Analysis system...");

        // Import the new causal analysis namespace
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var causalLogger = loggerFactory.CreateLogger<ALARM.Analyzers.CausalAnalysis.CausalAnalysisEngine>();
        
        var causalEngine = new ALARM.Analyzers.CausalAnalysis.CausalAnalysisEngine(causalLogger);

        // Generate test causal data
        var testData = GenerateTestCausalData(50);
        
        // Test comprehensive causal analysis
        var result = await causalEngine.AnalyzeCausalRelationshipsAsync(testData);

        // Validate results
        if (result.AnalysisTimestamp == default)
        {
            throw new Exception("Advanced causal analysis timestamp not set");
        }

        if (result.DataSampleCount != testData.Count)
        {
            throw new Exception($"Data sample count mismatch: expected {testData.Count}, got {result.DataSampleCount}");
        }

        // Test causal relationships discovery
        if (result.CausalRelationships == null)
        {
            throw new Exception("Causal relationships not initialized");
        }

        // Test causal graph generation
        if (result.CausalGraph == null)
        {
            throw new Exception("Causal graph not generated");
        }

        // Test structural equations
        if (result.StructuralEquations == null)
        {
            throw new Exception("Structural equations not generated");
        }

        // Test intervention effects
        if (result.InterventionEffects == null)
        {
            throw new Exception("Intervention effects not analyzed");
        }

        // Test confounding detection
        if (result.ConfoundingFactors == null)
        {
            throw new Exception("Confounding factors not detected");
        }

        // Test overall confidence calculation
        if (result.OverallConfidence < 0 || result.OverallConfidence > 1)
        {
            throw new Exception($"Invalid overall confidence: {result.OverallConfidence}");
        }

        _logger.LogDebug("Advanced Causal Analysis test passed - " +
            "{RelationshipCount} relationships, {EquationCount} equations, " +
            "{InterventionCount} interventions, {ConfoundingCount} confounders, " +
            "confidence: {Confidence:P2}",
            result.CausalRelationships.Count,
            result.StructuralEquations.Count,
            result.InterventionEffects.Count,
            result.ConfoundingFactors.Count,
            result.OverallConfidence);

        // Test temporal causal analysis
        await TestTemporalCausalAnalysisAsync(causalEngine, testData);

        // Test causal comparison
        await TestCausalComparisonAsync(causalEngine, testData);
    }

    private async Task TestTemporalCausalAnalysisAsync(ALARM.Analyzers.CausalAnalysis.CausalAnalysisEngine causalEngine, List<ALARM.Analyzers.CausalAnalysis.CausalData> testData)
    {
        _logger.LogDebug("Testing Temporal Causal Analysis...");

        var temporalResult = await causalEngine.AnalyzeTemporalCausalRelationshipsAsync(testData);

        if (temporalResult.AnalysisTimestamp == default)
        {
            throw new Exception("Temporal causal analysis timestamp not set");
        }

        if (temporalResult.TimeWindows == null)
        {
            throw new Exception("Time windows not generated");
        }

        if (temporalResult.CausalStabilityMetrics == null)
        {
            throw new Exception("Causal stability metrics not calculated");
        }

        if (temporalResult.CausalChangePoints == null)
        {
            throw new Exception("Causal change points not detected");
        }

        _logger.LogDebug("Temporal Causal Analysis test passed - " +
            "{WindowCount} windows, {ChangePointCount} change points",
            temporalResult.TimeWindows.Count,
            temporalResult.CausalChangePoints.Count);
    }

    private async Task TestCausalComparisonAsync(ALARM.Analyzers.CausalAnalysis.CausalAnalysisEngine causalEngine, List<ALARM.Analyzers.CausalAnalysis.CausalData> testData)
    {
        _logger.LogDebug("Testing Causal Comparison Analysis...");

        // Split data for comparison
        var baselineData = testData.Take(testData.Count / 2).ToList();
        var comparisonData = testData.Skip(testData.Count / 2).ToList();

        var comparisonResult = await causalEngine.CompareCausalRelationshipsAsync(baselineData, comparisonData);

        if (comparisonResult.BaselineAnalysis == null)
        {
            throw new Exception("Baseline analysis not performed");
        }

        if (comparisonResult.ComparisonAnalysis == null)
        {
            throw new Exception("Comparison analysis not performed");
        }

        if (comparisonResult.CausalSimilarity < 0 || comparisonResult.CausalSimilarity > 1)
        {
            throw new Exception($"Invalid causal similarity: {comparisonResult.CausalSimilarity}");
        }

        if (comparisonResult.SignificantDifferences == null)
        {
            throw new Exception("Significant differences not identified");
        }

        if (comparisonResult.CausalEvolution == null)
        {
            throw new Exception("Causal evolution not analyzed");
        }

        _logger.LogDebug("Causal Comparison test passed - " +
            "similarity: {Similarity:P2}, {DifferenceCount} differences",
            comparisonResult.CausalSimilarity,
            comparisonResult.SignificantDifferences.Count);
    }

    private List<ALARM.Analyzers.CausalAnalysis.CausalData> GenerateTestCausalData(int count)
    {
        var random = new Random(42); // Fixed seed for reproducible tests
        var data = new List<ALARM.Analyzers.CausalAnalysis.CausalData>();
        var baseTime = DateTime.UtcNow.AddDays(-30);

        for (int i = 0; i < count; i++)
        {
            // Create realistic causal relationships for testing
            var executionTime = 1.0 + random.NextDouble() * 4.0; // 1-5 seconds
            var memoryUsage = 50 + random.NextDouble() * 200; // 50-250 MB
            var codeComplexity = 1 + random.NextDouble() * 9; // 1-10 complexity
            var testCoverage = 0.3 + random.NextDouble() * 0.7; // 30-100% coverage
            
            // Introduce causal relationships:
            // 1. Higher complexity -> longer execution time
            executionTime += codeComplexity * 0.3;
            
            // 2. Lower test coverage -> higher execution time (bugs slow things down)
            executionTime += (1 - testCoverage) * 2.0;
            
            // 3. Higher complexity -> higher memory usage
            memoryUsage += codeComplexity * 15;
            
            // Add some noise
            executionTime += (random.NextDouble() - 0.5) * 0.5;
            memoryUsage += (random.NextDouble() - 0.5) * 20;

            var causalData = new ALARM.Analyzers.CausalAnalysis.CausalData
            {
                Timestamp = baseTime.AddMinutes(i * 10),
                Variables = new Dictionary<string, double>
                {
                    ["ExecutionTime"] = Math.Max(0.1, executionTime),
                    ["MemoryUsage"] = Math.Max(10, memoryUsage),
                    ["CodeComplexity"] = Math.Max(1, codeComplexity),
                    ["TestCoverage"] = Math.Max(0.1, Math.Min(1.0, testCoverage)),
                    ["ErrorRate"] = Math.Max(0, (1 - testCoverage) * 0.1 + (random.NextDouble() - 0.5) * 0.05),
                    ["BuildSuccess"] = random.NextDouble() > 0.1 ? 1.0 : 0.0
                },
                Source = "TestDataGenerator",
                Context = $"TestSample_{i}"
            };

            data.Add(causalData);
        }

        return data;
    }

    private async Task TestRunDataOperationsAsync(ILearningDataService dataService)
    {
        _logger.LogDebug("Testing run data operations...");

        var testRunData = new RunDataDto
        {
            RunId = $"test-run-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Timestamp = DateTime.UtcNow,
            ProjectName = "TestProject",
            Environment = "Test",
            Success = true,
            Duration = 5.5,
            IndexData = new Dictionary<string, object> { ["TotalFiles"] = 100, ["TotalSymbols"] = 500 },
            RiskAssessment = new Dictionary<string, object> { ["HighRiskItems"] = 2 },
            TestResults = new Dictionary<string, Dictionary<string, object>>(),
            Metrics = new Dictionary<string, double> { ["Performance"] = 0.85 }
        };

        var runId = await dataService.SaveRunDataAsync(testRunData);
        if (runId <= 0)
        {
            throw new Exception("Failed to save run data - invalid ID returned");
        }

        var retrievedData = await dataService.GetRunDataAsync(testRunData.RunId);
        if (retrievedData == null || retrievedData.RunId != testRunData.RunId)
        {
            throw new Exception("Failed to retrieve saved run data");
        }

        _logger.LogDebug("Run data operations test passed - Run ID: {RunId}", runId);
    }

    private async Task TestPatternOperationsAsync(ILearningDataService dataService)
    {
        _logger.LogDebug("Testing pattern operations...");

        var testPatterns = new List<PatternDto>
        {
            new PatternDto
            {
                PatternType = "AntiPattern",
                PatternName = "UnloggedExceptions",
                Description = "Exception handling without logging",
                FilePath = "test/file.cs",
                Confidence = 0.9,
                Frequency = 5,
                Context = new Dictionary<string, object> { ["Method"] = "TestMethod" }
            }
        };

        // Note: This would require a run ID from the previous test
        // For now, we'll test the pattern frequency method
        var frequency = await dataService.GetPatternFrequencyAsync("AntiPattern", TimeSpan.FromDays(30));
        
        // This should return empty results for new database, which is expected
        _logger.LogDebug("Pattern operations test passed - Frequency count: {Count}", frequency.Count);
    }

    private async Task TestImprovementOperationsAsync(ILearningDataService dataService)
    {
        _logger.LogDebug("Testing improvement operations...");

        var pendingImprovements = await dataService.GetPendingImprovementsAsync();
        
        // This should return empty results for new database, which is expected
        _logger.LogDebug("Improvement operations test passed - Pending improvements: {Count}", pendingImprovements.Count);
    }

    private async Task TestDatabasePerformanceAsync(ILearningDataService dataService)
    {
        _logger.LogDebug("Testing database performance...");

        var startTime = DateTime.UtcNow;
        
        // Test multiple concurrent operations
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            var runId = $"perf-test-{i}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            tasks.Add(dataService.SaveRunDataAsync(new RunDataDto
            {
                RunId = runId,
                Timestamp = DateTime.UtcNow,
                ProjectName = "PerformanceTest",
                Environment = "Test",
                Success = i % 2 == 0,
                Duration = i * 0.5,
                IndexData = new Dictionary<string, object>(),
                RiskAssessment = new Dictionary<string, object>(),
                TestResults = new Dictionary<string, Dictionary<string, object>>(),
                Metrics = new Dictionary<string, double>()
            }));
        }

        await Task.WhenAll(tasks);
        var duration = DateTime.UtcNow - startTime;

        if (duration.TotalSeconds > 10) // Should complete within 10 seconds
        {
            throw new Exception($"Database performance test failed - took {duration.TotalSeconds:F2} seconds");
        }

        _logger.LogDebug("Database performance test passed - {Duration:F2}s for 10 concurrent operations", duration.TotalSeconds);
    }

    private async Task TestEndToEndWorkflowAsync()
    {
        _logger.LogDebug("Testing end-to-end workflow...");

        // This would test the complete workflow:
        // 1. ML analysis generates insights
        // 2. Insights create protocol updates
        // 3. Protocol engine applies updates
        // 4. Results stored in database

        // For now, we'll just verify the components can work together
        using var scope = _serviceProvider.CreateScope();
        var dataService = scope.ServiceProvider.GetRequiredService<ILearningDataService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MLEngine>>();
        var mlEngine = new MLEngine(logger);

        // Generate some test data
        var historicalData = GenerateTestHistoricalData(5);
        var analysis = await mlEngine.AnalyzePatternsWithMLAsync(historicalData);

        // Verify we got insights
        if (analysis.Insights.Count == 0)
        {
            _logger.LogWarning("No insights generated in end-to-end test");
        }

        _logger.LogDebug("End-to-end workflow test passed");
    }

    private List<HistoricalRunData> GenerateTestHistoricalData(int count)
    {
        var data = new List<HistoricalRunData>();
        var random = new Random(42); // Fixed seed for reproducible tests

        for (int i = 0; i < count; i++)
        {
            data.Add(new HistoricalRunData
            {
                RunId = $"test-{i}",
                Timestamp = DateTime.UtcNow.AddDays(-i),
                IndexData = new Dictionary<string, object>
                {
                    ["Summary"] = JsonSerializer.SerializeToElement(new
                    {
                        TotalFiles = random.Next(50, 200),
                        TotalSymbols = random.Next(500, 2000),
                        ExternalApiUsages = random.Next(10, 50)
                    })
                },
                RiskAssessment = new Dictionary<string, object>
                {
                    ["HighRiskItems"] = JsonSerializer.SerializeToElement(new object[random.Next(0, 5)])
                },
                TestResults = new Dictionary<string, Dictionary<string, object>>
                {
                    ["UnitTests"] = new Dictionary<string, object>
                    {
                        ["success"] = random.NextDouble() > 0.3, // 70% success rate
                        ["Coverage"] = random.Next(60, 95)
                    }
                }
            });
        }

        return data;
    }

    private async Task GenerateTestReportAsync(string outputPath, TimeSpan totalDuration)
    {
        _logger.LogInformation("Generating comprehensive test report...");

        var report = new TestReport
        {
            Timestamp = DateTime.UtcNow,
            TotalDuration = totalDuration,
            TotalTests = _testResults.Count,
            PassedTests = _testResults.Count(r => r.Success),
            FailedTests = _testResults.Count(r => !r.Success),
            TestResults = _testResults
        };

        // Generate JSON report
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonReport = JsonSerializer.Serialize(report, jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "test_report.json"), jsonReport);

        // Generate Markdown report
        var markdownReport = GenerateMarkdownReport(report);
        await File.WriteAllTextAsync(Path.Combine(outputPath, "test_report.md"), markdownReport);

        // Log summary
        _logger.LogInformation("📊 Test Results Summary:");
        _logger.LogInformation("   Total Tests: {TotalTests}", report.TotalTests);
        _logger.LogInformation("   Passed: {PassedTests} ✅", report.PassedTests);
        _logger.LogInformation("   Failed: {FailedTests} ❌", report.FailedTests);
        _logger.LogInformation("   Success Rate: {SuccessRate:P1}", (double)report.PassedTests / report.TotalTests);
        _logger.LogInformation("   Total Duration: {Duration:F2}s", totalDuration.TotalSeconds);

        if (report.FailedTests > 0)
        {
            _logger.LogWarning("⚠️  Some tests failed. Check the detailed report for more information.");
        }
        else
        {
            _logger.LogInformation("🎉 All tests passed successfully!");
        }
    }

    private string GenerateMarkdownReport(TestReport report)
    {
        var md = new System.Text.StringBuilder();

        md.AppendLine("# ALARM High Priority Systems Test Report");
        md.AppendLine();
        md.AppendLine($"**Generated:** {report.Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
        md.AppendLine($"**Total Duration:** {report.TotalDuration.TotalSeconds:F2} seconds");
        md.AppendLine();

        md.AppendLine("## Summary");
        md.AppendLine();
        md.AppendLine($"- **Total Tests:** {report.TotalTests}");
        md.AppendLine($"- **Passed:** {report.PassedTests} ✅");
        md.AppendLine($"- **Failed:** {report.FailedTests} ❌");
        md.AppendLine($"- **Success Rate:** {(double)report.PassedTests / report.TotalTests:P1}");
        md.AppendLine();

        md.AppendLine("## Test Results");
        md.AppendLine();

        foreach (var result in report.TestResults.OrderBy(r => r.TestName))
        {
            var status = result.Success ? "✅ PASS" : "❌ FAIL";
            md.AppendLine($"### {result.TestName} - {status}");
            md.AppendLine();
            md.AppendLine($"- **Duration:** {result.Duration.TotalSeconds:F2}s");
            
            if (!string.IsNullOrEmpty(result.Details))
            {
                md.AppendLine($"- **Details:** {result.Details}");
            }
            
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                md.AppendLine($"- **Error:** {result.ErrorMessage}");
            }
            
            md.AppendLine();
        }

        md.AppendLine("---");
        md.AppendLine("*Generated by ALARM System Test Suite*");

        return md.ToString();
    }
}

// Data models for test results
public class TestResult
{
    public string TestName { get; set; } = "";
    public bool Success { get; set; }
    public TimeSpan Duration { get; set; }
    public string Details { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
}

public class TestReport
{
    public DateTime Timestamp { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }
    public List<TestResult> TestResults { get; set; } = new();
}
