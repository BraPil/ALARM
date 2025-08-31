# ALARM MCP Directive Dictionary

## Command Reference for Cursor Integration

Use these directive tags in your Cursor prompts to execute specific MCP operations with consistent behavior.

### Core Directives

#### `[INDEX]`
**Purpose**: Crawl the repository and build comprehensive symbol/dependency graph.

**Usage**:
```
[INDEX] Build complete index of ADDS legacy codebase
- Target: app-legacy directory
- Include: All C#, C++, config, and script files
- Output: index.json + index.md + risk_assessment.json
```

**Outputs**:
- Symbol graph with cross-references
- External API usage catalog
- Risk assessment with severity levels
- Updated file manifest

#### `[PLAN]`
**Purpose**: Generate detailed migration plan with sequenced PRs.

**Usage**:
```
[PLAN] Create 12-PR migration sequence for .NET 8 + AutoCAD 2025 + Oracle 19
- Priority: Critical path first (launcher, core services)
- Risk: Low-risk changes before high-risk
- Dependencies: Respect build dependencies
```

**Outputs**:
- Sequenced PR plan with themes
- Risk assessment per PR
- Dependency mapping
- Effort estimates

#### `[ADAPT]`
**Purpose**: Generate adapter/shim code for external API integration.

**Usage**:
```
[ADAPT] AutoCAD 2025 adapter interfaces
- Create: IAcadDatabase, IAcadTransaction, ISelectionService
- Implement: Concrete adapters using Autodesk 2025 APIs
- Isolate: All P/Invoke and COM interop in /interop layer
```

**Outputs**:
- Interface definitions
- Concrete adapter implementations
- Test doubles for unit testing
- Migration examples

#### `[REFACTOR:<pattern>]`
**Purpose**: Apply atomic code modifications using Roslyn analyzers.

**Usage**:
```
[REFACTOR:framework-to-core] Convert Framework APIs to .NET Core
- Target: ConfigurationManager → IOptions<T>
- Target: WebClient → HttpClient
- Target: Thread.Sleep → Task.Delay (async contexts)
- Scope: Max 300 LOC per PR
```

**Outputs**:
- Roslyn analyzer/codefix implementations
- Automated code changes
- PR with before/after diffs
- Unit tests for analyzers

#### `[TEST]`
**Purpose**: Create/expand test coverage for changed components.

**Usage**:
```
[TEST] Add comprehensive tests for Oracle adapter
- Unit: Mock all external dependencies
- Integration: Real Oracle connection (CI secrets)
- Golden Files: For complex query results
- Coverage: >80% line coverage target
```

**Outputs**:
- xUnit test projects
- Test doubles and mocks
- Integration test harness
- Coverage reports

#### `[SMOKE]`
**Purpose**: Create/update end-to-end validation tests.

**Usage**:
```
[SMOKE] CLI smoke test for core application flow
- Validate: Configuration loading
- Validate: Oracle connectivity (optional)
- Validate: AutoCAD adapter initialization
- Validate: Core business logic paths
```

**Outputs**:
- Smoke test console application
- Test scenario matrix
- CI integration scripts
- Pass/fail reporting

#### `[VERIFY]`
**Purpose**: Run complete quality gate validation.

**Usage**:
```
[VERIFY] Execute all quality gates for current changes
- Build: Compile all projects
- Test: Unit + integration + smoke
- Analysis: Static analysis + security scan
- Performance: Baseline comparison
```

**Outputs**:
- Gate status report (pass/fail)
- Performance metrics
- Coverage reports
- Issue summary with owners

#### `[LOG]`
**Purpose**: Generate run artifacts and update tracking.

**Usage**:
```
[LOG] Record MCP run results and update tracking
- Artifacts: JSON logs + markdown summary
- Metrics: Duration, success rate, coverage delta
- Attention: Flag failing tests or repeated issues
```

**Outputs**:
- Timestamped run directory
- Structured JSON logs
- Executive summary
- Trend analysis

#### `[RISK]`
**Purpose**: Maintain risk register with mitigation strategies.

**Usage**:
```
[RISK] Update risk register for Oracle migration
- Assess: Connection string security
- Assess: Transaction handling changes
- Assess: Performance impact
- Mitigation: Rollback procedures
```

**Outputs**:
- Updated risk register
- Mitigation plans
- Owner assignments
- Review schedules

#### `[REVIEW]`
**Purpose**: Generate PR review checklist and code maps.

**Usage**:
```
[REVIEW] Create review checklist for AutoCAD adapter PR
- Checklist: API isolation verified
- Checklist: Tests cover error cases
- Checklist: Documentation updated
- Code Map: Show affected components
```

**Outputs**:
- PR review template
- Code impact visualization
- Acceptance criteria
- Testing instructions

#### `[RELEASE]`
**Purpose**: Prepare release artifacts and documentation.

**Usage**:
```
[RELEASE] Tag and prepare artifacts for milestone release
- Version: Semantic versioning
- Notes: Feature summary + breaking changes
- Artifacts: Binaries + documentation
- Deploy: Production deployment checklist
```

**Outputs**:
- Git tags and releases
- Release notes
- Deployment artifacts
- Rollback procedures

### Composite Directives

#### `[INDEX] → [PLAN]`
**Purpose**: Full discovery and planning sequence.

**Usage**:
```
[INDEX] → [PLAN] Complete assessment and migration planning
- Index the entire ADDS legacy codebase
- Generate 12-PR migration sequence
- Identify critical path dependencies
- Estimate effort and risk levels
```

#### `[ADAPT] → [TEST] → [VERIFY]`
**Purpose**: Adapter development with full validation.

**Usage**:
```
[ADAPT] → [TEST] → [VERIFY] Oracle adapter with complete testing
- Create Oracle 19 adapter interfaces and implementations
- Build comprehensive test suite (unit + integration)
- Run full quality gate validation
- Generate PR with review checklist
```

#### `[REFACTOR] → [TEST] → [SMOKE] → [VERIFY]`
**Purpose**: Safe refactoring with comprehensive validation.

**Usage**:
```
[REFACTOR:config-migration] → [TEST] → [SMOKE] → [VERIFY]
- Convert app.config to appsettings.json
- Update all tests for new configuration system
- Validate smoke tests pass with new config
- Run complete quality gates
```

### Directive Parameters

#### Target Scoping
- `--scope=<path>`: Limit operation to specific directory
- `--files=<pattern>`: Target specific file patterns
- `--exclude=<pattern>`: Exclude specific patterns

#### Quality Gates
- `--coverage=<percent>`: Set minimum coverage threshold
- `--performance=<percent>`: Set maximum performance degradation
- `--complexity=<value>`: Set cyclomatic complexity limit

#### Output Control
- `--format=json|markdown|both`: Control output format
- `--verbose`: Include detailed diagnostic information
- `--quiet`: Minimal output (errors only)

### Example Cursor Prompts

#### Initial Setup
```
SYSTEM ROLE: You are my ALARM MCP Refactor Orchestrator. Follow directives exactly.

CONTEXT:
- Legacy App: ADDS (C:\Users\kidsg\Downloads\ADDS\ADDS25v1_Clean\)
- Target: .NET 8, AutoCAD Map 3D 2025, Oracle 19
- Patterns: Adapter isolation, incremental migration, test-first

TASK: [INDEX] → [PLAN]
Build complete index of ADDS codebase and generate 12-PR migration plan.
Use manifests in /mcp/manifests/ for guidance.
Output to /mcp_runs/<timestamp>/ with JSON + markdown.
```

#### Adapter Creation
```
[ADAPT] AutoCAD 2025 Integration Layer
- Create IAcadDatabase, IAcadTransaction, ISelectionService interfaces
- Implement concrete adapters using Map3D 2025 APIs
- Isolate all Autodesk dependencies in /app-core/adapters/
- Provide test doubles for unit testing
- Show migration example for 1-2 existing call sites
```

#### Refactoring Sprint
```
[REFACTOR:framework-to-core] Configuration System Migration
- Convert ConfigurationManager.AppSettings to IOptions<T>
- Replace app.config with appsettings.json
- Update dependency injection registration
- Limit scope to 300 LOC maximum
- Include unit tests for configuration loading
```

### Error Handling

#### Directive Failures
- Log failure reason to `/mcp_runs/<timestamp>/errors.json`
- Provide specific remediation steps
- Update attention queue for repeated failures
- Generate incident report for critical failures

#### Recovery Procedures
- `[ROLLBACK]`: Revert to last known good state
- `[RETRY]`: Re-execute failed directive with same parameters
- `[DIAGNOSE]`: Deep dive analysis of failure root cause
- `[ESCALATE]`: Flag for manual intervention

This directive system provides the structured commands needed to drive the ALARM MCP system through Cursor with consistent, predictable behavior.
