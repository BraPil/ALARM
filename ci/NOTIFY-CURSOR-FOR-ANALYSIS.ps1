# ADDS25 Cursor Notification Script
# Purpose: Notify Cursor when new test results are detected for immediate analysis
# Environment: Dev Computer (kidsg)
# Date: September 2, 2025

param(
    [string]$CommitHash = "",
    [string]$CommitMessage = "",
    [switch]$TimeZoneAdjusted = $false
)

Write-Host "*** ADDS25 Cursor Notification System ***" -ForegroundColor Cyan
Write-Host "Purpose: Trigger immediate Cursor analysis of test results" -ForegroundColor Yellow
Write-Host ""

# Configuration
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
$notificationFile = "$repoPath\ci\CURSOR-ANALYSIS-REQUEST.txt"

# Get current timestamp
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

# Create notification content
$notificationContent = @"
CURSOR ANALYSIS REQUEST
=======================

Timestamp: $timestamp
Commit Hash: $CommitHash
Commit Message: $CommitMessage
Time Zone Adjusted: $TimeZoneAdjusted

ACTION REQUIRED:
1. Pull latest changes: git pull origin main
2. Search for test results with time zone consideration:
   - Dev computer time (ET): Current timestamp
   - Test computer time (CT): 1 hour behind
3. Analyze latest test results immediately
4. Generate fixes if needed
5. Deploy fixes to continue CI cycle

ANALYSIS CHECKLIST:
[ ] git pull origin main executed
[ ] Latest test results identified (consider CT time zone)
[ ] Test results analyzed with Master Protocol
[ ] Issues identified and documented
[ ] Fixes generated and deployed (if needed)
[ ] Next CI cycle monitoring activated

DELETE THIS FILE AFTER ANALYSIS COMPLETE
"@

# Write notification file
$notificationContent | Out-File $notificationFile -Encoding UTF8

Write-Host "✅ Cursor notification created: $notificationFile" -ForegroundColor Green
Write-Host ""
Write-Host "INSTRUCTIONS FOR USER:" -ForegroundColor Yellow
Write-Host "1. Copy this text and send to Cursor:" -ForegroundColor White
Write-Host "   'New test results detected - analyze now with time zone adjustment'" -ForegroundColor Cyan
Write-Host "2. Cursor will pull latest changes and analyze results" -ForegroundColor White
Write-Host "3. Automated fixes will be generated if needed" -ForegroundColor White
Write-Host ""

# Also display the key information for immediate copy-paste
Write-Host "COPY-PASTE MESSAGE FOR CURSOR:" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Yellow
Write-Host "New test results detected - analyze now with time zone adjustment" -ForegroundColor Cyan
Write-Host "Commit: $CommitHash" -ForegroundColor White
Write-Host "Time: $timestamp (ET)" -ForegroundColor White
Write-Host "Note: Test computer is 1 hour behind (CT)" -ForegroundColor White
Write-Host "===============================================" -ForegroundColor Yellow
