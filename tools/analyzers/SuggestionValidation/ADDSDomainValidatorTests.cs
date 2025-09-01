using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ALARM.Analyzers.SuggestionValidation.Tests
{
    /// <summary>
    /// Comprehensive tests for ADDS Domain Validator
    /// Target: 85%+ accuracy for CAD integration and database optimization scoring
    /// </summary>
    public class ADDSDomainValidatorTests
    {
        private readonly ADDSDomainValidator _validator;
        private readonly EnhancedFeatureExtractor _featureExtractor;
        private readonly ILogger<ADDSDomainValidator> _logger;
        private readonly ILogger<EnhancedFeatureExtractor> _featureLogger;

        public ADDSDomainValidatorTests()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<ADDSDomainValidator>();
            _featureLogger = loggerFactory.CreateLogger<EnhancedFeatureExtractor>();
            _featureExtractor = new EnhancedFeatureExtractor(_featureLogger);
            _validator = new ADDSDomainValidator(_logger, _featureExtractor);
        }

        [Fact]
        public async Task ValidateADDSDomain_ComprehensiveCADIntegration_ReturnsHighScore()
        {
            // Arrange
            var suggestionText = @"
                Implement comprehensive AutoCAD Map3D 2025 API integration for ADDS25 migration.
                Update spatial data handling with Oracle 19c spatial optimization and coordinate system transformation.
                Migrate drawing database entities using .NET Core 8 ObjectARX API compatibility layer.
                Optimize CAD rendering performance with GPU acceleration and viewport management.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();
            var expertiseContext = CreateTestExpertiseContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext, expertiseContext);

            // Assert
            Assert.True(result.OverallDomainScore >= 0.7, $"Expected domain score >= 0.7, got {result.OverallDomainScore:F2}");
            Assert.True(result.CADIntegrationAnalysis.HasMap3DReferences);
            Assert.True(result.CADIntegrationAnalysis.HasAPIIntegration);
            Assert.True(result.CADIntegrationAnalysis.HasSpatialDataHandling);
            Assert.True(result.CADIntegrationAnalysis.OverallCADScore >= 0.6);
            Assert.True(result.Confidence >= 0.7);
        }

        [Fact]
        public async Task ValidateADDSDomain_OracleSpacialOptimization_IdentifiesDatabaseExpertise()
        {
            // Arrange
            var suggestionText = @"
                Optimize Oracle 19c spatial data operations with SDO_GEOMETRY indexing for improved query performance.
                Implement Oracle Spatial Data Option (SDO) for coordinate system transformations and spatial queries.
                Configure Oracle connection pooling and query optimization for ADDS spatial data processing.
                Migrate from Oracle 12c to Oracle 19c with spatial index optimization and performance tuning.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.DatabaseOptimizationAnalysis.HasOracleReferences);
            Assert.True(result.DatabaseOptimizationAnalysis.HasSpatialDataOptimization);
            Assert.True(result.DatabaseOptimizationAnalysis.HasQueryOptimization);
            Assert.True(result.DatabaseOptimizationAnalysis.OverallDatabaseScore >= 0.6, 
                $"Expected database score >= 0.6, got {result.DatabaseOptimizationAnalysis.OverallDatabaseScore:F2}");
            Assert.True(result.DomainExpertiseAssessment.DatabaseExpertiseLevel >= 0.6);
        }

        [Fact]
        public async Task ValidateADDSDomain_DotNetCore8Migration_RecognizesFrameworkExpertise()
        {
            // Arrange
            var suggestionText = @"
                Migrate ADDS from .NET Framework 4.8 to .NET Core 8 for improved performance and cross-platform compatibility.
                Update AutoCAD API integration to work with .NET Core 8 runtime and modern C# features.
                Implement async/await patterns and utilize .NET Core 8 performance improvements for spatial data processing.
                Configure deployment strategy for .NET Core 8 with improved startup performance and memory management.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.FrameworkMigrationAnalysis.HasDotNetCoreReferences);
            Assert.True(result.FrameworkMigrationAnalysis.HasFrameworkMigrationStrategy);
            Assert.True(result.FrameworkMigrationAnalysis.OverallFrameworkScore >= 0.5, 
                $"Expected framework score >= 0.5, got {result.FrameworkMigrationAnalysis.OverallFrameworkScore:F2}");
            Assert.True(result.DomainExpertiseAssessment.FrameworkExpertiseLevel >= 0.5);
        }

        [Fact]
        public async Task ValidateADDSDomain_CADAPIIntegration_DetectsSpecializedKnowledge()
        {
            // Arrange
            var suggestionText = @"
                Implement ObjectARX API integration for AutoCAD Map3D 2025 with entity manipulation and block reference management.
                Update drawing database access patterns for spatial data entities and coordinate system handling.
                Configure layer management and viewport control for improved CAD rendering performance.
                Integrate with AutoCAD Map3D spatial data providers for geographic coordinate system transformations.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.CADIntegrationAnalysis.HasAPIIntegration);
            Assert.True(result.CADIntegrationAnalysis.HasCoordinateSystemHandling);
            Assert.True(result.CADIntegrationAnalysis.APICompatibilityScore >= 0.6, 
                $"Expected API compatibility >= 0.6, got {result.CADIntegrationAnalysis.APICompatibilityScore:F2}");
            Assert.True(result.DomainExpertiseAssessment.CADExpertiseLevel >= 0.6);
        }

        [Fact]
        public async Task ValidateADDSDomain_SpatialDataProcessing_IntegratesCADAndDatabase()
        {
            // Arrange
            var suggestionText = @"
                Implement integrated spatial data processing pipeline connecting AutoCAD Map3D 2025 with Oracle 19c Spatial.
                Configure coordinate system transformations between CAD drawing coordinates and database spatial geometry.
                Optimize spatial indexing for both AutoCAD spatial queries and Oracle SDO_GEOMETRY operations.
                Implement spatial data synchronization between drawing entities and database spatial tables.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.CADIntegrationAnalysis.HasSpatialDataHandling);
            Assert.True(result.DatabaseOptimizationAnalysis.HasSpatialDataOptimization);
            Assert.True(result.CADIntegrationAnalysis.SpatialDataMigrationScore >= 0.6);
            Assert.True(result.DatabaseOptimizationAnalysis.SpatialDataOptimizationScore >= 0.6);
            Assert.True(result.OverallDomainScore >= 0.64, $"Expected overall score >= 0.64, got {result.OverallDomainScore:F2}");
        }

        [Fact]
        public async Task ValidateADDSDomain_ADDSSpecificTerminology_RecognizesDomainExpertise()
        {
            // Arrange
            var suggestionText = @"
                Optimize ADDS spatial data workflow with Map3D coordinate system integration and Oracle spatial indexing.
                Implement ADDS-specific entity handling for transmission line spatial data and geographic coordinate transformations.
                Configure ADDS launcher optimization for improved startup performance with local deployment strategy.
                Integrate ADDS spatial queries with Oracle 19c spatial operators for enhanced performance.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.DomainExpertiseAssessment.ADDSSpecificKnowledge >= 0.5, 
                $"Expected ADDS knowledge >= 0.5, got {result.DomainExpertiseAssessment.ADDSSpecificKnowledge:F2}");
            Assert.True(result.DomainExpertiseAssessment.OverallExpertiseScore >= 0.5);
            Assert.True(result.Recommendations.Any(), "Expected at least one recommendation");
        }

        [Fact]
        public async Task ValidateADDSDomain_MigrationComplexityAssessment_EvaluatesRiskFactors()
        {
            // Arrange
            var suggestionText = @"
                Complex migration from ADDS 2019 to ADDS25 involving AutoCAD Map3D 2019 to 2025 API breaking changes.
                Oracle 12c to 19c migration with spatial data schema updates and query optimization requirements.
                .NET Framework 4.8 to .NET Core 8 migration with potential compatibility issues and performance testing needs.
                Integration testing required for CAD-database spatial data synchronization and coordinate system accuracy.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.NotNull(result.MigrationComplexityAssessment);
            Assert.True(result.DomainExpertiseAssessment.MigrationExpertise >= 0.5);
            Assert.True(result.FrameworkMigrationAnalysis.HasCompatibilityConsiderations);
            Assert.True(result.Recommendations.Any(), "Expected at least one recommendation");
        }

        [Fact]
        public async Task ValidateADDSDomain_PerformanceOptimization_IntegratesAllDomains()
        {
            // Arrange
            var suggestionText = @"
                Comprehensive ADDS performance optimization strategy integrating AutoCAD Map3D 2025, Oracle 19c, and .NET Core 8.
                Optimize CAD rendering with GPU acceleration and spatial data caching for improved viewport performance.
                Implement Oracle spatial indexing with query optimization and connection pooling for database performance.
                Utilize .NET Core 8 async patterns and memory management for enhanced application performance.
                Configure integrated performance monitoring across CAD, database, and framework components.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.CADIntegrationAnalysis.OverallCADScore >= 0.6);
            Assert.True(result.DatabaseOptimizationAnalysis.OverallDatabaseScore >= 0.6);
            Assert.True(result.FrameworkMigrationAnalysis.OverallFrameworkScore >= 0.6);
            Assert.True(result.OverallDomainScore >= 0.7, $"Expected overall score >= 0.7, got {result.OverallDomainScore:F2}");
            Assert.True(result.Confidence >= 0.75);
        }

        [Fact]
        public async Task ValidateADDSDomain_NonDomainSpecific_ReturnsLowerScore()
        {
            // Arrange
            var suggestionText = @"
                Update user interface colors and fonts for better visual appearance and accessibility.
                Modify application menus and toolbar layouts for improved user experience.
                Update help documentation and user training materials for clarity.
                Implement generic logging and error handling improvements.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallDomainScore >= 0.3 && result.OverallDomainScore <= 0.6, 
                $"Expected neutral domain score (0.3-0.6), got {result.OverallDomainScore:F2}");
            Assert.False(result.CADIntegrationAnalysis.HasMap3DReferences);
            Assert.False(result.DatabaseOptimizationAnalysis.HasOracleReferences);
            Assert.False(result.FrameworkMigrationAnalysis.HasDotNetCoreReferences);
        }

        [Fact]
        public async Task ValidateADDSDomain_ErrorHandling_ReturnsGracefulFailure()
        {
            // Arrange
            var context = CreateTestValidationContext();
            context.ComplexityInfo = null; // This may cause processing errors
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync("test", context, migrationContext);

            // Assert - Should handle errors gracefully
            Assert.True(result.OverallDomainScore >= 0.0);
            Assert.NotNull(result.CADIntegrationAnalysis);
            Assert.NotNull(result.DatabaseOptimizationAnalysis);
            Assert.NotNull(result.FrameworkMigrationAnalysis);
            Assert.NotNull(result.Recommendations);
            Assert.True(result.Recommendations.Any(), "Expected at least one recommendation even in error case");
        }

        [Fact]
        public async Task ValidateADDSDomain_PerformanceTest_CompletesWithinReasonableTime()
        {
            // Arrange
            var largeSuggestionText = string.Join(" ", Enumerable.Repeat(
                "Optimize ADDS25 with AutoCAD Map3D 2025 API integration, Oracle 19c spatial optimization, " +
                ".NET Core 8 performance improvements, spatial data processing, coordinate system transformations, " +
                "database query optimization, CAD rendering performance, and migration strategy implementation", 50));
            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();
            var startTime = DateTime.UtcNow;

            // Act
            var result = await _validator.ValidateADDSDomainAsync(largeSuggestionText, context, migrationContext);
            var duration = DateTime.UtcNow - startTime;

            // Assert
            Assert.True(duration.TotalSeconds < 10, $"Validation took too long: {duration.TotalSeconds:F2} seconds");
            Assert.True(result.OverallDomainScore >= 0.0);
        }

        [Theory]
        [InlineData("AutoCAD Map3D API integration", 0.56)]
        [InlineData("Oracle 19c spatial optimization", 0.56)]
        [InlineData(".NET Core 8 migration", 0.52)]
        [InlineData("spatial data processing", 0.48)]
        [InlineData("coordinate system transformation", 0.48)]
        public async Task ValidateADDSDomain_SpecificDomainAreas_ReturnsExpectedScore(
            string domainArea, double expectedMinScore)
        {
            // Arrange
            var suggestionText = $"Implement {domainArea} for ADDS25 migration with comprehensive optimization and best practices.";
            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext);

            // Assert
            Assert.True(result.OverallDomainScore >= expectedMinScore * 0.8, // Allow 20% tolerance
                $"Expected score >= {expectedMinScore * 0.8:F2} for {domainArea}, got {result.OverallDomainScore:F2}");
        }

        [Fact]
        public async Task ValidateADDSDomain_ComprehensiveADDSMigration_MeetsAccuracyTargets()
        {
            // Arrange
            var suggestionText = @"
                Comprehensive ADDS 2019 to ADDS25 migration strategy with 100% functionality preservation:
                
                CAD Integration: Migrate AutoCAD Map3D 2019 to 2025 with ObjectARX API updates, spatial data entity 
                handling, coordinate system transformation optimization, and drawing database compatibility.
                
                Database Optimization: Upgrade Oracle 12c to 19c with spatial data optimization, SDO_GEOMETRY 
                indexing, query performance tuning, and connection pooling for improved spatial query performance.
                
                Framework Migration: Migrate from .NET Framework 4.8 to .NET Core 8 with API compatibility 
                assessment, performance optimization, async/await pattern implementation, and deployment strategy.
                
                ADDS-Specific Integration: Implement transmission line spatial data processing, coordinate system 
                accuracy validation, launcher performance optimization, and integrated testing strategy.
                
                Performance validation shows 40% improvement in spatial query performance, 60% faster CAD rendering, 
                and 25% overall application performance improvement with 99.9% uptime target.";

            var context = CreateTestValidationContext();
            var migrationContext = CreateTestMigrationContext();
            var expertiseContext = CreateTestExpertiseContext();

            // Act
            var result = await _validator.ValidateADDSDomainAsync(suggestionText, context, migrationContext, expertiseContext);

            // Assert - Target: 85%+ accuracy
            Assert.True(result.OverallDomainScore >= 0.75, $"Expected overall score >= 0.75, got {result.OverallDomainScore:F2}");
            Assert.True(result.CADIntegrationAnalysis.OverallCADScore >= 0.7);
            Assert.True(result.DatabaseOptimizationAnalysis.OverallDatabaseScore >= 0.7);
            Assert.True(result.FrameworkMigrationAnalysis.OverallFrameworkScore >= 0.7);
            Assert.True(result.DomainExpertiseAssessment.OverallExpertiseScore >= 0.7);
            Assert.True(result.Confidence >= 0.8);
            Assert.True(result.Recommendations.Count >= 3);
        }

        #region Helper Methods

        private ValidationContext CreateTestValidationContext()
        {
            return new ValidationContext
            {
                UserId = "TestUser",
                ProjectId = "ADDS25Migration",
                SystemType = "ADDS",
                ValidationPurpose = "Domain Validation Testing",
                ComplexityInfo = new SystemComplexityInfo
                {
                    ComplexityScore = 0.8,
                    NumberOfIntegrations = 7
                },
                DomainContext = new DomainSpecificContext
                {
                    PrimaryDomains = new List<string> { "ADDS", "AutoCAD", "Map3D", "Oracle", ".NET Core" },
                    DomainExpertise = new Dictionary<string, double>
                    {
                        { "ADDS", 0.9 },
                        { "AutoCAD", 0.85 },
                        { "Map3D", 0.8 },
                        { "Oracle", 0.8 },
                        { ".NET Core", 0.8 }
                    }
                },
                PerformanceConstraints = new PerformanceConstraints
                {
                    MaxResponseTimeMs = 3000,
                    MaxMemoryUsageMB = 2048,
                    MaxConcurrentUsers = 100,
                    MinThroughputRPS = 75
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

        private DomainExpertiseContext CreateTestExpertiseContext()
        {
            return new DomainExpertiseContext
            {
                ExpertiseLevels = new Dictionary<string, double>
                {
                    { "CAD", 0.85 },
                    { "Database", 0.8 },
                    { "Framework", 0.8 },
                    { "ADDS", 0.9 },
                    { "Migration", 0.75 }
                },
                PrimaryDomains = new List<string> { "ADDS", "AutoCAD", "Oracle", ".NET" },
                SecondaryDomains = new List<string> { "Spatial", "Performance", "Migration" }
            };
        }

        #endregion
    }
}
