using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ALARM.DomainLibraries;
using ALARM.DomainLibraries.Core;
using ALARM.Analyzers.PatternDetection;

namespace ALARM.DomainLibraries.IntegrationTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create logger
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<DomainLibrariesAggregator>();

            Console.WriteLine("🔧 Domain Libraries Integration Test");
            Console.WriteLine("=====================================");

            try
            {
                // Test DomainLibrariesAggregator
                var aggregator = new DomainLibrariesAggregator(logger);
                var manager = aggregator.Manager;

                // Test data
                var testData = new PatternData
                {
                    Content = "Sample test content for domain analysis",
                    Metadata = new Dictionary<string, object>
                    {
                        ["source"] = "integration_test",
                        ["type"] = "test_data"
                    }
                };

                Console.WriteLine("\n✅ Domain Libraries Aggregator created successfully");
                Console.WriteLine($"📊 Registered libraries: {manager.GetRegisteredLibraries().Count()}");

                // Test pattern detection across domains
                var patternConfig = new DomainPatternConfig();
                var result = await manager.DetectPatternsAsync(testData, patternConfig);
                
                Console.WriteLine($"🔍 Cross-domain pattern detection completed");
                Console.WriteLine($"   - Patterns found: {result.AllPatterns.Count}");
                Console.WriteLine($"   - Anomalies found: {result.AllAnomalies.Count}");
                Console.WriteLine($"   - Processing time: {result.TotalProcessingTime.TotalMilliseconds}ms");

                Console.WriteLine("\n🎯 Integration test completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Integration test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}