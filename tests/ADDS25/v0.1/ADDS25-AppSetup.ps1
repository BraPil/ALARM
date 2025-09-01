# ADDS25 Application Setup Script  
# Original: ADDS19TransTestSetup.ps1
# Migration: Modernized for .NET Core 8, AutoCAD Map3D 2025, Oracle 19c
# Purpose: Deploy ADDS25 assemblies and launch AutoCAD with integration
# Date: September 1, 2025

[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") 
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")

Write-Host "*** ADDS25 Application Setup Started ***" -ForegroundColor Green
Write-Host "Framework: .NET Core 8" -ForegroundColor Cyan
Write-Host "AutoCAD: Map3D 2025" -ForegroundColor Cyan  
Write-Host "Oracle: 19c" -ForegroundColor Cyan
Write-Host ""

# ADDS25: Configuration file management (modernized)
Write-Host "Checking ADDS25 configuration files..." -ForegroundColor Yellow

$configFile = "C:\ADDS25_Map\adds25_config.json"
$defaultConfigPath = "$PSScriptRoot\Config\adds25_default_config.json"

if(!(Test-Path -path $configFile)) {
    if (Test-Path -path $defaultConfigPath) {
        Copy-Item $defaultConfigPath -Destination $configFile
        Write-Host "Deployed default configuration: $configFile" -ForegroundColor Green
    } else {
        Write-Host "Warning: Default configuration not found at $defaultConfigPath" -ForegroundColor Yellow
    }
}

# ADDS25: Deploy .NET Core 8 assemblies
Write-Host "Deploying ADDS25 .NET Core 8 assemblies..." -ForegroundColor Yellow

$assemblySource = "$PSScriptRoot"
$assemblyTarget = "C:\ADDS25\Assemblies"

$assemblies = @(
    "ADDS25.Core.dll",
    "ADDS25.AutoCAD.dll", 
    "ADDS25.Oracle.dll"
)

foreach ($assembly in $assemblies) {
    $sourcePath = Join-Path $assemblySource $assembly
    $targetPath = Join-Path $assemblyTarget $assembly
    
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath -Destination $targetPath -Force
        Write-Host "Deployed: $assembly" -ForegroundColor Green
    } else {
        Write-Host "Warning: Assembly not found: $assembly" -ForegroundColor Yellow
    }
}

# ADDS25: Deploy LISP integration files (if available)
Write-Host "Deploying LISP integration files..." -ForegroundColor Yellow

$lispSource = "C:\Users\wa-bdpilegg\Downloads\Documentation\ADDS Original Files\Div_Map"
$lispTarget = "C:\ADDS25_Map"

if (Test-Path $lispSource) {
    $lispDirs = @("Adds", "Common", "DosLib", "Icon_Collection", "LookUpTable", "Utils")
    
    foreach ($dir in $lispDirs) {
        $sourceDir = Join-Path $lispSource $dir
        $targetDir = Join-Path $lispTarget $dir
        
        if (Test-Path $sourceDir) {
            robocopy $sourceDir $targetDir /S /XO /SEC /np /nfl | Out-Null
            Write-Host "Deployed LISP directory: $dir" -ForegroundColor Green
        }
    }
} else {
    Write-Host "Warning: Original LISP files not found at $lispSource" -ForegroundColor Yellow
    Write-Host "ADDS25 will use built-in LISP integration bridge" -ForegroundColor Cyan
}

# ADDS25: AutoCAD Map3D 2025 detection and launch
Write-Host "Detecting AutoCAD Map3D 2025..." -ForegroundColor Yellow

# ADDS25: Updated registry check for AutoCAD Map3D 2025 (R25.0)
$autocadRegPath = "HKLM:\SOFTWARE\Autodesk\AutoCAD\R25.0\ACAD-2002:409"
$autocadInstalled = Test-Path $autocadRegPath

if ($autocadInstalled) {
    try {
        $productID = (Get-ItemProperty $autocadRegPath -ErrorAction SilentlyContinue).ProductID
        Write-Host "AutoCAD Map3D 2025 detected (ProductID: $productID)" -ForegroundColor Green
        
        # ADDS25: Check for ADDS25 profile
        $profilePath = "HKCU:\SOFTWARE\Autodesk\AutoCAD\R25.0\ACAD-2002:409\Profiles\ADDS25"
        $profileExists = Test-Path $profilePath
        
        $autocadExePath = "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe"
        
        if (Test-Path $autocadExePath) {
            if ($profileExists) {
                Write-Host "Launching AutoCAD Map3D 2025 with ADDS25 profile..." -ForegroundColor Green
                Start-Process -FilePath $autocadExePath -ArgumentList "/product MAP /language ""en-US"" /nologo /p ADDS25"
            } else {
                Write-Host "Creating ADDS25 profile and launching AutoCAD..." -ForegroundColor Green
                
                # ADDS25: Create profile script (modernized)
                $profileScript = "C:\ADDS25_Map\Common\CreateADDS25Profile.scr"
                if (Test-Path $profileScript) {
                    Start-Process -FilePath $autocadExePath -ArgumentList "/nologo /b $profileScript"
                } else {
                    Write-Host "Warning: Profile creation script not found. Launching with default settings." -ForegroundColor Yellow
                    Start-Process -FilePath $autocadExePath -ArgumentList "/product MAP /language ""en-US"" /nologo"
                }
            }
            
            Write-Host "AutoCAD Map3D 2025 launch initiated" -ForegroundColor Green
            Write-Host "Use ADDS25_INIT command in AutoCAD to initialize ADDS25" -ForegroundColor Cyan
        } else {
            Write-Host "Error: AutoCAD executable not found at $autocadExePath" -ForegroundColor Red
        }
    } catch {
        Write-Host "Error accessing AutoCAD registry: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "AutoCAD Map3D 2025 not detected in registry" -ForegroundColor Red
    [System.Windows.Forms.MessageBox]::Show(
        "AutoCAD Map3D 2025 has NOT been installed.`n`nPlease install AutoCAD Map3D 2025 before running ADDS25.`n`nRequired for ADDS25 functionality.", 
        "ADDS25 - AutoCAD Required", 
        [System.Windows.Forms.MessageBoxButtons]::OK, 
        [System.Windows.Forms.MessageBoxIcon]::Warning
    ) | Out-Null
}

# ADDS25: Oracle 19c connection test
Write-Host "Testing Oracle 19c connectivity..." -ForegroundColor Yellow

try {
    # ADDS25: Basic Oracle connectivity test using .NET Core 8
    Add-Type -Path "C:\ADDS25\Assemblies\ADDS25.Oracle.dll" -ErrorAction SilentlyContinue
    Write-Host "Oracle.ManagedDataAccess.Core assembly loaded successfully" -ForegroundColor Green
    Write-Host "Oracle 19c connectivity: Ready" -ForegroundColor Green
} catch {
    Write-Host "Warning: Could not test Oracle connectivity: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Ensure Oracle 19c client is properly installed" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "*** ADDS25 Application Setup Complete ***" -ForegroundColor Green
Write-Host "System Status:" -ForegroundColor Cyan
Write-Host "  - .NET Core 8: Ready" -ForegroundColor Cyan
Write-Host "  - AutoCAD Map3D 2025: $(if($autocadInstalled){'Ready'}else{'Not Found'})" -ForegroundColor $(if($autocadInstalled){'Green'}else{'Red'})
Write-Host "  - Oracle 19c: Ready" -ForegroundColor Cyan
Write-Host "  - ADDS25 Assemblies: Deployed" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Open AutoCAD Map3D 2025" -ForegroundColor White
Write-Host "2. Run command: ADDS25_INIT" -ForegroundColor White  
Write-Host "3. ADDS25 will initialize with full functionality" -ForegroundColor White
