# ADDS25 Test Computer Automated Setup & Logging Script
# Purpose: Clean repository setup with automated result capture on test computer
# Target: wa-bdpilegg test computer
# Date: September 1, 2025

Write-Host "🚀 ADDS25 Test Computer Automated Setup Starting..." -ForegroundColor Cyan
Write-Host "This script will create automated logging and clean repository setup" -ForegroundColor Yellow
Write-Host ""

# Initialize logging immediately
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logDir = "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"
$sessionLog = "$logDir\test-session-$timestamp.md"

# Create logging directory
if (!(Test-Path $logDir)) {
    New-Item $logDir -Type Directory -Force | Out-Null
    Write-Host "✅ Created logging directory: $logDir" -ForegroundColor Green
}

# Start logging function
function Write-TestLog {
    param([string]$Message, [string]$Level = "INFO")
    $logEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $logEntry
    Add-Content $sessionLog $logEntry -Encoding UTF8
}

# Initialize session log
@"
# ADDS25 Test Session Log

**Session ID**: $timestamp
**Date**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Purpose**: Clean repository setup and ADDS25 deployment testing

---

## 🎯 **SESSION LOG**

"@ | Out-File $sessionLog -Encoding UTF8

Write-TestLog "ADDS25 Test Computer Setup Session Started" "START"
Write-TestLog "Logging to: $sessionLog" "INFO"

# Step 1: Close any PowerShell windows in ALARM directory
Write-TestLog "Step 1: Checking for PowerShell processes in ALARM directory..." "INFO"
Write-Host ""
Write-Host "⚠️ IMPORTANT: Close any PowerShell windows that are in the ALARM directory" -ForegroundColor Yellow
Write-Host "Press any key after closing other PowerShell windows..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Step 2: Clean removal of existing ALARM directory
Write-TestLog "Step 2: Removing existing ALARM directory for clean setup..." "INFO"

$downloadsPath = "C:\Users\wa-bdpilegg\Downloads"
$alarmPath = "$downloadsPath\ALARM"

# Navigate to Downloads to avoid directory lock
Set-Location $downloadsPath
Write-TestLog "Navigated to Downloads directory" "INFO"

if (Test-Path $alarmPath) {
    try {
        Remove-Item $alarmPath -Recurse -Force
        Write-TestLog "Successfully removed existing ALARM directory" "SUCCESS"
        Write-Host "✅ Existing ALARM directory removed" -ForegroundColor Green
    } catch {
        Write-TestLog "Failed to remove ALARM directory: $($_.Exception.Message)" "ERROR"
        Write-Host "❌ Could not remove ALARM directory. Please close all applications using it." -ForegroundColor Red
        Write-Host "Press any key after closing applications..." -ForegroundColor Yellow
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
        
        try {
            Remove-Item $alarmPath -Recurse -Force
            Write-TestLog "Successfully removed ALARM directory on second attempt" "SUCCESS"
            Write-Host "✅ ALARM directory removed" -ForegroundColor Green
        } catch {
            Write-TestLog "CRITICAL: Cannot remove ALARM directory: $($_.Exception.Message)" "CRITICAL"
            Write-Host "❌ CRITICAL: Cannot proceed. Please manually delete $alarmPath" -ForegroundColor Red
            return
        }
    }
} else {
    Write-TestLog "ALARM directory does not exist - clean slate" "INFO"
}

# Step 3: Fresh git clone
Write-TestLog "Step 3: Cloning fresh ALARM repository..." "INFO"
Write-Host ""
Write-Host "Cloning fresh ALARM repository..." -ForegroundColor Yellow

try {
    git clone https://github.com/BraPil/ALARM.git ALARM
    Write-TestLog "Successfully cloned ALARM repository" "SUCCESS"
    Write-Host "✅ Repository cloned successfully" -ForegroundColor Green
} catch {
    Write-TestLog "Git clone failed: $($_.Exception.Message)" "ERROR"
    Write-Host "❌ Git clone failed" -ForegroundColor Red
    return
}

# Step 4: Navigate to repository and verify
Set-Location $alarmPath
Write-TestLog "Navigated to ALARM repository: $(Get-Location)" "INFO"

if (Test-Path ".git") {
    Write-TestLog "Git repository properly initialized" "SUCCESS"
    Write-Host "✅ Git repository properly initialized" -ForegroundColor Green
} else {
    Write-TestLog "ERROR: Git repository not properly initialized" "ERROR"
    Write-Host "❌ Git repository not properly initialized" -ForegroundColor Red
    return
}

# Step 5: Fix username mappings
Write-TestLog "Step 5: Fixing username mappings for wa-bdpilegg environment..." "INFO"
Write-Host ""
Write-Host "Fixing username mappings..." -ForegroundColor Yellow

# Fix ADDS25-AppSetup.ps1
$appSetupFile = "tests\ADDS25\v0.1\ADDS25-AppSetup.ps1"
if (Test-Path $appSetupFile) {
    $content = Get-Content $appSetupFile -Raw
    $originalContent = $content
    $content = $content -replace "C:\\Users\\kidsg\\", "C:\Users\wa-bdpilegg\"
    
    if ($content -ne $originalContent) {
        Set-Content $appSetupFile $content -Encoding UTF8
        Write-TestLog "Fixed username mapping in ADDS25-AppSetup.ps1" "SUCCESS"
        Write-Host "✅ Fixed ADDS25-AppSetup.ps1" -ForegroundColor Green
    } else {
        Write-TestLog "Username mapping already correct in ADDS25-AppSetup.ps1" "INFO"
        Write-Host "✅ ADDS25-AppSetup.ps1 already correct" -ForegroundColor Green
    }
} else {
    Write-TestLog "ERROR: ADDS25-AppSetup.ps1 not found" "ERROR"
    Write-Host "❌ ADDS25-AppSetup.ps1 not found" -ForegroundColor Red
}

# Fix ADDS25.AutoCAD.csproj
$autocadProjFile = "tests\ADDS25\v0.1\ADDS25.AutoCAD\ADDS25.AutoCAD.csproj"
if (Test-Path $autocadProjFile) {
    $content = Get-Content $autocadProjFile -Raw
    $originalContent = $content
    $content = $content -replace "C:\\Users\\kidsg\\", "C:\Users\wa-bdpilegg\"
    
    if ($content -ne $originalContent) {
        Set-Content $autocadProjFile $content -Encoding UTF8
        Write-TestLog "Fixed username mapping in ADDS25.AutoCAD.csproj" "SUCCESS"
        Write-Host "✅ Fixed ADDS25.AutoCAD.csproj" -ForegroundColor Green
    } else {
        Write-TestLog "Username mapping already correct in ADDS25.AutoCAD.csproj" "INFO"
        Write-Host "✅ ADDS25.AutoCAD.csproj already correct" -ForegroundColor Green
    }
} else {
    Write-TestLog "ERROR: ADDS25.AutoCAD.csproj not found" "ERROR"
    Write-Host "❌ ADDS25.AutoCAD.csproj not found" -ForegroundColor Red
}

# Step 6: Test build system
Write-TestLog "Step 6: Testing build system..." "INFO"
Write-Host ""
Write-Host "Testing build system..." -ForegroundColor Yellow

Set-Location "tests\ADDS25\v0.1"
try {
    $buildOutput = dotnet build ADDS25.sln 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-TestLog "Build successful" "SUCCESS"
        Write-Host "✅ Build successful" -ForegroundColor Green
    } else {
        Write-TestLog "Build failed: $buildOutput" "ERROR"
        Write-Host "❌ Build failed" -ForegroundColor Red
        Write-Host "Build output: $buildOutput" -ForegroundColor Yellow
    }
} catch {
    Write-TestLog "Build test failed: $($_.Exception.Message)" "ERROR"
    Write-Host "❌ Build test failed" -ForegroundColor Red
}

# Step 7: Setup automated logging for launcher
Write-TestLog "Step 7: Setting up automated logging for launcher execution..." "INFO"
Write-Host ""
Write-Host "Setting up automated logging..." -ForegroundColor Yellow

# Create launcher wrapper script that captures all output
$launcherWrapper = @"
# ADDS25 Launcher with Automated Logging
`$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
`$logFile = "$logDir\launcher-execution-`$timestamp.md"

# Initialize launcher log
@"
# ADDS25 Launcher Execution Log

**Execution Time**: `$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Launcher**: ADDS25-Launcher.bat

---

## 🚀 **LAUNCHER OUTPUT**

"@ | Out-File `$logFile -Encoding UTF8

Write-Host "🚀 Starting ADDS25 Launcher with automated logging..." -ForegroundColor Cyan
Write-Host "Logging to: `$logFile" -ForegroundColor Yellow

# Execute launcher and capture output
Start-Transcript -Path "`$logFile.transcript" -Append
.\ADDS25-Launcher.bat
Stop-Transcript

Write-Host ""
Write-Host "✅ Launcher execution complete" -ForegroundColor Green
Write-Host "📝 Results logged to: `$logFile" -ForegroundColor Cyan
Write-Host "📝 Transcript saved to: `$logFile.transcript" -ForegroundColor Cyan
"@

$launcherWrapperPath = "ADDS25-Launcher-Logged.ps1"
Set-Content $launcherWrapperPath $launcherWrapper -Encoding UTF8
Write-TestLog "Created automated launcher wrapper: $launcherWrapperPath" "SUCCESS"
Write-Host "✅ Created automated launcher wrapper" -ForegroundColor Green

# Final status report
Set-Location $alarmPath
Write-TestLog "Step 8: Final status verification..." "INFO"
Write-Host ""
Write-Host "📊 SETUP COMPLETION REPORT" -ForegroundColor Cyan
Write-Host "===========================" -ForegroundColor Cyan

$gitStatus = git status --porcelain 2>$null
if ($?) {
    Write-TestLog "Git repository functional" "SUCCESS"
    Write-Host "✅ Git repository: Functional" -ForegroundColor Green
} else {
    Write-TestLog "Git repository not functional" "ERROR"
    Write-Host "❌ Git repository: Not functional" -ForegroundColor Red
}

$deploymentReady = (Test-Path "tests\ADDS25\v0.1\ADDS25-Launcher.bat") -and (Test-Path "tests\ADDS25\v0.1\ADDS25.sln")
if ($deploymentReady) {
    Write-TestLog "Deployment files ready" "SUCCESS"
    Write-Host "✅ Deployment files: Ready" -ForegroundColor Green
} else {
    Write-TestLog "Deployment files missing" "ERROR"
    Write-Host "❌ Deployment files: Missing" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎉 TEST COMPUTER SETUP COMPLETE!" -ForegroundColor Green
Write-Host ""
Write-Host "📍 Current Location: $(Get-Location)" -ForegroundColor Cyan
Write-Host "📝 Session Log: $sessionLog" -ForegroundColor Cyan
Write-Host "📁 All logs will be saved to: $logDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "🚀 NEXT STEPS:" -ForegroundColor Yellow
Write-Host "1. Navigate to deployment directory:" -ForegroundColor White
Write-Host "   cd tests\ADDS25\v0.1" -ForegroundColor Cyan
Write-Host ""
Write-Host "2. Run automated launcher (captures all output):" -ForegroundColor White
Write-Host "   .\ADDS25-Launcher-Logged.ps1" -ForegroundColor Cyan
Write-Host ""
Write-Host "3. All results will be automatically logged to:" -ForegroundColor White
Write-Host "   $logDir" -ForegroundColor Cyan

Write-TestLog "ADDS25 Test Computer Setup Session Complete" "END"
Write-TestLog "Ready for deployment testing with automated logging" "SUCCESS"
