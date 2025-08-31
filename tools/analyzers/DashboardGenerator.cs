using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ALARM.Analyzers;

public class DashboardGenerator
{
    private readonly ILogger<DashboardGenerator> _logger;

    public DashboardGenerator(ILogger<DashboardGenerator> logger)
    {
        _logger = logger;
    }

    public async Task GenerateLearningDashboardAsync(string outputPath, List<HistoricalRunData> historicalData, 
        PatternAnalysisResult patternAnalysis, PerformanceAnalysis performanceAnalysis)
    {
        _logger.LogInformation("Generating learning dashboard...");

        var html = GenerateDashboardHtml(historicalData, patternAnalysis, performanceAnalysis);
        await File.WriteAllTextAsync(outputPath, html);

        _logger.LogInformation("Learning dashboard generated: {OutputPath}", outputPath);
    }

    private string GenerateDashboardHtml(List<HistoricalRunData> historicalData, 
        PatternAnalysisResult patternAnalysis, PerformanceAnalysis performanceAnalysis)
    {
        var successRateData = GenerateSuccessRateChartData(historicalData);
        var performanceData = GeneratePerformanceChartData(performanceAnalysis);
        var patternData = GeneratePatternData(patternAnalysis);

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>ALARM Learning Dashboard</title>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js""></script>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            border-radius: 10px;
            margin-bottom: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 2.5em;
        }}
        .header p {{
            margin: 10px 0 0 0;
            opacity: 0.9;
        }}
        .dashboard-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }}
        .card {{
            background: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }}
        .card h2 {{
            margin-top: 0;
            color: #333;
            border-bottom: 2px solid #667eea;
            padding-bottom: 10px;
        }}
        .metric {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 10px 0;
            border-bottom: 1px solid #eee;
        }}
        .metric:last-child {{
            border-bottom: none;
        }}
        .metric-value {{
            font-weight: bold;
            font-size: 1.2em;
        }}
        .success {{ color: #28a745; }}
        .warning {{ color: #ffc107; }}
        .danger {{ color: #dc3545; }}
        .info {{ color: #17a2b8; }}
        .chart-container {{
            position: relative;
            height: 300px;
            margin-top: 20px;
        }}
        .pattern-list {{
            list-style: none;
            padding: 0;
        }}
        .pattern-list li {{
            padding: 8px 0;
            border-bottom: 1px solid #eee;
        }}
        .pattern-list li:last-child {{
            border-bottom: none;
        }}
        .pattern-success {{
            background-color: #d4edda;
            border-left: 4px solid #28a745;
            padding-left: 10px;
        }}
        .pattern-failure {{
            background-color: #f8d7da;
            border-left: 4px solid #dc3545;
            padding-left: 10px;
        }}
        .recommendations {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin-top: 20px;
        }}
        .footer {{
            text-align: center;
            color: #666;
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #ddd;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>🤖 ALARM Learning Dashboard</h1>
        <p>Automated Legacy App Refactoring - Continuous Improvement Analytics</p>
        <p>Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
    </div>

    <div class=""dashboard-grid"">
        <div class=""card"">
            <h2>📊 Key Metrics</h2>
            <div class=""metric"">
                <span>Success Rate</span>
                <span class=""metric-value {GetSuccessRateClass(patternAnalysis.SuccessRate)}"">{patternAnalysis.SuccessRate:P1}</span>
            </div>
            <div class=""metric"">
                <span>Total Runs Analyzed</span>
                <span class=""metric-value info"">{patternAnalysis.TotalRuns}</span>
            </div>
            <div class=""metric"">
                <span>Success Patterns Found</span>
                <span class=""metric-value success"">{patternAnalysis.SuccessPatterns.Count}</span>
            </div>
            <div class=""metric"">
                <span>Failure Patterns Found</span>
                <span class=""metric-value {(patternAnalysis.FailurePatterns.Count > 5 ? "danger" : "warning")}"">{patternAnalysis.FailurePatterns.Count}</span>
            </div>
            <div class=""metric"">
                <span>Performance Regressions</span>
                <span class=""metric-value {(performanceAnalysis.Regressions.Count > 0 ? "danger" : "success")}"">{performanceAnalysis.Regressions.Count}</span>
            </div>
        </div>

        <div class=""card"">
            <h2>📈 Success Rate Trend</h2>
            <div class=""chart-container"">
                <canvas id=""successRateChart""></canvas>
            </div>
        </div>

        <div class=""card"">
            <h2>⚡ Performance Trends</h2>
            <div class=""chart-container"">
                <canvas id=""performanceChart""></canvas>
            </div>
        </div>

        <div class=""card"">
            <h2>✅ Success Patterns</h2>
            <ul class=""pattern-list"">
                {string.Join("", patternAnalysis.SuccessPatterns.Take(10).Select(p => $"<li class=\"pattern-success\">{p}</li>"))}
            </ul>
        </div>

        <div class=""card"">
            <h2>❌ Failure Patterns</h2>
            <ul class=""pattern-list"">
                {string.Join("", patternAnalysis.FailurePatterns.Take(10).Select(p => $"<li class=\"pattern-failure\">{p}</li>"))}
            </ul>
        </div>

        <div class=""card"">
            <h2>🎯 Key Insights</h2>
            <ul class=""pattern-list"">
                {string.Join("", patternAnalysis.Insights.Select(i => $"<li>{i}</li>"))}
            </ul>
            {(patternAnalysis.Insights.Count == 0 ? "<p>No specific insights generated. System is performing within normal parameters.</p>" : "")}
        </div>
    </div>

    <div class=""recommendations"">
        <h3>🚀 Recommended Actions</h3>
        {GenerateRecommendations(patternAnalysis, performanceAnalysis)}
    </div>

    <div class=""footer"">
        <p>ALARM Learning Dashboard - Powered by Machine Learning and Pattern Recognition</p>
        <p>Data retention: Learning data preserved for 90 days, run artifacts for 30 days</p>
    </div>

    <script>
        // Success Rate Chart
        const successRateCtx = document.getElementById('successRateChart').getContext('2d');
        new Chart(successRateCtx, {{
            type: 'line',
            data: {successRateData},
            options: {{
                responsive: true,
                maintainAspectRatio: false,
                scales: {{
                    y: {{
                        beginAtZero: true,
                        max: 100,
                        ticks: {{
                            callback: function(value) {{
                                return value + '%';
                            }}
                        }}
                    }}
                }},
                plugins: {{
                    title: {{
                        display: true,
                        text: 'Success Rate Over Time'
                    }}
                }}
            }}
        }});

        // Performance Chart
        const performanceCtx = document.getElementById('performanceChart').getContext('2d');
        new Chart(performanceCtx, {{
            type: 'line',
            data: {performanceData},
            options: {{
                responsive: true,
                maintainAspectRatio: false,
                scales: {{
                    y: {{
                        beginAtZero: true
                    }}
                }},
                plugins: {{
                    title: {{
                        display: true,
                        text: 'Performance Metrics Over Time'
                    }}
                }}
            }}
        }});
    </script>
</body>
</html>";
    }

    private string GetSuccessRateClass(double successRate)
    {
        return successRate switch
        {
            >= 0.9 => "success",
            >= 0.7 => "warning",
            _ => "danger"
        };
    }

    private string GenerateSuccessRateChartData(List<HistoricalRunData> historicalData)
    {
        if (!historicalData.Any())
        {
            return @"{
                labels: ['No Data'],
                datasets: [{
                    label: 'Success Rate',
                    data: [0],
                    borderColor: 'rgb(75, 192, 192)',
                    tension: 0.1
                }]
            }";
        }

        var sortedData = historicalData.OrderBy(r => r.Timestamp).ToList();
        var labels = sortedData.Select(r => r.Timestamp.ToString("MM/dd")).ToList();
        
        // Calculate rolling success rate
        var successRates = new List<double>();
        var windowSize = Math.Min(5, sortedData.Count);
        
        for (int i = 0; i < sortedData.Count; i++)
        {
            var windowStart = Math.Max(0, i - windowSize + 1);
            var windowData = sortedData.Skip(windowStart).Take(i - windowStart + 1);
            var successCount = windowData.Count(IsSuccessfulRun);
            var successRate = (double)successCount / windowData.Count() * 100;
            successRates.Add(successRate);
        }

        return JsonSerializer.Serialize(new
        {
            labels = labels,
            datasets = new[]
            {
                new
                {
                    label = "Success Rate (%)",
                    data = successRates,
                    borderColor = "rgb(75, 192, 192)",
                    backgroundColor = "rgba(75, 192, 192, 0.2)",
                    tension = 0.1
                }
            }
        });
    }

    private string GeneratePerformanceChartData(PerformanceAnalysis performanceAnalysis)
    {
        if (!performanceAnalysis.HistoricalMetrics.Any())
        {
            return @"{
                labels: ['No Data'],
                datasets: [{
                    label: 'Performance',
                    data: [0],
                    borderColor: 'rgb(255, 99, 132)',
                    tension: 0.1
                }]
            }";
        }

        var sortedMetrics = performanceAnalysis.HistoricalMetrics.OrderBy(kv => kv.Key).ToList();
        var labels = sortedMetrics.Select(kv => kv.Key.ToString("MM/dd")).ToList();

        // Get the most common performance metric
        var commonMetric = sortedMetrics
            .SelectMany(kv => kv.Value.Keys)
            .GroupBy(k => k)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? "Duration";

        var values = sortedMetrics
            .Select(kv => kv.Value.GetValueOrDefault(commonMetric, 0))
            .ToList();

        return JsonSerializer.Serialize(new
        {
            labels = labels,
            datasets = new[]
            {
                new
                {
                    label = $"{commonMetric}",
                    data = values,
                    borderColor = "rgb(255, 99, 132)",
                    backgroundColor = "rgba(255, 99, 132, 0.2)",
                    tension = 0.1
                }
            }
        });
    }

    private string GeneratePatternData(PatternAnalysisResult patternAnalysis)
    {
        return JsonSerializer.Serialize(new
        {
            successPatterns = patternAnalysis.SuccessPatterns,
            failurePatterns = patternAnalysis.FailurePatterns,
            insights = patternAnalysis.Insights
        });
    }

    private string GenerateRecommendations(PatternAnalysisResult patternAnalysis, PerformanceAnalysis performanceAnalysis)
    {
        var recommendations = new List<string>();

        if (patternAnalysis.SuccessRate < 0.8)
        {
            recommendations.Add("🔴 <strong>Critical:</strong> Success rate is below 80%. Review failure patterns and implement fixes.");
        }

        if (patternAnalysis.FailurePatterns.Count > 5)
        {
            recommendations.Add("🟡 <strong>Warning:</strong> Multiple failure patterns detected. Consider systematic refactoring approach.");
        }

        if (performanceAnalysis.Regressions.Any())
        {
            recommendations.Add($"⚡ <strong>Performance:</strong> {performanceAnalysis.Regressions.Count} performance regression(s) detected. Review recent changes.");
        }

        if (patternAnalysis.Trends.TryGetValue("SuccessRateTrend", out var trendValue) && trendValue is double trend && trend > 0.1)
        {
            recommendations.Add("✅ <strong>Good News:</strong> Success rate is improving. Current practices are effective.");
        }

        if (!recommendations.Any())
        {
            recommendations.Add("✅ <strong>All Good:</strong> System is performing well. Continue current practices and monitor trends.");
        }

        return string.Join("<br>", recommendations.Select(r => $"<p>{r}</p>"));
    }

    private bool IsSuccessfulRun(HistoricalRunData runData)
    {
        // Simplified success determination for dashboard
        if (runData.TestResults.Any())
        {
            foreach (var testResult in runData.TestResults.Values)
            {
                if (testResult.TryGetValue("success", out var successValue) && 
                    successValue is JsonElement success && 
                    success.ValueKind == JsonValueKind.False)
                {
                    return false;
                }
            }
        }

        return true; // Default to success if no clear failure indicators
    }
}
