# ADDS25 .NET 8.0 SDK Installation Script for wa-bdpilegg Test Computer
# Purpose: Install .NET 8.0 SDK and complete ADDS25 setup
# Target: wa-bdpilegg test computer
# Date: September 1, 2025

Write-Host "🚀 ADDS25 .NET 8.0 SDK Installation Starting..." -ForegroundColor Cyan
Write-Host "This will install .NET 8.0 SDK and complete ADDS25 setup" -ForegroundColor Yellow
Write-Host ""

# Initialize logging
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logDir = "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"
$installLog = "$logDir\net8-install-$timestamp.md"

if (!(Test-Path $logDir)) {
    New-Item $logDir -Type Directory -Force | Out-Null
}

# Start logging
@"
# .NET 8.0 SDK Installation Log

**Installation Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Purpose**: Install .NET 8.0 SDK for ADDS25 compatibility

---

## 🎯 **INSTALLATION LOG**

"@ | Out-File $installLog -Encoding UTF8

function Write-InstallLog {
    param([string]$Message, [string]$Level = "INFO")
    $logEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $logEntry
    Add-Content $installLog $logEntry -Encoding UTF8
}

Write-InstallLog "Starting .NET 8.0 SDK installation" "START"

# Step 1: Check current .NET version
Write-InstallLog "Step 1: Checking current .NET version..." "INFO"
$currentVersion = dotnet --version 2>$null
if ($currentVersion) {
    Write-InstallLog "Current .NET version: $currentVersion" "INFO"
    Write-Host "Current .NET version: $currentVersion" -ForegroundColor Yellow
} else {
    Write-InstallLog "No .NET SDK detected" "WARNING"
    Write-Host "No .NET SDK detected" -ForegroundColor Yellow
}

# Step 2: Install .NET 8.0 SDK using winget
Write-InstallLog "Step 2: Installing .NET 8.0 SDK via winget..." "INFO"
Write-Host ""
Write-Host "Installing .NET 8.0 SDK..." -ForegroundColor Yellow

try {
    $installResult = winget install Microsoft.DotNet.SDK.8 --accept-package-agreements --accept-source-agreements 2>&1
    Write-InstallLog "Winget install completed" "SUCCESS"
    Write-Host "✅ .NET 8.0 SDK installation completed" -ForegroundColor Green
} catch {
    Write-InstallLog "Winget install failed: $($_.Exception.Message)" "ERROR"
    Write-Host "❌ Winget install failed" -ForegroundColor Red
    
    # Fallback: Manual download instructions
    Write-InstallLog "Providing manual download instructions" "INFO"
    Write-Host ""
    Write-Host "⚠️ MANUAL INSTALLATION REQUIRED:" -ForegroundColor Yellow
    Write-Host "1. Go to: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
    Write-Host "2. Download 'SDK 8.0.x' for Windows x64" -ForegroundColor White
    Write-Host "3. Run the installer" -ForegroundColor White
    Write-Host "4. Restart PowerShell" -ForegroundColor White
    Write-Host "5. Run this script again" -ForegroundColor White
    Write-Host ""
    Write-Host "Press any key to continue with current setup..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# Step 3: Verify .NET 8.0 installation
Write-InstallLog "Step 3: Verifying .NET 8.0 installation..." "INFO"
Write-Host ""
Write-Host "Verifying .NET 8.0 installation..." -ForegroundColor Yellow

# Refresh environment variables
$env:PATH = [System.Environment]::GetEnvironmentVariable("PATH", "Machine") + ";" + [System.Environment]::GetEnvironmentVariable("PATH", "User")

$newVersion = dotnet --version 2>$null
if ($newVersion -and $newVersion.StartsWith("8.")) {
    Write-InstallLog ".NET 8.0 successfully installed: $newVersion" "SUCCESS"
    Write-Host "✅ .NET 8.0 successfully installed: $newVersion" -ForegroundColor Green
    $net8Available = $true
} else {
    Write-InstallLog ".NET 8.0 not detected. Current version: $newVersion" "WARNING"
    Write-Host "⚠️ .NET 8.0 not detected. Current version: $newVersion" -ForegroundColor Yellow
    $net8Available = $false
}

# Step 4: Navigate to ADDS25 and apply username mappings
Write-InstallLog "Step 4: Applying username mappings..." "INFO"
Write-Host ""
Write-Host "Applying username mappings..." -ForegroundColor Yellow

Set-Location "C:\Users\wa-bdpilegg\Downloads\ALARM\tests\ADDS25\v0.1"

# Fix ADDS25-AppSetup.ps1
$appSetupFile = "ADDS25-AppSetup.ps1"
if (Test-Path $appSetupFile) {
    $content = Get-Content $appSetupFile -Raw
    $originalContent = $content
    $content = $content -replace "C:\\Users\\kidsg\\", "C:\Users\wa-bdpilegg\"
    
    if ($content -ne $originalContent) {
        Set-Content $appSetupFile $content -Encoding UTF8
        Write-InstallLog "Fixed username mapping in ADDS25-AppSetup.ps1" "SUCCESS"
        Write-Host "✅ Fixed ADDS25-AppSetup.ps1" -ForegroundColor Green
    } else {
        Write-InstallLog "Username mapping already correct in ADDS25-AppSetup.ps1" "INFO"
        Write-Host "✅ ADDS25-AppSetup.ps1 already correct" -ForegroundColor Green
    }
}

# Fix ADDS25.AutoCAD.csproj
$autocadProjFile = "ADDS25.AutoCAD\ADDS25.AutoCAD.csproj"
if (Test-Path $autocadProjFile) {
    $content = Get-Content $autocadProjFile -Raw
    $originalContent = $content
    $content = $content -replace "C:\\Users\\kidsg\\", "C:\Users\wa-bdpilegg\"
    
    if ($content -ne $originalContent) {
        Set-Content $autocadProjFile $content -Encoding UTF8
        Write-InstallLog "Fixed username mapping in ADDS25.AutoCAD.csproj" "SUCCESS"
        Write-Host "✅ Fixed ADDS25.AutoCAD.csproj" -ForegroundColor Green
    } else {
        Write-InstallLog "Username mapping already correct in ADDS25.AutoCAD.csproj" "INFO"
        Write-Host "✅ ADDS25.AutoCAD.csproj already correct" -ForegroundColor Green
    }
}

# Step 5: Test build system
Write-InstallLog "Step 5: Testing build system..." "INFO"
Write-Host ""
Write-Host "Testing build system..." -ForegroundColor Yellow

try {
    $buildOutput = dotnet build ADDS25.sln --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-InstallLog "Build successful with .NET $newVersion" "SUCCESS"
        Write-Host "✅ Build successful!" -ForegroundColor Green
        $buildSuccess = $true
    } else {
        Write-InstallLog "Build failed: $buildOutput" "ERROR"
        Write-Host "❌ Build failed" -ForegroundColor Red
        Write-Host "Build output: $buildOutput" -ForegroundColor Yellow
        $buildSuccess = $false
    }
} catch {
    Write-InstallLog "Build test failed: $($_.Exception.Message)" "ERROR"
    Write-Host "❌ Build test failed" -ForegroundColor Red
    $buildSuccess = $false
}

# Step 6: Create automated launcher wrapper
Write-InstallLog "Step 6: Creating automated launcher wrapper..." "INFO"
Write-Host ""
Write-Host "Creating automated launcher wrapper..." -ForegroundColor Yellow

$launcherWrapper = @"
# ADDS25 Launcher with Automated Logging
`$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
`$logFile = "$logDir\launcher-execution-`$timestamp.md"

# Initialize launcher log
@"
# ADDS25 Launcher Execution Log

**Execution Time**: `$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Launcher**: ADDS25-Launcher.bat
**.NET Version**: `$(dotnet --version)

---

## 🚀 **LAUNCHER OUTPUT**

"@ | Out-File `$logFile -Encoding UTF8

Write-Host "🚀 Starting ADDS25 Launcher with automated logging..." -ForegroundColor Cyan
Write-Host "Logging to: `$logFile" -ForegroundColor Yellow

# Execute launcher and capture output
Start-Transcript -Path "`$logFile.transcript" -Append
.\ADDS25-Launcher.bat
Stop-Transcript

Write-Host ""
Write-Host "✅ Launcher execution complete" -ForegroundColor Green
Write-Host "📝 Results logged to: `$logFile" -ForegroundColor Cyan
Write-Host "📝 Transcript saved to: `$logFile.transcript" -ForegroundColor Cyan
"@

$launcherWrapperPath = "ADDS25-Launcher-Logged.ps1"
Set-Content $launcherWrapperPath $launcherWrapper -Encoding UTF8
Write-InstallLog "Created automated launcher wrapper: $launcherWrapperPath" "SUCCESS"
Write-Host "✅ Created automated launcher wrapper" -ForegroundColor Green

# Final status report
Write-InstallLog "Step 7: Final status report..." "INFO"
Write-Host ""
Write-Host "📊 INSTALLATION COMPLETION REPORT" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

Write-Host "✅ .NET Version: $(if ($net8Available) { 'NET 8.0 Available' } else { 'NET 8.0 Not Available' })" -ForegroundColor $(if ($net8Available) { 'Green' } else { 'Red' })
Write-Host "✅ Username Mappings: Applied" -ForegroundColor Green
Write-Host "✅ Build System: $(if ($buildSuccess) { 'Working' } else { 'Failed' })" -ForegroundColor $(if ($buildSuccess) { 'Green' } else { 'Red' })
Write-Host "✅ Automated Logging: Ready" -ForegroundColor Green

Write-Host ""
if ($net8Available -and $buildSuccess) {
    Write-Host "🎉 ENVIRONMENT READY FOR TESTING!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 NEXT STEPS:" -ForegroundColor Yellow
    Write-Host "1. Run automated launcher:" -ForegroundColor White
    Write-Host "   .\ADDS25-Launcher-Logged.ps1" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "2. All results will be logged to:" -ForegroundColor White
    Write-Host "   $logDir" -ForegroundColor Cyan
} else {
    Write-Host "⚠️ MANUAL STEPS REQUIRED:" -ForegroundColor Yellow
    if (!$net8Available) {
        Write-Host "• Install .NET 8.0 SDK manually" -ForegroundColor White
    }
    if (!$buildSuccess) {
        Write-Host "• Resolve build issues" -ForegroundColor White
    }
    Write-Host "• Re-run this script after fixes" -ForegroundColor White
}

Write-InstallLog "Installation and setup process complete" "END"
Write-InstallLog "Environment ready status: $(if ($net8Available -and $buildSuccess) { 'READY' } else { 'NEEDS_ATTENTION' })" "FINAL"
