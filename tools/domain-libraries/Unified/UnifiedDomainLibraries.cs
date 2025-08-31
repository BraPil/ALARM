using Microsoft.Extensions.Logging;
using ALARM.DomainLibraries.AutoCAD;
using ALARM.DomainLibraries.Oracle;
using ALARM.DomainLibraries.DotNetCore;
using ALARM.DomainLibraries.ADDS;
using ALARM.Analyzers.PatternDetection;

namespace ALARM.DomainLibraries
{
    /// <summary>
    /// Unified access point for all domain-specific libraries
    /// </summary>
    public class UnifiedDomainLibraries
    {
        private readonly DomainLibraryManager _manager;

        public UnifiedDomainLibraries(ILogger<UnifiedDomainLibraries> logger)
        {
            var managerLogger = LoggerFactory.Create(builder => builder.AddConsole())
                .CreateLogger<DomainLibraryManager>();
            _manager = new DomainLibraryManager(managerLogger);
            
            RegisterAllDomainLibraries();
        }

        /// <summary>
        /// Gets the domain library manager with all registered libraries
        /// </summary>
        public DomainLibraryManager Manager => _manager;

        /// <summary>
        /// Registers all available domain libraries
        /// </summary>
        private void RegisterAllDomainLibraries()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            
            _manager.RegisterDomainLibrary(new AutoCADPatterns(loggerFactory.CreateLogger<AutoCADPatterns>()));
            _manager.RegisterDomainLibrary(new OraclePatterns(loggerFactory.CreateLogger<OraclePatterns>()));
            _manager.RegisterDomainLibrary(new DotNetCorePatterns(loggerFactory.CreateLogger<DotNetCorePatterns>()));
            _manager.RegisterDomainLibrary(new ADDSPatterns(loggerFactory.CreateLogger<ADDSPatterns>()));
        }

        /// <summary>
        /// Convenience method for cross-domain pattern detection
        /// </summary>
        public async Task<CombinedDomainPatternResult> AnalyzePatternsAsync(List<PatternData> data)
        {
            var config = new PatternDetectionConfig();
            return await _manager.DetectPatternsAsync(data, config);
        }

        /// <summary>
        /// Convenience method for cross-domain validation
        /// </summary>
        public async Task<CombinedDomainValidationResult> ValidateContentAsync(string content, string contentType)
        {
            var config = new DomainValidationConfig();
            return await _manager.ValidateAsync(content, contentType, config);
        }

        /// <summary>
        /// Get all registered domain library names
        /// </summary>
        public IEnumerable<string> GetAvailableDomains()
        {
            return _manager.GetRegisteredLibraries().Keys;
        }
    }
}
