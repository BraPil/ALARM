using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ALARM.Analyzers.PatternDetection;
using ALARM.DomainLibraries.Core;

namespace ALARM.DomainLibraries.Core
{
    /// <summary>
    /// Manages and orchestrates multiple domain-specific libraries
    /// </summary>
    public class DomainLibraryManager
    {
        private readonly ILogger<DomainLibraryManager> _logger;
        private readonly Dictionary<string, IDomainLibrary> _domainLibraries;
        private readonly DomainLibraryConfig _config;

        public DomainLibraryManager(ILogger<DomainLibraryManager> logger, DomainLibraryConfig? config = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _domainLibraries = new Dictionary<string, IDomainLibrary>();
            _config = config ?? new DomainLibraryConfig();
        }

        /// <summary>
        /// Register a domain-specific library
        /// </summary>
        public void RegisterDomainLibrary(string name, IDomainLibrary library)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Domain library name cannot be null or empty", nameof(name));
            if (library == null)
                throw new ArgumentNullException(nameof(library));

            _domainLibraries[name] = library;
            _logger.LogInformation("Registered domain library: {Name} - {Description}", name, library.Description);
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
        public async Task<CombinedDomainPatternResult> DetectPatternsAsync(IEnumerable<PatternData> data, PatternDetectionConfig config)
        {
            var result = new CombinedDomainPatternResult
            {
                DomainResults = new Dictionary<string, DomainPatternResult>(),
                AllPatterns = new List<DomainPattern>(),
                AllAnomalies = new List<DomainAnomaly>(),
                CrossDomainPatterns = new List<DomainPattern>(),
                PatternConflicts = new List<PatternConflict>(),
                TotalProcessingTime = TimeSpan.Zero
            };

            var tasks = _domainLibraries.Select(async kvp =>
            {
                try
                {
                    var domainResult = await kvp.Value.DetectPatternsAsync(data, config);
                    return new { Domain = kvp.Key, Result = domainResult };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error detecting patterns in domain {Domain}", kvp.Key);
                    return null;
                }
            });

            var results = await Task.WhenAll(tasks);

            foreach (var domainResult in results.Where(r => r != null))
            {
                result.DomainResults[domainResult!.Domain] = domainResult.Result;
                result.AllPatterns.AddRange(domainResult.Result.Patterns);
                result.AllAnomalies.AddRange(domainResult.Result.Anomalies);
                result.TotalProcessingTime = result.TotalProcessingTime.Add(domainResult.Result.ProcessingTime);
            }

            // Analyze cross-domain patterns and conflicts
            result.CrossDomainPatterns = AnalyzeCrossDomainPatterns(result);
            result.PatternConflicts = DetectPatternConflicts(result);

            return result;
        }

        /// <summary>
        /// Validate content across all registered domain libraries
        /// </summary>
        public async Task<CombinedDomainValidationResult> ValidateAsync(string content, string contentType, DomainValidationConfig config)
        {
            var result = new CombinedDomainValidationResult
            {
                IsValid = true,
                DomainResults = new Dictionary<string, DomainValidationResult>(),
                AllWarnings = new List<DomainValidationWarning>(),
                AllErrors = new List<string>(),
                ValidationConflicts = new List<ValidationConflict>(),
                TotalProcessingTime = TimeSpan.Zero
            };

            var tasks = _domainLibraries.Select(async kvp =>
            {
                try
                {
                    var domainResult = await kvp.Value.ValidateAsync(content, contentType, config);
                    return new { Domain = kvp.Key, Result = domainResult };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating content in domain {Domain}", kvp.Key);
                    return null;
                }
            });

            var results = await Task.WhenAll(tasks);

            foreach (var domainResult in results.Where(r => r != null))
            {
                result.DomainResults[domainResult!.Domain] = domainResult.Result;
                result.AllWarnings.AddRange(domainResult.Result.Warnings);
                result.AllErrors.AddRange(domainResult.Result.Errors);
                result.TotalProcessingTime = result.TotalProcessingTime.Add(domainResult.Result.ProcessingTime);
                
                if (!domainResult.Result.IsValid)
                    result.IsValid = false;
            }

            // Analyze validation conflicts
            result.ValidationConflicts = AnalyzeValidationConflicts(result);

            return result;
        }

        /// <summary>
        /// Extract features across all registered domain libraries
        /// </summary>
        public async Task<CombinedDomainFeatureResult> ExtractFeaturesAsync(IEnumerable<PatternData> data, DomainFeatureConfig config)
        {
            var result = new CombinedDomainFeatureResult
            {
                DomainResults = new Dictionary<string, DomainFeatureResult>(),
                AllFeatures = new List<DomainFeature>(),
                CrossDomainCorrelations = new List<FeatureCorrelation>(),
                TotalProcessingTime = TimeSpan.Zero
            };

            var tasks = _domainLibraries.Select(async kvp =>
            {
                try
                {
                    var domainResult = await kvp.Value.ExtractFeaturesAsync(data, config);
                    return new { Domain = kvp.Key, Result = domainResult };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error extracting features in domain {Domain}", kvp.Key);
                    return null;
                }
            });

            var results = await Task.WhenAll(tasks);

            foreach (var domainResult in results.Where(r => r != null))
            {
                result.DomainResults[domainResult!.Domain] = domainResult.Result;
                result.AllFeatures.AddRange(domainResult.Result.Features);
                result.TotalProcessingTime = result.TotalProcessingTime.Add(domainResult.Result.ProcessingTime);
            }

            // Calculate cross-domain feature correlations
            result.CrossDomainCorrelations = CalculateCrossDomainFeatureCorrelations(result);

            return result;
        }

        /// <summary>
        /// Get optimization suggestions across all registered domain libraries
        /// </summary>
        public async Task<CombinedDomainOptimizationResult> GetOptimizationSuggestionsAsync(CombinedDomainPatternResult patterns, DomainOptimizationConfig config)
        {
            var result = new CombinedDomainOptimizationResult
            {
                DomainResults = new Dictionary<string, DomainOptimizationResult>(),
                AllSuggestions = new List<DomainOptimizationSuggestion>(),
                GroupedSuggestions = new Dictionary<string, List<DomainOptimizationSuggestion>>(),
                CombinedExpectedImprovements = new Dictionary<string, double>(),
                TotalProcessingTime = TimeSpan.Zero
            };

            var tasks = patterns.DomainResults.Select(async kvp =>
            {
                try
                {
                    if (_domainLibraries.TryGetValue(kvp.Key, out var library))
                    {
                        var domainResult = await library.GetOptimizationSuggestionsAsync(kvp.Value, config);
                        return new { Domain = kvp.Key, Result = domainResult };
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting optimization suggestions in domain {Domain}", kvp.Key);
                    return null;
                }
            });

            var results = await Task.WhenAll(tasks);

            foreach (var domainResult in results.Where(r => r != null))
            {
                result.DomainResults[domainResult!.Domain] = domainResult.Result;
                result.AllSuggestions.AddRange(domainResult.Result.Suggestions);
                result.TotalProcessingTime = result.TotalProcessingTime.Add(domainResult.Result.ProcessingTime);
            }

            // Group and prioritize cross-domain suggestions
            var suggestionGroups = result.AllSuggestions.GroupBy(s => s.Category);
            foreach (var group in suggestionGroups)
            {
                result.GroupedSuggestions[group.Key.ToString()] = group.ToList();
            }

            result.GroupedSuggestions = PrioritizeCrossDomainSuggestions(result);
            result.CombinedExpectedImprovements = CalculateCombinedMetrics(result);

            return result;
        }

        #region Private Helper Methods

        private List<DomainPattern> AnalyzeCrossDomainPatterns(CombinedDomainPatternResult result)
        {
            var crossDomainPatterns = new List<DomainPattern>();
            
            // Simple cross-domain pattern detection
            var patternsByName = result.AllPatterns.GroupBy(p => p.Name);
            foreach (var group in patternsByName.Where(g => g.Count() > 1))
            {
                crossDomainPatterns.Add(new DomainPattern
                {
                    Name = $"CrossDomain_{group.Key}",
                    Description = $"Pattern {group.Key} found across {group.Count()} domains",
                    Confidence = group.Average(p => p.Confidence),
                    Category = "CrossDomain",
                    Severity = "Medium"
                });
            }

            return crossDomainPatterns;
        }

        private List<PatternConflict> DetectPatternConflicts(CombinedDomainPatternResult result)
        {
            var conflicts = new List<PatternConflict>();
            
            // Detect conflicting patterns between domains
            foreach (var domain1 in result.DomainResults)
            {
                foreach (var domain2 in result.DomainResults.Where(d => d.Key != domain1.Key))
                {
                    var conflictingPatterns = domain1.Value.Patterns
                        .Where(p1 => domain2.Value.Patterns.Any(p2 => 
                            p1.Name == p2.Name && Math.Abs(p1.Confidence - p2.Confidence) > 0.3))
                        .ToList();

                    foreach (var pattern in conflictingPatterns)
                    {
                        conflicts.Add(new PatternConflict
                        {
                            PatternName = pattern.Name,
                            Domain1 = domain1.Key,
                            Domain2 = domain2.Key,
                            ConflictType = "ConfidenceDiscrepancy",
                            Description = $"Confidence mismatch for pattern {pattern.Name}"
                        });
                    }
                }
            }

            return conflicts;
        }

        private List<ValidationConflict> AnalyzeValidationConflicts(CombinedDomainValidationResult result)
        {
            var conflicts = new List<ValidationConflict>();
            
            // Analyze conflicts between domain validation results
            var domainValidations = result.DomainResults.ToList();
            for (int i = 0; i < domainValidations.Count; i++)
            {
                for (int j = i + 1; j < domainValidations.Count; j++)
                {
                    var domain1 = domainValidations[i];
                    var domain2 = domainValidations[j];
                    
                    if (domain1.Value.IsValid != domain2.Value.IsValid)
                    {
                        conflicts.Add(new ValidationConflict
                        {
                            Domain1 = domain1.Key,
                            Domain2 = domain2.Key,
                            ConflictType = "ValidationDisagreement",
                            Description = $"Domains {domain1.Key} and {domain2.Key} disagree on validation"
                        });
                    }
                }
            }

            return conflicts;
        }

        private List<FeatureCorrelation> CalculateCrossDomainFeatureCorrelations(CombinedDomainFeatureResult result)
        {
            var correlations = new List<FeatureCorrelation>();
            
            // Calculate correlations between features from different domains
            var allFeatures = result.AllFeatures.ToList();
            for (int i = 0; i < allFeatures.Count; i++)
            {
                for (int j = i + 1; j < allFeatures.Count; j++)
                {
                    var feature1 = allFeatures[i];
                    var feature2 = allFeatures[j];
                    
                    // Simple correlation calculation (would be more sophisticated in practice)
                    var correlation = CalculateCorrelation(feature1, feature2);
                    
                    if (Math.Abs(correlation) > 0.5) // Significant correlation
                    {
                        correlations.Add(new FeatureCorrelation
                        {
                            Feature1Name = feature1.Name,
                            Feature2Name = feature2.Name,
                            CorrelationCoefficient = correlation,
                            Significance = Math.Abs(correlation) > 0.7 ? "High" : "Medium"
                        });
                    }
                }
            }

            return correlations;
        }

        private Dictionary<string, List<DomainOptimizationSuggestion>> PrioritizeCrossDomainSuggestions(CombinedDomainOptimizationResult result)
        {
            var prioritized = new Dictionary<string, List<DomainOptimizationSuggestion>>();
            
            foreach (var group in result.GroupedSuggestions)
            {
                var sorted = group.Value
                    .OrderBy(s => s.Priority)
                    .ThenByDescending(s => GetImpactScore(s.Impact))
                    .ToList();
                    
                prioritized[group.Key] = sorted;
            }

            return prioritized;
        }

        private Dictionary<string, double> CalculateCombinedMetrics(CombinedDomainOptimizationResult result)
        {
            var metrics = new Dictionary<string, double>();
            
            // Calculate combined impact metrics
            var impactGroups = result.AllSuggestions.GroupBy(s => s.Impact);
            foreach (var group in impactGroups)
            {
                metrics[group.Key] = group.Count() * GetImpactScore(group.Key);
            }

            return metrics;
        }

        private double CalculateCorrelation(DomainFeature feature1, DomainFeature feature2)
        {
            // Simplified correlation calculation
            // In practice, this would use proper statistical methods
            return (feature1.Importance + feature2.Importance) / 2.0 - 0.5;
        }

        private double GetImpactScore(string impact)
        {
            return impact?.ToLower() switch
            {
                "high" => 3.0,
                "medium" => 2.0,
                "low" => 1.0,
                _ => 1.0
            };
        }

        #endregion
    }

    /// <summary>
    /// Configuration for domain library manager
    /// </summary>
    public class DomainLibraryConfig
    {
        public bool EnableCrossDomainAnalysis { get; set; } = true;
        public bool EnableConflictDetection { get; set; } = true;
        public double CorrelationThreshold { get; set; } = 0.5;
        public TimeSpan MaxProcessingTime { get; set; } = TimeSpan.FromMinutes(5);
    }

    #region Supporting Classes

    public class PatternConflict
    {
        public string PatternName { get; set; } = string.Empty;
        public string Domain1 { get; set; } = string.Empty;
        public string Domain2 { get; set; } = string.Empty;
        public string ConflictType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ValidationConflict
    {
        public string Domain1 { get; set; } = string.Empty;
        public string Domain2 { get; set; } = string.Empty;
        public string ConflictType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class FeatureCorrelation
    {
        public string Feature1Name { get; set; } = string.Empty;
        public string Feature2Name { get; set; } = string.Empty;
        public double CorrelationCoefficient { get; set; }
        public string Significance { get; set; } = string.Empty;
    }

    public class CombinedDomainPatternResult
    {
        public Dictionary<string, DomainPatternResult> DomainResults { get; set; } = new();
        public List<DomainPattern> AllPatterns { get; set; } = new();
        public List<DomainAnomaly> AllAnomalies { get; set; } = new();
        public List<DomainPattern> CrossDomainPatterns { get; set; } = new();
        public List<PatternConflict> PatternConflicts { get; set; } = new();
        public TimeSpan TotalProcessingTime { get; set; }
    }

    public class CombinedDomainValidationResult
    {
        public bool IsValid { get; set; }
        public Dictionary<string, DomainValidationResult> DomainResults { get; set; } = new();
        public List<DomainValidationWarning> AllWarnings { get; set; } = new();
        public List<string> AllErrors { get; set; } = new();
        public List<ValidationConflict> ValidationConflicts { get; set; } = new();
        public TimeSpan TotalProcessingTime { get; set; }
    }

    public class CombinedDomainFeatureResult
    {
        public Dictionary<string, DomainFeatureResult> DomainResults { get; set; } = new();
        public List<DomainFeature> AllFeatures { get; set; } = new();
        public List<FeatureCorrelation> CrossDomainCorrelations { get; set; } = new();
        public TimeSpan TotalProcessingTime { get; set; }
    }

    public class CombinedDomainOptimizationResult
    {
        public Dictionary<string, DomainOptimizationResult> DomainResults { get; set; } = new();
        public List<DomainOptimizationSuggestion> AllSuggestions { get; set; } = new();
        public Dictionary<string, List<DomainOptimizationSuggestion>> GroupedSuggestions { get; set; } = new();
        public Dictionary<string, double> CombinedExpectedImprovements { get; set; } = new();
        public TimeSpan TotalProcessingTime { get; set; }
    }

    #endregion
}
