using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.Statistics;
// using Accord.MachineLearning; // Commented out to avoid dependency

namespace ALARM.Analyzers.PatternDetection
{
    /// <summary>
    /// Advanced clustering algorithms implementation
    /// </summary>
    public class ClusteringAlgorithms
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<ClusteringAlgorithms> _logger;

        public ClusteringAlgorithms(MLContext mlContext, ILogger<ClusteringAlgorithms> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Perform advanced clustering using multiple algorithms
        /// </summary>
        public async Task<ClusteringResult> PerformAdvancedClusteringAsync(
            FeatureEngineeringResult featureResults,
            PatternDetectionConfig config)
        {
            _logger.LogInformation("Starting advanced clustering analysis");

            var result = new ClusteringResult
            {
                ClusteringTimestamp = DateTime.UtcNow,
                TotalDataPoints = featureResults.OriginalDataCount,
                Clusters = new List<Cluster>(),
                Algorithm = "Multi-Algorithm Ensemble"
            };

            try
            {
                // Try K-means clustering first
                var kmeansResult = await PerformKMeansClusteringAsync(featureResults, config);
                if (kmeansResult.OverallQuality > 0.5)
                {
                    result = kmeansResult;
                    result.Algorithm = "K-Means";
                }

                // Try DBSCAN if K-means quality is low
                if (result.OverallQuality < 0.6)
                {
                    var dbscanResult = await PerformDBSCANClusteringAsync(featureResults, config);
                    if (dbscanResult.OverallQuality > result.OverallQuality)
                    {
                        result = dbscanResult;
                        result.Algorithm = "DBSCAN";
                    }
                }

                // Try hierarchical clustering as fallback
                if (result.OverallQuality < 0.5)
                {
                    var hierarchicalResult = await PerformHierarchicalClusteringAsync(featureResults, config);
                    if (hierarchicalResult.OverallQuality > result.OverallQuality)
                    {
                        result = hierarchicalResult;
                        result.Algorithm = "Hierarchical";
                    }
                }

                _logger.LogInformation("Clustering completed using {Algorithm} with {ClusterCount} clusters and {Quality:F2} quality",
                    result.Algorithm, result.ClusterCount, result.OverallQuality);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during clustering analysis");
                throw;
            }
        }

        /// <summary>
        /// K-Means clustering implementation
        /// </summary>
        private async Task<ClusteringResult> PerformKMeansClusteringAsync(
            FeatureEngineeringResult featureResults,
            PatternDetectionConfig config)
        {
            var result = new ClusteringResult
            {
                ClusteringTimestamp = DateTime.UtcNow,
                TotalDataPoints = featureResults.OriginalDataCount,
                Algorithm = "K-Means"
            };

            if (featureResults.FeatureMatrix == null || !featureResults.FeatureMatrix.Any())
            {
                return result;
            }

            // Determine optimal number of clusters using elbow method
            var optimalK = DetermineOptimalKMeansK(featureResults.FeatureMatrix, config.MaxClusterCount);
            
            // Perform K-means clustering using simplified implementation
            var (centroids, labels) = PerformSimpleKMeans(featureResults.FeatureMatrix, optimalK);

            // Convert results to our format
            result.ClusterCount = optimalK;
            result.Clusters = new List<Cluster>();

            for (int i = 0; i < optimalK; i++)
            {
                var clusterIndices = labels.Select((label, index) => new { label, index })
                                          .Where(x => x.label == i)
                                          .Select(x => x.index)
                                          .ToList();

                if (!clusterIndices.Any()) continue;

                var cluster = new Cluster
                {
                    Id = $"kmeans_cluster_{i}",
                    Size = clusterIndices.Count,
                    Centroid = centroids[i],
                    DataPointIndices = clusterIndices,
                    Cohesion = CalculateClusterCohesion(featureResults.FeatureMatrix, clusterIndices, centroids[i]),
                    AnomalyScore = CalculateAnomalyScore(clusterIndices.Count, featureResults.OriginalDataCount),
                    MaxDistanceFromCentroid = CalculateMaxDistanceFromCentroid(featureResults.FeatureMatrix, clusterIndices, centroids[i])
                };

                result.Clusters.Add(cluster);
            }

            result.OverallQuality = CalculateOverallClusteringQuality(result);
            return result;
        }

        /// <summary>
        /// DBSCAN clustering implementation
        /// </summary>
        private async Task<ClusteringResult> PerformDBSCANClusteringAsync(
            FeatureEngineeringResult featureResults,
            PatternDetectionConfig config)
        {
            var result = new ClusteringResult
            {
                ClusteringTimestamp = DateTime.UtcNow,
                TotalDataPoints = featureResults.OriginalDataCount,
                Algorithm = "DBSCAN"
            };

            if (featureResults.FeatureMatrix == null || !featureResults.FeatureMatrix.Any())
            {
                return result;
            }

            // Estimate epsilon using k-distance graph
            var epsilon = EstimateDBSCANEpsilon(featureResults.FeatureMatrix);
            var minPoints = Math.Max(3, featureResults.FeatureMatrix[0].Length); // Minimum points based on dimensionality

            // Perform DBSCAN clustering using simplified implementation
            var (clusterLists, noisePoints) = PerformSimpleDBSCAN(featureResults.FeatureMatrix, epsilon, minPoints);

            // Convert results to our format
            result.ClusterCount = clusterLists.Count;
            result.Clusters = new List<Cluster>();

            for (int i = 0; i < clusterLists.Count; i++)
            {
                var clusterIndices = clusterLists[i];

                if (!clusterIndices.Any()) continue;

                var centroid = CalculateCentroid(featureResults.FeatureMatrix, clusterIndices);
                
                var cluster = new Cluster
                {
                    Id = $"dbscan_cluster_{i}",
                    Size = clusterIndices.Count,
                    Centroid = centroid,
                    DataPointIndices = clusterIndices,
                    Cohesion = CalculateClusterCohesion(featureResults.FeatureMatrix, clusterIndices, centroid),
                    AnomalyScore = CalculateAnomalyScore(clusterIndices.Count, featureResults.OriginalDataCount),
                    MaxDistanceFromCentroid = CalculateMaxDistanceFromCentroid(featureResults.FeatureMatrix, clusterIndices, centroid)
                };

                result.Clusters.Add(cluster);
            }

            // Handle noise points as potential anomalies
            if (noisePoints.Any())
            {
                var noiseCluster = new Cluster
                {
                    Id = "dbscan_noise",
                    Size = noisePoints.Count,
                    Centroid = CalculateCentroid(featureResults.FeatureMatrix, noisePoints),
                    DataPointIndices = noisePoints,
                    Cohesion = 0.0, // Noise points have no cohesion
                    AnomalyScore = 1.0, // High anomaly score for noise
                    MaxDistanceFromCentroid = double.MaxValue
                };
                result.Clusters.Add(noiseCluster);
            }

            result.OverallQuality = CalculateOverallClusteringQuality(result);
            return result;
        }

        /// <summary>
        /// Hierarchical clustering implementation
        /// </summary>
        private async Task<ClusteringResult> PerformHierarchicalClusteringAsync(
            FeatureEngineeringResult featureResults,
            PatternDetectionConfig config)
        {
            var result = new ClusteringResult
            {
                ClusteringTimestamp = DateTime.UtcNow,
                TotalDataPoints = featureResults.OriginalDataCount,
                Algorithm = "Hierarchical"
            };

            if (featureResults.FeatureMatrix == null || !featureResults.FeatureMatrix.Any())
            {
                return result;
            }

            // Use simple hierarchical clustering approach
            var optimalK = Math.Min(config.MaxClusterCount, featureResults.OriginalDataCount / 5);
            
            // For simplicity, use K-means as a fallback for hierarchical
            // In a full implementation, you would use proper hierarchical clustering
            var (centroids, labels) = PerformSimpleKMeans(featureResults.FeatureMatrix, optimalK);

            result.ClusterCount = optimalK;
            result.Clusters = new List<Cluster>();

            for (int i = 0; i < optimalK; i++)
            {
                var clusterIndices = labels.Select((label, index) => new { label, index })
                                          .Where(x => x.label == i)
                                          .Select(x => x.index)
                                          .ToList();

                if (!clusterIndices.Any()) continue;

                var cluster = new Cluster
                {
                    Id = $"hierarchical_cluster_{i}",
                    Size = clusterIndices.Count,
                    Centroid = centroids[i],
                    DataPointIndices = clusterIndices,
                    Cohesion = CalculateClusterCohesion(featureResults.FeatureMatrix, clusterIndices, centroids[i]),
                    AnomalyScore = CalculateAnomalyScore(clusterIndices.Count, featureResults.OriginalDataCount),
                    MaxDistanceFromCentroid = CalculateMaxDistanceFromCentroid(featureResults.FeatureMatrix, clusterIndices, centroids[i])
                };

                result.Clusters.Add(cluster);
            }

            result.OverallQuality = CalculateOverallClusteringQuality(result);
            return result;
        }

        #region Simplified Clustering Implementations

        /// <summary>
        /// Simplified K-means implementation without external dependencies
        /// </summary>
        private (double[][] centroids, int[] labels) PerformSimpleKMeans(double[][] data, int k)
        {
            var random = new Random(42);
            var dimensions = data[0].Length;
            var centroids = new double[k][];
            var labels = new int[data.Length];
            
            // Initialize centroids randomly
            for (int i = 0; i < k; i++)
            {
                centroids[i] = new double[dimensions];
                var randomPoint = data[random.Next(data.Length)];
                Array.Copy(randomPoint, centroids[i], dimensions);
            }
            
            // K-means iterations
            for (int iteration = 0; iteration < 100; iteration++) // Max 100 iterations
            {
                var changed = false;
                
                // Assign points to nearest centroid
                for (int i = 0; i < data.Length; i++)
                {
                    var nearestCentroid = 0;
                    var minDistance = CalculateEuclideanDistance(data[i], centroids[0]);
                    
                    for (int j = 1; j < k; j++)
                    {
                        var distance = CalculateEuclideanDistance(data[i], centroids[j]);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestCentroid = j;
                        }
                    }
                    
                    if (labels[i] != nearestCentroid)
                    {
                        labels[i] = nearestCentroid;
                        changed = true;
                    }
                }
                
                if (!changed) break;
                
                // Update centroids
                for (int i = 0; i < k; i++)
                {
                    var clusterPoints = data.Where((point, index) => labels[index] == i).ToArray();
                    if (clusterPoints.Any())
                    {
                        for (int d = 0; d < dimensions; d++)
                        {
                            centroids[i][d] = clusterPoints.Average(p => p[d]);
                        }
                    }
                }
            }
            
            return (centroids, labels);
        }

        /// <summary>
        /// Simplified DBSCAN implementation
        /// </summary>
        private (List<List<int>> clusters, List<int> noise) PerformSimpleDBSCAN(double[][] data, double epsilon, int minPoints)
        {
            var clusters = new List<List<int>>();
            var visited = new bool[data.Length];
            var noise = new List<int>();
            
            for (int i = 0; i < data.Length; i++)
            {
                if (visited[i]) continue;
                visited[i] = true;
                
                var neighbors = GetNeighbors(data, i, epsilon);
                
                if (neighbors.Count < minPoints)
                {
                    noise.Add(i);
                }
                else
                {
                    var cluster = new List<int>();
                    ExpandCluster(data, i, neighbors, cluster, visited, epsilon, minPoints);
                    clusters.Add(cluster);
                }
            }
            
            return (clusters, noise);
        }
        
        private List<int> GetNeighbors(double[][] data, int pointIndex, double epsilon)
        {
            var neighbors = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (CalculateEuclideanDistance(data[pointIndex], data[i]) <= epsilon)
                {
                    neighbors.Add(i);
                }
            }
            return neighbors;
        }
        
        private void ExpandCluster(double[][] data, int pointIndex, List<int> neighbors, List<int> cluster, 
            bool[] visited, double epsilon, int minPoints)
        {
            cluster.Add(pointIndex);
            
            for (int i = 0; i < neighbors.Count; i++)
            {
                var neighborIndex = neighbors[i];
                
                if (!visited[neighborIndex])
                {
                    visited[neighborIndex] = true;
                    var neighborNeighbors = GetNeighbors(data, neighborIndex, epsilon);
                    
                    if (neighborNeighbors.Count >= minPoints)
                    {
                        neighbors.AddRange(neighborNeighbors.Where(n => !neighbors.Contains(n)));
                    }
                }
                
                if (!cluster.Contains(neighborIndex))
                {
                    cluster.Add(neighborIndex);
                }
            }
        }

        #endregion

        #region Helper Methods

        private int DetermineOptimalKMeansK(double[][] data, int maxK)
        {
            var wcss = new List<double>(); // Within-cluster sum of squares
            
            for (int k = 1; k <= Math.Min(maxK, data.Length / 2); k++)
            {
                try
                {
                    var (centroids, labels) = PerformSimpleKMeans(data, k);
                    
                    var totalWCSS = 0.0;
                    for (int i = 0; i < k; i++)
                    {
                        var clusterPoints = data.Where((point, index) => labels[index] == i).ToArray();
                        if (clusterPoints.Any())
                        {
                            var centroid = centroids[i];
                            totalWCSS += clusterPoints.Sum(point => CalculateEuclideanDistance(point, centroid));
                        }
                    }
                    wcss.Add(totalWCSS);
                }
                catch
                {
                    wcss.Add(double.MaxValue);
                }
            }

            // Use elbow method to find optimal K
            return FindElbowPoint(wcss) + 1; // +1 because we start from k=1
        }

        private int FindElbowPoint(List<double> wcss)
        {
            if (wcss.Count < 3) return 0;

            var maxImprovement = 0.0;
            var elbowPoint = 0;

            for (int i = 1; i < wcss.Count - 1; i++)
            {
                var improvement = wcss[i - 1] - wcss[i];
                var nextImprovement = wcss[i] - wcss[i + 1];
                var improvementRatio = improvement / Math.Max(nextImprovement, 1e-8);

                if (improvementRatio > maxImprovement)
                {
                    maxImprovement = improvementRatio;
                    elbowPoint = i;
                }
            }

            return elbowPoint;
        }

        private double EstimateDBSCANEpsilon(double[][] data)
        {
            // Calculate k-distance graph for k=4 (common choice)
            var k = 4;
            var distances = new List<double>();

            foreach (var point in data)
            {
                var pointDistances = data.Select(otherPoint => CalculateEuclideanDistance(point, otherPoint))
                                        .OrderBy(d => d)
                                        .Skip(1) // Skip self-distance (0)
                                        .Take(k)
                                        .ToArray();
                
                if (pointDistances.Length >= k)
                {
                    distances.Add(pointDistances[k - 1]); // k-th nearest neighbor distance
                }
            }

            // Use knee point detection or percentile
            distances.Sort();
            var percentileIndex = (int)(distances.Count * 0.95); // 95th percentile
            return distances[Math.Min(percentileIndex, distances.Count - 1)];
        }

        private double[] CalculateCentroid(double[][] data, List<int> indices)
        {
            if (!indices.Any()) return new double[0];

            var dimensions = data[0].Length;
            var centroid = new double[dimensions];

            foreach (var index in indices)
            {
                for (int d = 0; d < dimensions; d++)
                {
                    centroid[d] += data[index][d];
                }
            }

            for (int d = 0; d < dimensions; d++)
            {
                centroid[d] /= indices.Count;
            }

            return centroid;
        }

        private double CalculateClusterCohesion(double[][] data, List<int> indices, double[] centroid)
        {
            if (!indices.Any()) return 0.0;

            var totalDistance = indices.Sum(index => CalculateEuclideanDistance(data[index], centroid));
            var avgDistance = totalDistance / indices.Count;
            
            // Normalize cohesion (higher is better, so invert distance)
            var maxPossibleDistance = Math.Sqrt(centroid.Length) * 2; // Rough estimate
            return Math.Max(0, 1.0 - (avgDistance / maxPossibleDistance));
        }

        private double CalculateAnomalyScore(int clusterSize, int totalDataPoints)
        {
            var expectedSize = totalDataPoints / 5.0; // Assume 5 clusters on average
            var sizeDifference = Math.Abs(clusterSize - expectedSize) / expectedSize;
            
            // Smaller clusters have higher anomaly scores
            if (clusterSize < expectedSize * 0.1) // Very small cluster
            {
                return Math.Min(1.0, sizeDifference);
            }
            
            return Math.Min(0.5, sizeDifference * 0.5);
        }

        private double CalculateMaxDistanceFromCentroid(double[][] data, List<int> indices, double[] centroid)
        {
            if (!indices.Any()) return 0.0;

            return indices.Max(index => CalculateEuclideanDistance(data[index], centroid));
        }

        private double CalculateEuclideanDistance(double[] point1, double[] point2)
        {
            if (point1.Length != point2.Length) return double.MaxValue;

            return Math.Sqrt(point1.Zip(point2, (a, b) => Math.Pow(a - b, 2)).Sum());
        }

        private double CalculateOverallClusteringQuality(ClusteringResult result)
        {
            if (!result.Clusters.Any()) return 0.0;

            // Quality based on average cohesion and cluster size distribution
            var avgCohesion = result.Clusters.Average(c => c.Cohesion);
            var sizeVariance = CalculateClusterSizeVariance(result.Clusters);
            var sizeBalance = Math.Max(0, 1.0 - sizeVariance); // Lower variance is better

            return (avgCohesion + sizeBalance) / 2.0;
        }

        private double CalculateClusterSizeVariance(List<Cluster> clusters)
        {
            if (clusters.Count < 2) return 0.0;

            var sizes = clusters.Select(c => (double)c.Size).ToArray();
            var mean = sizes.Average();
            var variance = sizes.Select(s => Math.Pow(s - mean, 2)).Average();
            
            // Normalize by mean to get coefficient of variation
            return Math.Sqrt(variance) / Math.Max(mean, 1.0);
        }

        #endregion
    }
}
