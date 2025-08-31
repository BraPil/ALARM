using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Integrates suggestion validation with the feedback system
    /// </summary>
    public class FeedbackIntegrationService
    {
        private readonly ILogger _logger;
        private readonly string _databasePath;

        public FeedbackIntegrationService(ILogger logger, string? databasePath = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databasePath = databasePath ?? "suggestion_validation.db";
        }

        /// <summary>
        /// Record validation result in the feedback system
        /// </summary>
        public async Task RecordValidationResultAsync(SuggestionValidationResult validationResult)
        {
            _logger.LogInformation("Recording validation result for {AnalysisType} with {SuggestionCount} suggestions", 
                validationResult.AnalysisType, validationResult.SuggestionValidations.Count);

            try
            {
                using var context = CreateValidationDbContext();
                await context.Database.EnsureCreatedAsync();

                // Create validation record
                var validationRecord = new ValidationRecord
                {
                    ValidationId = validationResult.ValidationId,
                    AnalysisType = validationResult.AnalysisType.ToString(),
                    ValidationTimestamp = validationResult.ValidationTimestamp,
                    SourceAnalysisId = validationResult.SourceAnalysisId,
                    OverallQualityScore = validationResult.OverallQualityScore,
                    QualityMetricsJson = JsonSerializer.Serialize(validationResult.QualityMetrics),
                    ImprovementRecommendationsJson = JsonSerializer.Serialize(validationResult.ImprovementRecommendations),
                    ValidationContextJson = JsonSerializer.Serialize(validationResult.ValidationContext)
                };

                context.ValidationRecords.Add(validationRecord);

                // Create individual suggestion records
                foreach (var suggestionValidation in validationResult.SuggestionValidations)
                {
                    var suggestionRecord = new SuggestionValidationRecord
                    {
                        SuggestionId = suggestionValidation.SuggestionId,
                        ValidationId = validationResult.ValidationId,
                        SuggestionText = suggestionValidation.SuggestionText,
                        OverallScore = suggestionValidation.OverallScore,
                        QualityScoresJson = JsonSerializer.Serialize(suggestionValidation.QualityScores),
                        ValidationDetailsJson = JsonSerializer.Serialize(suggestionValidation.ValidationDetails),
                        IssuesJson = JsonSerializer.Serialize(suggestionValidation.Issues),
                        ImprovementsJson = JsonSerializer.Serialize(suggestionValidation.Improvements),
                        ValidationTimestamp = suggestionValidation.ValidationTimestamp
                    };

                    context.SuggestionValidationRecords.Add(suggestionRecord);
                }

                await context.SaveChangesAsync();
                _logger.LogDebug("Validation result recorded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording validation result");
                throw;
            }
        }

        /// <summary>
        /// Record comprehensive validation result
        /// </summary>
        public async Task RecordComprehensiveValidationAsync(ComprehensiveSuggestionValidationResult comprehensiveResult)
        {
            _logger.LogInformation("Recording comprehensive validation result with {AnalysisCount} analysis types", 
                comprehensiveResult.ValidationResults.Count);

            try
            {
                // Record individual validation results
                foreach (var validationResult in comprehensiveResult.ValidationResults.Values)
                {
                    await RecordValidationResultAsync(validationResult);
                }

                // Record comprehensive analysis
                using var context = CreateValidationDbContext();
                var comprehensiveRecord = new ComprehensiveValidationRecord
                {
                    ValidationId = comprehensiveResult.ValidationId,
                    ValidationTimestamp = comprehensiveResult.ValidationTimestamp,
                    OverallSystemQuality = comprehensiveResult.OverallSystemQuality,
                    CrossAnalysisConsistencyJson = JsonSerializer.Serialize(comprehensiveResult.CrossAnalysisConsistency),
                    SystemWideImprovementsJson = JsonSerializer.Serialize(comprehensiveResult.SystemWideImprovements),
                    ValidationContextJson = JsonSerializer.Serialize(comprehensiveResult.ValidationContext),
                    ComprehensiveMetricsJson = JsonSerializer.Serialize(comprehensiveResult.ComprehensiveMetrics)
                };

                context.ComprehensiveValidationRecords.Add(comprehensiveRecord);
                await context.SaveChangesAsync();

                _logger.LogDebug("Comprehensive validation result recorded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording comprehensive validation result");
                throw;
            }
        }

        /// <summary>
        /// Get validation trends over specified time window
        /// </summary>
        public async Task<FeedbackTrendsData> GetValidationTrendsAsync(TimeSpan timeWindow, AnalysisType? analysisType = null)
        {
            _logger.LogInformation("Retrieving validation trends for {TimeWindow} period", timeWindow);

            try
            {
                using var context = CreateValidationDbContext();
                var cutoffDate = DateTime.UtcNow - timeWindow;

                var query = context.ValidationRecords
                    .Where(r => r.ValidationTimestamp >= cutoffDate);

                if (analysisType.HasValue)
                {
                    query = query.Where(r => r.AnalysisType == analysisType.Value.ToString());
                }

                var records = await query.OrderBy(r => r.ValidationTimestamp).ToListAsync();

                var trendsData = new FeedbackTrendsData
                {
                    StartDate = cutoffDate,
                    EndDate = DateTime.UtcNow,
                    QualityTrends = CalculateQualityTrends(records),
                    VolumeStats = CalculateVolumeStats(records),
                    TopIssues = await CalculateTopIssuesAsync(records, context)
                };

                return trendsData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving validation trends");
                throw;
            }
        }

        /// <summary>
        /// Get training data for ML model improvement
        /// </summary>
        public async Task<List<SuggestionTrainingData>> GetTrainingDataAsync(
            AnalysisType analysisType, 
            int maxSamples = 1000)
        {
            _logger.LogInformation("Retrieving training data for {AnalysisType}, max {MaxSamples} samples", 
                analysisType, maxSamples);

            try
            {
                using var context = CreateValidationDbContext();
                
                var validationRecords = await context.ValidationRecords
                    .Where(r => r.AnalysisType == analysisType.ToString())
                    .OrderByDescending(r => r.ValidationTimestamp)
                    .Take(maxSamples)
                    .Include(r => r.SuggestionValidations)
                    .ToListAsync();

                var trainingData = new List<SuggestionTrainingData>();

                foreach (var record in validationRecords)
                {
                    foreach (var suggestion in record.SuggestionValidations)
                    {
                        var qualityBreakdown = string.IsNullOrEmpty(suggestion.QualityScoresJson) 
                            ? new Dictionary<string, double>()
                            : JsonSerializer.Deserialize<Dictionary<string, double>>(suggestion.QualityScoresJson) ?? new();

                        trainingData.Add(new SuggestionTrainingData
                        {
                            SuggestionText = suggestion.SuggestionText,
                            ActualQualityScore = suggestion.OverallScore,
                            AnalysisType = analysisType,
                            QualityBreakdown = qualityBreakdown,
                            ValidationDate = suggestion.ValidationTimestamp,
                            ValidatorId = record.ValidationId
                        });
                    }
                }

                _logger.LogDebug("Retrieved {TrainingDataCount} training samples for {AnalysisType}", 
                    trainingData.Count, analysisType);

                return trainingData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving training data for {AnalysisType}", analysisType);
                return new List<SuggestionTrainingData>();
            }
        }

        #region Private Helper Methods

        private ValidationDbContext CreateValidationDbContext()
        {
            var options = new DbContextOptionsBuilder<ValidationDbContext>()
                .UseSqlite($"Data Source={_databasePath}")
                .Options;
            
            return new ValidationDbContext(options);
        }

        private Dictionary<string, List<double>> CalculateQualityTrends(List<ValidationRecord> records)
        {
            var trends = new Dictionary<string, List<double>>();

            // Overall quality trend
            trends["OverallQuality"] = records.Select(r => r.OverallQualityScore).ToList();

            // Daily averages
            var dailyGroups = records.GroupBy(r => r.ValidationTimestamp.Date);
            trends["DailyAverageQuality"] = dailyGroups.Select(g => g.Average(r => r.OverallQualityScore)).ToList();

            return trends;
        }

        private ValidationVolumeStats CalculateVolumeStats(List<ValidationRecord> records)
        {
            var stats = new ValidationVolumeStats
            {
                TotalValidations = records.Count,
                AverageQualityScore = records.Any() ? records.Average(r => r.OverallQualityScore) : 0.0,
                HighQualitySuggestions = records.Count(r => r.OverallQualityScore > 0.8),
                LowQualitySuggestions = records.Count(r => r.OverallQualityScore < 0.6)
            };

            // Count by analysis type
            var typeGroups = records.GroupBy(r => r.AnalysisType);
            foreach (var group in typeGroups)
            {
                if (Enum.TryParse<AnalysisType>(group.Key, out var analysisType))
                {
                    stats.ValidationsByType[analysisType] = group.Count();
                }
            }

            return stats;
        }

        private async Task<List<TopValidationIssue>> CalculateTopIssuesAsync(List<ValidationRecord> records, ValidationDbContext context)
        {
            var topIssues = new List<TopValidationIssue>();

            try
            {
                // Get all suggestion validations for these records
                var validationIds = records.Select(r => r.ValidationId).ToList();
                var suggestionRecords = await context.SuggestionValidationRecords
                    .Where(sr => validationIds.Contains(sr.ValidationId))
                    .ToListAsync();

                // Extract and analyze issues
                var allIssues = new List<ValidationIssue>();
                foreach (var suggestionRecord in suggestionRecords)
                {
                    if (!string.IsNullOrEmpty(suggestionRecord.IssuesJson))
                    {
                        var issues = JsonSerializer.Deserialize<List<ValidationIssue>>(suggestionRecord.IssuesJson);
                        if (issues != null)
                        {
                            allIssues.AddRange(issues);
                        }
                    }
                }

                // Group by issue type and calculate statistics
                var issueGroups = allIssues.GroupBy(i => i.IssueType);
                foreach (var group in issueGroups.Take(10)) // Top 10 issues
                {
                    var issues = group.ToList();
                    topIssues.Add(new TopValidationIssue
                    {
                        IssueType = group.Key,
                        Frequency = issues.Count,
                        TypicalSeverity = GetMostCommonSeverity(issues),
                        CommonSuggestions = GetCommonSuggestions(issues)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error calculating top issues");
            }

            return topIssues.OrderByDescending(i => i.Frequency).ToList();
        }

        private ValidationIssueSeverity GetMostCommonSeverity(List<ValidationIssue> issues)
        {
            return issues.GroupBy(i => i.Severity)
                        .OrderByDescending(g => g.Count())
                        .First()
                        .Key;
        }

        private List<string> GetCommonSuggestions(List<ValidationIssue> issues)
        {
            return issues.Select(i => i.SuggestedFix)
                        .GroupBy(s => s)
                        .OrderByDescending(g => g.Count())
                        .Take(3)
                        .Select(g => g.Key)
                        .ToList();
        }

        #endregion
    }

    #region Database Models

    /// <summary>
    /// Entity Framework context for validation data
    /// </summary>
    public class ValidationDbContext : DbContext
    {
        public ValidationDbContext(DbContextOptions<ValidationDbContext> options) : base(options) { }

        public DbSet<ValidationRecord> ValidationRecords { get; set; }
        public DbSet<SuggestionValidationRecord> SuggestionValidationRecords { get; set; }
        public DbSet<ComprehensiveValidationRecord> ComprehensiveValidationRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValidationRecord>(entity =>
            {
                entity.HasKey(e => e.ValidationId);
                entity.Property(e => e.AnalysisType).IsRequired();
                entity.Property(e => e.ValidationTimestamp).IsRequired();
                entity.HasMany(e => e.SuggestionValidations)
                      .WithOne()
                      .HasForeignKey(sv => sv.ValidationId);
            });

            modelBuilder.Entity<SuggestionValidationRecord>(entity =>
            {
                entity.HasKey(e => e.SuggestionId);
                entity.Property(e => e.SuggestionText).IsRequired();
                entity.Property(e => e.ValidationTimestamp).IsRequired();
            });

            modelBuilder.Entity<ComprehensiveValidationRecord>(entity =>
            {
                entity.HasKey(e => e.ValidationId);
                entity.Property(e => e.ValidationTimestamp).IsRequired();
            });
        }
    }

    /// <summary>
    /// Database entity for validation records
    /// </summary>
    public class ValidationRecord
    {
        public string ValidationId { get; set; } = string.Empty;
        public string AnalysisType { get; set; } = string.Empty;
        public DateTime ValidationTimestamp { get; set; }
        public string SourceAnalysisId { get; set; } = string.Empty;
        public double OverallQualityScore { get; set; }
        public string QualityMetricsJson { get; set; } = string.Empty;
        public string ImprovementRecommendationsJson { get; set; } = string.Empty;
        public string ValidationContextJson { get; set; } = string.Empty;
        
        public List<SuggestionValidationRecord> SuggestionValidations { get; set; } = new();
    }

    /// <summary>
    /// Database entity for individual suggestion validation records
    /// </summary>
    public class SuggestionValidationRecord
    {
        public string SuggestionId { get; set; } = string.Empty;
        public string ValidationId { get; set; } = string.Empty;
        public string SuggestionText { get; set; } = string.Empty;
        public double OverallScore { get; set; }
        public string QualityScoresJson { get; set; } = string.Empty;
        public string ValidationDetailsJson { get; set; } = string.Empty;
        public string IssuesJson { get; set; } = string.Empty;
        public string ImprovementsJson { get; set; } = string.Empty;
        public DateTime ValidationTimestamp { get; set; }
    }

    /// <summary>
    /// Database entity for comprehensive validation records
    /// </summary>
    public class ComprehensiveValidationRecord
    {
        public string ValidationId { get; set; } = string.Empty;
        public DateTime ValidationTimestamp { get; set; }
        public double OverallSystemQuality { get; set; }
        public string CrossAnalysisConsistencyJson { get; set; } = string.Empty;
        public string SystemWideImprovementsJson { get; set; } = string.Empty;
        public string ValidationContextJson { get; set; } = string.Empty;
        public string ComprehensiveMetricsJson { get; set; } = string.Empty;
    }

    #endregion
}
