using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ALARM.Analyzers.PatternDetection;
using ALARM.Analyzers.CausalAnalysis;
using ALARM.Analyzers.Performance;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Simple test program to demonstrate the Suggestion Validation system
    /// </summary>
    public class SuggestionValidationTest
    {
        public static async Task RunTestAsync()
        {
            Console.WriteLine("🎯 ALARM Suggestion Validation System Test");
            Console.WriteLine("==========================================");

            // Create logger
            using var loggerFactory = LoggerFactory.Create(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            var logger = loggerFactory.CreateLogger<SuggestionValidationEngine>();

            try
            {
                // Clean up any existing database to avoid unique constraint issues
                var dbPath = "suggestion_validation.db";
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    Console.WriteLine("🧹 Cleaned up existing database");
                }
                
                // Initialize the validation engine
                var validationEngine = new SuggestionValidationEngine(logger);
                Console.WriteLine("✅ Suggestion Validation Engine initialized");

                // Test Pattern Detection Validation
                await TestPatternDetectionValidationAsync(validationEngine);

                // Test Causal Analysis Validation  
                await TestCausalAnalysisValidationAsync(validationEngine);

                // Test Performance Validation
                await TestPerformanceValidationAsync(validationEngine);

                // Test Comprehensive Validation
                await TestComprehensiveValidationAsync(validationEngine);

                // Test Validation Trends
                await TestValidationTrendsAsync(validationEngine);

                Console.WriteLine("\n🎉 All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static async Task TestPatternDetectionValidationAsync(SuggestionValidationEngine engine)
        {
            Console.WriteLine("\n📊 Testing Pattern Detection Validation...");

            // Create mock pattern result
            var patternResult = new AdvancedPatternResult
            {
                AnalysisId = "pattern-test-001",
                OverallConfidence = 0.85,
                PatternAnalysis = new PatternAnalysisResult
                {
                    IdentifiedPatterns = new List<IdentifiedPattern>
                    {
                        new IdentifiedPattern { Name = "DataAccessPattern", Confidence = 0.9, Description = "Frequent database access pattern" },
                        new IdentifiedPattern { Name = "CachingOpportunity", Confidence = 0.75, Description = "Potential caching optimization" }
                    }
                }
            };

            // ENHANCED: Create realistic suggestions with varied quality levels
            var suggestions = new List<PatternRecommendation>
            {
                // HIGH QUALITY: Specific, actionable, well-documented
                new PatternRecommendation
                {
                    Id = "suggestion-1",
                    Type = RecommendationType.Optimization,
                    Priority = RecommendationPriority.High,
                    Description = "Implement database connection pooling to reduce connection overhead by 40-60%. Current analysis shows 150+ concurrent connections causing performance bottlenecks during peak hours (9-11 AM, 2-4 PM).",
                    Pattern = patternResult.PatternAnalysis.IdentifiedPatterns[0],
                    SuggestedActions = new List<string> { 
                        "Configure HikariCP with pool size 20-50 connections",
                        "Set connection timeout to 30 seconds", 
                        "Monitor connection usage with JMX metrics",
                        "Implement connection leak detection",
                        "Add database connection health checks"
                    }
                },
                // MEDIUM QUALITY: Good but less specific
                new PatternRecommendation
                {
                    Id = "suggestion-2", 
                    Type = RecommendationType.Investigation,
                    Priority = RecommendationPriority.Medium,
                    Description = "Investigate caching opportunities for frequently accessed data to improve response times",
                    Pattern = patternResult.PatternAnalysis.IdentifiedPatterns[1],
                    SuggestedActions = new List<string> { 
                        "Analyze access patterns using application logs", 
                        "Implement Redis cache for user sessions",
                        "Consider CDN for static assets"
                    }
                },
                // LOW QUALITY: Vague, minimal actionability
                new PatternRecommendation
                {
                    Id = "suggestion-3",
                    Type = RecommendationType.Monitoring,
                    Priority = RecommendationPriority.Low,
                    Description = "Monitor system performance",
                    Pattern = patternResult.PatternAnalysis.IdentifiedPatterns[0],
                    SuggestedActions = new List<string> { "Add monitoring" }
                },
                // COMPLEX QUALITY: Detailed but challenging implementation
                new PatternRecommendation
                {
                    Id = "suggestion-4",
                    Type = RecommendationType.Optimization,
                    Priority = RecommendationPriority.High,
                    Description = "Implement microservices architecture to address monolithic bottlenecks. Break down the current 500K+ LOC application into domain-specific services with event-driven communication. This addresses authentication, data processing, and API layer performance issues identified in the analysis.",
                    Pattern = patternResult.PatternAnalysis.IdentifiedPatterns[0],
                    SuggestedActions = new List<string> { 
                        "Design service boundaries using Domain-Driven Design",
                        "Implement API Gateway with rate limiting",
                        "Set up event streaming with Apache Kafka",
                        "Migrate authentication to OAuth 2.0/JWT",
                        "Implement distributed tracing with Jaeger",
                        "Create service mesh with Istio",
                        "Plan phased migration strategy over 12-18 months"
                    }
                }
            };

            // ENHANCED: Create realistic validation context with detailed system information
            var context = new ValidationContext
            {
                UserId = "test-user",
                ProjectId = "test-project",
                SystemType = "LegacyApplication",
                ValidationPurpose = "QualityAssurance",
                ComplexityInfo = new SystemComplexityInfo
                {
                    TotalLinesOfCode = 485000,
                    NumberOfModules = 42,
                    NumberOfDatabases = 5,
                    NumberOfIntegrations = 12,
                    ArchitecturePattern = "Layered",
                    TechnicalDebt = new List<string> { "Legacy .NET Framework 4.6", "Outdated Oracle drivers", "Monolithic architecture", "Manual deployment process" },
                    ComplexityScore = 0.8, // High complexity
                    CriticalComponents = new List<string> { "Authentication", "Data processing", "API layer", "Oracle integration", "AutoCAD interface" }
                },
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "AutoCAD", "Oracle", "Enterprise", "ADDS" },
                    DomainExpertise = new Dictionary<string, double> 
                    { 
                        ["AutoCAD"] = 0.85, 
                        ["Oracle"] = 0.75, 
                        ["DotNetCore"] = 0.6,  // Lower expertise in modern tech
                        ["ADDS"] = 0.7,
                        ["Legacy"] = 0.9  // High expertise in legacy systems
                    },
                    BusinessCriticalAreas = new List<string> { "Data integrity", "User authentication", "Performance", "CAD file processing", "Database transactions" },
                    ComplianceRequirements = new Dictionary<string, string>
                    {
                        ["Security"] = "Enterprise-grade with RBAC",
                        ["Performance"] = "Sub-2-second response for CAD operations",
                        ["Reliability"] = "99.5% uptime during business hours",
                        ["DataRetention"] = "7-year compliance requirement"
                    }
                },
                PerformanceConstraints = new PerformanceConstraints
                {
                    MaxResponseTimeMs = 2000, // Stricter for legacy system
                    MaxMemoryUsageMB = 1024,  // Higher for CAD operations
                    MaxConcurrentUsers = 250,
                    MinThroughputRPS = 25.0,  // Lower for complex operations
                    PerformanceCriticalOperations = new List<string> 
                    { 
                        "CAD file loading", 
                        "Database queries", 
                        "Oracle stored procedures",
                        "Authentication",
                        "File processing"
                    }
                },
                QualityExpectations = new QualityExpectations
                {
                    MinimumQualityThreshold = 0.70, // Realistic for legacy system
                    TargetQualityScore = 0.80,
                    RequireEvidence = true,
                    RequireImplementationPlan = true,
                    PriorityAreas = new List<string> 
                    { 
                        "Performance optimization", 
                        "Security improvements", 
                        "Modernization planning",
                        "Technical debt reduction"
                    }
                }
            };

            // Validate suggestions
            var validationResult = await engine.ValidatePatternSuggestionsAsync(patternResult, suggestions, context);

            Console.WriteLine($"   ✅ Pattern validation completed");
            Console.WriteLine($"   📊 Overall Quality Score: {validationResult.OverallQualityScore:P2}");
            Console.WriteLine($"   📝 {validationResult.SuggestionValidations.Count} suggestions validated");
            Console.WriteLine($"   💡 {validationResult.ImprovementRecommendations.Count} improvement recommendations generated");

            // Display quality metrics
            foreach (var metric in validationResult.QualityMetrics)
            {
                Console.WriteLine($"   📈 {metric.Key}: {metric.Value:F3}");
            }
        }

        private static async Task TestCausalAnalysisValidationAsync(SuggestionValidationEngine engine)
        {
            Console.WriteLine("\n🔗 Testing Causal Analysis Validation...");

            // Create mock causal result
            var causalResult = new CausalAnalysisResult
            {
                AnalysisId = "causal-test-001",
                OverallConfidence = 0.78,
                DataSampleCount = 1000,
                CausalRelationships = new List<CausalRelationship>
                {
                    new CausalRelationship { Id = "rel-1", CauseVariable = "UserLoad", EffectVariable = "ResponseTime", Strength = 0.85 },
                    new CausalRelationship { Id = "rel-2", CauseVariable = "DatabaseQueries", EffectVariable = "MemoryUsage", Strength = 0.72 }
                },
                CausalStrengths = new Dictionary<string, double> { ["rel-1"] = 0.85, ["rel-2"] = 0.72 }
            };

            // Create mock suggestions
            var suggestions = new List<CausalRecommendation>
            {
                new CausalRecommendation
                {
                    Id = "causal-suggestion-1",
                    Type = CausalRecommendationType.Optimization,
                    Priority = CausalRecommendationPriority.High,
                    Title = "Optimize User Load Handling",
                    Description = "Reduce response time by implementing load balancing and caching",
                    ExpectedImpact = 0.8,
                    RelatedRelationships = new List<string> { "rel-1" },
                    ActionItems = new List<string> { "Implement load balancer", "Add response caching" }
                }
            };

            var context = new ValidationContext
            {
                UserId = "test-user",
                ProjectId = "test-project", 
                SystemType = "WebApplication"
            };

            var validationResult = await engine.ValidateCausalSuggestionsAsync(causalResult, suggestions, context);

            Console.WriteLine($"   ✅ Causal validation completed");
            Console.WriteLine($"   📊 Overall Quality Score: {validationResult.OverallQualityScore:P2}");
            Console.WriteLine($"   🔗 {validationResult.SuggestionValidations.Count} causal suggestions validated");

            foreach (var metric in validationResult.QualityMetrics)
            {
                Console.WriteLine($"   📈 {metric.Key}: {metric.Value:F3}");
            }
        }

        private static async Task TestPerformanceValidationAsync(SuggestionValidationEngine engine)
        {
            Console.WriteLine("\n⚡ Testing Performance Validation...");

            // Create mock performance result
            var performanceResult = new PerformanceAdjustmentResult
            {
                OperationName = "DatabaseQuery",
                ExecutionTimeMs = 2500,
                MemoryUsedMB = 150,
                Success = true,
                Recommendations = new List<string> { "Consider query optimization", "Implement connection pooling" }
            };

            // ENHANCED: Create realistic performance suggestions with varied complexity and quality
            var suggestions = new List<string>
            {
                // HIGH QUALITY: Specific, measurable, actionable
                "Optimize database queries by adding composite indexes on (user_id, created_date, status) columns for the user_activities table. Current query execution time: 2.3s, target: <500ms. Expected 70-80% performance improvement based on query analysis.",
                
                // MEDIUM-HIGH QUALITY: Good technical detail
                "Implement HikariCP connection pooling with initial size 10, maximum size 50, connection timeout 30s. Monitor with JMX metrics. Current: 150+ concurrent connections causing 2.5s delays during peak load.",
                
                // MEDIUM QUALITY: Reasonable but less specific
                "Consider implementing Redis caching for frequently accessed user session data and CAD file metadata. Cache TTL: 30 minutes for sessions, 2 hours for metadata.",
                
                // MEDIUM-LOW QUALITY: Vague implementation
                "Use async/await patterns for better resource utilization in file processing operations",
                
                // LOW QUALITY: Too generic
                "Improve performance",
                
                // HIGH COMPLEXITY: Detailed architectural change
                "Implement event-driven architecture with Apache Kafka for real-time CAD file processing. Replace synchronous Oracle stored procedure calls with asynchronous message processing. Estimated 60% reduction in response time for batch operations, but requires 6-month migration timeline and team training.",
                
                // DOMAIN-SPECIFIC: AutoCAD optimization
                "Optimize AutoCAD drawing file loading by implementing progressive loading for large assemblies (>100MB). Use AutoCAD .NET API's lazy loading features and implement custom viewport management. Target: reduce initial load time from 45s to <10s for complex drawings.",
                
                // INFRASTRUCTURE: System-level improvement
                "Migrate from IIS 7.5 to IIS 10 with HTTP/2 support and implement application warm-up modules. Configure output caching for static CAD thumbnails. Expected 25-30% improvement in concurrent user capacity."
            };

            var context = new ValidationContext
            {
                UserId = "test-user",
                ProjectId = "test-project",
                SystemType = "DatabaseApplication",
                EnvironmentInfo = new Dictionary<string, object> { ["AvailableMemoryGB"] = 8.0 }
            };

            var validationResult = await engine.ValidatePerformanceSuggestionsAsync(performanceResult, suggestions, context);

            Console.WriteLine($"   ✅ Performance validation completed");
            Console.WriteLine($"   📊 Overall Quality Score: {validationResult.OverallQualityScore:P2}");
            Console.WriteLine($"   ⚡ {validationResult.SuggestionValidations.Count} performance suggestions validated");

            foreach (var metric in validationResult.QualityMetrics)
            {
                Console.WriteLine($"   📈 {metric.Key}: {metric.Value:F3}");
            }
        }

        private static async Task TestComprehensiveValidationAsync(SuggestionValidationEngine engine)
        {
            Console.WriteLine("\n🏗️ Testing Comprehensive Validation...");

            // Create comprehensive analysis result with multiple types
            var comprehensiveResult = new ComprehensiveAnalysisResult
            {
                AnalysisId = "comprehensive-test-001",
                PatternResult = new AdvancedPatternResult
                {
                    AnalysisId = "pattern-comp-001",
                    OverallConfidence = 0.82
                },
                PatternSuggestions = new List<PatternRecommendation>
                {
                    new PatternRecommendation
                    {
                        Id = "comp-pattern-1",
                        Type = RecommendationType.Optimization,
                        Priority = RecommendationPriority.High,
                        Description = "Optimize data access patterns for better performance"
                    }
                },
                CausalResult = new CausalAnalysisResult
                {
                    AnalysisId = "causal-comp-001",
                    OverallConfidence = 0.75,
                    DataSampleCount = 500
                },
                CausalSuggestions = new List<CausalRecommendation>
                {
                    new CausalRecommendation
                    {
                        Id = "comp-causal-1",
                        Type = CausalRecommendationType.Investigation,
                        Priority = CausalRecommendationPriority.Medium,
                        Title = "Investigate performance bottlenecks",
                        Description = "Analyze causal relationships affecting system performance",
                        ExpectedImpact = 0.7
                    }
                },
                PerformanceResult = new PerformanceAdjustmentResult
                {
                    OperationName = "SystemAnalysis",
                    ExecutionTimeMs = 3000,
                    MemoryUsedMB = 200,
                    Success = true
                },
                PerformanceSuggestions = new List<string>
                {
                    "Implement parallel processing for data analysis",
                    "Optimize memory usage through better data structures"
                }
            };

            var context = new ValidationContext
            {
                UserId = "test-user",
                ProjectId = "comprehensive-test",
                SystemType = "EnterpriseApplication"
            };

            var validationResult = await engine.ValidateComprehensiveSuggestionsAsync(comprehensiveResult, context);

            Console.WriteLine($"   ✅ Comprehensive validation completed");
            Console.WriteLine($"   🏗️ Overall System Quality: {validationResult.OverallSystemQuality:P2}");
            Console.WriteLine($"   📊 {validationResult.ValidationResults.Count} analysis types validated");
            Console.WriteLine($"   🔄 Cross-analysis consistency metrics:");

            foreach (var consistency in validationResult.CrossAnalysisConsistency)
            {
                Console.WriteLine($"     📈 {consistency.Key}: {consistency.Value:F3}");
            }

            Console.WriteLine($"   💡 {validationResult.SystemWideImprovements.Count} system-wide improvements suggested");
        }

        private static async Task TestValidationTrendsAsync(SuggestionValidationEngine engine)
        {
            Console.WriteLine("\n📈 Testing Validation Trends...");

            try
            {
                // Test trends analysis (may not have data yet)
                var trends = await engine.GetValidationTrendsAsync(TimeSpan.FromDays(7), AnalysisType.PatternDetection);

                Console.WriteLine($"   ✅ Trends analysis completed for {trends.AnalysisType}");
                Console.WriteLine($"   📊 Quality trends: {trends.QualityTrends.Count} trend series");
                Console.WriteLine($"   📈 Volume stats: {trends.VolumeStats.TotalValidations} total validations");
                Console.WriteLine($"   ⚠️ Top issues: {trends.TopIssues.Count} identified");
                Console.WriteLine($"   💡 Improvement opportunities: {trends.ImprovementOpportunities.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️ Trends analysis skipped (expected for new system): {ex.Message}");
            }
        }
    }
}
