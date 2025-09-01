using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace ALARM.Analyzers.SuggestionValidation.Tests
{
    /// <summary>
    /// Comprehensive test suite for EnhancedFeatureExtractor
    /// Phase 2 Implementation: Automated testing for advanced feature engineering
    /// </summary>
    public class EnhancedFeatureExtractorTests
    {
        private readonly EnhancedFeatureExtractor _extractor;
        private readonly Mock<ILogger<EnhancedFeatureExtractor>> _mockLogger;

        public EnhancedFeatureExtractorTests()
        {
            _mockLogger = new Mock<ILogger<EnhancedFeatureExtractor>>();
            _extractor = new EnhancedFeatureExtractor(_mockLogger.Object);
        }

        #region Domain Keyword Extraction Tests

        [Fact]
        public async Task ExtractDomainKeywords_ADDSContext_ReturnsExpectedKeywords()
        {
            // Arrange
            var suggestion = "Migrate ADDS v24 to v25 architecture with AutoCAD Map 3D 2025 and Oracle database optimization";
            var domain = "ADDS";

            // Act
            var keywords = _extractor.ExtractDomainKeywords(suggestion, domain);

            // Assert
            Assert.True(keywords.ContainsKey("AutoCAD"));
            Assert.True(keywords.ContainsKey("Oracle"));
            Assert.True(keywords.ContainsKey("Migration"));
            Assert.True(keywords.ContainsKey("Architecture"));
            Assert.True(keywords.ContainsKey("Database"));
            Assert.True(keywords.ContainsKey("Optimization"));

            // Verify confidence scores
            Assert.True(keywords["AutoCAD"] >= 0.4);
            Assert.True(keywords["Oracle"] >= 0.4);
            Assert.True(keywords["Migration"] >= 0.4);
        }

        [Fact]
        public async Task ExtractDomainKeywords_AutoCADContext_ReturnsCADSpecificKeywords()
        {
            // Arrange
            var suggestion = "Create new drawing template with standard layers, blocks, and text styles for DWG files";
            var domain = "AutoCAD";

            // Act
            var keywords = _extractor.ExtractDomainKeywords(suggestion, domain);

            // Assert
            Assert.True(keywords.ContainsKey("Drawing"));
            Assert.True(keywords.ContainsKey("Layer"));
            Assert.True(keywords.ContainsKey("Block"));
            Assert.True(keywords.ContainsKey("Text"));
            Assert.True(keywords.ContainsKey("DWG"));
            Assert.True(keywords.ContainsKey("Template"));
            Assert.True(keywords.ContainsKey("Standard"));
        }

        [Fact]
        public async Task ExtractDomainKeywords_OracleContext_ReturnsDatabaseKeywords()
        {
            // Arrange
            var suggestion = "Optimize SQL queries with proper indexes and stored procedures for better database performance";
            var domain = "Oracle";

            // Act
            var keywords = _extractor.ExtractDomainKeywords(suggestion, domain);

            // Assert
            Assert.True(keywords.ContainsKey("SQL"));
            Assert.True(keywords.ContainsKey("Index"));
            Assert.True(keywords.ContainsKey("Procedure"));
            Assert.True(keywords.ContainsKey("Database"));
            Assert.True(keywords.ContainsKey("Performance"));
            Assert.True(keywords.ContainsKey("Optimization"));
        }

        [Fact]
        public async Task ExtractDomainKeywords_EmptyText_ReturnsEmptyDictionary()
        {
            // Arrange
            var suggestion = "";
            var domain = "ADDS";

            // Act
            var keywords = _extractor.ExtractDomainKeywords(suggestion, domain);

            // Assert
            Assert.Empty(keywords);
        }

        [Fact]
        public async Task ExtractDomainKeywords_UnknownDomain_UsesGenericKeywords()
        {
            // Arrange
            var suggestion = "Implement new features and optimize existing functionality";
            var domain = "UnknownDomain";

            // Act
            var keywords = _extractor.ExtractDomainKeywords(suggestion, domain);

            // Assert
            Assert.True(keywords.ContainsKey("Implement"));
            Assert.True(keywords.ContainsKey("Optimize"));
        }

        #endregion

        #region Semantic Similarity Tests

        [Fact]
        public async Task CalculateSemanticSimilarity_SimilarTexts_ReturnsHighSimilarity()
        {
            // Arrange
            var suggestion = "Optimize database performance with proper indexing";
            var context = "Database optimization using index structures for better performance";

            // Act
            var similarity = _extractor.CalculateSemanticSimilarity(suggestion, context);

            // Assert
            Assert.True(similarity >= 0.6, $"Expected similarity >= 0.6, got {similarity}");
        }

        [Fact]
        public async Task CalculateSemanticSimilarity_DifferentTexts_ReturnsLowSimilarity()
        {
            // Arrange
            var suggestion = "Draw new AutoCAD blocks with layers";
            var context = "Configure Oracle database connection strings";

            // Act
            var similarity = _extractor.CalculateSemanticSimilarity(suggestion, context);

            // Assert
            Assert.True(similarity <= 0.3, $"Expected similarity <= 0.3, got {similarity}");
        }

        [Fact]
        public async Task CalculateSemanticSimilarity_EmptyTexts_ReturnsZero()
        {
            // Arrange
            var suggestion = "";
            var context = "";

            // Act
            var similarity = _extractor.CalculateSemanticSimilarity(suggestion, context);

            // Assert
            Assert.Equal(0.0, similarity);
        }

        #endregion

        #region Technical Complexity Tests

        [Fact]
        public async Task AssessTechnicalComplexity_HighComplexitySuggestion_ReturnsHighScore()
        {
            // Arrange
            var suggestion = "Refactor the entire system architecture with database migration and API integration optimization";
            var context = CreateTestValidationContext("High");

            // Act
            var complexity = _extractor.AssessTechnicalComplexity(suggestion, context);

            // Assert
            Assert.True(complexity >= 0.7, $"Expected complexity >= 0.7, got {complexity}");
        }

        [Fact]
        public async Task AssessTechnicalComplexity_LowComplexitySuggestion_ReturnsLowScore()
        {
            // Arrange
            var suggestion = "Update documentation and add comments to the code";
            var context = CreateTestValidationContext("Low");

            // Act
            var complexity = _extractor.AssessTechnicalComplexity(suggestion, context);

            // Assert
            Assert.True(complexity <= 0.4, $"Expected complexity <= 0.4, got {complexity}");
        }

        [Fact]
        public async Task AssessTechnicalComplexity_EmptyText_ReturnsZero()
        {
            // Arrange
            var suggestion = "";
            var context = CreateTestValidationContext("Medium");

            // Act
            var complexity = _extractor.AssessTechnicalComplexity(suggestion, context);

            // Assert
            Assert.Equal(0.0, complexity);
        }

        #endregion

        #region Enhanced Feature Extraction Tests

        [Fact]
        public async Task ExtractFeaturesAsync_ComprehensiveSuggestion_ReturnsCompleteFeatureSet()
        {
            // Arrange
            var suggestion = "Migrate ADDS v24 to v25 architecture with AutoCAD Map 3D 2025, implement new Oracle database optimization with 50% performance improvement, and configure new API endpoints for better integration.";
            var context = CreateComprehensiveValidationContext();

            // Act
            var features = await _extractor.ExtractFeaturesAsync(suggestion, context);

            // Assert
            // Basic features
            Assert.True(features.WordCount > 20);
            Assert.True(features.CharacterCount > 100);
            Assert.True(features.SentenceCount >= 1);

            // Enhanced features
            Assert.True(features.DomainKeywords.Count > 0);
            Assert.True(features.SemanticComplexity > 0);
            Assert.True(features.TechnicalComplexity > 0);
            Assert.True(features.ContextualRelevance > 0);

            // Advanced features
            Assert.True(features.ActionVerbCount > 0);
            Assert.True(features.TechnicalTermCount > 0);
            Assert.True(features.QuantifiableElementCount > 0);
            Assert.True(features.SpecificityScore > 0);

            // Domain-specific features
            Assert.True(features.CADIntegrationScore > 0);
            Assert.True(features.DatabaseOperationScore > 0);
            Assert.True(features.LegacyMigrationScore > 0);
            Assert.True(features.PerformanceImpactScore > 0);
        }

        [Fact]
        public async Task ExtractFeaturesAsync_ADDSSuggestion_ReturnsHighDomainScores()
        {
            // Arrange
            var suggestion = "Migrate ADDS legacy system with AutoCAD Map 3D integration and Oracle database optimization";
            var context = CreateADDSValidationContext();

            // Act
            var features = await _extractor.ExtractFeaturesAsync(suggestion, context);

            // Assert
            Assert.True(features.CADIntegrationScore >= 0.3, $"CAD Integration Score: {features.CADIntegrationScore}");
            Assert.True(features.DatabaseOperationScore >= 0.2, $"Database Operation Score: {features.DatabaseOperationScore}");
            Assert.True(features.LegacyMigrationScore >= 0.4, $"Legacy Migration Score: {features.LegacyMigrationScore}");
        }

        [Fact]
        public async Task ExtractFeaturesAsync_PerformanceSuggestion_ReturnsHighPerformanceScore()
        {
            // Arrange
            var suggestion = "Optimize system performance by implementing caching, improving memory usage, and reducing CPU load for faster response times";
            var context = CreatePerformanceValidationContext();

            // Act
            var features = await _extractor.ExtractFeaturesAsync(suggestion, context);

            // Assert
            Assert.True(features.PerformanceImpactScore >= 0.6, $"Performance Impact Score: {features.PerformanceImpactScore}");
            Assert.True(features.TechnicalComplexity >= 0.4, $"Technical Complexity: {features.TechnicalComplexity}");
        }

        #endregion

        #region Feature Dictionary Conversion Tests

        [Fact]
        public async Task ToFeatureDictionary_CompleteFeatureSet_ReturnsAllFeatures()
        {
            // Arrange
            var suggestion = "Implement AutoCAD integration with Oracle database optimization";
            var context = CreateTestValidationContext("Medium");
            var features = await _extractor.ExtractFeaturesAsync(suggestion, context);

            // Act
            var featureDict = features.ToFeatureDictionary();

            // Assert
            // Verify basic features are present
            Assert.True(featureDict.ContainsKey("WordCount"));
            Assert.True(featureDict.ContainsKey("CharacterCount"));
            Assert.True(featureDict.ContainsKey("SentenceCount"));

            // Verify enhanced features are present
            Assert.True(featureDict.ContainsKey("SemanticComplexity"));
            Assert.True(featureDict.ContainsKey("TechnicalComplexity"));
            Assert.True(featureDict.ContainsKey("ContextualRelevance"));

            // Verify domain-specific features are present
            Assert.True(featureDict.ContainsKey("CADIntegrationScore"));
            Assert.True(featureDict.ContainsKey("DatabaseOperationScore"));
            Assert.True(featureDict.ContainsKey("LegacyMigrationScore"));
            Assert.True(featureDict.ContainsKey("PerformanceImpactScore"));

            // Verify all values are numeric
            foreach (var feature in featureDict)
            {
                Assert.True(feature.Value >= 0.0, $"Feature {feature.Key} has negative value: {feature.Value}");
                Assert.True(feature.Value <= 100.0, $"Feature {feature.Key} has unreasonably high value: {feature.Value}");
            }
        }

        #endregion

        #region Performance and Edge Case Tests

        [Fact]
        public async Task ExtractFeaturesAsync_LargeSuggestion_CompletesInReasonableTime()
        {
            // Arrange
            var largeSuggestion = string.Join(" ", Enumerable.Repeat(
                "Implement comprehensive system architecture migration with AutoCAD integration Oracle database optimization performance improvements", 100));
            var context = CreateTestValidationContext("High");
            var startTime = DateTime.UtcNow;

            // Act
            var features = await _extractor.ExtractFeaturesAsync(largeSuggestion, context);
            var duration = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(duration.TotalMilliseconds < 5000, $"Feature extraction took too long: {duration.TotalMilliseconds}ms");
            Assert.True(features.WordCount > 500);
        }

        [Fact]
        public async Task ExtractFeaturesAsync_SpecialCharacters_HandlesGracefully()
        {
            // Arrange
            var suggestion = "Implement @#$%^&*() system with 100% performance improvement! Are you sure? Yes, definitely.";
            var context = CreateTestValidationContext("Medium");

            // Act
            var features = await _extractor.ExtractFeaturesAsync(suggestion, context);

            // Assert
            Assert.True(features.WordCount > 0);
            Assert.True(features.QuantifiableElementCount > 0); // Should detect "100%"
            Assert.True(features.SentenceCount >= 2); // Should detect multiple sentences
        }

        [Fact]
        public async Task ExtractFeaturesAsync_NullContext_HandlesGracefully()
        {
            // Arrange
            var suggestion = "Implement new features";
            ValidationContext? context = null;

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => 
                await _extractor.ExtractFeaturesAsync(suggestion, context!));
            
            // Should not throw exception, should handle gracefully
            Assert.Null(exception);
        }

        #endregion

        #region Helper Methods

        private ValidationContext CreateTestValidationContext(string complexityLevel)
        {
            var complexityScore = complexityLevel == "High" ? 0.8 : complexityLevel == "Medium" ? 0.6 : 0.4;
            
            return new ValidationContext
            {
                ComplexityInfo = new SystemComplexityInfo
                {
                    ComplexityScore = complexityScore,
                    NumberOfIntegrations = complexityLevel == "High" ? 8 : complexityLevel == "Medium" ? 5 : 2
                },
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "ADDS" },
                    DomainExpertise = new Dictionary<string, double>
                    {
                        ["AutoCAD"] = 0.9,
                        ["Oracle"] = 0.8,
                        [".NET Core"] = 0.7
                    }
                }
            };
        }

        private ValidationContext CreateComprehensiveValidationContext()
        {
            return new ValidationContext
            {
                ComplexityInfo = new SystemComplexityInfo
                {
                    ComplexityScore = 0.8,
                    NumberOfIntegrations = 7,
                    NumberOfModules = 15,
                    TechnicalDebt = new List<string> { "Legacy components", "Technical debt" }
                },
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "ADDS" },
                    DomainExpertise = new Dictionary<string, double>
                    {
                        ["AutoCAD"] = 0.95,
                        ["Oracle"] = 0.90,
                        [".NET Core"] = 0.85,
                        ["GIS"] = 0.80
                    }
                },
                PerformanceConstraints = new PerformanceConstraints
                {
                    MaxResponseTimeMs = 2000,
                    MaxMemoryUsageMB = 512,
                    MaxConcurrentUsers = 80,
                    MinThroughputRPS = 100
                },
                QualityExpectations = new QualityExpectations
                {
                    MinimumQualityThreshold = 0.85,
                    TargetQualityScore = 0.90
                }
            };
        }

        private ValidationContext CreateADDSValidationContext()
        {
            return new ValidationContext
            {
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "ADDS" },
                    DomainExpertise = new Dictionary<string, double>
                    {
                        ["AutoCAD"] = 0.95,
                        ["Oracle"] = 0.90,
                        ["GIS"] = 0.85
                    }
                }
            };
        }

        private ValidationContext CreatePerformanceValidationContext()
        {
            return new ValidationContext
            {
                PerformanceConstraints = new PerformanceConstraints
                {
                    MaxResponseTimeMs = 1000,
                    MaxMemoryUsageMB = 256,
                    MaxConcurrentUsers = 70,
                    MinThroughputRPS = 200
                }
            };
        }

        #endregion
    }
}
