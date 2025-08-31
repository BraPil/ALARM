using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ALARM.Analyzers.PatternDetection;

namespace ALARM.DomainLibraries
{
    /// <summary>
    /// Manages and orchestrates multiple domain-specific libraries
    /// Provides unified interface for pattern detection across all domains
    /// </summary>
    public class DomainLibraryManager
    {
        private readonly ILogger<DomainLibraryManager> _logger;
        private readonly Dictionary<string, IDomainLibrary> _domainLibraries;
        private readonly DomainLibraryConfig _config;

        public DomainLibraryManager(ILogger<DomainLibraryManager> logger, DomainLibraryConfig? config = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? new DomainLibraryConfig();
            _domainLibraries = new Dictionary<string, IDomainLibrary>();
        }

        /// <summary>
        /// Register a domain library with the manager
        /// </summary>
        public void RegisterDomainLibrary(IDomainLibrary domainLibrary)
        {
            if (domainLibrary == null)
                throw new ArgumentNullException(nameof(domainLibrary));

            _domainLibraries[domainLibrary.DomainName] = domainLibrary;
            _logger.LogInformation("Registered domain library: {DomainName} v{Version}", 
                domainLibrary.DomainName, domainLibrary.Version);
        }

        /// <summary>
        /// Get all registered domain libraries
        /// </summary>
        public IReadOnlyDictionary<string, IDomainLibrary> GetRegisteredLibraries()
        {
            return _domainLibraries.AsReadOnly();
        }

        /// <summary>
        /// Detect patterns across all registered domain libraries
        /// </summary>
        public async Task<CombinedDomainPatternResult> DetectPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            _logger.LogInformation("Starting combined domain pattern detection across {LibraryCount} libraries", 
                _domainLibraries.Count);

            var result = new CombinedDomainPatternResult
            {
                DetectionTimestamp = DateTime.UtcNow,
                DomainResults = new Dictionary<string, DomainPatternResult>()
            };

            try
            {
                // Run pattern detection in parallel for all domain libraries
                var detectionTasks = _domainLibraries.Values.Select(async library =>
                {
                    try
                    {
                        _logger.LogDebug("Running pattern detection for domain: {DomainName}", library.DomainName);
                        var domainResult = await library.DetectPatternsAsync(data, config);
                        return (library.DomainName, domainResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in pattern detection for domain: {DomainName}", library.DomainName);
                        return (library.DomainName, new DomainPatternResult
                        {
                            DomainName = library.DomainName,
                            DetectionTimestamp = DateTime.UtcNow
                        });
                    }
                });

                var detectionResults = await Task.WhenAll(detectionTasks);

                // Collect results from all domains
                foreach (var (domainName, domainResult) in detectionResults)
                {
                    result.DomainResults[domainName] = domainResult;
                    result.AllDetectedPatterns.AddRange(domainResult.DetectedPatterns);
                    result.AllDetectedAnomalies.AddRange(domainResult.DetectedAnomalies);
                }

                // Analyze cross-domain patterns and conflicts
                await AnalyzeCrossDomainPatternsAsync(result);

                // Calculate combined metrics
                CalculateCombinedMetrics(result);

                _logger.LogInformation("Combined domain pattern detection completed: {TotalPatterns} patterns, {TotalAnomalies} anomalies across {DomainCount} domains",
                    result.AllDetectedPatterns.Count, result.AllDetectedAnomalies.Count, result.DomainResults.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during combined domain pattern detection");
                throw;
            }
        }

        /// <summary>
        /// Validate content using appropriate domain libraries
        /// </summary>
        public async Task<CombinedDomainValidationResult> ValidateAsync(
            string content,
            string contentType,
            DomainValidationConfig config,
            IEnumerable<string>? targetDomains = null)
        {
            _logger.LogInformation("Starting combined domain validation for content type: {ContentType}", contentType);

            var result = new CombinedDomainValidationResult
            {
                IsValid = true,
                DomainResults = new Dictionary<string, DomainValidationResult>()
            };

            try
            {
                // Determine which domains to use for validation
                var domainsToValidate = targetDomains?.ToList() ?? _domainLibraries.Keys.ToList();
                
                // Run validation in parallel for selected domain libraries
                var validationTasks = domainsToValidate.Select(async domainName =>
                {
                    if (!_domainLibraries.TryGetValue(domainName, out var library))
                    {
                        _logger.LogWarning("Domain library not found: {DomainName}", domainName);
                        return (domainName, new DomainValidationResult { IsValid = true });
                    }

                    try
                    {
                        _logger.LogDebug("Running validation for domain: {DomainName}", domainName);
                        var domainResult = await library.ValidateAsync(content, contentType, config);
                        return (domainName, domainResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in validation for domain: {DomainName}", domainName);
                        return (domainName, new DomainValidationResult
                        {
                            IsValid = false,
                            Issues = new List<DomainValidationIssue>
                            {
                                new DomainValidationIssue
                                {
                                    IssueId = "DOMAIN_VALIDATION_ERROR",
                                    RuleName = "System Error",
                                    Description = $"Validation failed for domain {domainName}: {ex.Message}",
                                    Severity = IssueSeverity.Error
                                }
                            }
                        });
                    }
                });

                var validationResults = await Task.WhenAll(validationTasks);

                // Collect results from all domains
                foreach (var (domainName, domainResult) in validationResults)
                {
                    result.DomainResults[domainName] = domainResult;
                    result.AllIssues.AddRange(domainResult.Issues);
                    result.AllWarnings.AddRange(domainResult.Warnings);
                    
                    if (!domainResult.IsValid)
                        result.IsValid = false;
                }

                // Analyze cross-domain validation conflicts
                AnalyzeCrossDomainValidationConflicts(result);

                _logger.LogInformation("Combined domain validation completed: {IsValid}, {TotalIssues} issues, {TotalWarnings} warnings across {DomainCount} domains",
                    result.IsValid, result.AllIssues.Count, result.AllWarnings.Count, result.DomainResults.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during combined domain validation");
                result.IsValid = false;
                result.AllIssues.Add(new DomainValidationIssue
                {
                    IssueId = "COMBINED_VALIDATION_ERROR",
                    RuleName = "System Error",
                    Description = $"Combined validation failed: {ex.Message}",
                    Severity = IssueSeverity.Critical
                });
                return result;
            }
        }

        /// <summary>
        /// Extract features using all registered domain libraries
        /// </summary>
        public async Task<CombinedDomainFeatureResult> ExtractFeaturesAsync(
            IEnumerable<PatternData> data,
            DomainFeatureConfig config)
        {
            _logger.LogInformation("Starting combined domain feature extraction across {LibraryCount} libraries", 
                _domainLibraries.Count);

            var result = new CombinedDomainFeatureResult
            {
                DomainResults = new Dictionary<string, DomainFeatureResult>()
            };

            try
            {
                // Run feature extraction in parallel for all domain libraries
                var extractionTasks = _domainLibraries.Values.Select(async library =>
                {
                    try
                    {
                        _logger.LogDebug("Running feature extraction for domain: {DomainName}", library.DomainName);
                        var domainResult = await library.ExtractFeaturesAsync(data, config);
                        return (library.DomainName, domainResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in feature extraction for domain: {DomainName}", library.DomainName);
                        return (library.DomainName, new DomainFeatureResult());
                    }
                });

                var extractionResults = await Task.WhenAll(extractionTasks);

                // Collect results from all domains
                foreach (var (domainName, domainResult) in extractionResults)
                {
                    result.DomainResults[domainName] = domainResult;
                    result.AllExtractedFeatures.AddRange(domainResult.ExtractedFeatures);
                    
                    // Merge feature importance scores
                    foreach (var kvp in domainResult.FeatureImportanceScores)
                    {
                        result.CombinedFeatureImportanceScores[kvp.Key] = kvp.Value;
                    }
                }

                // Calculate cross-domain feature correlations
                CalculateCrossDomainFeatureCorrelations(result);

                _logger.LogInformation("Combined domain feature extraction completed: {TotalFeatures} features across {DomainCount} domains",
                    result.AllExtractedFeatures.Count, result.DomainResults.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during combined domain feature extraction");
                throw;
            }
        }

        /// <summary>
        /// Get optimization suggestions from all domain libraries
        /// </summary>
        public async Task<CombinedDomainOptimizationResult> GetOptimizationSuggestionsAsync(
            CombinedDomainPatternResult patternResults,
            DomainOptimizationConfig config)
        {
            _logger.LogInformation("Starting combined domain optimization analysis");

            var result = new CombinedDomainOptimizationResult
            {
                DomainResults = new Dictionary<string, DomainOptimizationResult>()
            };

            try
            {
                // Run optimization analysis in parallel for all domain libraries
                var optimizationTasks = _domainLibraries.Values.Select(async library =>
                {
                    if (!patternResults.DomainResults.TryGetValue(library.DomainName, out var domainPatternResult))
                    {
                        return (library.DomainName, new DomainOptimizationResult());
                    }

                    try
                    {
                        _logger.LogDebug("Running optimization analysis for domain: {DomainName}", library.DomainName);
                        var domainResult = await library.GetOptimizationSuggestionsAsync(domainPatternResult, config);
                        return (library.DomainName, domainResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in optimization analysis for domain: {DomainName}", library.DomainName);
                        return (library.DomainName, new DomainOptimizationResult());
                    }
                });

                var optimizationResults = await Task.WhenAll(optimizationTasks);

                // Collect results from all domains
                foreach (var (domainName, domainResult) in optimizationResults)
                {
                    result.DomainResults[domainName] = domainResult;
                    result.AllSuggestions.AddRange(domainResult.Suggestions);
                    
                    // Merge expected improvements
                    foreach (var kvp in domainResult.ExpectedImprovements)
                    {
                        result.CombinedExpectedImprovements[kvp.Key] = kvp.Value;
                    }
                }

                // Prioritize suggestions across domains
                PrioritizeCrossDomainSuggestions(result);

                _logger.LogInformation("Combined domain optimization analysis completed: {TotalSuggestions} suggestions across {DomainCount} domains",
                    result.AllSuggestions.Count, result.DomainResults.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during combined domain optimization analysis");
                throw;
            }
        }

        #region Private Helper Methods

        private Task AnalyzeCrossDomainPatternsAsync(CombinedDomainPatternResult result)
        {
            // Analyze patterns that appear across multiple domains
            var patternGroups = result.AllDetectedPatterns
                .GroupBy(p => p.PatternName)
                .Where(g => g.Count() > 1);

            foreach (var group in patternGroups)
            {
                var crossDomainPattern = new CrossDomainPattern
                {
                    PatternName = group.Key,
                    DomainsDetected = group.Select(p => p.Metadata?.GetValueOrDefault("Domain")?.ToString() ?? "Unknown").ToList(),
                    AverageConfidence = group.Average(p => p.ConfidenceScore),
                    TotalOccurrences = group.Count()
                };

                result.CrossDomainPatterns.Add(crossDomainPattern);
            }

            // Detect conflicting patterns between domains
            DetectConflictingPatterns(result);
            return Task.CompletedTask;
        }

        private void DetectConflictingPatterns(CombinedDomainPatternResult result)
        {
            // Example: Legacy patterns in one domain vs modern patterns in another
            var legacyPatterns = result.AllDetectedPatterns
                .Where(p => p.Severity == PatternSeverity.High && p.Category.Contains("Legacy"));
            var modernPatterns = result.AllDetectedPatterns
                .Where(p => p.Severity == PatternSeverity.Info && p.RecommendedAction.Contains("Good"));

            if (legacyPatterns.Any() && modernPatterns.Any())
            {
                result.PatternConflicts.Add(new PatternConflict
                {
                    ConflictType = "Legacy vs Modern",
                    Description = "Legacy patterns detected alongside modern patterns",
                    ConflictingPatterns = legacyPatterns.Concat(modernPatterns).ToList(),
                    Severity = ConflictSeverity.Medium,
                    RecommendedResolution = "Prioritize migration of legacy patterns to maintain consistency"
                });
            }
        }

        private void CalculateCombinedMetrics(CombinedDomainPatternResult result)
        {
            result.CombinedMetrics["TotalPatterns"] = result.AllDetectedPatterns.Count;
            result.CombinedMetrics["TotalAnomalies"] = result.AllDetectedAnomalies.Count;
            result.CombinedMetrics["AverageConfidence"] = result.AllDetectedPatterns.Any() 
                ? result.AllDetectedPatterns.Average(p => p.ConfidenceScore) : 0.0;
            result.CombinedMetrics["HighSeverityPatterns"] = result.AllDetectedPatterns.Count(p => p.Severity == PatternSeverity.High);
            result.CombinedMetrics["CriticalAnomalies"] = result.AllDetectedAnomalies.Count(a => a.Severity == AnomalySeverity.Critical);
        }

        private void AnalyzeCrossDomainValidationConflicts(CombinedDomainValidationResult result)
        {
            // Check for conflicting validation rules between domains
            var issueGroups = result.AllIssues
                .GroupBy(i => i.Location)
                .Where(g => g.Count() > 1 && g.Select(i => i.Severity).Distinct().Count() > 1);

            foreach (var group in issueGroups)
            {
                result.ValidationConflicts.Add(new ValidationConflict
                {
                    Location = group.Key,
                    ConflictingIssues = group.ToList(),
                    ConflictType = "Severity Disagreement",
                    RecommendedResolution = "Review domain-specific validation rules for consistency"
                });
            }
        }

        private void CalculateCrossDomainFeatureCorrelations(CombinedDomainFeatureResult result)
        {
            // Calculate correlations between features from different domains
            var featuresByName = result.AllExtractedFeatures
                .GroupBy(f => f.FeatureName)
                .Where(g => g.Count() > 1);

            foreach (var group in featuresByName)
            {
                var values = group.Select(f => f.FeatureValue).ToList();
                if (values.Count > 1)
                {
                    var correlation = CalculateCorrelation(values, values); // Simplified correlation
                    result.CrossDomainCorrelations[group.Key] = correlation;
                }
            }
        }

        private void PrioritizeCrossDomainSuggestions(CombinedDomainOptimizationResult result)
        {
            // Prioritize suggestions based on cross-domain impact and expected improvement
            result.PrioritizedSuggestions = result.AllSuggestions
                .OrderByDescending(s => s.Priority)
                .ThenByDescending(s => s.ExpectedImprovement)
                .ThenBy(s => s.Category)
                .ToList();

            // Group related suggestions
            var suggestionGroups = result.AllSuggestions
                .GroupBy(s => s.Category)
                .Where(g => g.Count() > 1);

            foreach (var group in suggestionGroups)
            {
                result.GroupedSuggestions[group.Key.ToString()] = group.ToList();
            }
        }

        private double CalculateCorrelation(List<double> x, List<double> y)
        {
            // Simplified correlation calculation
            if (x.Count != y.Count || x.Count < 2)
                return 0.0;

            var avgX = x.Average();
            var avgY = y.Average();
            var sumXY = x.Zip(y, (xi, yi) => (xi - avgX) * (yi - avgY)).Sum();
            var sumX2 = x.Sum(xi => Math.Pow(xi - avgX, 2));
            var sumY2 = y.Sum(yi => Math.Pow(yi - avgY, 2));

            if (sumX2 == 0 || sumY2 == 0)
                return 0.0;

            return sumXY / Math.Sqrt(sumX2 * sumY2);
        }

        #endregion
    }

    #region Combined Result Models

    /// <summary>
    /// Combined pattern detection results from multiple domain libraries
    /// </summary>
    public class CombinedDomainPatternResult
    {
        public DateTime DetectionTimestamp { get; set; }
        public Dictionary<string, DomainPatternResult> DomainResults { get; set; } = new();
        public List<DomainPattern> AllDetectedPatterns { get; set; } = new();
        public List<DomainAnomaly> AllDetectedAnomalies { get; set; } = new();
        public List<CrossDomainPattern> CrossDomainPatterns { get; set; } = new();
        public List<PatternConflict> PatternConflicts { get; set; } = new();
        public Dictionary<string, double> CombinedMetrics { get; set; } = new();
    }

    /// <summary>
    /// Combined validation results from multiple domain libraries
    /// </summary>
    public class CombinedDomainValidationResult
    {
        public bool IsValid { get; set; }
        public Dictionary<string, DomainValidationResult> DomainResults { get; set; } = new();
        public List<DomainValidationIssue> AllIssues { get; set; } = new();
        public List<DomainValidationWarning> AllWarnings { get; set; } = new();
        public List<ValidationConflict> ValidationConflicts { get; set; } = new();
    }

    /// <summary>
    /// Combined feature extraction results from multiple domain libraries
    /// </summary>
    public class CombinedDomainFeatureResult
    {
        public Dictionary<string, DomainFeatureResult> DomainResults { get; set; } = new();
        public List<DomainFeature> AllExtractedFeatures { get; set; } = new();
        public Dictionary<string, double> CombinedFeatureImportanceScores { get; set; } = new();
        public Dictionary<string, double> CrossDomainCorrelations { get; set; } = new();
    }

    /// <summary>
    /// Combined optimization results from multiple domain libraries
    /// </summary>
    public class CombinedDomainOptimizationResult
    {
        public Dictionary<string, DomainOptimizationResult> DomainResults { get; set; } = new();
        public List<DomainOptimizationSuggestion> AllSuggestions { get; set; } = new();
        public List<DomainOptimizationSuggestion> PrioritizedSuggestions { get; set; } = new();
        public Dictionary<string, List<DomainOptimizationSuggestion>> GroupedSuggestions { get; set; } = new();
        public Dictionary<string, double> CombinedExpectedImprovements { get; set; } = new();
    }

    /// <summary>
    /// Pattern detected across multiple domains
    /// </summary>
    public class CrossDomainPattern
    {
        public string PatternName { get; set; } = string.Empty;
        public List<string> DomainsDetected { get; set; } = new();
        public double AverageConfidence { get; set; }
        public int TotalOccurrences { get; set; }
    }

    /// <summary>
    /// Conflict between patterns from different domains
    /// </summary>
    public class PatternConflict
    {
        public string ConflictType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<DomainPattern> ConflictingPatterns { get; set; } = new();
        public ConflictSeverity Severity { get; set; }
        public string RecommendedResolution { get; set; } = string.Empty;
    }

    /// <summary>
    /// Conflict between validation results from different domains
    /// </summary>
    public class ValidationConflict
    {
        public string Location { get; set; } = string.Empty;
        public string ConflictType { get; set; } = string.Empty;
        public List<DomainValidationIssue> ConflictingIssues { get; set; } = new();
        public string RecommendedResolution { get; set; } = string.Empty;
    }

    /// <summary>
    /// Configuration for domain library manager
    /// </summary>
    public class DomainLibraryConfig
    {
        public bool EnableParallelProcessing { get; set; } = true;
        public int MaxConcurrentDomains { get; set; } = Environment.ProcessorCount;
        public bool EnableCrossDomainAnalysis { get; set; } = true;
        public bool EnableConflictDetection { get; set; } = true;
        public Dictionary<string, bool> DomainEnabledFlags { get; set; } = new();
    }

    /// <summary>
    /// Severity levels for cross-domain conflicts
    /// </summary>
    public enum ConflictSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    #endregion
}
