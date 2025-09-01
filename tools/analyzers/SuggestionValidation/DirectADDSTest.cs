using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Direct ADDS Training Data Generation Test with Enhanced Monitoring
    /// </summary>
    public class DirectADDSTest
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 ALARM Phase 2 - Direct ADDS Training Data Generation Test");
            Console.WriteLine("============================================================");
            Console.WriteLine($"Start Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Process ID: {System.Diagnostics.Process.GetCurrentProcess().Id}");
            Console.WriteLine();
            
            try
            {
                // Create logger with detailed output
                using var loggerFactory = LoggerFactory.Create(builder => 
                    builder.AddConsole()
                           .SetMinimumLevel(LogLevel.Debug)
                           .AddFilter("Microsoft", LogLevel.Warning));
                
                var featureExtractorLogger = loggerFactory.CreateLogger<EnhancedFeatureExtractor>();
                var generatorLogger = loggerFactory.CreateLogger<ADDSTrainingDataGenerator>();
                
                Console.WriteLine("📊 Step 1: Initializing Enhanced Feature Extractor...");
                var featureExtractor = new EnhancedFeatureExtractor(featureExtractorLogger);
                Console.WriteLine("✅ Enhanced Feature Extractor initialized");
                
                Console.WriteLine("📊 Step 2: Initializing ADDS Training Data Generator...");
                var trainingDataGenerator = new ADDSTrainingDataGenerator(generatorLogger, featureExtractor);
                Console.WriteLine("✅ ADDS Training Data Generator initialized");
                
                Console.WriteLine("📊 Step 3: Generating ADDS training samples...");
                Console.WriteLine("   Target: 250 samples across 10 categories");
                Console.WriteLine("   Expected time: 2-5 minutes");
                Console.WriteLine();
                
                var startTime = DateTime.Now;
                var trainingData = await trainingDataGenerator.GenerateADDSTrainingDataAsync(250);
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                
                Console.WriteLine($"✅ SUCCESS! Generated {trainingData.Count} training samples");
                Console.WriteLine($"⏱️  Generation time: {duration.TotalSeconds:F2} seconds");
                Console.WriteLine();
                
                // Quick analysis
                Console.WriteLine("📈 Quick Quality Analysis:");
                var highQuality = trainingData.Count(x => x.ActualQualityScore >= 0.85);
                var mediumQuality = trainingData.Count(x => x.ActualQualityScore >= 0.65 && x.ActualQualityScore < 0.85);
                var lowQuality = trainingData.Count(x => x.ActualQualityScore < 0.65);
                
                Console.WriteLine($"   High Quality (85%+): {highQuality} samples ({highQuality * 100.0 / trainingData.Count:F1}%)");
                Console.WriteLine($"   Medium Quality (65-84%): {mediumQuality} samples ({mediumQuality * 100.0 / trainingData.Count:F1}%)");
                Console.WriteLine($"   Low Quality (<65%): {lowQuality} samples ({lowQuality * 100.0 / trainingData.Count:F1}%)");
                Console.WriteLine();
                
                // Feature extraction test
                Console.WriteLine("🧠 Testing Feature Extraction on Sample...");
                var testSample = trainingData.FirstOrDefault();
                if (testSample != null)
                {
                    var features = await featureExtractor.ExtractFeaturesAsync(testSample.SuggestionText, testSample.Context);
                    var featureDict = features.ToFeatureDictionary();
                    
                    Console.WriteLine($"   Sample Text: {testSample.SuggestionText.Substring(0, Math.Min(100, testSample.SuggestionText.Length))}...");
                    Console.WriteLine($"   Quality Score: {testSample.ActualQualityScore:F2}");
                    Console.WriteLine($"   Features Extracted: {featureDict.Count}");
                    Console.WriteLine($"   Analysis Type: {testSample.AnalysisType}");
                    
                    // Show top features
                    Console.WriteLine("   Top 5 Features:");
                    foreach (var feature in featureDict.OrderByDescending(x => Math.Abs(x.Value)).Take(5))
                    {
                        Console.WriteLine($"     {feature.Key}: {feature.Value:F3}");
                    }
                }
                
                Console.WriteLine();
                Console.WriteLine("🎉 ADDS Training Data Generation SUCCESSFUL!");
                Console.WriteLine($"   Ready for Advanced ML Model Training");
                Console.WriteLine($"   Target: 85%+ quality score accuracy");
                Console.WriteLine($"   End Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}

