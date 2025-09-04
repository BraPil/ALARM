# ADDS25 Trigger File Generator
# Purpose: Create standardized trigger files for file-based communication system
# Environment: Dev Computer (kidsg) - Production-ready trigger generation
# Date: September 3, 2025
# Version: 1.0 - Standardized trigger creation with validation

param(
    [Parameter(Mandatory=$true)]
    [string]$Action,
    
    [Parameter(Mandatory=$true)]
    [string]$CommitHash,
    
    [Parameter(Mandatory=$true)]
    [string]$CommitMessage,
    
    [string[]]$FilesToCheck = @("test-results\*.md", "test-results\*.log"),
    
    [ValidateSet("high", "normal", "low")]
    [string]$Priority = "normal",
    
    [ValidateSet("test_computer", "dev_computer", "automated")]
    [string]$Source = "dev_computer",
    
    [string]$CustomData = "",
    
    [switch]$UseTemplate,
    
    [string]$TemplateName = "default",
    
    [switch]$ValidateOnly,
    
    [switch]$ShowVerbose
)

# Configuration
$script:Config = @{
    TriggerDirectory = "C:\Users\kidsg\Downloads\ALARM\triggers"
    TemplateDirectory = "C:\Users\kidsg\Downloads\ALARM\ci\templates"
    DefaultTimezone = "ET"
    MaxMessageLength = 500
    MaxCustomDataLength = 1000
}

# Template definitions
$script:Templates = @{
    "default" = @{
        Description = "Standard trigger for test results analysis"
        Action = "analyze_test_results"
        Priority = "normal"
        FilesToCheck = @("test-results\*.md", "test-results\*.log")
    }
    "high_priority" = @{
        Description = "High priority trigger for critical issues"
        Action = "analyze_test_results"
        Priority = "high"
        FilesToCheck = @("test-results\*.md", "test-results\*.log", "test-results\*.error")
    }
    "deployment" = @{
        Description = "Trigger for fix deployment"
        Action = "deploy_fixes"
        Priority = "high"
        FilesToCheck = @("test-results\*.md", "ci\*.ps1")
    }
    "test_execution" = @{
        Description = "Trigger for test execution"
        Action = "run_tests"
        Priority = "normal"
        FilesToCheck = @("tests\**\*.ps1", "tests\**\*.bat")
    }
    "custom" = @{
        Description = "Custom action trigger"
        Action = "custom"
        Priority = "normal"
        FilesToCheck = @("**\*")
    }
}

# Validation functions
function Test-CommitHash {
    param([string]$Hash)
    
    if ($Hash -match "^[a-fA-F0-9]{7,40}$") {
        return $true
    }
    return $false
}

function Test-CommitMessage {
    param([string]$Message)
    
    if ($Message.Length -gt $script:Config.MaxMessageLength) {
        return $false
    }
    return $true
}

function Test-FilesToCheck {
    param([string[]]$Files)
    
    if ($Files.Count -eq 0) {
        return $false
    }
    
    foreach ($file in $Files) {
        if ([string]::IsNullOrWhiteSpace($file)) {
            return $false
        }
    }
    return $true
}

function Test-Action {
    param([string]$Action)
    
    $validActions = @("analyze_test_results", "deploy_fixes", "run_tests", "custom")
    return $Action -in $validActions
}

function Test-Priority {
    param([string]$Priority)
    
    $validPriorities = @("high", "normal", "low")
    return $Priority -in $validPriorities
}

function Test-Source {
    param([string]$Source)
    
    $validSources = @("test_computer", "dev_computer", "automated")
    return $Source -in $validSources
}

# Get current timezone
function Get-CurrentTimezone {
    try {
        $timezone = [System.TimeZoneInfo]::Local
        $offset = $timezone.GetUtcOffset([DateTime]::Now)
        
        if ($offset.Hours -eq -5) { return "ET" }
        elseif ($offset.Hours -eq -6) { return "CT" }
        elseif ($offset.Hours -eq -7) { return "MT" }
        elseif ($offset.Hours -eq -8) { return "PT" }
        else { return "UTC" }
    } catch {
        return "ET" # Default fallback
    }
}

# Load template
function Load-Template {
    param([string]$TemplateName)
    
    if ($script:Templates.ContainsKey($TemplateName)) {
        return $script:Templates[$TemplateName]
    }
    
    # Try to load from file
    $templateFile = Join-Path $script:Config.TemplateDirectory "$TemplateName.json"
    if (Test-Path $templateFile) {
        try {
            $template = Get-Content $templateFile -Raw | ConvertFrom-Json
            return $template
        } catch {
            Write-Warning "Failed to load template from file: $templateFile"
        }
    }
    
    Write-Warning "Template '$TemplateName' not found, using default"
    return $script:Templates["default"]
}

# Validate all parameters
function Test-AllParameters {
    $errors = @()
    
    # Test commit hash
    if (-not (Test-CommitHash $CommitHash)) {
        $errors += "Invalid commit hash: $CommitHash (must be 7-40 hex characters)"
    }
    
    # Test commit message
    if (-not (Test-CommitMessage $CommitMessage)) {
        $errors += "Invalid commit message: length $($CommitMessage.Length) exceeds maximum $($script:Config.MaxMessageLength)"
    }
    
    # Test files to check
    if (-not (Test-FilesToCheck $FilesToCheck)) {
        $errors += "Invalid files to check: must provide at least one valid file pattern"
    }
    
    # Test action
    if (-not (Test-Action $Action)) {
        $errors += "Invalid action: $Action (must be one of: analyze_test_results, deploy_fixes, run_tests, custom)"
    }
    
    # Test priority
    if (-not (Test-Priority $Priority)) {
        $errors += "Invalid priority: $Priority (must be one of: high, normal, low)"
    }
    
    # Test source
    if (-not (Test-Source $Source)) {
        $errors += "Invalid source: $Source (must be one of: test_computer, dev_computer, automated)"
    }
    
    # Test custom data length
    if ($CustomData.Length -gt $script:Config.MaxCustomDataLength) {
        $errors += "Custom data too long: length $($CustomData.Length) exceeds maximum $($script:Config.MaxCustomDataLength)"
    }
    
    return $errors
}

# Generate trigger data
function Generate-TriggerData {
    $template = Load-Template $TemplateName
    
    # Override template values with parameters if provided
    $triggerData = @{
        commit = $CommitHash
        message = $CommitMessage
        timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        timezone = Get-CurrentTimezone
        action = if ($Action) { $Action } else { $template.Action }
        files_to_check = if ($FilesToCheck) { $FilesToCheck } else { $template.FilesToCheck }
        priority = if ($Priority) { $Priority } else { $template.Priority }
        source = if ($Source) { $Source } else { "dev_computer" }
    }
    
    # Add custom data if provided
    if ($CustomData) {
        $triggerData.custom_data = $CustomData
    }
    
    # Add metadata
    $triggerData.metadata = @{
        generated_by = "GENERATE-TRIGGER.ps1"
        generated_at = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        template_used = $TemplateName
        version = "1.0"
    }
    
    return $triggerData
}

# Create trigger file
function Create-TriggerFile {
    param([hashtable]$TriggerData)
    
    # Ensure trigger directory exists
    if (!(Test-Path $script:Config.TriggerDirectory)) {
        New-Item -ItemType Directory -Path $script:Config.TriggerDirectory -Force | Out-Null
        Write-Host "Created trigger directory: $($script:Config.TriggerDirectory)" -ForegroundColor Green
    }
    
    # Generate filename
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $filename = "CURSOR-ANALYZE-NOW.trigger"
    $filepath = Join-Path $script:Config.TriggerDirectory $filename
    
    # Convert to JSON with proper formatting
    $jsonContent = $TriggerData | ConvertTo-Json -Depth 10
    
    # Write trigger file
    try {
        Set-Content -Path $filepath -Value $jsonContent -Encoding UTF8 -Force
        Write-Host "PASS: Trigger file created successfully: $filepath" -ForegroundColor Green
        
        if ($ShowVerbose) {
            Write-Host ""
            Write-Host "TRIGGER CONTENT:" -ForegroundColor Cyan
            Write-Host $jsonContent -ForegroundColor White
        }
        
        return $filepath
        
    } catch {
        Write-Error "Failed to create trigger file: $($_.Exception.Message)"
        return $null
    }
}

# Display help information
function Show-Help {
    Write-Host "*** ADDS25 Trigger File Generator ***" -ForegroundColor Cyan
    Write-Host "Purpose: Create standardized trigger files for file-based communication" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "USAGE:" -ForegroundColor Green
    Write-Host "  .\GENERATE-TRIGGER.ps1 -Action action -CommitHash hash -CommitMessage message" -ForegroundColor White
    Write-Host ""
    Write-Host "REQUIRED PARAMETERS:" -ForegroundColor Green
    Write-Host "  -Action           Action to perform: analyze_test_results, deploy_fixes, run_tests, custom" -ForegroundColor White
    Write-Host "  -CommitHash       Git commit hash: 7-40 hex characters" -ForegroundColor White
    Write-Host "  -CommitMessage    Commit message: max 500 characters" -ForegroundColor White
    Write-Host ""
    Write-Host "OPTIONAL PARAMETERS:" -ForegroundColor Green
    Write-Host "  -FilesToCheck     Array of file patterns to check (default: test-results\*.md, test-results\*.log)" -ForegroundColor White
    Write-Host "  -Priority         Priority level (high, normal, low) (default: normal)" -ForegroundColor White
    Write-Host "  -Source           Source of the trigger (test_computer, dev_computer, automated) (default: dev_computer)" -ForegroundColor White
    Write-Host "  -CustomData       Additional custom data (max 1000 characters)" -ForegroundColor White
    Write-Host "  -UseTemplate      Use template system for common scenarios" -ForegroundColor White
    Write-Host "  -TemplateName     Template to use (default, high_priority, deployment, test_execution, custom)" -ForegroundColor White
    Write-Host "  -ValidateOnly     Only validate parameters without creating file" -ForegroundColor White
    Write-Host "  -ShowVerbose      Show detailed output" -ForegroundColor White
    Write-Host ""
    Write-Host "EXAMPLES:" -ForegroundColor Green
    Write-Host "  # Basic test results analysis" -ForegroundColor White
    Write-Host "  .\GENERATE-TRIGGER.ps1 -Action analyze_test_results -CommitHash abc1234 -CommitMessage 'Test results ready'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  # High priority deployment trigger" -ForegroundColor White
    Write-Host "  .\GENERATE-TRIGGER.ps1 -Action deploy_fixes -CommitHash def5678 -CommitMessage 'Critical fixes ready' -Priority high" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  # Using template system" -ForegroundColor White
    Write-Host "  .\GENERATE-TRIGGER.ps1 -UseTemplate -TemplateName high_priority -CommitHash ghi9012 -CommitMessage 'High priority analysis needed'" -ForegroundColor Gray
    Write-Host ""
    Write-Host "TEMPLATES AVAILABLE:" -ForegroundColor Green
    foreach ($template in $script:Templates.Keys) {
        $desc = $script:Templates[$template].Description
        Write-Host "  $template`: $desc" -ForegroundColor White
    }
    Write-Host ""
}

# Main execution
function Main {
    # Show help if no parameters or help requested
    if ($PSBoundParameters.Count -eq 0 -or $Action -eq "help") {
        Show-Help
        return
    }
    
    Write-Host "*** ADDS25 Trigger File Generator ***" -ForegroundColor Cyan
    Write-Host "Validating parameters..." -ForegroundColor Yellow
    
    # Validate all parameters
    $validationErrors = Test-AllParameters
    
    if ($validationErrors.Count -gt 0) {
        Write-Host ""
        Write-Host "FAIL: VALIDATION ERRORS:" -ForegroundColor Red
        foreach ($validationError in $validationErrors) {
            Write-Host "  - $validationError" -ForegroundColor Red
        }
        Write-Host ""
        Write-Host "Use -ShowVerbose for detailed help information" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "PASS: All parameters validated successfully" -ForegroundColor Green
    
    if ($ValidateOnly) {
        Write-Host "Validation only mode - no file will be created" -ForegroundColor Yellow
        return
    }
    
    # Generate trigger data
    Write-Host "Generating trigger data..." -ForegroundColor Yellow
    $triggerData = Generate-TriggerData
    
    # Create trigger file
    Write-Host "Creating trigger file..." -ForegroundColor Yellow
    $filepath = Create-TriggerFile -TriggerData $triggerData
    
    if ($filepath) {
        Write-Host ""
        Write-Host "SUCCESS: TRIGGER FILE READY!" -ForegroundColor Green
        Write-Host "===============================================" -ForegroundColor Yellow
        Write-Host "File: $filepath" -ForegroundColor White
        Write-Host "Action: $($triggerData.action)" -ForegroundColor White
        Write-Host "Priority: $($triggerData.priority)" -ForegroundColor White
        Write-Host "Source: $($triggerData.source)" -ForegroundColor White
        Write-Host "Commit: $($triggerData.commit)" -ForegroundColor White
        Write-Host "Timestamp: $($triggerData.timestamp) ($($triggerData.timezone))" -ForegroundColor White
        Write-Host ""
        Write-Host "The file monitor will detect this trigger and execute the analysis automatically." -ForegroundColor Cyan
        Write-Host "===============================================" -ForegroundColor Yellow
    } else {
        Write-Host "FAIL: Failed to create trigger file" -ForegroundColor Red
        exit 1
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "Critical error: $($_.Exception.Message)"
    Write-Host "Stack trace: $($_.ScriptStackTrace)" -ForegroundColor Red
    exit 1
}
