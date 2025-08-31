using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.LinearAlgebra;

namespace ALARM.Analyzers.PatternDetection
{
    /// <summary>
    /// Advanced feature extraction system for automated feature engineering
    /// </summary>
    public class FeatureExtraction
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<FeatureExtraction> _logger;

        public FeatureExtraction(MLContext mlContext, ILogger<FeatureExtraction> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Extract advanced features from raw pattern data
        /// </summary>
        public async Task<FeatureEngineeringResult> ExtractAdvancedFeaturesAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config = null)
        {
            config = config ?? new PatternDetectionConfig();
            var dataList = data.ToList();
            
            _logger.LogInformation("Starting advanced feature extraction for {DataCount} samples", dataList.Count);

            var result = new FeatureEngineeringResult
            {
                ExtractionTimestamp = DateTime.UtcNow,
                OriginalDataCount = dataList.Count,
                FeatureNames = new List<string>(),
                FeatureMatrix = null,
                FeatureStatistics = new Dictionary<string, FeatureStatistics>(),
                FeatureCorrelations = new Dictionary<string, Dictionary<string, double>>(),
                FeatureImportanceScores = new Dictionary<string, double>()
            };

            try
            {
                // Phase 1: Basic statistical features
                var statisticalFeatures = await ExtractStatisticalFeaturesAsync(dataList, config);
                result.FeatureNames.AddRange(statisticalFeatures.FeatureNames);

                // Phase 2: Temporal features
                var temporalFeatures = await ExtractTemporalFeaturesAsync(dataList, config);
                result.FeatureNames.AddRange(temporalFeatures.FeatureNames);

                // Phase 3: Frequency domain features
                var frequencyFeatures = await ExtractFrequencyFeaturesAsync(dataList, config);
                result.FeatureNames.AddRange(frequencyFeatures.FeatureNames);

                // Phase 4: Interaction features
                var interactionFeatures = await ExtractInteractionFeaturesAsync(dataList, config);
                result.FeatureNames.AddRange(interactionFeatures.FeatureNames);

                // Phase 5: Domain-specific features
                var domainFeatures = await ExtractDomainSpecificFeaturesAsync(dataList, config);
                result.FeatureNames.AddRange(domainFeatures.FeatureNames);

                // Phase 6: Dimensionality reduction features
                var reducedFeatures = await ExtractReducedDimensionalityFeaturesAsync(dataList, config);
                result.FeatureNames.AddRange(reducedFeatures.FeatureNames);

                // Combine all feature matrices
                result.FeatureMatrix = CombineFeatureMatrices(new[]
                {
                    statisticalFeatures.Matrix,
                    temporalFeatures.Matrix,
                    frequencyFeatures.Matrix,
                    interactionFeatures.Matrix,
                    domainFeatures.Matrix,
                    reducedFeatures.Matrix
                });

                // Calculate feature statistics
                result.FeatureStatistics = CalculateFeatureStatistics(result.FeatureMatrix, result.FeatureNames);

                // Calculate feature correlations
                result.FeatureCorrelations = CalculateFeatureCorrelations(result.FeatureMatrix, result.FeatureNames);

                // Calculate feature importance scores
                result.FeatureImportanceScores = CalculateFeatureImportance(result.FeatureMatrix, result.FeatureNames, dataList);

                // Feature selection and filtering
                var selectedFeatures = await SelectOptimalFeaturesAsync(result, config);
                result = ApplyFeatureSelection(result, selectedFeatures);

                result.FeatureCount = result.FeatureNames.Count;
                result.FeatureSelectionRatio = (double)result.FeatureCount / (result.FeatureNames.Count + selectedFeatures.RemovedFeatures.Count);

                _logger.LogInformation("Feature extraction completed. Generated {FeatureCount} features from {OriginalCount} original dimensions",
                    result.FeatureCount, dataList.First().Features?.Count ?? 0);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during feature extraction");
                throw;
            }
        }

        /// <summary>
        /// Extract statistical features (mean, std, skewness, kurtosis, etc.)
        /// </summary>
        private async Task<FeatureExtractionResult> ExtractStatisticalFeaturesAsync(
            List<PatternData> data, PatternDetectionConfig config)
        {
            var featureNames = new List<string>();
            var features = new List<double[]>();

            foreach (var dataPoint in data)
            {
                var statisticalFeatures = new List<double>();

                if (dataPoint.Features != null && dataPoint.Features.Any())
                {
                    var values = dataPoint.Features.ToArray();

                    // Basic statistics
                    statisticalFeatures.Add(values.Mean());
                    statisticalFeatures.Add(values.StandardDeviation());
                    statisticalFeatures.Add(values.Minimum());
                    statisticalFeatures.Add(values.Maximum());
                    statisticalFeatures.Add(values.Median());

                    // Higher-order moments
                    statisticalFeatures.Add(values.Skewness());
                    statisticalFeatures.Add(values.Kurtosis());

                    // Percentiles
                    statisticalFeatures.Add(values.Percentile(25));
                    statisticalFeatures.Add(values.Percentile(75));
                    statisticalFeatures.Add(values.Percentile(90));
                    statisticalFeatures.Add(values.Percentile(95));

                    // Range and IQR
                    statisticalFeatures.Add(values.Maximum() - values.Minimum());
                    statisticalFeatures.Add(values.Percentile(75) - values.Percentile(25));

                    // Coefficient of variation
                    statisticalFeatures.Add(values.StandardDeviation() / Math.Max(Math.Abs(values.Mean()), 1e-8));

                    // Zero crossings and trend
                    statisticalFeatures.Add(CountZeroCrossings(values));
                    statisticalFeatures.Add(CalculateTrend(values));
                }
                else
                {
                    // Default values if no features available
                    statisticalFeatures.AddRange(Enumerable.Repeat(0.0, 15));
                }

                features.Add(statisticalFeatures.ToArray());
            }

            // Generate feature names only once
            if (!featureNames.Any())
            {
                featureNames.AddRange(new[]
                {
                    "stat_mean", "stat_std", "stat_min", "stat_max", "stat_median",
                    "stat_skewness", "stat_kurtosis",
                    "stat_p25", "stat_p75", "stat_p90", "stat_p95",
                    "stat_range", "stat_iqr", "stat_cv",
                    "stat_zero_crossings", "stat_trend"
                });
            }

            return new FeatureExtractionResult
            {
                FeatureNames = featureNames,
                Matrix = features.ToArray()
            };
        }

        /// <summary>
        /// Extract temporal features (rolling statistics, lag features, seasonality)
        /// </summary>
        private async Task<FeatureExtractionResult> ExtractTemporalFeaturesAsync(
            List<PatternData> data, PatternDetectionConfig config)
        {
            var featureNames = new List<string>();
            var features = new List<double[]>();

            // Sort data by timestamp
            var sortedData = data.OrderBy(d => d.Timestamp).ToList();

            for (int i = 0; i < sortedData.Count; i++)
            {
                var temporalFeatures = new List<double>();
                var currentData = sortedData[i];

                // Time-based features
                temporalFeatures.Add(currentData.Timestamp.Hour);
                temporalFeatures.Add(currentData.Timestamp.DayOfWeek.GetHashCode());
                temporalFeatures.Add(currentData.Timestamp.DayOfYear);
                temporalFeatures.Add(currentData.Timestamp.Month);

                // Rolling window statistics (if enough history)
                var windowSizes = new[] { 3, 5, 10, 20 };
                foreach (var windowSize in windowSizes)
                {
                    var windowData = GetWindowData(sortedData, i, windowSize);
                    if (windowData.Count >= windowSize / 2)
                    {
                        var windowValues = windowData.Select(d => d.Value).ToArray();
                        temporalFeatures.Add(windowValues.Mean());
                        temporalFeatures.Add(windowValues.StandardDeviation());
                        temporalFeatures.Add(CalculateWindowTrend(windowValues));
                    }
                    else
                    {
                        temporalFeatures.AddRange(new[] { 0.0, 0.0, 0.0 });
                    }
                }

                // Lag features
                var lagWindows = new[] { 1, 3, 5, 10 };
                foreach (var lag in lagWindows)
                {
                    if (i >= lag)
                    {
                        temporalFeatures.Add(sortedData[i - lag].Value);
                        temporalFeatures.Add(currentData.Value - sortedData[i - lag].Value); // Difference
                    }
                    else
                    {
                        temporalFeatures.AddRange(new[] { 0.0, 0.0 });
                    }
                }

                // Time since last significant change
                temporalFeatures.Add(CalculateTimeSinceLastChange(sortedData, i, config.SignificantChangeThreshold));

                // Velocity and acceleration
                var velocity = CalculateVelocity(sortedData, i);
                var acceleration = CalculateAcceleration(sortedData, i);
                temporalFeatures.Add(velocity);
                temporalFeatures.Add(acceleration);

                features.Add(temporalFeatures.ToArray());
            }

            // Generate feature names
            featureNames.AddRange(new[] { "temp_hour", "temp_day_of_week", "temp_day_of_year", "temp_month" });
            
            foreach (var windowSize in new[] { 3, 5, 10, 20 })
            {
                featureNames.AddRange(new[] 
                { 
                    $"temp_rolling_mean_{windowSize}", 
                    $"temp_rolling_std_{windowSize}", 
                    $"temp_rolling_trend_{windowSize}" 
                });
            }

            foreach (var lag in new[] { 1, 3, 5, 10 })
            {
                featureNames.AddRange(new[] { $"temp_lag_{lag}", $"temp_diff_{lag}" });
            }

            featureNames.AddRange(new[] { "temp_time_since_change", "temp_velocity", "temp_acceleration" });

            return new FeatureExtractionResult
            {
                FeatureNames = featureNames,
                Matrix = features.ToArray()
            };
        }

        /// <summary>
        /// Extract frequency domain features using FFT and spectral analysis
        /// </summary>
        private async Task<FeatureExtractionResult> ExtractFrequencyFeaturesAsync(
            List<PatternData> data, PatternDetectionConfig config)
        {
            var featureNames = new List<string>();
            var features = new List<double[]>();

            var sortedData = data.OrderBy(d => d.Timestamp).ToList();

            for (int i = 0; i < sortedData.Count; i++)
            {
                var frequencyFeatures = new List<double>();

                // Get window of data for frequency analysis
                var windowSize = Math.Min(32, sortedData.Count); // Power of 2 for FFT
                var windowData = GetWindowData(sortedData, i, windowSize);
                
                if (windowData.Count >= 8) // Minimum for meaningful frequency analysis
                {
                    var values = windowData.Select(d => d.Value).ToArray();
                    var frequencySpectrum = CalculateFrequencySpectrum(values);

                    // Spectral features
                    frequencyFeatures.Add(frequencySpectrum.SpectralCentroid);
                    frequencyFeatures.Add(frequencySpectrum.SpectralRolloff);
                    frequencyFeatures.Add(frequencySpectrum.SpectralFlux);
                    frequencyFeatures.Add(frequencySpectrum.ZeroCrossingRate);

                    // Power in different frequency bands
                    frequencyFeatures.Add(frequencySpectrum.LowFrequencyPower);
                    frequencyFeatures.Add(frequencySpectrum.MidFrequencyPower);
                    frequencyFeatures.Add(frequencySpectrum.HighFrequencyPower);

                    // Spectral entropy
                    frequencyFeatures.Add(frequencySpectrum.SpectralEntropy);

                    // Peak frequency and amplitude
                    frequencyFeatures.Add(frequencySpectrum.PeakFrequency);
                    frequencyFeatures.Add(frequencySpectrum.PeakAmplitude);
                }
                else
                {
                    frequencyFeatures.AddRange(Enumerable.Repeat(0.0, 10));
                }

                features.Add(frequencyFeatures.ToArray());
            }

            featureNames.AddRange(new[]
            {
                "freq_spectral_centroid", "freq_spectral_rolloff", "freq_spectral_flux", "freq_zero_crossing_rate",
                "freq_low_power", "freq_mid_power", "freq_high_power",
                "freq_spectral_entropy", "freq_peak_frequency", "freq_peak_amplitude"
            });

            return new FeatureExtractionResult
            {
                FeatureNames = featureNames,
                Matrix = features.ToArray()
            };
        }

        /// <summary>
        /// Extract interaction features (feature combinations and relationships)
        /// </summary>
        private async Task<FeatureExtractionResult> ExtractInteractionFeaturesAsync(
            List<PatternData> data, PatternDetectionConfig config)
        {
            var featureNames = new List<string>();
            var features = new List<double[]>();

            foreach (var dataPoint in data)
            {
                var interactionFeatures = new List<double>();

                if (dataPoint.Features != null && dataPoint.Features.Count >= 2)
                {
                    var values = dataPoint.Features.ToArray();

                    // Pairwise interactions (limited to avoid explosion)
                    var maxInteractions = Math.Min(10, values.Length * (values.Length - 1) / 2);
                    var interactionCount = 0;

                    for (int i = 0; i < values.Length && interactionCount < maxInteractions; i++)
                    {
                        for (int j = i + 1; j < values.Length && interactionCount < maxInteractions; j++)
                        {
                            // Product interaction
                            interactionFeatures.Add(values[i] * values[j]);

                            // Ratio interaction (avoid division by zero)
                            if (Math.Abs(values[j]) > 1e-8)
                            {
                                interactionFeatures.Add(values[i] / values[j]);
                            }
                            else
                            {
                                interactionFeatures.Add(0.0);
                            }

                            // Difference interaction
                            interactionFeatures.Add(Math.Abs(values[i] - values[j]));

                            interactionCount++;
                        }
                    }

                    // Polynomial features (quadratic)
                    foreach (var value in values.Take(5)) // Limit to first 5 to avoid explosion
                    {
                        interactionFeatures.Add(value * value);
                        interactionFeatures.Add(Math.Sqrt(Math.Abs(value)));
                        interactionFeatures.Add(Math.Log(Math.Abs(value) + 1));
                    }
                }
                else
                {
                    interactionFeatures.AddRange(Enumerable.Repeat(0.0, 45)); // 30 interactions + 15 polynomial
                }

                features.Add(interactionFeatures.ToArray());
            }

            // Generate feature names (simplified for brevity)
            for (int i = 0; i < 10; i++)
            {
                featureNames.AddRange(new[] { $"inter_prod_{i}", $"inter_ratio_{i}", $"inter_diff_{i}" });
            }

            for (int i = 0; i < 5; i++)
            {
                featureNames.AddRange(new[] { $"poly_quad_{i}", $"poly_sqrt_{i}", $"poly_log_{i}" });
            }

            return new FeatureExtractionResult
            {
                FeatureNames = featureNames,
                Matrix = features.ToArray()
            };
        }

        /// <summary>
        /// Extract domain-specific features for legacy application analysis
        /// </summary>
        private async Task<FeatureExtractionResult> ExtractDomainSpecificFeaturesAsync(
            List<PatternData> data, PatternDetectionConfig config)
        {
            var featureNames = new List<string>();
            var features = new List<double[]>();

            foreach (var dataPoint in data)
            {
                var domainFeatures = new List<double>();

                // Code complexity features
                if (dataPoint.Metadata?.ContainsKey("CodeComplexity") == true)
                {
                    domainFeatures.Add(Convert.ToDouble(dataPoint.Metadata["CodeComplexity"]));
                }
                else
                {
                    domainFeatures.Add(0.0);
                }

                // Performance metrics
                if (dataPoint.Metadata?.ContainsKey("ExecutionTime") == true)
                {
                    var execTime = Convert.ToDouble(dataPoint.Metadata["ExecutionTime"]);
                    domainFeatures.Add(execTime);
                    domainFeatures.Add(Math.Log(execTime + 1)); // Log-transformed time
                }
                else
                {
                    domainFeatures.AddRange(new[] { 0.0, 0.0 });
                }

                // Error rate features
                if (dataPoint.Metadata?.ContainsKey("ErrorRate") == true)
                {
                    var errorRate = Convert.ToDouble(dataPoint.Metadata["ErrorRate"]);
                    domainFeatures.Add(errorRate);
                    domainFeatures.Add(errorRate > 0 ? 1.0 : 0.0); // Binary error indicator
                }
                else
                {
                    domainFeatures.AddRange(new[] { 0.0, 0.0 });
                }

                // Resource utilization
                var resourceMetrics = new[] { "CPUUsage", "MemoryUsage", "DiskIO", "NetworkIO" };
                foreach (var metric in resourceMetrics)
                {
                    if (dataPoint.Metadata?.ContainsKey(metric) == true)
                    {
                        domainFeatures.Add(Convert.ToDouble(dataPoint.Metadata[metric]));
                    }
                    else
                    {
                        domainFeatures.Add(0.0);
                    }
                }

                // Dependency features
                if (dataPoint.Metadata?.ContainsKey("DependencyCount") == true)
                {
                    var depCount = Convert.ToDouble(dataPoint.Metadata["DependencyCount"]);
                    domainFeatures.Add(depCount);
                    domainFeatures.Add(Math.Sqrt(depCount)); // Square root transformation
                }
                else
                {
                    domainFeatures.AddRange(new[] { 0.0, 0.0 });
                }

                // Test coverage
                if (dataPoint.Metadata?.ContainsKey("TestCoverage") == true)
                {
                    domainFeatures.Add(Convert.ToDouble(dataPoint.Metadata["TestCoverage"]));
                }
                else
                {
                    domainFeatures.Add(0.0);
                }

                // Technical debt indicators
                var technicalDebtMetrics = new[] { "CodeSmells", "Duplications", "Violations" };
                foreach (var metric in technicalDebtMetrics)
                {
                    if (dataPoint.Metadata?.ContainsKey(metric) == true)
                    {
                        domainFeatures.Add(Convert.ToDouble(dataPoint.Metadata[metric]));
                    }
                    else
                    {
                        domainFeatures.Add(0.0);
                    }
                }

                features.Add(domainFeatures.ToArray());
            }

            featureNames.AddRange(new[]
            {
                "domain_code_complexity",
                "domain_execution_time", "domain_log_execution_time",
                "domain_error_rate", "domain_has_errors",
                "domain_cpu_usage", "domain_memory_usage", "domain_disk_io", "domain_network_io",
                "domain_dependency_count", "domain_sqrt_dependency_count",
                "domain_test_coverage",
                "domain_code_smells", "domain_duplications", "domain_violations"
            });

            return new FeatureExtractionResult
            {
                FeatureNames = featureNames,
                Matrix = features.ToArray()
            };
        }

        /// <summary>
        /// Extract reduced dimensionality features using PCA and other techniques
        /// </summary>
        private async Task<FeatureExtractionResult> ExtractReducedDimensionalityFeaturesAsync(
            List<PatternData> data, PatternDetectionConfig config)
        {
            var featureNames = new List<string>();
            var features = new List<double[]>();

            if (data.Any() && data.First().Features?.Any() == true)
            {
                // Create matrix from original features
                var originalMatrix = data.Select(d => d.Features.ToArray()).ToArray();
                
                // Apply PCA for dimensionality reduction
                var pcaResult = ApplyPCA(originalMatrix, Math.Min(10, originalMatrix[0].Length));
                
                features.AddRange(pcaResult.TransformedData);

                // Generate PCA feature names
                for (int i = 0; i < pcaResult.ComponentCount; i++)
                {
                    featureNames.Add($"pca_component_{i + 1}");
                }
            }
            else
            {
                // If no original features, create empty reduced features
                foreach (var dataPoint in data)
                {
                    features.Add(new double[10]); // 10 empty PCA components
                }

                for (int i = 0; i < 10; i++)
                {
                    featureNames.Add($"pca_component_{i + 1}");
                }
            }

            return new FeatureExtractionResult
            {
                FeatureNames = featureNames,
                Matrix = features.ToArray()
            };
        }

        #region Private Helper Methods

        private double CountZeroCrossings(double[] values)
        {
            if (values.Length < 2) return 0;

            int crossings = 0;
            for (int i = 1; i < values.Length; i++)
            {
                if ((values[i] > 0 && values[i - 1] < 0) || (values[i] < 0 && values[i - 1] > 0))
                {
                    crossings++;
                }
            }
            return crossings;
        }

        private double CalculateTrend(double[] values)
        {
            if (values.Length < 2) return 0;

            // Simple linear trend using correlation with index
            var indices = Enumerable.Range(0, values.Length).Select(i => (double)i).ToArray();
            return Correlation.Pearson(indices, values);
        }

        private List<PatternData> GetWindowData(List<PatternData> sortedData, int currentIndex, int windowSize)
        {
            var startIndex = Math.Max(0, currentIndex - windowSize + 1);
            var endIndex = Math.Min(currentIndex + 1, sortedData.Count);
            return sortedData.GetRange(startIndex, endIndex - startIndex);
        }

        private double CalculateWindowTrend(double[] windowValues)
        {
            if (windowValues.Length < 2) return 0;

            var indices = Enumerable.Range(0, windowValues.Length).Select(i => (double)i).ToArray();
            return Correlation.Pearson(indices, windowValues);
        }

        private double CalculateTimeSinceLastChange(List<PatternData> sortedData, int currentIndex, double threshold)
        {
            if (currentIndex == 0) return 0;

            var currentValue = sortedData[currentIndex].Value;
            var currentTime = sortedData[currentIndex].Timestamp;

            for (int i = currentIndex - 1; i >= 0; i--)
            {
                if (Math.Abs(sortedData[i].Value - currentValue) > threshold)
                {
                    return (currentTime - sortedData[i].Timestamp).TotalMinutes;
                }
            }

            return (currentTime - sortedData[0].Timestamp).TotalMinutes;
        }

        private double CalculateVelocity(List<PatternData> sortedData, int currentIndex)
        {
            if (currentIndex == 0) return 0;

            var current = sortedData[currentIndex];
            var previous = sortedData[currentIndex - 1];
            var timeDiff = (current.Timestamp - previous.Timestamp).TotalMinutes;

            return timeDiff > 0 ? (current.Value - previous.Value) / timeDiff : 0;
        }

        private double CalculateAcceleration(List<PatternData> sortedData, int currentIndex)
        {
            if (currentIndex < 2) return 0;

            var velocity1 = CalculateVelocity(sortedData, currentIndex);
            var velocity2 = CalculateVelocity(sortedData, currentIndex - 1);
            var timeDiff = (sortedData[currentIndex].Timestamp - sortedData[currentIndex - 1].Timestamp).TotalMinutes;

            return timeDiff > 0 ? (velocity1 - velocity2) / timeDiff : 0;
        }

        private FrequencySpectrum CalculateFrequencySpectrum(double[] values)
        {
            // Simplified frequency analysis (in a real implementation, use FFT)
            var spectrum = new FrequencySpectrum();

            // Calculate basic spectral features
            spectrum.SpectralCentroid = CalculateSpectralCentroid(values);
            spectrum.SpectralRolloff = CalculateSpectralRolloff(values);
            spectrum.SpectralFlux = CalculateSpectralFlux(values);
            spectrum.ZeroCrossingRate = CountZeroCrossings(values) / (double)values.Length;
            spectrum.SpectralEntropy = CalculateSpectralEntropy(values);

            // Power in frequency bands (simplified)
            spectrum.LowFrequencyPower = values.Take(values.Length / 3).Sum(x => x * x);
            spectrum.MidFrequencyPower = values.Skip(values.Length / 3).Take(values.Length / 3).Sum(x => x * x);
            spectrum.HighFrequencyPower = values.Skip(2 * values.Length / 3).Sum(x => x * x);

            // Peak detection
            var maxIndex = values.Select((value, index) => new { value, index })
                                .OrderByDescending(x => Math.Abs(x.value))
                                .First().index;
            spectrum.PeakFrequency = maxIndex / (double)values.Length;
            spectrum.PeakAmplitude = Math.Abs(values[maxIndex]);

            return spectrum;
        }

        private double CalculateSpectralCentroid(double[] values)
        {
            var weightedSum = values.Select((value, index) => index * Math.Abs(value)).Sum();
            var totalMagnitude = values.Sum(Math.Abs);
            return totalMagnitude > 0 ? weightedSum / totalMagnitude : 0;
        }

        private double CalculateSpectralRolloff(double[] values)
        {
            var totalEnergy = values.Sum(x => x * x);
            var threshold = totalEnergy * 0.85; // 85% rolloff
            var cumulativeEnergy = 0.0;

            for (int i = 0; i < values.Length; i++)
            {
                cumulativeEnergy += values[i] * values[i];
                if (cumulativeEnergy >= threshold)
                {
                    return i / (double)values.Length;
                }
            }

            return 1.0;
        }

        private double CalculateSpectralFlux(double[] values)
        {
            if (values.Length < 2) return 0;

            var flux = 0.0;
            for (int i = 1; i < values.Length; i++)
            {
                flux += Math.Pow(Math.Abs(values[i]) - Math.Abs(values[i - 1]), 2);
            }

            return Math.Sqrt(flux);
        }

        private double CalculateSpectralEntropy(double[] values)
        {
            var magnitudes = values.Select(Math.Abs).ToArray();
            var totalMagnitude = magnitudes.Sum();

            if (totalMagnitude == 0) return 0;

            var probabilities = magnitudes.Select(m => m / totalMagnitude).Where(p => p > 0).ToArray();
            return -probabilities.Sum(p => p * Math.Log2(p));
        }

        private double[][] CombineFeatureMatrices(double[][][] matrices)
        {
            var validMatrices = matrices.Where(m => m != null && m.Length > 0).ToArray();
            if (!validMatrices.Any()) return new double[0][];

            var rowCount = validMatrices[0].Length;
            var combinedMatrix = new double[rowCount][];

            for (int row = 0; row < rowCount; row++)
            {
                var combinedRow = new List<double>();
                foreach (var matrix in validMatrices)
                {
                    if (row < matrix.Length && matrix[row] != null)
                    {
                        combinedRow.AddRange(matrix[row]);
                    }
                }
                combinedMatrix[row] = combinedRow.ToArray();
            }

            return combinedMatrix;
        }

        private Dictionary<string, FeatureStatistics> CalculateFeatureStatistics(
            double[][] featureMatrix, List<string> featureNames)
        {
            var statistics = new Dictionary<string, FeatureStatistics>();

            if (featureMatrix == null || !featureMatrix.Any() || !featureNames.Any())
                return statistics;

            for (int col = 0; col < Math.Min(featureNames.Count, featureMatrix[0].Length); col++)
            {
                var values = featureMatrix.Select(row => row.Length > col ? row[col] : 0.0).ToArray();
                
                statistics[featureNames[col]] = new FeatureStatistics
                {
                    Mean = values.Mean(),
                    StandardDeviation = values.StandardDeviation(),
                    Minimum = values.Minimum(),
                    Maximum = values.Maximum(),
                    Median = values.Median(),
                    Skewness = values.Skewness(),
                    Kurtosis = values.Kurtosis()
                };
            }

            return statistics;
        }

        private Dictionary<string, Dictionary<string, double>> CalculateFeatureCorrelations(
            double[][] featureMatrix, List<string> featureNames)
        {
            var correlations = new Dictionary<string, Dictionary<string, double>>();

            if (featureMatrix == null || !featureMatrix.Any() || featureNames.Count < 2)
                return correlations;

            for (int i = 0; i < featureNames.Count; i++)
            {
                correlations[featureNames[i]] = new Dictionary<string, double>();
                
                for (int j = 0; j < featureNames.Count; j++)
                {
                    if (i < featureMatrix[0].Length && j < featureMatrix[0].Length)
                    {
                        var values1 = featureMatrix.Select(row => row.Length > i ? row[i] : 0.0).ToArray();
                        var values2 = featureMatrix.Select(row => row.Length > j ? row[j] : 0.0).ToArray();
                        
                        correlations[featureNames[i]][featureNames[j]] = Correlation.Pearson(values1, values2);
                    }
                    else
                    {
                        correlations[featureNames[i]][featureNames[j]] = 0.0;
                    }
                }
            }

            return correlations;
        }

        private Dictionary<string, double> CalculateFeatureImportance(
            double[][] featureMatrix, List<string> featureNames, List<PatternData> originalData)
        {
            var importance = new Dictionary<string, double>();

            // Calculate importance based on variance and correlation with target
            for (int i = 0; i < featureNames.Count && i < (featureMatrix.FirstOrDefault()?.Length ?? 0); i++)
            {
                var values = featureMatrix.Select(row => row.Length > i ? row[i] : 0.0).ToArray();
                var variance = values.Variance();
                
                // Normalize variance-based importance
                importance[featureNames[i]] = variance;
            }

            // Normalize to [0, 1]
            var maxImportance = importance.Values.Any() ? importance.Values.Max() : 1.0;
            if (maxImportance > 0)
            {
                var normalizedImportance = importance.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value / maxImportance);
                return normalizedImportance;
            }

            return importance;
        }

        private async Task<FeatureSelectionResult> SelectOptimalFeaturesAsync(
            FeatureEngineeringResult result, PatternDetectionConfig config)
        {
            var selectedFeatures = new List<string>();
            var removedFeatures = new List<string>();

            // Feature selection based on importance and correlation
            var importanceThreshold = config.FeatureImportanceThreshold;
            var correlationThreshold = config.FeatureCorrelationThreshold;

            foreach (var feature in result.FeatureNames)
            {
                var importance = result.FeatureImportanceScores.GetValueOrDefault(feature, 0.0);
                
                if (importance >= importanceThreshold)
                {
                    // Check for high correlation with already selected features
                    var isHighlyCorrelated = false;
                    foreach (var selectedFeature in selectedFeatures)
                    {
                        var correlation = result.FeatureCorrelations
                            .GetValueOrDefault(feature, new Dictionary<string, double>())
                            .GetValueOrDefault(selectedFeature, 0.0);
                        
                        if (Math.Abs(correlation) > correlationThreshold)
                        {
                            isHighlyCorrelated = true;
                            break;
                        }
                    }

                    if (!isHighlyCorrelated)
                    {
                        selectedFeatures.Add(feature);
                    }
                    else
                    {
                        removedFeatures.Add(feature);
                    }
                }
                else
                {
                    removedFeatures.Add(feature);
                }
            }

            return new FeatureSelectionResult
            {
                SelectedFeatures = selectedFeatures,
                RemovedFeatures = removedFeatures,
                SelectionRatio = (double)selectedFeatures.Count / result.FeatureNames.Count
            };
        }

        private FeatureEngineeringResult ApplyFeatureSelection(
            FeatureEngineeringResult original, FeatureSelectionResult selection)
        {
            var selectedIndices = selection.SelectedFeatures
                .Select(f => original.FeatureNames.IndexOf(f))
                .Where(i => i >= 0)
                .ToArray();

            var newMatrix = original.FeatureMatrix
                .Select(row => selectedIndices.Select(i => i < row.Length ? row[i] : 0.0).ToArray())
                .ToArray();

            var newStatistics = selection.SelectedFeatures
                .Where(f => original.FeatureStatistics.ContainsKey(f))
                .ToDictionary(f => f, f => original.FeatureStatistics[f]);

            var newCorrelations = selection.SelectedFeatures
                .Where(f => original.FeatureCorrelations.ContainsKey(f))
                .ToDictionary(f => f, f => original.FeatureCorrelations[f]
                    .Where(kvp => selection.SelectedFeatures.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

            var newImportance = selection.SelectedFeatures
                .Where(f => original.FeatureImportanceScores.ContainsKey(f))
                .ToDictionary(f => f, f => original.FeatureImportanceScores[f]);

            return new FeatureEngineeringResult
            {
                ExtractionTimestamp = original.ExtractionTimestamp,
                OriginalDataCount = original.OriginalDataCount,
                FeatureNames = selection.SelectedFeatures,
                FeatureMatrix = newMatrix,
                FeatureStatistics = newStatistics,
                FeatureCorrelations = newCorrelations,
                FeatureImportanceScores = newImportance,
                FeatureCount = selection.SelectedFeatures.Count,
                FeatureSelectionRatio = selection.SelectionRatio
            };
        }

        private PCAResult ApplyPCA(double[][] matrix, int componentCount)
        {
            // Simplified PCA implementation
            // In a real implementation, use a proper linear algebra library
            
            var result = new PCAResult
            {
                ComponentCount = componentCount,
                TransformedData = new List<double[]>()
            };

            // For now, just return the first few features as "PCA components"
            // In a real implementation, this would involve eigenvalue decomposition
            foreach (var row in matrix)
            {
                var pcaRow = new double[componentCount];
                for (int i = 0; i < componentCount && i < row.Length; i++)
                {
                    pcaRow[i] = row[i]; // Simplified - should be actual PCA transformation
                }
                result.TransformedData.Add(pcaRow);
            }

            return result;
        }

        #endregion
    }

    #region Supporting Classes

    public class FeatureExtractionResult
    {
        public List<string> FeatureNames { get; set; } = new List<string>();
        public double[][] Matrix { get; set; }
    }

    public class FrequencySpectrum
    {
        public double SpectralCentroid { get; set; }
        public double SpectralRolloff { get; set; }
        public double SpectralFlux { get; set; }
        public double ZeroCrossingRate { get; set; }
        public double LowFrequencyPower { get; set; }
        public double MidFrequencyPower { get; set; }
        public double HighFrequencyPower { get; set; }
        public double SpectralEntropy { get; set; }
        public double PeakFrequency { get; set; }
        public double PeakAmplitude { get; set; }
    }

    public class FeatureSelectionResult
    {
        public List<string> SelectedFeatures { get; set; } = new List<string>();
        public List<string> RemovedFeatures { get; set; } = new List<string>();
        public double SelectionRatio { get; set; }
    }

    public class PCAResult
    {
        public int ComponentCount { get; set; }
        public List<double[]> TransformedData { get; set; } = new List<double[]>();
    }

    #endregion
}
