# ADDS25 Pure PowerShell Launcher
# Purpose: Launch ADDS25 with comprehensive logging - Pure PowerShell implementation
# Environment: Test Computer (wa-bdpilegg)
# Date: September 2, 2025

Write-Host "*** ADDS25 Pure PowerShell Launcher Starting ***" -ForegroundColor Cyan
Write-Host "Application: ADDS25 - Modernized Legacy System" -ForegroundColor Green
Write-Host "Framework: .NET Core 8" -ForegroundColor Green
Write-Host "AutoCAD: Map3D 2025" -ForegroundColor Green
Write-Host "Oracle: 19c" -ForegroundColor Green
Write-Host ""

# Configuration
$repoPath = "C:\Users\wa-bdpilegg\Downloads\ALARM"
$adds25Path = "$repoPath\tests\ADDS25\v0.1"
$logDir = "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$sessionLog = "$logDir\launcher-execution-$timestamp.md"
$transcriptLog = "$logDir\launcher-transcript-$timestamp.txt"

function Write-LauncherLog {
    param([string]$Message, [string]$Level = "INFO")
    $logEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $logEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } })
    if (Test-Path $sessionLog) {
        Add-Content $sessionLog $logEntry -Encoding UTF8
    }
}

# Step 1: Create logging directory
Write-Host "*** Step 1: Creating logging directory ***" -ForegroundColor Yellow
if (!(Test-Path $logDir)) {
    New-Item $logDir -Type Directory -Force | Out-Null
    Write-LauncherLog "Created logging directory: $logDir" "SUCCESS"
} else {
    Write-LauncherLog "Logging directory exists: $logDir" "SUCCESS"
}

# Step 2: Initialize session logging
Write-Host "*** Step 2: Initializing session logging ***" -ForegroundColor Yellow
$logContent = @"
# ADDS25 Launcher Execution Log - Pure PowerShell

**Execution Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Launcher**: ADDS25-Launcher.ps1 (Pure PowerShell)
**Session ID**: $timestamp
**Working Directory**: $(Get-Location)
**PowerShell Version**: $($PSVersionTable.PSVersion)

---

## LAUNCHER EXECUTION LOG

Starting pure PowerShell launcher at: $(Get-Date -Format 'HH:mm:ss')

### Phase 1: Logging Initialization
- Logging directory: $logDir
- Session log: $sessionLog
- Transcript log: $transcriptLog

"@

$logContent | Out-File $sessionLog -Encoding UTF8
Write-LauncherLog "Session logging initialized: $sessionLog" "SUCCESS"

# Step 3: Start PowerShell transcript
Write-Host "*** Step 3: Starting PowerShell transcript ***" -ForegroundColor Yellow
try {
    Start-Transcript -Path $transcriptLog -Force
    Write-LauncherLog "PowerShell transcript started: $transcriptLog" "SUCCESS"
} catch {
    Write-LauncherLog "Warning: Could not start transcript: $($_.Exception.Message)" "WARNING"
}

# Step 4: Verify ADDS25 build
Write-Host "*** Step 4: Verifying ADDS25 build ***" -ForegroundColor Yellow
Write-LauncherLog "Checking ADDS25 build status..." "INFO"

Set-Location $adds25Path
Write-LauncherLog "Changed to ADDS25 directory: $(Get-Location)" "INFO"

try {
    Write-LauncherLog "Starting .NET build..." "INFO"
    $buildResult = & dotnet build --configuration Debug 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-LauncherLog "ADDS25 build completed successfully" "SUCCESS"
        Add-Content $sessionLog "`n### Phase 2: Build Status - SUCCESS" -Encoding UTF8
        Add-Content $sessionLog "Build completed at: $(Get-Date -Format 'HH:mm:ss')" -Encoding UTF8
    } else {
        Write-LauncherLog "ADDS25 build failed with exit code: $LASTEXITCODE" "ERROR"
        Add-Content $sessionLog "`n### Phase 2: Build Status - FAILED" -Encoding UTF8
        Add-Content $sessionLog "Build failed at: $(Get-Date -Format 'HH:mm:ss')" -Encoding UTF8
        Add-Content $sessionLog "Exit code: $LASTEXITCODE" -Encoding UTF8
    }
} catch {
    Write-LauncherLog "Build error: $($_.Exception.Message)" "ERROR"
    Add-Content $sessionLog "`n### Phase 2: Build Status - ERROR" -Encoding UTF8
    Add-Content $sessionLog "Build error: $($_.Exception.Message)" -Encoding UTF8
}

# Step 5: Check for AutoCAD installation
Write-Host "*** Step 5: Checking AutoCAD installation ***" -ForegroundColor Yellow
Write-LauncherLog "Verifying AutoCAD Map3D 2025 installation..." "INFO"

$autocadPath = "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe"
if (Test-Path $autocadPath) {
    Write-LauncherLog "AutoCAD Map3D 2025 found: $autocadPath" "SUCCESS"
    Add-Content $sessionLog "`n### Phase 3: AutoCAD Verification - SUCCESS" -Encoding UTF8
    Add-Content $sessionLog "AutoCAD found at: $autocadPath" -Encoding UTF8
} else {
    Write-LauncherLog "AutoCAD Map3D 2025 not found at expected location" "WARNING"
    Add-Content $sessionLog "`n### Phase 3: AutoCAD Verification - WARNING" -Encoding UTF8
    Add-Content $sessionLog "AutoCAD not found at: $autocadPath" -Encoding UTF8
}

# Step 6: Launch AutoCAD (if available)
Write-Host "*** Step 6: Attempting AutoCAD launch ***" -ForegroundColor Yellow
if (Test-Path $autocadPath) {
    try {
        Write-LauncherLog "Launching AutoCAD Map3D 2025..." "INFO"
        Start-Process $autocadPath -PassThru
        
        # Wait a moment for AutoCAD to start
        Start-Sleep -Seconds 5
        
        # Check if AutoCAD is running
        $autocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
        if ($autocadProcess) {
            Write-LauncherLog "AutoCAD launched successfully (PID: $($autocadProcess.Id))" "SUCCESS"
            Add-Content $sessionLog "`n### Phase 4: AutoCAD Launch - SUCCESS" -Encoding UTF8
            Add-Content $sessionLog "AutoCAD process ID: $($autocadProcess.Id)" -Encoding UTF8
            Add-Content $sessionLog "Launch time: $(Get-Date -Format 'HH:mm:ss')" -Encoding UTF8
        } else {
            Write-LauncherLog "AutoCAD process not detected after launch attempt" "WARNING"
            Add-Content $sessionLog "`n### Phase 4: AutoCAD Launch - WARNING" -Encoding UTF8
            Add-Content $sessionLog "Process not detected after launch" -Encoding UTF8
        }
    } catch {
        Write-LauncherLog "Error launching AutoCAD: $($_.Exception.Message)" "ERROR"
        Add-Content $sessionLog "`n### Phase 4: AutoCAD Launch - ERROR" -Encoding UTF8
        Add-Content $sessionLog "Launch error: $($_.Exception.Message)" -Encoding UTF8
    }
} else {
    Write-LauncherLog "Skipping AutoCAD launch - not found" "WARNING"
    Add-Content $sessionLog "`n### Phase 4: AutoCAD Launch - SKIPPED" -Encoding UTF8
    Add-Content $sessionLog "Reason: AutoCAD not found" -Encoding UTF8
}

# Step 7: Final status and cleanup
Write-Host "*** Step 7: Final status and cleanup ***" -ForegroundColor Yellow
Write-LauncherLog "Completing launcher execution..." "INFO"

Add-Content $sessionLog "`n### Final Status" -Encoding UTF8
Add-Content $sessionLog "Launcher completed at: $(Get-Date -Format 'HH:mm:ss')" -Encoding UTF8
Add-Content $sessionLog "Working directory: $(Get-Location)" -Encoding UTF8
Add-Content $sessionLog "Session status: COMPLETED" -Encoding UTF8

# Stop transcript
try {
    Stop-Transcript
    Write-LauncherLog "PowerShell transcript stopped" "SUCCESS"
} catch {
    Write-LauncherLog "Warning: Could not stop transcript: $($_.Exception.Message)" "WARNING"
}

Write-Host ""
Write-Host "*** ADDS25 Pure PowerShell Launcher Complete ***" -ForegroundColor Green
Write-Host "*** Check $logDir for comprehensive logs ***" -ForegroundColor Yellow
Write-Host "*** Session log: $sessionLog ***" -ForegroundColor Yellow
Write-Host "*** Transcript: $transcriptLog ***" -ForegroundColor Yellow
Write-Host ""

# Check final AutoCAD status
$finalAutocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
if ($finalAutocadProcess) {
    Write-Host "SUCCESS: AutoCAD is running (PID: $($finalAutocadProcess.Id))" -ForegroundColor Green
} else {
    Write-Host "INFO: AutoCAD process not detected" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
