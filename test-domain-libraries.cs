using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ALARM.DomainLibraries;
using ALARM.DomainLibraries.AutoCAD;
using ALARM.DomainLibraries.Oracle;
using ALARM.DomainLibraries.DotNetCore;
using ALARM.DomainLibraries.ADDS;
using ALARM.Analyzers.PatternDetection;

namespace DomainLibrariesTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🎯 ALARM Domain Libraries Integration Test");
            Console.WriteLine("==========================================");

            // Create logger factory
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            try
            {
                // Test individual domain libraries
                await TestAutoCADLibraryAsync(loggerFactory);
                await TestOracleLibraryAsync(loggerFactory);
                await TestDotNetCoreLibraryAsync(loggerFactory);
                await TestADDSLibraryAsync(loggerFactory);

                // Test domain library manager
                await TestDomainLibraryManagerAsync(loggerFactory);

                Console.WriteLine("\n✅ All domain library tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        static async Task TestAutoCADLibraryAsync(ILoggerFactory loggerFactory)
        {
            Console.WriteLine("\n🔧 Testing AutoCAD Domain Library...");
            
            var logger = loggerFactory.CreateLogger<AutoCADPatterns>();
            var autocadLibrary = new AutoCADPatterns(logger);
            
            Console.WriteLine($"Domain: {autocadLibrary.DomainName} v{autocadLibrary.Version}");
            Console.WriteLine($"Categories: {string.Join(", ", autocadLibrary.SupportedPatternCategories)}");

            // Create test data with AutoCAD patterns
            var testData = new List<PatternData>
            {
                new PatternData
                {
                    Timestamp = DateTime.UtcNow,
                    Value = 1.0,
                    Category = "AutoCAD",
                    Source = "Test",
                    Metadata = new Dictionary<string, object>
                    {
                        ["CodeContent"] = "AcDbEntity entity = new AcDbEntity(); acrxEntryPoint();"
                    }
                }
            };

            var config = new PatternDetectionConfig();
            var result = await autocadLibrary.DetectPatternsAsync(testData, config);
            
            Console.WriteLine($"Detected {result.DetectedPatterns.Count} patterns, {result.DetectedAnomalies.Count} anomalies");
            
            foreach (var pattern in result.DetectedPatterns)
            {
                Console.WriteLine($"  - {pattern.PatternName} ({pattern.Category}): {pattern.ConfidenceScore:F2}");
            }
        }

        static async Task TestOracleLibraryAsync(ILoggerFactory loggerFactory)
        {
            Console.WriteLine("\n🗄️ Testing Oracle Domain Library...");
            
            var logger = loggerFactory.CreateLogger<OraclePatterns>();
            var oracleLibrary = new OraclePatterns(logger);
            
            Console.WriteLine($"Domain: {oracleLibrary.DomainName} v{oracleLibrary.Version}");
            Console.WriteLine($"Categories: {string.Join(", ", oracleLibrary.SupportedPatternCategories)}");

            // Create test data with Oracle patterns
            var testData = new List<PatternData>
            {
                new PatternData
                {
                    Timestamp = DateTime.UtcNow,
                    Value = 1.0,
                    Category = "Oracle",
                    Source = "Test",
                    Metadata = new Dictionary<string, object>
                    {
                        ["SQLContent"] = "SELECT * FROM users WHERE id = " + "' + userId + '",
                        ["CodeContent"] = "new OracleConnection(connectionString)"
                    }
                }
            };

            var config = new PatternDetectionConfig();
            var result = await oracleLibrary.DetectPatternsAsync(testData, config);
            
            Console.WriteLine($"Detected {result.DetectedPatterns.Count} patterns, {result.DetectedAnomalies.Count} anomalies");
            
            foreach (var pattern in result.DetectedPatterns)
            {
                Console.WriteLine($"  - {pattern.PatternName} ({pattern.Category}): {pattern.ConfidenceScore:F2}");
            }
        }

        static async Task TestDotNetCoreLibraryAsync(ILoggerFactory loggerFactory)
        {
            Console.WriteLine("\n⚙️ Testing .NET Core Domain Library...");
            
            var logger = loggerFactory.CreateLogger<DotNetCorePatterns>();
            var dotnetLibrary = new DotNetCorePatterns(logger);
            
            Console.WriteLine($"Domain: {dotnetLibrary.DomainName} v{dotnetLibrary.Version}");
            Console.WriteLine($"Categories: {string.Join(", ", dotnetLibrary.SupportedPatternCategories)}");

            // Create test data with .NET Core patterns
            var testData = new List<PatternData>
            {
                new PatternData
                {
                    Timestamp = DateTime.UtcNow,
                    Value = 1.0,
                    Category = "DotNetCore",
                    Source = "Test",
                    Metadata = new Dictionary<string, object>
                    {
                        ["CodeContent"] = "using System.Web; HttpContext.Current.Request; ConfigurationManager.AppSettings[\"key\"];"
                    }
                }
            };

            var config = new PatternDetectionConfig();
            var result = await dotnetLibrary.DetectPatternsAsync(testData, config);
            
            Console.WriteLine($"Detected {result.DetectedPatterns.Count} patterns, {result.DetectedAnomalies.Count} anomalies");
            
            foreach (var pattern in result.DetectedPatterns)
            {
                Console.WriteLine($"  - {pattern.PatternName} ({pattern.Category}): {pattern.ConfidenceScore:F2}");
            }
        }

        static async Task TestADDSLibraryAsync(ILoggerFactory loggerFactory)
        {
            Console.WriteLine("\n🎯 Testing ADDS Domain Library...");
            
            var logger = loggerFactory.CreateLogger<ADDSPatterns>();
            var addsLibrary = new ADDSPatterns(logger);
            
            Console.WriteLine($"Domain: {addsLibrary.DomainName} v{addsLibrary.Version}");
            Console.WriteLine($"Categories: {string.Join(", ", addsLibrary.SupportedPatternCategories)}");

            // Create test data with ADDS patterns
            var testData = new List<PatternData>
            {
                new PatternData
                {
                    Timestamp = DateTime.UtcNow,
                    Value = 1.0,
                    Category = "ADDS",
                    Source = "Test",
                    Metadata = new Dictionary<string, object>
                    {
                        ["CodeContent"] = "DrawingFile.dwg; WorkflowEngine.ExecuteWorkflow(); ADDS24.LegacyProcess();"
                    }
                }
            };

            var config = new PatternDetectionConfig();
            var result = await addsLibrary.DetectPatternsAsync(testData, config);
            
            Console.WriteLine($"Detected {result.DetectedPatterns.Count} patterns, {result.DetectedAnomalies.Count} anomalies");
            
            foreach (var pattern in result.DetectedPatterns)
            {
                Console.WriteLine($"  - {pattern.PatternName} ({pattern.Category}): {pattern.ConfidenceScore:F2}");
            }
        }

        static async Task TestDomainLibraryManagerAsync(ILoggerFactory loggerFactory)
        {
            Console.WriteLine("\n🎯 Testing Domain Library Manager...");
            
            var logger = loggerFactory.CreateLogger<DomainLibraryManager>();
            var manager = new DomainLibraryManager(logger);

            // Register all domain libraries
            manager.RegisterDomainLibrary(new AutoCADPatterns(loggerFactory.CreateLogger<AutoCADPatterns>()));
            manager.RegisterDomainLibrary(new OraclePatterns(loggerFactory.CreateLogger<OraclePatterns>()));
            manager.RegisterDomainLibrary(new DotNetCorePatterns(loggerFactory.CreateLogger<DotNetCorePatterns>()));
            manager.RegisterDomainLibrary(new ADDSPatterns(loggerFactory.CreateLogger<ADDSPatterns>()));

            Console.WriteLine($"Registered {manager.GetRegisteredLibraries().Count} domain libraries");

            // Create comprehensive test data
            var testData = new List<PatternData>
            {
                new PatternData
                {
                    Timestamp = DateTime.UtcNow,
                    Value = 1.0,
                    Category = "Mixed",
                    Source = "Test",
                    Metadata = new Dictionary<string, object>
                    {
                        ["CodeContent"] = @"
                            // AutoCAD patterns
                            AcDbEntity entity = new AcDbEntity();
                            acrxEntryPoint();
                            
                            // Oracle patterns
                            SELECT * FROM users WHERE id = ' + userId + ';
                            new OracleConnection(connectionString);
                            
                            // .NET Core patterns
                            using System.Web;
                            HttpContext.Current.Request;
                            ConfigurationManager.AppSettings[""key""];
                            
                            // ADDS patterns
                            DrawingFile.dwg;
                            WorkflowEngine.ExecuteWorkflow();
                            ADDS24.LegacyProcess();
                        ",
                        ["SQLContent"] = "SELECT * FROM DRAWINGS WHERE project_id = ' + projectId + '",
                        ["PLSQLContent"] = "BEGIN EXCEPTION END;",
                        ["DrawingInfo"] = "ComplexDrawing.dwg"
                    }
                }
            };

            var config = new PatternDetectionConfig();
            var combinedResult = await manager.DetectPatternsAsync(testData, config);
            
            Console.WriteLine($"\nCombined Results:");
            Console.WriteLine($"Total patterns: {combinedResult.AllDetectedPatterns.Count}");
            Console.WriteLine($"Total anomalies: {combinedResult.AllDetectedAnomalies.Count}");
            Console.WriteLine($"Cross-domain patterns: {combinedResult.CrossDomainPatterns.Count}");
            Console.WriteLine($"Pattern conflicts: {combinedResult.PatternConflicts.Count}");
            
            Console.WriteLine($"\nDomain breakdown:");
            foreach (var kvp in combinedResult.DomainResults)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value.DetectedPatterns.Count} patterns, {kvp.Value.DetectedAnomalies.Count} anomalies");
            }

            Console.WriteLine($"\nCombined metrics:");
            foreach (var kvp in combinedResult.CombinedMetrics)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
        }
    }
}
