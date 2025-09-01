using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Enhanced feature extraction for ML model training with domain-specific capabilities
    /// Phase 2 Implementation: Advanced feature engineering for improved model accuracy
    /// </summary>
    public class EnhancedFeatureExtractor
    {
        private readonly ILogger<EnhancedFeatureExtractor> _logger;
        private readonly Dictionary<string, HashSet<string>> _domainKeywords;
        private readonly Dictionary<string, double> _technicalComplexityWeights;

        public EnhancedFeatureExtractor(ILogger<EnhancedFeatureExtractor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _domainKeywords = InitializeDomainKeywords();
            _technicalComplexityWeights = InitializeTechnicalComplexityWeights();
        }

        /// <summary>
        /// Extract comprehensive features for ML model training
        /// </summary>
        public async Task<EnhancedFeatureSet> ExtractFeaturesAsync(
            string suggestionText, 
            ValidationContext context)
        {
            _logger.LogDebug("Extracting enhanced features for suggestion: {SuggestionLength} characters", 
                suggestionText?.Length ?? 0);

            // Handle null context gracefully
            context ??= new ValidationContext();

            var features = new EnhancedFeatureSet
            {
                // Basic features (Phase 1 compatibility)
                WordCount = CountWords(suggestionText ?? string.Empty),
                CharacterCount = suggestionText?.Length ?? 0,
                SentenceCount = CountSentences(suggestionText ?? string.Empty),
                
                // Enhanced Phase 2 features
                DomainKeywords = ExtractDomainKeywords(suggestionText ?? string.Empty, context.DomainContext?.PrimaryDomains?.FirstOrDefault() ?? "Generic"),
                SemanticComplexity = CalculateSemanticComplexity(suggestionText ?? string.Empty),
                TechnicalComplexity = AssessTechnicalComplexity(suggestionText ?? string.Empty, context),
                ContextualRelevance = await CalculateContextualRelevanceAsync(suggestionText ?? string.Empty, context),
                
                // Advanced linguistic features
                ActionVerbCount = CountActionVerbs(suggestionText ?? string.Empty),
                TechnicalTermCount = CountTechnicalTerms(suggestionText ?? string.Empty),
                QuantifiableElementCount = CountQuantifiableElements(suggestionText ?? string.Empty),
                SpecificityScore = CalculateSpecificityScore(suggestionText ?? string.Empty),
                
                // Domain-specific features
                CADIntegrationScore = CalculateCADIntegrationScore(suggestionText ?? string.Empty),
                DatabaseOperationScore = CalculateDatabaseOperationScore(suggestionText ?? string.Empty),
                LegacyMigrationScore = CalculateLegacyMigrationScore(suggestionText ?? string.Empty),
                PerformanceImpactScore = CalculatePerformanceImpactScore(suggestionText ?? string.Empty)
            };

            _logger.LogDebug("Enhanced feature extraction completed. Domain keywords: {DomainKeywordCount}, " +
                "Technical complexity: {TechnicalComplexity:F2}", 
                features.DomainKeywords.Count, features.TechnicalComplexity);

            return features;
        }

        /// <summary>
        /// Extract domain-specific keywords with confidence scores
        /// </summary>
        public Dictionary<string, double> ExtractDomainKeywords(string text, string domain)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new Dictionary<string, double>();

            var keywords = new Dictionary<string, double>();
            var textLower = text.ToLowerInvariant();

            // Get domain-specific keyword set
            var domainKeywordSet = _domainKeywords.ContainsKey(domain) 
                ? _domainKeywords[domain] 
                : _domainKeywords["Generic"];

            foreach (var keyword in domainKeywordSet)
            {
                // Use simple contains check for better matching
                if (textLower.Contains(keyword.ToLowerInvariant()))
                {
                    // Calculate keyword confidence based on frequency and context
                    var count = CountOccurrences(textLower, keyword.ToLowerInvariant());
                    var confidence = Math.Min(1.0, count * 0.2 + 0.6); // More generous baseline
                    keywords[keyword] = confidence;
                }
            }

            return keywords;
        }

        /// <summary>
        /// Calculate semantic similarity using advanced text analysis
        /// </summary>
        public double CalculateSemanticSimilarity(string suggestion, string contextText)
        {
            if (string.IsNullOrWhiteSpace(suggestion) || string.IsNullOrWhiteSpace(contextText))
                return 0.0;

            // Extract key terms from both texts
            var suggestionTerms = ExtractKeyTerms(suggestion);
            var contextTerms = ExtractKeyTerms(contextText);

            if (!suggestionTerms.Any() || !contextTerms.Any())
                return 0.0;

            // Calculate Jaccard similarity coefficient
            var intersection = suggestionTerms.Intersect(contextTerms, StringComparer.OrdinalIgnoreCase).Count();
            var union = suggestionTerms.Union(contextTerms, StringComparer.OrdinalIgnoreCase).Count();

            var jaccardSimilarity = union > 0 ? (double)intersection / union : 0.0;

            // Enhance with semantic relationships
            var semanticBonus = CalculateSemanticRelationshipBonus(suggestionTerms, contextTerms);
            
            // More generous similarity calculation
            var baseSimilarity = jaccardSimilarity * 2.0; // Double the weight
            return Math.Min(1.0, baseSimilarity + semanticBonus);
        }

        /// <summary>
        /// Assess technical complexity of the suggestion
        /// </summary>
        public double AssessTechnicalComplexity(string suggestion, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(suggestion))
                return 0.0;

            var complexityScore = 0.0;
            var textLower = suggestion.ToLowerInvariant();

            // Base complexity from technical terms - more generous scoring
            foreach (var term in _technicalComplexityWeights.Keys)
            {
                if (textLower.Contains(term.ToLowerInvariant()))
                {
                    // Increase the weight contribution
                    complexityScore += _technicalComplexityWeights[term] * 1.5;
                }
            }

            // Context-based complexity adjustments
            if (context.ComplexityInfo != null)
            {
                // Adjust based on system complexity score
                var systemComplexityMultiplier = context.ComplexityInfo.ComplexityScore > 0.7 ? 1.3 : 
                                               context.ComplexityInfo.ComplexityScore > 0.5 ? 1.0 : 0.8;
                complexityScore *= systemComplexityMultiplier;

                // Adjust based on integration complexity
                if (context.ComplexityInfo.NumberOfIntegrations > 5)
                {
                    complexityScore += 0.2;
                }
            }

            // Normalize to 0-1 range
            return Math.Min(1.0, complexityScore / 5.0);
        }

        /// <summary>
        /// Extract contextual features based on validation context
        /// </summary>
        public Dictionary<string, double> ExtractContextualFeatures(string suggestion, ValidationContext context)
        {
            var features = new Dictionary<string, double>();

            if (context.DomainContext != null)
            {
                // Domain expertise alignment
                features["DomainExpertiseAlignment"] = CalculateDomainExpertiseAlignment(suggestion, context.DomainContext);
                
                // Technology stack alignment
                features["TechnologyStackAlignment"] = CalculateTechnologyStackAlignment(suggestion, context.DomainContext);
            }

            if (context.PerformanceConstraints != null)
            {
                // Performance constraint awareness
                features["PerformanceConstraintAwareness"] = CalculatePerformanceConstraintAwareness(suggestion, context.PerformanceConstraints);
            }

            if (context.QualityExpectations != null)
            {
                // Quality expectation alignment
                features["QualityExpectationAlignment"] = CalculateQualityExpectationAlignment(suggestion, context.QualityExpectations);
            }

            return features;
        }

        #region Private Helper Methods

        private Dictionary<string, HashSet<string>> InitializeDomainKeywords()
        {
            return new Dictionary<string, HashSet<string>>
            {
                ["ADDS"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "AutoCAD", "Map3D", "ADDS", "Oracle", "Database", "Migration", "Architecture", "Drawing", 
                    "Layer", "Block", "Entity", "Coordinate", "Projection", "Spatial", "GIS", 
                    "Workflow", "Process", "Legacy", "Modernization", "Performance", "Optimization"
                },
                ["AutoCAD"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Drawing", "Layer", "Block", "Entity", "Polyline", "Arc", "Circle", "Text",
                    "Dimension", "Viewport", "Layout", "Model", "Paper", "Plot", "Print",
                    "DWG", "DXF", "Template", "Standard", "Library", "Map3D", "AutoCAD"
                },
                ["Oracle"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Database", "Table", "Index", "Query", "SQL", "PL/SQL", "Procedure", "Function",
                    "Trigger", "View", "Sequence", "Package", "Schema", "Tablespace", "Connection",
                    "Performance", "Tuning", "Optimization", "Backup", "Recovery"
                },
                [".NET Core"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Framework", "Core", "API", "Controller", "Service", "Repository", "Model",
                    "Entity", "Configuration", "Dependency", "Injection", "Middleware", "Pipeline",
                    "Authentication", "Authorization", "Logging", "Testing", "Deployment"
                },
                ["Generic"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Implement", "Optimize", "Improve", "Enhance", "Refactor", "Migrate", "Update",
                    "Configure", "Install", "Deploy", "Test", "Validate", "Monitor", "Maintain"
                }
            };
        }

        private Dictionary<string, double> InitializeTechnicalComplexityWeights()
        {
            return new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                // High complexity terms
                ["Architecture"] = 1.0,
                ["Migration"] = 1.0,
                ["Integration"] = 0.9,
                ["Optimization"] = 0.8,
                ["Refactoring"] = 0.8,
                
                // Medium complexity terms
                ["Configuration"] = 0.6,
                ["Implementation"] = 0.6,
                ["Enhancement"] = 0.5,
                ["Update"] = 0.4,
                
                // Low complexity terms
                ["Documentation"] = 0.2,
                ["Testing"] = 0.3,
                ["Monitoring"] = 0.3
            };
        }

        private int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return text.Split(new char[] { ' ', '\t', '\n', '\r' }, 
                StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private int CountSentences(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return Regex.Matches(text, @"[.!?]+").Count;
        }

        private double CalculateSemanticComplexity(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;

            // Calculate based on sentence structure, vocabulary diversity, and technical depth
            var words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, 
                StringSplitOptions.RemoveEmptyEntries);
            
            if (words.Length == 0)
                return 0.0;

            // Vocabulary diversity (unique words / total words)
            var uniqueWords = words.Distinct(StringComparer.OrdinalIgnoreCase).Count();
            var vocabularyDiversity = (double)uniqueWords / words.Length;

            // Average word length
            var averageWordLength = words.Average(w => w.Length);

            // Technical term density
            var technicalTerms = words.Count(w => _technicalComplexityWeights.ContainsKey(w));
            var technicalDensity = (double)technicalTerms / words.Length;

            // Combine factors
            var semanticComplexity = (vocabularyDiversity * 0.4) + 
                                   (Math.Min(averageWordLength / 10.0, 1.0) * 0.3) + 
                                   (technicalDensity * 0.3);

            return Math.Min(1.0, semanticComplexity);
        }

        private async Task<double> CalculateContextualRelevanceAsync(string suggestion, ValidationContext context)
        {
            // Placeholder for advanced contextual relevance calculation
            // In a full implementation, this could use ML models or semantic analysis
            
            var relevanceScore = 0.5; // Base relevance
            
            // Adjust based on domain context
            if (context.DomainContext?.PrimaryDomains != null && context.DomainContext.PrimaryDomains.Any())
            {
                var domainKeywords = ExtractDomainKeywords(suggestion, context.DomainContext.PrimaryDomains.First());
                if (domainKeywords.Any())
                {
                    relevanceScore += domainKeywords.Values.Average() * 0.3;
                }
            }
            
            // Adjust based on performance constraints
            if (context.PerformanceConstraints != null)
            {
                var performanceTerms = new[] { "performance", "optimize", "fast", "efficient", "speed" };
                var hasPerformanceTerms = performanceTerms.Any(term => 
                    suggestion.Contains(term, StringComparison.OrdinalIgnoreCase));
                
                if (hasPerformanceTerms)
                {
                    relevanceScore += 0.2;
                }
            }

            return Math.Min(1.0, relevanceScore);
        }

        private int CountActionVerbs(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            var actionVerbs = new[] 
            {
                "implement", "create", "build", "develop", "design", "configure", "install",
                "deploy", "migrate", "update", "upgrade", "optimize", "improve", "enhance",
                "refactor", "test", "validate", "monitor", "maintain", "fix", "resolve"
            };

            var textLower = text.ToLowerInvariant();
            return actionVerbs.Count(verb => textLower.Contains(verb));
        }

        private int CountTechnicalTerms(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            var textLower = text.ToLowerInvariant();
            return _technicalComplexityWeights.Keys.Count(term => 
                textLower.Contains(term.ToLowerInvariant()));
        }

        private int CountQuantifiableElements(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            // Count numbers, percentages, time references, etc.
            var quantifiablePatterns = new[]
            {
                @"\d+%",           // Percentages
                @"\d+\s*(ms|sec|min|hour|day|week|month)", // Time units
                @"\d+\s*(MB|GB|TB|KB)", // Size units
                @"\d+\s*times?",   // Frequency
                @"\$\d+",          // Currency
                @"\d+\.\d+",       // Decimals
                @"\d+"             // Any numbers
            };

            return quantifiablePatterns.Sum(pattern => Regex.Matches(text, pattern, RegexOptions.IgnoreCase).Count);
        }

        private double CalculateSpecificityScore(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;

            var specificityFactors = 0.0;
            var textLower = text.ToLowerInvariant();

            // Specific technical terms
            if (_technicalComplexityWeights.Keys.Any(term => textLower.Contains(term.ToLowerInvariant())))
                specificityFactors += 0.3;

            // Quantifiable elements
            if (CountQuantifiableElements(text) > 0)
                specificityFactors += 0.2;

            // Action verbs
            if (CountActionVerbs(text) > 0)
                specificityFactors += 0.2;

            // Specific file types, technologies, or tools
            var specificTerms = new[] { ".dll", ".exe", ".config", "API", "SQL", "JSON", "XML" };
            if (specificTerms.Any(term => textLower.Contains(term.ToLowerInvariant())))
                specificityFactors += 0.3;

            return Math.Min(1.0, specificityFactors);
        }

        private double CalculateCADIntegrationScore(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;

            var cadTerms = new[] 
            {
                "autocad", "map3d", "drawing", "dwg", "dxf", "layer", "block", "entity",
                "coordinate", "projection", "spatial", "geometry", "viewport", "layout", "cad"
            };

            var textLower = text.ToLowerInvariant();
            var matchCount = cadTerms.Count(term => textLower.Contains(term));
            
            // More generous scoring - each term contributes more
            return Math.Min(1.0, matchCount * 0.2 + (matchCount > 0 ? 0.1 : 0.0));
        }

        private double CalculateDatabaseOperationScore(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;

            var dbTerms = new[] 
            {
                "database", "oracle", "sql", "query", "table", "index", "procedure", "function",
                "trigger", "view", "schema", "connection", "transaction", "commit", "rollback"
            };

            var textLower = text.ToLowerInvariant();
            var matchCount = dbTerms.Count(term => textLower.Contains(term));
            
            return Math.Min(1.0, matchCount * 0.12);
        }

        private double CalculateLegacyMigrationScore(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;

            var migrationTerms = new[] 
            {
                "migration", "migrate", "legacy", "modernize", "upgrade", "port", "convert",
                "transform", "transition", "replace", "deprecate", "sunset"
            };

            var textLower = text.ToLowerInvariant();
            var matchCount = migrationTerms.Count(term => textLower.Contains(term));
            
            return Math.Min(1.0, matchCount * 0.2);
        }

        private double CalculatePerformanceImpactScore(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;

            var performanceTerms = new[] 
            {
                "performance", "optimize", "speed", "fast", "slow", "efficient", "cache",
                "memory", "cpu", "load", "throughput", "latency", "response", "scalable"
            };

            var textLower = text.ToLowerInvariant();
            var matchCount = performanceTerms.Count(term => textLower.Contains(term));
            
            return Math.Min(1.0, matchCount * 0.15);
        }

        private HashSet<string> ExtractKeyTerms(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new HashSet<string>();

            // Remove common stop words and extract meaningful terms
            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for", "of", "with", "by", "is", "are", "was", "were", "be", "been", "have", "has", "had", "do", "does", "did", "will", "would", "could", "should", "may", "might", "must", "can", "this", "that", "these", "those"
            };

            var words = text.Split(new char[] { ' ', '\t', '\n', '\r', '.', ',', ';', ':', '!', '?' }, 
                StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2 && !stopWords.Contains(w))
                .Select(w => w.ToLowerInvariant());

            return new HashSet<string>(words);
        }

        private double CalculateSemanticRelationshipBonus(HashSet<string> suggestionTerms, HashSet<string> contextTerms)
        {
            // Simple semantic relationship detection
            // In a full implementation, this could use word embeddings or semantic networks
            
            var semanticPairs = new Dictionary<string, HashSet<string>>
            {
                ["database"] = new HashSet<string> { "sql", "query", "table", "oracle", "connection" },
                ["performance"] = new HashSet<string> { "optimize", "speed", "fast", "efficient", "cache" },
                ["migration"] = new HashSet<string> { "legacy", "modernize", "upgrade", "convert", "transform" },
                ["autocad"] = new HashSet<string> { "drawing", "dwg", "layer", "block", "map3d" }
            };

            var relationshipBonus = 0.0;
            
            foreach (var term in suggestionTerms)
            {
                if (semanticPairs.ContainsKey(term))
                {
                    var relatedTerms = semanticPairs[term];
                    var relatedCount = contextTerms.Count(ct => relatedTerms.Contains(ct));
                    relationshipBonus += relatedCount * 0.1;
                }
            }

            return Math.Min(0.3, relationshipBonus); // Cap at 30% bonus
        }

        private double CalculateDomainExpertiseAlignment(string suggestion, DomainSpecificContext domainContext)
        {
            if (domainContext.DomainExpertise == null || !domainContext.DomainExpertise.Any())
                return 0.5;

            var alignmentScore = 0.0;
            var textLower = suggestion.ToLowerInvariant();

            foreach (var expertise in domainContext.DomainExpertise)
            {
                if (textLower.Contains(expertise.Key.ToLowerInvariant()))
                {
                    alignmentScore += expertise.Value * 0.2;
                }
            }

            return Math.Min(1.0, alignmentScore);
        }

        private double CalculateTechnologyStackAlignment(string suggestion, DomainSpecificContext domainContext)
        {
            // Since TechnologyStack property doesn't exist, use DomainExpertise instead
            if (domainContext.DomainExpertise == null || !domainContext.DomainExpertise.Any())
                return 0.5;

            var alignmentScore = 0.0;
            var textLower = suggestion.ToLowerInvariant();

            foreach (var technology in domainContext.DomainExpertise.Keys)
            {
                if (textLower.Contains(technology.ToLowerInvariant()))
                {
                    alignmentScore += 0.15;
                }
            }

            return Math.Min(1.0, alignmentScore);
        }

        private double CalculatePerformanceConstraintAwareness(string suggestion, PerformanceConstraints constraints)
        {
            var awarenessScore = 0.5; // Base score
            var textLower = suggestion.ToLowerInvariant();

            // Check for performance-related terms
            var performanceTerms = new[] { "performance", "speed", "optimize", "efficient", "fast" };
            if (performanceTerms.Any(term => textLower.Contains(term)))
            {
                awarenessScore += 0.3;
            }

            // Check for resource-related terms
            var resourceTerms = new[] { "memory", "cpu", "disk", "network", "resource" };
            if (resourceTerms.Any(term => textLower.Contains(term)))
            {
                awarenessScore += 0.2;
            }

            return Math.Min(1.0, awarenessScore);
        }

        private double CalculateQualityExpectationAlignment(string suggestion, QualityExpectations expectations)
        {
            var alignmentScore = 0.5; // Base score
            var textLower = suggestion.ToLowerInvariant();

            // Check for quality-related terms
            var qualityTerms = new[] { "quality", "accurate", "reliable", "robust", "stable" };
            if (qualityTerms.Any(term => textLower.Contains(term)))
            {
                alignmentScore += 0.2;
            }

            // Check for testing-related terms
            var testingTerms = new[] { "test", "validate", "verify", "check", "ensure" };
            if (testingTerms.Any(term => textLower.Contains(term)))
            {
                alignmentScore += 0.2;
            }

            // Check for best practice terms
            var bestPracticeTerms = new[] { "standard", "practice", "guideline", "pattern", "architecture" };
            if (bestPracticeTerms.Any(term => textLower.Contains(term)))
            {
                alignmentScore += 0.1;
            }

            return Math.Min(1.0, alignmentScore);
        }

        private int CountOccurrences(string text, string searchTerm)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm))
                return 0;

            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(searchTerm, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                count++;
                index += searchTerm.Length;
            }
            return count;
        }

        #endregion
    }

    /// <summary>
    /// Comprehensive feature set for enhanced ML model training
    /// </summary>
    public class EnhancedFeatureSet
    {
        // Basic features (Phase 1 compatibility)
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public int SentenceCount { get; set; }

        // Enhanced linguistic features
        public Dictionary<string, double> DomainKeywords { get; set; } = new();
        public double SemanticComplexity { get; set; }
        public double TechnicalComplexity { get; set; }
        public double ContextualRelevance { get; set; }

        // Advanced linguistic features
        public int ActionVerbCount { get; set; }
        public int TechnicalTermCount { get; set; }
        public int QuantifiableElementCount { get; set; }
        public double SpecificityScore { get; set; }

        // Domain-specific features
        public double CADIntegrationScore { get; set; }
        public double DatabaseOperationScore { get; set; }
        public double LegacyMigrationScore { get; set; }
        public double PerformanceImpactScore { get; set; }

        /// <summary>
        /// Convert to flat dictionary for ML model consumption
        /// </summary>
        public Dictionary<string, double> ToFeatureDictionary()
        {
            var features = new Dictionary<string, double>
            {
                // Basic features
                ["WordCount"] = WordCount,
                ["CharacterCount"] = CharacterCount,
                ["SentenceCount"] = SentenceCount,

                // Enhanced features
                ["SemanticComplexity"] = SemanticComplexity,
                ["TechnicalComplexity"] = TechnicalComplexity,
                ["ContextualRelevance"] = ContextualRelevance,

                // Advanced features
                ["ActionVerbCount"] = ActionVerbCount,
                ["TechnicalTermCount"] = TechnicalTermCount,
                ["QuantifiableElementCount"] = QuantifiableElementCount,
                ["SpecificityScore"] = SpecificityScore,

                // Domain-specific features
                ["CADIntegrationScore"] = CADIntegrationScore,
                ["DatabaseOperationScore"] = DatabaseOperationScore,
                ["LegacyMigrationScore"] = LegacyMigrationScore,
                ["PerformanceImpactScore"] = PerformanceImpactScore
            };

            // Add domain keywords as separate features
            foreach (var keyword in DomainKeywords)
            {
                features[$"Keyword_{keyword.Key}"] = keyword.Value;
            }

            return features;
        }
    }
}
