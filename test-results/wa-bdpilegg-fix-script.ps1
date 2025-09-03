# ADDS25 wa-bdpilegg Environment Fix Script
# Purpose: Fix all identified issues from test session
# Date: September 1, 2025

Write-Host "🔧 ADDS25 Environment Fix Script Starting..." -ForegroundColor Cyan
Write-Host "Target: wa-bdpilegg test environment" -ForegroundColor Yellow
Write-Host ""

$errors = @()
$fixes = @()

# Step 1: Fix Git Repository Initialization
Write-Host "Step 1: Fixing Git Repository..." -ForegroundColor Yellow

$alarmPath = "C:\Users\wa-bdpilegg\Downloads\ALARM"
Set-Location $alarmPath

if (!(Test-Path ".git")) {
    Write-Host "  Initializing git repository..." -ForegroundColor Cyan
    try {
        git init
        git remote add origin https://github.com/BraPil/ALARM.git
        git fetch origin main
        git reset --hard origin/main
        Write-Host "  ✅ Git repository initialized and synced" -ForegroundColor Green
        $fixes += "Git repository initialized successfully"
    } catch {
        $errors += "Git initialization failed: $($_.Exception.Message)"
        Write-Host "  ❌ Git initialization failed" -ForegroundColor Red
    }
} else {
    Write-Host "  ✅ Git repository already exists" -ForegroundColor Green
}

# Step 2: Fix Username Mappings
Write-Host ""
Write-Host "Step 2: Fixing Username Mappings..." -ForegroundColor Yellow

# Fix ADDS25-AppSetup.ps1
$appSetupFile = "tests\ADDS25\v0.1\ADDS25-AppSetup.ps1"
if (Test-Path $appSetupFile) {
    try {
        $content = Get-Content $appSetupFile -Raw
        $originalContent = $content
        $content = $content -replace "C:\\Users\\kidsg\\", "C:\Users\wa-bdpilegg\"
        
        if ($content -ne $originalContent) {
            Set-Content $appSetupFile $content -Encoding UTF8
            Write-Host "  ✅ Fixed username mapping in ADDS25-AppSetup.ps1" -ForegroundColor Green
            $fixes += "Fixed username mapping in ADDS25-AppSetup.ps1"
        } else {
            Write-Host "  ✅ Username mapping already correct in ADDS25-AppSetup.ps1" -ForegroundColor Green
        }
    } catch {
        $errors += "Could not fix ADDS25-AppSetup.ps1: $($_.Exception.Message)"
        Write-Host "  ❌ Could not fix ADDS25-AppSetup.ps1" -ForegroundColor Red
    }
} else {
    $errors += "ADDS25-AppSetup.ps1 not found"
    Write-Host "  ❌ ADDS25-AppSetup.ps1 not found" -ForegroundColor Red
}

# Fix ADDS25.AutoCAD.csproj
$autocadProjFile = "tests\ADDS25\v0.1\ADDS25.AutoCAD\ADDS25.AutoCAD.csproj"
if (Test-Path $autocadProjFile) {
    try {
        $content = Get-Content $autocadProjFile -Raw
        $originalContent = $content
        $content = $content -replace "C:\\Users\\kidsg\\", "C:\Users\wa-bdpilegg\"
        
        if ($content -ne $originalContent) {
            Set-Content $autocadProjFile $content -Encoding UTF8
            Write-Host "  ✅ Fixed username mapping in ADDS25.AutoCAD.csproj" -ForegroundColor Green
            $fixes += "Fixed username mapping in ADDS25.AutoCAD.csproj"
        } else {
            Write-Host "  ✅ Username mapping already correct in ADDS25.AutoCAD.csproj" -ForegroundColor Green
        }
    } catch {
        $errors += "Could not fix ADDS25.AutoCAD.csproj: $($_.Exception.Message)"
        Write-Host "  ❌ Could not fix ADDS25.AutoCAD.csproj" -ForegroundColor Red
    }
} else {
    $errors += "ADDS25.AutoCAD.csproj not found"
    Write-Host "  ❌ ADDS25.AutoCAD.csproj not found" -ForegroundColor Red
}

# Step 3: Verify Deployment Files
Write-Host ""
Write-Host "Step 3: Verifying Deployment Files..." -ForegroundColor Yellow

$deploymentPath = "tests\ADDS25\v0.1"
$criticalFiles = @(
    "ADDS25-Launcher.bat",
    "ADDS25-DirSetup.ps1",
    "ADDS25-AppSetup.ps1",
    "ADDS25.sln",
    "ADDS25.AutoCAD\ADDS25.AutoCAD.csproj",
    "ADDS25.Core\ADDS25.Core.csproj",
    "ADDS25.Oracle\ADDS25.Oracle.csproj",
    "Config\adds25_default_config.json"
)

$missingFiles = @()
foreach ($file in $criticalFiles) {
    $filePath = Join-Path $deploymentPath $file
    if (Test-Path $filePath) {
        Write-Host "  ✅ Found: $file" -ForegroundColor Green
    } else {
        Write-Host "  ❌ Missing: $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

if ($missingFiles.Count -eq 0) {
    $fixes += "All critical deployment files present"
} else {
    $errors += "Missing deployment files: $($missingFiles -join ', ')"
}

# Step 4: Test Build System
Write-Host ""
Write-Host "Step 4: Testing Build System..." -ForegroundColor Yellow

Set-Location $deploymentPath
try {
    $buildResult = dotnet build ADDS25.sln --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ✅ Solution builds successfully" -ForegroundColor Green
        $fixes += "ADDS25 solution builds without errors"
    } else {
        Write-Host "  ❌ Build failed" -ForegroundColor Red
        Write-Host "  Build output: $buildResult" -ForegroundColor Yellow
        $errors += "Build failed: $buildResult"
    }
} catch {
    $errors += "Build test failed: $($_.Exception.Message)"
    Write-Host "  ❌ Build test failed" -ForegroundColor Red
}

# Step 5: Final Verification
Write-Host ""
Write-Host "Step 5: Final Environment Verification..." -ForegroundColor Yellow

Set-Location $alarmPath

# Verify git status
try {
    $gitStatus = git status --porcelain 2>$null
    Write-Host "  ✅ Git repository functional" -ForegroundColor Green
    if ($gitStatus) {
        Write-Host "  ⚠️ Repository has uncommitted changes" -ForegroundColor Yellow
    }
} catch {
    $errors += "Git repository not functional"
    Write-Host "  ❌ Git repository not functional" -ForegroundColor Red
}

# Verify username mappings one final time
$deploymentFiles = @(
    "$deploymentPath\ADDS25-DirSetup.ps1",
    "$deploymentPath\ADDS25-AppSetup.ps1",
    "$deploymentPath\ADDS25.AutoCAD\ADDS25.AutoCAD.csproj"
)

$mappingErrors = 0
foreach ($file in $deploymentFiles) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        if ($content -match "\\kidsg\\") {
            Write-Host "  ❌ $($file | Split-Path -Leaf): Still contains 'kidsg' reference" -ForegroundColor Red
            $mappingErrors++
        }
    }
}

if ($mappingErrors -eq 0) {
    Write-Host "  ✅ All username mappings correct" -ForegroundColor Green
    $fixes += "All username mappings verified correct"
} else {
    $errors += "$mappingErrors files still have incorrect username mappings"
}

# Final Report
Write-Host ""
Write-Host "📊 ENVIRONMENT FIX COMPLETION REPORT" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

if ($errors.Count -eq 0) {
    Write-Host "🎉 ALL FIXES SUCCESSFUL!" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Environment Status: READY FOR DEPLOYMENT TESTING" -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 NEXT STEPS:" -ForegroundColor Yellow
    Write-Host "1. Run ADDS25 Launcher:" -ForegroundColor White
    Write-Host "   cd tests\ADDS25\v0.1" -ForegroundColor Cyan
    Write-Host "   .\ADDS25-Launcher.bat" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "2. Monitor results and capture logs for analysis" -ForegroundColor White
} else {
    Write-Host "⚠️ FIXES COMPLETED WITH SOME ISSUES" -ForegroundColor Yellow
}

if ($fixes.Count -gt 0) {
    Write-Host ""
    Write-Host "Successful Fixes:" -ForegroundColor Green
    $fixes | ForEach-Object { Write-Host "  ✅ $_" -ForegroundColor Green }
}

if ($errors.Count -gt 0) {
    Write-Host ""
    Write-Host "Remaining Issues:" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host "  ❌ $_" -ForegroundColor Red }
}

Write-Host ""
Write-Host "Current Location: $(Get-Location)" -ForegroundColor Cyan
Write-Host "Fix Script Complete" -ForegroundColor Green
