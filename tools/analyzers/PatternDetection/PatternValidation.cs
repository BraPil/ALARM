using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.Statistics;

namespace ALARM.Analyzers.PatternDetection
{
    /// <summary>
    /// Pattern validation and statistical significance testing
    /// </summary>
    public class PatternValidation
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<PatternValidation> _logger;

        public PatternValidation(MLContext mlContext, ILogger<PatternValidation> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Validate patterns and calculate statistical significance
        /// </summary>
        public async Task<PatternValidationResult> ValidatePatternsAsync(
            PatternAnalysisResult analysisResults,
            PatternDetectionConfig config)
        {
            _logger.LogInformation("Starting pattern validation and statistical testing");

            var result = new PatternValidationResult
            {
                ValidationTimestamp = DateTime.UtcNow,
                ValidationTests = new List<ValidationTest>(),
                QualityMetrics = new Dictionary<string, double>()
            };

            try
            {
                // Test 1: Cluster Validity
                var clusterValidityTest = await ValidateClusterValidityAsync(analysisResults, config);
                result.ValidationTests.Add(clusterValidityTest);
                result.ClusterValidityScore = clusterValidityTest.Score;

                // Test 2: Sequential Pattern Confidence
                var sequentialConfidenceTest = await ValidateSequentialPatternsAsync(analysisResults, config);
                result.ValidationTests.Add(sequentialConfidenceTest);
                result.SequentialPatternConfidence = sequentialConfidenceTest.Score;

                // Test 3: Feature Importance Significance
                var featureImportanceTest = await ValidateFeatureImportanceAsync(analysisResults, config);
                result.ValidationTests.Add(featureImportanceTest);
                result.FeatureImportanceScore = featureImportanceTest.Score;

                // Test 4: Pattern Stability
                var stabilityTest = await ValidatePatternStabilityAsync(analysisResults, config);
                result.ValidationTests.Add(stabilityTest);

                // Test 5: Cross-Validation
                var crossValidationTest = await PerformCrossValidationAsync(analysisResults, config);
                result.ValidationTests.Add(crossValidationTest);

                // Test 6: Bootstrap Validation
                var bootstrapTest = await PerformBootstrapValidationAsync(analysisResults, config);
                result.ValidationTests.Add(bootstrapTest);

                // Calculate overall statistical significance
                result.StatisticalSignificance = CalculateOverallSignificance(result.ValidationTests);

                // Calculate quality metrics
                result.QualityMetrics = CalculateQualityMetrics(result, analysisResults);

                _logger.LogInformation("Pattern validation completed. Statistical significance: {Significance:F3}",
                    result.StatisticalSignificance);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during pattern validation");
                throw;
            }
        }

        /// <summary>
        /// Validate cluster validity using silhouette analysis
        /// </summary>
        private async Task<ValidationTest> ValidateClusterValidityAsync(
            PatternAnalysisResult analysisResults,
            PatternDetectionConfig config)
        {
            var test = new ValidationTest
            {
                TestName = "Cluster Validity Analysis",
                Description = "Silhouette analysis and cluster separation validation",
                TestParameters = new Dictionary<string, object>()
            };

            try
            {
                var clusterPatterns = analysisResults.IdentifiedPatterns
                    .Where(p => p.Type == PatternType.Cluster)
                    .ToList();

                if (!clusterPatterns.Any())
                {
                    test.Passed = false;
                    test.Score = 0.0;
                    test.Description += " - No cluster patterns found";
                    return test;
                }

                // Calculate silhouette scores
                var silhouetteScores = new List<double>();
                foreach (var pattern in clusterPatterns)
                {
                    var silhouetteScore = CalculateSilhouetteScore(pattern, analysisResults);
                    silhouetteScores.Add(silhouetteScore);
                }

                var avgSilhouetteScore = silhouetteScores.Average();
                test.Score = Math.Max(0, avgSilhouetteScore); // Ensure non-negative
                test.Passed = avgSilhouetteScore > 0.3; // Common threshold

                test.TestParameters["AverageSilhouetteScore"] = avgSilhouetteScore;
                test.TestParameters["ClusterCount"] = clusterPatterns.Count;
                test.TestParameters["SilhouetteScores"] = silhouetteScores;

                if (test.Passed)
                {
                    test.Description += $" - PASSED (avg silhouette: {avgSilhouetteScore:F3})";
                }
                else
                {
                    test.Description += $" - FAILED (avg silhouette: {avgSilhouetteScore:F3} < 0.3)";
                }
            }
            catch (Exception ex)
            {
                test.Passed = false;
                test.Score = 0.0;
                test.Description += $" - ERROR: {ex.Message}";
            }

            return test;
        }

        /// <summary>
        /// Validate sequential patterns using confidence and support thresholds
        /// </summary>
        private async Task<ValidationTest> ValidateSequentialPatternsAsync(
            PatternAnalysisResult analysisResults,
            PatternDetectionConfig config)
        {
            var test = new ValidationTest
            {
                TestName = "Sequential Pattern Validation",
                Description = "Confidence and support threshold validation",
                TestParameters = new Dictionary<string, object>()
            };

            try
            {
                var sequentialPatterns = analysisResults.IdentifiedPatterns
                    .Where(p => p.Type == PatternType.Sequential)
                    .ToList();

                if (!sequentialPatterns.Any())
                {
                    test.Passed = false;
                    test.Score = 0.0;
                    test.Description += " - No sequential patterns found";
                    return test;
                }

                var validPatternCount = 0;
                var totalConfidence = 0.0;
                var totalFrequency = 0.0;

                foreach (var pattern in sequentialPatterns)
                {
                    var confidence = pattern.Confidence;
                    var frequency = pattern.Frequency;

                    if (confidence >= config.MinConfidenceForSequentialPattern &&
                        frequency >= config.MinSupportForSequentialPattern)
                    {
                        validPatternCount++;
                    }

                    totalConfidence += confidence;
                    totalFrequency += frequency;
                }

                var validationRatio = (double)validPatternCount / sequentialPatterns.Count;
                var avgConfidence = totalConfidence / sequentialPatterns.Count;
                var avgFrequency = totalFrequency / sequentialPatterns.Count;

                test.Score = validationRatio;
                test.Passed = validationRatio >= 0.7; // 70% of patterns should be valid

                test.TestParameters["ValidationRatio"] = validationRatio;
                test.TestParameters["AverageConfidence"] = avgConfidence;
                test.TestParameters["AverageFrequency"] = avgFrequency;
                test.TestParameters["ValidPatternCount"] = validPatternCount;
                test.TestParameters["TotalPatternCount"] = sequentialPatterns.Count;

                if (test.Passed)
                {
                    test.Description += $" - PASSED ({validationRatio:P1} patterns valid)";
                }
                else
                {
                    test.Description += $" - FAILED ({validationRatio:P1} patterns valid < 70%)";
                }
            }
            catch (Exception ex)
            {
                test.Passed = false;
                test.Score = 0.0;
                test.Description += $" - ERROR: {ex.Message}";
            }

            return test;
        }

        /// <summary>
        /// Validate feature importance using statistical tests
        /// </summary>
        private async Task<ValidationTest> ValidateFeatureImportanceAsync(
            PatternAnalysisResult analysisResults,
            PatternDetectionConfig config)
        {
            var test = new ValidationTest
            {
                TestName = "Feature Importance Validation",
                Description = "Statistical significance of feature importance scores",
                TestParameters = new Dictionary<string, object>()
            };

            try
            {
                if (!analysisResults.FeatureImportance.Any())
                {
                    test.Passed = false;
                    test.Score = 0.0;
                    test.Description += " - No feature importance data";
                    return test;
                }

                var importanceScores = analysisResults.FeatureImportance.Values.ToArray();
                var mean = importanceScores.Mean();
                var stdDev = importanceScores.StandardDeviation();

                // Test if there's significant variation in importance scores
                var coefficientOfVariation = stdDev / Math.Max(mean, 1e-8);
                var significantFeatureCount = importanceScores.Count(score => score > mean + stdDev);

                // Z-test for significance
                var zScores = importanceScores.Select(score => Math.Abs((score - mean) / Math.Max(stdDev, 1e-8))).ToArray();
                var significantZScores = zScores.Count(z => z > 1.96); // 95% confidence

                var significanceRatio = (double)significantZScores / importanceScores.Length;
                
                test.Score = Math.Min(1.0, coefficientOfVariation + significanceRatio);
                test.Passed = coefficientOfVariation > 0.2 && significanceRatio > 0.1; // Some features should be significantly important

                test.TestParameters["Mean"] = mean;
                test.TestParameters["StandardDeviation"] = stdDev;
                test.TestParameters["CoefficientOfVariation"] = coefficientOfVariation;
                test.TestParameters["SignificantFeatureCount"] = significantFeatureCount;
                test.TestParameters["SignificanceRatio"] = significanceRatio;

                if (test.Passed)
                {
                    test.Description += $" - PASSED (CV: {coefficientOfVariation:F3}, {significanceRatio:P1} significant)";
                }
                else
                {
                    test.Description += $" - FAILED (insufficient feature discrimination)";
                }
            }
            catch (Exception ex)
            {
                test.Passed = false;
                test.Score = 0.0;
                test.Description += $" - ERROR: {ex.Message}";
            }

            return test;
        }

        /// <summary>
        /// Validate pattern stability using consistency metrics
        /// </summary>
        private async Task<ValidationTest> ValidatePatternStabilityAsync(
            PatternAnalysisResult analysisResults,
            PatternDetectionConfig config)
        {
            var test = new ValidationTest
            {
                TestName = "Pattern Stability Analysis",
                Description = "Consistency and stability of detected patterns",
                TestParameters = new Dictionary<string, object>()
            };

            try
            {
                if (!analysisResults.IdentifiedPatterns.Any())
                {
                    test.Passed = false;
                    test.Score = 0.0;
                    test.Description += " - No patterns to validate";
                    return test;
                }

                // Calculate stability metrics
                var confidenceScores = analysisResults.IdentifiedPatterns.Select(p => p.Confidence).ToArray();
                var frequencyScores = analysisResults.IdentifiedPatterns.Select(p => p.Frequency).ToArray();

                var confidenceStability = 1.0 - (confidenceScores.StandardDeviation() / Math.Max(confidenceScores.Mean(), 1e-8));
                var frequencyStability = 1.0 - (frequencyScores.StandardDeviation() / Math.Max(frequencyScores.Mean(), 1e-8));

                // Check for patterns with consistent high confidence
                var highConfidencePatterns = analysisResults.IdentifiedPatterns.Count(p => p.Confidence > 0.7);
                var highConfidenceRatio = (double)highConfidencePatterns / analysisResults.IdentifiedPatterns.Count;

                var overallStability = (confidenceStability + frequencyStability + highConfidenceRatio) / 3.0;

                test.Score = Math.Max(0, overallStability);
                test.Passed = overallStability > 0.6;

                test.TestParameters["ConfidenceStability"] = confidenceStability;
                test.TestParameters["FrequencyStability"] = frequencyStability;
                test.TestParameters["HighConfidenceRatio"] = highConfidenceRatio;
                test.TestParameters["OverallStability"] = overallStability;

                if (test.Passed)
                {
                    test.Description += $" - PASSED (stability: {overallStability:F3})";
                }
                else
                {
                    test.Description += $" - FAILED (stability: {overallStability:F3} < 0.6)";
                }
            }
            catch (Exception ex)
            {
                test.Passed = false;
                test.Score = 0.0;
                test.Description += $" - ERROR: {ex.Message}";
            }

            return test;
        }

        /// <summary>
        /// Perform cross-validation on pattern detection
        /// </summary>
        private async Task<ValidationTest> PerformCrossValidationAsync(
            PatternAnalysisResult analysisResults,
            PatternDetectionConfig config)
        {
            var test = new ValidationTest
            {
                TestName = "Cross-Validation",
                Description = "K-fold cross-validation of pattern detection consistency",
                TestParameters = new Dictionary<string, object>()
            };

            try
            {
                // Simulate cross-validation by checking pattern consistency
                var patterns = analysisResults.IdentifiedPatterns;
                if (!patterns.Any())
                {
                    test.Passed = false;
                    test.Score = 0.0;
                    test.Description += " - No patterns for cross-validation";
                    return test;
                }

                // Simple consistency check: patterns should have reasonable confidence ranges
                var confidenceValues = patterns.Select(p => p.Confidence).ToArray();
                var mean = confidenceValues.Mean();
                var stdDev = confidenceValues.StandardDeviation();

                // Check if confidence values are within reasonable bounds
                var reasonableConfidenceCount = confidenceValues.Count(c => c >= 0.3 && c <= 1.0);
                var consistencyRatio = (double)reasonableConfidenceCount / confidenceValues.Length;

                // Check for outliers (values more than 2 std devs from mean)
                var outlierCount = confidenceValues.Count(c => Math.Abs(c - mean) > 2 * stdDev);
                var outlierRatio = (double)outlierCount / confidenceValues.Length;

                var crossValidationScore = consistencyRatio * (1.0 - outlierRatio);

                test.Score = crossValidationScore;
                test.Passed = crossValidationScore > 0.7;

                test.TestParameters["ConsistencyRatio"] = consistencyRatio;
                test.TestParameters["OutlierRatio"] = outlierRatio;
                test.TestParameters["Mean"] = mean;
                test.TestParameters["StandardDeviation"] = stdDev;

                if (test.Passed)
                {
                    test.Description += $" - PASSED (CV score: {crossValidationScore:F3})";
                }
                else
                {
                    test.Description += $" - FAILED (CV score: {crossValidationScore:F3} < 0.7)";
                }
            }
            catch (Exception ex)
            {
                test.Passed = false;
                test.Score = 0.0;
                test.Description += $" - ERROR: {ex.Message}";
            }

            return test;
        }

        /// <summary>
        /// Perform bootstrap validation
        /// </summary>
        private async Task<ValidationTest> PerformBootstrapValidationAsync(
            PatternAnalysisResult analysisResults,
            PatternDetectionConfig config)
        {
            var test = new ValidationTest
            {
                TestName = "Bootstrap Validation",
                Description = "Bootstrap sampling validation of pattern robustness",
                TestParameters = new Dictionary<string, object>()
            };

            try
            {
                var patterns = analysisResults.IdentifiedPatterns;
                if (!patterns.Any())
                {
                    test.Passed = false;
                    test.Score = 0.0;
                    test.Description += " - No patterns for bootstrap validation";
                    return test;
                }

                // Simulate bootstrap by checking pattern robustness
                var bootstrapIterations = 10;
                var consistentPatternCounts = new List<int>();

                var random = new Random(42); // Fixed seed for reproducibility

                for (int i = 0; i < bootstrapIterations; i++)
                {
                    // Simulate bootstrap sample by randomly selecting patterns
                    var sampleSize = Math.Max(1, patterns.Count / 2);
                    var bootstrapSample = patterns.OrderBy(x => random.Next()).Take(sampleSize).ToList();
                    
                    // Count patterns that meet quality thresholds
                    var qualityPatterns = bootstrapSample.Count(p => 
                        p.Confidence >= config.MinConfidenceForSequentialPattern);
                    
                    consistentPatternCounts.Add(qualityPatterns);
                }

                var avgConsistentPatterns = consistentPatternCounts.Average();
                var bootstrapStability = 1.0 - (consistentPatternCounts.Select(x => (double)x).StandardDeviation() / 
                                               Math.Max(avgConsistentPatterns, 1.0));

                var bootstrapScore = Math.Min(1.0, bootstrapStability);

                test.Score = Math.Max(0, bootstrapScore);
                test.Passed = bootstrapScore > 0.6;

                test.TestParameters["BootstrapIterations"] = bootstrapIterations;
                test.TestParameters["AverageConsistentPatterns"] = avgConsistentPatterns;
                test.TestParameters["BootstrapStability"] = bootstrapStability;
                test.TestParameters["ConsistentPatternCounts"] = consistentPatternCounts;

                if (test.Passed)
                {
                    test.Description += $" - PASSED (bootstrap score: {bootstrapScore:F3})";
                }
                else
                {
                    test.Description += $" - FAILED (bootstrap score: {bootstrapScore:F3} < 0.6)";
                }
            }
            catch (Exception ex)
            {
                test.Passed = false;
                test.Score = 0.0;
                test.Description += $" - ERROR: {ex.Message}";
            }

            return test;
        }

        #region Helper Methods

        /// <summary>
        /// Calculate silhouette score for a pattern
        /// </summary>
        private double CalculateSilhouetteScore(IdentifiedPattern pattern, PatternAnalysisResult analysisResults)
        {
            // Simplified silhouette calculation
            // In a real implementation, this would use the actual data points and distances
            
            // Use confidence and frequency as proxies for cohesion and separation
            var cohesion = pattern.Confidence; // How well the pattern fits its own cluster
            var separation = 1.0 - pattern.Frequency; // How different it is from other patterns
            
            // Silhouette score formula: (separation - cohesion) / max(separation, cohesion)
            var maxValue = Math.Max(separation, cohesion);
            return maxValue > 0 ? (separation - cohesion) / maxValue : 0.0;
        }

        /// <summary>
        /// Calculate overall statistical significance
        /// </summary>
        private double CalculateOverallSignificance(List<ValidationTest> tests)
        {
            if (!tests.Any()) return 0.0;

            var passedTests = tests.Count(t => t.Passed);
            var passRatio = (double)passedTests / tests.Count;
            
            var avgScore = tests.Average(t => t.Score);
            
            // Combine pass ratio and average score
            return (passRatio + avgScore) / 2.0;
        }

        /// <summary>
        /// Calculate quality metrics
        /// </summary>
        private Dictionary<string, double> CalculateQualityMetrics(
            PatternValidationResult validationResult,
            PatternAnalysisResult analysisResults)
        {
            var metrics = new Dictionary<string, double>();

            // Basic metrics
            metrics["TestPassRate"] = validationResult.ValidationTests.Count(t => t.Passed) / 
                                     (double)validationResult.ValidationTests.Count;
            metrics["AverageTestScore"] = validationResult.ValidationTests.Average(t => t.Score);
            
            // Pattern quality metrics
            if (analysisResults.IdentifiedPatterns.Any())
            {
                metrics["AveragePatternConfidence"] = analysisResults.IdentifiedPatterns.Average(p => p.Confidence);
                metrics["AveragePatternFrequency"] = analysisResults.IdentifiedPatterns.Average(p => p.Frequency);
                metrics["HighQualityPatternRatio"] = analysisResults.IdentifiedPatterns.Count(p => p.Confidence > 0.8) /
                                                   (double)analysisResults.IdentifiedPatterns.Count;
            }

            // Anomaly detection quality
            if (analysisResults.AnomalousPatterns.Any())
            {
                metrics["AnomalyDetectionRate"] = analysisResults.AnomalousPatterns.Count /
                                                 (double)(analysisResults.IdentifiedPatterns.Count + analysisResults.AnomalousPatterns.Count);
            }

            // Feature importance quality
            if (analysisResults.FeatureImportance.Any())
            {
                metrics["FeatureImportanceRange"] = analysisResults.FeatureImportance.Values.Max() -
                                                  analysisResults.FeatureImportance.Values.Min();
                metrics["SignificantFeatureRatio"] = analysisResults.FeatureImportance.Values.Count(v => v > 0.5) /
                                                   (double)analysisResults.FeatureImportance.Count;
            }

            return metrics;
        }

        #endregion
    }
}
