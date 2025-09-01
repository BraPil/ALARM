using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using ALARM.Analyzers.SuggestionValidation.Models;

namespace ALARM.Analyzers.SuggestionValidation.Tests
{
    public class InnovationAndRiskAssessmentTests
    {
        private readonly InnovationAndRiskAssessment _assessmentEngine;
        private readonly ILogger<InnovationAndRiskAssessment> _logger;

        public InnovationAndRiskAssessmentTests()
        {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<InnovationAndRiskAssessment>();
            _assessmentEngine = new InnovationAndRiskAssessment(_logger);
        }

        [Fact]
        public async Task AssessInnovationAndRiskAsync_ValidInput_ReturnsResult()
        {
            // Arrange
            var suggestionText = "Implement innovative machine learning algorithm for pattern detection with advanced neural networks";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(suggestionText, result.SuggestionText);
            Assert.Equal(AnalysisType.PatternDetection, result.AnalysisType);
            Assert.True(result.AssessmentTimestamp > DateTime.MinValue);
            Assert.NotNull(result.InnovationAssessment);
            Assert.NotNull(result.RiskAssessment);
            Assert.InRange(result.RiskAdjustedScore, 0.0, 1.0);
            Assert.NotEmpty(result.Recommendations);
        }

        [Theory]
        [InlineData("Implement revolutionary AI-powered breakthrough solution with cutting-edge neural networks", InnovationLevel.Revolutionary)]
        [InlineData("Use innovative machine learning approach with novel algorithms", InnovationLevel.Highly_Innovative)]
        [InlineData("Apply creative solution with alternative approach", InnovationLevel.Moderately_Innovative)]
        [InlineData("Implement solution with some unique features", InnovationLevel.Somewhat_Innovative)]
        [InlineData("Use standard approach with basic implementation", InnovationLevel.Conventional)]
        public async Task InnovationAssessment_VariousInnovationLevels_AssignsCorrectLevel(
            string suggestionText, InnovationLevel expectedLevel)
        {
            // Arrange
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.Equal(expectedLevel, result.InnovationAssessment.InnovationLevel);
        }

        [Theory]
        [InlineData("Simple implementation with well-documented approach", RiskLevel.Minimal)]
        [InlineData("Standard solution with some complexity", RiskLevel.Low)]
        [InlineData("Complex implementation with multiple technologies", RiskLevel.Medium)]
        [InlineData("High complexity solution with breaking changes and performance impact", RiskLevel.High)]
        [InlineData("Critical system changes with downtime and security vulnerabilities", RiskLevel.Critical)]
        public async Task RiskAssessment_VariousRiskLevels_AssignsCorrectLevel(
            string suggestionText, RiskLevel expectedLevel)
        {
            // Arrange
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PerformanceOptimization);

            // Assert
            Assert.Equal(expectedLevel, result.RiskAssessment.RiskLevel);
        }

        [Fact]
        public async Task NoveltyAssessment_HighNoveltyText_ReturnsHighScore()
        {
            // Arrange
            var suggestionText = "Implement groundbreaking revolutionary approach using cutting-edge AI and novel machine learning techniques";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.True(result.InnovationAssessment.NoveltyScore > 0.7);
        }

        [Fact]
        public async Task CreativityAssessment_CreativeText_ReturnsHighScore()
        {
            // Arrange
            var suggestionText = "Creative out-of-the-box solution with imaginative approach using multiple alternative methods";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.CausalAnalysis);

            // Assert
            Assert.True(result.InnovationAssessment.CreativityScore > 0.6);
        }

        [Fact]
        public async Task TechnicalAdvancementAssessment_AdvancedTech_ReturnsHighScore()
        {
            // Arrange
            var suggestionText = "Advanced architecture with sophisticated algorithms, distributed scalability, and .NET Core 8 implementation";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PerformanceOptimization);

            // Assert
            Assert.True(result.InnovationAssessment.TechnicalAdvancementScore > 0.6);
        }

        [Fact]
        public async Task TechnicalComplexityRisk_ComplexText_ReturnsHighRisk()
        {
            // Arrange
            var suggestionText = "Complex sophisticated multi-layered distributed concurrent system with intricate parallel processing";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.True(result.RiskAssessment.TechnicalComplexityRisk > 0.6);
        }

        [Fact]
        public async Task CompatibilityRisk_BreakingChanges_ReturnsHighRisk()
        {
            // Arrange
            var suggestionText = "Breaking change with incompatible legacy system requiring migration from deprecated .NET Framework";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.ComprehensiveAnalysis);

            // Assert
            Assert.True(result.RiskAssessment.CompatibilityRisk > 0.5);
        }

        [Fact]
        public async Task PerformanceImpactRisk_PerformanceIssues_ReturnsHighRisk()
        {
            // Arrange
            var suggestionText = "Performance impact with slow database queries causing bottlenecks and high memory usage";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PerformanceOptimization);

            // Assert
            Assert.True(result.RiskAssessment.PerformanceImpactRisk > 0.5);
        }

        [Fact]
        public async Task SecurityRisk_SecurityConcerns_ReturnsHighRisk()
        {
            // Arrange
            var suggestionText = "User input handling with database access and sensitive data processing vulnerable to SQL injection";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.True(result.RiskAssessment.SecurityRisk > 0.4);
        }

        [Fact]
        public async Task RiskAdjustedScore_HighInnovationLowRisk_ReturnsHighScore()
        {
            // Arrange
            var suggestionText = "Innovative well-documented modular solution with clean code and comprehensive unit tests";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.True(result.RiskAdjustedScore > 0.6);
        }

        [Fact]
        public async Task RiskAdjustedScore_LowInnovationHighRisk_ReturnsLowScore()
        {
            // Arrange
            var suggestionText = "Standard approach with complex implementation causing downtime and security vulnerabilities";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PerformanceOptimization);

            // Assert
            Assert.True(result.RiskAdjustedScore < 0.5);
        }

        [Fact]
        public async Task Recommendations_HighRisk_ContainsRiskMitigation()
        {
            // Arrange
            var suggestionText = "Complex system with breaking changes causing downtime and security vulnerabilities";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.Contains(result.Recommendations, r => r.Contains("risk") || r.Contains("mitigation"));
        }

        [Fact]
        public async Task Recommendations_LowInnovation_ContainsInnovationSuggestions()
        {
            // Arrange
            var suggestionText = "Basic standard implementation";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.CausalAnalysis);

            // Assert
            Assert.Contains(result.Recommendations, r => r.Contains("innovative") || r.Contains("alternative"));
        }

        [Fact]
        public async Task ADDSSpecificAssessment_ADDSContext_ConsidersADDSFactors()
        {
            // Arrange
            var suggestionText = "AutoCAD Map3D integration with Oracle database migration for ADDS system";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.ComprehensiveAnalysis);

            // Assert
            Assert.True(result.RiskAssessment.CompatibilityRisk > 0.3); // ADDS-specific risks considered
        }

        [Theory]
        [InlineData(AnalysisType.PatternDetection)]
        [InlineData(AnalysisType.CausalAnalysis)]
        [InlineData(AnalysisType.PerformanceOptimization)]
        [InlineData(AnalysisType.ComprehensiveAnalysis)]
        public async Task AssessInnovationAndRiskAsync_AllAnalysisTypes_ReturnsValidResults(AnalysisType analysisType)
        {
            // Arrange
            var suggestionText = "Innovative solution with moderate complexity and good documentation";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, analysisType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(analysisType, result.AnalysisType);
            Assert.InRange(result.InnovationAssessment.OverallInnovationScore, 0.0, 1.0);
            Assert.InRange(result.RiskAssessment.OverallRiskScore, 0.0, 1.0);
            Assert.InRange(result.RiskAdjustedScore, 0.0, 1.0);
        }

        [Fact]
        public async Task InnovationScores_AllDimensions_WithinValidRange()
        {
            // Arrange
            var suggestionText = "Comprehensive innovative solution with creative approach and advanced technical implementation";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            var innovation = result.InnovationAssessment;
            Assert.InRange(innovation.NoveltyScore, 0.0, 1.0);
            Assert.InRange(innovation.CreativityScore, 0.0, 1.0);
            Assert.InRange(innovation.TechnicalAdvancementScore, 0.0, 1.0);
            Assert.InRange(innovation.ApproachUniquenessScore, 0.0, 1.0);
            Assert.InRange(innovation.ProblemSolvingInnovationScore, 0.0, 1.0);
            Assert.InRange(innovation.OverallInnovationScore, 0.0, 1.0);
        }

        [Fact]
        public async Task RiskScores_AllDimensions_WithinValidRange()
        {
            // Arrange
            var suggestionText = "Complex system with potential compatibility and performance issues";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PerformanceOptimization);

            // Assert
            var risk = result.RiskAssessment;
            Assert.InRange(risk.TechnicalComplexityRisk, 0.0, 1.0);
            Assert.InRange(risk.CompatibilityRisk, 0.0, 1.0);
            Assert.InRange(risk.PerformanceImpactRisk, 0.0, 1.0);
            Assert.InRange(risk.MaintenanceRisk, 0.0, 1.0);
            Assert.InRange(risk.SecurityRisk, 0.0, 1.0);
            Assert.InRange(risk.BusinessContinuityRisk, 0.0, 1.0);
            Assert.InRange(risk.OverallRiskScore, 0.0, 1.0);
        }

        [Fact]
        public async Task PerformanceConstraints_StrictConstraints_IncreasesRisk()
        {
            // Arrange
            var suggestionText = "Database intensive solution with complex queries";
            var context = CreateTestValidationContext();
            context.PerformanceConstraints = new PerformanceConstraints
            {
                MaxResponseTimeMs = 500, // Strict response time
                MaxMemoryUsageMB = 256   // Memory constraints
            };

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PerformanceOptimization);

            // Assert
            Assert.True(result.RiskAssessment.PerformanceImpactRisk > 0.4);
        }

        [Fact]
        public async Task EmptyInput_HandledGracefully()
        {
            // Arrange
            var suggestionText = "";
            var context = CreateTestValidationContext();

            // Act
            var result = await _assessmentEngine.AssessInnovationAndRiskAsync(
                suggestionText, context, AnalysisType.PatternDetection);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result.SuggestionText);
            Assert.InRange(result.RiskAdjustedScore, 0.0, 1.0);
        }

        private ValidationContext CreateTestValidationContext()
        {
            return new ValidationContext
            {
                ComplexityInfo = new SystemComplexityInfo
                {
                    NumberOfModules = 50,
                    TotalLinesOfCode = 5000,
                    ComplexityScore = 0.6
                },
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "Performance", "Security", "Database" },
                    DomainExpertise = new Dictionary<string, double>
                    {
                        ["Performance"] = 0.8,
                        ["Security"] = 0.7,
                        ["Database"] = 0.9
                    }
                },
                PerformanceConstraints = new PerformanceConstraints
                {
                    MaxResponseTimeMs = 2000,
                    MaxMemoryUsageMB = 1024,
                    MinThroughputRPS = 100
                },
                QualityExpectations = new QualityExpectations
                {
                    MinimumQualityThreshold = 0.7
                }
            };
        }
    }
}
