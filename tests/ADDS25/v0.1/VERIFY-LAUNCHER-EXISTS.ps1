# ADDS25 Launcher Verification Script
# Purpose: Verify launcher files exist and are accessible on test computer
# Environment: Test Computer (wa-bdpilegg)
# Date: September 2, 2025

Write-Host "*** ADDS25 Launcher Verification Script ***" -ForegroundColor Cyan
Write-Host "Purpose: Verify launcher files exist and provide diagnostics" -ForegroundColor Yellow
Write-Host ""

# Define paths
$repoPath = "C:\Users\wa-bdpilegg\Downloads\ALARM"
$adds25Path = "$repoPath\tests\ADDS25\v0.1"
$simpleLauncher = "$adds25Path\ADDS25-Launcher-Simple.bat"
$fullLauncher = "$adds25Path\ADDS25-Launcher.bat"
$verificationScript = "$adds25Path\VERIFY-LAUNCHER-EXISTS.ps1"

# Create results log
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logDir = "$repoPath\test-results"
$logFile = "$logDir\launcher-verification-$timestamp.md"

if (!(Test-Path $logDir)) { New-Item $logDir -Type Directory -Force | Out-Null }

# Initialize log
@"
# ADDS25 Launcher Verification Report

**Verification Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Purpose**: Diagnose launcher file availability issues

---

## FILE EXISTENCE VERIFICATION

"@ | Out-File $logFile -Encoding UTF8

function Write-VerificationLog {
    param([string]$Message, [string]$Status = "INFO")
    $logEntry = "**[$Status]** $Message"
    Write-Host "[$Status] $Message" -ForegroundColor $(switch($Status) { "SUCCESS" { "Green" } "ERROR" { "Red" } "WARNING" { "Yellow" } default { "White" } })
    Add-Content $logFile $logEntry -Encoding UTF8
}

# Check repository directory
Write-VerificationLog "Repository Path: $repoPath"
if (Test-Path $repoPath) {
    Write-VerificationLog "Repository directory EXISTS" "SUCCESS"
} else {
    Write-VerificationLog "Repository directory MISSING" "ERROR"
}

# Check ADDS25 directory
Write-VerificationLog "ADDS25 Path: $adds25Path"
if (Test-Path $adds25Path) {
    Write-VerificationLog "ADDS25 directory EXISTS" "SUCCESS"
} else {
    Write-VerificationLog "ADDS25 directory MISSING" "ERROR"
}

# Check verification script (self-check)
Write-VerificationLog "Verification Script: $verificationScript"
if (Test-Path $verificationScript) {
    Write-VerificationLog "Verification script EXISTS (this file)" "SUCCESS"
} else {
    Write-VerificationLog "Verification script MISSING" "ERROR"
}

# Check simple launcher
Write-VerificationLog "Simple Launcher: $simpleLauncher"
if (Test-Path $simpleLauncher) {
    Write-VerificationLog "Simple launcher EXISTS" "SUCCESS"
    $simpleSize = (Get-Item $simpleLauncher).Length
    Write-VerificationLog "Simple launcher size: $simpleSize bytes" "INFO"
} else {
    Write-VerificationLog "Simple launcher MISSING" "ERROR"
}

# Check full launcher
Write-VerificationLog "Full Launcher: $fullLauncher"
if (Test-Path $fullLauncher) {
    Write-VerificationLog "Full launcher EXISTS" "SUCCESS"
    $fullSize = (Get-Item $fullLauncher).Length
    Write-VerificationLog "Full launcher size: $fullSize bytes" "INFO"
} else {
    Write-VerificationLog "Full launcher MISSING" "ERROR"
}

# List all files in ADDS25 directory
Write-VerificationLog "Listing all files in ADDS25 directory:" "INFO"
Add-Content $logFile "`n### ADDS25 Directory Contents:`n" -Encoding UTF8

if (Test-Path $adds25Path) {
    $files = Get-ChildItem $adds25Path -File | Select-Object Name, Length, LastWriteTime
    foreach ($file in $files) {
        $fileInfo = "- **$($file.Name)** ($($file.Length) bytes, modified: $($file.LastWriteTime))"
        Add-Content $logFile $fileInfo -Encoding UTF8
        Write-Host "  - $($file.Name) ($($file.Length) bytes)" -ForegroundColor Gray
    }
} else {
    Add-Content $logFile "- **ERROR**: ADDS25 directory not accessible" -Encoding UTF8
}

# Git status check
Write-VerificationLog "Checking Git repository status:" "INFO"
Add-Content $logFile "`n### Git Repository Status:`n" -Encoding UTF8

try {
    Set-Location $repoPath
    $gitStatus = git status --porcelain 2>&1
    $gitBranch = git branch --show-current 2>&1
    $gitLastCommit = git log -1 --oneline 2>&1
    
    Add-Content $logFile "- **Branch**: $gitBranch" -Encoding UTF8
    Add-Content $logFile "- **Last Commit**: $gitLastCommit" -Encoding UTF8
    Add-Content $logFile "- **Working Directory Status**: $(if($gitStatus){'Modified files detected'}else{'Clean'})" -Encoding UTF8
    
    Write-VerificationLog "Git branch: $gitBranch" "INFO"
    Write-VerificationLog "Last commit: $gitLastCommit" "INFO"
    Write-VerificationLog "Working directory: $(if($gitStatus){'Modified'}else{'Clean'})" "INFO"
    
} catch {
    $gitError = $_.Exception.Message
    Add-Content $logFile "- **ERROR**: $gitError" -Encoding UTF8
    Write-VerificationLog "Git error: $gitError" "ERROR"
}

# Final summary
Add-Content $logFile "`n---`n" -Encoding UTF8
Add-Content $logFile "**Verification Complete**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -Encoding UTF8
Add-Content $logFile "**Log File**: $logFile" -Encoding UTF8

Write-Host ""
Write-Host "*** Verification Complete ***" -ForegroundColor Green
Write-Host "Log file created: $logFile" -ForegroundColor Cyan
Write-Host "This file will help diagnose why the CI system cannot find the launcher files." -ForegroundColor Yellow
