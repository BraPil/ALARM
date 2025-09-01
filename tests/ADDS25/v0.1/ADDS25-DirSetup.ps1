# ADDS25 Directory Setup Script
# Original: ADDS19DirSetup.ps1
# Migration: Modernized directory structure for ADDS25
# Purpose: Creates essential directory structure with proper permissions
# Date: September 1, 2025

Write-Host "*** ADDS25 Directory Setup Started ***" -ForegroundColor Green
Write-Host "Target Framework: .NET Core 8" -ForegroundColor Cyan
Write-Host "AutoCAD Version: Map3D 2025" -ForegroundColor Cyan
Write-Host "Oracle Version: 19c" -ForegroundColor Cyan
Write-Host ""

# ADDS25: Create main application directories
Write-Host "Creating ADDS25 application directories..." -ForegroundColor Yellow

# ADDS25: Main application directory (updated path)
if(!(Test-Path -path C:\ADDS25)) { 
    New-Item C:\ADDS25 -type directory 
    Write-Host "Created: C:\ADDS25" -ForegroundColor Green
}
icacls.exe C:\ADDS25 /grant 'Users:(oi)(ci)(f)' | Out-Null

# ADDS25: Application subdirectories
$adds25Dirs = @(
    "C:\ADDS25\Dwg",
    "C:\ADDS25\Logs", 
    "C:\ADDS25\Plot",
    "C:\ADDS25\Config",
    "C:\ADDS25\Assemblies"
)

foreach ($dir in $adds25Dirs) {
    if(!(Test-Path -path $dir)) { 
        New-Item $dir -type directory 
        Write-Host "Created: $dir" -ForegroundColor Green
    }
}

# ADDS25: Temporary data directory (modernized)
Write-Host "Creating ADDS25 temporary directories..." -ForegroundColor Yellow

$tempTarget = "C:\ProgramData\ADDS25Temp"
if(!(Test-Path -path $tempTarget)) { 
    New-Item $tempTarget -type directory 
    Write-Host "Created: $tempTarget" -ForegroundColor Green
}

$tempTarget2 = "C:\ProgramData\ADDS25Temp\Dwg"
if(!(Test-Path -path $tempTarget2)) { 
    New-Item $tempTarget2 -type directory 
    Write-Host "Created: $tempTarget2" -ForegroundColor Green
}
icacls.exe $tempTarget /grant 'Users:(oi)(ci)(f)' | Out-Null

# ADDS25: LISP and AutoCAD integration directories (modernized)
Write-Host "Creating ADDS25 LISP integration directories..." -ForegroundColor Yellow

$lispDirs = @(
    "C:\ADDS25_Map",
    "C:\ADDS25_Map\Adds",
    "C:\ADDS25_Map\Common", 
    "C:\ADDS25_Map\DosLib",
    "C:\ADDS25_Map\Icon_Collection",
    "C:\ADDS25_Map\LookUpTable",
    "C:\ADDS25_Map\Utils"
)

foreach ($dir in $lispDirs) {
    if(!(Test-Path -path $dir)) { 
        New-Item $dir -type directory 
        Write-Host "Created: $dir" -ForegroundColor Green
    }
}
icacls.exe C:\ADDS25_Map /grant 'Users:(oi)(ci)(f)' | Out-Null

# ADDS25: .NET Core 8 runtime verification
Write-Host "Verifying .NET Core 8 runtime..." -ForegroundColor Yellow

try {
    $dotnetVersion = & dotnet --version 2>$null
    if ($dotnetVersion -like "8.*") {
        Write-Host ".NET Core 8 Runtime: $dotnetVersion" -ForegroundColor Green
    } else {
        Write-Host "Warning: .NET Core 8 not detected. Current version: $dotnetVersion" -ForegroundColor Red
        Write-Host "Please install .NET Core 8 runtime for ADDS25" -ForegroundColor Red
    }
} catch {
    Write-Host "Error: .NET Core runtime not found. Please install .NET Core 8" -ForegroundColor Red
}

# ADDS25: Oracle 19c client verification
Write-Host "Verifying Oracle 19c client..." -ForegroundColor Yellow

$oracleRegPath = "HKLM:\SOFTWARE\ORACLE"
if (Test-Path $oracleRegPath) {
    Write-Host "Oracle client installation detected" -ForegroundColor Green
} else {
    Write-Host "Warning: Oracle client not detected. Please install Oracle 19c client" -ForegroundColor Red
}

Write-Host ""
Write-Host "*** ADDS25 Directory Setup Complete ***" -ForegroundColor Green
Write-Host "All directories created with proper permissions" -ForegroundColor Cyan
Write-Host "Ready for ADDS25 application deployment" -ForegroundColor Cyan
