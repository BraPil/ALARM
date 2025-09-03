# ADDS25 Comprehensive Logged Launcher
# Purpose: Execute ADDS25-Launcher.bat with complete automated logging
# Target: wa-bdpilegg test computer
# Date: September 1, 2025

Write-Host "🚀 ADDS25 Comprehensive Logged Launcher Starting..." -ForegroundColor Cyan
Write-Host "This will capture ALL output from the ADDS25 launcher execution" -ForegroundColor Yellow

# Initialize comprehensive logging
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logDir = "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"
$sessionLog = "$logDir\launcher-execution-$timestamp.md"
$transcriptLog = "$logDir\launcher-transcript-$timestamp.txt"
$errorLog = "$logDir\launcher-errors-$timestamp.log"

# Create logging directory
if (!(Test-Path $logDir)) {
    New-Item $logDir -Type Directory -Force | Out-Null
    Write-Host "✅ Created logging directory: $logDir" -ForegroundColor Green
}

# Initialize session log
@"
# ADDS25 Launcher Execution Log

**Execution Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Launcher**: ADDS25-Launcher.bat
**.NET Version**: $(dotnet --version)
**Session ID**: $timestamp

---

## 🎯 **PRE-EXECUTION STATUS**

**Current Directory**: $(Get-Location)
**Build Status**: $(if ((dotnet build ADDS25.sln --verbosity quiet 2>&1; $LASTEXITCODE -eq 0)) { 'SUCCESS' } else { 'FAILED' })
**AutoCAD Installation**: $(if (Test-Path 'C:\Program Files\Autodesk\AutoCAD 2025') { 'FOUND at C:\Program Files\Autodesk\AutoCAD 2025' } elseif (Test-Path 'C:\Program Files\Autodesk\AutoCAD Map 3D 2025') { 'FOUND at C:\Program Files\Autodesk\AutoCAD Map 3D 2025' } else { 'NOT FOUND' })

---

## 🚀 **LAUNCHER EXECUTION OUTPUT**

Starting launcher execution at: $(Get-Date -Format 'HH:mm:ss')

"@ | Out-File $sessionLog -Encoding UTF8

Write-Host "✅ Session log initialized: $sessionLog" -ForegroundColor Green

# Navigate to ADDS25 directory
$adds25Path = "C:\Users\wa-bdpilegg\Downloads\ALARM\tests\ADDS25\v0.1"
if (!(Test-Path $adds25Path)) {
    Write-Host "❌ ADDS25 directory not found: $adds25Path" -ForegroundColor Red
    Add-Content $sessionLog "`n❌ **CRITICAL ERROR**: ADDS25 directory not found at $adds25Path" -Encoding UTF8
    return
}

Set-Location $adds25Path
Write-Host "✅ Navigated to: $(Get-Location)" -ForegroundColor Green
Add-Content $sessionLog "`nNavigated to: $(Get-Location)" -Encoding UTF8

# Verify launcher exists
if (!(Test-Path "ADDS25-Launcher.bat")) {
    Write-Host "❌ ADDS25-Launcher.bat not found in current directory" -ForegroundColor Red
    Add-Content $sessionLog "`n❌ **CRITICAL ERROR**: ADDS25-Launcher.bat not found" -Encoding UTF8
    return
}

Write-Host "✅ ADDS25-Launcher.bat found" -ForegroundColor Green

# Execute launcher with comprehensive logging
Write-Host ""
Write-Host "🎯 EXECUTING ADDS25-Launcher.bat with full logging..." -ForegroundColor Cyan
Write-Host "📝 All output will be captured to: $logDir" -ForegroundColor Yellow
Write-Host ""

Add-Content $sessionLog "`n### Launcher Output:" -Encoding UTF8
Add-Content $sessionLog "``````" -Encoding UTF8

# Start transcript for complete capture
Start-Transcript -Path $transcriptLog -Force

try {
    # Execute the launcher and capture all output
    $launcherOutput = & ".\ADDS25-Launcher.bat" 2>&1 | Tee-Object -Variable output
    
    # Log the output
    $output | ForEach-Object {
        Add-Content $sessionLog $_ -Encoding UTF8
    }
    
    Write-Host "✅ Launcher execution completed" -ForegroundColor Green
    
} catch {
    $errorMsg = "Launcher execution failed: $($_.Exception.Message)"
    Write-Host "❌ $errorMsg" -ForegroundColor Red
    Add-Content $sessionLog "`n❌ **ERROR**: $errorMsg" -Encoding UTF8
    Add-Content $errorLog "$(Get-Date): $errorMsg" -Encoding UTF8
} finally {
    Stop-Transcript
}

# Close launcher output section
Add-Content $sessionLog "``````" -Encoding UTF8

# Post-execution analysis
Add-Content $sessionLog "`n---`n" -Encoding UTF8
Add-Content $sessionLog "## 📋 **POST-EXECUTION ANALYSIS**`n" -Encoding UTF8
Add-Content $sessionLog "**Completion Time**: $(Get-Date -Format 'HH:mm:ss')" -Encoding UTF8

# Check for common indicators
$processCheck = Get-Process -Name "acad" -ErrorAction SilentlyContinue
if ($processCheck) {
    Add-Content $sessionLog "✅ **AutoCAD Process**: RUNNING (PID: $($processCheck.Id))" -Encoding UTF8
    Write-Host "✅ AutoCAD is running (PID: $($processCheck.Id))" -ForegroundColor Green
} else {
    Add-Content $sessionLog "❌ **AutoCAD Process**: NOT RUNNING" -Encoding UTF8
    Write-Host "⚠️ AutoCAD process not detected" -ForegroundColor Yellow
}

# Check for error indicators in output
if ($output -match "error|failed|exception") {
    Add-Content $sessionLog "⚠️ **Potential Issues**: Error keywords detected in output" -Encoding UTF8
    Write-Host "⚠️ Potential issues detected - check logs for details" -ForegroundColor Yellow
} else {
    Add-Content $sessionLog "✅ **Output Analysis**: No obvious error indicators" -Encoding UTF8
    Write-Host "✅ No obvious error indicators in output" -ForegroundColor Green
}

# Final summary
Write-Host ""
Write-Host "📋 EXECUTION COMPLETE - LOGS CREATED:" -ForegroundColor Cyan
Write-Host "📝 Session Log: $sessionLog" -ForegroundColor White
Write-Host "📝 Transcript: $transcriptLog" -ForegroundColor White
if (Test-Path $errorLog) {
    Write-Host "📝 Error Log: $errorLog" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎯 NEXT STEPS:" -ForegroundColor Yellow
Write-Host "1. Review the session log for detailed execution results" -ForegroundColor White
Write-Host "2. Check if AutoCAD opened successfully" -ForegroundColor White
Write-Host "3. Verify ADDS25 functionality within AutoCAD" -ForegroundColor White
Write-Host "4. Upload logs to GitHub for analysis" -ForegroundColor White

Add-Content $sessionLog "`n---`n**Session completed at**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -Encoding UTF8

Write-Host ""
Write-Host "✅ COMPREHENSIVE LOGGING COMPLETE!" -ForegroundColor Green
