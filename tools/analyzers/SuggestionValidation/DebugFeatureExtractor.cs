using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Debug utility to test Enhanced Feature Extractor functionality
    /// </summary>
    public class DebugFeatureExtractor
    {
        public static async Task RunDebugTests()
        {
            Console.WriteLine("🔍 ENHANCED FEATURE EXTRACTOR DEBUG TESTS");
            Console.WriteLine("=" + new string('=', 50));

            // Create logger
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<EnhancedFeatureExtractor>();
            var extractor = new EnhancedFeatureExtractor(logger);

            // Test 1: Domain Keywords
            Console.WriteLine("\n📋 TEST 1: Domain Keyword Extraction");
            var suggestion1 = "Migrate ADDS v24 to v25 architecture with AutoCAD Map 3D 2025 and Oracle database optimization";
            var keywords = extractor.ExtractDomainKeywords(suggestion1, "ADDS");
            Console.WriteLine($"Suggestion: {suggestion1}");
            Console.WriteLine($"Domain: ADDS");
            Console.WriteLine($"Keywords found: {keywords.Count}");
            foreach (var kv in keywords)
            {
                Console.WriteLine($"  - {kv.Key}: {kv.Value:F2}");
            }

            // Test 2: Oracle Keywords
            Console.WriteLine("\n📋 TEST 2: Oracle Domain Keywords");
            var suggestion2 = "Optimize SQL queries with proper indexes and stored procedures for better database performance";
            var oracleKeywords = extractor.ExtractDomainKeywords(suggestion2, "Oracle");
            Console.WriteLine($"Suggestion: {suggestion2}");
            Console.WriteLine($"Domain: Oracle");
            Console.WriteLine($"Keywords found: {oracleKeywords.Count}");
            foreach (var kv in oracleKeywords)
            {
                Console.WriteLine($"  - {kv.Key}: {kv.Value:F2}");
            }

            // Test 3: Semantic Similarity
            Console.WriteLine("\n📋 TEST 3: Semantic Similarity");
            var text1 = "Optimize database performance with proper indexing";
            var text2 = "Database optimization using index structures for better performance";
            var similarity = extractor.CalculateSemanticSimilarity(text1, text2);
            Console.WriteLine($"Text 1: {text1}");
            Console.WriteLine($"Text 2: {text2}");
            Console.WriteLine($"Similarity: {similarity:F2}");

            // Test 4: Technical Complexity
            Console.WriteLine("\n📋 TEST 4: Technical Complexity");
            var complexSuggestion = "Optimize system performance by implementing caching, improving memory usage, and reducing CPU load for faster response times";
            var context = new ValidationContext
            {
                ComplexityInfo = new SystemComplexityInfo
                {
                    ComplexityScore = 0.6,
                    NumberOfIntegrations = 5
                }
            };
            var complexity = extractor.AssessTechnicalComplexity(complexSuggestion, context);
            Console.WriteLine($"Suggestion: {complexSuggestion}");
            Console.WriteLine($"Technical Complexity: {complexity:F2}");

            // Test 5: Full Feature Extraction
            Console.WriteLine("\n📋 TEST 5: Full Feature Extraction");
            var cadSuggestion = "Migrate ADDS legacy system with AutoCAD Map 3D integration and Oracle database optimization";
            var cadContext = new ValidationContext
            {
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "ADDS" }
                }
            };
            var features = await extractor.ExtractFeaturesAsync(cadSuggestion, cadContext);
            Console.WriteLine($"Suggestion: {cadSuggestion}");
            Console.WriteLine($"CAD Integration Score: {features.CADIntegrationScore:F2}");
            Console.WriteLine($"Database Operation Score: {features.DatabaseOperationScore:F2}");
            Console.WriteLine($"Legacy Migration Score: {features.LegacyMigrationScore:F2}");
            Console.WriteLine($"Technical Complexity: {features.TechnicalComplexity:F2}");

            Console.WriteLine("\n✅ Debug tests completed!");
        }
    }
}
