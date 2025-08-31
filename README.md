# ALARM - Automated Legacy App Refactoring CI/CD MCP

A comprehensive "poor man's MCP" (Model Context Protocol) system designed to systematically modernize legacy applications to work with .NET 8, AutoCAD Map 3D 2025, and Oracle 19c.

## 🎯 Overview

ALARM provides a structured, protocol-driven approach to legacy application modernization with strong emphasis on:
- **Adapter Pattern Isolation**: Domain code never directly calls external APIs
- **Incremental Migration**: Small, testable, reversible changes (max 300 LOC per PR)
- **Test-First Development**: Comprehensive testing at every layer
- **Automated Quality Gates**: CI/CD pipeline with multiple validation stages
- **Risk-Based Planning**: Prioritized migration sequence based on complexity analysis

## 🏗️ Architecture

```
┌─────────────────────────────────────┐
│           Domain Layer              │
│        (Pure C# Logic)              │
│  ┌─────────────────────────────┐    │
│  │  Business Services & Models │    │
│  │     (Interfaces Only)       │    │
│  └─────────────────────────────┘    │
└─────────────────────────────────────┘
                  │
                  ▼ (Interfaces Only)
┌─────────────────────────────────────┐
│           Adapter Layer             │
│  ┌─────────────┐  ┌─────────────┐   │
│  │   AutoCAD   │  │   Oracle    │   │
│  │  Adapter    │  │  Adapter    │   │
│  └─────────────┘  └─────────────┘   │
└─────────────────────────────────────┘
                  │
                  ▼ (P/Invoke, COM)
┌─────────────────────────────────────┐
│           Interop Layer             │
│  ┌─────────────┐  ┌─────────────┐   │
│  │    ARX      │  │   ODP.NET   │   │
│  │  Interop    │  │   Native    │   │
│  └─────────────┘  └─────────────┘   │
└─────────────────────────────────────┘
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# extension
- AutoCAD Map 3D 2025 (for full integration testing)
- Oracle 19c client (for database integration)

### Initial Setup
1. **Clone and examine the repository structure:**
   ```bash
   git clone <repository-url>
   cd ALARM
   ```

2. **Run the indexer to analyze your legacy codebase:**
   ```bash
   dotnet run --project tools/indexer/Indexer.csproj -- --legacy-path "C:\path\to\your\legacy\app" --verbose
   ```

3. **Review the generated analysis:**
   - Check `/mcp_runs/<timestamp>/index.md` for human-readable summary
   - Review `/mcp_runs/<timestamp>/risk_assessment.json` for prioritized issues
   - Examine `/mcp_runs/<timestamp>/external_apis.json` for API dependencies

## 📁 Repository Structure

```
ALARM/
├── app-legacy/              # Original legacy application (read-only after import)
├── app-core/                # Refactored .NET 8 solution
│   ├── src/ALARM.Core/      # Domain layer with interfaces
│   ├── adapters/            # AutoCAD and Oracle adapters
│   ├── interop/             # P/Invoke and COM interop layer
│   └── tests/               # Comprehensive test suites
├── tools/
│   ├── indexer/             # Codebase analysis and cataloging tool
│   ├── analyzers/           # Roslyn analyzers for automated refactoring
│   └── smoke/               # End-to-end validation CLI tool
├── mcp/
│   ├── manifests/           # System configuration (files.manifest.json, apis.manifest.json)
│   ├── protocols/           # Detailed protocol documentation
│   └── directives/          # MCP command reference
├── ci/                      # CI/CD pipeline configuration
├── docs/                    # Architecture and usage documentation
└── mcp_runs/               # Generated analysis and run artifacts
```

## 🔧 Core Tools

### 1. Indexer Tool
Analyzes legacy codebase and generates comprehensive symbol catalog:
```bash
dotnet run --project tools/indexer/Indexer.csproj -- --legacy-path "C:\path\to\legacy" --output-path "mcp_runs/analysis"
```

### 2. Smoke Test Tool
Validates system functionality end-to-end:
```bash
# Basic smoke tests
dotnet run --project tools/smoke/Smoke.csproj

# Include Oracle connectivity tests
dotnet run --project tools/smoke/Smoke.csproj -- --include-oracle

# Production validation (critical tests only)
dotnet run --project tools/smoke/Smoke.csproj -- --environment production --critical-only
```

### 3. Analyzer Tools
Advanced learning and pattern recognition system:
```bash
# Basic analysis
dotnet run --project tools/analyzers/Analyzers.csproj -- --target app-core --output mcp_runs/analysis

# Learning analysis with ML
dotnet run --project tools/analyzers/Analyzers.csproj -- --learn --scope "last-30-runs" --ml-enabled

# Generate predictive insights
dotnet run --project tools/analyzers/Analyzers.csproj -- --predict --operation "oracle-migration" --context large-codebase

# Apply learned optimizations
dotnet run --project tools/analyzers/Analyzers.csproj -- --optimize --target indexer --validation
```

## 🎮 MCP Directive System

ALARM uses a directive-based system for consistent operations. Use these tags in your Cursor prompts:

### Core Directives
- `[INDEX]` - Analyze and catalog codebase
- `[PLAN]` - Generate migration sequence
- `[ADAPT]` - Create adapter implementations
- `[REFACTOR:<pattern>]` - Apply code transformations
- `[TEST]` - Create/expand test coverage
- `[SMOKE]` - End-to-end validation
- `[VERIFY]` - Quality gate validation
- `[LOG]` - Generate run artifacts
- `[RISK]` - Update risk assessment
- `[REVIEW]` - Create review checklists
- `[RELEASE]` - Prepare release artifacts

### 🧠 Learning Directives (Advanced)
- `[LEARN]` - Analyze patterns and generate insights
- `[PREDICT]` - Generate predictive insights for success probability
- `[OPTIMIZE]` - Apply learned parameter optimizations
- `[EVOLVE]` - Update protocols based on learning
- `[DIAGNOSE]` - AI-powered root cause analysis
- `[BASELINE]` - Establish performance baselines
- `[FEEDBACK]` - Integrate user feedback into learning models

### Example Usage with Cursor

**Initial Assessment:**
```
SYSTEM ROLE: You are my ALARM MCP Refactor Orchestrator.

CONTEXT:
- Legacy App: C:\path\to\legacy\app
- Target: .NET 8, AutoCAD Map 3D 2025, Oracle 19c
- Manifests: /mcp/manifests/

TASK: [INDEX] → [PLAN]
Analyze legacy codebase and generate 12-PR migration sequence.
```

**Adapter Development:**
```
TASK: [ADAPT] AutoCAD 2025 Integration
Create comprehensive AutoCAD adapter layer with:
- IAutoCadService, IAcadDocument, ISelectionService interfaces
- Map 3D 2025 API implementations
- Test doubles for unit testing
- Migration examples for existing call sites
```

**Learning-Enhanced Development:**
```
TASK: [LEARN] → [OPTIMIZE] → [ADAPT] 
Smart adapter development with learning:
- Analyze historical adapter development patterns
- Apply learned optimizations for similar codebases
- Create AutoCAD adapter using proven patterns
- Record results for future learning
```

## 🧪 Testing Strategy

### Test Pyramid
1. **Unit Tests** (Fast, Isolated)
   - Mock all external dependencies
   - Test business logic and adapters separately
   - Target: >80% code coverage

2. **Integration Tests** (Real Dependencies)
   - Test adapters with actual AutoCAD/Oracle
   - Validate configuration and connectivity
   - Run in CI with appropriate secrets

3. **Smoke Tests** (End-to-End)
   - Critical path validation
   - Performance baseline verification
   - Production readiness checks

### Running Tests
```bash
# Unit tests
dotnet test app-core/AppCore.sln --collect:"XPlat Code Coverage"

# Integration tests (requires Oracle connection)
ORACLE_CONNECTION_STRING="your-connection-string" dotnet test --filter "Category=Integration"

# Smoke tests
dotnet run --project tools/smoke/Smoke.csproj
```

## 📊 Quality Gates

Every change must pass these automated gates:

### Build Gates
- ✅ Solution compiles without errors
- ✅ All projects target .NET 8
- ✅ No direct external API references in domain layer
- ✅ Roslyn analyzer compliance

### Test Gates
- ✅ Unit tests: >80% code coverage
- ✅ Integration tests: All critical paths
- ✅ Smoke tests: End-to-end validation
- ✅ Performance: <5% degradation

### Security Gates
- ✅ No hardcoded secrets
- ✅ SQL injection prevention
- ✅ Dependency vulnerability scan
- ✅ Secure configuration patterns

## 🔄 CI/CD Pipeline with Learning

The system includes a comprehensive GitHub Actions workflow with integrated learning:

```yaml
# Triggers on PR and push to main/develop
- Apply Learned Optimizations (Pre-execution)
- Build and Test (Windows)
- Static Analysis (Roslyn + Custom) 
- Oracle Integration Tests (with secrets)
- Performance Baseline
- Security Scan
- Learning Analysis (Pattern Recognition)
- Protocol Evolution (Auto-improvement)
- Deployment (Staging/Production)
- Final Learning Integration (Post-execution)
```

### 🧠 Learning Integration
- **Pre-execution**: Apply learned optimizations and predict success probability
- **During execution**: Record detailed metrics and context
- **Post-execution**: Analyze patterns, update knowledge base, evolve protocols
- **Continuous**: Generate learning dashboard and recommendations

### Setting up CI/CD
1. Copy `ci/build.yml` to `.github/workflows/`
2. Configure repository secrets:
   - `ORACLE_CONNECTION_STRING`
   - `ORACLE_TEST_SCHEMA`
3. Adjust paths and configuration as needed

## 📈 Migration Phases

### Phase 1: Foundation (PRs 1-4)
1. **Assessment & Planning**
   - Run indexer and generate migration plan
   - Set up CI/CD pipeline
   - Create adapter framework

2. **Configuration Migration**
   - app.config → appsettings.json
   - Implement IOptions pattern
   - Set up dependency injection

### Phase 2: Core Migration (PRs 5-8)
3. **Oracle Adapter**
   - ODP.NET Core integration
   - Connection factory and health checks
   - Migrate 2-3 key DAL classes

4. **AutoCAD Adapter**
   - Map 3D 2025 API integration
   - Document/transaction management
   - Migrate 2-3 key integration points

### Phase 3: Optimization (PRs 9-12)
5. **Framework Migration**
   - Automated Roslyn refactoring
   - WebClient → HttpClient
   - Async/await patterns

6. **Final Polish**
   - Performance optimization
   - UI modernization
   - Deployment automation

## 📚 Documentation

- **[Architecture Guide](docs/architecture.md)** - Detailed system design
- **[Cursor Prompts](docs/cursor-prompts.md)** - Copy-paste ready MCP commands
- **[Protocol Reference](mcp/protocols/protocols.md)** - Detailed process documentation
- **[Directive Dictionary](mcp/directives/directives.md)** - Complete command reference
- **[Learning Protocol](mcp/protocols/learning_protocol.md)** - Continuous improvement system
- **[Learning Directives](mcp/directives/learning_directives.md)** - Advanced learning commands

## 🛠️ Customization

### Adapting for Your Legacy App

1. **Update Manifests:**
   - Modify `mcp/manifests/files.manifest.json` for your file patterns
   - Update `mcp/manifests/apis.manifest.json` for your API dependencies

2. **Configure Indexer:**
   - Adjust file role patterns in the manifest
   - Add custom external API patterns

3. **Extend Adapters:**
   - Add interfaces for your specific external dependencies
   - Implement adapters following the established patterns

### Adding New Directives

1. Document the directive in `mcp/directives/directives.md`
2. Add corresponding protocol in `mcp/protocols/protocols.md`
3. Update the Cursor prompts in `docs/cursor-prompts.md`

## 🤝 Contributing

1. Follow the established adapter pattern
2. Maintain >80% test coverage
3. Update documentation with changes
4. Use MCP directives for consistency
5. Keep PRs focused and <300 LOC

## 📄 License

[Add your license information here]

## 🆘 Support

For issues and questions:
1. Check the documentation in `/docs/`
2. Review existing MCP run logs in `/mcp_runs/`
3. Use the diagnostic tools in `/tools/`
4. Create an issue with detailed context

## 🧠 Self-Learning and Continuous Improvement

ALARM's most powerful feature is its ability to **learn and improve from every run**:

### Learning Cycle
1. **Data Collection**: Every directive execution captures detailed metrics, context, and outcomes
2. **Pattern Recognition**: ML algorithms identify success/failure patterns and optimization opportunities  
3. **Knowledge Synthesis**: Learnings are synthesized into actionable insights and protocol improvements
4. **Adaptive Implementation**: System automatically applies optimizations and evolves protocols

### Learning Dashboard
Access the auto-generated learning dashboard at `/mcp_runs/learning_dashboard.html`:
- Success rate trends and performance metrics
- Pattern analysis and failure mode identification
- Predictive insights and optimization recommendations
- Protocol evolution tracking and validation results

### Continuous Improvement Features
- **Predictive Success Probability**: AI predicts migration success before execution
- **Auto-Parameter Optimization**: System learns optimal parameters for different contexts
- **Protocol Evolution**: Protocols automatically improve based on empirical evidence
- **Intelligent Error Recovery**: System learns from failures and applies recovery strategies
- **Performance Baseline Tracking**: Continuous monitoring with regression detection

---

**Remember**: ALARM is designed to be methodical, safe, and **continuously improving**. Start with `[INDEX] → [PLAN]` to understand your codebase, then let the learning system guide optimization. The system gets smarter and more effective with every use, building institutional knowledge that compounds over time. 
