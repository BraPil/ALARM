# ADDS25 Deployment Monitor Script
# Purpose: Comprehensive monitoring and logging of ADDS25 deployment process
# Date: September 1, 2025

param(
    [string]$Environment = "Development", # Development (kidsg) or Test (wa-bdpilegg)
    [string]$LogDirectory = "",
    [switch]$Verbose
)

# Determine environment and set appropriate paths
if ($LogDirectory -eq "") {
    if ($Environment -eq "Test") {
        $LogDirectory = "C:\Users\wa-bdpilegg\Downloads\ALARM\test-results"
    } else {
        $LogDirectory = "C:\Users\kidsg\Downloads\ALARM\test-results"
    }
}

# Initialize logging
$Timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$SessionLogFile = Join-Path $LogDirectory "ADDS25-Deployment-Session-$Timestamp.md"
$PowerShellLogFile = Join-Path $LogDirectory "PowerShell-Results-Log.md"
$ErrorLogFile = Join-Path $LogDirectory "ADDS25-Error-Analysis-Report.md"

# Ensure log directory exists
if (!(Test-Path $LogDirectory)) { New-Item $LogDirectory -Type Directory -Force | Out-Null }

# Initialize session log
@"
# ADDS25 Deployment Session Log

**Session ID**: $Timestamp
**Date**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**User**: $env:USERNAME
**Computer**: $env:COMPUTERNAME
**PowerShell Version**: $($PSVersionTable.PSVersion)

---

## 🎯 **PRE-DEPLOYMENT SYSTEM CHECK**

"@ | Out-File $SessionLogFile -Encoding UTF8

function Write-SessionLog {
    param(
        [string]$Message,
        [string]$Level = "INFO"
    )
    $LogEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $LogEntry
    Add-Content $SessionLogFile $LogEntry -Encoding UTF8
}

function Test-Prerequisites {
    Write-SessionLog "=== PREREQUISITE VALIDATION ===" "HEADER"
    
    # Test .NET Core 8
    try {
        $dotnetVersion = & dotnet --version 2>$null
        if ($dotnetVersion -like "8.*") {
            Write-SessionLog "✅ .NET Core 8 Runtime: $dotnetVersion" "SUCCESS"
        } else {
            Write-SessionLog "❌ .NET Core 8 Runtime: Not found (Current: $dotnetVersion)" "ERROR"
        }
    } catch {
        Write-SessionLog "❌ .NET Core 8 Runtime: Not installed" "ERROR"
    }
    
    # Test AutoCAD Map3D 2025
    $autocadRegPath = "HKLM:\SOFTWARE\Autodesk\AutoCAD\R25.0\ACAD-2002:409"
    if (Test-Path $autocadRegPath) {
        $productID = (Get-ItemProperty $autocadRegPath -ErrorAction SilentlyContinue).ProductID
        Write-SessionLog "✅ AutoCAD Map3D 2025: Detected (ProductID: $productID)" "SUCCESS"
        
        $autocadExe = "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe"
        if (Test-Path $autocadExe) {
            Write-SessionLog "✅ AutoCAD Executable: Found" "SUCCESS"
        } else {
            Write-SessionLog "⚠️ AutoCAD Executable: Not found at expected location" "WARNING"
        }
    } else {
        Write-SessionLog "❌ AutoCAD Map3D 2025: Not detected in registry" "ERROR"
    }
    
    # Test Oracle Client
    try {
        $oracleReg = Get-ChildItem "HKLM:\SOFTWARE\ORACLE" -ErrorAction SilentlyContinue
        if ($oracleReg) {
            Write-SessionLog "✅ Oracle Client: Detected" "SUCCESS"
        } else {
            Write-SessionLog "⚠️ Oracle Client: Not detected (optional)" "WARNING"
        }
    } catch {
        Write-SessionLog "⚠️ Oracle Client: Not detected (optional)" "WARNING"
    }
    
    # Test Administrator Rights
    $currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    if ($currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        Write-SessionLog "✅ Administrator Rights: Available" "SUCCESS"
    } else {
        Write-SessionLog "❌ Administrator Rights: Not available" "ERROR"
    }
    
    # Test Build Artifacts
    if ($Environment -eq "Test") {
        $solutionPath = "C:\Users\wa-bdpilegg\Downloads\ALARM\tests\ADDS25\v0.1"
    } else {
        $solutionPath = "C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1"
    }
    if (Test-Path "$solutionPath\ADDS25.sln") {
        Write-SessionLog "✅ ADDS25 Solution: Found" "SUCCESS"
        
        # Check build artifacts
        $coreAssembly = "$solutionPath\ADDS25.Core\bin\Debug\net8.0-windows\ADDS25.Core.dll"
        $autocadAssembly = "$solutionPath\ADDS25.AutoCAD\bin\Debug\net8.0-windows\ADDS25.AutoCAD.dll"
        $oracleAssembly = "$solutionPath\ADDS25.Oracle\bin\Debug\net8.0\ADDS25.Oracle.dll"
        
        if (Test-Path $coreAssembly) {
            Write-SessionLog "✅ ADDS25.Core Assembly: Built" "SUCCESS"
        } else {
            Write-SessionLog "❌ ADDS25.Core Assembly: Not built" "ERROR"
        }
        
        if (Test-Path $autocadAssembly) {
            Write-SessionLog "✅ ADDS25.AutoCAD Assembly: Built" "SUCCESS"
        } else {
            Write-SessionLog "❌ ADDS25.AutoCAD Assembly: Not built" "ERROR"
        }
        
        if (Test-Path $oracleAssembly) {
            Write-SessionLog "✅ ADDS25.Oracle Assembly: Built" "SUCCESS"
        } else {
            Write-SessionLog "❌ ADDS25.Oracle Assembly: Not built" "ERROR"
        }
    } else {
        Write-SessionLog "❌ ADDS25 Solution: Not found" "ERROR"
    }
}

function Monitor-LauncherExecution {
    param([string]$LauncherPath)
    
    Write-SessionLog "=== LAUNCHER EXECUTION MONITORING ===" "HEADER"
    Write-SessionLog "Launcher Path: $LauncherPath" "INFO"
    
    if (!(Test-Path $LauncherPath)) {
        Write-SessionLog "❌ Launcher not found: $LauncherPath" "ERROR"
        return
    }
    
    try {
        Write-SessionLog "🚀 Starting ADDS25-Launcher.bat execution..." "INFO"
        $startTime = Get-Date
        
        # Execute launcher and capture output
        $process = Start-Process -FilePath $LauncherPath -Wait -PassThru -NoNewWindow
        
        $endTime = Get-Date
        $duration = $endTime - $startTime
        
        Write-SessionLog "⏱️ Launcher execution completed in $($duration.TotalSeconds) seconds" "INFO"
        Write-SessionLog "📊 Exit Code: $($process.ExitCode)" "INFO"
        
        if ($process.ExitCode -eq 0) {
            Write-SessionLog "✅ Launcher execution: SUCCESS" "SUCCESS"
        } else {
            Write-SessionLog "❌ Launcher execution: FAILED (Exit Code: $($process.ExitCode))" "ERROR"
        }
        
    } catch {
        Write-SessionLog "❌ Launcher execution error: $($_.Exception.Message)" "ERROR"
    }
}

function Test-PostDeployment {
    Write-SessionLog "=== POST-DEPLOYMENT VALIDATION ===" "HEADER"
    
    # Test directory creation
    $directories = @(
        "C:\ADDS25",
        "C:\ADDS25\Assemblies",
        "C:\ADDS25_Map",
        "C:\ProgramData\ADDS25Temp"
    )
    
    foreach ($dir in $directories) {
        if (Test-Path $dir) {
            Write-SessionLog "✅ Directory created: $dir" "SUCCESS"
        } else {
            Write-SessionLog "❌ Directory missing: $dir" "ERROR"
        }
    }
    
    # Test assembly deployment
    $assemblies = @(
        "C:\ADDS25\Assemblies\ADDS25.Core.dll",
        "C:\ADDS25\Assemblies\ADDS25.AutoCAD.dll",
        "C:\ADDS25\Assemblies\ADDS25.Oracle.dll"
    )
    
    foreach ($assembly in $assemblies) {
        if (Test-Path $assembly) {
            Write-SessionLog "✅ Assembly deployed: $(Split-Path $assembly -Leaf)" "SUCCESS"
        } else {
            Write-SessionLog "❌ Assembly missing: $(Split-Path $assembly -Leaf)" "ERROR"
        }
    }
    
    # Test configuration deployment
    $configFile = "C:\ADDS25_Map\adds25_config.json"
    if (Test-Path $configFile) {
        Write-SessionLog "✅ Configuration file deployed: adds25_config.json" "SUCCESS"
    } else {
        Write-SessionLog "❌ Configuration file missing: adds25_config.json" "ERROR"
    }
}

function Generate-Summary {
    Write-SessionLog "=== DEPLOYMENT SESSION SUMMARY ===" "HEADER"
    
    # Count success/error messages
    $logContent = Get-Content $SessionLogFile
    $successCount = ($logContent | Where-Object { $_ -like "*SUCCESS*" }).Count
    $errorCount = ($logContent | Where-Object { $_ -like "*ERROR*" }).Count
    $warningCount = ($logContent | Where-Object { $_ -like "*WARNING*" }).Count
    
    Write-SessionLog "📊 Success Messages: $successCount" "INFO"
    Write-SessionLog "⚠️ Warning Messages: $warningCount" "INFO"
    Write-SessionLog "❌ Error Messages: $errorCount" "INFO"
    
    if ($errorCount -eq 0) {
        Write-SessionLog "🎉 DEPLOYMENT STATUS: SUCCESS" "SUCCESS"
    } elseif ($errorCount -le 2) {
        Write-SessionLog "⚠️ DEPLOYMENT STATUS: SUCCESS WITH WARNINGS" "WARNING"
    } else {
        Write-SessionLog "❌ DEPLOYMENT STATUS: FAILED" "ERROR"
    }
    
    Write-SessionLog "📝 Session log saved to: $SessionLogFile" "INFO"
    Write-SessionLog "📝 PowerShell log available at: $PowerShellLogFile" "INFO"
    Write-SessionLog "📝 Error analysis available at: $ErrorLogFile" "INFO"
}

# Main execution
Write-Host "🔍 ADDS25 Deployment Monitor Starting..." -ForegroundColor Cyan
Write-Host "📝 Session Log: $SessionLogFile" -ForegroundColor Green

# Run prerequisite tests
Test-Prerequisites

    # Monitor launcher execution
if ($Environment -eq "Test") {
    $launcherPath = "C:\Users\wa-bdpilegg\Downloads\ALARM\tests\ADDS25\v0.1\ADDS25-Launcher.bat"
} else {
    $launcherPath = "C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1\ADDS25-Launcher.bat"
}
Monitor-LauncherExecution $launcherPath

# Test post-deployment state
Test-PostDeployment

# Generate summary
Generate-Summary

Write-Host "✅ ADDS25 Deployment Monitor Complete!" -ForegroundColor Green
Write-Host "📊 Review session log: $SessionLogFile" -ForegroundColor Yellow

# Open session log for review
if (Test-Path $SessionLogFile) {
    try {
        Start-Process notepad.exe $SessionLogFile
    } catch {
        Write-Host "Session log created at: $SessionLogFile" -ForegroundColor Yellow
    }
}
