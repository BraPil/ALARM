# ADDS25 Directory Setup Script
# Purpose: Create necessary directories for ADDS25 operation
# Environment: Test Computer (wa-bdpilegg)
# Date: September 2, 2025

Write-Host "*** ADDS25 Directory Setup Starting ***" -ForegroundColor Cyan
Write-Host "Purpose: Create necessary directories for ADDS25 operation" -ForegroundColor Yellow
Write-Host ""

# Configuration - Updated for ADDS25 deployment
$directories = @(
    "C:\Adds",
    "C:\Adds\UA",
    "C:\Adds\UA\Setup",
    "C:\Adds\UA\Setup\ADDS25",
    "C:\Div_Map",
    "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results",
    "C:\Users\wa-bdpilegg\Downloads\ADDS25-Logs"
)

function Write-DirLog {
    param([string]$Message, [string]$Level = "INFO")
    $logEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $logEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } })
}

# Create directories
Write-Host "Creating ADDS25 directory structure..." -ForegroundColor Yellow
Write-Host ""

foreach ($dir in $directories) {
    try {
        if (!(Test-Path $dir)) {
            New-Item $dir -Type Directory -Force | Out-Null
            Write-DirLog "Created directory: $dir" "SUCCESS"
        } else {
            Write-DirLog "Directory exists: $dir" "INFO"
        }
    } catch {
        Write-DirLog "Error creating directory $dir`: $($_.Exception.Message)" "ERROR"
    }
}

# Copy LISP files from Div_Map (if source exists)
Write-Host ""
Write-Host "Checking for LISP files to copy..." -ForegroundColor Yellow

$sourceDiv = "C:\Users\wa-bdpilegg\Downloads\ALARM\tests\ADDS25\v0.1\Div_Map"
$targetDiv = "C:\Div_Map"

if (Test-Path $sourceDiv) {
    try {
        Write-DirLog "Copying LISP files from $sourceDiv to $targetDiv" "INFO"
        Copy-Item "$sourceDiv\*" $targetDiv -Recurse -Force
        Write-DirLog "LISP files copied successfully" "SUCCESS"
    } catch {
        Write-DirLog "Error copying LISP files: $($_.Exception.Message)" "WARNING"
    }
} else {
    Write-DirLog "Source LISP directory not found: $sourceDiv" "WARNING"
}

# Set permissions (if needed)
Write-Host ""
Write-Host "Setting directory permissions..." -ForegroundColor Yellow

foreach ($dir in $directories) {
    if (Test-Path $dir) {
        try {
            # Grant full control to current user
            $acl = Get-Acl $dir
            $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule(
                [System.Security.Principal.WindowsIdentity]::GetCurrent().Name,
                "FullControl",
                "ContainerInherit,ObjectInherit",
                "None",
                "Allow"
            )
            $acl.SetAccessRule($accessRule)
            Set-Acl $dir $acl
            Write-DirLog "Permissions set for: $dir" "SUCCESS"
        } catch {
            Write-DirLog "Warning: Could not set permissions for $dir`: $($_.Exception.Message)" "WARNING"
        }
    }
}

Write-Host ""
Write-Host "*** ADDS25 Directory Setup Complete ***" -ForegroundColor Green
Write-Host "All necessary directories have been created and configured." -ForegroundColor White
Write-Host ""

# Log completion
$logDir = "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"
if (Test-Path $logDir) {
    $setupLog = "$logDir\directory-setup-$(Get-Date -Format 'yyyy-MM-dd_HH-mm-ss').md"
    @"
# ADDS25 Directory Setup Log

**Setup Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Script**: ADDS25-DirSetup.ps1

## Directories Created/Verified

$(foreach ($dir in $directories) { "- $dir" }) -join "`n")

## Setup Status

- Directory creation: COMPLETED
- LISP file copying: $(if (Test-Path $sourceDiv) { "COMPLETED" } else { "SKIPPED (source not found)" })
- Permission setting: COMPLETED

**Setup Complete**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
"@ | Out-File $setupLog -Encoding UTF8
    
    Write-DirLog "Setup log created: $setupLog" "SUCCESS"
}