using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ALARM.DataPersistence.Models;

namespace ALARM.DataPersistence.Services;

public interface ILearningDataService
{
    // Run data operations
    Task<int> SaveRunDataAsync(RunDataDto runData, List<PatternDto>? patterns = null, List<ImprovementDto>? improvements = null, List<PerformanceMetricDto>? metrics = null);
    Task<RunDataDto?> GetRunDataAsync(string runId);
    Task<List<RunDataDto>> GetRunDataAsync(DateTime startDate, DateTime endDate, string? projectName = null);
    Task<List<RunDataDto>> GetRecentRunsAsync(int count = 50);
    
    // Pattern operations
    Task SavePatternsAsync(int runId, List<PatternDto> patterns);
    Task<List<PatternDto>> GetPatternsAsync(string patternType, DateTime? startDate = null, DateTime? endDate = null);
    Task<Dictionary<string, int>> GetPatternFrequencyAsync(string patternType, TimeSpan timeWindow);
    
    // Improvement operations
    Task SaveImprovementsAsync(int runId, List<ImprovementDto> improvements);
    Task<List<ImprovementDto>> GetPendingImprovementsAsync(string? priority = null);
    Task MarkImprovementAppliedAsync(int improvementId, string result);
    Task<List<ImprovementDto>> GetAppliedImprovementsAsync(DateTime startDate, DateTime endDate);
    
    // Performance metrics operations
    Task SavePerformanceMetricsAsync(int runId, List<PerformanceMetricDto> metrics);
    Task<List<PerformanceMetricDto>> GetPerformanceMetricsAsync(string metricName, DateTime startDate, DateTime endDate);
    Task<Dictionary<string, double>> GetLatestMetricsAsync(List<string> metricNames);
    Task<Dictionary<string, List<double>>> GetMetricTrendsAsync(List<string> metricNames, TimeSpan timeWindow);
    
    // ML model operations
    Task<int> SaveMLModelAsync(string modelName, string modelType, byte[] modelData, double accuracy, int trainingDataCount, Dictionary<string, object> hyperParameters, Dictionary<string, double> validationMetrics);
    Task<MLModelEntity?> GetActiveModelAsync(string modelName, string modelType);
    Task<bool> DeactivateModelAsync(int modelId);
    Task<List<MLModelEntity>> GetModelHistoryAsync(string modelName);
    
    // Prediction operations
    Task SavePredictionAsync(int modelId, int runId, string predictionType, double predictedValue, double confidence, Dictionary<string, object> inputFeatures);
    Task UpdatePredictionValidationAsync(int predictionId, double actualValue);
    Task<List<PredictionEntity>> GetPredictionsAsync(string predictionType, DateTime startDate, DateTime endDate);
    Task<double> GetModelAccuracyAsync(int modelId);
    
    // Feedback operations
    Task SaveFeedbackAsync(int runId, FeedbackDto feedback);
    Task<List<FeedbackDto>> GetUnprocessedFeedbackAsync();
    Task MarkFeedbackProcessedAsync(int feedbackId);
    Task<Dictionary<string, double>> GetFeedbackStatsAsync(DateTime startDate, DateTime endDate);
    
    // Protocol version operations
    Task<int> SaveProtocolVersionAsync(string protocolName, string version, string content, string changeLog, string createdBy);
    Task<bool> ActivateProtocolVersionAsync(int versionId);
    Task<ProtocolVersionEntity?> GetActiveProtocolVersionAsync(string protocolName);
    Task<List<ProtocolVersionEntity>> GetProtocolHistoryAsync(string protocolName);
    
    // Analytics and reporting
    Task<Dictionary<string, object>> GetAnalyticsSummaryAsync(DateTime startDate, DateTime endDate);
    Task<List<Dictionary<string, object>>> GetSuccessFactorsAnalysisAsync();
    Task<List<Dictionary<string, object>>> GetFailureAnalysisAsync();
    Task<Dictionary<string, double>> GetPerformanceTrendsAsync(TimeSpan timeWindow);
    
    // Data cleanup and maintenance
    Task<int> CleanupOldDataAsync(DateTime cutoffDate);
    Task<bool> BackupDataAsync(string backupPath);
    Task<Dictionary<string, object>> GetDataHealthAsync();
}

public class LearningDataService : ILearningDataService
{
    private readonly LearningDataContext _context;
    private readonly ILogger<LearningDataService> _logger;

    public LearningDataService(LearningDataContext context, ILogger<LearningDataService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SaveRunDataAsync(RunDataDto runData, List<PatternDto>? patterns = null, List<ImprovementDto>? improvements = null, List<PerformanceMetricDto>? metrics = null)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            // Save run data
            var runEntity = new RunEntity
            {
                RunId = runData.RunId,
                Timestamp = runData.Timestamp,
                ProjectName = runData.ProjectName,
                Environment = runData.Environment,
                Success = runData.Success,
                Duration = runData.Duration,
                IndexDataJson = JsonSerializer.Serialize(runData.IndexData),
                RiskAssessmentJson = JsonSerializer.Serialize(runData.RiskAssessment),
                TestResultsJson = JsonSerializer.Serialize(runData.TestResults),
                MetricsJson = JsonSerializer.Serialize(runData.Metrics)
            };

            _context.Runs.Add(runEntity);
            await _context.SaveChangesAsync();

            var runId = runEntity.Id;

            // Save associated data
            if (patterns != null && patterns.Any())
            {
                await SavePatternsAsync(runId, patterns);
            }

            if (improvements != null && improvements.Any())
            {
                await SaveImprovementsAsync(runId, improvements);
            }

            if (metrics != null && metrics.Any())
            {
                await SavePerformanceMetricsAsync(runId, metrics);
            }

            await transaction.CommitAsync();
            
            _logger.LogInformation("Saved run data for {RunId} with {PatternCount} patterns, {ImprovementCount} improvements, {MetricCount} metrics",
                runData.RunId, patterns?.Count ?? 0, improvements?.Count ?? 0, metrics?.Count ?? 0);

            return runId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save run data for {RunId}", runData.RunId);
            throw;
        }
    }

    public async Task<RunDataDto?> GetRunDataAsync(string runId)
    {
        try
        {
            var runEntity = await _context.Runs
                .Include(r => r.Patterns)
                .Include(r => r.Improvements)
                .Include(r => r.PerformanceMetrics)
                .FirstOrDefaultAsync(r => r.RunId == runId);

            if (runEntity == null)
                return null;

            return MapToRunDataDto(runEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get run data for {RunId}", runId);
            throw;
        }
    }

    public async Task<List<RunDataDto>> GetRunDataAsync(DateTime startDate, DateTime endDate, string? projectName = null)
    {
        try
        {
            var query = _context.Runs
                .Include(r => r.Patterns)
                .Include(r => r.Improvements)
                .Include(r => r.PerformanceMetrics)
                .Where(r => r.Timestamp >= startDate && r.Timestamp <= endDate);

            if (!string.IsNullOrEmpty(projectName))
            {
                query = query.Where(r => r.ProjectName == projectName);
            }

            var runEntities = await query
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();

            return runEntities.Select(MapToRunDataDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get run data for date range {StartDate} - {EndDate}", startDate, endDate);
            throw;
        }
    }

    public async Task<List<RunDataDto>> GetRecentRunsAsync(int count = 50)
    {
        try
        {
            var runEntities = await _context.Runs
                .Include(r => r.Patterns)
                .Include(r => r.Improvements)
                .Include(r => r.PerformanceMetrics)
                .OrderByDescending(r => r.Timestamp)
                .Take(count)
                .ToListAsync();

            return runEntities.Select(MapToRunDataDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get recent runs");
            throw;
        }
    }

    public async Task SavePatternsAsync(int runId, List<PatternDto> patterns)
    {
        try
        {
            var patternEntities = patterns.Select(p => new PatternEntity
            {
                RunId = runId,
                PatternType = p.PatternType,
                PatternName = p.PatternName,
                Description = p.Description,
                FilePath = p.FilePath,
                Confidence = p.Confidence,
                Frequency = p.Frequency,
                ContextJson = JsonSerializer.Serialize(p.Context),
                DetectedAt = DateTime.UtcNow
            }).ToList();

            _context.Patterns.AddRange(patternEntities);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Saved {PatternCount} patterns for run {RunId}", patterns.Count, runId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save patterns for run {RunId}", runId);
            throw;
        }
    }

    public async Task<List<PatternDto>> GetPatternsAsync(string patternType, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var query = _context.Patterns
                .Where(p => p.PatternType == patternType);

            if (startDate.HasValue)
            {
                query = query.Where(p => p.DetectedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.DetectedAt <= endDate.Value);
            }

            var patternEntities = await query
                .OrderByDescending(p => p.DetectedAt)
                .ToListAsync();

            return patternEntities.Select(MapToPatternDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get patterns for type {PatternType}", patternType);
            throw;
        }
    }

    public async Task<Dictionary<string, int>> GetPatternFrequencyAsync(string patternType, TimeSpan timeWindow)
    {
        try
        {
            var startDate = DateTime.UtcNow - timeWindow;

            var frequencies = await _context.Patterns
                .Where(p => p.PatternType == patternType && p.DetectedAt >= startDate)
                .GroupBy(p => p.PatternName)
                .Select(g => new { PatternName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PatternName, x => x.Count);

            return frequencies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pattern frequency for type {PatternType}", patternType);
            throw;
        }
    }

    public async Task SaveImprovementsAsync(int runId, List<ImprovementDto> improvements)
    {
        try
        {
            var improvementEntities = improvements.Select(i => new ImprovementEntity
            {
                RunId = runId,
                Type = i.Type,
                Priority = i.Priority,
                Description = i.Description,
                Recommendation = i.Recommendation,
                EstimatedEffort = i.EstimatedEffort,
                Confidence = i.Confidence,
                Applied = i.Applied,
                AppliedAt = i.AppliedAt,
                AppliedResult = i.AppliedResult,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.Improvements.AddRange(improvementEntities);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Saved {ImprovementCount} improvements for run {RunId}", improvements.Count, runId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save improvements for run {RunId}", runId);
            throw;
        }
    }

    public async Task<List<ImprovementDto>> GetPendingImprovementsAsync(string? priority = null)
    {
        try
        {
            var query = _context.Improvements
                .Where(i => !i.Applied);

            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(i => i.Priority == priority);
            }

            var improvementEntities = await query
                .OrderBy(i => i.Priority == "Critical" ? 1 : i.Priority == "High" ? 2 : i.Priority == "Medium" ? 3 : 4)
                .ThenByDescending(i => i.Confidence)
                .ToListAsync();

            return improvementEntities.Select(MapToImprovementDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get pending improvements");
            throw;
        }
    }

    public async Task MarkImprovementAppliedAsync(int improvementId, string result)
    {
        try
        {
            var improvement = await _context.Improvements.FindAsync(improvementId);
            if (improvement != null)
            {
                improvement.Applied = true;
                improvement.AppliedAt = DateTime.UtcNow;
                improvement.AppliedResult = result;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Marked improvement {ImprovementId} as applied", improvementId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark improvement {ImprovementId} as applied", improvementId);
            throw;
        }
    }

    // Additional methods would continue here...
    // Due to length constraints, I'll include key mapping methods

    private RunDataDto MapToRunDataDto(RunEntity entity)
    {
        return new RunDataDto
        {
            RunId = entity.RunId,
            Timestamp = entity.Timestamp,
            ProjectName = entity.ProjectName,
            Environment = entity.Environment,
            Success = entity.Success,
            Duration = entity.Duration,
            IndexData = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.IndexDataJson) ?? new(),
            RiskAssessment = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.RiskAssessmentJson) ?? new(),
            TestResults = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(entity.TestResultsJson) ?? new(),
            Metrics = JsonSerializer.Deserialize<Dictionary<string, double>>(entity.MetricsJson) ?? new()
        };
    }

    private PatternDto MapToPatternDto(PatternEntity entity)
    {
        return new PatternDto
        {
            PatternType = entity.PatternType,
            PatternName = entity.PatternName,
            Description = entity.Description,
            FilePath = entity.FilePath,
            Confidence = entity.Confidence,
            Frequency = entity.Frequency,
            Context = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.ContextJson) ?? new()
        };
    }

    private ImprovementDto MapToImprovementDto(ImprovementEntity entity)
    {
        return new ImprovementDto
        {
            Type = entity.Type,
            Priority = entity.Priority,
            Description = entity.Description,
            Recommendation = entity.Recommendation,
            EstimatedEffort = entity.EstimatedEffort,
            Confidence = entity.Confidence,
            Applied = entity.Applied,
            AppliedAt = entity.AppliedAt,
            AppliedResult = entity.AppliedResult
        };
    }

    // Placeholder implementations for remaining interface methods
    public Task<List<ImprovementDto>> GetAppliedImprovementsAsync(DateTime startDate, DateTime endDate) => throw new NotImplementedException();
    public Task SavePerformanceMetricsAsync(int runId, List<PerformanceMetricDto> metrics) => throw new NotImplementedException();
    public Task<List<PerformanceMetricDto>> GetPerformanceMetricsAsync(string metricName, DateTime startDate, DateTime endDate) => throw new NotImplementedException();
    public Task<Dictionary<string, double>> GetLatestMetricsAsync(List<string> metricNames) => throw new NotImplementedException();
    public Task<Dictionary<string, List<double>>> GetMetricTrendsAsync(List<string> metricNames, TimeSpan timeWindow) => throw new NotImplementedException();
    public Task<int> SaveMLModelAsync(string modelName, string modelType, byte[] modelData, double accuracy, int trainingDataCount, Dictionary<string, object> hyperParameters, Dictionary<string, double> validationMetrics) => throw new NotImplementedException();
    public Task<MLModelEntity?> GetActiveModelAsync(string modelName, string modelType) => throw new NotImplementedException();
    public Task<bool> DeactivateModelAsync(int modelId) => throw new NotImplementedException();
    public Task<List<MLModelEntity>> GetModelHistoryAsync(string modelName) => throw new NotImplementedException();
    public Task SavePredictionAsync(int modelId, int runId, string predictionType, double predictedValue, double confidence, Dictionary<string, object> inputFeatures) => throw new NotImplementedException();
    public Task UpdatePredictionValidationAsync(int predictionId, double actualValue) => throw new NotImplementedException();
    public Task<List<PredictionEntity>> GetPredictionsAsync(string predictionType, DateTime startDate, DateTime endDate) => throw new NotImplementedException();
    public Task<double> GetModelAccuracyAsync(int modelId) => throw new NotImplementedException();
    public Task SaveFeedbackAsync(int runId, FeedbackDto feedback) => throw new NotImplementedException();
    public Task<List<FeedbackDto>> GetUnprocessedFeedbackAsync() => throw new NotImplementedException();
    public Task MarkFeedbackProcessedAsync(int feedbackId) => throw new NotImplementedException();
    public Task<Dictionary<string, double>> GetFeedbackStatsAsync(DateTime startDate, DateTime endDate) => throw new NotImplementedException();
    public Task<int> SaveProtocolVersionAsync(string protocolName, string version, string content, string changeLog, string createdBy) => throw new NotImplementedException();
    public Task<bool> ActivateProtocolVersionAsync(int versionId) => throw new NotImplementedException();
    public Task<ProtocolVersionEntity?> GetActiveProtocolVersionAsync(string protocolName) => throw new NotImplementedException();
    public Task<List<ProtocolVersionEntity>> GetProtocolHistoryAsync(string protocolName) => throw new NotImplementedException();
    public Task<Dictionary<string, object>> GetAnalyticsSummaryAsync(DateTime startDate, DateTime endDate) => throw new NotImplementedException();
    public Task<List<Dictionary<string, object>>> GetSuccessFactorsAnalysisAsync() => throw new NotImplementedException();
    public Task<List<Dictionary<string, object>>> GetFailureAnalysisAsync() => throw new NotImplementedException();
    public Task<Dictionary<string, double>> GetPerformanceTrendsAsync(TimeSpan timeWindow) => throw new NotImplementedException();
    public Task<int> CleanupOldDataAsync(DateTime cutoffDate) => throw new NotImplementedException();
    public Task<bool> BackupDataAsync(string backupPath) => throw new NotImplementedException();
    public Task<Dictionary<string, object>> GetDataHealthAsync() => throw new NotImplementedException();
}
