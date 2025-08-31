# ALARM Cursor Prompt Guide

## Quick Reference for Using ALARM MCP System

This document provides copy-paste ready prompts for using the ALARM (Automated Legacy App Refactoring MCP) system with Cursor.

## System Setup Prompt

```
SYSTEM ROLE: You are my ALARM MCP Refactor Orchestrator. Follow MCP directives exactly.

CONTEXT:
- Project: ALARM (Automated Legacy App Refactoring CI/CD MCP)
- Legacy App: ADDS (C:\Users\kidsg\Downloads\ADDS\ADDS25v1_Clean\)
- Target Stack: .NET 8, AutoCAD Map 3D 2025, Oracle 19c
- Architecture: Adapter pattern isolation, incremental migration, test-first approach
- Manifests: Located in /mcp/manifests/ (files.manifest.json, apis.manifest.json)
- Protocols: Located in /mcp/protocols/ and /mcp/directives/

RULES:
1. Domain layer NEVER directly calls AutoCAD or Oracle APIs
2. All external dependencies isolated behind adapter interfaces
3. Every change must be atomic, testable, and reversible
4. Maximum 300 LOC changes per PR
5. Tests written before implementation changes
6. All outputs logged to /mcp_runs/<timestamp>/

AVAILABLE DIRECTIVES:
[INDEX], [PLAN], [ADAPT], [REFACTOR:<pattern>], [TEST], [SMOKE], [VERIFY], [LOG], [RISK], [REVIEW], [RELEASE]
```

## Initial Assessment and Planning

### Complete Codebase Analysis
```
TASK: [INDEX] → [PLAN]

Perform complete assessment of ADDS legacy application:

1. [INDEX] Analyze the legacy codebase at C:\Users\kidsg\Downloads\ADDS\ADDS25v1_Clean\
   - Use manifests in /mcp/manifests/ for file categorization
   - Generate symbol graph and dependency mapping
   - Identify all AutoCAD, Oracle, and Framework API usage
   - Assess risk levels and complexity
   - Output to /mcp_runs/<timestamp>/

2. [PLAN] Create detailed 12-PR migration sequence
   - Prioritize by risk level (low-risk first)
   - Respect dependency order
   - Include effort estimates and acceptance criteria
   - Generate reviewer checklists
   - Document rollback procedures

Expected outputs:
- index.json with complete symbol catalog
- index.md with human-readable summary
- external_apis.json with API usage analysis
- risk_assessment.json with prioritized issues
- migration_plan.md with sequenced PRs
```

### Risk Assessment Update
```
TASK: [RISK]

Update risk register for current migration phase:
- Analyze high-risk files with >5 external API dependencies
- Assess impact of AutoCAD Map 3D 2025 breaking changes
- Evaluate Oracle 19c migration complexity
- Document mitigation strategies
- Assign owners and due dates
- Update attention queue for repeated failures

Output: Updated risk_register.md with current status
```

## Adapter Development

### AutoCAD 2025 Adapter Creation
```
TASK: [ADAPT] AutoCAD Map 3D 2025 Integration

Create comprehensive AutoCAD adapter layer:

INTERFACES TO IMPLEMENT:
- IAutoCadService (main service entry point)
- IAcadDocument (document abstraction)
- IAcadTransaction (transaction management)
- ISelectionService (entity selection)
- ILayerService (layer management)
- IAcadDatabase (database operations)

REQUIREMENTS:
- Use AutoCAD Map 3D 2025 APIs exclusively
- Isolate all Autodesk.* dependencies in adapter layer
- Provide test doubles for unit testing
- Handle version-specific breaking changes
- Implement proper resource disposal
- Support both sync and async operations

DELIVERABLES:
1. Complete adapter implementations in /app-core/adapters/ALARM.Adapters.AutoCAD/
2. Test doubles in TestDoubles/ subdirectory
3. Integration tests demonstrating functionality
4. Migration example showing before/after for 2-3 existing call sites
5. Documentation of breaking changes and workarounds

VALIDATION:
- No Autodesk.* references in domain layer
- All adapter methods have corresponding test doubles
- Integration tests pass with real AutoCAD installation
- Performance benchmarks within 10% of direct API calls
```

### Oracle 19c Adapter Creation
```
TASK: [ADAPT] Oracle 19c Integration with ODP.NET Core

Create comprehensive Oracle adapter layer:

INTERFACES TO IMPLEMENT:
- IOracleService (main service entry point)
- IOracleConnectionFactory (connection management)
- IOracleDataService (high-level data operations)
- IOracleCommandBuilder (query building)
- IOracleConnection, IOracleCommand, IOracleDataReader abstractions

REQUIREMENTS:
- Use Oracle.ManagedDataAccess.Core exclusively
- Implement connection pooling and health checks
- Support async/await patterns throughout
- Provide parameterized query building
- Handle Oracle-specific error conditions
- Implement retry policies with exponential backoff

DELIVERABLES:
1. Complete adapter implementations in /app-core/adapters/ALARM.Adapters.Oracle/
2. Connection factory with health monitoring
3. High-level data service with CRUD operations
4. Command builder with fluent API
5. Test doubles for unit testing
6. Migration example for 2-3 existing DAL classes

VALIDATION:
- No direct Oracle.* references in domain layer
- Connection health checks functional
- Async operations properly implemented
- SQL injection prevention verified
- Performance tests show acceptable overhead
```

## Code Refactoring

### Framework to .NET Core Migration
```
TASK: [REFACTOR:framework-to-core]

Apply automated refactoring for .NET Framework to .NET 8 migration:

TARGET PATTERNS:
1. ConfigurationManager.AppSettings → IOptions<T> with appsettings.json
2. WebClient → HttpClient with dependency injection
3. Thread.Sleep → Task.Delay in async contexts
4. DateTime.Now → DateTimeOffset.UtcNow (unless UI-specific)
5. AppDomain → AssemblyLoadContext
6. Remoting → gRPC or HTTP APIs
7. WCF → ASP.NET Core Web APIs

SCOPE LIMITATIONS:
- Maximum 300 LOC changes per execution
- Focus on one pattern type per run
- Prioritize high-impact, low-risk changes
- Maintain backward compatibility during transition

DELIVERABLES:
1. Roslyn analyzer project in /tools/analyzers/
2. Code fix providers for each pattern
3. Unit tests for analyzers
4. Automated application to target files
5. Before/after comparison report
6. Updated configuration files

VALIDATION:
- All unit tests pass after changes
- No regression in functionality
- Configuration properly migrated
- Dependency injection properly configured
```

### Configuration System Migration
```
TASK: [REFACTOR:config-migration]

Migrate configuration system from app.config to modern .NET 8 patterns:

MIGRATION TARGETS:
- app.config → appsettings.json + appsettings.Development.json
- ConfigurationManager.AppSettings → IOptions<T>
- ConnectionStrings → IOptions<ConnectionStrings>
- Custom config sections → strongly-typed options classes

IMPLEMENTATION:
1. Create strongly-typed configuration classes
2. Set up dependency injection for IOptions<T>
3. Migrate all ConfigurationManager calls
4. Add configuration validation
5. Support environment-specific overrides
6. Implement configuration change notifications

DELIVERABLES:
- Updated project files with configuration packages
- Strongly-typed configuration classes
- DI container registration
- Migration of existing configuration usage
- Unit tests for configuration loading
- Documentation of new configuration patterns
```

## Testing and Validation

### Comprehensive Test Suite Creation
```
TASK: [TEST] → [SMOKE] → [VERIFY]

Create comprehensive testing framework:

1. [TEST] Unit Test Implementation
   - xUnit with FluentAssertions
   - Moq for mocking dependencies
   - >80% code coverage requirement
   - Test all adapter implementations
   - Test configuration loading
   - Test error handling paths

2. [SMOKE] End-to-End Smoke Tests
   - CLI tool for automated validation
   - Configuration loading verification
   - Oracle connectivity test (optional via env var)
   - AutoCAD adapter initialization
   - Core business logic validation
   - Performance baseline measurement

3. [VERIFY] Quality Gate Validation
   - Build compilation check
   - Unit test execution (>80% coverage)
   - Integration test execution
   - Static analysis (Roslyn analyzers)
   - Security scan (dependency vulnerabilities)
   - Performance regression check (<5% degradation)

DELIVERABLES:
- Complete test projects in /app-core/tests/
- Smoke test CLI in /tools/smoke/
- CI/CD pipeline integration
- Test result reporting
- Coverage reports
- Performance benchmarks
```

### Integration Testing
```
TASK: [TEST] Integration Test Suite

Create integration tests for external dependencies:

ORACLE INTEGRATION:
- Real database connection tests
- CRUD operation validation
- Transaction handling verification
- Connection pooling tests
- Error handling and retry logic
- Performance benchmarking

AUTOCAD INTEGRATION:
- Document creation and manipulation
- Transaction management
- Selection operations
- Layer management
- Entity creation and modification
- Performance with large datasets

CONFIGURATION:
- Environment-specific settings
- Secret management
- Configuration change detection
- Validation error handling

REQUIREMENTS:
- Tests run in CI/CD with appropriate secrets
- Fallback to mocks when external systems unavailable
- Clear pass/fail criteria
- Performance baselines established
- Error conditions properly tested
```

## Deployment and Release

### Release Preparation
```
TASK: [RELEASE]

Prepare release artifacts and documentation:

RELEASE ARTIFACTS:
1. Compiled binaries for target platform
2. Configuration templates
3. Database migration scripts
4. Installation documentation
5. Upgrade procedures from legacy version
6. Rollback procedures

DOCUMENTATION:
1. Updated architecture documentation
2. API reference documentation
3. Migration guide for end users
4. Troubleshooting guide
5. Performance tuning guide
6. Security configuration guide

VALIDATION:
1. All quality gates pass
2. Integration tests successful
3. Performance benchmarks acceptable
4. Security scan clean
5. Documentation complete and accurate
6. Rollback procedures tested

DELIVERABLES:
- Release package with all artifacts
- Git tags and release notes
- Deployment runbooks
- Post-deployment validation checklist
- Communication plan for stakeholders
```

## Troubleshooting and Maintenance

### Diagnostic Analysis
```
TASK: [LOG] → [DIAGNOSE]

Analyze system issues and performance:

LOG ANALYSIS:
- Parse structured logs for error patterns
- Identify performance bottlenecks
- Track resource utilization trends
- Analyze user behavior patterns
- Generate executive summary

PERFORMANCE ANALYSIS:
- Compare current metrics to baselines
- Identify regression sources
- Recommend optimization strategies
- Validate fix effectiveness

HEALTH CHECK:
- Database connectivity and performance
- AutoCAD integration responsiveness
- External dependency health
- Resource utilization status
- Error rate trends

OUTPUT:
- Diagnostic report with findings
- Recommended actions with priorities
- Performance optimization plan
- Health status dashboard update
```

### Emergency Response
```
TASK: [ROLLBACK] → [DIAGNOSE] → [FIX]

Emergency response for production issues:

IMMEDIATE ACTIONS:
1. [ROLLBACK] Revert to last known good version
   - Identify rollback point
   - Execute rollback procedures
   - Verify system functionality
   - Notify stakeholders

2. [DIAGNOSE] Root cause analysis
   - Analyze logs and metrics
   - Identify failure point
   - Determine impact scope
   - Document findings

3. [FIX] Implement and validate fix
   - Develop targeted fix
   - Test in staging environment
   - Apply fix with monitoring
   - Validate resolution

DELIVERABLES:
- Incident report with timeline
- Root cause analysis
- Fix implementation and validation
- Process improvement recommendations
- Updated runbooks and procedures
```

## Custom Directive Combinations

### Full Migration Sprint
```
TASK: [INDEX] → [ADAPT] → [REFACTOR:framework-to-core] → [TEST] → [VERIFY] → [LOG]

Complete migration sprint for specific component:

1. Index component dependencies and risks
2. Create necessary adapters
3. Refactor Framework dependencies
4. Implement comprehensive tests
5. Validate all quality gates
6. Log results and update tracking

Target: Single component or feature area
Timeline: 1-2 week sprint
Success Criteria: All quality gates pass, no regressions
```

### Quality Assurance Cycle
```
TASK: [VERIFY] → [RISK] → [REVIEW] → [LOG]

Comprehensive quality assurance cycle:

1. Execute all quality gates
2. Update risk assessment
3. Generate review artifacts
4. Log results and trends

Output: Quality status report with recommendations
```

## Tips for Effective Usage

### Best Practices
1. **Start Small**: Begin with [INDEX] to understand the codebase
2. **Incremental Changes**: Use 300 LOC limit religiously
3. **Test First**: Always implement tests before production code
4. **Document Everything**: Update docs with every significant change
5. **Monitor Progress**: Use [LOG] frequently to track trends

### Common Patterns
- `[INDEX] → [PLAN]` - Initial assessment
- `[ADAPT] → [TEST] → [VERIFY]` - Adapter development
- `[REFACTOR] → [TEST] → [VERIFY]` - Safe refactoring
- `[VERIFY] → [LOG] → [REVIEW]` - Quality assurance

### Error Recovery
- Always have rollback procedures ready
- Use [DIAGNOSE] for systematic troubleshooting
- Update risk register after incidents
- Learn from failures and update protocols
