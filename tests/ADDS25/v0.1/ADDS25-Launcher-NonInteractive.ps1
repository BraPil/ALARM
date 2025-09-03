# ADDS25 Non-Interactive PowerShell Launcher
# Purpose: Launch ADDS25 with comprehensive logging - No user interaction for CI
# Environment: Test Computer (wa-bdpilegg) - CI Automation
# Date: September 3, 2025

param(
    [switch]$Silent,
    [int]$AutoCloseTimeout = 30
)

if (!$Silent) {
    Write-Host "*** ADDS25 Non-Interactive Launcher Starting ***" -ForegroundColor Cyan
    Write-Host "Application: ADDS25 - Modernized Legacy System" -ForegroundColor Green
    Write-Host "Framework: .NET Core 8" -ForegroundColor Green
    Write-Host "AutoCAD: Map3D 2025" -ForegroundColor Green
    Write-Host "Oracle: 19c" -ForegroundColor Green
    Write-Host "Mode: NON-INTERACTIVE (CI Automation)" -ForegroundColor Yellow
    Write-Host ""
}

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
    if (!$Silent) {
        Write-Host $logEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } })
    }
    if (Test-Path $sessionLog) {
        Add-Content $sessionLog $logEntry -Encoding UTF8
    }
}

# Import AutoCAD Process Manager
try {
    Import-Module "$repoPath\ci\AUTOCAD-PROCESS-MANAGER.ps1" -Force
    Write-LauncherLog "AutoCAD Process Manager imported successfully" "SUCCESS"
} catch {
    Write-LauncherLog "Warning: Could not import AutoCAD Process Manager: $($_.Exception.Message)" "WARNING"
}

# Step 1: Create logging directory
if (!$Silent) { Write-Host "*** Step 1: Creating logging directory ***" -ForegroundColor Yellow }
if (!(Test-Path $logDir)) {
    New-Item $logDir -Type Directory -Force | Out-Null
    Write-LauncherLog "Created logging directory: $logDir" "SUCCESS"
} else {
    Write-LauncherLog "Logging directory exists: $logDir" "SUCCESS"
}

# Step 2: Initialize session logging
if (!$Silent) { Write-Host "*** Step 2: Initializing session logging ***" -ForegroundColor Yellow }
$logContent = @"
# ADDS25 Launcher Execution Log - Non-Interactive

**Execution Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Launcher**: ADDS25-Launcher-NonInteractive.ps1 (CI Automation)
**Session ID**: $timestamp
**Working Directory**: $(Get-Location)
**PowerShell Version**: $($PSVersionTable.PSVersion)
**Mode**: NON-INTERACTIVE
**Auto-Close Timeout**: $AutoCloseTimeout seconds

---

## LAUNCHER EXECUTION LOG

Starting non-interactive launcher at: $(Get-Date -Format 'HH:mm:ss')

### Phase 1: Logging Initialization
- Logging directory: $logDir
- Session log: $sessionLog
- Transcript log: $transcriptLog

"@

$logContent | Out-File $sessionLog -Encoding UTF8
Write-LauncherLog "Session logging initialized: $sessionLog" "SUCCESS"

# Step 3: Start PowerShell transcript
if (!$Silent) { Write-Host "*** Step 3: Starting PowerShell transcript ***" -ForegroundColor Yellow }
try {
    Start-Transcript -Path $transcriptLog -Force
    Write-LauncherLog "PowerShell transcript started: $transcriptLog" "SUCCESS"
} catch {
    Write-LauncherLog "Warning: Could not start transcript: $($_.Exception.Message)" "WARNING"
}

# Step 4: Clean AutoCAD processes before starting
if (!$Silent) { Write-Host "*** Step 4: Cleaning AutoCAD processes ***" -ForegroundColor Yellow }
try {
    if (Get-Command "Ensure-AutoCADClosed" -ErrorAction SilentlyContinue) {
        $cleanResult = Ensure-AutoCADClosed
        if ($cleanResult) {
            Write-LauncherLog "AutoCAD processes cleaned successfully" "SUCCESS"
        } else {
            Write-LauncherLog "Warning: AutoCAD cleanup had issues" "WARNING"
        }
    } else {
        Write-LauncherLog "AutoCAD Process Manager not available - manual cleanup" "WARNING"
        $existingProcesses = Get-Process -Name "acad" -ErrorAction SilentlyContinue
        if ($existingProcesses) {
            $existingProcesses | ForEach-Object { $_.Kill() }
            Write-LauncherLog "Killed $($existingProcesses.Count) existing AutoCAD processes" "WARNING"
        }
    }
} catch {
    Write-LauncherLog "Error during AutoCAD cleanup: $($_.Exception.Message)" "WARNING"
}

# Step 5: Verify ADDS25 build
if (!$Silent) { Write-Host "*** Step 5: Verifying ADDS25 build ***" -ForegroundColor Yellow }
Write-LauncherLog "Checking ADDS25 build status..." "INFO"

Set-Location $adds25Path
Write-LauncherLog "Changed to ADDS25 directory: $(Get-Location)" "INFO"

try {
    Write-LauncherLog "Starting .NET build..." "INFO"
    $buildResult = & dotnet build --configuration Debug --verbosity quiet 2>&1
    
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

# Step 6: Execute directory setup
if (!$Silent) { Write-Host "*** Step 6: Executing directory setup ***" -ForegroundColor Yellow }
try {
    $dirSetupScript = "$adds25Path\ADDS25-DirSetup.ps1"
    if (Test-Path $dirSetupScript) {
        Write-LauncherLog "Executing directory setup script..." "INFO"
        & $dirSetupScript
        Write-LauncherLog "Directory setup completed" "SUCCESS"
    } else {
        Write-LauncherLog "Directory setup script not found: $dirSetupScript" "WARNING"
    }
} catch {
    Write-LauncherLog "Directory setup error: $($_.Exception.Message)" "WARNING"
}

# Step 7: Check for AutoCAD installation and launch
if (!$Silent) { Write-Host "*** Step 7: Launching AutoCAD ***" -ForegroundColor Yellow }
Write-LauncherLog "Verifying AutoCAD Map3D 2025 installation..." "INFO"

$autocadPath = "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe"
if (Test-Path $autocadPath) {
    Write-LauncherLog "AutoCAD Map3D 2025 found: $autocadPath" "SUCCESS"
    Add-Content $sessionLog "`n### Phase 3: AutoCAD Launch" -Encoding UTF8
    Add-Content $sessionLog "AutoCAD found at: $autocadPath" -Encoding UTF8
    
    try {
        Write-LauncherLog "Launching AutoCAD Map3D 2025..." "INFO"
        $autocadProcess = Start-Process $autocadPath -PassThru
        
        # Wait for AutoCAD to start
        Start-Sleep -Seconds 8
        
        # Check if AutoCAD is running
        $runningProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
        if ($runningProcess) {
            Write-LauncherLog "AutoCAD launched successfully (PID: $($runningProcess.Id))" "SUCCESS"
            Add-Content $sessionLog "AutoCAD process ID: $($runningProcess.Id)" -Encoding UTF8
            Add-Content $sessionLog "Launch time: $(Get-Date -Format 'HH:mm:ss')" -Encoding UTF8
            
            # Step 8: Execute application setup
            if (!$Silent) { Write-Host "*** Step 8: Executing application setup ***" -ForegroundColor Yellow }
            try {
                $appSetupScript = "$adds25Path\ADDS25-AppSetup.ps1"
                if (Test-Path $appSetupScript) {
                    Write-LauncherLog "Executing application setup script..." "INFO"
                    & $appSetupScript
                    Write-LauncherLog "Application setup completed" "SUCCESS"
                } else {
                    Write-LauncherLog "Application setup script not found: $appSetupScript" "WARNING"
                }
            } catch {
                Write-LauncherLog "Application setup error: $($_.Exception.Message)" "WARNING"
            }
            
            # Step 9: Monitor AutoCAD activity and auto-close
            if (!$Silent) { Write-Host "*** Step 9: Monitoring AutoCAD activity ***" -ForegroundColor Yellow }
            Write-LauncherLog "Starting AutoCAD monitoring with $AutoCloseTimeout second timeout..." "INFO"
            
            try {
                if (Get-Command "Monitor-AutoCADActivity" -ErrorAction SilentlyContinue) {
                    Monitor-AutoCADActivity -TimeoutSeconds $AutoCloseTimeout
                } else {
                    Write-LauncherLog "AutoCAD monitoring not available - using basic timeout" "WARNING"
                    Start-Sleep -Seconds $AutoCloseTimeout
                    $stillRunning = Get-Process -Name "acad" -ErrorAction SilentlyContinue
                    if ($stillRunning) {
                        $stillRunning | ForEach-Object { $_.Kill() }
                        Write-LauncherLog "Auto-closed AutoCAD after timeout" "INFO"
                    }
                }
            } catch {
                Write-LauncherLog "AutoCAD monitoring error: $($_.Exception.Message)" "WARNING"
            }
            
        } else {
            Write-LauncherLog "AutoCAD process not detected after launch attempt" "WARNING"
            Add-Content $sessionLog "Process not detected after launch" -Encoding UTF8
        }
    } catch {
        Write-LauncherLog "Error launching AutoCAD: $($_.Exception.Message)" "ERROR"
        Add-Content $sessionLog "Launch error: $($_.Exception.Message)" -Encoding UTF8
    }
} else {
    Write-LauncherLog "AutoCAD Map3D 2025 not found at expected location" "WARNING"
    Add-Content $sessionLog "`n### Phase 3: AutoCAD Launch - SKIPPED" -Encoding UTF8
    Add-Content $sessionLog "Reason: AutoCAD not found" -Encoding UTF8
}

# Step 10: Final status and cleanup
if (!$Silent) { Write-Host "*** Step 10: Final status and cleanup ***" -ForegroundColor Yellow }
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

if (!$Silent) {
    Write-Host ""
    Write-Host "*** ADDS25 Non-Interactive Launcher Complete ***" -ForegroundColor Green
    Write-Host "*** Check $logDir for comprehensive logs ***" -ForegroundColor Yellow
    Write-Host "*** Session log: $sessionLog ***" -ForegroundColor Yellow
    Write-Host "*** Transcript: $transcriptLog ***" -ForegroundColor Yellow
}

# Final AutoCAD status
$finalAutocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
if ($finalAutocadProcess) {
    Write-LauncherLog "Final Status: AutoCAD is running (PID: $($finalAutocadProcess.Id))" "INFO"
    return @{ Status = "SUCCESS"; AutoCAD = "RUNNING"; PID = $finalAutocadProcess.Id }
} else {
    Write-LauncherLog "Final Status: AutoCAD process not detected" "INFO"
    return @{ Status = "SUCCESS"; AutoCAD = "NOT_RUNNING"; PID = $null }
}
