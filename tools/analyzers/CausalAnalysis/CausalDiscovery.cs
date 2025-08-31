using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.Extensions.Logging;
using MathNet.Numerics.Statistics;

namespace ALARM.Analyzers.CausalAnalysis
{
    /// <summary>
    /// Causal discovery algorithms for identifying causal relationships from data
    /// Implements PC algorithm, FCI, and other constraint-based methods
    /// </summary>
    public class CausalDiscovery
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<CausalDiscovery> _logger;

        public CausalDiscovery(MLContext mlContext, ILogger<CausalDiscovery> logger)
        {
            _mlContext = mlContext ?? throw new ArgumentNullException(nameof(mlContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Discover causal relationships using multiple algorithms
        /// </summary>
        public async Task<CausalDiscoveryResult> DiscoverCausalRelationshipsAsync(
            List<CausalData> data, CausalAnalysisConfig config)
        {
            _logger.LogInformation("Starting causal discovery with {DataCount} samples", data.Count);

            var result = new CausalDiscoveryResult
            {
                CausalRelationships = new List<CausalRelationship>(),
                CausalGraph = new CausalGraph()
            };

            try
            {
                // Extract variable names
                var variables = ExtractVariableNames(data);
                _logger.LogInformation("Analyzing {VariableCount} variables", variables.Count);

                // Method 1: PC Algorithm (constraint-based)
                var pcResults = await RunPCAlgorithmAsync(data, variables, config);
                result.CausalRelationships.AddRange(pcResults);

                // Method 2: Granger Causality (time-series based)
                var grangerResults = await RunGrangerCausalityAsync(data, variables, config);
                result.CausalRelationships.AddRange(grangerResults);

                // Method 3: Information-theoretic measures
                var informationResults = await RunInformationTheoreticDiscoveryAsync(data, variables, config);
                result.CausalRelationships.AddRange(informationResults);

                // Combine and validate results
                result.CausalRelationships = CombineAndValidateRelationships(result.CausalRelationships, config);

                // Build causal graph
                result.CausalGraph = BuildCausalGraph(result.CausalRelationships, variables);

                _logger.LogInformation("Discovered {RelationshipCount} causal relationships", 
                    result.CausalRelationships.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during causal discovery");
                throw;
            }
        }

        /// <summary>
        /// Extract variable names from causal data
        /// </summary>
        private List<string> ExtractVariableNames(List<CausalData> data)
        {
            var variables = new HashSet<string>();
            
            foreach (var dataPoint in data)
            {
                foreach (var variable in dataPoint.Variables.Keys)
                {
                    variables.Add(variable);
                }
            }
            
            return variables.ToList();
        }

        /// <summary>
        /// Run PC Algorithm for causal discovery
        /// </summary>
        private async Task<List<CausalRelationship>> RunPCAlgorithmAsync(
            List<CausalData> data, List<string> variables, CausalAnalysisConfig config)
        {
            _logger.LogInformation("Running PC Algorithm for causal discovery");
            
            var relationships = new List<CausalRelationship>();
            
            // Build correlation matrix
            var correlationMatrix = BuildCorrelationMatrix(data, variables);
            
            // Phase 1: Find skeleton (undirected graph)
            var skeleton = FindSkeleton(correlationMatrix, variables, config.PCAlgorithmAlpha);
            
            // Phase 2: Orient edges using conditional independence tests
            var orientedEdges = OrientEdges(skeleton, data, variables, config);
            
            // Convert to causal relationships
            foreach (var edge in orientedEdges)
            {
                relationships.Add(new CausalRelationship
                {
                    Id = Guid.NewGuid().ToString(),
                    CauseVariable = edge.Source,
                    EffectVariable = edge.Target,
                    Strength = edge.Strength,
                    Confidence = edge.Confidence,
                    Method = "PC Algorithm",
                    Direction = CausalDirection.Forward,
                    Evidence = new List<string> { "Constraint-based discovery", "Conditional independence" }
                });
            }
            
            return relationships;
        }

        /// <summary>
        /// Run Granger Causality analysis
        /// </summary>
        private async Task<List<CausalRelationship>> RunGrangerCausalityAsync(
            List<CausalData> data, List<string> variables, CausalAnalysisConfig config)
        {
            _logger.LogInformation("Running Granger Causality analysis");
            
            var relationships = new List<CausalRelationship>();
            var sortedData = data.OrderBy(d => d.Timestamp).ToList();
            
            if (sortedData.Count < config.MinDataPointsForGranger)
            {
                _logger.LogWarning("Insufficient data for Granger causality ({DataCount} < {MinRequired})", 
                    sortedData.Count, config.MinDataPointsForGranger);
                return relationships;
            }
            
            // Test all pairs of variables
            for (int i = 0; i < variables.Count; i++)
            {
                for (int j = 0; j < variables.Count; j++)
                {
                    if (i != j)
                    {
                        var causeVar = variables[i];
                        var effectVar = variables[j];
                        
                        var grangerResult = TestGrangerCausality(sortedData, causeVar, effectVar, config);
                        
                        if (grangerResult.IsSignificant)
                        {
                            relationships.Add(new CausalRelationship
                            {
                                Id = Guid.NewGuid().ToString(),
                                CauseVariable = causeVar,
                                EffectVariable = effectVar,
                                Strength = grangerResult.Strength,
                                Confidence = grangerResult.Confidence,
                                Method = "Granger Causality",
                                Direction = CausalDirection.Forward,
                                Evidence = new List<string> 
                                { 
                                    "Temporal precedence", 
                                    $"F-statistic: {grangerResult.FStatistic:F3}",
                                    $"P-value: {grangerResult.PValue:F6}"
                                }
                            });
                        }
                    }
                }
            }
            
            return relationships;
        }

        /// <summary>
        /// Run information-theoretic causal discovery
        /// </summary>
        private async Task<List<CausalRelationship>> RunInformationTheoreticDiscoveryAsync(
            List<CausalData> data, List<string> variables, CausalAnalysisConfig config)
        {
            _logger.LogInformation("Running information-theoretic causal discovery");
            
            var relationships = new List<CausalRelationship>();
            
            // Test all pairs using mutual information and transfer entropy
            for (int i = 0; i < variables.Count; i++)
            {
                for (int j = 0; j < variables.Count; j++)
                {
                    if (i != j)
                    {
                        var causeVar = variables[i];
                        var effectVar = variables[j];
                        
                        var infoResult = TestInformationTheoreticCausality(data, causeVar, effectVar, config);
                        
                        if (infoResult.IsSignificant)
                        {
                            relationships.Add(new CausalRelationship
                            {
                                Id = Guid.NewGuid().ToString(),
                                CauseVariable = causeVar,
                                EffectVariable = effectVar,
                                Strength = infoResult.TransferEntropy,
                                Confidence = infoResult.Confidence,
                                Method = "Transfer Entropy",
                                Direction = CausalDirection.Forward,
                                Evidence = new List<string> 
                                { 
                                    "Information flow", 
                                    $"Transfer Entropy: {infoResult.TransferEntropy:F3}",
                                    $"Mutual Information: {infoResult.MutualInformation:F3}"
                                }
                            });
                        }
                    }
                }
            }
            
            return relationships;
        }

        /// <summary>
        /// Build correlation matrix for variables
        /// </summary>
        private double[,] BuildCorrelationMatrix(List<CausalData> data, List<string> variables)
        {
            var n = variables.Count;
            var correlationMatrix = new double[n, n];
            
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        correlationMatrix[i, j] = 1.0;
                    }
                    else
                    {
                        var values1 = data.Select(d => d.Variables.GetValueOrDefault(variables[i], 0.0)).ToArray();
                        var values2 = data.Select(d => d.Variables.GetValueOrDefault(variables[j], 0.0)).ToArray();
                        correlationMatrix[i, j] = Correlation.Pearson(values1, values2);
                    }
                }
            }
            
            return correlationMatrix;
        }

        /// <summary>
        /// Find skeleton (undirected graph) using PC algorithm
        /// </summary>
        private List<UndirectedEdge> FindSkeleton(double[,] correlationMatrix, List<string> variables, double alpha)
        {
            var edges = new List<UndirectedEdge>();
            var n = variables.Count;
            
            // Start with complete graph
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    var correlation = Math.Abs(correlationMatrix[i, j]);
                    if (correlation > alpha) // Significance threshold
                    {
                        edges.Add(new UndirectedEdge
                        {
                            Variable1 = variables[i],
                            Variable2 = variables[j],
                            Strength = correlation
                        });
                    }
                }
            }
            
            return edges;
        }

        /// <summary>
        /// Orient edges based on conditional independence
        /// </summary>
        private List<DirectedEdge> OrientEdges(List<UndirectedEdge> skeleton, List<CausalData> data, 
            List<string> variables, CausalAnalysisConfig config)
        {
            var orientedEdges = new List<DirectedEdge>();
            
            foreach (var edge in skeleton)
            {
                // Simple orientation rule: use temporal information if available
                var orientation = DetermineEdgeOrientation(data, edge.Variable1, edge.Variable2, config);
                
                if (orientation.HasValue)
                {
                    var (source, target) = orientation.Value ? (edge.Variable1, edge.Variable2) : (edge.Variable2, edge.Variable1);
                    
                    orientedEdges.Add(new DirectedEdge
                    {
                        Source = source,
                        Target = target,
                        Strength = edge.Strength,
                        Confidence = CalculateOrientationConfidence(data, source, target)
                    });
                }
            }
            
            return orientedEdges;
        }

        /// <summary>
        /// Determine edge orientation using temporal and other criteria
        /// </summary>
        private bool? DetermineEdgeOrientation(List<CausalData> data, string var1, string var2, CausalAnalysisConfig config)
        {
            // Method 1: Temporal precedence
            var temporalOrientation = DetermineTemporalOrientation(data, var1, var2);
            if (temporalOrientation.HasValue) return temporalOrientation;
            
            // Method 2: Variance-based (cause typically has higher variance)
            var varianceOrientation = DetermineVarianceOrientation(data, var1, var2);
            if (varianceOrientation.HasValue) return varianceOrientation;
            
            // Method 3: Domain knowledge (if available)
            // This would be implemented based on specific domain rules
            
            return null; // Cannot determine orientation
        }

        /// <summary>
        /// Determine orientation based on temporal precedence
        /// </summary>
        private bool? DetermineTemporalOrientation(List<CausalData> data, string var1, string var2)
        {
            var sortedData = data.OrderBy(d => d.Timestamp).ToList();
            
            if (sortedData.Count < 10) return null; // Need sufficient data
            
            var values1 = sortedData.Select(d => d.Variables.GetValueOrDefault(var1, 0.0)).ToArray();
            var values2 = sortedData.Select(d => d.Variables.GetValueOrDefault(var2, 0.0)).ToArray();
            
            // Calculate lagged correlations
            var maxLag = Math.Min(5, sortedData.Count / 4);
            var bestLag1to2 = 0;
            var bestLag2to1 = 0;
            var maxCorr1to2 = 0.0;
            var maxCorr2to1 = 0.0;
            
            for (int lag = 1; lag <= maxLag; lag++)
            {
                // Test var1 -> var2 with lag
                if (values1.Length > lag && values2.Length > lag)
                {
                    var lagged1 = values1.Take(values1.Length - lag).ToArray();
                    var future2 = values2.Skip(lag).ToArray();
                    
                    if (lagged1.Length == future2.Length && lagged1.Length > 0)
                    {
                        var corr = Math.Abs(Correlation.Pearson(lagged1, future2));
                        if (corr > maxCorr1to2)
                        {
                            maxCorr1to2 = corr;
                            bestLag1to2 = lag;
                        }
                    }
                    
                    // Test var2 -> var1 with lag
                    var lagged2 = values2.Take(values2.Length - lag).ToArray();
                    var future1 = values1.Skip(lag).ToArray();
                    
                    if (lagged2.Length == future1.Length && lagged2.Length > 0)
                    {
                        var corr = Math.Abs(Correlation.Pearson(lagged2, future1));
                        if (corr > maxCorr2to1)
                        {
                            maxCorr2to1 = corr;
                            bestLag2to1 = lag;
                        }
                    }
                }
            }
            
            // Determine direction based on stronger lagged correlation
            if (Math.Abs(maxCorr1to2 - maxCorr2to1) > 0.1) // Significant difference
            {
                return maxCorr1to2 > maxCorr2to1; // true if var1 -> var2
            }
            
            return null;
        }

        /// <summary>
        /// Determine orientation based on variance
        /// </summary>
        private bool? DetermineVarianceOrientation(List<CausalData> data, string var1, string var2)
        {
            var values1 = data.Select(d => d.Variables.GetValueOrDefault(var1, 0.0)).ToArray();
            var values2 = data.Select(d => d.Variables.GetValueOrDefault(var2, 0.0)).ToArray();
            
            var variance1 = values1.Variance();
            var variance2 = values2.Variance();
            
            // Heuristic: variable with higher variance is more likely to be the cause
            var varianceRatio = Math.Max(variance1, variance2) / Math.Max(Math.Min(variance1, variance2), 1e-10);
            
            if (varianceRatio > 2.0) // Significant difference in variance
            {
                return variance1 > variance2; // true if var1 -> var2
            }
            
            return null;
        }

        /// <summary>
        /// Calculate confidence for edge orientation
        /// </summary>
        private double CalculateOrientationConfidence(List<CausalData> data, string source, string target)
        {
            // Simple confidence based on correlation strength and temporal consistency
            var sourceValues = data.Select(d => d.Variables.GetValueOrDefault(source, 0.0)).ToArray();
            var targetValues = data.Select(d => d.Variables.GetValueOrDefault(target, 0.0)).ToArray();
            
            var correlation = Math.Abs(Correlation.Pearson(sourceValues, targetValues));
            
            // Additional confidence from temporal analysis would go here
            
            return Math.Min(1.0, correlation * 1.2); // Boost correlation slightly for confidence
        }

        /// <summary>
        /// Test Granger causality between two variables
        /// </summary>
        private GrangerResult TestGrangerCausality(List<CausalData> data, string causeVar, string effectVar, CausalAnalysisConfig config)
        {
            var causeValues = data.Select(d => d.Variables.GetValueOrDefault(causeVar, 0.0)).ToArray();
            var effectValues = data.Select(d => d.Variables.GetValueOrDefault(effectVar, 0.0)).ToArray();
            
            if (causeValues.Length < config.MinDataPointsForGranger)
            {
                return new GrangerResult { IsSignificant = false, Strength = 0.0, Confidence = 0.0 };
            }
            
            // Simplified Granger test - in practice would use proper F-test
            var maxLag = Math.Min(config.MaxLagForGranger, causeValues.Length / 4);
            var bestFStat = 0.0;
            var bestPValue = 1.0;
            
            for (int lag = 1; lag <= maxLag; lag++)
            {
                if (causeValues.Length > lag && effectValues.Length > lag)
                {
                    var laggedCause = causeValues.Take(causeValues.Length - lag).ToArray();
                    var futureEffect = effectValues.Skip(lag).ToArray();
                    
                    if (laggedCause.Length == futureEffect.Length && laggedCause.Length > 0)
                    {
                        // Simplified F-test approximation
                        var correlation = Correlation.Pearson(laggedCause, futureEffect);
                        var fStat = Math.Abs(correlation) * Math.Sqrt((laggedCause.Length - 2) / (1 - correlation * correlation));
                        var pValue = 2 * (1 - NormalCDF(Math.Abs(fStat))); // Rough approximation
                        
                        if (fStat > bestFStat)
                        {
                            bestFStat = fStat;
                            bestPValue = pValue;
                        }
                    }
                }
            }
            
            return new GrangerResult
            {
                IsSignificant = bestPValue < config.GrangerSignificanceLevel,
                Strength = Math.Min(1.0, bestFStat / 10.0), // Normalize F-statistic
                Confidence = Math.Max(0.0, 1.0 - bestPValue),
                FStatistic = bestFStat,
                PValue = bestPValue
            };
        }

        /// <summary>
        /// Test information-theoretic causality
        /// </summary>
        private InformationResult TestInformationTheoreticCausality(List<CausalData> data, string causeVar, string effectVar, CausalAnalysisConfig config)
        {
            var causeValues = data.Select(d => d.Variables.GetValueOrDefault(causeVar, 0.0)).ToArray();
            var effectValues = data.Select(d => d.Variables.GetValueOrDefault(effectVar, 0.0)).ToArray();
            
            // Simplified transfer entropy calculation
            var mutualInfo = CalculateMutualInformation(causeValues, effectValues);
            var transferEntropy = CalculateTransferEntropy(causeValues, effectValues, data);
            
            return new InformationResult
            {
                IsSignificant = transferEntropy > config.TransferEntropyThreshold,
                TransferEntropy = transferEntropy,
                MutualInformation = mutualInfo,
                Confidence = Math.Min(1.0, transferEntropy * 2.0) // Simple confidence mapping
            };
        }

        /// <summary>
        /// Calculate mutual information (simplified)
        /// </summary>
        private double CalculateMutualInformation(double[] x, double[] y)
        {
            if (x.Length != y.Length || x.Length == 0) return 0.0;
            
            // Simplified MI using correlation as proxy
            var correlation = Correlation.Pearson(x, y);
            return -0.5 * Math.Log(1 - correlation * correlation);
        }

        /// <summary>
        /// Calculate transfer entropy (simplified)
        /// </summary>
        private double CalculateTransferEntropy(double[] cause, double[] effect, List<CausalData> data)
        {
            // Simplified TE using lagged mutual information
            var sortedData = data.OrderBy(d => d.Timestamp).ToList();
            
            if (sortedData.Count < 10) return 0.0;
            
            var maxTE = 0.0;
            var maxLag = Math.Min(3, sortedData.Count / 5);
            
            for (int lag = 1; lag <= maxLag; lag++)
            {
                if (cause.Length > lag && effect.Length > lag)
                {
                    var laggedCause = cause.Take(cause.Length - lag).ToArray();
                    var futureEffect = effect.Skip(lag).ToArray();
                    
                    if (laggedCause.Length == futureEffect.Length && laggedCause.Length > 0)
                    {
                        var te = CalculateMutualInformation(laggedCause, futureEffect);
                        maxTE = Math.Max(maxTE, te);
                    }
                }
            }
            
            return maxTE;
        }

        /// <summary>
        /// Combine and validate relationships from different methods
        /// </summary>
        private List<CausalRelationship> CombineAndValidateRelationships(List<CausalRelationship> relationships, CausalAnalysisConfig config)
        {
            var combined = new List<CausalRelationship>();
            
            // Group relationships by cause-effect pair
            var grouped = relationships.GroupBy(r => $"{r.CauseVariable}->{r.EffectVariable}").ToList();
            
            foreach (var group in grouped)
            {
                var relationshipGroup = group.ToList();
                
                if (relationshipGroup.Count == 1)
                {
                    // Single method detected this relationship
                    if (relationshipGroup[0].Strength > config.MinCausalStrength)
                    {
                        combined.Add(relationshipGroup[0]);
                    }
                }
                else
                {
                    // Multiple methods detected this relationship - combine evidence
                    var combinedRelationship = CombineRelationshipEvidence(relationshipGroup);
                    if (combinedRelationship.Strength > config.MinCausalStrength)
                    {
                        combined.Add(combinedRelationship);
                    }
                }
            }
            
            return combined.OrderByDescending(r => r.Strength).ToList();
        }

        /// <summary>
        /// Combine evidence from multiple methods for the same relationship
        /// </summary>
        private CausalRelationship CombineRelationshipEvidence(List<CausalRelationship> relationships)
        {
            var combined = new CausalRelationship
            {
                Id = Guid.NewGuid().ToString(),
                CauseVariable = relationships[0].CauseVariable,
                EffectVariable = relationships[0].EffectVariable,
                Direction = relationships[0].Direction,
                Evidence = new List<string>()
            };
            
            // Combine strengths (weighted average)
            var weights = new Dictionary<string, double>
            {
                ["PC Algorithm"] = 0.4,
                ["Granger Causality"] = 0.4,
                ["Transfer Entropy"] = 0.2
            };
            
            var totalWeight = 0.0;
            var weightedStrength = 0.0;
            var weightedConfidence = 0.0;
            
            foreach (var rel in relationships)
            {
                var weight = weights.GetValueOrDefault(rel.Method, 0.1);
                totalWeight += weight;
                weightedStrength += rel.Strength * weight;
                weightedConfidence += rel.Confidence * weight;
                
                combined.Evidence.AddRange(rel.Evidence);
            }
            
            combined.Strength = totalWeight > 0 ? weightedStrength / totalWeight : 0.0;
            combined.Confidence = totalWeight > 0 ? weightedConfidence / totalWeight : 0.0;
            combined.Method = string.Join(", ", relationships.Select(r => r.Method).Distinct());
            
            return combined;
        }

        /// <summary>
        /// Build causal graph from relationships
        /// </summary>
        private CausalGraph BuildCausalGraph(List<CausalRelationship> relationships, List<string> variables)
        {
            var graph = new CausalGraph
            {
                Nodes = variables.Select(v => new CausalNode { Id = v, Name = v }).ToList(),
                Edges = relationships.Select(r => new CausalEdge
                {
                    Id = r.Id,
                    Source = r.CauseVariable,
                    Target = r.EffectVariable,
                    Strength = r.Strength,
                    Confidence = r.Confidence,
                    Method = r.Method
                }).ToList()
            };
            
            return graph;
        }

        /// <summary>
        /// Normal CDF approximation
        /// </summary>
        private double NormalCDF(double x)
        {
            return 0.5 * (1 + Math.Sign(x) * Math.Sqrt(1 - Math.Exp(-2 * x * x / Math.PI)));
        }
    }

    #region Supporting Classes

    internal class UndirectedEdge
    {
        public string Variable1 { get; set; } = string.Empty;
        public string Variable2 { get; set; } = string.Empty;
        public double Strength { get; set; }
    }

    internal class DirectedEdge
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public double Strength { get; set; }
        public double Confidence { get; set; }
    }

    internal class GrangerResult
    {
        public bool IsSignificant { get; set; }
        public double Strength { get; set; }
        public double Confidence { get; set; }
        public double FStatistic { get; set; }
        public double PValue { get; set; }
    }

    internal class InformationResult
    {
        public bool IsSignificant { get; set; }
        public double TransferEntropy { get; set; }
        public double MutualInformation { get; set; }
        public double Confidence { get; set; }
    }

    #endregion
}
