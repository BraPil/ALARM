using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;

namespace ALARM.Analyzers.PatternDetection
{
    /// <summary>
    /// Sequential pattern mining implementation
    /// </summary>
    public class SequentialPatterns
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<SequentialPatterns> _logger;

        public SequentialPatterns(MLContext mlContext, ILogger<SequentialPatterns> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Mine sequential patterns from time-series data
        /// </summary>
        public async Task<SequentialPatternResult> MineSequentialPatternsAsync(
            IEnumerable<PatternData> data,
            PatternDetectionConfig config)
        {
            _logger.LogInformation("Starting sequential pattern mining");

            var sortedData = data.OrderBy(d => d.Timestamp).ToList();
            
            var result = new SequentialPatternResult
            {
                MiningTimestamp = DateTime.UtcNow,
                Algorithm = "PrefixSpan",
                Patterns = new List<SequentialPattern>()
            };

            try
            {
                // Convert data to sequences
                var sequences = ConvertToSequences(sortedData, config);
                
                // Mine frequent sequential patterns using PrefixSpan-like algorithm
                var frequentPatterns = await MineFrequentPatternsAsync(sequences, config);
                
                // Calculate confidence and other metrics
                result.Patterns = await CalculatePatternMetricsAsync(frequentPatterns, sequences, config);
                
                // Calculate overall metrics
                result.PatternCount = result.Patterns.Count;
                result.AverageFrequency = result.Patterns.Any() ? result.Patterns.Average(p => p.Support) : 0.0;
                result.TemporalComplexity = CalculateTemporalComplexity(result.Patterns);

                _logger.LogInformation("Sequential pattern mining completed. Found {PatternCount} patterns",
                    result.PatternCount);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sequential pattern mining");
                throw;
            }
        }

        /// <summary>
        /// Convert pattern data to sequences for mining
        /// </summary>
        private List<Sequence> ConvertToSequences(List<PatternData> data, PatternDetectionConfig config)
        {
            var sequences = new List<Sequence>();
            
            // Group data by source or category to create sequences
            var groupedData = data.GroupBy(d => d.Source ?? "default").ToList();
            
            foreach (var group in groupedData)
            {
                var sortedGroup = group.OrderBy(d => d.Timestamp).ToList();
                var sequence = new Sequence
                {
                    Id = group.Key,
                    Items = new List<SequenceItem>()
                };

                foreach (var dataPoint in sortedGroup)
                {
                    var item = new SequenceItem
                    {
                        Timestamp = dataPoint.Timestamp,
                        Value = dataPoint.Value,
                        Category = DetermineCategory(dataPoint, config),
                        Metadata = dataPoint.Metadata
                    };
                    sequence.Items.Add(item);
                }

                if (sequence.Items.Count >= 2) // Need at least 2 items for a sequence
                {
                    sequences.Add(sequence);
                }
            }

            // If no natural grouping, create sliding window sequences
            if (sequences.Count < 2)
            {
                sequences = CreateSlidingWindowSequences(data, config);
            }

            return sequences;
        }

        /// <summary>
        /// Create sequences using sliding window approach
        /// </summary>
        private List<Sequence> CreateSlidingWindowSequences(List<PatternData> data, PatternDetectionConfig config)
        {
            var sequences = new List<Sequence>();
            var windowSize = Math.Min(config.MaxSequenceLength, 10);
            var stepSize = Math.Max(1, windowSize / 2);

            for (int i = 0; i <= data.Count - windowSize; i += stepSize)
            {
                var sequence = new Sequence
                {
                    Id = $"window_{i}",
                    Items = new List<SequenceItem>()
                };

                for (int j = 0; j < windowSize && i + j < data.Count; j++)
                {
                    var dataPoint = data[i + j];
                    var item = new SequenceItem
                    {
                        Timestamp = dataPoint.Timestamp,
                        Value = dataPoint.Value,
                        Category = DetermineCategory(dataPoint, config),
                        Metadata = dataPoint.Metadata
                    };
                    sequence.Items.Add(item);
                }

                sequences.Add(sequence);
            }

            return sequences;
        }

        /// <summary>
        /// Determine category for sequence item
        /// </summary>
        private string DetermineCategory(PatternData dataPoint, PatternDetectionConfig config)
        {
            // Simple categorization based on value ranges
            if (dataPoint.Value < -1.0) return "VeryLow";
            if (dataPoint.Value < -0.5) return "Low";
            if (dataPoint.Value < 0.5) return "Normal";
            if (dataPoint.Value < 1.0) return "High";
            return "VeryHigh";
        }

        /// <summary>
        /// Mine frequent patterns using PrefixSpan-like algorithm
        /// </summary>
        private async Task<List<FrequentPattern>> MineFrequentPatternsAsync(
            List<Sequence> sequences,
            PatternDetectionConfig config)
        {
            var frequentPatterns = new List<FrequentPattern>();
            
            // Find frequent 1-patterns (single items)
            var itemCounts = new Dictionary<string, int>();
            foreach (var sequence in sequences)
            {
                var uniqueItems = sequence.Items.Select(i => i.Category).Distinct();
                foreach (var item in uniqueItems)
                {
                    itemCounts[item] = itemCounts.GetValueOrDefault(item, 0) + 1;
                }
            }

            var minSupport = (int)(sequences.Count * config.MinSupportForSequentialPattern);
            var frequent1Patterns = itemCounts.Where(kvp => kvp.Value >= minSupport)
                                             .Select(kvp => new FrequentPattern
                                             {
                                                 Pattern = new List<string> { kvp.Key },
                                                 Support = kvp.Value / (double)sequences.Count,
                                                 Occurrences = new List<PatternOccurrence>()
                                             })
                                             .ToList();

            frequentPatterns.AddRange(frequent1Patterns);

            // Recursively mine longer patterns
            for (int length = 2; length <= config.MaxSequenceLength; length++)
            {
                var longerPatterns = await MinePatternsByLengthAsync(
                    sequences, frequent1Patterns.Select(p => p.Pattern.First()).ToList(), 
                    length, minSupport, config);
                
                if (!longerPatterns.Any()) break; // No more frequent patterns found
                
                frequentPatterns.AddRange(longerPatterns);
            }

            return frequentPatterns;
        }

        /// <summary>
        /// Mine patterns of specific length
        /// </summary>
        private async Task<List<FrequentPattern>> MinePatternsByLengthAsync(
            List<Sequence> sequences,
            List<string> frequentItems,
            int targetLength,
            int minSupport,
            PatternDetectionConfig config)
        {
            var patterns = new List<FrequentPattern>();
            
            // Generate candidate patterns
            var candidatePatterns = GenerateCandidatePatterns(frequentItems, targetLength);
            
            foreach (var candidate in candidatePatterns)
            {
                var supportCount = 0;
                var occurrences = new List<PatternOccurrence>();

                foreach (var sequence in sequences)
                {
                    var patternOccurrences = FindPatternInSequence(sequence, candidate);
                    if (patternOccurrences.Any())
                    {
                        supportCount++;
                        occurrences.AddRange(patternOccurrences);
                    }
                }

                var support = supportCount / (double)sequences.Count;
                if (support >= config.MinSupportForSequentialPattern)
                {
                    patterns.Add(new FrequentPattern
                    {
                        Pattern = candidate,
                        Support = support,
                        Occurrences = occurrences
                    });
                }
            }

            return patterns;
        }

        /// <summary>
        /// Generate candidate patterns of target length
        /// </summary>
        private List<List<string>> GenerateCandidatePatterns(List<string> frequentItems, int targetLength)
        {
            var candidates = new List<List<string>>();
            
            // Simple approach: generate all combinations
            GenerateCombinationsRecursive(frequentItems, targetLength, new List<string>(), candidates);
            
            return candidates;
        }

        /// <summary>
        /// Recursively generate combinations
        /// </summary>
        private void GenerateCombinationsRecursive(
            List<string> items, 
            int remainingLength, 
            List<string> currentPattern, 
            List<List<string>> results)
        {
            if (remainingLength == 0)
            {
                results.Add(new List<string>(currentPattern));
                return;
            }

            foreach (var item in items)
            {
                currentPattern.Add(item);
                GenerateCombinationsRecursive(items, remainingLength - 1, currentPattern, results);
                currentPattern.RemoveAt(currentPattern.Count - 1);
            }
        }

        /// <summary>
        /// Find pattern occurrences in a sequence
        /// </summary>
        private List<PatternOccurrence> FindPatternInSequence(Sequence sequence, List<string> pattern)
        {
            var occurrences = new List<PatternOccurrence>();
            
            for (int i = 0; i <= sequence.Items.Count - pattern.Count; i++)
            {
                var match = true;
                var matchConfidence = 1.0;

                for (int j = 0; j < pattern.Count; j++)
                {
                    if (sequence.Items[i + j].Category != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    occurrences.Add(new PatternOccurrence
                    {
                        Timestamp = sequence.Items[i].Timestamp,
                        Value = sequence.Items[i].Value,
                        DataIndex = i,
                        MatchConfidence = matchConfidence
                    });
                }
            }

            return occurrences;
        }

        /// <summary>
        /// Calculate pattern metrics including confidence
        /// </summary>
        private async Task<List<SequentialPattern>> CalculatePatternMetricsAsync(
            List<FrequentPattern> frequentPatterns,
            List<Sequence> sequences,
            PatternDetectionConfig config)
        {
            var sequentialPatterns = new List<SequentialPattern>();

            foreach (var pattern in frequentPatterns)
            {
                var confidence = CalculatePatternConfidence(pattern, sequences);
                var averageInterval = CalculateAverageInterval(pattern.Occurrences);

                var sequentialPattern = new SequentialPattern
                {
                    PatternId = Guid.NewGuid().ToString(),
                    Sequence = pattern.Pattern,
                    Support = pattern.Support,
                    Confidence = confidence,
                    Occurrences = pattern.Occurrences,
                    AverageInterval = averageInterval,
                    SequenceMetrics = new Dictionary<string, double>
                    {
                        ["Length"] = pattern.Pattern.Count,
                        ["Frequency"] = pattern.Occurrences.Count,
                        ["AverageInterval"] = averageInterval.TotalMinutes,
                        ["Distinctness"] = CalculatePatternDistinctness(pattern.Pattern)
                    }
                };

                sequentialPatterns.Add(sequentialPattern);
            }

            return sequentialPatterns.Where(p => p.Confidence >= config.MinConfidenceForSequentialPattern)
                                   .OrderByDescending(p => p.Support * p.Confidence)
                                   .ToList();
        }

        /// <summary>
        /// Calculate pattern confidence
        /// </summary>
        private double CalculatePatternConfidence(FrequentPattern pattern, List<Sequence> sequences)
        {
            if (pattern.Pattern.Count <= 1) return 1.0;

            // Calculate confidence as P(full pattern | prefix pattern)
            var prefix = pattern.Pattern.Take(pattern.Pattern.Count - 1).ToList();
            var prefixCount = 0;
            var fullPatternCount = 0;

            foreach (var sequence in sequences)
            {
                var prefixOccurrences = FindPatternInSequence(sequence, prefix);
                var fullOccurrences = FindPatternInSequence(sequence, pattern.Pattern);

                if (prefixOccurrences.Any()) prefixCount++;
                if (fullOccurrences.Any()) fullPatternCount++;
            }

            return prefixCount > 0 ? (double)fullPatternCount / prefixCount : 0.0;
        }

        /// <summary>
        /// Calculate average interval between pattern occurrences
        /// </summary>
        private TimeSpan CalculateAverageInterval(List<PatternOccurrence> occurrences)
        {
            if (occurrences.Count < 2) return TimeSpan.Zero;

            var sortedOccurrences = occurrences.OrderBy(o => o.Timestamp).ToList();
            var intervals = new List<TimeSpan>();

            for (int i = 1; i < sortedOccurrences.Count; i++)
            {
                intervals.Add(sortedOccurrences[i].Timestamp - sortedOccurrences[i - 1].Timestamp);
            }

            var totalTicks = intervals.Sum(i => i.Ticks);
            return new TimeSpan(totalTicks / intervals.Count);
        }

        /// <summary>
        /// Calculate pattern distinctness (diversity of elements)
        /// </summary>
        private double CalculatePatternDistinctness(List<string> pattern)
        {
            var uniqueElements = pattern.Distinct().Count();
            return (double)uniqueElements / pattern.Count;
        }

        /// <summary>
        /// Calculate temporal complexity of all patterns
        /// </summary>
        private double CalculateTemporalComplexity(List<SequentialPattern> patterns)
        {
            if (!patterns.Any()) return 0.0;

            var avgLength = patterns.Average(p => p.Sequence.Count);
            var lengthVariance = patterns.Select(p => p.Sequence.Count)
                                       .Select(l => Math.Pow(l - avgLength, 2))
                                       .Average();
            var uniqueElements = patterns.SelectMany(p => p.Sequence).Distinct().Count();
            var avgInterval = patterns.Where(p => p.AverageInterval.TotalMinutes > 0)
                                    .Select(p => p.AverageInterval.TotalMinutes)
                                    .DefaultIfEmpty(0)
                                    .Average();

            // Combine metrics for overall complexity
            var lengthComplexity = Math.Min(avgLength / 10.0, 1.0);
            var varianceComplexity = Math.Min(Math.Sqrt(lengthVariance) / 5.0, 1.0);
            var elementComplexity = Math.Min(uniqueElements / 20.0, 1.0);
            var intervalComplexity = Math.Min(avgInterval / 60.0, 1.0); // Normalize by hour

            return (lengthComplexity + varianceComplexity + elementComplexity + intervalComplexity) / 4.0;
        }
    }

    #region Supporting Classes

    /// <summary>
    /// Sequence for pattern mining
    /// </summary>
    internal class Sequence
    {
        public string Id { get; set; } = string.Empty;
        public List<SequenceItem> Items { get; set; } = new List<SequenceItem>();
    }

    /// <summary>
    /// Item in a sequence
    /// </summary>
    internal class SequenceItem
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Frequent pattern during mining
    /// </summary>
    internal class FrequentPattern
    {
        public List<string> Pattern { get; set; } = new List<string>();
        public double Support { get; set; }
        public List<PatternOccurrence> Occurrences { get; set; } = new List<PatternOccurrence>();
    }

    #endregion
}
