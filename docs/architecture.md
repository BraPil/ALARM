# ALARM Architecture Documentation

## Overview

ALARM (Automated Legacy App Refactoring MCP) is a comprehensive system designed to systematically modernize legacy applications to work with .NET 8, AutoCAD Map 3D 2025, and Oracle 19c. The system follows a structured, protocol-driven approach with strong emphasis on adapter patterns and incremental migration.

## Architecture Principles

### 1. Adapter Isolation Pattern
- **Domain Layer**: Pure C# business logic with no external dependencies
- **Adapter Layer**: Implements domain interfaces, isolates external APIs
- **Interop Layer**: Contains P/Invoke, COM, and native API calls

```
┌─────────────────────────────────────┐
│           Domain Layer              │
│        (Pure C# Logic)              │
│                                     │
│  ┌─────────────────────────────┐    │
│  │     Business Services       │    │
│  │     Domain Models          │    │
│  │     Interfaces Only        │    │
│  └─────────────────────────────┘    │
└─────────────────────────────────────┘
                  │
                  ▼ (Interfaces Only)
┌─────────────────────────────────────┐
│           Adapter Layer             │
│    (External API Abstractions)     │
│                                     │
│  ┌─────────────┐  ┌─────────────┐   │
│  │   AutoCAD   │  │   Oracle    │   │
│  │  Adapter    │  │  Adapter    │   │
│  └─────────────┘  └─────────────┘   │
└─────────────────────────────────────┘
                  │
                  ▼ (P/Invoke, COM)
┌─────────────────────────────────────┐
│           Interop Layer             │
│     (Native API Boundaries)        │
│                                     │
│  ┌─────────────┐  ┌─────────────┐   │
│  │    ARX      │  │   ODP.NET   │   │
│  │  Interop    │  │   Native    │   │
│  └─────────────┘  └─────────────┘   │
└─────────────────────────────────────┘
```

### 2. Dependency Injection and Configuration
- Microsoft.Extensions.DependencyInjection for IoC container
- Microsoft.Extensions.Configuration for settings management
- Microsoft.Extensions.Logging for structured logging
- Microsoft.Extensions.Hosting for application lifecycle

### 3. Test-Driven Development
- Unit tests with mocked dependencies
- Integration tests with real external systems
- Smoke tests for end-to-end validation
- Golden file tests for complex outputs

## System Components

### Core Domain Layer (`ALARM.Core`)

**Interfaces**:
- `IAutoCadService` - Main AutoCAD integration service
- `IAcadDocument` - Document abstraction
- `IAcadTransaction` - Transaction management
- `ISelectionService` - Entity selection operations
- `ILayerService` - Layer management
- `IOracleService` - Main Oracle integration service
- `IOracleConnectionFactory` - Connection management
- `IOracleDataService` - High-level data operations
- `IOracleCommandBuilder` - Query building

**Models**:
- Domain entities and value objects
- Configuration POCOs
- Event arguments and enums

### AutoCAD Adapter (`ALARM.Adapters.AutoCAD`)

**Responsibilities**:
- Implement AutoCAD interfaces using Map 3D 2025 APIs
- Handle AutoCAD-specific error conditions
- Manage document lifecycle and transactions
- Provide test doubles for unit testing

**Key Classes**:
- `AutoCadService` - Main service implementation
- `AcadDocument` - Document wrapper
- `AcadTransaction` - Transaction wrapper
- `SelectionService` - Selection operations
- `LayerService` - Layer management
- `TestDoubles/MockAutoCadService` - For unit testing

### Oracle Adapter (`ALARM.Adapters.Oracle`)

**Responsibilities**:
- Implement Oracle interfaces using ODP.NET Core
- Provide connection pooling and health checks
- Handle Oracle-specific error conditions and retries
- Support both sync and async operations

**Key Classes**:
- `OracleService` - Main service implementation
- `OracleConnectionFactory` - Connection management
- `OracleDataService` - High-level data operations
- `OracleCommandBuilder` - Query building
- `TestDoubles/MockOracleService` - For unit testing

### Interop Layer (`ALARM.Interop`)

**Responsibilities**:
- Isolate all P/Invoke and COM interop calls
- Provide safe wrappers for native API calls
- Handle marshaling and memory management
- Platform-specific implementations

**Structure**:
```
ALARM.Interop/
├── AutoCAD/
│   ├── ArxInterop.cs        # ARX P/Invoke declarations
│   ├── ComInterop.cs        # COM interface definitions
│   └── SafeHandles.cs       # Safe handle implementations
├── Oracle/
│   ├── OciInterop.cs        # OCI native calls (if needed)
│   └── SafeHandles.cs       # Oracle-specific handles
└── Common/
    ├── Win32Interop.cs      # Common Win32 APIs
    └── ErrorHandling.cs     # Native error handling
```

## Migration Strategy

### Phase 1: Foundation (PRs 1-4)
1. **Indexing and Assessment**
   - Run indexer tool on legacy codebase
   - Generate risk assessment and migration plan
   - Set up CI/CD pipeline

2. **Adapter Framework**
   - Create core interfaces and abstractions
   - Implement test doubles and mocks
   - Basic adapter implementations

3. **Configuration Migration**
   - Convert app.config to appsettings.json
   - Implement options pattern
   - Set up dependency injection

4. **Testing Infrastructure**
   - Unit test framework setup
   - Integration test harness
   - Smoke test CLI tool

### Phase 2: Core Migration (PRs 5-8)
5. **Oracle Adapter Implementation**
   - Full ODP.NET Core integration
   - Connection factory and health checks
   - Data service implementation
   - Migration of 2-3 key DAL classes

6. **AutoCAD Adapter Implementation**
   - Map 3D 2025 API integration
   - Document and transaction management
   - Selection and layer services
   - Migration of 2-3 key AutoCAD integration points

7. **Framework API Migration**
   - Roslyn analyzers for automated refactoring
   - WebClient → HttpClient
   - ConfigurationManager → IOptions
   - Thread.Sleep → Task.Delay

8. **Domain Layer Isolation**
   - Remove direct external API dependencies
   - Implement adapter pattern throughout
   - Update unit tests

### Phase 3: Optimization (PRs 9-12)
9. **Performance Optimization**
   - Async/await pattern implementation
   - Connection pooling optimization
   - Memory usage optimization

10. **Advanced Features**
    - Error handling and retry policies
    - Logging and telemetry
    - Health checks and monitoring

11. **UI Modernization**
    - WinForms/WPF isolation behind interfaces
    - Dependency injection in UI layer
    - Modern styling and UX improvements

12. **Deployment and Packaging**
    - MSI installer creation
    - Deployment automation
    - Version management

## Quality Gates

### Build Gates
- ✅ Solution compiles without errors
- ✅ All projects target .NET 8
- ✅ No direct references to deprecated APIs in domain layer
- ✅ Roslyn analyzers pass

### Test Gates
- ✅ Unit tests: >80% code coverage
- ✅ Integration tests: All critical paths covered
- ✅ Smoke tests: End-to-end scenarios pass
- ✅ Performance tests: No >5% degradation

### Security Gates
- ✅ No hardcoded connection strings or secrets
- ✅ SQL injection prevention (parameterized queries)
- ✅ Secure configuration management
- ✅ Dependency vulnerability scan

### Documentation Gates
- ✅ API documentation complete
- ✅ Migration guide updated
- ✅ Architecture decision records (ADRs)
- ✅ Deployment runbooks

## Monitoring and Observability

### Logging Strategy
- **Structured Logging**: JSON format with correlation IDs
- **Log Levels**: Appropriate use of Trace/Debug/Info/Warning/Error/Critical
- **Contextual Information**: User ID, session ID, operation context
- **Performance Metrics**: Operation duration, resource usage

### Health Checks
- **Database Connectivity**: Oracle connection health
- **AutoCAD Availability**: Application responsiveness
- **External Dependencies**: Third-party service health
- **Resource Utilization**: Memory, CPU, disk usage

### Error Handling
- **Graceful Degradation**: Fallback mechanisms for non-critical failures
- **Retry Policies**: Exponential backoff with jitter
- **Circuit Breakers**: Prevent cascade failures
- **User-Friendly Messages**: Clear error communication

## Development Workflow

### MCP Directive Usage
```
# Initial assessment
[INDEX] → [PLAN] Complete codebase analysis and migration planning

# Adapter development
[ADAPT] AutoCAD 2025 → [TEST] → [VERIFY] Full cycle for each adapter

# Code refactoring
[REFACTOR:framework-to-core] → [TEST] → [SMOKE] → [VERIFY] Safe refactoring

# Quality assurance
[VERIFY] → [LOG] → [REVIEW] Comprehensive quality checks
```

### Branching Strategy
- `main` - Production-ready code
- `develop` - Integration branch for features
- `feature/adapter-autocad` - Feature branches
- `hotfix/critical-bug` - Emergency fixes

### Code Review Process
1. **Automated Checks**: Build, test, analysis gates
2. **Peer Review**: Architecture, code quality, test coverage
3. **Documentation Review**: API docs, migration notes
4. **Security Review**: Vulnerability assessment

## Risk Management

### High-Risk Areas
- **Native Interop**: P/Invoke and COM integration
- **Database Migration**: Schema and data changes
- **AutoCAD Integration**: Version-specific API changes
- **Performance**: Large dataset processing

### Mitigation Strategies
- **Comprehensive Testing**: Unit, integration, smoke tests
- **Gradual Rollout**: Feature flags and canary deployments
- **Rollback Procedures**: Quick revert mechanisms
- **Monitoring**: Real-time health and performance monitoring

## Future Considerations

### Extensibility
- Plugin architecture for custom adapters
- Configuration-driven behavior
- Extensible logging and monitoring

### Scalability
- Horizontal scaling support
- Distributed processing capabilities
- Cloud deployment options

### Maintainability
- Clear separation of concerns
- Comprehensive documentation
- Automated testing and deployment
