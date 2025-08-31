using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ALARM.Analyzers.PatternDetection;
namespace ALARM.DomainLibraries.Oracle
{
    /// <summary>
    /// Oracle-specific pattern detection and analysis
    /// Specializes in Oracle 19c patterns, SQL optimization, and database best practices
    /// </summary>
    public class OraclePatterns : IDomainLibrary
    {
        private readonly ILogger<OraclePatterns> _logger;
        private readonly Dictionary<string, OraclePatternRule> _patternRules;
        private readonly Dictionary<string, OracleValidationRule> _validationRules;

        public string DomainName => "Oracle";
        public string Version => "19c.1.0";

        public IReadOnlyList<string> SupportedPatternCategories => new[]
        {
            "SQL_Optimization",
            "Connection_Management",
            "Performance",
            "Security",
            "Migration",
            "PL_SQL",
            "Data_Types",
            "Indexing"
        };

        public OraclePatterns(ILogger<OraclePatterns> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _patternRules = InitializePatternRules();
            _validationRules = InitializeValidationRules();
        }

        public async Task<DomainPatternResult> DetectPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            _logger.LogInformation("Starting Oracle pattern detection on {DataCount} samples", data.Count());

            var result = new DomainPatternResult
            {
                DomainName = DomainName,
                DetectionTimestamp = DateTime.UtcNow
            };

            try
            {
                // Detect SQL optimization patterns
                var sqlPatterns = await DetectSQLOptimizationPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(sqlPatterns);

                // Detect connection management patterns
                var connectionPatterns = await DetectConnectionManagementPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(connectionPatterns);

                // Detect performance patterns
                var performancePatterns = await DetectPerformancePatternsAsync(data, config);
                result.DetectedPatterns.AddRange(performancePatterns);

                // Detect security patterns
                var securityPatterns = await DetectSecurityPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(securityPatterns);

                // Detect PL/SQL patterns
                var plsqlPatterns = await DetectPLSQLPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(plsqlPatterns);

                // Detect migration patterns
                var migrationPatterns = await DetectMigrationPatternsAsync(data, config);
                result.DetectedPatterns.AddRange(migrationPatterns);

                // Calculate confidence scores
                CalculatePatternConfidenceScores(result);

                // Detect anomalies
                result.DetectedAnomalies = await DetectOracleAnomaliesAsync(data, result.DetectedPatterns);

                _logger.LogInformation("Oracle pattern detection completed: {PatternCount} patterns, {AnomalyCount} anomalies",
                    result.DetectedPatterns.Count, result.DetectedAnomalies.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Oracle pattern detection");
                throw;
            }
        }

        public async Task<DomainValidationResult> ValidateAsync(
            string content,
            string contentType,
            DomainValidationConfig config)
        {
            _logger.LogInformation("Starting Oracle validation for content type: {ContentType}", contentType);

            var result = new DomainValidationResult
            {
                IsValid = true
            };

            try
            {
                // Validate based on content type
                switch (contentType.ToLowerInvariant())
                {
                    case "sql":
                        await ValidateSQLAsync(content, result, config);
                        break;
                    case "plsql":
                    case "pl/sql":
                        await ValidatePLSQLAsync(content, result, config);
                        break;
                    case "connection":
                        await ValidateConnectionStringAsync(content, result, config);
                        break;
                    case "schema":
                        await ValidateSchemaAsync(content, result, config);
                        break;
                    default:
                        await ValidateGeneralOraclePatternsAsync(content, result, config);
                        break;
                }

                result.IsValid = result.Issues.Count == 0 || result.Issues.All(i => i.Severity != IssueSeverity.Critical);

                _logger.LogInformation("Oracle validation completed: {IsValid}, {IssueCount} issues, {WarningCount} warnings",
                    result.IsValid, result.Issues.Count, result.Warnings.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Oracle validation");
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
            _logger.LogInformation("Starting Oracle feature extraction");

            var result = new DomainFeatureResult();

            try
            {
                foreach (var dataPoint in data)
                {
                    // Extract SQL complexity features
                    var sqlFeatures = ExtractSQLComplexityFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(sqlFeatures);

                    // Extract connection features
                    var connectionFeatures = ExtractConnectionFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(connectionFeatures);

                    // Extract performance features
                    var performanceFeatures = ExtractPerformanceFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(performanceFeatures);

                    // Extract security features
                    var securityFeatures = ExtractSecurityFeatures(dataPoint);
                    result.ExtractedFeatures.AddRange(securityFeatures);
                }

                // Calculate feature importance scores
                CalculateFeatureImportanceScores(result);

                _logger.LogInformation("Oracle feature extraction completed: {FeatureCount} features extracted",
                    result.ExtractedFeatures.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Oracle feature extraction");
                throw;
            }
        }

        public async Task<DomainOptimizationResult> GetOptimizationSuggestionsAsync(
            DomainPatternResult patternResult,
            DomainOptimizationConfig config)
        {
            _logger.LogInformation("Starting Oracle optimization analysis");

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

                _logger.LogInformation("Oracle optimization analysis completed: {SuggestionCount} suggestions",
                    result.Suggestions.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Oracle optimization analysis");
                throw;
            }
        }

        public async Task<DomainMigrationResult> GetMigrationRecommendationsAsync(
            string legacyPattern,
            string targetVersion,
            DomainMigrationConfig config)
        {
            _logger.LogInformation("Starting Oracle migration analysis for pattern: {Pattern} to version: {Version}",
                legacyPattern, targetVersion);

            var result = new DomainMigrationResult();

            try
            {
                // Analyze legacy pattern and generate migration recommendations
                var recommendations = GenerateMigrationRecommendations(legacyPattern, targetVersion, config);
                result.Recommendations.AddRange(recommendations);

                // Calculate complexity scores
                CalculateMigrationComplexityScores(result);

                _logger.LogInformation("Oracle migration analysis completed: {RecommendationCount} recommendations",
                    result.Recommendations.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Oracle migration analysis");
                throw;
            }
        }

        #region Private Pattern Detection Methods

        private async Task<List<DomainPattern>> DetectSQLOptimizationPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("SQLContent") == true)
                {
                    var sqlContent = dataPoint.Metadata["SQLContent"]?.ToString() ?? "";

                    // Detect SELECT * anti-pattern
                    if (Regex.IsMatch(sqlContent, @"SELECT\s+\*\s+FROM", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "SQL_SELECT_STAR",
                            PatternName = "SELECT * Anti-pattern",
                            Category = "SQL_Optimization",
                            Description = "SELECT * statement detected - potential performance issue",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Specify explicit column names instead of SELECT *"
                        });
                    }

                    // Detect missing WHERE clause in UPDATE/DELETE
                    if (Regex.IsMatch(sqlContent, @"(UPDATE|DELETE)\s+\w+(?!\s+WHERE)", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "SQL_MISSING_WHERE",
                            PatternName = "Missing WHERE Clause",
                            Category = "SQL_Optimization",
                            Description = "UPDATE/DELETE without WHERE clause detected",
                            ConfidenceScore = 0.95,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Add WHERE clause to limit affected rows"
                        });
                    }

                    // Detect inefficient JOIN patterns
                    if (Regex.IsMatch(sqlContent, @"SELECT.*FROM\s+\w+\s*,\s*\w+.*WHERE", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "SQL_IMPLICIT_JOIN",
                            PatternName = "Implicit JOIN Pattern",
                            Category = "SQL_Optimization",
                            Description = "Implicit JOIN detected - consider explicit JOIN syntax",
                            ConfidenceScore = 0.80,
                            Severity = PatternSeverity.Low,
                            RecommendedAction = "Use explicit JOIN syntax for better readability"
                        });
                    }

                    // Detect function calls in WHERE clause
                    if (Regex.IsMatch(sqlContent, @"WHERE\s+\w*\(.*\)\s*[=<>]", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "SQL_FUNCTION_IN_WHERE",
                            PatternName = "Function in WHERE Clause",
                            Category = "SQL_Optimization",
                            Description = "Function call in WHERE clause may prevent index usage",
                            ConfidenceScore = 0.75,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Consider function-based indexes or query rewriting"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectConnectionManagementPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect connection string in code
                    if (Regex.IsMatch(codeContent, @"Data Source\s*=|Server\s*=|Host\s*=", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "HARDCODED_CONNECTION_STRING",
                            PatternName = "Hardcoded Connection String",
                            Category = "Connection_Management",
                            Description = "Connection string embedded in code",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Move connection strings to configuration"
                        });
                    }

                    // Detect missing using statements for connections
                    if (Regex.IsMatch(codeContent, @"new\s+OracleConnection", RegexOptions.IgnoreCase) &&
                        !Regex.IsMatch(codeContent, @"using\s*\(.*OracleConnection", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "CONNECTION_NOT_DISPOSED",
                            PatternName = "Connection Not Properly Disposed",
                            Category = "Connection_Management",
                            Description = "Oracle connection may not be properly disposed",
                            ConfidenceScore = 0.70,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Use using statement or ensure proper disposal"
                        });
                    }

                    // Detect connection pooling issues
                    if (Regex.IsMatch(codeContent, @"Pooling\s*=\s*false", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "CONNECTION_POOLING_DISABLED",
                            PatternName = "Connection Pooling Disabled",
                            Category = "Connection_Management",
                            Description = "Connection pooling is disabled",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Enable connection pooling for better performance"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectPerformancePatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                // Check for performance-related metadata
                if (dataPoint.Metadata?.ContainsKey("QueryExecutionTime") == true)
                {
                    var execTime = Convert.ToDouble(dataPoint.Metadata["QueryExecutionTime"]);
                    
                    if (execTime > 5000) // > 5 seconds
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "SLOW_QUERY",
                            PatternName = "Slow Query",
                            Category = "Performance",
                            Description = $"Slow query detected: {execTime}ms",
                            ConfidenceScore = 0.85,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Analyze query execution plan and add indexes"
                        });
                    }
                }

                if (dataPoint.Metadata?.ContainsKey("SQLContent") == true)
                {
                    var sqlContent = dataPoint.Metadata["SQLContent"]?.ToString() ?? "";

                    // Detect DISTINCT without necessity
                    if (Regex.IsMatch(sqlContent, @"SELECT\s+DISTINCT", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "UNNECESSARY_DISTINCT",
                            PatternName = "Potentially Unnecessary DISTINCT",
                            Category = "Performance",
                            Description = "DISTINCT clause detected - verify if necessary",
                            ConfidenceScore = 0.60,
                            Severity = PatternSeverity.Low,
                            RecommendedAction = "Review if DISTINCT is actually needed"
                        });
                    }

                    // Detect ORDER BY without LIMIT
                    if (Regex.IsMatch(sqlContent, @"ORDER\s+BY.*(?!LIMIT|ROWNUM)", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "ORDER_BY_WITHOUT_LIMIT",
                            PatternName = "ORDER BY Without LIMIT",
                            Category = "Performance",
                            Description = "ORDER BY without LIMIT may be inefficient",
                            ConfidenceScore = 0.65,
                            Severity = PatternSeverity.Low,
                            RecommendedAction = "Consider adding ROWNUM or pagination"
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
                if (dataPoint.Metadata?.ContainsKey("SQLContent") == true)
                {
                    var sqlContent = dataPoint.Metadata["SQLContent"]?.ToString() ?? "";

                    // Detect potential SQL injection vulnerabilities
                    if (Regex.IsMatch(sqlContent, @"['""].*\+.*['""]|['""].*&.*['""]", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "SQL_INJECTION_RISK",
                            PatternName = "SQL Injection Risk",
                            Category = "Security",
                            Description = "String concatenation in SQL detected - potential SQL injection risk",
                            ConfidenceScore = 0.80,
                            Severity = PatternSeverity.Critical,
                            RecommendedAction = "Use parameterized queries instead of string concatenation"
                        });
                    }
                }

                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect hardcoded passwords
                    if (Regex.IsMatch(codeContent, @"password\s*=\s*['""][^'""]+['""]", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "HARDCODED_PASSWORD",
                            PatternName = "Hardcoded Password",
                            Category = "Security",
                            Description = "Hardcoded password detected in code",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Critical,
                            RecommendedAction = "Move passwords to secure configuration or use integrated authentication"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectPLSQLPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("PLSQLContent") == true)
                {
                    var plsqlContent = dataPoint.Metadata["PLSQLContent"]?.ToString() ?? "";

                    // Detect exception handling patterns
                    if (Regex.IsMatch(plsqlContent, @"BEGIN.*END", RegexOptions.IgnoreCase | RegexOptions.Singleline) &&
                        !Regex.IsMatch(plsqlContent, @"EXCEPTION", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "PLSQL_MISSING_EXCEPTION_HANDLER",
                            PatternName = "Missing Exception Handler",
                            Category = "PL_SQL",
                            Description = "PL/SQL block without exception handling",
                            ConfidenceScore = 0.75,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Add EXCEPTION block for proper error handling"
                        });
                    }

                    // Detect cursor management
                    if (Regex.IsMatch(plsqlContent, @"CURSOR\s+\w+", RegexOptions.IgnoreCase) &&
                        !Regex.IsMatch(plsqlContent, @"CLOSE\s+\w+", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "PLSQL_CURSOR_NOT_CLOSED",
                            PatternName = "Cursor Not Properly Closed",
                            Category = "PL_SQL",
                            Description = "Cursor declared but not explicitly closed",
                            ConfidenceScore = 0.70,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Ensure cursors are properly closed"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainPattern>> DetectMigrationPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            var patterns = new List<DomainPattern>();

            foreach (var dataPoint in data)
            {
                if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
                {
                    var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";

                    // Detect legacy Oracle client usage
                    if (Regex.IsMatch(codeContent, @"System\.Data\.OracleClient", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "LEGACY_ORACLE_CLIENT",
                            PatternName = "Legacy Oracle Client",
                            Category = "Migration",
                            Description = "Legacy System.Data.OracleClient usage detected",
                            ConfidenceScore = 0.95,
                            Severity = PatternSeverity.High,
                            RecommendedAction = "Migrate to Oracle.ManagedDataAccess"
                        });
                    }

                    // Detect deprecated connection string parameters
                    if (Regex.IsMatch(codeContent, @"Provider\s*=\s*OraOLEDB", RegexOptions.IgnoreCase))
                    {
                        patterns.Add(new DomainPattern
                        {
                            PatternId = "DEPRECATED_OLEDB_PROVIDER",
                            PatternName = "Deprecated OLE DB Provider",
                            Category = "Migration",
                            Description = "Deprecated OLE DB provider usage detected",
                            ConfidenceScore = 0.90,
                            Severity = PatternSeverity.Medium,
                            RecommendedAction = "Migrate to Oracle.ManagedDataAccess"
                        });
                    }
                }
            }

            return patterns;
        }

        private async Task<List<DomainAnomaly>> DetectOracleAnomaliesAsync(
            IEnumerable<PatternData> data,
            List<DomainPattern> detectedPatterns)
        {
            var anomalies = new List<DomainAnomaly>();

            // Check for unusual pattern combinations
            var securityIssues = detectedPatterns.Where(p => p.Category == "Security").Count();
            var performanceIssues = detectedPatterns.Where(p => p.Category == "Performance").Count();

            if (securityIssues > 3 && performanceIssues > 5)
            {
                anomalies.Add(new DomainAnomaly
                {
                    AnomalyId = "HIGH_RISK_COMBINATION",
                    AnomalyType = "Risk Pattern Combination",
                    Description = "High number of security and performance issues detected",
                    AnomalyScore = 0.8,
                    Severity = AnomalySeverity.High,
                    RecommendedInvestigation = "Comprehensive code review recommended"
                });
            }

            return anomalies;
        }

        #endregion

        #region Private Validation Methods

        private async Task ValidateSQLAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for basic SQL syntax issues
            var keywords = new[] { "SELECT", "FROM", "WHERE", "INSERT", "UPDATE", "DELETE" };
            var hasKeyword = keywords.Any(k => content.ToUpperInvariant().Contains(k));

            if (!hasKeyword)
            {
                result.Issues.Add(new DomainValidationIssue
                {
                    IssueId = "INVALID_SQL",
                    RuleName = "SQL Syntax",
                    Description = "No recognizable SQL keywords found",
                    Severity = IssueSeverity.Error,
                    SuggestedFix = "Verify SQL statement syntax"
                });
            }

            // Check for Oracle-specific syntax
            if (Regex.IsMatch(content, @"ROWNUM\s*>\s*\d+", RegexOptions.IgnoreCase))
            {
                result.Warnings.Add(new DomainValidationWarning
                {
                    WarningId = "INEFFICIENT_ROWNUM_USAGE",
                    Description = "ROWNUM > n pattern detected - may be inefficient",
                    Recommendation = "Consider using OFFSET/FETCH or analytic functions"
                });
            }
        }

        private async Task ValidatePLSQLAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for PL/SQL block structure
            var hasBegin = Regex.IsMatch(content, @"\bBEGIN\b", RegexOptions.IgnoreCase);
            var hasEnd = Regex.IsMatch(content, @"\bEND\b", RegexOptions.IgnoreCase);

            if (hasBegin && !hasEnd)
            {
                result.Issues.Add(new DomainValidationIssue
                {
                    IssueId = "UNMATCHED_BEGIN_END",
                    RuleName = "PL/SQL Structure",
                    Description = "BEGIN without matching END",
                    Severity = IssueSeverity.Error,
                    SuggestedFix = "Add matching END statement"
                });
            }
        }

        private async Task ValidateConnectionStringAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Check for required connection string components
            if (!Regex.IsMatch(content, @"Data Source\s*=", RegexOptions.IgnoreCase))
            {
                result.Issues.Add(new DomainValidationIssue
                {
                    IssueId = "MISSING_DATA_SOURCE",
                    RuleName = "Connection String",
                    Description = "Missing Data Source in connection string",
                    Severity = IssueSeverity.Error,
                    SuggestedFix = "Add Data Source parameter"
                });
            }

            // Check for security best practices
            if (Regex.IsMatch(content, @"Persist Security Info\s*=\s*true", RegexOptions.IgnoreCase))
            {
                result.Warnings.Add(new DomainValidationWarning
                {
                    WarningId = "PERSIST_SECURITY_INFO",
                    Description = "Persist Security Info=true detected",
                    Recommendation = "Set to false for better security"
                });
            }
        }

        private async Task ValidateSchemaAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // Placeholder for schema validation
            result.Warnings.Add(new DomainValidationWarning
            {
                WarningId = "SCHEMA_VALIDATION_PLACEHOLDER",
                Description = "Schema validation not yet implemented",
                Recommendation = "Implement schema structure validation"
            });
        }

        private async Task ValidateGeneralOraclePatternsAsync(
            string content,
            DomainValidationResult result,
            DomainValidationConfig config)
        {
            // General Oracle pattern validation
            if (content.Length > 5000 && !content.Contains("--") && !content.Contains("/*"))
            {
                result.Warnings.Add(new DomainValidationWarning
                {
                    WarningId = "LARGE_CODE_NO_COMMENTS",
                    Description = "Large code block without comments",
                    Recommendation = "Add comments for better maintainability"
                });
            }
        }

        #endregion

        #region Private Feature Extraction Methods

        private List<DomainFeature> ExtractSQLComplexityFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("SQLContent") == true)
            {
                var sqlContent = dataPoint.Metadata["SQLContent"]?.ToString() ?? "";
                
                // Feature: SQL complexity score
                var joinCount = Regex.Matches(sqlContent, @"\bJOIN\b", RegexOptions.IgnoreCase).Count;
                var subqueryCount = Regex.Matches(sqlContent, @"\(SELECT\b", RegexOptions.IgnoreCase).Count;
                var complexityScore = joinCount * 2 + subqueryCount * 3;

                features.Add(new DomainFeature
                {
                    FeatureName = "SQL_Complexity_Score",
                    FeatureType = "Numeric",
                    FeatureValue = complexityScore,
                    Description = "SQL complexity based on JOINs and subqueries"
                });

                // Feature: Table count
                var tableMatches = Regex.Matches(sqlContent, @"FROM\s+(\w+)", RegexOptions.IgnoreCase);
                features.Add(new DomainFeature
                {
                    FeatureName = "Table_Count",
                    FeatureType = "Numeric",
                    FeatureValue = tableMatches.Count,
                    Description = "Number of tables referenced in query"
                });
            }

            return features;
        }

        private List<DomainFeature> ExtractConnectionFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("ConnectionInfo") == true)
            {
                // Extract connection-specific features
                features.Add(new DomainFeature
                {
                    FeatureName = "Connection_Pool_Enabled",
                    FeatureType = "Boolean",
                    FeatureValue = 1.0, // Placeholder
                    Description = "Whether connection pooling is enabled"
                });
            }

            return features;
        }

        private List<DomainFeature> ExtractPerformanceFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("QueryExecutionTime") == true)
            {
                var execTime = Convert.ToDouble(dataPoint.Metadata["QueryExecutionTime"]);
                features.Add(new DomainFeature
                {
                    FeatureName = "Query_Execution_Time",
                    FeatureType = "Numeric",
                    FeatureValue = execTime,
                    Description = "Query execution time in milliseconds"
                });

                // Feature: Performance category
                var perfCategory = execTime switch
                {
                    < 100 => 1.0, // Fast
                    < 1000 => 2.0, // Medium
                    < 5000 => 3.0, // Slow
                    _ => 4.0 // Very slow
                };

                features.Add(new DomainFeature
                {
                    FeatureName = "Performance_Category",
                    FeatureType = "Categorical",
                    FeatureValue = perfCategory,
                    Description = "Performance category (1=Fast, 2=Medium, 3=Slow, 4=Very Slow)"
                });
            }

            return features;
        }

        private List<DomainFeature> ExtractSecurityFeatures(PatternData dataPoint)
        {
            var features = new List<DomainFeature>();

            if (dataPoint.Metadata?.ContainsKey("CodeContent") == true)
            {
                var codeContent = dataPoint.Metadata["CodeContent"]?.ToString() ?? "";
                
                // Feature: Security risk score
                var riskScore = 0.0;
                if (Regex.IsMatch(codeContent, @"password\s*=", RegexOptions.IgnoreCase)) riskScore += 0.5;
                if (Regex.IsMatch(codeContent, @"['""].*\+.*['""]", RegexOptions.IgnoreCase)) riskScore += 0.3;

                features.Add(new DomainFeature
                {
                    FeatureName = "Security_Risk_Score",
                    FeatureType = "Numeric",
                    FeatureValue = riskScore,
                    Description = "Security risk score based on detected patterns"
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
                // Calculate importance based on feature type and domain knowledge
                var importance = feature.FeatureName switch
                {
                    "SQL_Complexity_Score" => 0.9,
                    "Query_Execution_Time" => 0.95,
                    "Security_Risk_Score" => 0.85,
                    _ => 0.7
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
                case "SLOW_QUERY":
                    suggestions.Add(new DomainOptimizationSuggestion
                    {
                        SuggestionId = "OPTIMIZE_SLOW_QUERY",
                        Title = "Optimize Slow Query",
                        Description = "Analyze execution plan and add appropriate indexes",
                        Category = OptimizationCategory.Performance,
                        Priority = OptimizationPriority.High,
                        ExpectedImprovement = 0.7,
                        Implementation = "Use EXPLAIN PLAN and create indexes on WHERE clause columns"
                    });
                    break;

                case "SQL_SELECT_STAR":
                    suggestions.Add(new DomainOptimizationSuggestion
                    {
                        SuggestionId = "REPLACE_SELECT_STAR",
                        Title = "Replace SELECT * with Explicit Columns",
                        Description = "Specify only required columns instead of SELECT *",
                        Category = OptimizationCategory.Performance,
                        Priority = OptimizationPriority.Medium,
                        ExpectedImprovement = 0.3,
                        Implementation = "Replace SELECT * with explicit column names"
                    });
                    break;

                case "SQL_INJECTION_RISK":
                    suggestions.Add(new DomainOptimizationSuggestion
                    {
                        SuggestionId = "FIX_SQL_INJECTION",
                        Title = "Fix SQL Injection Vulnerability",
                        Description = "Replace string concatenation with parameterized queries",
                        Category = OptimizationCategory.Security,
                        Priority = OptimizationPriority.Critical,
                        ExpectedImprovement = 1.0,
                        Implementation = "Use OracleParameter for all user inputs"
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

            // Example migration recommendations
            if (legacyPattern.Contains("System.Data.OracleClient"))
            {
                recommendations.Add(new DomainMigrationRecommendation
                {
                    RecommendationId = "MIGRATE_ORACLE_CLIENT",
                    Title = "Migrate to Oracle.ManagedDataAccess",
                    Description = "Replace deprecated System.Data.OracleClient with Oracle.ManagedDataAccess",
                    Category = MigrationCategory.Dependencies,
                    Complexity = MigrationComplexity.Medium,
                    LegacyPattern = "System.Data.OracleClient",
                    ModernPattern = "Oracle.ManagedDataAccess.Client",
                    MigrationSteps = new List<string>
                    {
                        "Install Oracle.ManagedDataAccess.Core NuGet package",
                        "Replace using System.Data.OracleClient with Oracle.ManagedDataAccess.Client",
                        "Update OracleConnection, OracleCommand, etc. references",
                        "Test connection strings and functionality"
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

        private Dictionary<string, OraclePatternRule> InitializePatternRules()
        {
            // Initialize pattern rules - this would be loaded from configuration in production
            return new Dictionary<string, OraclePatternRule>();
        }

        private Dictionary<string, OracleValidationRule> InitializeValidationRules()
        {
            // Initialize validation rules - this would be loaded from configuration in production
            return new Dictionary<string, OracleValidationRule>();
        }

        #endregion
    }

    #region Oracle-Specific Rule Models

    public class OraclePatternRule
    {
        public string RuleId { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public PatternSeverity Severity { get; set; }
        public string Description { get; set; } = string.Empty;
        public string RecommendedAction { get; set; } = string.Empty;
    }

    public class OracleValidationRule
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
