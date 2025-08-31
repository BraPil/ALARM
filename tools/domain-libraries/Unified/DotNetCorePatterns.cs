using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ALARM.Analyzers.PatternDetection;
namespace ALARM.DomainLibraries.DotNetCore
{
    /// <summary>
    /// .NET Core-specific pattern detection and analysis
    /// Specializes in .NET Framework to .NET Core/8 migration patterns and modernization
    /// </summary>
    public class DotNetCorePatterns : IDomainLibrary
    {
        private readonly ILogger<DotNetCorePatterns> _logger;
        private readonly Dictionary<string, DotNetCorePatternRule> _patternRules;
        private readonly Dictionary<string, DotNetCoreValidationRule> _validationRules;

        public string DomainName => "DotNetCore";
        public string Version => "8.0.1.0";

        public IReadOnlyList<string> SupportedPatternCategories => new[]
        {
            "Framework_Migration",
            "Configuration",
            "Dependency_Injection",
            "Hosting",
            "Logging",
            "Web_API",
            "Entity_Framework",
            "Security",
            "Performance",
            "Compatibility"
        };

        public DotNetCorePatterns(ILogger<DotNetCorePatterns> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _patternRules = InitializePatternRules();
            _validationRules = InitializeValidationRules();
        }

        public async Task<DomainPatternResult> DetectPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            _logger.LogInformation("Starting .NET Core pattern detection on {DataCount} samples", data.Count());

            var result = new DomainPatternResult
            {
                DomainName = DomainName,
                DetectionTimestamp = DateTime.UtcNow
            };

            try
            {
                // Detect Framework migration patterns
                var migrationPatterns = await DetectFrameworkMigrationPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(migrationPatterns);

                // Detect configuration patterns
                var configPatterns = await DetectConfigurationPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(configPatterns);

                // Detect dependency injection patterns
                var diPatterns = await DetectDependencyInjectionPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(diPatterns);

                // Detect hosting patterns
                var hostingPatterns = await DetectHostingPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(hostingPatterns);

                // Detect logging patterns
                var loggingPatterns = await DetectLoggingPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(loggingPatterns);

                // Detect Web API patterns
                var webApiPatterns = await DetectWebAPIPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(webApiPatterns);

                // Detect Entity Framework patterns
                var efPatterns = await DetectEntityFrameworkPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(efPatterns);

                // Detect security patterns
                var securityPatterns = await DetectSecurityPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(securityPatterns);

                // Calculate confidence scores
                CalculatePatternConfidenceScores(result);

                // Detect anomalies
                result.DetectedAnomalies = await DetectDotNetCoreAnomaliesAsync(data, result.DetectedPatterns);

                _logger.LogInformation(".NET Core pattern detection completed: {PatternCount} patterns, {AnomalyCount} anomalies",
                    result.DetectedPatterns.Count, result.DetectedAnomalies.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during .NET Core pattern detection");
                throw;
            }
        }

        public async Task<DomainValidationResult> ValidateAsync(
            string content,
            string contentType,
            DomainValidationConfig config)
        {
            _logger.LogInformation("Starting .NET Core validation for content type: {ContentType}", contentType);

            var result = new DomainValidationResult
            {
                IsValid = true
            };

            try
            {
                // Validate based on content type
                switch (contentType.ToLowerInvariant())
                {
                    case "csproj":
                    case "project":
                        await ValidateProjectFileAsync(content, result, config);
                        break;
                    case "startup":
                    case "program":
                        await ValidateStartupConfigurationAsync(content, result, config);
                        break;
                    case "appsettings":
                    case "configuration":
                        await ValidateConfigurationAsync(content, result, config);
                        break;
                    case "controller":
                        await ValidateControllerAsync(content, result, config);
                        break;
                    case "service":
                        await ValidateServiceAsync(content, result, config);
                        break;
                    default:
                        await ValidateGeneralDotNetCorePatternsAsync(content, result, config);
                        break;
                }

                result.IsValid = result.Issues.Count == 0 || result.Issues.All(i => i.Severity != IssueSeverity.Critical);

                _logger.LogInformation(".NET Core validation completed: {IsValid}, {IssueCount} issues, {WarningCount} warnings",
                    result.IsValid, result.Issues.Count, result.Warnings.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during .NET Core validation");
                result.IsValid = false;
                result.Issues.Add(new DomainValidationIssue
                {
                    IssueId = "VALIDATION_ERROR",
                    RuleName = "System Error",
                    Description = $"Validation failed with error: {ex.Message}",
                    Severity = IssueSeverity.Critical
                });
                return result;
            }
        }

        public async Task<DomainFeatureResult> ExtractFeaturesAsync(
            IEnumerable<PatternData> data,
            DomainFeatureConfig config)
        {
            _logger.LogInformation("Starting .NET Core feature extraction");

            var result = new DomainFeatureResult();

            try
            {
                foreach (var dataPoint in data)
                {
                    // Extract migration readiness features
                    var migrationFeatures = ExtractMigrationReadinessFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(migrationFeatures);

                    // Extract modernization features
                    var modernizationFeatures = ExtractModernizationFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(modernizationFeatures);

                    // Extract performance features
                    var performanceFeatures = ExtractPerformanceFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(performanceFeatures);

                    // Extract compatibility features
                    var compatibilityFeatures = ExtractCompatibilityFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(compatibilityFeatures);
                }

                // Calculate feature importance scores
                CalculateFeatureImportanceScores(result);

                _logger.LogInformation(".NET Core feature extraction completed: {FeatureCount} features extracted",
                    result.ExtractedFeatures.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during .NET Core feature extraction");
                throw;
            }
        }

        public async Task<DomainOptimizationResult> GetOptimizationSuggestionsAsync(
            DomainPatternResult patternResult,
            DomainOptimizationConfig config)
        {
            _logger.LogInformation("Starting .NET Core optimization analysis");

            var result = new DomainOptimizationResult();

            try
            {
                foreach (var pattern in patternResult.DetectedPatterns)
                {
                    var suggestions = GenerateOptimizationSuggestions(pattern, config);
                    result.Suggestions.AddRange(suggestions);
                }

                // Calculate expected improvements
                CalculateExpectedImprovements(result);

                _logger.LogInformation(".NET Core optimization analysis completed: {SuggestionCount} suggestions",
                    result.Suggestions.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during .NET Core optimization analysis");
                throw;
            }
        }

        public async Task<DomainMigrationResult> GetMigrationRecommendationsAsync(
            string legacyPattern,
            string targetVersion,
            DomainMigrationConfig config)
        {
            _logger.LogInformation("Starting .NET Core migration analysis for pattern: {Pattern} to version: {Version}",
                legacyPattern, targetVersion);

            var result = new DomainMigrationResult();

            try
            {
                // Analyze legacy pattern and generate migration recommendations
                var recommendations = GenerateMigrationRecommendations(legacyPattern, targetVersion, config);
                result.Recommendations.AddRange(recommendations);

                // Calculate complexity scores
                CalculateMigrationComplexityScores(result);

                _logger.LogInformation(".NET Core migration analysis completed: {RecommendationCount} recommendations",
                    result.Recommendations.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during .NET Core migration analysis");
                throw;
            }
        }

        #region Private Pattern Detection Methods

        private async Task<List<DomainPattern>> DetectFrameworkMigrationPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect .NET Framework references
                    if (Regex.IsMatch(codeContent, @"System\.Web\.|System\.Configuration\.ConfigurationManager", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "LEGACY_FRAMEWORK_REFERENCES",
                            PatternName = "Legacy .NET Framework References",
                            Category = "Framework_Migration",
                            Description = "Legacy .NET Framework namespaces detected",
                            ConfidenceScore = 0.95,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Migrate to .NET Core equivalents"
                        });
                    }

                    // Detect Global.asax patterns
                    if (Regex.IsMatch(codeContent, @"Application_Start|Application_End|Global\.asax", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "GLOBAL_ASAX_PATTERN",
                            PatternName = "Global.asax Application Events",
                            Category = "Framework_Migration",
                            Description = "Global.asax application lifecycle events detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Migrate to .NET Core startup and hosted services"
                        });
                    }

                    // Detect Web.config patterns
                    if (Regex.IsMatch(codeContent, @"<system\.web>|<appSettings>|web\.config", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "WEB_CONFIG_PATTERN",
                            PatternName = "Web.config Configuration",
                            Category = "Configuration",
                            Description = "Web.config configuration detected",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Migrate to appsettings.json and .NET Core configuration"
                        });
                    }

                    // Detect HttpContext.Current usage
                    if (Regex.IsMatch(codeContent, @"HttpContext\.Current", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "HTTPCONTEXT_CURRENT",
                            PatternName = "HttpContext.Current Usage",
                            Category = "Framework_Migration",
                            Description = "HttpContext.Current usage detected",
                            ConfidenceScore = 0.95,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Use IHttpContextAccessor or controller HttpContext property"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectConfigurationPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect ConfigurationManager usage
                    if (Regex.IsMatch(codeContent, @"ConfigurationManager\.AppSettings", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "CONFIGURATION_MANAGER_USAGE",
                            PatternName = "ConfigurationManager Usage",
                            Category = "Configuration",
                            Description = "Legacy ConfigurationManager usage detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Migrate to IConfiguration and Options pattern"
                        });
                    }

                    // Detect modern configuration patterns
                    if (Regex.IsMatch(codeContent, @"IConfiguration|IOptions<|appsettings\.json", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "MODERN_CONFIGURATION_PATTERN",
                            PatternName = "Modern Configuration Pattern",
                            Category = "Configuration",
                            Description = "Modern .NET Core configuration pattern detected",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - continue using modern configuration patterns"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectDependencyInjectionPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect manual object instantiation
                    if (Regex.IsMatch(codeContent, @"new\s+\w+Repository\(|new\s+\w+Service\(", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "MANUAL_OBJECT_CREATION",
                            PatternName = "Manual Object Creation",
                            Category = "Dependency_Injection",
                            Description = "Manual service/repository instantiation detected",
                            ConfidenceScore = 0.75,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Use dependency injection container"
                        });
                    }

                    // Detect DI container usage
                    if (Regex.IsMatch(codeContent, @"services\.AddScoped|services\.AddTransient|services\.AddSingleton", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "DI_CONTAINER_USAGE",
                            PatternName = "Dependency Injection Container",
                            Category = "Dependency_Injection",
                            Description = "Proper DI container usage detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - continue using DI container"
                        });
                    }

                    // Detect service locator anti-pattern
                    if (Regex.IsMatch(codeContent, @"ServiceLocator\.|GetService<|GetRequiredService<", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "SERVICE_LOCATOR_ANTIPATTERN",
                            PatternName = "Service Locator Anti-pattern",
                            Category = "Dependency_Injection",
                            Description = "Service locator pattern usage detected",
                            ConfidenceScore = 0.80,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Use constructor injection instead of service locator"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectHostingPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect modern hosting patterns
                    if (Regex.IsMatch(codeContent, @"WebApplication\.CreateBuilder|Host\.CreateDefaultBuilder", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "MODERN_HOSTING_PATTERN",
                            PatternName = "Modern Hosting Pattern",
                            Category = "Hosting",
                            Description = "Modern .NET Core hosting pattern detected",
                            ConfidenceScore = 0.95,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - using modern hosting patterns"
                        });
                    }

                    // Detect legacy hosting patterns
                    if (Regex.IsMatch(codeContent, @"WebHostBuilder|CreateWebHostBuilder", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "LEGACY_HOSTING_PATTERN",
                            PatternName = "Legacy Hosting Pattern",
                            Category = "Hosting",
                            Description = "Legacy WebHostBuilder pattern detected",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Consider migrating to WebApplication.CreateBuilder"
                        });
                    }

                    // Detect background services
                    if (Regex.IsMatch(codeContent, @"BackgroundService|IHostedService", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "BACKGROUND_SERVICE_PATTERN",
                            PatternName = "Background Service Pattern",
                            Category = "Hosting",
                            Description = "Background service implementation detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - using background services for long-running tasks"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectLoggingPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect legacy logging patterns
                    if (Regex.IsMatch(codeContent, @"log4net|NLog|Console\.WriteLine", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "LEGACY_LOGGING_PATTERN",
                            PatternName = "Legacy Logging Pattern",
                            Category = "Logging",
                            Description = "Legacy logging framework or Console.WriteLine usage detected",
                            ConfidenceScore = 0.80,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Migrate to Microsoft.Extensions.Logging"
                        });
                    }

                    // Detect modern logging patterns
                    if (Regex.IsMatch(codeContent, @"ILogger<|_logger\.Log|Microsoft\.Extensions\.Logging", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "MODERN_LOGGING_PATTERN",
                            PatternName = "Modern Logging Pattern",
                            Category = "Logging",
                            Description = "Modern .NET Core logging pattern detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - using modern logging abstractions"
                        });
                    }

                    // Detect structured logging
                    if (Regex.IsMatch(codeContent, @"LogInformation\(.*\{.*\}|LogError\(.*\{.*\}", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "STRUCTURED_LOGGING_PATTERN",
                            PatternName = "Structured Logging Pattern",
                            Category = "Logging",
                            Description = "Structured logging pattern detected",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Excellent - using structured logging"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectWebAPIPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect Web API controller patterns
                    if (Regex.IsMatch(codeContent, @"\[ApiController\]|\[Route\(", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "WEB_API_CONTROLLER_PATTERN",
                            PatternName = "Web API Controller Pattern",
                            Category = "Web_API",
                            Description = "Web API controller pattern detected",
                            ConfidenceScore = 0.95,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - using modern Web API patterns"
                        });
                    }

                    // Detect minimal APIs
                    if (Regex.IsMatch(codeContent, @"app\.MapGet\(|app\.MapPost\(|app\.MapPut\(|app\.MapDelete\(", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "MINIMAL_API_PATTERN",
                            PatternName = "Minimal API Pattern",
                            Category = "Web_API",
                            Description = "Minimal API pattern detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - using modern minimal API patterns"
                        });
                    }

                    // Detect legacy ActionResult patterns
                    if (Regex.IsMatch(codeContent, @"return\s+Ok\(\)|return\s+BadRequest\(\)", RegexOptions.IgnoreCase) &&
                        !Regex.IsMatch(codeContent, @"ActionResult<", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "LEGACY_ACTION_RESULT",
                            PatternName = "Legacy ActionResult Pattern",
                            Category = "Web_API",
                            Description = "Legacy ActionResult usage without generic typing",
                            ConfidenceScore = 0.70,
                            Severity = PatternSeverity.Low,
                            RecommendedAction = "Consider using ActionResult<T> for better type safety"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectEntityFrameworkPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect EF6 patterns
                    if (Regex.IsMatch(codeContent, @"EntityFramework\.|System\.Data\.Entity", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "EF6_PATTERN",
                            PatternName = "Entity Framework 6 Pattern",
                            Category = "Entity_Framework",
                            Description = "Entity Framework 6 usage detected",
                            ConfidenceScore = 0.95,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Migrate to Entity Framework Core"
                        });
                    }

                    // Detect EF Core patterns
                    if (Regex.IsMatch(codeContent, @"Microsoft\.EntityFrameworkCore|DbContext", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "EF_CORE_PATTERN",
                            PatternName = "Entity Framework Core Pattern",
                            Category = "Entity_Framework",
                            Description = "Entity Framework Core usage detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - using Entity Framework Core"
                        });
                    }

                    // Detect DbContext disposal patterns
                    if (Regex.IsMatch(codeContent, @"new\s+\w*DbContext\(", RegexOptions.IgnoreCase) &&
                        !Regex.IsMatch(codeContent, @"using\s*\(.*DbContext", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "DBCONTEXT_NOT_DISPOSED",
                            PatternName = "DbContext Not Properly Disposed",
                            Category = "Entity_Framework",
                            Description = "DbContext may not be properly disposed",
                            ConfidenceScore = 0.75,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Use using statement or dependency injection"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectSecurityPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect authentication patterns
                    if (Regex.IsMatch(codeContent, @"AddAuthentication\(|AddJwtBearer\(", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "MODERN_AUTHENTICATION_PATTERN",
                            PatternName = "Modern Authentication Pattern",
                            Category = "Security",
                            Description = "Modern authentication configuration detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - using modern authentication patterns"
                        });
                    }

                    // Detect authorization patterns
                    if (Regex.IsMatch(codeContent, @"\[Authorize\]|AddAuthorization\(", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "AUTHORIZATION_PATTERN",
                            PatternName = "Authorization Pattern",
                            Category = "Security",
                            Description = "Authorization implementation detected",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.Info,
                            RecommendedAction = "Good - implementing proper authorization"
                        });
                    }

                    // Detect hardcoded secrets
                    if (Regex.IsMatch(codeContent, @"password\s*=\s*['""][^'""]+['""]|connectionstring.*password", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "HARDCODED_SECRETS",
                            PatternName = "Hardcoded Secrets",
                            Category = "Security",
                            Description = "Hardcoded secrets detected in code",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.Critical,
                            RecommendedAction = "Move secrets to secure configuration or Azure Key Vault"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainAnomaly>> DetectDotNetCoreAnomaliesAsync(
            IEnumerable<PatternData> data,
            List<DomainPattern> detectedPatterns)
        {
            var anomalies = new List<DomainAnomaly>();

            // Check for mixed framework patterns
            var legacyPatterns = detectedPatterns.Where(p => p.Severity == PatternSeverity.High && 
                p.Category == "Framework_Migration").Count();
            var modernPatterns = detectedPatterns.Where(p => p.Severity == PatternSeverity.Info).Count();

            if (legacyPatterns > 0 && modernPatterns > 0)
            {
                anomalies.Add(new DomainAnomaly
                {
                    AnomalyId = "MIXED_FRAMEWORK_PATTERNS",
                    AnomalyType = "Framework Inconsistency",
                    Description = "Mixed legacy and modern framework patterns detected",
                    AnomalyScore = 0.75,
                    Severity = AnomalySeverity.Medium,
                    RecommendedInvestigation = "Review migration progress and ensure consistent patterns"
                });
            }

            return anomalies;
        }

        #endregion

        #region Private Validation Methods

        private async Task ValidateProjectFileAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for .NET Core target framework
            if (!Regex.IsMatch(content, @"<TargetFramework>net[6-8]\.0</TargetFramework>", RegexOptions.IgnoreCase))
            {
                result.Issues.Add(new DomainValidationIssue
                {
                    IssueId = "OUTDATED_TARGET_FRAMEWORK",
                    RuleName = "Target Framework",
                    Description = "Project not targeting modern .NET version",
                    Severity = IssueSeverity.Warning,
                    SuggestedFix = "Update to net8.0 target framework"
                });
            }

            // Check for nullable reference types
            if (!Regex.IsMatch(content, @"<Nullable>enable</Nullable>", RegexOptions.IgnoreCase))
            {
                result.Warnings.Add(new DomainValidationWarning
                {
                    WarningId = "NULLABLE_NOT_ENABLED",
                    Description = "Nullable reference types not enabled",
                    Recommendation = "Enable nullable reference types for better null safety"
                });
            }
        }

        private async Task ValidateStartupConfigurationAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for proper service registration
            if (Regex.IsMatch(content, @"services\.Add\w+\(", RegexOptions.IgnoreCase))
            {
                if (!Regex.IsMatch(content, @"services\.AddLogging\(", RegexOptions.IgnoreCase))
                {
                    result.Warnings.Add(new DomainValidationWarning
                    {
                        WarningId = "LOGGING_NOT_CONFIGURED",
                        Description = "Logging not explicitly configured",
                        Recommendation = "Add services.AddLogging() for proper logging setup"
                    });
                }
            }
        }

        private async Task ValidateConfigurationAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for JSON structure if it's appsettings
            if (content.TrimStart().StartsWith("{"))
            {
                try
                {
                    System.Text.Json.JsonDocument.Parse(content);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    result.Issues.Add(new DomainValidationIssue
                    {
                        IssueId = "INVALID_JSON_CONFIG",
                        RuleName = "JSON Configuration",
                        Description = $"Invalid JSON in configuration: {ex.Message}",
                        Severity = IssueSeverity.Error,
                        SuggestedFix = "Fix JSON syntax errors"
                    });
                }
            }
        }

        private async Task ValidateControllerAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for proper API controller attributes
            if (Regex.IsMatch(content, @"class\s+\w*Controller", RegexOptions.IgnoreCase))
            {
                if (!Regex.IsMatch(content, @"\[ApiController\]", RegexOptions.IgnoreCase))
                {
                    result.Warnings.Add(new DomainValidationWarning
                    {
                        WarningId = "MISSING_API_CONTROLLER_ATTRIBUTE",
                        Description = "Missing [ApiController] attribute",
                        Recommendation = "Add [ApiController] attribute for better API behavior"
                    });
                }
            }
        }

        private async Task ValidateServiceAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for proper interface implementation
            if (Regex.IsMatch(content, @"class\s+\w*Service", RegexOptions.IgnoreCase))
            {
                if (!Regex.IsMatch(content, @":\s*I\w*Service", RegexOptions.IgnoreCase))
                {
                    result.Warnings.Add(new DomainValidationWarning
                    {
                        WarningId = "SERVICE_WITHOUT_INTERFACE",
                        Description = "Service class without interface",
                        Recommendation = "Consider implementing an interface for better testability"
                    });
                }
            }
        }

        private async Task ValidateGeneralDotNetCorePatternsAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // General .NET Core pattern validation
            if (content.Length > 10000 && !content.Contains("namespace"))
            {
                result.Warnings.Add(new DomainValidationWarning
                {
                    WarningId = "LARGE_FILE_NO_NAMESPACE",
                    Description = "Large file without namespace declaration",
                    Recommendation = "Organize code with proper namespace structure"
                });
            }
        }

        #endregion

        #region Private Feature Extraction Methods

        private List<DomainFeature> ExtractMigrationReadinessFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
            {
                var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";
                
                // Feature: Legacy framework dependency count
                var legacyMatches = Regex.Matches(codeContent, @"System\.Web\.|System\.Configuration\.|HttpContext\.Current", RegexOptions.IgnoreCase);
                features.Add(new DomainFeature
                {
                    FeatureName = "Legacy_Framework_Dependencies",
                    FeatureType = "Numeric",
                    FeatureValue = legacyMatches.Count,
                    Description = "Count of legacy .NET Framework dependencies"
                });

                // Feature: Migration readiness score
                var modernPatterns = Regex.Matches(codeContent, @"IConfiguration|ILogger<|services\.Add", RegexOptions.IgnoreCase).Count;
                var legacyPatterns = legacyMatches.Count;
                var readinessScore = modernPatterns > 0 ? (double)modernPatterns / (modernPatterns + legacyPatterns) : 0.0;
                
                features.Add(new DomainFeature
                {
                    FeatureName = "Migration_Readiness_Score",
                    FeatureType = "Numeric",
                    FeatureValue = readinessScore,
                    Description = "Migration readiness score (0-1)"
                });
            }

            return features;
        }

        private List<DomainFeature> ExtractModernizationFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
            {
                var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";
                
                // Feature: Modern pattern usage
                var modernPatternCount = 0;
                if (Regex.IsMatch(codeContent, @"async\s+Task", RegexOptions.IgnoreCase)) modernPatternCount++;
                if (Regex.IsMatch(codeContent, @"ILogger<", RegexOptions.IgnoreCase)) modernPatternCount++;
                if (Regex.IsMatch(codeContent, @"IConfiguration", RegexOptions.IgnoreCase)) modernPatternCount++;
                if (Regex.IsMatch(codeContent, @"services\.Add", RegexOptions.IgnoreCase)) modernPatternCount++;

                features.Add(new DomainFeature
                {
                    FeatureName = "Modern_Pattern_Count",
                    FeatureType = "Numeric",
                    FeatureValue = modernPatternCount,
                    Description = "Count of modern .NET patterns used"
                });
            }

            return features;
        }

        private List<DomainFeature> ExtractPerformanceFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
            {
                var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";
                
                // Feature: Async/await usage
                var asyncMatches = Regex.Matches(codeContent, @"async\s+Task|await\s+", RegexOptions.IgnoreCase);
                features.Add(new DomainFeature
                {
                    FeatureName = "Async_Pattern_Usage",
                    FeatureType = "Numeric",
                    FeatureValue = asyncMatches.Count,
                    Description = "Count of async/await pattern usage"
                });
            }

            return features;
        }

        private List<DomainFeature> ExtractCompatibilityFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
            {
                var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";
                
                // Feature: Compatibility issues count
                var compatibilityIssues = 0;
                if (Regex.IsMatch(codeContent, @"System\.Web\.", RegexOptions.IgnoreCase)) compatibilityIssues++;
                if (Regex.IsMatch(codeContent, @"HttpContext\.Current", RegexOptions.IgnoreCase)) compatibilityIssues++;
                if (Regex.IsMatch(codeContent, @"ConfigurationManager", RegexOptions.IgnoreCase)) compatibilityIssues++;

                features.Add(new DomainFeature
                {
                    FeatureName = "Compatibility_Issues_Count",
                    FeatureType = "Numeric",
                    FeatureValue = compatibilityIssues,
                    Description = "Count of potential compatibility issues"
                });
            }

            return features;
        }

        #endregion

        #region Private Helper Methods

        private void CalculatePatternConfidenceScores(DomainPatternResult result)
        {
            foreach (var pattern in result.DetectedPatterns)
            {
                result.PatternConfidenceScores[pattern.PatternId] = pattern.ConfidenceScore;
            }
        }

        private void CalculateFeatureImportanceScores(DomainFeatureResult result)
        {
            foreach (var feature in result.ExtractedFeatures)
            {
                var importance = feature.FeatureName switch
                {
                    "Migration_Readiness_Score" => 0.95,
                    "Legacy_Framework_Dependencies" => 0.90,
                    "Compatibility_Issues_Count" => 0.85,
                    "Modern_Pattern_Count" => 0.80,
                    _ => 0.70
                };
                result.FeatureImportanceScores[feature.FeatureName] = importance;
            }
        }

        private List<DomainOptimizationSuggestion> GenerateOptimizationSuggestions(
            DomainPattern pattern,
            DomainOptimizationConfig config)
        {
            var suggestions = new List<DomainOptimizationSuggestion>();

            switch (pattern.PatternId)
            {
                case "LEGACY_FRAMEWORK_REFERENCES":
                    suggestions.Add(new DomainOptimizationSuggestion
                    {
                        SuggestionId = "MIGRATE_FRAMEWORK_REFERENCES",
                        Title = "Migrate Legacy Framework References",
                        Description = "Replace .NET Framework references with .NET Core equivalents",
                        Category = OptimizationCategory.Maintainability,
                        Priority = OptimizationPriority.High,
                        ExpectedImprovement = 0.8,
                        Implementation = "Update using statements and replace legacy APIs"
                    });
                    break;

                case "CONFIGURATION_MANAGER_USAGE":
                    suggestions.Add(new DomainOptimizationSuggestion
                    {
                        SuggestionId = "MODERNIZE_CONFIGURATION",
                        Title = "Modernize Configuration Access",
                        Description = "Replace ConfigurationManager with IConfiguration",
                        Category = OptimizationCategory.Maintainability,
                        Priority = OptimizationPriority.Medium,
                        ExpectedImprovement = 0.6,
                        Implementation = "Use IConfiguration and Options pattern"
                    });
                    break;

                case "HARDCODED_SECRETS":
                    suggestions.Add(new DomainOptimizationSuggestion
                    {
                        SuggestionId = "SECURE_SECRETS_MANAGEMENT",
                        Title = "Implement Secure Secrets Management",
                        Description = "Move hardcoded secrets to secure configuration",
                        Category = OptimizationCategory.Security,
                        Priority = OptimizationPriority.Critical,
                        ExpectedImprovement = 1.0,
                        Implementation = "Use Azure Key Vault or secure configuration"
                    });
                    break;
            }

            return suggestions;
        }

        private void CalculateExpectedImprovements(DomainOptimizationResult result)
        {
            foreach (var suggestion in result.Suggestions)
            {
                result.ExpectedImprovements[suggestion.SuggestionId] = suggestion.ExpectedImprovement;
            }
        }

        private List<DomainMigrationRecommendation> GenerateMigrationRecommendations(
            string legacyPattern,
            string targetVersion,
            DomainMigrationConfig config)
        {
            var recommendations = new List<DomainMigrationRecommendation>();

            // .NET Framework to .NET Core migration recommendations
            if (legacyPattern.Contains("System.Web"))
            {
                recommendations.Add(new DomainMigrationRecommendation
                {
                    RecommendationId = "MIGRATE_SYSTEM_WEB",
                    Title = "Migrate System.Web Dependencies",
                    Description = "Replace System.Web with ASP.NET Core equivalents",
                    Category = MigrationCategory.API,
                    Complexity = MigrationComplexity.High,
                    LegacyPattern = "System.Web.*",
                    ModernPattern = "Microsoft.AspNetCore.*",
                    MigrationSteps = new List<string>
                    {
                        "Replace HttpContext.Current with IHttpContextAccessor",
                        "Replace HttpUtility with WebUtility or System.Text.Encodings.Web",
                        "Replace HttpServerUtility with built-in ASP.NET Core features",
                        "Update routing from Web Forms/MVC to ASP.NET Core routing"
                    }
                });
            }

            if (legacyPattern.Contains("ConfigurationManager"))
            {
                recommendations.Add(new DomainMigrationRecommendation
                {
                    RecommendationId = "MIGRATE_CONFIGURATION",
                    Title = "Migrate Configuration System",
                    Description = "Replace ConfigurationManager with IConfiguration",
                    Category = MigrationCategory.Configuration,
                    Complexity = MigrationComplexity.Medium,
                    LegacyPattern = "ConfigurationManager.AppSettings",
                    ModernPattern = "IConfiguration + Options pattern",
                    MigrationSteps = new List<string>
                    {
                        "Create appsettings.json configuration file",
                        "Register IConfiguration in dependency injection",
                        "Replace ConfigurationManager calls with IConfiguration",
                        "Implement strongly-typed configuration with IOptions<T>"
                    }
                });
            }

            return recommendations;
        }

        private void CalculateMigrationComplexityScores(DomainMigrationResult result)
        {
            foreach (var recommendation in result.Recommendations)
            {
                var complexityScore = recommendation.Complexity switch
                {
                    MigrationComplexity.Low => 0.25,
                    MigrationComplexity.Medium => 0.5,
                    MigrationComplexity.High => 0.75,
                    MigrationComplexity.VeryHigh => 1.0,
                    _ => 0.5
                };
                result.MigrationComplexityScores[recommendation.RecommendationId] = complexityScore;
            }
        }

        private Dictionary<string, DotNetCorePatternRule> InitializePatternRules()
        {
            return new Dictionary<string, DotNetCorePatternRule>();
        }

        private Dictionary<string, DotNetCoreValidationRule> InitializeValidationRules()
        {
            return new Dictionary<string, DotNetCoreValidationRule>();
        }

        #endregion
    }

    #region .NET Core-Specific Rule Models

    public class DotNetCorePatternRule
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public PatternSeverity Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string RecommendedAction { get; set; } = string.Empty;
    }

    public class DotNetCoreValidationRule
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string ValidationPattern { get; set; } = string.Empty;
        public IssueSeverity Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SuggestedFix { get; set; } = string.Empty;
    }

    #endregion
}
