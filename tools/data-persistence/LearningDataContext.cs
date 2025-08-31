using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ALARM.DataPersistence.Models;

namespace ALARM.DataPersistence;

public class LearningDataContext : DbContext
{
    private readonly IConfiguration? _configuration;
    private readonly ILogger<LearningDataContext>? _logger;

    public LearningDataContext(DbContextOptions<LearningDataContext> options, 
        IConfiguration? configuration = null, 
        ILogger<LearningDataContext>? logger = null) 
        : base(options)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // DbSets for all entities
    public DbSet<RunEntity> Runs { get; set; }
    public DbSet<PatternEntity> Patterns { get; set; }
    public DbSet<ImprovementEntity> Improvements { get; set; }
    public DbSet<PerformanceMetricEntity> PerformanceMetrics { get; set; }
    public DbSet<MLModelEntity> MLModels { get; set; }
    public DbSet<PredictionEntity> Predictions { get; set; }
    public DbSet<FeedbackEntity> Feedback { get; set; }
    public DbSet<ProtocolVersionEntity> ProtocolVersions { get; set; }
    public DbSet<ProtocolUpdateEntity> ProtocolUpdates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Default to SQLite for development
            var connectionString = _configuration?.GetConnectionString("DefaultConnection") 
                ?? "Data Source=alarm_learning.db";
            
            if (connectionString.Contains("Server=") || connectionString.Contains("Data Source=") && connectionString.Contains("Initial Catalog"))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlite(connectionString);
            }

            if (_logger != null)
            {
                optionsBuilder.LogTo(message => _logger.LogInformation(message), LogLevel.Information);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<PatternEntity>()
            .HasOne(p => p.Run)
            .WithMany(r => r.Patterns)
            .HasForeignKey(p => p.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ImprovementEntity>()
            .HasOne(i => i.Run)
            .WithMany(r => r.Improvements)
            .HasForeignKey(i => i.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PerformanceMetricEntity>()
            .HasOne(pm => pm.Run)
            .WithMany(r => r.PerformanceMetrics)
            .HasForeignKey(pm => pm.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PredictionEntity>()
            .HasOne(p => p.Model)
            .WithMany(m => m.Predictions)
            .HasForeignKey(p => p.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PredictionEntity>()
            .HasOne(p => p.Run)
            .WithMany()
            .HasForeignKey(p => p.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FeedbackEntity>()
            .HasOne(f => f.Run)
            .WithMany()
            .HasForeignKey(f => f.RunId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProtocolUpdateEntity>()
            .HasOne(pu => pu.ProtocolVersion)
            .WithMany(pv => pv.Updates)
            .HasForeignKey(pu => pu.ProtocolVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes for performance
        modelBuilder.Entity<RunEntity>()
            .HasIndex(r => r.RunId)
            .IsUnique();

        modelBuilder.Entity<RunEntity>()
            .HasIndex(r => r.Timestamp);

        modelBuilder.Entity<RunEntity>()
            .HasIndex(r => new { r.ProjectName, r.Environment });

        modelBuilder.Entity<PatternEntity>()
            .HasIndex(p => new { p.PatternType, p.PatternName });

        modelBuilder.Entity<PatternEntity>()
            .HasIndex(p => p.DetectedAt);

        modelBuilder.Entity<ImprovementEntity>()
            .HasIndex(i => new { i.Type, i.Priority });

        modelBuilder.Entity<ImprovementEntity>()
            .HasIndex(i => i.Applied);

        modelBuilder.Entity<PerformanceMetricEntity>()
            .HasIndex(pm => new { pm.MetricName, pm.Category });

        modelBuilder.Entity<PerformanceMetricEntity>()
            .HasIndex(pm => pm.MeasuredAt);

        modelBuilder.Entity<MLModelEntity>()
            .HasIndex(m => new { m.ModelName, m.ModelType });

        modelBuilder.Entity<MLModelEntity>()
            .HasIndex(m => m.IsActive);

        modelBuilder.Entity<PredictionEntity>()
            .HasIndex(p => new { p.PredictionType, p.PredictedAt });

        modelBuilder.Entity<FeedbackEntity>()
            .HasIndex(f => new { f.FeedbackType, f.Processed });

        modelBuilder.Entity<ProtocolVersionEntity>()
            .HasIndex(pv => new { pv.ProtocolName, pv.Version })
            .IsUnique();

        modelBuilder.Entity<ProtocolVersionEntity>()
            .HasIndex(pv => pv.IsActive);

        // JSON columns are stored as string and converted in the service layer

        // Configure default values
        modelBuilder.Entity<RunEntity>()
            .Property(r => r.Timestamp)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<PatternEntity>()
            .Property(p => p.DetectedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ImprovementEntity>()
            .Property(i => i.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<PerformanceMetricEntity>()
            .Property(pm => pm.MeasuredAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<MLModelEntity>()
            .Property(m => m.TrainedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<MLModelEntity>()
            .Property(m => m.IsActive)
            .HasDefaultValue(true);

        modelBuilder.Entity<PredictionEntity>()
            .Property(p => p.PredictedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<FeedbackEntity>()
            .Property(f => f.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<FeedbackEntity>()
            .Property(f => f.Processed)
            .HasDefaultValue(false);

        modelBuilder.Entity<ProtocolVersionEntity>()
            .Property(pv => pv.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProtocolVersionEntity>()
            .Property(pv => pv.IsActive)
            .HasDefaultValue(false);

        modelBuilder.Entity<ProtocolUpdateEntity>()
            .Property(pu => pu.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProtocolUpdateEntity>()
            .Property(pu => pu.Applied)
            .HasDefaultValue(false);

        // Seed some initial data
        SeedInitialData(modelBuilder);
    }

    private void SeedInitialData(ModelBuilder modelBuilder)
    {
        // Seed initial protocol versions
        modelBuilder.Entity<ProtocolVersionEntity>().HasData(
            new ProtocolVersionEntity
            {
                Id = 1,
                ProtocolName = "Master Protocol",
                Version = "1.0",
                Content = "Initial Master Protocol version",
                ChangeLog = "Initial creation",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ActivatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new ProtocolVersionEntity
            {
                Id = 2,
                ProtocolName = "Research Protocol",
                Version = "1.0",
                Content = "Initial Research Protocol version",
                ChangeLog = "Initial creation",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ActivatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            },
            new ProtocolVersionEntity
            {
                Id = 3,
                ProtocolName = "Adapt Protocol",
                Version = "1.0",
                Content = "Initial Adapt Protocol version",
                ChangeLog = "Initial creation",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ActivatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }
        );
    }

    public async Task<bool> EnsureDatabaseCreatedAsync()
    {
        try
        {
            var created = await Database.EnsureCreatedAsync();
            if (created)
            {
                _logger?.LogInformation("Learning database created successfully");
            }
            return created;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create learning database");
            return false;
        }
    }

    public async Task<bool> MigrateDatabaseAsync()
    {
        try
        {
            await Database.MigrateAsync();
            _logger?.LogInformation("Learning database migrated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to migrate learning database");
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetDatabaseStatsAsync()
    {
        try
        {
            var stats = new Dictionary<string, object>
            {
                ["TotalRuns"] = await Runs.CountAsync(),
                ["TotalPatterns"] = await Patterns.CountAsync(),
                ["TotalImprovements"] = await Improvements.CountAsync(),
                ["TotalMetrics"] = await PerformanceMetrics.CountAsync(),
                ["TotalModels"] = await MLModels.CountAsync(),
                ["TotalPredictions"] = await Predictions.CountAsync(),
                ["TotalFeedback"] = await Feedback.CountAsync(),
                ["ActiveProtocols"] = await ProtocolVersions.CountAsync(pv => pv.IsActive),
                ["PendingUpdates"] = await ProtocolUpdates.CountAsync(pu => !pu.Applied),
                ["DatabaseSize"] = GetDatabaseSize(),
                ["LastRunDate"] = await Runs.MaxAsync(r => (DateTime?)r.Timestamp),
                ["SuccessRate"] = await CalculateOverallSuccessRateAsync()
            };

            return stats;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get database stats");
            return new Dictionary<string, object> { ["Error"] = ex.Message };
        }
    }

    private string GetDatabaseSize()
    {
        try
        {
            // This would need to be implemented differently for different database providers
            // For SQLite, we could check the file size
            // For SQL Server, we could query system tables
            return "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    private async Task<double> CalculateOverallSuccessRateAsync()
    {
        var totalRuns = await Runs.CountAsync();
        if (totalRuns == 0) return 0.0;
        
        var successfulRuns = await Runs.CountAsync(r => r.Success);
        return (double)successfulRuns / totalRuns;
    }
}
