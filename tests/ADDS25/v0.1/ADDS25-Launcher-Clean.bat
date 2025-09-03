@ECHO OFF
REM ADDS25 Clean Launcher - Fixed Version
REM Purpose: Launch ADDS25 with proper batch/PowerShell separation
REM Date: September 2, 2025

ECHO *** ADDS25 Clean Launcher Starting ***
ECHO Application: ADDS25 - Modernized Legacy System
ECHO Framework: .NET Core 8
ECHO AutoCAD: Map3D 2025
ECHO Oracle: 19c
ECHO.

REM Step 1: Create logging directory
ECHO *** Step 1: Creating logging directory ***
if not exist "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results" (
    mkdir "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"
    ECHO Created logging directory
) else (
    ECHO Logging directory exists
)

REM Step 2: Initialize PowerShell logging
ECHO *** Step 2: Initializing PowerShell logging ***
PowerShell.exe -ExecutionPolicy Bypass -Command "& {
    $timestamp = Get-Date -Format 'yyyy-MM-dd_HH-mm-ss'
    $logDir = 'C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results'
    $sessionLog = \"$logDir\launcher-execution-$timestamp.md\"
    
    # Create session log
    @\"
# ADDS25 Launcher Execution Log - Clean Version

**Execution Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Launcher**: ADDS25-Launcher-Clean.bat
**Session ID**: $timestamp

---

## LAUNCHER EXECUTION LOG

Starting clean launcher at: $(Get-Date -Format 'HH:mm:ss')

### Phase 1: Logging Initialization
✅ Logging directory: $logDir
✅ Session log: $sessionLog

\"@ | Out-File $sessionLog -Encoding UTF8
    
    Write-Host \"Logging initialized: $sessionLog\" -ForegroundColor Green
}"

REM Step 3: Directory setup
ECHO *** Step 3: Directory setup ***
PowerShell.exe -ExecutionPolicy Bypass -Command "& {
    $logDir = 'C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results'
    $latestLog = Get-ChildItem \"$logDir\launcher-execution-*.md\" | Sort-Object LastWriteTime | Select-Object -Last 1
    if ($latestLog) {
        Add-Content $latestLog.FullName \"`n### Phase 2: Directory Setup\" -Encoding UTF8
        Add-Content $latestLog.FullName \"Starting directory setup at: $(Get-Date -Format 'HH:mm:ss')\" -Encoding UTF8
    }
    
    # Execute directory setup
    Start-Process PowerShell.exe -ArgumentList '-ExecutionPolicy Bypass -File \"%~dp0ADDS25-DirSetup.ps1\"' -Verb RunAs -Wait
}"

REM Step 4: Wait for setup completion
ECHO *** Step 4: Waiting for setup completion ***
ECHO Waiting 10 seconds for directory setup...
timeout /t 10 /nobreak >nul

REM Step 5: Application setup
ECHO *** Step 5: Application setup ***
PowerShell.exe -ExecutionPolicy Bypass -Command "& {
    $logDir = 'C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results'
    $latestLog = Get-ChildItem \"$logDir\launcher-execution-*.md\" | Sort-Object LastWriteTime | Select-Object -Last 1
    if ($latestLog) {
        Add-Content $latestLog.FullName \"`n### Phase 3: Application Setup\" -Encoding UTF8
        Add-Content $latestLog.FullName \"Starting application setup at: $(Get-Date -Format 'HH:mm:ss')\" -Encoding UTF8
    }
    
    # Execute application setup
    & '%~dp0ADDS25-AppSetup.ps1'
    
    # Post-execution analysis
    if ($latestLog) {
        Add-Content $latestLog.FullName \"`n### Post-Execution Analysis\" -Encoding UTF8
        $autocadProcess = Get-Process -Name 'acad' -ErrorAction SilentlyContinue
        if ($autocadProcess) {
            Add-Content $latestLog.FullName \"SUCCESS: AutoCAD Process RUNNING (PID: $($autocadProcess.Id))\" -Encoding UTF8
        } else {
            Add-Content $latestLog.FullName \"WARNING: AutoCAD Process NOT DETECTED\" -Encoding UTF8
        }
        Add-Content $latestLog.FullName \"Launcher execution completed at: $(Get-Date -Format 'HH:mm:ss')\" -Encoding UTF8
        Add-Content $latestLog.FullName \"`n---`n**Session Status**: COMPLETED\" -Encoding UTF8
    }
}"

ECHO *** ADDS25 Clean Launcher Complete ***
ECHO *** Check C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results\ for logs ***
ECHO *** Working Directory: %CD% ***
ECHO *** Launcher Path: %~dp0 ***
ECHO *** Current User: %USERNAME% ***
PAUSE
