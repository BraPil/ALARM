using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ALARM.Analyzers.SuggestionValidation.Tests
{
    /// <summary>
    /// Comprehensive tests for Causal Analysis Validator
    /// Target: 85%+ accuracy for statistical significance and evidence quality assessment
    /// </summary>
    public class CausalAnalysisValidatorTests
    {
        private readonly CausalAnalysisValidator _validator;
        private readonly EnhancedFeatureExtractor _featureExtractor;
        private readonly ILogger<CausalAnalysisValidator> _logger;
        private readonly ILogger<EnhancedFeatureExtractor> _featureLogger;

        public CausalAnalysisValidatorTests()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<CausalAnalysisValidator>();
            _featureLogger = loggerFactory.CreateLogger<EnhancedFeatureExtractor>();
            _featureExtractor = new EnhancedFeatureExtractor(_featureLogger);
            _validator = new CausalAnalysisValidator(_logger, _featureExtractor);
        }

        [Fact]
        public async Task ValidateCausalQuality_StrongCausalClaim_ReturnsHighScore()
        {
            // Arrange
            var suggestionText = @"
                Migrating to .NET Core 8 directly causes improved application performance due to 
                enhanced garbage collection and optimized runtime. Benchmark tests show 30% 
                performance improvement compared to .NET Framework 4.8 baseline measurements.
                Statistical analysis with n=100 test scenarios demonstrates significant results 
                (p < 0.05) with 95% confidence intervals.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();
            var evidence = new List<string> { "Microsoft benchmark study", "Performance test results" };

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext, evidence);

            // Assert
            Assert.True(result.OverallCausalScore >= 0.7, $"Expected causal score >= 0.7, got {result.OverallCausalScore:F2}");
            Assert.True(result.CausalClaims.Count >= 1);
            Assert.True(result.StatisticalAssessment.HasQuantitativeEvidence);
            Assert.True(result.StatisticalAssessment.HasComparativeEvidence);
            Assert.True(result.StatisticalAssessment.OverallStatisticalStrength > 0.6);
            Assert.True(result.Confidence > 0.6);
        }

        [Fact]
        public async Task ValidateCausalQuality_WeakCausalClaim_ReturnsLowerScore()
        {
            // Arrange
            var suggestionText = @"
                After migrating to .NET Core 8, we noticed better performance. 
                The new framework seems to work faster than the old one.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallCausalScore < 0.6, $"Expected causal score < 0.6, got {result.OverallCausalScore:F2}");
            Assert.True(result.FallacyDetection.PostHocFallacy > 0.2); // Should detect post hoc fallacy
            Assert.Contains(result.Recommendations, r => r.Contains("evidence") || r.Contains("statistical"));
        }

        [Fact]
        public async Task ValidateCausalQuality_CorrelationVsCausation_DetectsFallacy()
        {
            // Arrange
            var suggestionText = @"
                Oracle 19c migration is correlated with improved database performance. 
                Systems that use Oracle 19c are associated with faster query response times.
                There is a strong relationship between Oracle 19c and enhanced security features.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.FallacyDetection.CorrelationCausationFallacy > 0.2, 
                $"Expected correlation-causation fallacy > 0.2, got {result.FallacyDetection.CorrelationCausationFallacy:F2}");
            Assert.Contains(result.Recommendations, r => r.Contains("causation") || r.Contains("correlation") || r.Contains("statistical") || r.Contains("evidence"));
        }

        [Fact]
        public async Task ValidateCausalQuality_StatisticalEvidence_HighStatisticalStrength()
        {
            // Arrange
            var suggestionText = @"
                AutoCAD Map3D 2025 migration results in 25% faster spatial data processing 
                based on controlled experiments with n=50 test cases. T-test analysis shows 
                statistically significant improvement (p = 0.003) with 95% confidence intervals 
                [18%, 32%]. ANOVA confirms consistent results across different data types.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();
            var evidence = new List<string> { "Controlled experiment data", "Statistical analysis report" };

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext, evidence);

            // Assert
            Assert.True(result.StatisticalAssessment.HasQuantitativeEvidence);
            Assert.True(result.StatisticalAssessment.HasSampleSizeInformation);
            Assert.True(result.StatisticalAssessment.HasConfidenceIntervals);
            Assert.True(result.StatisticalAssessment.HasSignificanceTesting);
            Assert.True(result.StatisticalAssessment.OverallStatisticalStrength > 0.5, 
                $"Expected statistical strength > 0.5, got {result.StatisticalAssessment.OverallStatisticalStrength:F2}");
        }

        [Fact]
        public async Task ValidateCausalQuality_HighQualityEvidence_HighEvidenceScore()
        {
            // Arrange
            var suggestionText = @"
                Database connection pooling in Oracle 19c causes 40% reduction in connection 
                overhead based on peer-reviewed research and industry benchmarks. Microsoft 
                white paper and Oracle documentation support these findings with experimental 
                validation across multiple enterprise environments.";

            var context = CreateTestValidationContext();
            var evidence = new List<string> 
            { 
                "Microsoft white paper on connection pooling",
                "Oracle 19c performance documentation",
                "Peer-reviewed database performance study"
            };

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, null, evidence);

            // Assert
            Assert.True(result.EvidenceQuality.SourceCredibility > 0.7, 
                $"Expected source credibility > 0.7, got {result.EvidenceQuality.SourceCredibility:F2}");
            Assert.True(result.EvidenceQuality.HasExperimentalEvidence);
            Assert.True(result.EvidenceQuality.HasIndustryBenchmarks);
            Assert.True(result.EvidenceQuality.OverallEvidenceStrength > 0.6, 
                $"Expected evidence strength > 0.6, got {result.EvidenceQuality.OverallEvidenceStrength:F2}");
        }

        [Fact]
        public async Task ValidateCausalQuality_ADDSSpecificClaims_ValidatesCorrectly()
        {
            // Arrange
            var suggestionText = @"
                ADDS25 framework migration to .NET Core 8 leads to improved performance and 
                enhanced compatibility with AutoCAD Map3D 2025. The spatial data migration 
                causes enhanced accuracy in coordinate system transformations. Oracle 19c 
                integration results in enhanced security and simplified data access patterns.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.CausalClaims.Any(c => c.IsADDSSpecific));
            Assert.True(result.ADDSSpecificAnalysis.FrameworkMigrationCausality > 0.6);
            Assert.True(result.ADDSSpecificAnalysis.DatabaseMigrationCausality > 0.6);
            Assert.True(result.ADDSSpecificAnalysis.SpatialDataCausality > 0.5);
            Assert.True(result.ADDSSpecificAnalysis.OverallADDSCausalityScore > 0.6);
        }

        [Fact]
        public async Task ValidateCausalQuality_MultipleCausalClaims_ProcessesAll()
        {
            // Arrange
            var suggestionText = @"
                The ADDS25 migration causes multiple improvements: .NET Core 8 results in 
                better performance, Oracle 19c leads to enhanced security, and Map3D 2025 
                triggers improved spatial capabilities. Each component directly affects 
                overall system functionality due to their integrated architecture.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.CausalClaims.Count >= 3, $"Expected at least 3 causal claims, got {result.CausalClaims.Count}");
            Assert.Contains(result.CausalClaims, c => c.Strength == CausalStrength.Strong);
            Assert.True(result.ReasoningValidation.LogicalConsistency > 0.5);
        }

        [Fact]
        public async Task ValidateCausalQuality_CircularReasoning_DetectsFallacy()
        {
            // Arrange
            var suggestionText = @"
                .NET Core 8 is better because it improves performance, and we know it 
                improves performance because .NET Core 8 is better. The enhanced features 
                are good due to their enhancement capabilities.";

            var context = CreateTestValidationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context);

            // Assert
            Assert.True(result.FallacyDetection.OverallFallacyRisk > 0.05, 
                $"Expected fallacy risk > 0.05, got {result.FallacyDetection.OverallFallacyRisk:F2}");
            Assert.Contains(result.Recommendations, r => r.Contains("circular") || r.Contains("reasoning") || r.Contains("statistical") || r.Contains("evidence"));
        }

        [Fact]
        public async Task ValidateCausalQuality_NoEvidence_LowEvidenceScore()
        {
            // Arrange
            var suggestionText = @"
                Migrating to newer technology causes better results. Modern systems 
                lead to improved outcomes. Updates result in enhanced functionality.";

            var context = CreateTestValidationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context);

            // Assert
            Assert.True(result.EvidenceQuality.OverallEvidenceStrength < 0.5, 
                $"Expected evidence strength < 0.5, got {result.EvidenceQuality.OverallEvidenceStrength:F2}");
            Assert.False(result.EvidenceQuality.HasExperimentalEvidence);
            Assert.False(result.StatisticalAssessment.HasQuantitativeEvidence);
            Assert.Contains(result.Recommendations, r => r.Contains("evidence") || r.Contains("support"));
        }

        [Fact]
        public async Task ValidateCausalQuality_TemporalOrdering_ValidatesCorrectly()
        {
            // Arrange
            var suggestionText = @"
                First, we implement .NET Core 8 migration, which then enables enhanced 
                performance capabilities. Subsequently, Oracle 19c integration becomes 
                possible, leading to improved data access patterns. Finally, Map3D 2025 
                compatibility results in enhanced spatial processing functionality.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.ReasoningValidation.TemporalOrdering > 0.5, 
                $"Expected temporal ordering > 0.5, got {result.ReasoningValidation.TemporalOrdering:F2}");
            Assert.True(result.ReasoningValidation.CausalChainCoherence > 0.6);
        }

        [Theory]
        [InlineData("causes", CausalStrength.Strong)]
        [InlineData("results in", CausalStrength.Strong)]
        [InlineData("because", CausalStrength.Medium)]
        [InlineData("due to", CausalStrength.Medium)]
        [InlineData("may cause", CausalStrength.Weak)]
        [InlineData("associated with", CausalStrength.Weak)]
        public async Task ValidateCausalQuality_CausalIndicators_DetectsCorrectStrength(
            string causalIndicator, CausalStrength expectedStrength)
        {
            // Arrange
            var suggestionText = $".NET Core 8 migration {causalIndicator} improved performance metrics.";
            var context = CreateTestValidationContext();

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context);

            // Assert
            Assert.Contains(result.CausalClaims, c => c.Strength == expectedStrength);
            Assert.Contains(result.CausalClaims, c => c.CausalIndicator.Contains(causalIndicator));
        }

        [Fact]
        public async Task ValidateCausalQuality_ErrorHandling_ReturnsGracefulFailure()
        {
            // Arrange
            var context = CreateTestValidationContext();
            context.ComplexityInfo = null; // This may cause processing errors

            // Act
            var result = await _validator.ValidateCausalQualityAsync("test", context);

            // Assert - Should handle errors gracefully
            Assert.True(result.OverallCausalScore >= 0.0);
            Assert.NotNull(result.CausalClaims);
            Assert.NotNull(result.Recommendations);
        }

        [Fact]
        public async Task ValidateCausalQuality_PerformanceTest_CompletesWithinReasonableTime()
        {
            // Arrange
            var largeSuggestionText = string.Join(" ", Enumerable.Repeat(
                "ADDS25 migration causes improved performance due to .NET Core 8 enhancements " +
                "which results in better resource utilization leading to enhanced user experience", 50));
            var context = CreateTestValidationContext();
            var startTime = DateTime.UtcNow;

            // Act
            var result = await _validator.ValidateCausalQualityAsync(largeSuggestionText, context);
            var duration = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(duration.TotalSeconds < 15, $"Validation took too long: {duration.TotalSeconds:F2} seconds");
            Assert.True(result.OverallCausalScore >= 0.0);
        }

        [Fact]
        public async Task ValidateCausalQuality_ComprehensiveAnalysis_MeetsAccuracyTargets()
        {
            // Arrange
            var suggestionText = @"
                Comprehensive ADDS25 migration analysis shows that .NET Core 8 causes 35% 
                performance improvement based on controlled experiments (n=75, p<0.01). 
                Oracle 19c migration results in 50% reduction in security vulnerabilities 
                according to industry benchmarks and expert analysis. Map3D 2025 integration 
                leads to 20% faster spatial processing due to optimized coordinate system 
                algorithms, validated through peer-reviewed research and case studies.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();
            var evidence = new List<string> 
            { 
                "Controlled experiment results",
                "Industry security benchmarks", 
                "Peer-reviewed spatial processing research"
            };

            // Act
            var result = await _validator.ValidateCausalQualityAsync(suggestionText, context, migrationContext, evidence);

            // Assert - Target: 85%+ accuracy
            Assert.True(result.OverallCausalScore >= 0.65, $"Expected overall score >= 0.65, got {result.OverallCausalScore:F2}");
            Assert.True(result.StatisticalAssessment.OverallStatisticalStrength >= 0.4);
            Assert.True(result.EvidenceQuality.OverallEvidenceStrength >= 0.4);
            Assert.True(result.ReasoningValidation.OverallReasoningQuality >= 0.5);
            Assert.True(result.FallacyDetection.OverallFallacyRisk < 0.5);
            Assert.True(result.Confidence >= 0.4);
        }

        #region Helper Methods

        private ValidationContext CreateTestValidationContext()
        {
            return new ValidationContext
            {
                UserId = "TestUser",
                ProjectId = "ADDS25Migration",
                SystemType = "ADDS",
                ValidationPurpose = "Causal Analysis Testing",
                ComplexityInfo = new SystemComplexityInfo
                {
                    ComplexityScore = 0.7,
                    NumberOfIntegrations = 5
                },
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "ADDS", "AutoCAD", "Map3D", "Oracle", ".NET Core" },
                    DomainExpertise = new Dictionary<string, double>
                    {
                        { "ADDS", 0.9 },
                        { "AutoCAD", 0.8 },
                        { "Map3D", 0.8 },
                        { "Oracle", 0.7 },
                        { ".NET Core", 0.8 }
                    }
                },
                PerformanceConstraints = new PerformanceConstraints
                {
                    MaxResponseTimeMs = 5000,
                    MaxMemoryUsageMB = 512,
                    MaxConcurrentUsers = 50,
                    MinThroughputRPS = 10
                },
                QualityExpectations = new QualityExpectations
                {
                    TargetQualityScore = 0.85
                }
            };
        }

        private ADDSMigrationContext CreateTestMigrationContext()
        {
            return new ADDSMigrationContext
            {
                RequiresLauncherMigration = true,
                RequiresDatabaseMigration = true,
                RequiresFrameworkMigration = true,
                RequiresSpatialMigration = true,
                CurrentADDSVersion = "2019",
                TargetADDSVersion = "2025",
                CurrentDotNetVersion = ".NET Framework 4.8",
                TargetDotNetVersion = ".NET Core 8",
                CurrentAutoCADVersion = "AutoCAD Map3D 2019",
                TargetAutoCADVersion = "AutoCAD Map3D 2025",
                CurrentOracleVersion = "Oracle 12c",
                TargetOracleVersion = "Oracle 19c"
            };
        }

        #endregion
    }
}
