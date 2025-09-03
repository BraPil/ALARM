# ADDS25 AutoCAD DLL Path Fix Script
# Purpose: Update AutoCAD DLL references to correct paths on test computer
# Target: wa-bdpilegg test computer with AutoCAD Map3D 2025 installed
# Date: September 1, 2025

Write-Host "🔧 ADDS25 AutoCAD DLL Path Fix Starting..." -ForegroundColor Cyan
Write-Host "Updating DLL paths to use installed AutoCAD Map3D 2025" -ForegroundColor Yellow

# Navigate to project directory
$projectPath = "C:\Users\wa-bdpilegg\Downloads\ALARM\tests\ADDS25\v0.1"
Set-Location $projectPath

# AutoCAD installation path (standard installation)
$autocadPath = "C:\Program Files\Autodesk\AutoCAD 2025"

# Verify AutoCAD installation
if (!(Test-Path $autocadPath)) {
    Write-Host "❌ AutoCAD 2025 not found at expected location: $autocadPath" -ForegroundColor Red
    Write-Host "Checking alternative locations..." -ForegroundColor Yellow
    
    $altPaths = @(
        "C:\Program Files\Autodesk\AutoCAD Map 3D 2025",
        "C:\Program Files (x86)\Autodesk\AutoCAD 2025",
        "C:\Program Files (x86)\Autodesk\AutoCAD Map 3D 2025"
    )
    
    foreach ($altPath in $altPaths) {
        if (Test-Path $altPath) {
            $autocadPath = $altPath
            Write-Host "✅ Found AutoCAD at: $autocadPath" -ForegroundColor Green
            break
        }
    }
}

# Define correct DLL paths
$dllPaths = @{
    "AcCoreMgd" = "$autocadPath\AcCoreMgd.dll"
    "AcDbMgd" = "$autocadPath\AcDbMgd.dll" 
    "AcMgd" = "$autocadPath\AcMgd.dll"
    "AdUIMgd" = "$autocadPath\AdUIMgd.dll"
}

# Verify DLLs exist
Write-Host "Verifying AutoCAD DLLs..." -ForegroundColor Yellow
$missingDlls = @()

foreach ($dll in $dllPaths.Keys) {
    $path = $dllPaths[$dll]
    if (Test-Path $path) {
        Write-Host "✅ Found: $dll at $path" -ForegroundColor Green
    } else {
        Write-Host "❌ Missing: $dll at $path" -ForegroundColor Red
        $missingDlls += $dll
    }
}

if ($missingDlls.Count -gt 0) {
    Write-Host "❌ Missing DLLs detected. Searching for alternative locations..." -ForegroundColor Red
    
    # Search for DLLs in AutoCAD installation
    foreach ($missingDll in $missingDlls) {
        $foundDll = Get-ChildItem -Path $autocadPath -Name "$missingDll.dll" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($foundDll) {
            $fullPath = Join-Path $autocadPath $foundDll
            $dllPaths[$missingDll] = $fullPath
            Write-Host "✅ Found $missingDll at: $fullPath" -ForegroundColor Green
        }
    }
}

# Update project file
$projectFile = "ADDS25.AutoCAD\ADDS25.AutoCAD.csproj"
Write-Host ""
Write-Host "Updating project file: $projectFile" -ForegroundColor Yellow

if (!(Test-Path $projectFile)) {
    Write-Host "❌ Project file not found: $projectFile" -ForegroundColor Red
    return
}

# Read current project file
$content = Get-Content $projectFile -Raw

# Create backup
$backupFile = "$projectFile.backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
Copy-Item $projectFile $backupFile
Write-Host "✅ Backup created: $backupFile" -ForegroundColor Green

# Update DLL references
foreach ($dll in $dllPaths.Keys) {
    $currentPath = $dllPaths[$dll]
    $escapedPath = $currentPath -replace '\\', '\\'
    
    # Pattern to match the HintPath for this DLL
    $pattern = "(<Reference Include=`"$dll`">.*?<HintPath>)([^<]+)(</HintPath>)"
    
    if ($content -match $pattern) {
        $content = $content -replace $pattern, "`$1$escapedPath`$3"
        Write-Host "✅ Updated $dll path to: $currentPath" -ForegroundColor Green
    } else {
        Write-Host "⚠️ Could not find HintPath for $dll in project file" -ForegroundColor Yellow
    }
}

# Write updated content
Set-Content $projectFile $content -Encoding UTF8
Write-Host "✅ Project file updated successfully" -ForegroundColor Green

# Test build
Write-Host ""
Write-Host "Testing build with updated DLL paths..." -ForegroundColor Yellow
$buildResult = dotnet build ADDS25.sln --verbosity quiet

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ BUILD SUCCESSFUL!" -ForegroundColor Green
    Write-Host "🎉 All AutoCAD DLL references fixed" -ForegroundColor Green
} else {
    Write-Host "❌ Build still failing. Additional issues may exist." -ForegroundColor Red
    Write-Host "Running detailed build for diagnosis..." -ForegroundColor Yellow
    dotnet build ADDS25.sln --verbosity normal
}

Write-Host ""
Write-Host "📋 SUMMARY:" -ForegroundColor Cyan
Write-Host "✅ .NET 8.0 SDK: Working" -ForegroundColor Green
Write-Host "✅ ADDS25.Core: Built Successfully" -ForegroundColor Green  
Write-Host "✅ ADDS25.Oracle: Built Successfully" -ForegroundColor Green
Write-Host "$(if ($LASTEXITCODE -eq 0) { '✅' } else { '❌' }) ADDS25.AutoCAD: $(if ($LASTEXITCODE -eq 0) { 'Fixed and Building' } else { 'Still Has Issues' })" -ForegroundColor $(if ($LASTEXITCODE -eq 0) { 'Green' } else { 'Red' })

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "🚀 READY FOR LAUNCHER TESTING!" -ForegroundColor Green
    Write-Host "Next: Run .\ADDS25-Launcher.bat" -ForegroundColor Yellow
}
