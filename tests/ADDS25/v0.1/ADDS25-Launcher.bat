@ECHO OFF
REM ADDS25 Modernized Launcher with Integrated Logging
REM Original: ADDS19TransTest.bat
REM Migration: Updated for ADDS25 with .NET Core 8, AutoCAD Map3D 2025, Oracle 19c
REM Date: September 1, 2025
REM Features: Automatic comprehensive logging, single execution point

ECHO *** ADDS25 Launcher with Integrated Logging Starting ***
ECHO Application: ADDS25 - Modernized Legacy System
ECHO Framework: .NET Core 8
ECHO AutoCAD: Map3D 2025  
ECHO Oracle: 19c
ECHO Logging: Comprehensive automated capture
ECHO.

REM ADDS25: Initialize Comprehensive Logging System
ECHO *** Initializing Comprehensive Logging ***
PowerShell.exe -Command "& {
    $timestamp = Get-Date -Format 'yyyy-MM-dd_HH-mm-ss'
    $logDir = 'C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results'
    $sessionLog = \"$logDir\launcher-execution-$timestamp.md\"
    $transcriptLog = \"$logDir\launcher-transcript-$timestamp.txt\"
    
    # Create logging directory
    if (!(Test-Path $logDir)) { New-Item $logDir -Type Directory -Force | Out-Null }
    
    # Initialize session log
    @\"
# ADDS25 Launcher Execution Log

**Execution Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Launcher**: ADDS25-Launcher.bat (Integrated Logging)
**.NET Version**: $(dotnet --version)
**Session ID**: $timestamp

---

## 🎯 **LAUNCHER EXECUTION LOG**

Starting integrated launcher with comprehensive logging at: $(Get-Date -Format 'HH:mm:ss')

### Phase 0: Logging Initialization
✅ Logging directory created: $logDir
✅ Session log initialized: $sessionLog
✅ Transcript log initialized: $transcriptLog

\"@ | Out-File $sessionLog -Encoding UTF8
    
    Write-Host \"✅ Comprehensive logging initialized: $sessionLog\" -ForegroundColor Green
    Start-Transcript -Path $transcriptLog -Force
}"

REM ADDS25: Phase 1 - Directory Setup with Administrator Elevation
ECHO *** Phase 1: Directory Setup ***
PowerShell.exe -Command "& {
    $logDir = 'C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results'
    $latestLog = Get-ChildItem \"$logDir\launcher-execution-*.md\" | Sort-Object LastWriteTime | Select-Object -Last 1
    if ($latestLog) {
        Add-Content $latestLog.FullName \"`n### Phase 1: Directory Setup\" -Encoding UTF8
        Add-Content $latestLog.FullName \"Starting directory setup at: $(Get-Date -Format 'HH:mm:ss')\" -Encoding UTF8
    }
    Start-Process PowerShell.exe -ArgumentList '-ExecutionPolicy Bypass -File \"%~dp0ADDS25-DirSetup.ps1\"' -Verb RunAs
}"

REM ADDS25: Wait for directory setup completion
ECHO Waiting for directory setup completion...
PowerShell.exe -Command "& {
    $logDir = 'C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results'
    $latestLog = Get-ChildItem \"$logDir\launcher-execution-*.md\" | Sort-Object LastWriteTime | Select-Object -Last 1
    if ($latestLog) {
        Add-Content $latestLog.FullName \"Directory setup wait period: 3 seconds\" -Encoding UTF8
    }
}"
timeout /t 3 /nobreak >nul

REM ADDS25: Phase 2 - Application Setup and AutoCAD Launch
ECHO *** Phase 2: Application Setup ***
PowerShell.exe -Command "& {
    $logDir = 'C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results'
    $latestLog = Get-ChildItem \"$logDir\launcher-execution-*.md\" | Sort-Object LastWriteTime | Select-Object -Last 1
    if ($latestLog) {
        Add-Content $latestLog.FullName \"`n### Phase 2: Application Setup and AutoCAD Launch\" -Encoding UTF8
        Add-Content $latestLog.FullName \"Starting application setup at: $(Get-Date -Format 'HH:mm:ss')\" -Encoding UTF8
    }
    & '%~dp0ADDS25-AppSetup.ps1'
    
    # Post-execution analysis
    if ($latestLog) {
        Add-Content $latestLog.FullName \"`n### Post-Execution Analysis\" -Encoding UTF8
        $autocadProcess = Get-Process -Name 'acad' -ErrorAction SilentlyContinue
        if ($autocadProcess) {
            Add-Content $latestLog.FullName \"✅ AutoCAD Process: RUNNING (PID: $($autocadProcess.Id))\" -Encoding UTF8
        } else {
            Add-Content $latestLog.FullName \"❌ AutoCAD Process: NOT DETECTED\" -Encoding UTF8
        }
        Add-Content $latestLog.FullName \"Launcher execution completed at: $(Get-Date -Format 'HH:mm:ss')\" -Encoding UTF8
        Add-Content $latestLog.FullName \"`n---`n**Session Status**: COMPLETED\" -Encoding UTF8
    }
    
    Stop-Transcript -ErrorAction SilentlyContinue
}"

ECHO *** ADDS25 Launcher Complete ***
ECHO *** Check C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results\ for comprehensive logs ***
PAUSE
