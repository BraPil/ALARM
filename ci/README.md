# ADDS25 Automated CI/CD System

## 🎯 **Overview**

This automated CI/CD system creates a continuous test/fix loop between the development computer and test computer using GitHub as the coordination hub.

## 🏗️ **Architecture**

```
Test Computer (wa-bdpilegg) → Push Test Results → GitHub Repository
                                                        ↓
                                                   Webhook Event
                                                        ↓
Dev Computer (kidsg) ← Master Protocol Analysis ← Auto-Pull Results
        ↓
Generate & Apply Fixes
        ↓
Push Fixes → GitHub Repository → Webhook Event → Test Computer Auto-Pull
                                                        ↓
                                                 Run Tests & Push Results
                                                        ↓
                                                    Loop Continues
```

## 🚀 **Setup Instructions**

### **Dev Computer (kidsg)**

1. **Start the CI system:**
   ```powershell
   cd C:\Users\kidsg\Downloads\ALARM\ci
   .\START-DEV-CI.ps1
   ```

2. **What it does:**
   - Monitors GitHub for test result commits
   - Automatically engages Master Protocol for analysis
   - Generates fixes based on test failures
   - Commits and pushes fixes back to GitHub

### **Test Computer (wa-bdpilegg)**

1. **Start the testing system:**
   ```powershell
   cd C:\Users\wa-bdpilegg\Downloads\ALARM\ci
   .\START-TEST-CI.ps1
   ```

2. **What it does:**
   - Monitors GitHub for fix commits
   - Automatically pulls latest changes
   - Runs ADDS25-Launcher.bat with comprehensive logging
   - Collects and pushes test results back to GitHub

## 📋 **Features**

### **Dev Computer Automation**
- ✅ **Master Protocol Integration**: Every analysis follows complete protocol
- ✅ **Intelligent Fix Generation**: Automatically identifies and fixes common issues
- ✅ **AutoCAD DLL Path Correction**: Updates project references to correct paths
- ✅ **Build System Fixes**: Handles .NET SDK and compilation issues
- ✅ **Automated Commits**: Generates descriptive commit messages for fixes

### **Test Computer Automation**
- ✅ **Comprehensive Testing**: Runs complete ADDS25 launcher with logging
- ✅ **Result Collection**: Automatically gathers all test outputs
- ✅ **AutoCAD Process Monitoring**: Detects if AutoCAD launches successfully
- ✅ **Build Status Detection**: Identifies build success/failure states
- ✅ **Automated Result Upload**: Pushes results to GitHub for analysis

## 🔧 **System Requirements**

### **Dev Computer**
- Git repository at `C:\Users\kidsg\Downloads\ALARM`
- PowerShell 5.0 or later
- Git command line tools
- Internet connection for GitHub access

### **Test Computer**
- Git repository at `C:\Users\wa-bdpilegg\Downloads\ALARM`
- AutoCAD Map3D 2025 installed
- .NET 8.0 SDK installed
- PowerShell 5.0 or later
- Git command line tools
- Internet connection for GitHub access

## 📊 **Monitoring & Logs**

### **Dev Computer Logs**
- **CI Session Log**: `ci/logs/dev-ci-session-[timestamp].md`
- **Analysis Results**: Detailed Master Protocol analysis of each test result
- **Fix Generation Log**: Record of all automated fixes applied

### **Test Computer Logs**
- **CI Session Log**: `ci/logs/test-ci-session-[timestamp].md`
- **Test Results**: `test-results/launcher-execution-[timestamp].md`
- **AutoCAD Monitoring**: Process detection and startup analysis

## 🔄 **CI/CD Workflow**

1. **Initial Test**: Test computer runs ADDS25 and pushes results
2. **Analysis**: Dev computer analyzes results with Master Protocol
3. **Fix Generation**: Automated fixes generated based on analysis
4. **Fix Deployment**: Fixes committed and pushed to GitHub
5. **Re-testing**: Test computer pulls fixes and runs tests again
6. **Iteration**: Process repeats until all tests pass

## ⚙️ **Configuration**

### **Monitoring Intervals**
- Both systems check GitHub every 30 seconds
- Error recovery waits 60 seconds before retry

### **Commit Detection**
- Dev computer looks for test result commits
- Test computer looks for fix commits (ignores test results)
- Smart filtering prevents infinite loops

## 🛠️ **Troubleshooting**

### **If CI System Stops**
1. Check the latest log files in `ci/logs/`
2. Verify GitHub connectivity: `git pull origin main`
3. Restart with the appropriate START script

### **If Tests Keep Failing**
1. Check test computer AutoCAD installation
2. Verify .NET 8.0 SDK installation
3. Review latest test results for specific error patterns

### **If Fixes Aren't Applied**
1. Check dev computer Git permissions
2. Verify repository write access
3. Review dev CI logs for fix generation errors

## 📚 **Manual Override**

Both systems can be stopped with `Ctrl+C` and run manually:
- **Dev**: `.\DEV-COMPUTER-AUTOMATED-CI.ps1`
- **Test**: `.\TEST-COMPUTER-AUTOMATED-CI.ps1`

## 🎯 **Success Criteria**

The CI/CD loop is successful when:
- ✅ ADDS25 builds without errors
- ✅ AutoCAD Map3D 2025 launches successfully
- ✅ All ADDS25 functionality works as expected
- ✅ Test results show green status across all components
