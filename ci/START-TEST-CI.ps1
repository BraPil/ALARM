# ADDS25 Test Computer CI Startup Script
# Purpose: Initialize and start the automated CI system on test computer
# Environment: Test Computer (wa-bdpilegg)
# Date: September 1, 2025

Write-Host "ADDS25 Test Computer CI Startup" -ForegroundColor Cyan

# Ensure we're in the correct directory
$repoPath = "C:\Users\wa-bdpilegg\Downloads\ALARM"
if (!(Test-Path $repoPath)) {
    Write-Host "Repository not found: $repoPath" -ForegroundColor Red
    Write-Host "Please ensure ALARM repository is cloned to: $repoPath" -ForegroundColor Yellow
    exit 1
}

Set-Location $repoPath

# Pull latest changes
Write-Host "Pulling latest changes from GitHub..." -ForegroundColor Yellow
git pull origin main

# Create necessary directories
$ciPath = "$repoPath\ci"
$logsPath = "$ciPath\logs"
$testResultsPath = "$repoPath\test-results"

@($logsPath, $testResultsPath) | ForEach-Object {
    if (!(Test-Path $_)) {
        New-Item $_ -Type Directory -Force | Out-Null
        Write-Host "Created directory: $_" -ForegroundColor Green
    }
}

# Verify ADDS25 installation
$adds25Path = "$repoPath\tests\ADDS25\v0.1"
if (!(Test-Path $adds25Path)) {
    Write-Host "ADDS25 not found: $adds25Path" -ForegroundColor Red
    Write-Host "Please ensure ADDS25 is properly installed" -ForegroundColor Yellow
    exit 1
}

# Check AutoCAD installation
$autocadFound = $false
$autocadPaths = @(
    "C:\Program Files\Autodesk\AutoCAD 2025",
    "C:\Program Files\Autodesk\AutoCAD Map 3D 2025"
)

foreach ($path in $autocadPaths) {
    if (Test-Path $path) {
        Write-Host "AutoCAD found: $path" -ForegroundColor Green
        $autocadFound = $true
        break
    }
}

if (!$autocadFound) {
    Write-Host "AutoCAD not found in standard locations" -ForegroundColor Yellow
    Write-Host "Testing will continue but AutoCAD functionality may fail" -ForegroundColor Yellow
}

# Check .NET installation
try {
    $dotnetVersion = dotnet --version
    Write-Host ".NET Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host ".NET not found - please install .NET 8.0 SDK" -ForegroundColor Red
    exit 1
}

# Start the automated CI system
Write-Host ""
Write-Host "Starting ADDS25 Automated Testing System..." -ForegroundColor Green
Write-Host "This will monitor GitHub for fixes and automatically:" -ForegroundColor Yellow
Write-Host "  • Pull latest code changes" -ForegroundColor White
Write-Host "  • Run ADDS25-Launcher.bat with comprehensive logging" -ForegroundColor White
Write-Host "  • Collect test results and analysis" -ForegroundColor White
Write-Host "  • Push results back to GitHub for dev analysis" -ForegroundColor White
Write-Host "  • Wait for new fixes and repeat" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the testing system" -ForegroundColor Yellow
Write-Host ""

# Execute the main CI script
& "$ciPath\TEST-COMPUTER-AUTOMATED-CI.ps1"
