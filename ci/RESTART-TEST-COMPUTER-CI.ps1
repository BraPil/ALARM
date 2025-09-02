# ADDS25 Test Computer CI Restart Script
# Purpose: Restart test computer CI monitoring after system freeze/crash
# Environment: Test Computer (wa-bdpilegg)
# Date: September 2, 2025

Write-Host "🔄 ADDS25 Test Computer CI Restart" -ForegroundColor Cyan
Write-Host "Purpose: Restart automated CI monitoring system" -ForegroundColor Yellow
Write-Host ""

# Configuration
$repoPath = "C:\Users\wa-bdpilegg\Downloads\ALARM"
$testResultsPath = "$repoPath\test-results"
$ciLogPath = "$repoPath\ci\logs"

Write-Host "Repository Path: $repoPath" -ForegroundColor Green
Write-Host "Test Results: $testResultsPath" -ForegroundColor Green
Write-Host "CI Logs: $ciLogPath" -ForegroundColor Green
Write-Host ""

# Step 1: Ensure we're in the correct directory
Write-Host "Step 1: Navigating to repository..." -ForegroundColor Yellow
if (!(Test-Path $repoPath)) {
    Write-Host "❌ Repository not found: $repoPath" -ForegroundColor Red
    Write-Host "Please ensure the ALARM repository exists at this location" -ForegroundColor Red
    exit 1
}

Set-Location $repoPath
Write-Host "✅ Current directory: $(Get-Location)" -ForegroundColor Green

# Step 2: Pull latest changes (including automated fixes)
Write-Host ""
Write-Host "Step 2: Pulling latest automated fixes from GitHub..." -ForegroundColor Yellow
try {
    git fetch origin
    git pull origin main
    Write-Host "✅ Successfully pulled latest changes" -ForegroundColor Green
    
    # Show latest commits
    Write-Host ""
    Write-Host "Latest commits:" -ForegroundColor Cyan
    git log --oneline -3
    
} catch {
    Write-Host "❌ Error pulling changes: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Continuing with local version..." -ForegroundColor Yellow
}

# Step 3: Create necessary directories
Write-Host ""
Write-Host "Step 3: Ensuring directory structure..." -ForegroundColor Yellow
@($testResultsPath, $ciLogPath) | ForEach-Object {
    if (!(Test-Path $_)) { 
        New-Item $_ -Type Directory -Force | Out-Null
        Write-Host "✅ Created directory: $_" -ForegroundColor Green
    } else {
        Write-Host "✅ Directory exists: $_" -ForegroundColor Green
    }
}

# Step 4: Check ADDS25 build status
Write-Host ""
Write-Host "Step 4: Verifying ADDS25 build readiness..." -ForegroundColor Yellow
$adds25Path = "$repoPath\tests\ADDS25\v0.1"
if (Test-Path $adds25Path) {
    Write-Host "✅ ADDS25 directory found: $adds25Path" -ForegroundColor Green
    
    # Check for launcher files
    $launchers = @(
        "$adds25Path\ADDS25-Launcher.bat",
        "$adds25Path\ADDS25-Launcher-Simple.bat"
    )
    
    foreach ($launcher in $launchers) {
        if (Test-Path $launcher) {
            Write-Host "✅ Launcher found: $(Split-Path $launcher -Leaf)" -ForegroundColor Green
        } else {
            Write-Host "⚠️ Launcher missing: $(Split-Path $launcher -Leaf)" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "❌ ADDS25 directory not found: $adds25Path" -ForegroundColor Red
}

# Step 5: Start the automated CI system
Write-Host ""
Write-Host "Step 5: Starting ADDS25 Test Computer Automated CI..." -ForegroundColor Yellow
Write-Host ""
Write-Host "🎯 AUTOMATED CI SYSTEM FEATURES:" -ForegroundColor Green
Write-Host "  • Monitor GitHub for dev computer fixes" -ForegroundColor White
Write-Host "  • Auto-pull latest changes" -ForegroundColor White
Write-Host "  • Build and test ADDS25" -ForegroundColor White
Write-Host "  • Execute launcher with enhanced diagnostics" -ForegroundColor White
Write-Host "  • Generate comprehensive test reports" -ForegroundColor White
Write-Host "  • Push results back to GitHub" -ForegroundColor White
Write-Host "  • Trigger next CI cycle" -ForegroundColor White
Write-Host ""
Write-Host "🔧 RECENT AUTOMATED FIXES APPLIED:" -ForegroundColor Cyan
Write-Host "  • Fixed AutoCAD DLL paths to actual installation" -ForegroundColor White
Write-Host "  • Resolved WindowsBase version conflicts" -ForegroundColor White
Write-Host "  • Enhanced launcher timeout (10 seconds)" -ForegroundColor White
Write-Host "  • Added comprehensive launcher diagnostics" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the CI system" -ForegroundColor Yellow
Write-Host ""
Write-Host "🚀 STARTING AUTOMATED CI MONITORING..." -ForegroundColor Green

# Execute the main test computer CI script
& "$repoPath\ci\TEST-COMPUTER-AUTOMATED-CI.ps1"
