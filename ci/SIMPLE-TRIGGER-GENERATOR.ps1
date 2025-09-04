# SIMPLE TRIGGER GENERATOR WORKAROUND
# Basic trigger file generation without complex help text

param(
    [string]$Action = "test",
    [string]$Hash = "test123",
    [string]$Message = "Test trigger",
    [string[]]$Files = @("test.txt"),
    [string]$Priority = "normal",
    [string]$Source = "simple-generator"
)

function Create-SimpleTrigger {
    param(
        [string]$Action,
        [string]$Hash,
        [string]$Message,
        [string[]]$Files,
        [string]$Priority,
        [string]$Source
    )
    
    # Basic parameter validation
    if ([string]::IsNullOrEmpty($Action)) {
        Write-Host "Error: Action parameter is required" -ForegroundColor Red
        return $null
    }
    
    if ([string]::IsNullOrEmpty($Hash)) {
        Write-Host "Error: Hash parameter is required" -ForegroundColor Red
        return $null
    }
    
    if ([string]::IsNullOrEmpty($Message)) {
        Write-Host "Error: Message parameter is required" -ForegroundColor Red
        return $null
    }
    
    if ($Files -eq $null -or $Files.Count -eq 0) {
        Write-Host "Error: Files parameter is required" -ForegroundColor Red
        return $null
    }
    
    if ($Priority -notin @("high", "normal", "low")) {
        Write-Host "Error: Priority must be high, normal, or low" -ForegroundColor Red
        return $null
    }
    
    if ($Source -notin @("test_computer", "dev_computer", "automated")) {
        Write-Host "Error: Source must be test_computer, dev_computer, or automated" -ForegroundColor Red
        return $null
    }
    
    try {
        # Create trigger object
        $Trigger = @{
            action = $Action
            hash = $Hash
            message = $Message
            files = $Files
            priority = $Priority
            source = $Source
            timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            timezone = "UTC"
        }
        
        # Create trigger file
        $TriggerFile = "triggers\simple-trigger-$(Get-Date -Format 'yyyyMMdd-HHmmss-fff').json"
        $TriggerDir = Split-Path $TriggerFile -Parent
        
        if (-not (Test-Path $TriggerDir)) {
            New-Item -ItemType Directory -Path $TriggerDir -Force | Out-Null
        }
        
        $Trigger | ConvertTo-Json -Depth 3 | Set-Content -Path $TriggerFile
        
        Write-Host "Trigger file created: $TriggerFile" -ForegroundColor Green
        return $TriggerFile
        
    } catch {
        Write-Host "Error creating trigger: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Main execution
try {
    $Result = Create-SimpleTrigger -Action $Action -Hash $Hash -Message $Message -Files $Files -Priority $Priority -Source $Source
    
    if ($Result) {
        Write-Host "Simple trigger generation completed successfully" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "Simple trigger generation failed" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "Error in simple trigger generator: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
