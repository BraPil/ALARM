using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra;

namespace ALARM.Analyzers.PatternDetection
{
    /// <summary>
    /// Advanced pattern analysis engine for interpreting clustering and sequential pattern results
    /// </summary>
    public class PatternAnalysisEngine
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<PatternAnalysisEngine> _logger;

        public PatternAnalysisEngine(MLContext mlContext, ILogger<PatternAnalysisEngine> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Analyze patterns from clustering and sequential mining results
        /// </summary>
        public async Task<PatternAnalysisResult> AnalyzePatternsAsync(
            ClusteringResult clusteringResults,
            SequentialPatternResult sequentialResults,
            FeatureEngineeringResult featureResults,
            PatternDetectionConfig config)
        {
            _logger.LogInformation("Starting comprehensive pattern analysis");

            var result = new PatternAnalysisResult
            {
                AnalysisTimestamp = DateTime.UtcNow,
                IdentifiedPatterns = new List<IdentifiedPattern>(),
                AnomalousPatterns = new List<IdentifiedPattern>(),
                PatternRelationships = new List<PatternRelationship>(),
                FeatureImportance = new Dictionary<string, double>(),
                PatternMetrics = new PatternMetrics()
            };

            // Analyze clustering patterns
            var clusterPatterns = await AnalyzeClusterPatternsAsync(clusteringResults, featureResults, config);
            result.IdentifiedPatterns.AddRange(clusterPatterns.Patterns);
            result.AnomalousPatterns.AddRange(clusterPatterns.Anomalies);

            // Analyze sequential patterns
            var sequentialPatterns = await AnalyzeSequentialPatternsAsync(sequentialResults, config);
            result.IdentifiedPatterns.AddRange(sequentialPatterns.Patterns);
            result.AnomalousPatterns.AddRange(sequentialPatterns.Anomalies);

            // Analyze pattern relationships and correlations
            result.PatternRelationships = await AnalyzePatternRelationshipsAsync(
                result.IdentifiedPatterns, featureResults, config);

            // Calculate feature importance
            result.FeatureImportance = await CalculateFeatureImportanceAsync(
                clusteringResults, sequentialResults, featureResults);

            // Calculate overall pattern metrics
            result.PatternMetrics = CalculatePatternMetrics(result);

            // Identify pattern hierarchies and dependencies
            result.PatternHierarchies = await BuildPatternHierarchiesAsync(result.IdentifiedPatterns, config);

            // Generate pattern insights and interpretations
            result.PatternInsights = await GeneratePatternInsightsAsync(result, config);

            _logger.LogInformation("Pattern analysis completed. Found {PatternCount} patterns, {AnomalyCount} anomalies",
                result.IdentifiedPatterns.Count, result.AnomalousPatterns.Count);

            return result;
        }

        /// <summary>
        /// Analyze clustering results to identify meaningful patterns
        /// </summary>
        private async Task<ClusterPatternAnalysis> AnalyzeClusterPatternsAsync(
            ClusteringResult clusteringResults,
            FeatureEngineeringResult featureResults,
            PatternDetectionConfig config)
        {
            var patterns = new List<IdentifiedPattern>();
            var anomalies = new List<IdentifiedPattern>();

            foreach (var cluster in clusteringResults.Clusters)
            {
                // Analyze cluster characteristics
                var clusterAnalysis = AnalyzeClusterCharacteristics(cluster, featureResults);
                
                // Determine if cluster represents a pattern or anomaly
                if (cluster.Size >= config.MinClusterSizeForPattern && 
                    cluster.Cohesion >= config.MinCohesionForPattern)
                {
                    patterns.Add(new IdentifiedPattern
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = PatternType.Cluster,
                        Description = GenerateClusterDescription(cluster, clusterAnalysis),
                        Confidence = CalculateClusterConfidence(cluster, clusteringResults),
                        Frequency = cluster.Size / (double)clusteringResults.TotalDataPoints,
                        Features = clusterAnalysis.DominantFeatures,
                        Metadata = new Dictionary<string, object>
                        {
                            ["ClusterId"] = cluster.Id,
                            ["ClusterSize"] = cluster.Size,
                            ["Cohesion"] = cluster.Cohesion,
                            ["Centroid"] = cluster.Centroid
                        }
                    });
                }
                else if (cluster.Size < config.MinClusterSizeForPattern && 
                         cluster.Cohesion < config.AnomalyThreshold)
                {
                    anomalies.Add(new IdentifiedPattern
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = PatternType.Anomaly,
                        Description = GenerateAnomalyDescription(cluster, clusterAnalysis),
                        Confidence = CalculateAnomalyConfidence(cluster),
                        Frequency = cluster.Size / (double)clusteringResults.TotalDataPoints,
                        Features = clusterAnalysis.OutlierFeatures,
                        Metadata = new Dictionary<string, object>
                        {
                            ["ClusterId"] = cluster.Id,
                            ["ClusterSize"] = cluster.Size,
                            ["AnomalyScore"] = cluster.AnomalyScore
                        }
                    });
                }
            }

            return new ClusterPatternAnalysis
            {
                Patterns = patterns,
                Anomalies = anomalies,
                ClusterQuality = CalculateOverallClusterQuality(clusteringResults),
                OptimalClusterCount = DetermineOptimalClusterCount(clusteringResults)
            };
        }

        /// <summary>
        /// Analyze sequential patterns to identify temporal relationships
        /// </summary>
        private async Task<SequentialPatternAnalysis> AnalyzeSequentialPatternsAsync(
            SequentialPatternResult sequentialResults,
            PatternDetectionConfig config)
        {
            var patterns = new List<IdentifiedPattern>();
            var anomalies = new List<IdentifiedPattern>();

            foreach (var sequentialPattern in sequentialResults.Patterns)
            {
                if (sequentialPattern.Support >= config.MinSupportForSequentialPattern &&
                    sequentialPattern.Confidence >= config.MinConfidenceForSequentialPattern)
                {
                    patterns.Add(new IdentifiedPattern
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = PatternType.Sequential,
                        Description = GenerateSequentialDescription(sequentialPattern),
                        Confidence = sequentialPattern.Confidence,
                        Frequency = sequentialPattern.Support,
                        Features = ExtractSequentialFeatures(sequentialPattern),
                        Metadata = new Dictionary<string, object>
                        {
                            ["PatternId"] = sequentialPattern.PatternId,
                            ["Sequence"] = sequentialPattern.Sequence,
                            ["Support"] = sequentialPattern.Support,
                            ["Length"] = sequentialPattern.Sequence.Count
                        }
                    });
                }
                else if (sequentialPattern.Support < config.MinSupportForSequentialPattern &&
                         sequentialPattern.Confidence > config.HighConfidenceThreshold)
                {
                    // High confidence but low support might indicate rare but important pattern
                    anomalies.Add(new IdentifiedPattern
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = PatternType.RareButSignificant,
                        Description = $"Rare but significant sequential pattern: {string.Join(" → ", sequentialPattern.Sequence)}",
                        Confidence = sequentialPattern.Confidence,
                        Frequency = sequentialPattern.Support,
                        Features = ExtractSequentialFeatures(sequentialPattern),
                        Metadata = new Dictionary<string, object>
                        {
                            ["PatternId"] = sequentialPattern.PatternId,
                            ["Sequence"] = sequentialPattern.Sequence,
                            ["RarityScore"] = 1.0 - sequentialPattern.Support
                        }
                    });
                }
            }

            return new SequentialPatternAnalysis
            {
                Patterns = patterns,
                Anomalies = anomalies,
                TemporalComplexity = CalculateTemporalComplexity(sequentialResults),
                AverageSequenceLength = sequentialResults.Patterns.Average(p => p.Sequence.Count)
            };
        }

        /// <summary>
        /// Analyze relationships between different patterns
        /// </summary>
        private async Task<List<PatternRelationship>> AnalyzePatternRelationshipsAsync(
            List<IdentifiedPattern> patterns,
            FeatureEngineeringResult featureResults,
            PatternDetectionConfig config)
        {
            var relationships = new List<PatternRelationship>();

            for (int i = 0; i < patterns.Count; i++)
            {
                for (int j = i + 1; j < patterns.Count; j++)
                {
                    var pattern1 = patterns[i];
                    var pattern2 = patterns[j];

                    var relationship = AnalyzePatternPairRelationship(pattern1, pattern2, featureResults);
                    
                    if (relationship.Strength >= config.MinRelationshipStrength)
                    {
                        relationships.Add(relationship);
                    }
                }
            }

            return relationships.OrderByDescending(r => r.Strength).ToList();
        }

        /// <summary>
        /// Calculate feature importance across all detected patterns
        /// </summary>
        private async Task<Dictionary<string, double>> CalculateFeatureImportanceAsync(
            ClusteringResult clusteringResults,
            SequentialPatternResult sequentialResults,
            FeatureEngineeringResult featureResults)
        {
            var importance = new Dictionary<string, double>();

            // Initialize all features with zero importance
            foreach (var featureName in featureResults.FeatureNames)
            {
                importance[featureName] = 0.0;
            }

            // Calculate importance from clustering results
            foreach (var cluster in clusteringResults.Clusters)
            {
                var clusterImportance = CalculateClusterFeatureImportance(cluster, featureResults);
                foreach (var kvp in clusterImportance)
                {
                    importance[kvp.Key] += kvp.Value * (cluster.Size / (double)clusteringResults.TotalDataPoints);
                }
            }

            // Calculate importance from sequential patterns
            foreach (var pattern in sequentialResults.Patterns)
            {
                var sequentialImportance = CalculateSequentialFeatureImportance(pattern, featureResults);
                foreach (var kvp in sequentialImportance)
                {
                    if (importance.ContainsKey(kvp.Key))
                    {
                        importance[kvp.Key] += kvp.Value * pattern.Support;
                    }
                }
            }

            // Normalize importance scores
            var maxImportance = importance.Values.Max();
            if (maxImportance > 0)
            {
                var normalizedImportance = importance.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value / maxImportance);
                return normalizedImportance;
            }

            return importance;
        }

        #region Private Helper Methods

        private ClusterCharacteristics AnalyzeClusterCharacteristics(
            Cluster cluster, FeatureEngineeringResult featureResults)
        {
            var characteristics = new ClusterCharacteristics
            {
                DominantFeatures = new List<string>(),
                OutlierFeatures = new List<string>(),
                FeatureStatistics = new Dictionary<string, FeatureStatistics>()
            };

            // Analyze each feature's contribution to the cluster
            for (int i = 0; i < cluster.Centroid.Length && i < featureResults.FeatureNames.Count; i++)
            {
                var featureName = featureResults.FeatureNames[i];
                var featureValue = cluster.Centroid[i];
                var globalMean = featureResults.FeatureStatistics[featureName].Mean;
                var globalStdDev = featureResults.FeatureStatistics[featureName].StandardDeviation;

                var zScore = Math.Abs(featureValue - globalMean) / globalStdDev;
                
                characteristics.FeatureStatistics[featureName] = new FeatureStatistics
                {
                    ClusterValue = featureValue,
                    GlobalMean = globalMean,
                    ZScore = zScore,
                    Importance = zScore / featureResults.FeatureStatistics.Values.Max(fs => Math.Abs(fs.Mean))
                };

                if (zScore > 2.0) // Feature significantly different from global mean
                {
                    characteristics.DominantFeatures.Add(featureName);
                }
                else if (zScore > 3.0) // Extreme outlier
                {
                    characteristics.OutlierFeatures.Add(featureName);
                }
            }

            return characteristics;
        }

        private string GenerateClusterDescription(Cluster cluster, ClusterCharacteristics characteristics)
        {
            var dominantFeatures = characteristics.DominantFeatures.Take(3);
            var description = $"Cluster with {cluster.Size} data points";
            
            if (dominantFeatures.Any())
            {
                description += $" characterized by high {string.Join(", ", dominantFeatures)}";
            }

            if (cluster.Cohesion > 0.8)
            {
                description += " (highly cohesive)";
            }
            else if (cluster.Cohesion < 0.4)
            {
                description += " (loosely cohesive)";
            }

            return description;
        }

        private string GenerateAnomalyDescription(Cluster cluster, ClusterCharacteristics characteristics)
        {
            var outlierFeatures = characteristics.OutlierFeatures.Take(3);
            var description = $"Anomalous cluster with {cluster.Size} data points";
            
            if (outlierFeatures.Any())
            {
                description += $" with extreme {string.Join(", ", outlierFeatures)}";
            }

            description += $" (anomaly score: {cluster.AnomalyScore:F2})";
            return description;
        }

        private double CalculateClusterConfidence(Cluster cluster, ClusteringResult clusteringResults)
        {
            // Confidence based on cluster cohesion, size, and separation from other clusters
            var sizeWeight = Math.Min(cluster.Size / (double)clusteringResults.TotalDataPoints * 10, 1.0);
            var cohesionWeight = cluster.Cohesion;
            var separationWeight = CalculateClusterSeparation(cluster, clusteringResults);

            return (sizeWeight + cohesionWeight + separationWeight) / 3.0;
        }

        private double CalculateAnomalyConfidence(Cluster cluster)
        {
            // Confidence for anomalies based on how unusual they are
            return Math.Min(cluster.AnomalyScore, 1.0);
        }

        private double CalculateClusterSeparation(Cluster cluster, ClusteringResult clusteringResults)
        {
            var minDistance = double.MaxValue;
            
            foreach (var otherCluster in clusteringResults.Clusters.Where(c => c.Id != cluster.Id))
            {
                var distance = CalculateEuclideanDistance(cluster.Centroid, otherCluster.Centroid);
                minDistance = Math.Min(minDistance, distance);
            }

            // Normalize separation score (higher is better)
            var maxPossibleDistance = Math.Sqrt(cluster.Centroid.Length) * 2; // Rough estimate
            return Math.Min(minDistance / maxPossibleDistance, 1.0);
        }

        private double CalculateEuclideanDistance(double[] point1, double[] point2)
        {
            if (point1.Length != point2.Length)
                return double.MaxValue;

            return Math.Sqrt(point1.Zip(point2, (a, b) => Math.Pow(a - b, 2)).Sum());
        }

        private string GenerateSequentialDescription(SequentialPattern pattern)
        {
            var sequence = string.Join(" → ", pattern.Sequence.Take(5));
            if (pattern.Sequence.Count > 5)
            {
                sequence += "...";
            }

            return $"Sequential pattern: {sequence} (support: {pattern.Support:P2}, confidence: {pattern.Confidence:P2})";
        }

        private List<string> ExtractSequentialFeatures(SequentialPattern pattern)
        {
            var features = new List<string>
            {
                $"sequence_length_{pattern.Sequence.Count}",
                $"support_level_{GetSupportLevel(pattern.Support)}",
                $"confidence_level_{GetConfidenceLevel(pattern.Confidence)}"
            };

            // Add features for common sequence elements
            var elementCounts = pattern.Sequence.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            foreach (var kvp in elementCounts.Take(3))
            {
                features.Add($"element_{kvp.Key}_count_{kvp.Value}");
            }

            return features;
        }

        private string GetSupportLevel(double support)
        {
            if (support > 0.5) return "high";
            if (support > 0.2) return "medium";
            return "low";
        }

        private string GetConfidenceLevel(double confidence)
        {
            if (confidence > 0.8) return "high";
            if (confidence > 0.6) return "medium";
            return "low";
        }

        private PatternRelationship AnalyzePatternPairRelationship(
            IdentifiedPattern pattern1,
            IdentifiedPattern pattern2,
            FeatureEngineeringResult featureResults)
        {
            var relationship = new PatternRelationship
            {
                Pattern1Id = pattern1.Id,
                Pattern2Id = pattern2.Id,
                RelationshipType = DetermineRelationshipType(pattern1, pattern2),
                Strength = CalculateRelationshipStrength(pattern1, pattern2),
                Description = GenerateRelationshipDescription(pattern1, pattern2)
            };

            return relationship;
        }

        private RelationshipType DetermineRelationshipType(IdentifiedPattern pattern1, IdentifiedPattern pattern2)
        {
            // Analyze feature overlap
            var commonFeatures = pattern1.Features.Intersect(pattern2.Features).Count();
            var totalFeatures = pattern1.Features.Union(pattern2.Features).Count();
            var featureOverlap = (double)commonFeatures / totalFeatures;

            // Analyze temporal relationship if both are sequential
            if (pattern1.Type == PatternType.Sequential && pattern2.Type == PatternType.Sequential)
            {
                return RelationshipType.Sequential;
            }

            // Analyze hierarchical relationship
            if (featureOverlap > 0.7)
            {
                return RelationshipType.Hierarchical;
            }

            // Analyze complementary relationship
            if (featureOverlap < 0.3 && Math.Abs(pattern1.Frequency - pattern2.Frequency) < 0.1)
            {
                return RelationshipType.Complementary;
            }

            return RelationshipType.Correlation;
        }

        private double CalculateRelationshipStrength(IdentifiedPattern pattern1, IdentifiedPattern pattern2)
        {
            var featureOverlap = CalculateFeatureOverlap(pattern1.Features, pattern2.Features);
            var frequencyCorrelation = 1.0 - Math.Abs(pattern1.Frequency - pattern2.Frequency);
            var confidenceCorrelation = 1.0 - Math.Abs(pattern1.Confidence - pattern2.Confidence);

            return (featureOverlap + frequencyCorrelation + confidenceCorrelation) / 3.0;
        }

        private double CalculateFeatureOverlap(List<string> features1, List<string> features2)
        {
            var intersection = features1.Intersect(features2).Count();
            var union = features1.Union(features2).Count();
            return union > 0 ? (double)intersection / union : 0.0;
        }

        private string GenerateRelationshipDescription(IdentifiedPattern pattern1, IdentifiedPattern pattern2)
        {
            return $"Relationship between {pattern1.Type} pattern and {pattern2.Type} pattern";
        }

        private double CalculateOverallClusterQuality(ClusteringResult clusteringResults)
        {
            if (!clusteringResults.Clusters.Any()) return 0.0;

            var avgCohesion = clusteringResults.Clusters.Average(c => c.Cohesion);
            var avgSeparation = CalculateAverageClusterSeparation(clusteringResults);
            
            return (avgCohesion + avgSeparation) / 2.0;
        }

        private double CalculateAverageClusterSeparation(ClusteringResult clusteringResults)
        {
            var separations = new List<double>();

            for (int i = 0; i < clusteringResults.Clusters.Count; i++)
            {
                for (int j = i + 1; j < clusteringResults.Clusters.Count; j++)
                {
                    var distance = CalculateEuclideanDistance(
                        clusteringResults.Clusters[i].Centroid,
                        clusteringResults.Clusters[j].Centroid);
                    separations.Add(distance);
                }
            }

            return separations.Any() ? separations.Average() : 0.0;
        }

        private int DetermineOptimalClusterCount(ClusteringResult clusteringResults)
        {
            // Use silhouette analysis or elbow method
            // For now, return current count if quality is good
            var quality = CalculateOverallClusterQuality(clusteringResults);
            
            if (quality > 0.7)
                return clusteringResults.ClusterCount;
            else if (quality < 0.4)
                return Math.Max(2, clusteringResults.ClusterCount - 1);
            else
                return clusteringResults.ClusterCount + 1;
        }

        private double CalculateTemporalComplexity(SequentialPatternResult sequentialResults)
        {
            if (!sequentialResults.Patterns.Any()) return 0.0;

            var avgLength = sequentialResults.Patterns.Average(p => p.Sequence.Count);
            var lengthVariance = sequentialResults.Patterns.Select(p => p.Sequence.Count)
                .Select(l => Math.Pow(l - avgLength, 2)).Average();
            var uniqueElements = sequentialResults.Patterns.SelectMany(p => p.Sequence).Distinct().Count();

            return (avgLength / 10.0 + Math.Sqrt(lengthVariance) / 5.0 + uniqueElements / 20.0) / 3.0;
        }

        private Dictionary<string, double> CalculateClusterFeatureImportance(
            Cluster cluster, FeatureEngineeringResult featureResults)
        {
            var importance = new Dictionary<string, double>();

            for (int i = 0; i < cluster.Centroid.Length && i < featureResults.FeatureNames.Count; i++)
            {
                var featureName = featureResults.FeatureNames[i];
                var featureValue = cluster.Centroid[i];
                var globalMean = featureResults.FeatureStatistics[featureName].Mean;
                var globalStdDev = featureResults.FeatureStatistics[featureName].StandardDeviation;

                var zScore = Math.Abs(featureValue - globalMean) / Math.Max(globalStdDev, 1e-8);
                importance[featureName] = Math.Min(zScore / 3.0, 1.0); // Normalize to [0,1]
            }

            return importance;
        }

        private Dictionary<string, double> CalculateSequentialFeatureImportance(
            SequentialPattern pattern, FeatureEngineeringResult featureResults)
        {
            var importance = new Dictionary<string, double>();

            // Sequential patterns contribute to temporal features
            foreach (var featureName in featureResults.FeatureNames.Where(f => f.Contains("temporal") || f.Contains("sequence")))
            {
                importance[featureName] = pattern.Confidence * pattern.Support;
            }

            return importance;
        }

        private PatternMetrics CalculatePatternMetrics(PatternAnalysisResult result)
        {
            return new PatternMetrics
            {
                TotalPatterns = result.IdentifiedPatterns.Count,
                TotalAnomalies = result.AnomalousPatterns.Count,
                AverageConfidence = result.IdentifiedPatterns.Any() ? 
                    result.IdentifiedPatterns.Average(p => p.Confidence) : 0.0,
                AverageFrequency = result.IdentifiedPatterns.Any() ? 
                    result.IdentifiedPatterns.Average(p => p.Frequency) : 0.0,
                PatternDiversity = CalculatePatternDiversity(result.IdentifiedPatterns),
                RelationshipDensity = result.PatternRelationships.Count / 
                    Math.Max(result.IdentifiedPatterns.Count * (result.IdentifiedPatterns.Count - 1) / 2.0, 1.0)
            };
        }

        private double CalculatePatternDiversity(List<IdentifiedPattern> patterns)
        {
            if (!patterns.Any()) return 0.0;

            var typeDistribution = patterns.GroupBy(p => p.Type)
                .Select(g => g.Count() / (double)patterns.Count)
                .ToArray();

            // Calculate Shannon entropy
            return -typeDistribution.Sum(p => p * Math.Log2(p));
        }

        private async Task<List<PatternHierarchy>> BuildPatternHierarchiesAsync(
            List<IdentifiedPattern> patterns, PatternDetectionConfig config)
        {
            var hierarchies = new List<PatternHierarchy>();

            // Group patterns by type and build hierarchies
            var patternsByType = patterns.GroupBy(p => p.Type);

            foreach (var typeGroup in patternsByType)
            {
                var hierarchy = new PatternHierarchy
                {
                    RootType = typeGroup.Key,
                    Levels = BuildHierarchyLevels(typeGroup.ToList(), config)
                };
                hierarchies.Add(hierarchy);
            }

            return hierarchies;
        }

        private List<HierarchyLevel> BuildHierarchyLevels(List<IdentifiedPattern> patterns, PatternDetectionConfig config)
        {
            var levels = new List<HierarchyLevel>();

            // Sort patterns by confidence and frequency
            var sortedPatterns = patterns.OrderByDescending(p => p.Confidence * p.Frequency).ToList();

            // Create hierarchy levels based on confidence thresholds
            var highConfidence = sortedPatterns.Where(p => p.Confidence > 0.8).ToList();
            var mediumConfidence = sortedPatterns.Where(p => p.Confidence > 0.6 && p.Confidence <= 0.8).ToList();
            var lowConfidence = sortedPatterns.Where(p => p.Confidence <= 0.6).ToList();

            if (highConfidence.Any())
            {
                levels.Add(new HierarchyLevel { Level = 1, Patterns = highConfidence, Description = "High Confidence Patterns" });
            }

            if (mediumConfidence.Any())
            {
                levels.Add(new HierarchyLevel { Level = 2, Patterns = mediumConfidence, Description = "Medium Confidence Patterns" });
            }

            if (lowConfidence.Any())
            {
                levels.Add(new HierarchyLevel { Level = 3, Patterns = lowConfidence, Description = "Low Confidence Patterns" });
            }

            return levels;
        }

        private async Task<List<PatternInsight>> GeneratePatternInsightsAsync(
            PatternAnalysisResult result, PatternDetectionConfig config)
        {
            var insights = new List<PatternInsight>();

            // Generate insights about pattern distribution
            if (result.IdentifiedPatterns.Any())
            {
                insights.Add(new PatternInsight
                {
                    Type = InsightType.Distribution,
                    Title = "Pattern Distribution Analysis",
                    Description = $"Identified {result.IdentifiedPatterns.Count} patterns across {result.IdentifiedPatterns.GroupBy(p => p.Type).Count()} different types",
                    Importance = CalculateInsightImportance(result.IdentifiedPatterns.Count, result.AnomalousPatterns.Count),
                    Recommendations = GenerateDistributionRecommendations(result)
                });
            }

            // Generate insights about feature importance
            if (result.FeatureImportance.Any())
            {
                var topFeatures = result.FeatureImportance.OrderByDescending(kvp => kvp.Value).Take(5);
                insights.Add(new PatternInsight
                {
                    Type = InsightType.FeatureImportance,
                    Title = "Key Feature Analysis",
                    Description = $"Top contributing features: {string.Join(", ", topFeatures.Select(f => f.Key))}",
                    Importance = topFeatures.First().Value,
                    Recommendations = GenerateFeatureRecommendations(topFeatures.ToList())
                });
            }

            // Generate insights about anomalies
            if (result.AnomalousPatterns.Any())
            {
                insights.Add(new PatternInsight
                {
                    Type = InsightType.Anomaly,
                    Title = "Anomaly Detection Results",
                    Description = $"Detected {result.AnomalousPatterns.Count} anomalous patterns requiring investigation",
                    Importance = result.AnomalousPatterns.Average(a => a.Confidence),
                    Recommendations = GenerateAnomalyRecommendations(result.AnomalousPatterns)
                });
            }

            return insights;
        }

        private double CalculateInsightImportance(int patternCount, int anomalyCount)
        {
            return Math.Min((patternCount + anomalyCount * 2) / 20.0, 1.0);
        }

        private List<string> GenerateDistributionRecommendations(PatternAnalysisResult result)
        {
            var recommendations = new List<string>();

            if (result.PatternMetrics.PatternDiversity < 0.5)
            {
                recommendations.Add("Pattern diversity is low - consider expanding feature engineering");
            }

            if (result.PatternMetrics.AverageConfidence < 0.6)
            {
                recommendations.Add("Average pattern confidence is low - review data quality and feature selection");
            }

            if (result.PatternRelationships.Count < result.IdentifiedPatterns.Count * 0.1)
            {
                recommendations.Add("Few pattern relationships detected - consider analyzing temporal dependencies");
            }

            return recommendations;
        }

        private List<string> GenerateFeatureRecommendations(List<KeyValuePair<string, double>> topFeatures)
        {
            var recommendations = new List<string>
            {
                $"Focus optimization efforts on top feature: {topFeatures.First().Key}",
                "Monitor these key features for changes in future analyses",
                "Consider creating derived features based on top contributors"
            };

            return recommendations;
        }

        private List<string> GenerateAnomalyRecommendations(List<IdentifiedPattern> anomalies)
        {
            var recommendations = new List<string>
            {
                "Investigate high-confidence anomalies for potential issues or opportunities",
                "Monitor anomaly frequency to detect system changes",
                "Consider adjusting detection thresholds if anomalies are expected behavior"
            };

            if (anomalies.Count > 10)
            {
                recommendations.Add("High number of anomalies detected - review data preprocessing and feature engineering");
            }

            return recommendations;
        }

        #endregion
    }
}
