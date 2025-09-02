# ADDS25 Dev Computer CI Startup Script
# Purpose: Initialize and start the automated CI system on dev computer
# Environment: Dev Computer (kidsg)
# Date: September 1, 2025

Write-Host "🚀 ADDS25 Dev Computer CI Startup" -ForegroundColor Cyan

# Ensure we're in the correct directory
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
if (!(Test-Path $repoPath)) {
    Write-Host "❌ Repository not found: $repoPath" -ForegroundColor Red
    exit 1
}

Set-Location $repoPath

# Pull latest changes
Write-Host "📥 Pulling latest changes from GitHub..." -ForegroundColor Yellow
git pull origin main

# Create necessary directories
$ciPath = "$repoPath\ci"
$logsPath = "$ciPath\logs"

if (!(Test-Path $logsPath)) {
    New-Item $logsPath -Type Directory -Force | Out-Null
    Write-Host "✅ Created CI logs directory: $logsPath" -ForegroundColor Green
}

# Start the automated CI system
Write-Host ""
Write-Host "🎯 Starting ADDS25 Automated CI System..." -ForegroundColor Green
Write-Host "This will monitor GitHub for test results and automatically:" -ForegroundColor Yellow
Write-Host "  • Engage Master Protocol for analysis" -ForegroundColor White
Write-Host "  • Generate fixes based on test failures" -ForegroundColor White
Write-Host "  • Commit and push fixes back to GitHub" -ForegroundColor White
Write-Host "  • Wait for test computer to test and repeat" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the CI system" -ForegroundColor Yellow
Write-Host ""

# Execute the main CI script
& "$ciPath\DEV-COMPUTER-AUTOMATED-CI.ps1"
