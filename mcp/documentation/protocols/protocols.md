# ALARM MCP Protocols

## Core Protocols for Automated Legacy App Refactoring

### 1. Indexing Protocol

**Purpose**: Systematically catalog every file, symbol, dependency, and external API in the legacy codebase.

**Steps**:
1. **File Discovery**: Use ripgrep to enumerate all source files based on `files.manifest.json` patterns
2. **Symbol Extraction**: Use Roslyn to build symbol graphs (types, methods, fields, properties)
3. **Dependency Mapping**: Map file→symbol and symbol→symbol references
4. **External API Detection**: Identify usage of AutoCAD, Oracle, and deprecated Framework APIs
5. **Risk Assessment**: Flag high-risk areas (P/Invoke, unsafe code, complex interop)
6. **Output Generation**: Produce structured JSON index and human-readable markdown summary

**Artifacts**:
- `/mcp_runs/<timestamp>/index.json` - Complete symbol and dependency graph
- `/mcp_runs/<timestamp>/index.md` - Human-readable summary
- `/mcp_runs/<timestamp>/external_apis.json` - External API usage catalog
- `/mcp_runs/<timestamp>/risk_assessment.json` - Risk areas and severity

### 2. Refactor Protocol

**Purpose**: Execute atomic, testable, incremental changes with full traceability.

**Rules**:
- **Atomic Changes**: Each PR addresses one specific concern (max 300 LOC changed)
- **Test-First**: Tests must be written/updated before implementation changes
- **Adapter Isolation**: Domain code never directly calls AutoCAD or Oracle APIs
- **Rollback Ready**: Every change must be easily reversible
- **Documentation**: Every PR includes updated documentation

**PR Workflow**:
1. **Planning**: Create detailed plan with affected files and test strategy
2. **Branch Creation**: Use descriptive branch names (`refactor/oracle-adapter`, `migrate/config-system`)
3. **Implementation**: Make changes following adapter patterns
4. **Testing**: Run unit, integration, and smoke tests
5. **Review**: Use generated reviewer checklist
6. **Merge**: Only after all gates pass (build, test, analysis)

### 3. Shim/Adapter Protocol

**Purpose**: Isolate external dependencies behind clean interfaces.

**Adapter Requirements**:
- **Interface-First**: Define clean abstractions before implementation
- **Testability**: Provide test doubles for all adapters
- **Error Handling**: Consistent error handling and logging
- **Async Support**: Use async/await patterns where appropriate
- **Resource Management**: Proper disposal and connection management

**Adapter Layers**:
```
Domain Layer (Pure C#)
    ↓ (interfaces only)
Adapter Layer (Implements interfaces)
    ↓ (isolates external APIs)
Interop Layer (P/Invoke, COM, native calls)
```

### 4. Test Protocol

**Purpose**: Ensure comprehensive test coverage with multiple test types.

**Test Types**:
- **Unit Tests**: Fast, isolated, test single components
- **Integration Tests**: Test adapter implementations with real dependencies
- **Smoke Tests**: End-to-end validation of critical paths
- **Golden File Tests**: For complex geometry/graphics outputs

**Test Standards**:
- **Naming**: `[MethodName]_[Scenario]_[ExpectedResult]`
- **AAA Pattern**: Arrange, Act, Assert
- **Coverage**: Minimum 80% line coverage for new/changed code
- **Performance**: Tests must complete in <5 seconds each

### 5. Logging Protocol

**Purpose**: Comprehensive logging for debugging, monitoring, and audit trails.

**Log Levels**:
- **Trace**: Detailed execution flow (development only)
- **Debug**: Diagnostic information for troubleshooting
- **Information**: General application flow and milestones
- **Warning**: Unexpected but recoverable conditions
- **Error**: Error conditions that don't stop the application
- **Critical**: Serious errors that may cause the application to terminate

**Structured Logging**:
```json
{
  "timestamp": "2024-01-01T12:00:00Z",
  "level": "Information",
  "message": "Oracle connection established",
  "properties": {
    "connectionString": "redacted",
    "duration": "150ms",
    "operation": "database_connect"
  }
}
```

### 6. Rollback Protocol

**Purpose**: Quick recovery from failed deployments or breaking changes.

**Rollback Triggers**:
- Build failures in main branch
- Critical test failures
- Performance degradation >20%
- Production issues within 24 hours

**Rollback Steps**:
1. **Immediate**: Revert to last known good commit
2. **Notification**: Alert team of rollback and reason
3. **Analysis**: Root cause analysis of failure
4. **Fix Forward**: Address issues in new PR
5. **Deployment**: Re-deploy with fixes

### 7. Process Logging Protocol

**Purpose**: Track MCP execution and maintain audit trail.

**Run Logging**:
- Every MCP directive execution logged to `/mcp_runs/<timestamp>/`
- Include command, parameters, duration, success/failure
- Capture stdout/stderr for debugging
- Generate summary reports

**Metrics Tracking**:
- Code coverage trends
- Test execution times
- Build duration
- Deployment frequency
- Mean time to recovery (MTTR)

### 8. Retesting Protocol

**Purpose**: Iterative improvement until all quality gates pass.

**Quality Gates**:
1. **Build**: Solution compiles without errors
2. **Unit Tests**: All unit tests pass with >80% coverage
3. **Integration Tests**: All integration tests pass
4. **Smoke Tests**: Critical path validation
5. **Static Analysis**: No high-severity issues
6. **Performance**: No degradation >5% on key metrics

**Iteration Process**:
1. Run all quality gates
2. If any fail, analyze and fix
3. Re-run failed gates plus dependencies
4. Repeat until all gates pass
5. Generate success report and artifacts
