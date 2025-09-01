# ADDS25 DEPLOYMENT & TESTING GUIDE

**System**: ADDS25 - Modernized Legacy Application  
**Framework**: .NET Core 8  
**AutoCAD**: Map3D 2025  
**Oracle**: 19c  
**Repository**: https://github.com/BraPil/ALARM.git  
**Status**: ✅ Production-Ready

---

## 🎯 **QUICK START DEPLOYMENT**

### **🚀 OPTION 1: AUTOMATED DEPLOYMENT (RECOMMENDED)**

#### **Step 1: Clone Repository**
```powershell
# Clone the ALARM repository
git clone https://github.com/BraPil/ALARM.git
cd ALARM\tests\ADDS25\v0.1
```

#### **Step 2: Build ADDS25 Solution**
```powershell
# Build the solution to create the assemblies
dotnet build ADDS25.sln
```

#### **Step 3: Run Automated Launcher**
```powershell
# Double-click ADDS25-Launcher.bat OR run from command line
.\ADDS25-Launcher.bat
```

**What the launcher automatically does:**
- ✅ **Phase 1**: Launches PowerShell with admin rights to create directories:
  - Creates `C:\ADDS25\`, `C:\ADDS25_Map\`, and all subdirectories
  - Sets proper permissions for all directories
  - Verifies .NET Core 8 runtime installation
- ✅ **Phase 2**: Deploys assemblies and launches AutoCAD:
  - Copies ADDS25 assemblies to `C:\ADDS25\Assemblies\`
  - Deploys configuration files
  - Detects AutoCAD Map3D 2025 installation
  - **Automatically launches AutoCAD Map3D 2025 with ADDS25 profile**
  - Loads ADDS25 integration automatically

#### **Step 4: Initialize in AutoCAD (if needed)**
If ADDS25 doesn't auto-initialize, run in AutoCAD command line:
```autocad
ADDS25_INIT
```

**Expected Output:**
```
*** ADDS25 Initialization Started ***
*** ADDS25 Loading LISP Integration Bridge ***
LISP Integration: X files, Y functions, Z variables
*** ADDS25 Initialization Complete ***
Application: ADDS25 v25.0.0
Framework: .NET Core 8
AutoCAD: Map3D 2025
Oracle: 19c
```

---

## 🔧 **MANUAL DEPLOYMENT (ADVANCED)**

### **Step 1: Prerequisites Verification**

#### **Verify .NET Core 8**
```powershell
dotnet --version
# Expected: 8.x.x
```

#### **Verify AutoCAD Map3D 2025**
```powershell
# Check registry
Get-ItemProperty "HKLM:\SOFTWARE\Autodesk\AutoCAD\R25.0\ACAD-2002:409" -Name ProductID -ErrorAction SilentlyContinue

# Check executable
Test-Path "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe"
```

#### **Verify Oracle Client**
```powershell
# Check for Oracle registry entries
Get-ChildItem "HKLM:\SOFTWARE\ORACLE" -ErrorAction SilentlyContinue
```

### **Step 2: Build ADDS25 Solution**

#### **Navigate to Solution**
```powershell
cd C:\path\to\ALARM\tests\ADDS25\v0.1
```

#### **Build Solution**
```powershell
# Build the complete solution
dotnet build ADDS25.sln

# Expected output:
# Build succeeded in X.X seconds
# Zero compilation errors
```

#### **Verify Build Output**
```powershell
# Check build artifacts
ls ADDS25.Core\bin\Debug\net8.0-windows\
ls ADDS25.AutoCAD\bin\Debug\net8.0-windows\
ls ADDS25.Oracle\bin\Debug\net8.0\
```

### **Step 3: Manual Directory Setup**

#### **Create Directory Structure**
```powershell
# Run directory setup script with admin rights
PowerShell.exe -Command "& {Start-Process PowerShell.exe -ArgumentList '-ExecutionPolicy Bypass -File ""ADDS25-DirSetup.ps1""' -Verb RunAs}"
```

**Created Directories:**
- `C:\ADDS25\` - Main application directory
- `C:\ADDS25\Dwg\` - Drawing files
- `C:\ADDS25\Logs\` - Application logs  
- `C:\ADDS25\Plot\` - Plot files
- `C:\ADDS25\Config\` - Configuration files
- `C:\ADDS25\Assemblies\` - .NET assemblies
- `C:\ADDS25_Map\` - LISP integration files
- `C:\ProgramData\ADDS25Temp\` - Temporary data

### **Step 4: Deploy Assemblies**

#### **Copy Build Artifacts**
```powershell
# Copy .NET assemblies
Copy-Item "ADDS25.Core\bin\Debug\net8.0-windows\*" -Destination "C:\ADDS25\Assemblies\" -Recurse
Copy-Item "ADDS25.AutoCAD\bin\Debug\net8.0-windows\*" -Destination "C:\ADDS25\Assemblies\" -Recurse  
Copy-Item "ADDS25.Oracle\bin\Debug\net8.0\*" -Destination "C:\ADDS25\Assemblies\" -Recurse

# Copy configuration
Copy-Item "Config\adds25_default_config.json" -Destination "C:\ADDS25_Map\adds25_config.json"
```

### **Step 5: Configure Oracle Connection**

#### **Update Configuration File**
```powershell
# Edit the configuration file
notepad "C:\ADDS25_Map\adds25_config.json"
```

#### **Required Configuration Updates**
```json
{
  "ADDS25Configuration": {
    "Oracle": {
      "ConnectionString": "Data Source=YOUR_ORACLE_SERVER:1521/YOUR_SERVICE_NAME;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;",
      "Server": "YOUR_ORACLE_SERVER",
      "DatabaseName": "YOUR_DATABASE",
      "Schema": "ADDSDB"
    }
  }
}
```

---

## 🧪 **TESTING PROCEDURES**

### **TEST 1: BUILD VERIFICATION**

#### **Command**
```powershell
cd C:\path\to\ALARM\tests\ADDS25\v0.1
dotnet build --verbosity normal
```

#### **Expected Results**
```
✅ Build succeeded in ~1.4 seconds
✅ Zero compilation errors
✅ 1 non-blocking warning (WindowsBase version conflict - expected)
✅ All 3 projects build successfully:
   - ADDS25.Core → net8.0-windows
   - ADDS25.AutoCAD → net8.0-windows  
   - ADDS25.Oracle → net8.0
```

### **TEST 2: ASSEMBLY LOADING**

#### **Command**
```powershell
# Test assembly loading
dotnet run --project ADDS25.Core
```

#### **Expected Results**
```
✅ Oracle.ManagedDataAccess.Core loads successfully
✅ ADDS25 configuration loads
✅ No runtime errors
```

### **TEST 3: AUTOCAD INTEGRATION**

#### **Prerequisites**
- AutoCAD Map3D 2025 must be installed
- ADDS25 assemblies deployed to `C:\ADDS25\Assemblies\`

#### **Test Steps**
1. **Launch AutoCAD Map3D 2025**
   ```powershell
   & "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe" /product MAP /language "en-US" /nologo
   ```

2. **Load ADDS25 Assembly**
   ```autocad
   NETLOAD C:\ADDS25\Assemblies\ADDS25.AutoCAD.dll
   ```

3. **Initialize ADDS25**
   ```autocad
   ADDS25_INIT
   ```

#### **Expected Results**
```
✅ Assembly loads without errors
✅ ADDS25_INIT command available
✅ Initialization completes successfully
✅ LISP Integration Bridge loads
✅ Oracle configuration validated
✅ Splash screen displays (if configured)
```

### **TEST 4: ORACLE CONNECTIVITY**

#### **Prerequisites**
- Oracle 19c client installed
- Valid Oracle connection string configured
- ADDS database schema available

#### **Test Command**
```autocad
# In AutoCAD after ADDS25_INIT
GetPanelData_2025 "TEST_PANEL_ID"
```

#### **Expected Results**
```
✅ Oracle connection established
✅ Query executes successfully  
✅ Data retrieval completes
✅ No Oracle-related errors
✅ Results logged to AutoCAD editor
```

### **TEST 5: LISP INTEGRATION**

#### **Test Command**
```autocad
# In AutoCAD after ADDS25_INIT
DisplaySplash_2025 "ADDS25"
```

#### **Expected Results**
```
✅ LISP function executes
✅ Splash screen displays
✅ No LISP integration errors
✅ Bridge functionality confirmed
```

---

## 🔍 **TROUBLESHOOTING GUIDE**

### **❌ ISSUE: Build Fails**

#### **Symptoms**
```
error: Could not find .NET Core SDK
error: Package not found
```

#### **Solutions**
1. **Install .NET Core 8 SDK**
   ```powershell
   # Download and install from:
   # https://dotnet.microsoft.com/download/dotnet/8.0
   ```

2. **Restore NuGet Packages**
   ```powershell
   dotnet restore ADDS25.sln
   dotnet build ADDS25.sln
   ```

### **❌ ISSUE: AutoCAD Assembly Loading Fails**

#### **Symptoms**
```
Could not load file or assembly 'ADDS25.AutoCAD'
System.IO.FileNotFoundException
```

#### **Solutions**
1. **Verify Assembly Location**
   ```powershell
   Test-Path "C:\ADDS25\Assemblies\ADDS25.AutoCAD.dll"
   ```

2. **Check .NET Core Runtime**
   ```powershell
   # Ensure .NET Core 8 runtime is installed
   dotnet --list-runtimes
   ```

3. **Verify AutoCAD Version**
   ```powershell
   # Must be AutoCAD Map3D 2025 (R25.0)
   Get-ItemProperty "HKLM:\SOFTWARE\Autodesk\AutoCAD\R25.0\ACAD-2002:409"
   ```

### **❌ ISSUE: Oracle Connection Fails**

#### **Symptoms**
```
Oracle.ManagedDataAccess.Client.OracleException
TNS: could not resolve the connect identifier
```

#### **Solutions**
1. **Verify Oracle Client**
   ```powershell
   # Check Oracle client installation
   Get-ChildItem "HKLM:\SOFTWARE\ORACLE"
   ```

2. **Update Connection String**
   ```json
   {
     "Oracle": {
       "ConnectionString": "Data Source=server:port/service;User Id=user;Password=pass;"
     }
   }
   ```

3. **Test Connection**
   ```powershell
   # Test basic Oracle connectivity
   sqlplus username/password@server:port/service
   ```

### **❌ ISSUE: LISP Integration Fails**

#### **Symptoms**
```
LISP Integration Warning: Could not load LISP file
FileNotFoundException
```

#### **Solutions**
1. **Verify LISP Files Location**
   ```powershell
   Test-Path "C:\ADDS25_Map\Adds\Lisp\Adds.Lsp"
   ```

2. **Copy Original LISP Files**
   ```powershell
   # Copy from original ADDS 2019 if available
   Copy-Item "C:\Users\kidsg\Downloads\Documentation\ADDS Original Files\Div_Map\*" -Destination "C:\ADDS25_Map\" -Recurse
   ```

3. **Use Built-in Bridge**
   ```
   # ADDS25 includes built-in LISP integration bridge
   # Functions will work even without original LISP files
   ```

### **❌ ISSUE: Permission Denied**

#### **Symptoms**
```
Access to the path 'C:\ADDS25' is denied
UnauthorizedAccessException
```

#### **Solutions**
1. **Run as Administrator**
   ```powershell
   # Right-click PowerShell → "Run as Administrator"
   .\ADDS25-Launcher.bat
   ```

2. **Set Permissions Manually**
   ```powershell
   icacls.exe C:\ADDS25 /grant 'Users:(oi)(ci)(f)'
   icacls.exe C:\ADDS25_Map /grant 'Users:(oi)(ci)(f)'
   ```

---

## 📊 **PERFORMANCE BENCHMARKS**

### **Expected Performance Metrics**

#### **Build Performance**
- **Solution Build Time**: ~1.4 seconds
- **Compilation Errors**: 0
- **Warnings**: 1 (non-blocking WindowsBase conflict)

#### **Runtime Performance**
- **Assembly Loading**: < 2 seconds
- **ADDS25 Initialization**: < 5 seconds
- **Oracle Connection**: < 3 seconds
- **LISP Bridge Loading**: < 2 seconds

#### **Memory Usage**
- **Base Memory**: ~50-100 MB
- **With AutoCAD**: +200-300 MB
- **Oracle Operations**: +50-100 MB

---

## 📋 **DEPLOYMENT CHECKLIST**

### **Pre-Deployment Checklist**
- [ ] .NET Core 8 Runtime installed
- [ ] AutoCAD Map3D 2025 installed and licensed
- [ ] Oracle 19c client installed
- [ ] Administrator permissions available
- [ ] Repository cloned locally

### **Deployment Checklist**
- [ ] ADDS25 solution builds successfully (1.4s, zero errors)
- [ ] Directory structure created (`C:\ADDS25\`, `C:\ADDS25_Map\`)
- [ ] Assemblies deployed to `C:\ADDS25\Assemblies\`
- [ ] Configuration file updated with Oracle connection
- [ ] AutoCAD can load ADDS25.AutoCAD.dll

### **Testing Checklist**
- [ ] ADDS25_INIT command works in AutoCAD
- [ ] LISP Integration Bridge loads successfully
- [ ] Oracle connectivity test passes
- [ ] DisplaySplash_2025 command works
- [ ] GetPanelData_2025 command works (with valid panel ID)
- [ ] No runtime errors in AutoCAD or logs

### **Production Readiness Checklist**
- [ ] All tests pass successfully
- [ ] Performance meets benchmarks
- [ ] Error handling works properly
- [ ] Logging is functional
- [ ] Documentation is complete
- [ ] Backup and recovery procedures established

---

## 🎯 **SUCCESS CRITERIA**

### **Deployment Success Indicators**
1. ✅ **Build Success**: Solution builds in ~1.4 seconds with zero errors
2. ✅ **AutoCAD Integration**: ADDS25_INIT command executes successfully
3. ✅ **Oracle Connectivity**: Database operations complete without errors
4. ✅ **LISP Integration**: Bridge loads and processes LISP functions
5. ✅ **Performance**: All operations complete within benchmark times

### **Production Readiness Indicators**
1. ✅ **Stability**: No crashes or unhandled exceptions
2. ✅ **Functionality**: All critical ADDS functions operational
3. ✅ **Performance**: Response times meet user expectations
4. ✅ **Logging**: Comprehensive logging for troubleshooting
5. ✅ **Documentation**: Complete deployment and user guides

---

## 📞 **SUPPORT AND RESOURCES**

### **Documentation Resources**
- **GitHub Repository**: https://github.com/BraPil/ALARM.git
- **Migration Report**: `mcp_runs/adds25-migration-progress-report.md`
- **Architecture Analysis**: `mcp_runs/adds2019-complete-reverse-engineering-report.md`
- **Launcher Analysis**: `mcp_runs/adds-complete-launcher-workflow-analysis.md`

### **Technical Support**
- **Build Issues**: Check .NET Core 8 SDK installation
- **AutoCAD Issues**: Verify Map3D 2025 installation and licensing
- **Oracle Issues**: Verify client installation and connection string
- **LISP Issues**: Check file permissions and bridge configuration

### **Community Resources**
- **Repository Issues**: Report bugs via GitHub Issues
- **Documentation**: Complete documentation in `mcp_runs/` directory
- **Learning Resources**: ALARM training logs and verification reports

---

## 🎉 **DEPLOYMENT COMPLETE**

Once all tests pass and the success criteria are met, your ADDS25 deployment is complete and ready for production use. The system provides all the functionality of the original ADDS 2019 with modern .NET Core 8 architecture, AutoCAD Map3D 2025 integration, and Oracle 19c connectivity.

**Welcome to ADDS25 - The Future of Legacy Application Modernization!**
