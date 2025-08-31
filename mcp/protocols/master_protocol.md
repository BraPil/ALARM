# ALARM MASTER PROTOCOL
## Central Decision-Making Framework for Legacy App Refactoring

**Date Created:** August 30, 2025  
**Purpose:** Central protocol management and decision-making for ALARM MCP operations  
**Status:** ACTIVE - Controls all refactoring activities  
**Launch Requirement:** MUST be reviewed at the beginning of every prompt response  

---

## **🎯 PRIME DIRECTIVE**

**The purpose of ALARM is to:** Create a comprehensive "poor man's MCP" (Model Context Protocol) system designed to systematically modernize legacy applications through complete reverse engineering with 100% functionality preservation.

**COMPREHENSIVE APPLICATION CRAWLING METHODOLOGY:**
1. **Launcher Discovery & Analysis** - Identify and analyze main application launchers (.bat, .ps1, .exe, entry points)
2. **Execution Path Mapping** - Follow all execution paths from launcher through the entire application ecosystem  
3. **Complete Ecosystem Crawling** - Like a search engine crawler, systematically discover and analyze every component
4. **Universal Indexing** - Index every file, line, function, variable, class, dependency, path, and relationship
5. **Deep Understanding** - Reverse engineer, map, document, and comprehend the complete application architecture
6. **Comprehensive Documentation** - Generate complete maps, dependency graphs, and architectural documentation

**UNIVERSAL SUCCESS CRITERIA:** Zero functionality loss, complete feature parity, modern technology stack compatibility, comprehensive understanding of entire codebase from launcher to every leaf dependency.

**PROJECT-SPECIFIC CONFIGURATION:** When applied to specific projects, target technologies are defined in project manifests (e.g., for ADDS: .NET Core 8, AutoCAD Map 3D 2025, Oracle 19c).

## **🔍 UNIVERSAL ANTI-SAMPLING DIRECTIVE**

**MANDATORY FOR ALL PROTOCOLS:** Always consider the entirety of the object. Read and understand the whole file, process, function, test, etc... and everything it touches. Do not just read the first 100 lines of the file. Do not sample parts of the file. Read and understand the entire thing every line. Every single one. Every word. Every Function, Every class. Every Variable. Every dependency.

**ENFORCEMENT RULES:**
- **Files < 1,000 lines:** ALWAYS read completely, no exceptions
- **Files 1,000-10,000 lines:** ALWAYS read completely, no exceptions  
- **Files > 10,000 lines:** Read completely when possible, sample only with explicit justification
- **ALL configuration files:** Read completely regardless of size
- **ALL documentation:** Read completely regardless of size
- **ALL log files:** Read completely for analysis

**VIOLATION CONSEQUENCES:** Any sampling without justification triggers immediate protocol restart with full compliance verification.

---

## **🚀 LAUNCH PROTOCOL - REQUIRED AT START OF EVERY RESPONSE**

**⚠️ CRITICAL PROTOCOL VIOLATIONS ADDRESSED**: 
- **MANDATORY LAUNCH**: Every response MUST begin with Master Protocol engagement
- **ANTI-SAMPLING ENFORCEMENT**: ANY file reading MUST be complete - NO LIMITS, NO OFFSETS, NO SAMPLING
- **VERIFICATION INTEGRITY**: Claims of "systematic verification" require complete file reading
- **VIOLATION DETECTION**: Any work without protocol launch triggers immediate correction
- **DOCUMENTATION REQUIREMENT**: All violations must be logged in `mcp_runs/protocol_violations/`
- **ZERO TOLERANCE**: No exceptions to protocol launch or anti-sampling requirements

**MANDATORY CHECKLIST - MUST COMPLETE BEFORE ANY WORK:**

### **1. PRIME DIRECTIVE CONFIRMATION**
- [ ] Prime directive reviewed and understood (universal legacy app reverse engineering with comprehensive crawling)
- [ ] Launcher identification and analysis approach confirmed (.bat, .ps1, .exe entry points)
- [ ] Complete ecosystem crawling methodology understood (follow all execution paths)
- [ ] Universal indexing commitment verified (every file, line, function, variable, class, dependency)
- [ ] Project-specific target technologies confirmed (if applicable)
- [ ] 100% functionality preservation commitment verified

### **2. ANTI-SAMPLING COMMITMENT**  
- [ ] Universal anti-sampling directive acknowledged
- [ ] Complete reading requirements understood
- [ ] No sampling < 10,000 lines without justification

### **3. PROTOCOL SELECTION**
- [ ] Current task analyzed against protocol selection matrix
- [ ] Primary and secondary protocols identified
- [ ] Quality gates identified for selected protocols

### **4. LOGGING PREPARATION**
- [ ] Appropriate log identified or created
- [ ] Success criteria defined for current work
- [ ] Evidence collection plan established

### **5. ORGANIZATION**
- [ ] Verify the correct directory(ies) are being referred to and present them to the user to validate
- [ ] Ensure proper organization is being maintained

**FAILURE TO COMPLETE LAUNCH PROTOCOL:** All work must stop until launch protocol is completed with full compliance.

---

## **📋 PROTOCOL CATALOG**

### **ANALYSIS PROTOCOLS**

#### **1. RESEARCH PROTOCOL**
- **Purpose:** Comprehensive investigation and evidence gathering
- **When:** Error investigation, dependency analysis, root cause identification
- **Key Rule:** Read every file completely - NO sampling for files < 10,000 lines
- **Output:** Complete evidence with sources and context

#### **2. PATTERN PROTOCOL** 
- **Purpose:** Success/failure pattern recognition and learning
- **When:** Analyzing trends, identifying optimization opportunities
- **Key Rule:** Statistical significance required (min 5 data points)
- **Output:** Validated patterns with confidence scores

### **IMPLEMENTATION PROTOCOLS**

#### **3. ADAPT PROTOCOL**
- **Purpose:** Create adapter interfaces for external dependencies
- **When:** Isolating AutoCAD, Oracle, or Framework APIs
- **Key Rule:** Domain layer never directly calls external APIs
- **Output:** Interface + implementation + test doubles

#### **4. REFACTOR PROTOCOL**
- **Purpose:** Safe code transformation with full traceability
- **When:** Modernizing code patterns, fixing anti-patterns
- **Key Rule:** Maximum 300 LOC per change, atomic commits
- **Output:** Before/after diffs with comprehensive tests

#### **5. CODE PROTOCOL**
- **Purpose:** Creation of new components following established patterns
- **When:** Building new functionality, replacement systems
- **Key Rule:** Test-first development, comprehensive error handling
- **Output:** Tested, documented, production-ready code

### **VALIDATION PROTOCOLS**

#### **6. VERIFY PROTOCOL**
- **Purpose:** Systematic validation of all changes
- **When:** After any modification, before merging
- **Key Rule:** Line-by-line verification for files < 1,000 lines
- **Output:** Verification report with evidence

#### **7. TEST PROTOCOL**
- **Purpose:** Comprehensive testing at all levels
- **When:** After implementation, before deployment
- **Key Rule:** >80% coverage, unit + integration + smoke tests
- **Output:** Test results with coverage metrics

### **PROCESS PROTOCOLS**

#### **8. LOG PROTOCOL**
- **Purpose:** Systematic recording of all activities and decisions
- **When:** Continuously throughout all work
- **Key Rule:** Every decision documented with rationale and timestamp
- **Output:** Chronological log with complete audit trail

---

## **🔍 PROTOCOL SELECTION MATRIX**

| Scenario | Primary Protocol | Secondary Protocol | Quality Gates |
|----------|-----------------|-------------------|---------------|
| **Investigate Issues** | RESEARCH | PATTERN | 1, 2, 4 |
| **Create Adapters** | ADAPT | CODE, TEST | 1, 2, 3, 4 |
| **Modernize Code** | REFACTOR | VERIFY, TEST | 1, 2, 3, 4 |
| **Build New Features** | CODE | TEST, VERIFY | 1, 2, 3, 4 |
| **Analyze Trends** | PATTERN | RESEARCH | 1, 2, 4 |
| **Validate Changes** | VERIFY | TEST | 2, 3, 4 |

---

## **📋 PROJECT-SPECIFIC CONFIGURATIONS**

### **ADDS PROJECT CONFIGURATION**
**Target Technologies:**
- **.NET Core 8** - Modern cross-platform framework
- **AutoCAD Map 3D 2025** - Latest CAD platform with ObjectARX and .NET API
- **Oracle 19c** - Enterprise database with ODP.NET Managed driver

**Critical Migration Areas:**
- **Legacy Framework APIs** → .NET Core 8 equivalents
- **AutoCAD ARX/COM** → Map 3D 2025 .NET API via adapters
- **Oracle Client** → ODP.NET Core with connection factory pattern

**ADDS-Specific Quality Gates:**
- AutoCAD adapter isolation verified (no direct Autodesk.* calls in domain)
- Oracle connection management through factory pattern
- Configuration migrated from app.config to appsettings.json

---

## **🚨 QUALITY GATES**

### **GATE 1: PROTOCOL SELECTION**
**MUST PASS BEFORE STARTING:**
- [ ] Correct protocol(s) identified from matrix
- [ ] Anti-sampling requirements understood
- [ ] Success criteria defined
- [ ] Logging plan established

### **GATE 2: EXECUTION**
**MUST PASS DURING WORK:**
- [ ] All protocol steps followed systematically
- [ ] Evidence collected with sources
- [ ] No sampling for files < 10,000 lines
- [ ] Progress logged in real-time
- [ ] Changes to files, code and functions must be reviewed against the original to ensure 100% functionality has been retained
- [ ] Path changes must be presented for validation

### **GATE 3: VERIFICATION**
**MUST PASS BEFORE COMPLETION:**
- [ ] All protocol requirements met
- [ ] Independent verification completed
- [ ] **BUILD SUCCESS MANDATORY**: All code compiles (exit code 0)
- [ ] **NO COMPILATION ERRORS**: Zero build failures allowed
- [ ] Test coverage >80% for code changes
- [ ] Evidence documented with confidence scores
- [ ] **COMPLETE FILE READING VERIFIED**: All files read entirely (documented)
- [ ] **VERIFICATION REPORT GENERATED**: Systematic validation evidence

### **GATE 4: DOCUMENTATION**
**MUST PASS BEFORE PROCEEDING:**
- [ ] Complete work logged with timestamps
- [ ] Decisions documented with rationale
- [ ] Master Log status updated
- [ ] Next steps identified and planned

---

## **🔒 ANTI-SAMPLING ENFORCEMENT**

### **MANDATORY COMPLETE READING:**
- **Files < 1,000 lines:** ALWAYS read completely
- **Files 1,000-10,000 lines:** ALWAYS read completely
- **Files > 10,000 lines:** Sample only if necessary, document why
- **ALL documentation files:** Read completely regardless of size
- **ALL configuration files:** Read completely regardless of size
- **ALL log files:** Read completely for analysis

### **FORBIDDEN PATTERNS:**
- ❌ Partial reads without justification
- ❌ Skipping protocol steps
- ❌ Claims without evidence
- ❌ Changes without tests
- ❌ Work without logging

---

## **📊 SUCCESS METRICS**

- ✅ **100% Protocol Compliance** - All protocols followed completely
- ✅ **100% Functionality Preservation** - No feature loss
- ✅ **100% Complete Reading** - No unjustified sampling
- ✅ **>80% Test Coverage** - All changes thoroughly tested
- ✅ **100% Documentation** - All decisions and evidence recorded

---

## **🚫 FAILURE TRIGGERS - STOP IMMEDIATELY IF:**
- Sampling files < 10,000 lines without justification
- Skipping quality gates
- Making claims without evidence
- Proceeding without proper protocol selection
- Working without logging activities
- **BUILD FAILURES**: Any compilation errors (exit code ≠ 0)
- **MARKING COMPLETED WITHOUT VERIFICATION**: Claims without evidence
- **SKIPPING VERIFICATION PROTOCOL**: No systematic validation performed

**FAILURE RESPONSE:** STOP → Document failure → Restart with compliance → Verify everything

### **MANDATORY COMPLETION CHECKLIST**
Before any "COMPLETED" status:
- [ ] `dotnet build` returns exit code 0 (if code project)
- [ ] All files read completely (documented with line counts)
- [ ] Verification report generated with evidence
- [ ] All tests pass with >80% coverage
- [ ] No compilation errors or critical warnings
- [ ] Protocol compliance verified and documented

---

**Last Updated:** August 30, 2025 - 2:15 PM  
**Status:** ACTIVE - Universal system with project-specific configurations
