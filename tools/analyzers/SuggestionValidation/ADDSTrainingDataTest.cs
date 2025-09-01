using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ALARM.Analyzers.SuggestionValidation
{
    /// <summary>
    /// Test program to generate ADDS training data for Phase 2 Advanced ML model training
    /// </summary>
    public class ADDSTrainingDataTest
    {
        public static async Task RunADDSTrainingDataGenerationAsync()
        {
            Console.WriteLine("🚀 ALARM Phase 2 - ADDS Training Data Generation");
            Console.WriteLine("==============================================");
            
            // Create logger
            using var loggerFactory = LoggerFactory.Create(builder => 
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            
            var featureExtractorLogger = loggerFactory.CreateLogger<EnhancedFeatureExtractor>();
            var generatorLogger = loggerFactory.CreateLogger<ADDSTrainingDataGenerator>();
            
            try
            {
                // Initialize components
                var featureExtractor = new EnhancedFeatureExtractor(featureExtractorLogger);
                var trainingDataGenerator = new ADDSTrainingDataGenerator(generatorLogger, featureExtractor);
                
                Console.WriteLine("📊 Generating 250 ADDS training samples...");
                
                // Generate training data
                var trainingData = await trainingDataGenerator.GenerateADDSTrainingDataAsync(250);
                
                Console.WriteLine($"✅ Generated {trainingData.Count} training samples successfully!");
                Console.WriteLine();
                
                // Display sample breakdown
                Console.WriteLine("📋 Training Data Breakdown:");
                Console.WriteLine("---------------------------");
                
                var categoryBreakdown = new Dictionary<ADDSSuggestionCategory, int>();
                foreach (var sample in trainingData)
                {
                    var category = DetermineCategoryFromText(sample.SuggestionText);
                    categoryBreakdown[category] = categoryBreakdown.GetValueOrDefault(category, 0) + 1;
                }
                
                foreach (var category in categoryBreakdown)
                {
                    Console.WriteLine($"{category.Key}: {category.Value} samples");
                }
                
                Console.WriteLine();
                
                // Display quality distribution
                Console.WriteLine("📈 Quality Score Distribution:");
                Console.WriteLine("------------------------------");
                
                var highQuality = trainingData.Count(x => x.ActualQualityScore >= 0.85);
                var mediumQuality = trainingData.Count(x => x.ActualQualityScore >= 0.65 && x.ActualQualityScore < 0.85);
                var lowQuality = trainingData.Count(x => x.ActualQualityScore < 0.65);
                
                Console.WriteLine($"High Quality (85%+): {highQuality} samples ({highQuality * 100.0 / trainingData.Count:F1}%)");
                Console.WriteLine($"Medium Quality (65-84%): {mediumQuality} samples ({mediumQuality * 100.0 / trainingData.Count:F1}%)");
                Console.WriteLine($"Low Quality (<65%): {lowQuality} samples ({lowQuality * 100.0 / trainingData.Count:F1}%)");
                
                Console.WriteLine();
                
                // Display sample examples
                Console.WriteLine("🔍 Sample Examples:");
                Console.WriteLine("-------------------");
                
                var highQualitySample = trainingData.Where(x => x.ActualQualityScore >= 0.85).FirstOrDefault();
                if (highQualitySample != null)
                {
                    Console.WriteLine($"HIGH QUALITY ({highQualitySample.ActualQualityScore:F2}):");
                    Console.WriteLine($"  {highQualitySample.SuggestionText.Substring(0, Math.Min(100, highQualitySample.SuggestionText.Length))}...");
                    Console.WriteLine($"  Analysis Type: {highQualitySample.AnalysisType}");
                    Console.WriteLine();
                }
                
                var mediumQualitySample = trainingData.Where(x => x.ActualQualityScore >= 0.65 && x.ActualQualityScore < 0.85).FirstOrDefault();
                if (mediumQualitySample != null)
                {
                    Console.WriteLine($"MEDIUM QUALITY ({mediumQualitySample.ActualQualityScore:F2}):");
                    Console.WriteLine($"  {mediumQualitySample.SuggestionText.Substring(0, Math.Min(100, mediumQualitySample.SuggestionText.Length))}...");
                    Console.WriteLine($"  Analysis Type: {mediumQualitySample.AnalysisType}");
                    Console.WriteLine();
                }
                
                var lowQualitySample = trainingData.Where(x => x.ActualQualityScore < 0.65).FirstOrDefault();
                if (lowQualitySample != null)
                {
                    Console.WriteLine($"LOW QUALITY ({lowQualitySample.ActualQualityScore:F2}):");
                    Console.WriteLine($"  {lowQualitySample.SuggestionText.Substring(0, Math.Min(100, lowQualitySample.SuggestionText.Length))}...");
                    Console.WriteLine($"  Analysis Type: {lowQualitySample.AnalysisType}");
                    Console.WriteLine();
                }
                
                // Test feature extraction on sample
                Console.WriteLine("🧠 Feature Extraction Test:");
                Console.WriteLine("---------------------------");
                
                var testSample = trainingData.FirstOrDefault();
                if (testSample != null)
                {
                    var features = await featureExtractor.ExtractFeaturesAsync(testSample.SuggestionText, testSample.Context);
                    var featureDict = features.ToFeatureDictionary();
                    
                    Console.WriteLine($"Sample: {testSample.SuggestionText.Substring(0, Math.Min(80, testSample.SuggestionText.Length))}...");
                    Console.WriteLine($"Extracted {featureDict.Count} features:");
                    
                    foreach (var feature in featureDict.Take(10))
                    {
                        Console.WriteLine($"  {feature.Key}: {feature.Value:F3}");
                    }
                    
                    Console.WriteLine($"  ... and {featureDict.Count - 10} more features");
                }
                
                Console.WriteLine();
                Console.WriteLine("🎉 ADDS Training Data Generation Complete!");
                Console.WriteLine($"   Ready for Advanced ML Model Training with {trainingData.Count} samples");
                Console.WriteLine($"   Target: 85%+ quality score accuracy with neural networks and ensemble methods");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during training data generation: {ex.Message}");
                Console.WriteLine($"   Stack Trace: {ex.StackTrace}");
                throw;
            }
        }
        
        private static ADDSSuggestionCategory DetermineCategoryFromText(string suggestionText)
        {
            var text = suggestionText.ToLower();
            
            if (text.Contains("launcher") || text.Contains("bat") || text.Contains("powershell"))
                return ADDSSuggestionCategory.LauncherMigration;
            if (text.Contains("database") || text.Contains("oracle") || text.Contains("connection"))
                return ADDSSuggestionCategory.DatabaseIntegration;
            if (text.Contains("autocad") || text.Contains("cuix") || text.Contains("command"))
                return ADDSSuggestionCategory.AutoCADIntegration;
            if (text.Contains("map3d") || text.Contains("spatial") || text.Contains("coordinate"))
                return ADDSSuggestionCategory.Map3DIntegration;
            if (text.Contains("performance") || text.Contains("optimization") || text.Contains("speed"))
                return ADDSSuggestionCategory.PerformanceOptimization;
            if (text.Contains("security") || text.Contains("credential") || text.Contains("authentication"))
                return ADDSSuggestionCategory.SecurityEnhancement;
            if (text.Contains("file") || text.Contains("path") || text.Contains("directory"))
                return ADDSSuggestionCategory.FileSystemMigration;
            if (text.Contains("interface") || text.Contains("ui") || text.Contains("moderniz"))
                return ADDSSuggestionCategory.UserInterfaceModernization;
            if (text.Contains("config") || text.Contains("setting") || text.Contains("environment"))
                return ADDSSuggestionCategory.ConfigurationManagement;
            if (text.Contains("error") || text.Contains("exception") || text.Contains("handling"))
                return ADDSSuggestionCategory.ErrorHandling;
                
            return ADDSSuggestionCategory.LauncherMigration; // Default
        }
    }
}

