# ADDS25 Test Environment Update Script
# Purpose: Pull GitHub updates and prepare test environment for wa-bdpilegg
# Target: Test computer with username wa-bdpilegg
# Date: September 1, 2025

Write-Host "🚀 ADDS25 Test Environment Update Starting..." -ForegroundColor Cyan
Write-Host "Target Environment: wa-bdpilegg" -ForegroundColor Yellow
Write-Host ""

# Step 1A: Navigate to ALARM directory on test computer
Write-Host "Step 1A: Navigating to ALARM directory..." -ForegroundColor Yellow
$alarmPath = "C:\Users\wa-bdpilegg\Downloads\ALARM"

if (!(Test-Path $alarmPath)) {
    Write-Host "Creating ALARM directory structure..." -ForegroundColor Yellow
    New-Item $alarmPath -Type Directory -Force | Out-Null
    Write-Host "✅ Created: $alarmPath" -ForegroundColor Green
}

Set-Location $alarmPath
Write-Host "✅ Current directory: $(Get-Location)" -ForegroundColor Green

# Step 1B: Clone or pull latest from GitHub
Write-Host ""
Write-Host "Step 1B: Updating from GitHub repository..." -ForegroundColor Yellow

if (Test-Path ".git") {
    Write-Host "Repository exists - pulling latest changes..." -ForegroundColor Cyan
    
    # Fetch latest changes
    git fetch origin main
    Write-Host "✅ Fetched latest changes from origin/main" -ForegroundColor Green
    
    # Pull and merge
    git pull origin main
    Write-Host "✅ Pulled and merged latest changes" -ForegroundColor Green
    
} else {
    Write-Host "Repository not found - cloning from GitHub..." -ForegroundColor Cyan
    
    # Clone the repository
    git clone https://github.com/BraPil/ALARM.git .
    Write-Host "✅ Cloned ALARM repository" -ForegroundColor Green
}

# Step 1C: Verify critical deployment files are present
Write-Host ""
Write-Host "Step 1C: Verifying deployment files..." -ForegroundColor Yellow

$deploymentPath = "$alarmPath\tests\ADDS25\v0.1"
$criticalFiles = @(
    "ADDS25-Launcher.bat",
    "ADDS25-DirSetup.ps1", 
    "ADDS25-AppSetup.ps1",
    "ADDS25.sln",
    "ADDS25.AutoCAD\ADDS25.AutoCAD.csproj",
    "Config\adds25_default_config.json"
)

$missingFiles = @()
foreach ($file in $criticalFiles) {
    $filePath = Join-Path $deploymentPath $file
    if (Test-Path $filePath) {
        Write-Host "✅ Found: $file" -ForegroundColor Green
    } else {
        Write-Host "❌ Missing: $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

# Step 1D: Verify monitoring tools are present
Write-Host ""
Write-Host "Step 1D: Verifying monitoring tools..." -ForegroundColor Yellow

$testResultsPath = "$alarmPath\test-results"
$monitoringFiles = @(
    "ADDS25-Deployment-Monitor.ps1",
    "PowerShell-Results-Log.md",
    "AutoCAD-Message-Bar-History-Log.md",
    "ADDS25-Error-Analysis-Report.md"
)

foreach ($file in $monitoringFiles) {
    $filePath = Join-Path $testResultsPath $file
    if (Test-Path $filePath) {
        Write-Host "✅ Found: $file" -ForegroundColor Green
    } else {
        Write-Host "❌ Missing: $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

# Step 1E: Verify username mappings in deployment files
Write-Host ""
Write-Host "Step 1E: Verifying username mappings..." -ForegroundColor Yellow

$deploymentFiles = @(
    "$deploymentPath\ADDS25-DirSetup.ps1",
    "$deploymentPath\ADDS25-AppSetup.ps1", 
    "$deploymentPath\ADDS25.AutoCAD\ADDS25.AutoCAD.csproj"
)

$mappingErrors = @()
foreach ($file in $deploymentFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        if ($content -match "\\kidsg\\") {
            Write-Host "❌ $($file | Split-Path -Leaf): Contains development username 'kidsg'" -ForegroundColor Red
            $mappingErrors += $file
        } else {
            Write-Host "✅ $($file | Split-Path -Leaf): Username mapping correct" -ForegroundColor Green
        }
    }
}

# Step 1F: Create test results directory structure
Write-Host ""
Write-Host "Step 1F: Ensuring test results directory structure..." -ForegroundColor Yellow

$testDirs = @(
    "$alarmPath\test-results",
    "$alarmPath\mcp_runs",
    "$alarmPath\mcp\documentation\protocols"
)

foreach ($dir in $testDirs) {
    if (!(Test-Path $dir)) {
        New-Item $dir -Type Directory -Force | Out-Null
        Write-Host "✅ Created: $dir" -ForegroundColor Green
    } else {
        Write-Host "✅ Exists: $dir" -ForegroundColor Green
    }
}

# Final Status Report
Write-Host ""
Write-Host "📊 STEP 1 COMPLETION REPORT" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

if ($missingFiles.Count -eq 0 -and $mappingErrors.Count -eq 0) {
    Write-Host "🎉 SUCCESS: Test environment ready for deployment!" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Repository updated from GitHub" -ForegroundColor Green
    Write-Host "✅ All critical deployment files present" -ForegroundColor Green  
    Write-Host "✅ All monitoring tools available" -ForegroundColor Green
    Write-Host "✅ Username mappings verified (wa-bdpilegg)" -ForegroundColor Green
    Write-Host "✅ Directory structure complete" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "🚀 READY FOR NEXT STEPS:" -ForegroundColor Yellow
    Write-Host "1. Build solution: cd $deploymentPath && dotnet build ADDS25.sln" -ForegroundColor White
    Write-Host "2. Run monitor: cd $testResultsPath && .\ADDS25-Deployment-Monitor.ps1 -Environment Test" -ForegroundColor White
    Write-Host "3. Or run launcher: cd $deploymentPath && .\ADDS25-Launcher.bat" -ForegroundColor White
    
} else {
    Write-Host "❌ ISSUES FOUND - Test environment not ready" -ForegroundColor Red
    
    if ($missingFiles.Count -gt 0) {
        Write-Host ""
        Write-Host "Missing Files:" -ForegroundColor Red
        $missingFiles | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    }
    
    if ($mappingErrors.Count -gt 0) {
        Write-Host ""
        Write-Host "Username Mapping Errors:" -ForegroundColor Red
        $mappingErrors | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    }
    
    Write-Host ""
    Write-Host "Please resolve these issues before proceeding with testing." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Current Location: $(Get-Location)" -ForegroundColor Cyan
Write-Host "Test Environment Update Complete" -ForegroundColor Cyan
