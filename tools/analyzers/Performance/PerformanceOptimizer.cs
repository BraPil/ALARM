using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ALARM.Analyzers.PatternDetection;
using ALARM.Analyzers.CausalAnalysis;

namespace ALARM.Analyzers.Performance
{
    /// <summary>
    /// Advanced performance optimization engine for ALARM system
    /// Provides adaptive threshold management and real-time performance tuning
    /// </summary>
    public class PerformanceOptimizer
    {
        private readonly ILogger<PerformanceOptimizer> _logger;
        private readonly PerformanceConfig _config;
        private readonly PerformanceMonitor _monitor;
        private readonly Dictionary<string, object> _performanceCache;
        private readonly Dictionary<string, DateTime> _cacheTimestamps;

        public PerformanceOptimizer(ILogger<PerformanceOptimizer> logger, PerformanceConfig? config = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? new PerformanceConfig();
            
            // Create a logger for the monitor using logger factory
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var monitorLogger = loggerFactory.CreateLogger<PerformanceMonitor>();
            _monitor = new PerformanceMonitor(monitorLogger);
            _performanceCache = new Dictionary<string, object>();
            _cacheTimestamps = new Dictionary<string, DateTime>();
        }

        /// <summary>
        /// Optimize pattern detection configuration based on current context
        /// </summary>
        public PatternDetectionConfig OptimizePatternDetectionConfig(
            PatternDetectionConfig baseConfig, 
            PerformanceTuningContext context)
        {
            _logger.LogInformation("Optimizing pattern detection configuration for {DataPoints} data points", 
                context.DataPointCount);

            var optimizedConfig = new PatternDetectionConfig
            {
                // Base values from original config
                MinCohesionForPattern = baseConfig.MinCohesionForPattern,
                AnomalyThreshold = baseConfig.AnomalyThreshold,
                MinSupportForSequentialPattern = baseConfig.MinSupportForSequentialPattern,
                MinConfidenceForSequentialPattern = baseConfig.MinConfidenceForSequentialPattern,
                HighConfidenceThreshold = baseConfig.HighConfidenceThreshold,
                FeatureImportanceThreshold = baseConfig.FeatureImportanceThreshold,
                FeatureCorrelationThreshold = baseConfig.FeatureCorrelationThreshold,
                SignificantChangeThreshold = baseConfig.SignificantChangeThreshold,
                MinRelationshipStrength = baseConfig.MinRelationshipStrength,
                ForecastWindowSize = baseConfig.ForecastWindowSize,
                ForecastSeriesLength = baseConfig.ForecastSeriesLength,
                PredictionHorizon = baseConfig.PredictionHorizon
            };

            // Adaptive cluster count optimization
            if (_config.Adaptive.EnableAdaptiveClustering)
            {
                optimizedConfig.MaxClusterCount = CalculateOptimalClusterCount(context);
                optimizedConfig.MinClusterSizeForPattern = CalculateOptimalMinClusterSize(context);
            }
            else
            {
                optimizedConfig.MaxClusterCount = baseConfig.MaxClusterCount;
                optimizedConfig.MinClusterSizeForPattern = baseConfig.MinClusterSizeForPattern;
            }

            // Adaptive window sizing
            if (_config.Adaptive.EnableAdaptiveWindowSizing)
            {
                optimizedConfig.StreamingWindowSize = CalculateOptimalWindowSize(context);
                optimizedConfig.MinWindowSizeForDetection = Math.Max(10, optimizedConfig.StreamingWindowSize / 10);
            }
            else
            {
                optimizedConfig.StreamingWindowSize = baseConfig.StreamingWindowSize;
                optimizedConfig.MinWindowSizeForDetection = baseConfig.MinWindowSizeForDetection;
            }

            // Sequence length optimization based on performance strategy
            optimizedConfig.MaxSequenceLength = context.Strategy switch
            {
                OptimizationStrategy.Speed => Math.Min(baseConfig.MaxSequenceLength, 5),
                OptimizationStrategy.Accuracy => Math.Max(baseConfig.MaxSequenceLength, 15),
                OptimizationStrategy.MemoryOptimized => Math.Min(baseConfig.MaxSequenceLength, 8),
                _ => baseConfig.MaxSequenceLength
            };

            _logger.LogDebug("Pattern detection config optimized: MaxClusters={MaxClusters}, WindowSize={WindowSize}, MaxSequence={MaxSequence}",
                optimizedConfig.MaxClusterCount, optimizedConfig.StreamingWindowSize, optimizedConfig.MaxSequenceLength);

            return optimizedConfig;
        }

        /// <summary>
        /// Optimize causal analysis configuration based on current context
        /// </summary>
        public CausalAnalysisConfig OptimizeCausalAnalysisConfig(
            CausalAnalysisConfig baseConfig, 
            PerformanceTuningContext context)
        {
            _logger.LogInformation("Optimizing causal analysis configuration for {Variables} variables", 
                context.VariableCount);

            var optimizedConfig = new CausalAnalysisConfig
            {
                // Base values from original config
                PCAlgorithmAlpha = baseConfig.PCAlgorithmAlpha,
                GrangerSignificanceLevel = baseConfig.GrangerSignificanceLevel,
                TransferEntropyThreshold = baseConfig.TransferEntropyThreshold,
                MinCausalStrength = baseConfig.MinCausalStrength,
                CausalValidationThreshold = baseConfig.CausalValidationThreshold,
                CausalStabilityThreshold = baseConfig.CausalStabilityThreshold,
                InterventionEffectThreshold = baseConfig.InterventionEffectThreshold,
                MinInterventionSamples = baseConfig.MinInterventionSamples
            };

            // Adaptive lag selection
            if (_config.Adaptive.EnableAdaptiveLagSelection)
            {
                optimizedConfig.MaxLagForGranger = CalculateOptimalMaxLag(context);
                optimizedConfig.MinDataPointsForGranger = CalculateOptimalMinDataPoints(context);
            }
            else
            {
                optimizedConfig.MaxLagForGranger = baseConfig.MaxLagForGranger;
                optimizedConfig.MinDataPointsForGranger = baseConfig.MinDataPointsForGranger;
            }

            // Adaptive iteration limits
            if (_config.Adaptive.EnableAdaptiveIterations)
            {
                optimizedConfig.MaxSEMIterations = CalculateOptimalMaxIterations(context);
                optimizedConfig.SEMConvergenceThreshold = CalculateOptimalConvergenceThreshold(context);
            }
            else
            {
                optimizedConfig.MaxSEMIterations = baseConfig.MaxSEMIterations;
                optimizedConfig.SEMConvergenceThreshold = baseConfig.SEMConvergenceThreshold;
            }

            // Temporal window optimization
            optimizedConfig.TemporalWindowSize = context.Strategy switch
            {
                OptimizationStrategy.Speed => Math.Min(baseConfig.TemporalWindowSize, 30),
                OptimizationStrategy.Accuracy => Math.Max(baseConfig.TemporalWindowSize, 100),
                OptimizationStrategy.MemoryOptimized => Math.Min(baseConfig.TemporalWindowSize, 25),
                _ => baseConfig.TemporalWindowSize
            };

            _logger.LogDebug("Causal analysis config optimized: MaxLag={MaxLag}, MaxIterations={MaxIterations}, WindowSize={WindowSize}",
                optimizedConfig.MaxLagForGranger, optimizedConfig.MaxSEMIterations, optimizedConfig.TemporalWindowSize);

            return optimizedConfig;
        }

        /// <summary>
        /// Monitor and adjust performance during execution
        /// </summary>
        public async Task<PerformanceAdjustmentResult> MonitorAndAdjustAsync(
            string operationName, 
            Func<Task<object>> operation,
            PerformanceTuningContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var initialMemory = GC.GetTotalMemory(false);
            
            _logger.LogInformation("Starting monitored operation: {OperationName}", operationName);

            try
            {
                // Start performance monitoring
                var monitoringTask = _monitor.StartMonitoringAsync(operationName, context);

                // Execute the operation
                var result = await operation();

                // Stop monitoring
                await _monitor.StopMonitoringAsync(operationName);

                stopwatch.Stop();
                var finalMemory = GC.GetTotalMemory(false);
                var memoryUsed = (finalMemory - initialMemory) / (1024.0 * 1024.0); // MB

                var adjustmentResult = new PerformanceAdjustmentResult
                {
                    OperationName = operationName,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                    MemoryUsedMB = memoryUsed,
                    Result = result,
                    Success = true,
                    PerformanceMetrics = await _monitor.GetMetricsAsync(operationName),
                    Recommendations = GeneratePerformanceRecommendations(
                        operationName, stopwatch.ElapsedMilliseconds, memoryUsed, context)
                };

                // Cache successful results if caching is enabled
                if (ShouldCacheResult(operationName, adjustmentResult))
                {
                    CacheResult(operationName, result, context);
                }

                _logger.LogInformation("Operation {OperationName} completed in {ElapsedMs}ms using {MemoryMB:F2}MB",
                    operationName, stopwatch.ElapsedMilliseconds, memoryUsed);

                return adjustmentResult;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _monitor.StopMonitoringAsync(operationName);

                _logger.LogError(ex, "Operation {OperationName} failed after {ElapsedMs}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);

                return new PerformanceAdjustmentResult
                {
                    OperationName = operationName,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                    Success = false,
                    Error = ex.Message,
                    Recommendations = new List<string> { "Consider reducing data size or complexity", "Check for resource constraints" }
                };
            }
        }

        /// <summary>
        /// Get cached result if available and valid
        /// </summary>
        public T? GetCachedResult<T>(string cacheKey, PerformanceTuningContext context) where T : class
        {
            if (!IsCachingEnabled()) return null;

            var fullKey = GenerateCacheKey(cacheKey, context);
            
            if (_performanceCache.TryGetValue(fullKey, out var cachedResult) && 
                _cacheTimestamps.TryGetValue(fullKey, out var timestamp))
            {
                if (IsCacheValid(cacheKey, timestamp))
                {
                    _logger.LogDebug("Cache hit for key: {CacheKey}", fullKey);
                    return cachedResult as T;
                }
                else
                {
                    // Remove expired cache entry
                    _performanceCache.Remove(fullKey);
                    _cacheTimestamps.Remove(fullKey);
                    _logger.LogDebug("Cache expired for key: {CacheKey}", fullKey);
                }
            }

            return null;
        }

        #region Private Helper Methods

        private int CalculateOptimalClusterCount(PerformanceTuningContext context)
        {
            var baseCount = Math.Max(_config.Adaptive.MinClusterCount,
                (int)(context.DataPointCount * _config.Adaptive.ClusterCountScalingFactor));
            
            return Math.Min(baseCount, _config.Adaptive.MaxClusterCount);
        }

        private int CalculateOptimalMinClusterSize(PerformanceTuningContext context)
        {
            // Minimum cluster size should be proportional to total data points
            var minSize = Math.Max(3, context.DataPointCount / 100);
            return Math.Min(minSize, 20); // Cap at 20
        }

        private int CalculateOptimalWindowSize(PerformanceTuningContext context)
        {
            var baseSize = Math.Max(_config.Adaptive.MinStreamingWindowSize,
                (int)(context.DataPointCount * _config.Adaptive.WindowSizeScalingFactor));
            
            return Math.Min(baseSize, _config.Adaptive.MaxStreamingWindowSize);
        }

        private int CalculateOptimalMaxLag(PerformanceTuningContext context)
        {
            // Adjust lag based on data complexity and available resources
            var baseLag = context.Strategy switch
            {
                OptimizationStrategy.Speed => _config.Adaptive.MinLagForGranger,
                OptimizationStrategy.Accuracy => _config.Adaptive.MaxLagForGranger,
                _ => (_config.Adaptive.MinLagForGranger + _config.Adaptive.MaxLagForGranger) / 2
            };

            // Adjust based on memory availability
            if (context.AvailableMemoryMB < 512) // Less than 512MB available
            {
                baseLag = Math.Min(baseLag, 3);
            }

            return baseLag;
        }

        private int CalculateOptimalMinDataPoints(PerformanceTuningContext context)
        {
            // Ensure we have enough data points for reliable analysis
            return Math.Max(20, context.DataPointCount / 10);
        }

        private int CalculateOptimalMaxIterations(PerformanceTuningContext context)
        {
            return context.Strategy switch
            {
                OptimizationStrategy.Speed => _config.Adaptive.MinSEMIterations,
                OptimizationStrategy.Accuracy => _config.Adaptive.MaxSEMIterations,
                OptimizationStrategy.MemoryOptimized => Math.Min(_config.Adaptive.MaxSEMIterations, 50),
                _ => (_config.Adaptive.MinSEMIterations + _config.Adaptive.MaxSEMIterations) / 2
            };
        }

        private double CalculateOptimalConvergenceThreshold(PerformanceTuningContext context)
        {
            return context.Strategy switch
            {
                OptimizationStrategy.Speed => _config.Adaptive.ConvergenceThreshold * 10, // Looser convergence
                OptimizationStrategy.Accuracy => _config.Adaptive.ConvergenceThreshold / 10, // Tighter convergence
                _ => _config.Adaptive.ConvergenceThreshold
            };
        }

        private List<string> GeneratePerformanceRecommendations(
            string operationName, 
            long executionTimeMs, 
            double memoryUsedMB, 
            PerformanceTuningContext context)
        {
            var recommendations = new List<string>();

            // Time-based recommendations
            if (executionTimeMs > _config.Monitoring.TargetPatternDetectionTime * 1000)
            {
                recommendations.Add("Consider reducing data complexity or using speed-optimized strategy");
                recommendations.Add("Enable parallel processing if not already active");
            }

            // Memory-based recommendations
            if (memoryUsedMB > _config.Resources.MemoryWarningThresholdMB)
            {
                recommendations.Add("Consider processing data in smaller batches");
                recommendations.Add("Enable streaming processing for large datasets");
            }

            // Strategy-specific recommendations
            if (context.Strategy == OptimizationStrategy.Balanced && executionTimeMs > 10000) // 10 seconds
            {
                recommendations.Add("Consider switching to Speed optimization strategy");
            }

            return recommendations;
        }

        private bool ShouldCacheResult(string operationName, PerformanceAdjustmentResult result)
        {
            if (!IsCachingEnabled()) return false;
            if (!result.Success) return false;
            
            // Cache successful results that took significant time
            return result.ExecutionTimeMs > 1000; // Cache results that took more than 1 second
        }

        private void CacheResult(string operationName, object result, PerformanceTuningContext context)
        {
            var cacheKey = GenerateCacheKey(operationName, context);
            
            // Implement LRU cache eviction if needed
            if (_performanceCache.Count >= GetMaxCacheSize(operationName))
            {
                EvictOldestCacheEntry();
            }

            _performanceCache[cacheKey] = result;
            _cacheTimestamps[cacheKey] = DateTime.UtcNow;
            
            _logger.LogDebug("Cached result for operation: {OperationName}", operationName);
        }

        private string GenerateCacheKey(string operationName, PerformanceTuningContext context)
        {
            if (_config.Caching.UseDataHashForCacheKeys)
            {
                var keyComponents = $"{operationName}_{context.DataPointCount}_{context.VariableCount}_{context.Strategy}";
                return keyComponents.GetHashCode().ToString();
            }
            
            return $"{operationName}_{context.DataPointCount}_{context.VariableCount}";
        }

        private bool IsCachingEnabled()
        {
            return _config.Caching.EnablePatternResultCaching || 
                   _config.Caching.EnableCausalAnalysisCaching ||
                   _config.Caching.EnableFeatureExtractionCaching;
        }

        private bool IsCacheValid(string operationName, DateTime timestamp)
        {
            var expirationMinutes = operationName.ToLower() switch
            {
                var op when op.Contains("pattern") => _config.Caching.PatternResultCacheExpiration,
                var op when op.Contains("causal") => _config.Caching.CausalResultCacheExpiration,
                var op when op.Contains("feature") => _config.Caching.FeatureCacheExpiration,
                _ => _config.Caching.PatternResultCacheExpiration
            };

            return DateTime.UtcNow - timestamp < TimeSpan.FromMinutes(expirationMinutes);
        }

        private int GetMaxCacheSize(string operationName)
        {
            return operationName.ToLower() switch
            {
                var op when op.Contains("pattern") => _config.Caching.MaxPatternResultCacheSize,
                var op when op.Contains("causal") => _config.Caching.MaxCausalResultCacheSize,
                var op when op.Contains("feature") => _config.Caching.MaxFeatureCacheSize,
                _ => _config.Caching.MaxPatternResultCacheSize
            };
        }

        private void EvictOldestCacheEntry()
        {
            var oldestKey = "";
            var oldestTime = DateTime.MaxValue;

            foreach (var kvp in _cacheTimestamps)
            {
                if (kvp.Value < oldestTime)
                {
                    oldestTime = kvp.Value;
                    oldestKey = kvp.Key;
                }
            }

            if (!string.IsNullOrEmpty(oldestKey))
            {
                _performanceCache.Remove(oldestKey);
                _cacheTimestamps.Remove(oldestKey);
                _logger.LogDebug("Evicted oldest cache entry: {CacheKey}", oldestKey);
            }
        }

        #endregion
    }

    /// <summary>
    /// Result of performance monitoring and adjustment
    /// </summary>
    public class PerformanceAdjustmentResult
    {
        public string OperationName { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
        public double MemoryUsedMB { get; set; }
        public bool Success { get; set; }
        public object? Result { get; set; }
        public string? Error { get; set; }
        public Dictionary<string, double>? PerformanceMetrics { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }
}
