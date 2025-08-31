using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ALARM.Analyzers.Performance
{
    /// <summary>
    /// Real-time performance monitoring system for ALARM operations
    /// Tracks CPU, memory, and execution metrics
    /// </summary>
    public class PerformanceMonitor
    {
        private readonly ILogger<PerformanceMonitor> _logger;
        private readonly ConcurrentDictionary<string, PerformanceSession> _activeSessions;
        private readonly ConcurrentDictionary<string, PerformanceHistory> _performanceHistory;
        private readonly Timer _resourceMonitorTimer;
        private readonly PerformanceCounter? _cpuCounter;
        private readonly PerformanceCounter? _memoryCounter;

        public PerformanceMonitor(ILogger<PerformanceMonitor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activeSessions = new ConcurrentDictionary<string, PerformanceSession>();
            _performanceHistory = new ConcurrentDictionary<string, PerformanceHistory>();

            // Initialize performance counters (Windows-specific)
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
                
                // Start resource monitoring timer
                _resourceMonitorTimer = new Timer(MonitorSystemResources, null, 
                    TimeSpan.Zero, TimeSpan.FromSeconds(10));
                
                _logger.LogInformation("Performance monitoring initialized with system counters");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not initialize system performance counters. Monitoring will use basic metrics only.");
                
                // Fallback timer for basic monitoring
                _resourceMonitorTimer = new Timer(MonitorSystemResourcesBasic, null,
                    TimeSpan.Zero, TimeSpan.FromSeconds(10));
            }
        }

        /// <summary>
        /// Start monitoring a specific operation
        /// </summary>
        public async Task<string> StartMonitoringAsync(string operationName, PerformanceTuningContext context)
        {
            var sessionId = $"{operationName}_{Guid.NewGuid():N}";
            
            var session = new PerformanceSession
            {
                SessionId = sessionId,
                OperationName = operationName,
                StartTime = DateTime.UtcNow,
                StartMemoryBytes = GC.GetTotalMemory(false),
                Context = context,
                Metrics = new Dictionary<string, double>()
            };

            _activeSessions[sessionId] = session;
            
            _logger.LogDebug("Started monitoring session {SessionId} for operation {OperationName}", 
                sessionId, operationName);

            return sessionId;
        }

        /// <summary>
        /// Stop monitoring and collect final metrics
        /// </summary>
        public async Task<PerformanceMetrics> StopMonitoringAsync(string operationName)
        {
            var sessionsToStop = new List<PerformanceSession>();
            
            // Find sessions for this operation
            foreach (var kvp in _activeSessions)
            {
                if (kvp.Value.OperationName == operationName)
                {
                    sessionsToStop.Add(kvp.Value);
                }
            }

            var aggregatedMetrics = new PerformanceMetrics
            {
                OperationName = operationName,
                SessionCount = sessionsToStop.Count,
                Timestamp = DateTime.UtcNow
            };

            foreach (var session in sessionsToStop)
            {
                var endTime = DateTime.UtcNow;
                var endMemory = GC.GetTotalMemory(false);
                
                session.EndTime = endTime;
                session.EndMemoryBytes = endMemory;
                session.ExecutionTimeMs = (long)(endTime - session.StartTime).TotalMilliseconds;
                session.MemoryDeltaBytes = endMemory - session.StartMemoryBytes;

                // Update aggregated metrics
                aggregatedMetrics.TotalExecutionTimeMs += session.ExecutionTimeMs;
                aggregatedMetrics.TotalMemoryUsedBytes += Math.Max(0, session.MemoryDeltaBytes);
                aggregatedMetrics.AverageExecutionTimeMs = aggregatedMetrics.TotalExecutionTimeMs / aggregatedMetrics.SessionCount;

                // Store in history
                StorePerformanceHistory(session);

                // Remove from active sessions
                _activeSessions.TryRemove(session.SessionId, out _);
                
                _logger.LogDebug("Stopped monitoring session {SessionId}, execution time: {ExecutionTime}ms", 
                    session.SessionId, session.ExecutionTimeMs);
            }

            // Calculate additional metrics
            aggregatedMetrics.MemoryEfficiencyRatio = CalculateMemoryEfficiency(aggregatedMetrics);
            aggregatedMetrics.PerformanceScore = CalculatePerformanceScore(aggregatedMetrics);

            return aggregatedMetrics;
        }

        /// <summary>
        /// Get current metrics for an operation
        /// </summary>
        public async Task<Dictionary<string, double>> GetMetricsAsync(string operationName)
        {
            var metrics = new Dictionary<string, double>();
            var activeSessions = GetActiveSessionsForOperation(operationName);

            if (activeSessions.Count == 0)
            {
                // Return historical metrics if available
                if (_performanceHistory.TryGetValue(operationName, out var history))
                {
                    metrics["AverageExecutionTimeMs"] = history.AverageExecutionTimeMs;
                    metrics["AverageMemoryUsageMB"] = history.AverageMemoryUsageMB;
                    metrics["SuccessRate"] = history.SuccessRate;
                    metrics["TotalExecutions"] = history.TotalExecutions;
                }
                return metrics;
            }

            // Calculate current metrics from active sessions
            var totalExecutionTime = 0L;
            var totalMemoryUsage = 0L;
            var currentTime = DateTime.UtcNow;

            foreach (var session in activeSessions)
            {
                var currentExecutionTime = (long)(currentTime - session.StartTime).TotalMilliseconds;
                var currentMemoryUsage = GC.GetTotalMemory(false) - session.StartMemoryBytes;

                totalExecutionTime += currentExecutionTime;
                totalMemoryUsage += Math.Max(0, currentMemoryUsage);
            }

            metrics["CurrentExecutionTimeMs"] = totalExecutionTime / activeSessions.Count;
            metrics["CurrentMemoryUsageMB"] = (totalMemoryUsage / activeSessions.Count) / (1024.0 * 1024.0);
            metrics["ActiveSessions"] = activeSessions.Count;

            // Add system metrics if available
            try
            {
                if (_cpuCounter != null)
                {
                    metrics["CPUUsagePercent"] = _cpuCounter.NextValue();
                }
                if (_memoryCounter != null)
                {
                    metrics["AvailableMemoryMB"] = _memoryCounter.NextValue();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not retrieve system performance counters");
            }

            return metrics;
        }

        /// <summary>
        /// Get performance recommendations based on current metrics
        /// </summary>
        public List<PerformanceRecommendation> GetPerformanceRecommendations(string operationName)
        {
            var recommendations = new List<PerformanceRecommendation>();

            if (_performanceHistory.TryGetValue(operationName, out var history))
            {
                // Execution time recommendations
                if (history.AverageExecutionTimeMs > 5000) // 5 seconds
                {
                    recommendations.Add(new PerformanceRecommendation
                    {
                        Type = RecommendationType.ExecutionTime,
                        Priority = RecommendationPriority.High,
                        Description = "Operation execution time is high. Consider optimizing algorithms or reducing data size.",
                        Metric = "AverageExecutionTimeMs",
                        CurrentValue = history.AverageExecutionTimeMs,
                        TargetValue = 2000,
                        Actions = new List<string>
                        {
                            "Enable parallel processing",
                            "Implement data batching",
                            "Use faster algorithms",
                            "Add result caching"
                        }
                    });
                }

                // Memory usage recommendations
                if (history.AverageMemoryUsageMB > 500) // 500 MB
                {
                    recommendations.Add(new PerformanceRecommendation
                    {
                        Type = RecommendationType.Memory,
                        Priority = RecommendationPriority.Medium,
                        Description = "Memory usage is high. Consider streaming processing or data reduction.",
                        Metric = "AverageMemoryUsageMB",
                        CurrentValue = history.AverageMemoryUsageMB,
                        TargetValue = 250,
                        Actions = new List<string>
                        {
                            "Implement streaming processing",
                            "Reduce data batch sizes",
                            "Optimize data structures",
                            "Enable garbage collection tuning"
                        }
                    });
                }

                // Success rate recommendations
                if (history.SuccessRate < 0.95) // Less than 95%
                {
                    recommendations.Add(new PerformanceRecommendation
                    {
                        Type = RecommendationType.Reliability,
                        Priority = RecommendationPriority.High,
                        Description = "Operation success rate is below target. Investigate error patterns.",
                        Metric = "SuccessRate",
                        CurrentValue = history.SuccessRate * 100,
                        TargetValue = 95,
                        Actions = new List<string>
                        {
                            "Add input validation",
                            "Implement retry mechanisms",
                            "Improve error handling",
                            "Add resource checks"
                        }
                    });
                }
            }

            return recommendations;
        }

        /// <summary>
        /// Get performance trend analysis
        /// </summary>
        public PerformanceTrendAnalysis GetTrendAnalysis(string operationName, TimeSpan timeWindow)
        {
            var analysis = new PerformanceTrendAnalysis
            {
                OperationName = operationName,
                TimeWindow = timeWindow,
                AnalysisTimestamp = DateTime.UtcNow
            };

            if (_performanceHistory.TryGetValue(operationName, out var history))
            {
                var recentSessions = history.RecentSessions
                    .Where(s => s.StartTime > DateTime.UtcNow - timeWindow)
                    .OrderBy(s => s.StartTime)
                    .ToList();

                if (recentSessions.Count >= 2)
                {
                    // Calculate trends
                    var executionTimes = recentSessions.Select(s => (double)s.ExecutionTimeMs).ToArray();
                    var memoryUsages = recentSessions.Select(s => s.MemoryDeltaBytes / (1024.0 * 1024.0)).ToArray();

                    analysis.ExecutionTimeTrend = CalculateTrend(executionTimes);
                    analysis.MemoryUsageTrend = CalculateTrend(memoryUsages);
                    analysis.ThroughputTrend = CalculateThroughputTrend(recentSessions);
                    
                    analysis.TrendDirection = DetermineTrendDirection(
                        analysis.ExecutionTimeTrend, analysis.MemoryUsageTrend);
                }
            }

            return analysis;
        }

        #region Private Methods

        private void MonitorSystemResources(object? state)
        {
            try
            {
                if (_cpuCounter != null && _memoryCounter != null)
                {
                    var cpuUsage = _cpuCounter.NextValue();
                    var availableMemory = _memoryCounter.NextValue();

                    // Log warnings if resources are constrained
                    if (cpuUsage > 80)
                    {
                        _logger.LogWarning("High CPU usage detected: {CPUUsage:F1}%", cpuUsage);
                    }
                    if (availableMemory < 512) // Less than 512 MB available
                    {
                        _logger.LogWarning("Low memory available: {AvailableMemory:F0} MB", availableMemory);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error monitoring system resources");
            }
        }

        private void MonitorSystemResourcesBasic(object? state)
        {
            try
            {
                var totalMemory = GC.GetTotalMemory(false);
                var memoryMB = totalMemory / (1024.0 * 1024.0);

                if (memoryMB > 1024) // More than 1GB in use
                {
                    _logger.LogWarning("High managed memory usage: {MemoryMB:F0} MB", memoryMB);
                }

                // Trigger garbage collection if memory usage is very high
                if (memoryMB > 2048) // More than 2GB
                {
                    _logger.LogInformation("Triggering garbage collection due to high memory usage");
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error in basic resource monitoring");
            }
        }

        private List<PerformanceSession> GetActiveSessionsForOperation(string operationName)
        {
            return _activeSessions.Values
                .Where(s => s.OperationName == operationName)
                .ToList();
        }

        private void StorePerformanceHistory(PerformanceSession session)
        {
            var history = _performanceHistory.GetOrAdd(session.OperationName, 
                _ => new PerformanceHistory { OperationName = session.OperationName });

            lock (history)
            {
                history.TotalExecutions++;
                history.TotalExecutionTimeMs += session.ExecutionTimeMs;
                history.TotalMemoryUsageBytes += Math.Max(0, session.MemoryDeltaBytes);

                history.AverageExecutionTimeMs = history.TotalExecutionTimeMs / history.TotalExecutions;
                history.AverageMemoryUsageMB = (history.TotalMemoryUsageBytes / history.TotalExecutions) / (1024.0 * 1024.0);

                // Update success rate (assume success if no exception recorded)
                if (session.Exception == null)
                {
                    history.SuccessfulExecutions++;
                }
                history.SuccessRate = (double)history.SuccessfulExecutions / history.TotalExecutions;

                // Keep recent sessions for trend analysis (max 100)
                history.RecentSessions.Add(session);
                if (history.RecentSessions.Count > 100)
                {
                    history.RecentSessions.RemoveAt(0);
                }

                history.LastUpdated = DateTime.UtcNow;
            }
        }

        private double CalculateMemoryEfficiency(PerformanceMetrics metrics)
        {
            if (metrics.TotalMemoryUsedBytes == 0) return 1.0;
            
            // Simple efficiency calculation: execution time per MB used
            var memoryMB = metrics.TotalMemoryUsedBytes / (1024.0 * 1024.0);
            return metrics.TotalExecutionTimeMs / memoryMB;
        }

        private double CalculatePerformanceScore(PerformanceMetrics metrics)
        {
            // Composite score based on execution time and memory efficiency
            var timeScore = Math.Max(0, 100 - (metrics.AverageExecutionTimeMs / 100.0));
            var memoryScore = Math.Max(0, 100 - (metrics.TotalMemoryUsedBytes / (1024.0 * 1024.0 * 10.0)));
            
            return (timeScore + memoryScore) / 2.0;
        }

        private double CalculateTrend(double[] values)
        {
            if (values.Length < 2) return 0.0;

            // Simple linear regression slope
            var n = values.Length;
            var sumX = 0.0;
            var sumY = 0.0;
            var sumXY = 0.0;
            var sumX2 = 0.0;

            for (int i = 0; i < n; i++)
            {
                sumX += i;
                sumY += values[i];
                sumXY += i * values[i];
                sumX2 += i * i;
            }

            var slope = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
            return slope;
        }

        private double CalculateThroughputTrend(List<PerformanceSession> sessions)
        {
            if (sessions.Count < 2) return 0.0;

            // Calculate throughput as operations per minute
            var timeSpan = sessions.Last().StartTime - sessions.First().StartTime;
            if (timeSpan.TotalMinutes == 0) return 0.0;

            return sessions.Count / timeSpan.TotalMinutes;
        }

        private TrendDirection DetermineTrendDirection(double executionTimeTrend, double memoryUsageTrend)
        {
            var threshold = 0.1;

            if (executionTimeTrend < -threshold && memoryUsageTrend < -threshold)
                return TrendDirection.Improving;
            else if (executionTimeTrend > threshold && memoryUsageTrend > threshold)
                return TrendDirection.Degrading;
            else
                return TrendDirection.Stable;
        }

        #endregion

        public void Dispose()
        {
            _resourceMonitorTimer?.Dispose();
            _cpuCounter?.Dispose();
            _memoryCounter?.Dispose();
        }
    }

    #region Supporting Classes

    public class PerformanceSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string OperationName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public long StartMemoryBytes { get; set; }
        public long? EndMemoryBytes { get; set; }
        public long ExecutionTimeMs { get; set; }
        public long MemoryDeltaBytes { get; set; }
        public PerformanceTuningContext? Context { get; set; }
        public Dictionary<string, double> Metrics { get; set; } = new();
        public Exception? Exception { get; set; }
    }

    public class PerformanceHistory
    {
        public string OperationName { get; set; } = string.Empty;
        public long TotalExecutions { get; set; }
        public long SuccessfulExecutions { get; set; }
        public double SuccessRate { get; set; }
        public long TotalExecutionTimeMs { get; set; }
        public long TotalMemoryUsageBytes { get; set; }
        public double AverageExecutionTimeMs { get; set; }
        public double AverageMemoryUsageMB { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<PerformanceSession> RecentSessions { get; set; } = new();
    }

    public class PerformanceMetrics
    {
        public string OperationName { get; set; } = string.Empty;
        public int SessionCount { get; set; }
        public long TotalExecutionTimeMs { get; set; }
        public double AverageExecutionTimeMs { get; set; }
        public long TotalMemoryUsedBytes { get; set; }
        public double MemoryEfficiencyRatio { get; set; }
        public double PerformanceScore { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PerformanceRecommendation
    {
        public RecommendationType Type { get; set; }
        public RecommendationPriority Priority { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Metric { get; set; } = string.Empty;
        public double CurrentValue { get; set; }
        public double TargetValue { get; set; }
        public List<string> Actions { get; set; } = new();
    }

    public class PerformanceTrendAnalysis
    {
        public string OperationName { get; set; } = string.Empty;
        public TimeSpan TimeWindow { get; set; }
        public DateTime AnalysisTimestamp { get; set; }
        public double ExecutionTimeTrend { get; set; }
        public double MemoryUsageTrend { get; set; }
        public double ThroughputTrend { get; set; }
        public TrendDirection TrendDirection { get; set; }
    }

    public enum RecommendationType
    {
        ExecutionTime,
        Memory,
        Reliability,
        Throughput,
        ResourceUtilization
    }

    public enum RecommendationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TrendDirection
    {
        Improving,
        Stable,
        Degrading
    }

    #endregion
}
