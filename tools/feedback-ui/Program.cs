using ALARM.FeedbackUI.Services;
using ALARM.FeedbackUI.Models;
using ALARM.DataPersistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5000", "http://localhost:8080")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Entity Framework with SQLite
builder.Services.AddDbContext<FeedbackDataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                      "Data Source=alarm_feedback.db"));

// Add custom services
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IFeedbackAnalyticsService, FeedbackAnalyticsService>();
builder.Services.AddScoped<ILearningIntegrationService, LearningIntegrationService>();

// Add background service for processing feedback
builder.Services.AddFeedbackBackgroundService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Serve the feedback UI
app.MapGet("/", () => Results.Redirect("/feedback"));
app.MapGet("/feedback", () => Results.Content(GetFeedbackHTML(), "text/html"));

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FeedbackDataContext>();
    await context.Database.EnsureCreatedAsync();
}

Console.WriteLine("🎯 ALARM Feedback UI Server starting...");
Console.WriteLine("📊 Feedback collection interface available at: http://localhost:5000/feedback");
Console.WriteLine("🔧 API documentation available at: http://localhost:5000/swagger");

app.Run();

static string GetFeedbackHTML()
{
    return """
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>ALARM Feedback Collection System</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { max-width: 800px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; }
        .header { text-align: center; margin-bottom: 30px; }
        .form-section { margin-bottom: 30px; padding: 20px; border: 1px solid #ddd; border-radius: 5px; }
        .form-group { margin-bottom: 15px; }
        label { display: block; margin-bottom: 5px; font-weight: bold; }
        input, select, textarea { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
        button { background: #007bff; color: white; padding: 10px 20px; border: none; border-radius: 4px; cursor: pointer; }
        button:hover { background: #0056b3; }
        .analytics { display: flex; gap: 20px; margin-top: 20px; }
        .metric { flex: 1; text-align: center; padding: 15px; background: #e9ecef; border-radius: 5px; }
        .success { background: #d4edda; color: #155724; padding: 10px; border-radius: 4px; margin-bottom: 20px; display: none; }
        .error { background: #f8d7da; color: #721c24; padding: 10px; border-radius: 4px; margin-bottom: 20px; display: none; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🎯 ALARM Feedback Collection</h1>
            <p>Help us improve by sharing your experience with ALARM</p>
        </div>
        
        <div class="success" id="successMessage">✅ Thank you! Your feedback has been recorded.</div>
        <div class="error" id="errorMessage">❌ Error submitting feedback. Please try again.</div>
        
        <div class="form-section">
            <h3>📊 Analysis Feedback</h3>
            <form id="analysisFeedbackForm">
                <div class="form-group">
                    <label for="analysisType">Analysis Type</label>
                    <select id="analysisType" name="analysisType" required>
                        <option value="">Select type...</option>
                        <option value="pattern-detection">Pattern Detection</option>
                        <option value="causal-analysis">Causal Analysis</option>
                        <option value="risk-assessment">Risk Assessment</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="accuracy">Accuracy (1-5)</label>
                    <input type="number" id="accuracy" min="1" max="5" />
                </div>
                <div class="form-group">
                    <label for="comments">Comments</label>
                    <textarea id="comments" rows="3"></textarea>
                </div>
                <button type="submit">Submit Feedback</button>
            </form>
        </div>
        
        <div class="analytics">
            <div class="metric">
                <div id="totalFeedback">0</div>
                <div>Total Feedback</div>
            </div>
            <div class="metric">
                <div id="avgAccuracy">0.0</div>
                <div>Avg Accuracy</div>
            </div>
        </div>
    </div>

    <script>
        document.getElementById('analysisFeedbackForm').addEventListener('submit', async (e) => {
            e.preventDefault();
            const data = {
                feedbackType: 'analysis',
                analysisType: document.getElementById('analysisType').value,
                accuracy: parseInt(document.getElementById('accuracy').value),
                comments: document.getElementById('comments').value
            };
            await submitFeedback(data);
        });

        async function submitFeedback(data) {
            try {
                const response = await fetch('/api/feedback', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(data)
                });
                if (response.ok) {
                    document.getElementById('successMessage').style.display = 'block';
                    document.getElementById('analysisFeedbackForm').reset();
                    loadAnalytics();
                } else {
                    throw new Error('Failed');
                }
            } catch (error) {
                document.getElementById('errorMessage').style.display = 'block';
            }
        }

        async function loadAnalytics() {
            try {
                const response = await fetch('/api/feedback/analytics');
                if (response.ok) {
                    const analytics = await response.json();
                    document.getElementById('totalFeedback').textContent = analytics.totalFeedback || 0;
                    document.getElementById('avgAccuracy').textContent = (analytics.averageAccuracy || 0).toFixed(1);
                }
            } catch (error) {
                console.error('Error loading analytics:', error);
            }
        }

        loadAnalytics();
    </script>
</body>
</html>
""";
}

// Make Program class accessible for testing
public partial class Program { }
