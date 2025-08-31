using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using ALARM.Analyzers.SuggestionValidation;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// MLFlow experiment tracking service for suggestion validation quality improvements
    /// </summary>
    public class MLFlowExperimentTracker
    {
        private readonly ILogger<MLFlowExperimentTracker> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _experimentName;
        private readonly string _trackingUri;
        private string? _currentRunId;
        private string? _experimentId;
        private readonly bool _isLocalMode;

        public MLFlowExperimentTracker(ILogger<MLFlowExperimentTracker> logger, string? mlflowTrackingUri = null)
        {
            _logger = logger;
            _experimentName = "ALARM-Suggestion-Validation-Quality-Improvement";
            _trackingUri = mlflowTrackingUri ?? "http://localhost:5000";
            _httpClient = new HttpClient();
            
            // For now, default to local mode (can be enhanced later with server connectivity)
            _isLocalMode = true;
            _logger.LogInformation("MLFlow tracker initialized in local logging mode for quality tracking");
        }

        private async Task<bool> IsMLFlowServerAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_trackingUri}/health", CancellationToken.None);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Initialize or get existing experiment
        /// </summary>
        public async Task<string> InitializeExperimentAsync()
        {
            if (_isLocalMode)
            {
                _experimentId = "local-experiment";
                _logger.LogInformation("Using local experiment tracking: {ExperimentName}", _experimentName);
                return _experimentId;
            }

            // Future: Add server-based experiment initialization
            _experimentId = "local-experiment";
            return _experimentId;
        }

        /// <summary>
        /// Start a new MLFlow run for tracking quality improvements
        /// </summary>
        public async Task<string> StartRunAsync(string runName, Dictionary<string, string>? tags = null)
        {
            if (string.IsNullOrEmpty(_experimentId))
            {
                await InitializeExperimentAsync();
            }

            _currentRunId = $"local-run-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
            
            _logger.LogInformation("Started quality tracking run: {RunName} (ID: {RunId})", runName, _currentRunId);
            
            if (tags != null)
            {
                _logger.LogInformation("Run tags: {Tags}", JsonSerializer.Serialize(tags));
            }
            
            return _currentRunId;
        }

        /// <summary>
        /// Log quality improvement parameters
        /// </summary>
        public async Task LogParametersAsync(Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(_currentRunId))
            {
                _logger.LogWarning("No active run. Cannot log parameters.");
                return;
            }

            _logger.LogInformation("Quality Parameters [{RunId}]: {Parameters}", _currentRunId, JsonSerializer.Serialize(parameters));
            await Task.CompletedTask;
        }

        /// <summary>
        /// Log quality metrics and improvements
        /// </summary>
        public async Task LogQualityMetricsAsync(Dictionary<string, double> metrics)
        {
            if (string.IsNullOrEmpty(_currentRunId))
            {
                _logger.LogWarning("No active run. Cannot log metrics.");
                return;
            }

            _logger.LogInformation("Quality Metrics [{RunId}]: {Metrics}", _currentRunId, JsonSerializer.Serialize(metrics));
            await Task.CompletedTask;
        }

        /// <summary>
        /// Log quality improvement comparison
        /// </summary>
        public async Task LogQualityComparisonAsync(string analysisType, double beforeScore, double afterScore, Dictionary<string, object>? additionalData = null)
        {
            var improvement = afterScore - beforeScore;
            var improvementPercent = beforeScore > 0 ? (improvement / beforeScore) * 100 : 0;

            _logger.LogInformation("🎯 Quality Improvement - {AnalysisType}: {Before:F2}% → {After:F2}% ({Improvement:+F2}%)", 
                analysisType, beforeScore * 100, afterScore * 100, improvementPercent);

            if (additionalData != null)
            {
                _logger.LogInformation("Additional Data: {Data}", JsonSerializer.Serialize(additionalData));
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Log algorithm configuration changes
        /// </summary>
        public async Task LogAlgorithmChangesAsync(string algorithmName, Dictionary<string, object> changes)
        {
            _logger.LogInformation("🔧 Algorithm Changes - {AlgorithmName}: {Changes}", 
                algorithmName, JsonSerializer.Serialize(changes));
            await Task.CompletedTask;
        }

        /// <summary>
        /// End the current MLFlow run
        /// </summary>
        public async Task EndRunAsync(string status = "FINISHED")
        {
            if (string.IsNullOrEmpty(_currentRunId))
            {
                _logger.LogWarning("No active run to end.");
                return;
            }

            _logger.LogInformation("✅ Ended quality tracking run {RunId} with status: {Status}", _currentRunId, status);
            _currentRunId = null;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Log phase completion with comprehensive metrics
        /// </summary>
        public async Task LogPhaseCompletionAsync(string phaseName, Dictionary<string, double> phaseMetrics, TimeSpan duration)
        {
            _logger.LogInformation("🎉 Phase {PhaseName} completed in {Duration} with {MetricsCount} metrics", 
                phaseName, duration, phaseMetrics.Count);
            _logger.LogInformation("Phase Metrics: {Metrics}", JsonSerializer.Serialize(phaseMetrics));
            await Task.CompletedTask;
        }

        /// <summary>
        /// Create a new run for A/B testing different scoring algorithms
        /// </summary>
        public async Task<string> StartABTestRunAsync(string testName, string variant, Dictionary<string, object> variantConfig)
        {
            var tags = new Dictionary<string, string>
            {
                ["test_type"] = "ab_test",
                ["test_name"] = testName,
                ["variant"] = variant,
                ["experiment_phase"] = "quality_improvement"
            };

            var runName = $"{testName}_variant_{variant}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
            var runId = await StartRunAsync(runName, tags);

            _logger.LogInformation("🧪 A/B Test Configuration: {Config}", JsonSerializer.Serialize(variantConfig));
            return runId;
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(_currentRunId))
            {
                Task.Run(async () => await EndRunAsync("FINISHED"));
            }
            _httpClient?.Dispose();
        }
    }
}
