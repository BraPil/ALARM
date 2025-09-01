using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ALARM.Analyzers.SuggestionValidation.Tests
{
    /// <summary>
    /// Comprehensive tests for Pattern Detection Validator
    /// Target: 85%+ accuracy for ADDS migration pattern detection
    /// </summary>
    public class PatternDetectionValidatorTests
    {
        private readonly PatternDetectionValidator _validator;
        private readonly EnhancedFeatureExtractor _featureExtractor;
        private readonly ILogger<PatternDetectionValidator> _logger;
        private readonly ILogger<EnhancedFeatureExtractor> _featureLogger;

        public PatternDetectionValidatorTests()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<PatternDetectionValidator>();
            _featureLogger = loggerFactory.CreateLogger<EnhancedFeatureExtractor>();
            _featureExtractor = new EnhancedFeatureExtractor(_featureLogger);
            _validator = new PatternDetectionValidator(_logger, _featureExtractor);
        }

        [Fact]
        public async Task ValidatePatternQuality_LauncherMigrationPattern_ReturnsHighQuality()
        {
            // Arrange
            var suggestionText = @"
                Migrate ADDS launcher from U:\ network drive to local deployment for ADDS25.
                Update PowerShell scripts to handle local execution while preserving 
                administrator elevation requirements. Implement fallback mechanisms for 
                network connectivity issues and ensure 100% functionality preservation.
                The new launcher should maintain compatibility with existing Map3D 2025 
                integration and Oracle 19c database connections.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallQualityScore >= 0.5, $"Expected quality score >= 0.5, got {result.OverallQualityScore:F2}");
            Assert.Contains(result.DetectedPatterns, p => p.PatternType == PatternType.LauncherMigration);
            Assert.True(result.MigrationCompliance.OverallComplianceScore > 0.5);
            Assert.True(result.Confidence > 0.5);
        }

        [Fact]
        public async Task ValidatePatternQuality_DatabaseMigrationPattern_ReturnsHighQuality()
        {
            // Arrange
            var suggestionText = @"
                Upgrade Oracle database connectivity from 12c to Oracle 19c for ADDS25.
                Implement Entity Framework Core migration with connection pooling and 
                enhanced security. Ensure database schema compatibility and preserve 
                all existing spatial data integrity. Update connection strings and 
                implement proper error handling for .NET Core 8 compatibility.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallQualityScore >= 0.65, $"Expected quality score >= 0.65, got {result.OverallQualityScore:F2}");
            Assert.Contains(result.DetectedPatterns, p => p.PatternType == PatternType.DatabaseMigration);
            Assert.True(result.MigrationCompliance.Oracle19cCompliance > 0.7);
            Assert.True(result.AccuracyMetrics.PatternDetectionAccuracy > 0.6);
        }

        [Fact]
        public async Task ValidatePatternQuality_FrameworkMigrationPattern_ReturnsHighQuality()
        {
            // Arrange
            var suggestionText = @"
                Migrate ADDS from .NET Framework 4.8 to .NET Core 8 for modernization.
                Update all dependencies to .NET 8 compatible versions, modernize APIs,
                and implement proper dependency injection. Ensure backward compatibility
                with existing AutoCAD Map3D 2025 integrations and maintain 100% 
                functionality preservation throughout the migration process.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallQualityScore >= 0.65, $"Expected quality score >= 0.65, got {result.OverallQualityScore:F2}");
            Assert.Contains(result.DetectedPatterns, p => p.PatternType == PatternType.FrameworkMigration);
            Assert.True(result.MigrationCompliance.DotNetCore8Compliance > 0.7);
            Assert.True(result.AccuracyMetrics.ClassificationConfidence > 0.5);
        }

        [Fact]
        public async Task ValidatePatternQuality_SpatialDataMigrationPattern_ReturnsHighQuality()
        {
            // Arrange
            var suggestionText = @"
                Update spatial data handling for AutoCAD Map3D 2025 compatibility.
                Migrate coordinate system definitions and projection parameters to 
                support enhanced GIS functionality. Ensure spatial data integrity 
                during migration and implement proper coordinate transformation 
                algorithms for seamless Map3D 2025 integration.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallQualityScore >= 0.5, $"Expected quality score >= 0.5, got {result.OverallQualityScore:F2}");
            Assert.Contains(result.DetectedPatterns, p => p.PatternType == PatternType.SpatialDataMigration);
            Assert.True(result.MigrationCompliance.Map3D2025Compliance > 0.6);
        }

        [Fact]
        public async Task ValidatePatternQuality_MultiplePatterns_ReturnsHighComplexityScore()
        {
            // Arrange
            var suggestionText = @"
                Comprehensive ADDS25 migration strategy addressing all core components:
                1. Migrate launcher from U:\ to local deployment with PowerShell updates
                2. Upgrade Oracle database from 12c to 19c with Entity Framework Core
                3. Modernize framework from .NET Framework 4.8 to .NET Core 8
                4. Update AutoCAD Map3D integration for 2025 compatibility
                5. Implement enhanced spatial data handling and coordinate systems
                Ensure 100% functionality preservation throughout migration process.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallQualityScore >= 0.7, $"Expected quality score >= 0.7, got {result.OverallQualityScore:F2}");
            Assert.True(result.DetectedPatterns.Count >= 3, $"Expected at least 3 patterns, got {result.DetectedPatterns.Count}");
            Assert.True(result.MigrationCompliance.OverallComplianceScore > 0.8);
            Assert.Contains(result.PatternQualityBreakdown, kvp => kvp.Key == "MigrationCompleteness" && kvp.Value > 0.7);
        }

        [Fact]
        public async Task ValidatePatternQuality_LowQualitySuggestion_ReturnsLowScore()
        {
            // Arrange
            var suggestionText = "Update ADDS to work better.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallQualityScore < 0.5, $"Expected quality score < 0.5, got {result.OverallQualityScore:F2}");
            Assert.True(result.DetectedPatterns.Count == 0 || result.DetectedPatterns.All(p => p.Confidence < 0.5));
            Assert.Contains(result.Recommendations, r => r.Contains("specific"));
        }

        [Fact]
        public async Task ValidatePatternQuality_NoPatterns_ReturnsRecommendations()
        {
            // Arrange
            var suggestionText = "This is a general software development suggestion without ADDS specifics.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallQualityScore < 0.4, $"Expected quality score < 0.4, got {result.OverallQualityScore:F2}");
            Assert.Empty(result.DetectedPatterns);
            Assert.Contains(result.Recommendations, r => r.Contains("ADDS migration patterns"));
        }

        [Fact]
        public async Task ValidatePatternQuality_FunctionalityPreservation_HighComplianceScore()
        {
            // Arrange
            var suggestionText = @"
                Migrate ADDS25 with strict functionality preservation requirements.
                Maintain backward compatibility with all existing features while 
                modernizing to .NET Core 8, AutoCAD Map3D 2025, and Oracle 19c.
                Implement comprehensive testing to preserve 100% of current 
                functionality and ensure seamless user experience transition.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.MigrationCompliance.FunctionalityPreservation > 0.8, 
                $"Expected functionality preservation > 0.8, got {result.MigrationCompliance.FunctionalityPreservation:F2}");
            Assert.True(result.MigrationCompliance.BackwardCompatibility > 0.6);
        }

        [Fact]
        public async Task ValidatePatternQuality_TechnicalAccuracy_CorrectVersions()
        {
            // Arrange
            var suggestionText = @"
                ADDS25 migration targeting .NET Core 8, AutoCAD Map3D 2025, and Oracle 19c.
                Implement specific version compatibility checks and update dependencies 
                accordingly. Ensure API compatibility and proper integration testing 
                for all target platform versions.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.MigrationCompliance.DotNetCore8Compliance > 0.6);
            Assert.True(result.MigrationCompliance.Map3D2025Compliance > 0.6);
            Assert.True(result.MigrationCompliance.Oracle19cCompliance > 0.6);
            Assert.True(result.PatternQualityBreakdown.GetValueOrDefault("TechnicalAccuracy", 0) > 0.6);
        }

        [Fact]
        public async Task ValidatePatternQuality_RiskMitigation_LowerRiskScore()
        {
            // Arrange
            var suggestionText = @"
                Implement gradual ADDS25 migration with comprehensive testing strategy.
                Create backup procedures and rollback mechanisms for each migration phase.
                Conduct incremental testing at each step to ensure functionality preservation.
                Implement proper error handling and recovery procedures throughout the process.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.PatternQualityBreakdown.GetValueOrDefault("MigrationRiskLevel", 1.0) < 0.8, 
                $"Expected migration risk < 0.8, got {result.PatternQualityBreakdown.GetValueOrDefault("MigrationRiskLevel", 1.0):F2}");
        }

        [Fact]
        public async Task ValidatePatternQuality_AccuracyMetrics_MeetTargetThresholds()
        {
            // Arrange
            var suggestionText = @"
                Comprehensive ADDS migration addressing launcher, database, framework, and spatial components.
                Specific implementation for .NET Core 8, Oracle 19c, and Map3D 2025 integration.
                Detailed migration path with testing and rollback procedures.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.AccuracyMetrics.PatternDetectionAccuracy >= 0.6, 
                $"Pattern detection accuracy should be >= 0.6, got {result.AccuracyMetrics.PatternDetectionAccuracy:F2}");
            Assert.True(result.AccuracyMetrics.ClassificationConfidence >= 0.5, 
                $"Classification confidence should be >= 0.5, got {result.AccuracyMetrics.ClassificationConfidence:F2}");
            Assert.True(result.AccuracyMetrics.FeatureAlignmentScore >= 0.4, 
                $"Feature alignment should be >= 0.4, got {result.AccuracyMetrics.FeatureAlignmentScore:F2}");
        }

        [Theory]
        [InlineData("launcher migration powershell local deployment", PatternType.LauncherMigration)]
        [InlineData("oracle 19c database entity framework migration", PatternType.DatabaseMigration)]
        [InlineData(".net core 8 framework modernization api update", PatternType.FrameworkMigration)]
        [InlineData("map3d 2025 spatial coordinate system projection", PatternType.SpatialDataMigration)]
        public async Task ValidatePatternQuality_SpecificPatternKeywords_DetectsCorrectPattern(
            string suggestionText, PatternType expectedPattern)
        {
            // Arrange
            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidatePatternQualityAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.Contains(result.DetectedPatterns, p => p.PatternType == expectedPattern);
            var detectedPattern = result.DetectedPatterns.First(p => p.PatternType == expectedPattern);
            Assert.True(detectedPattern.Confidence > 0.3, 
                $"Expected confidence > 0.3 for {expectedPattern}, got {detectedPattern.Confidence:F2}");
        }

        [Fact]
        public async Task ValidatePatternQuality_ErrorHandling_ReturnsGracefulFailure()
        {
            // Arrange - Test error handling within the validator method
            var context = CreateTestValidationContext();
            context.ComplexityInfo = null; // This will cause an error in processing

            // Act
            var result = await _validator.ValidatePatternQualityAsync("test", context);

            // Assert - Should handle the error gracefully
            Assert.True(result.OverallQualityScore >= 0.0); // Should not crash
            Assert.NotNull(result.DetectedPatterns);
            Assert.NotNull(result.Recommendations);
        }

        [Fact]
        public async Task ValidatePatternQuality_PerformanceTest_CompletesWithinReasonableTime()
        {
            // Arrange
            var largeSuggestionText = string.Join(" ", Enumerable.Repeat(
                "ADDS25 migration .NET Core 8 Oracle 19c Map3D 2025 launcher database framework spatial", 100));
            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            var startTime = DateTime.UtcNow;

            // Act
            var result = await _validator.ValidatePatternQualityAsync(largeSuggestionText, context, migrationContext);

            var duration = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(duration.TotalSeconds < 10, $"Validation took too long: {duration.TotalSeconds:F2} seconds");
            Assert.True(result.OverallQualityScore > 0.5); // Should detect patterns in repeated text
        }

        #region Helper Methods

        private ValidationContext CreateTestValidationContext()
        {
            return new ValidationContext
            {
                UserId = "TestUser",
                ProjectId = "ADDS25Migration",
                SystemType = "ADDS",
                ValidationPurpose = "Pattern Detection Testing",
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
