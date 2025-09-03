# ADDS25 Application Setup Script
# Purpose: Load ADDS25 into AutoCAD Map3D 2025 with LISP integration
# Environment: Test Computer (wa-bdpilegg)
# Date: September 2, 2025

Write-Host "*** ADDS25 Application Setup Starting ***" -ForegroundColor Cyan
Write-Host "Purpose: Load ADDS25 into AutoCAD Map3D 2025" -ForegroundColor Yellow
Write-Host ""

function Write-AppLog {
    param([string]$Message, [string]$Level = "INFO")
    $logEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $logEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } })
}

# Configuration
$autocadPath = "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe"
$adds25DllPath = "C:\Users\wa-bdpilegg\Downloads\ALARM\tests\ADDS25\v0.1\ADDS25.AutoCAD\bin\Debug\net8.0-windows\ADDS25.AutoCAD.dll"
$lispPath = "C:\Div_Map"
$logDir = "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"

# Step 1: Verify AutoCAD is running
Write-Host "Step 1: Checking AutoCAD status..." -ForegroundColor Yellow
$autocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue

if (!$autocadProcess) {
    Write-AppLog "AutoCAD not running. Attempting to start..." "INFO"
    try {
        Start-Process $autocadPath -PassThru
        Start-Sleep -Seconds 10
        $autocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
        
        if ($autocadProcess) {
            Write-AppLog "AutoCAD started successfully (PID: $($autocadProcess.Id))" "SUCCESS"
        } else {
            Write-AppLog "Failed to start AutoCAD" "ERROR"
            return
        }
    } catch {
        Write-AppLog "Error starting AutoCAD: $($_.Exception.Message)" "ERROR"
        return
    }
} else {
    Write-AppLog "AutoCAD already running (PID: $($autocadProcess.Id))" "SUCCESS"
}

# Step 2: Verify ADDS25 DLL exists
Write-Host ""
Write-Host "Step 2: Verifying ADDS25 DLL..." -ForegroundColor Yellow
if (Test-Path $adds25DllPath) {
    Write-AppLog "ADDS25 DLL found: $adds25DllPath" "SUCCESS"
} else {
    Write-AppLog "ADDS25 DLL not found: $adds25DllPath" "ERROR"
    Write-AppLog "Please ensure ADDS25 solution has been built successfully" "ERROR"
    return
}

# Step 3: Check LISP files
Write-Host ""
Write-Host "Step 3: Checking LISP files..." -ForegroundColor Yellow
if (Test-Path $lispPath) {
    $lispFiles = Get-ChildItem "$lispPath\*.lsp" -ErrorAction SilentlyContinue
    if ($lispFiles) {
        Write-AppLog "Found $($lispFiles.Count) LISP files in $lispPath" "SUCCESS"
        foreach ($lisp in $lispFiles) {
            Write-AppLog "  - $($lisp.Name)" "INFO"
        }
    } else {
        Write-AppLog "No LISP files found in $lispPath" "WARNING"
    }
} else {
    Write-AppLog "LISP directory not found: $lispPath" "WARNING"
}

# Step 4: Create AutoCAD script to load ADDS25
Write-Host ""
Write-Host "Step 4: Creating AutoCAD script to load ADDS25..." -ForegroundColor Yellow

$scriptPath = "$env:TEMP\ADDS25-LoadScript.scr"
$scriptContent = @"
; ADDS25 AutoCAD Loading Script
; Load ADDS25 .NET assembly into AutoCAD Map3D 2025

; Load the ADDS25 .NET DLL
NETLOAD "$adds25DllPath"

; Load LISP files if they exist
$(if (Test-Path $lispPath) {
    $lispFiles = Get-ChildItem "$lispPath\*.lsp" -ErrorAction SilentlyContinue
    if ($lispFiles) {
        foreach ($lisp in $lispFiles) {
            "(load `"$($lisp.FullName)`")"
        }
    }
})

; Set up ADDS25 environment
(setq ADDS25_LOADED T)
(princ "\nADDS25 loaded successfully into AutoCAD Map3D 2025")

; Initialize ADDS25 commands (if available)
; This would typically call ADDS25 initialization functions

"@

try {
    $scriptContent | Out-File $scriptPath -Encoding ASCII
    Write-AppLog "AutoCAD script created: $scriptPath" "SUCCESS"
} catch {
    Write-AppLog "Error creating AutoCAD script: $($_.Exception.Message)" "ERROR"
    return
}

# Step 5: Execute the script in AutoCAD
Write-Host ""
Write-Host "Step 5: Loading ADDS25 into AutoCAD..." -ForegroundColor Yellow

try {
    # Send the script to AutoCAD via command line
    # Note: This is a simplified approach. In production, you might use AutoCAD's COM interface
    Write-AppLog "Attempting to load ADDS25 into AutoCAD..." "INFO"
    
    # Create a simple command to execute the script
    $commandPath = "$env:TEMP\ADDS25-Command.txt"
    "SCRIPT `"$scriptPath`"" | Out-File $commandPath -Encoding ASCII
    
    Write-AppLog "Script execution command created: $commandPath" "SUCCESS"
    Write-AppLog "Manual step: In AutoCAD, type: SCRIPT `"$scriptPath`"" "INFO"
    
} catch {
    Write-AppLog "Error executing AutoCAD script: $($_.Exception.Message)" "ERROR"
}

# Step 6: Verify loading (basic check)
Write-Host ""
Write-Host "Step 6: Verification..." -ForegroundColor Yellow

# Check if AutoCAD is still running
$autocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
if ($autocadProcess) {
    Write-AppLog "AutoCAD still running after setup (PID: $($autocadProcess.Id))" "SUCCESS"
} else {
    Write-AppLog "AutoCAD process not detected after setup" "WARNING"
}

# Step 7: Create completion log
Write-Host ""
Write-Host "Step 7: Creating completion log..." -ForegroundColor Yellow

if (Test-Path $logDir) {
    $appSetupLog = "$logDir\app-setup-$(Get-Date -Format 'yyyy-MM-dd_HH-mm-ss').md"
    @"
# ADDS25 Application Setup Log

**Setup Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Script**: ADDS25-AppSetup.ps1

## Setup Components

### AutoCAD Status
- AutoCAD Path: $autocadPath
- Process Status: $(if ($autocadProcess) { "RUNNING (PID: $($autocadProcess.Id))" } else { "NOT DETECTED" })

### ADDS25 Components
- DLL Path: $adds25DllPath
- DLL Status: $(if (Test-Path $adds25DllPath) { "FOUND" } else { "NOT FOUND" })
- LISP Directory: $lispPath
- LISP Status: $(if (Test-Path $lispPath) { "FOUND" } else { "NOT FOUND" })

### Generated Files
- AutoCAD Script: $scriptPath
- Command File: $commandPath

## Manual Steps Required

To complete ADDS25 loading in AutoCAD:
1. Switch to AutoCAD Map3D 2025
2. Type: SCRIPT "$scriptPath"
3. Press Enter to execute

## Setup Status

- Directory verification: COMPLETED
- AutoCAD detection: $(if ($autocadProcess) { "SUCCESS" } else { "WARNING" })
- Script generation: COMPLETED
- Manual loading required: YES

**Setup Complete**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
"@ | Out-File $appSetupLog -Encoding UTF8
    
    Write-AppLog "Application setup log created: $appSetupLog" "SUCCESS"
}

Write-Host ""
Write-Host "*** ADDS25 Application Setup Complete ***" -ForegroundColor Green
Write-Host "AutoCAD script has been prepared for ADDS25 loading." -ForegroundColor White
Write-Host ""
Write-Host "MANUAL STEP REQUIRED:" -ForegroundColor Yellow
Write-Host "In AutoCAD, type: SCRIPT `"$scriptPath`"" -ForegroundColor Cyan
Write-Host "This will load the ADDS25 .NET assembly and LISP files." -ForegroundColor White
Write-Host ""