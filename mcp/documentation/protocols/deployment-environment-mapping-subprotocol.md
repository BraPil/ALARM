# Deployment Environment Mapping Sub-Protocol

**Version**: 1.0  
**Date**: September 1, 2025  
**Purpose**: Define and enforce username mapping requirements for deployment files  
**Authority**: Master Protocol Requirement - Environment Consistency  

---

## 🎯 **PROTOCOL OVERVIEW**

### **Primary Objective**
Ensure all deployment files contain correct username references for their target deployment environment, preventing deployment failures due to incorrect path resolution.

### **Critical Requirement**
**ALL FILES** located in deployment directories must reference the **TARGET ENVIRONMENT USERNAME**, not the development environment username.

---

## 📋 **ENVIRONMENT MAPPING SPECIFICATIONS**

### **🔴 MANDATORY USERNAME MAPPINGS**

#### **Development Environment**
- **Username**: `kidsg`
- **Base Path**: `C:\Users\kidsg\Downloads\ALARM\`
- **Purpose**: Development, testing, and protocol management
- **Usage**: Files that run on the development computer

#### **Test/Deployment Environment**
- **Username**: `wa-bdpilegg`  
- **Base Path**: `C:\Users\wa-bdpilegg\Downloads\ALARM\`
- **Purpose**: Target deployment and testing environment
- **Usage**: Files that will be deployed and run on the test computer

### **🔴 DEPLOYMENT DIRECTORY RULE**

**CRITICAL REQUIREMENT**: All files in the following directory **MUST** use `wa-bdpilegg` username:
```
C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1\
```

**Rationale**: These files are deployment artifacts that will be copied to the test computer where the username is `wa-bdpilegg`.

---

## 🛠️ **IMPLEMENTATION REQUIREMENTS**

### **File Categories and Username Requirements**

#### **Category 1: Deployment Files (wa-bdpilegg)**
**Location**: `C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1\`
**Required Username**: `wa-bdpilegg`
**Files Include**:
- `ADDS25-Launcher.bat`
- `ADDS25-DirSetup.ps1`  
- `ADDS25-AppSetup.ps1`
- `*.csproj` files (HintPath references)
- Configuration files (`*.json` in Config directory)
- Any scripts that will run on test computer

**Excluded from Compliance**:
- Build artifacts (`bin\`, `obj\` directories)
- NuGet generated files (`*.nuget.dgspec.json`, `project.assets.json`)
- Compiler generated files (`*.sourcelink.json`, `*.deps.json`)
- These files are regenerated during build and inherit environment automatically

#### **Category 2: Development/Protocol Files (kidsg)**
**Location**: `C:\Users\kidsg\Downloads\ALARM\` (excluding tests/ADDS25/v0.1/)
**Required Username**: `kidsg`
**Files Include**:
- Protocol documentation
- Development tools
- ALARM system files
- Monitoring scripts that run on development computer

#### **Category 3: Logging and Monitoring Files (Environment-Aware)**
**Location**: Various
**Required Behavior**: Auto-detect environment and use appropriate username
**Implementation**: Use conditional logic based on current environment

---

## 🔍 **VALIDATION PROCEDURES**

### **Pre-Deployment Validation**
Before any deployment operation, **MUST** verify:

1. **Username Reference Scan**:
   ```powershell
   # Scan deployment directory for incorrect username references
   Get-ChildItem "C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1" -Recurse -File | 
   ForEach-Object { 
       $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
       if ($content -match "kidsg" -and $_.Extension -in @('.ps1','.bat','.csproj','.json','.md')) {
           Write-Warning "PROTOCOL VIOLATION: File $($_.Name) contains 'kidsg' reference"
       }
   }
   ```

2. **Path Resolution Verification**:
   - Verify all hardcoded paths use correct username
   - Verify AutoCAD assembly references point to correct paths
   - Verify configuration files reference correct directories

3. **Deployment Readiness Check**:
   - All deployment files reference `wa-bdpilegg`
   - No development-specific paths in deployment files
   - All logging paths configured for target environment

### **Post-Update Validation**
After any file updates in deployment directory:

1. **Automated Scan**: Run username reference validation
2. **Build Verification**: Ensure all project references resolve correctly
3. **Path Testing**: Verify all paths will resolve correctly on target environment

---

## 🚨 **PROTOCOL VIOLATIONS AND ENFORCEMENT**

### **Violation Categories**

#### **🔴 Critical Violations (Deployment Blocking)**
- Deployment file contains `kidsg` username reference
- AutoCAD project references point to development paths
- Configuration files reference development directories

#### **🟡 Warning Violations (Review Required)**
- Ambiguous path references that may not resolve
- Missing environment detection logic
- Hardcoded paths without username consideration

### **Enforcement Actions**
1. **Immediate Correction**: Fix username references immediately upon detection
2. **Protocol Documentation**: Update this protocol with any new requirements
3. **Validation Integration**: Integrate validation into build/deployment processes
4. **Training Update**: Update ALARM training data with violation patterns

---

## 📊 **COMPLIANCE VERIFICATION**

### **Verification Checklist**
- [ ] All files in `tests/ADDS25/v0.1/` use `wa-bdpilegg` username
- [ ] All AutoCAD assembly references point to `wa-bdpilegg` paths
- [ ] All PowerShell scripts in deployment directory target correct environment
- [ ] All configuration files reference correct target directories
- [ ] No development-specific username references in deployment files

### **Automated Compliance Tools**
```powershell
# Deployment Environment Compliance Checker
function Test-DeploymentCompliance {
    param([string]$DeploymentPath = "C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1")
    
    $violations = @()
    $files = Get-ChildItem $DeploymentPath -Recurse -File -Include "*.ps1","*.bat","*.csproj","*.json"
    
    foreach ($file in $files) {
        $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
        if ($content -match "\\kidsg\\") {
            $violations += "VIOLATION: $($file.Name) contains development username reference"
        }
    }
    
    if ($violations.Count -eq 0) {
        Write-Host "✅ DEPLOYMENT COMPLIANCE: PASSED" -ForegroundColor Green
    } else {
        Write-Host "❌ DEPLOYMENT COMPLIANCE: FAILED" -ForegroundColor Red
        $violations | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
    }
    
    return $violations.Count -eq 0
}
```

---

## 🔄 **PROTOCOL INTEGRATION**

### **Master Protocol Integration**
This sub-protocol is **AUTOMATICALLY ACTIVATED** during:
- All deployment file creation or modification
- All ADDS25 project updates
- All AutoCAD integration development
- All configuration file updates
- All build and deployment processes

### **Error Analysis Integration**
- All username mapping violations are logged in error analysis system
- Violation patterns are included in ALARM training data
- Resolution strategies are documented for continuous improvement

### **Quality Gate Integration**
- Username mapping compliance is **REQUIRED** for Quality Gate 3 passage
- All deployment files **MUST** pass compliance verification
- Build processes **MUST** validate username references before deployment

---

## 🎯 **PROTOCOL AUTHORITY AND PERMANENCE**

### **Permanent Requirements**
These username mappings are **HARDCODED IN PERPETUITY**:
- **Development Environment**: `kidsg`
- **Test/Deployment Environment**: `wa-bdpilegg`

### **Non-Negotiable Rules**
1. **Deployment files MUST use wa-bdpilegg username**
2. **Development files MUST use kidsg username**  
3. **Environment-aware files MUST auto-detect correctly**
4. **No exceptions without Master Protocol authorization**

### **Enforcement Authority**
- This protocol has **FULL AUTHORITY** over username mapping requirements
- All team members **MUST** comply with mapping specifications
- All automated systems **MUST** validate compliance
- All deployment processes **MUST** enforce these requirements

---

## ✅ **PROTOCOL COMPLIANCE CONFIRMATION**

### **Implementation Status**
- ✅ **Username mappings defined and documented**
- ✅ **Deployment files updated to use wa-bdpilegg**
- ✅ **Validation procedures established**
- ✅ **Compliance tools created**
- ✅ **Integration with Master Protocol complete**

### **Verification Evidence**
- All files in `tests/ADDS25/v0.1/` verified to use `wa-bdpilegg`
- AutoCAD project references updated to target environment paths
- PowerShell scripts configured for correct target environment
- Compliance validation tools operational and tested

---

**This Deployment Environment Mapping Sub-Protocol is now ACTIVE and ENFORCED for all deployment operations with permanent username mapping requirements.**
