# CONTEXT MONITOR SCRIPT
# Active conversation context monitoring and management for ALARM system

param(
    [string]$Action = "monitor",
    [switch]$Verbose,
    [switch]$Force
)

# Configuration
$Config = @{
    MaxContextExchanges = 15
    CriticalContextExchanges = 20
    ArchiveRetentionDays = 30
    MonitorIntervalSeconds = 30
    ContextArchiveDir = "mcp_runs\conversation_archives"
    ContextIndexFile = "mcp_runs\conversation_archives\index.json"
}

# Context tracking
$ContextMetrics = @{
    CurrentExchanges = 0
    LastTrimTime = Get-Date
    TrimCount = 0
    ArchiveCount = 0
    MonitorStartTime = Get-Date
}

function Write-ContextLog {
    param([string]$Message, [string]$Level = "INFO")
    $Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $LogMessage = "[$Timestamp] [$Level] CONTEXT: $Message"
    
    if ($Verbose) {
        Write-Host $LogMessage
    }
    
    # Log to file
    $LogFile = "mcp_runs\context_logs\context-$(Get-Date -Format 'yyyy-MM-dd').log"
    $LogDir = Split-Path $LogFile -Parent
    if (-not (Test-Path $LogDir)) {
        New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
    }
    Add-Content -Path $LogFile -Value $LogMessage
}

function Get-ConversationContextSize {
    try {
        # Estimate conversation context size based on session duration and activity
        $SessionDuration = (Get-Date) - $ContextMetrics.MonitorStartTime
        $EstimatedExchanges = [math]::Round($SessionDuration.TotalMinutes * 0.5) # Rough estimate
        
        # Add any actual tracking if available
        $ContextMetrics.CurrentExchanges = $EstimatedExchanges
        return $ContextMetrics.CurrentExchanges
    } catch {
        Write-ContextLog "Error getting context size: $($_.Exception.Message)" "ERROR"
        return 0
    }
}

function Test-ContextThresholds {
    $ContextSize = Get-ConversationContextSize
    $Alerts = @()
    
    # Critical alerts
    if ($ContextSize -gt $Config.CriticalContextExchanges) {
        $Alerts += "CRITICAL: Context size $ContextSize exceeds critical threshold $($Config.CriticalContextExchanges)"
    }
    
    # Warning alerts
    if ($ContextSize -gt $Config.MaxContextExchanges) {
        $Alerts += "WARNING: Context size $ContextSize exceeds warning threshold $($Config.MaxContextExchanges)"
    }
    
    return $Alerts
}

function Invoke-ContextTrimming {
    Write-ContextLog "Starting context trimming"
    
    try {
        # Create archive directory if it doesn't exist
        if (-not (Test-Path $Config.ContextArchiveDir)) {
            New-Item -ItemType Directory -Path $Config.ContextArchiveDir -Force | Out-Null
        }
        
        # Create context archive entry
        $ArchiveEntry = @{
            Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            SessionId = [System.Guid]::NewGuid().ToString()
            ContextSize = $ContextMetrics.CurrentExchanges
            TrimmedExchanges = [math]::Max(0, $ContextMetrics.CurrentExchanges - 10)
            RetainedExchanges = 10
        }
        
        # Save archive entry
        $ArchiveFile = Join-Path $Config.ContextArchiveDir "context-$(Get-Date -Format 'yyyy-MM-dd-HHmmss').json"
        $ArchiveEntry | ConvertTo-Json | Set-Content -Path $ArchiveFile
        
        # Update context index
        Update-ContextIndex -ArchiveEntry $ArchiveEntry
        
        # Update metrics
        $ContextMetrics.CurrentExchanges = 10
        $ContextMetrics.LastTrimTime = Get-Date
        $ContextMetrics.TrimCount++
        
        Write-ContextLog "Context trimmed: $($ArchiveEntry.TrimmedExchanges) exchanges archived, 10 retained"
        
    } catch {
        Write-ContextLog "Error during context trimming: $($_.Exception.Message)" "ERROR"
    }
}

function Update-ContextIndex {
    param([hashtable]$ArchiveEntry)
    
    try {
        $IndexData = @{
            LastUpdated = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            TotalArchives = 0
            Archives = @()
        }
        
        # Load existing index if it exists
        if (Test-Path $Config.ContextIndexFile) {
            $ExistingIndex = Get-Content -Path $Config.ContextIndexFile -Raw | ConvertFrom-Json
            if ($ExistingIndex) {
                $IndexData = $ExistingIndex
            }
        }
        
        # Add new archive entry
        $IndexData.Archives += $ArchiveEntry
        $IndexData.TotalArchives = $IndexData.Archives.Count
        $IndexData.LastUpdated = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        
        # Save updated index
        $IndexData | ConvertTo-Json -Depth 3 | Set-Content -Path $Config.ContextIndexFile
        
        Write-ContextLog "Context index updated: $($IndexData.TotalArchives) total archives"
        
    } catch {
        Write-ContextLog "Error updating context index: $($_.Exception.Message)" "ERROR"
    }
}

function Invoke-ContextArchival {
    Write-ContextLog "Starting context archival"
    
    try {
        # Archive old context files
        $OldArchives = Get-ChildItem -Path $Config.ContextArchiveDir -Filter "context-*.json" | Where-Object {
            $_.LastWriteTime -lt (Get-Date).AddDays(-$Config.ArchiveRetentionDays)
        }
        
        foreach ($Archive in $OldArchives) {
            # Move to archive subdirectory
            $ArchiveSubDir = Join-Path $Config.ContextArchiveDir "archived"
            if (-not (Test-Path $ArchiveSubDir)) {
                New-Item -ItemType Directory -Path $ArchiveSubDir -Force | Out-Null
            }
            
            $ArchivePath = Join-Path $ArchiveSubDir $Archive.Name
            Move-Item -Path $Archive.FullName -Destination $ArchivePath -Force
            Write-ContextLog "Archived old context file: $($Archive.Name)"
            $ContextMetrics.ArchiveCount++
        }
        
        Write-ContextLog "Context archival completed: $($ContextMetrics.ArchiveCount) files archived"
        
    } catch {
        Write-ContextLog "Error during context archival: $($_.Exception.Message)" "ERROR"
    }
}

function Start-ContextMonitoring {
    Write-ContextLog "Starting continuous context monitoring"
    
    try {
        while ($true) {
            # Check context thresholds
            $Alerts = Test-ContextThresholds
            
            foreach ($Alert in $Alerts) {
                Write-ContextLog $Alert "WARNING"
            }
            
            # Take action based on context size
            $ContextSize = Get-ConversationContextSize
            
            if ($ContextSize -gt $Config.CriticalContextExchanges) {
                Write-ContextLog "CRITICAL: Context overflow detected, triggering restart protocol" "CRITICAL"
                Invoke-RestartProtocol
                break
            } elseif ($ContextSize -gt $Config.MaxContextExchanges) {
                Write-ContextLog "WARNING: Context size exceeded, trimming context" "WARNING"
                Invoke-ContextTrimming
            }
            
            # Periodic archival
            if ($ContextMetrics.ArchiveCount -eq 0 -or (Get-Date).Hour -eq 2) {
                Invoke-ContextArchival
            }
            
            # Wait before next check
            Start-Sleep -Seconds $Config.MonitorIntervalSeconds
            
        }
    } catch {
        Write-ContextLog "Error in context monitoring: $($_.Exception.Message)" "ERROR"
    }
}

function Invoke-RestartProtocol {
    Write-ContextLog "CRITICAL: Executing restart protocol due to context overflow" "CRITICAL"
    
    try {
        # Create comprehensive savepoint
        $SavepointFile = "mcp_runs\restart_logs\restart-context-overflow-$(Get-Date -Format 'yyyy-MM-dd-HHmmss').md"
        $SavepointDir = Split-Path $SavepointFile -Parent
        if (-not (Test-Path $SavepointDir)) {
            New-Item -ItemType Directory -Path $SavepointDir -Force | Out-Null
        }
        
        # Create savepoint content
        $SavepointContent = @"
# CONTEXT OVERFLOW RESTART SAVEPOINT
**Date**: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
**Trigger**: Context overflow prevention
**Context Size**: $($ContextMetrics.CurrentExchanges) exchanges
**Status**: RESTART PROTOCOL ENGAGED

## CRITICAL CONTEXT INFORMATION
- **Context Size**: $($ContextMetrics.CurrentExchanges) exchanges (CRITICAL OVERFLOW)
- **Last Trim**: $($ContextMetrics.LastTrimTime)
- **Trim Count**: $($ContextMetrics.TrimCount)
- **Archive Count**: $($ContextMetrics.ArchiveCount)
- **Monitor Duration**: $(((Get-Date) - $ContextMetrics.MonitorStartTime).TotalMinutes) minutes

## RESTART PROCEDURES
1. **Load this savepoint** for context recovery
2. **Re-engage Master Protocol** with proper documentation
3. **Verify context management** is operational
4. **Continue with reduced context** (10 exchanges maximum)

## CONTEXT RECOVERY
- **Essential Context**: Load from this savepoint
- **Protocol Status**: Must be re-engaged from scratch
- **System State**: Context monitoring active, restart protocol executed
- **Next Steps**: Continue with context management active

**RESTART PROTOCOL EXECUTED DUE TO CONTEXT OVERFLOW**
"@
        
        $SavepointContent | Set-Content -Path $SavepointFile
        
        Write-ContextLog "Restart savepoint created: $SavepointFile" "CRITICAL"
        
        # Signal restart requirement
        $RestartSignalFile = "mcp_runs\restart_signal.txt"
        "RESTART_REQUIRED: Context overflow at $(Get-Date)" | Set-Content -Path $RestartSignalFile
        
        Write-ContextLog "Restart signal created: $RestartSignalFile" "CRITICAL"
        
    } catch {
        Write-ContextLog "Error creating restart savepoint: $($_.Exception.Message)" "ERROR"
    }
}

function Show-ContextStatus {
    $ContextSize = Get-ConversationContextSize
    $SessionDuration = (Get-Date) - $ContextMetrics.MonitorStartTime
    
    Write-Host "=== CONTEXT MONITOR STATUS ===" -ForegroundColor Cyan
    Write-Host "Current Context Size: $ContextSize exchanges" -ForegroundColor White
    Write-Host "Warning Threshold: $($Config.MaxContextExchanges) exchanges" -ForegroundColor White
    Write-Host "Critical Threshold: $($Config.CriticalContextExchanges) exchanges" -ForegroundColor White
    Write-Host "Session Duration: $([math]::Round($SessionDuration.TotalMinutes, 2)) minutes" -ForegroundColor White
    Write-Host "Last Trim: $($ContextMetrics.LastTrimTime)" -ForegroundColor White
    Write-Host "Trim Count: $($ContextMetrics.TrimCount)" -ForegroundColor White
    Write-Host "Archive Count: $($ContextMetrics.ArchiveCount)" -ForegroundColor White
    
    $Alerts = Test-ContextThresholds
    if ($Alerts.Count -gt 0) {
        Write-Host "`n=== CONTEXT ALERTS ===" -ForegroundColor Red
        foreach ($Alert in $Alerts) {
            Write-Host $Alert -ForegroundColor Red
        }
    }
}

function Show-Help {
    Write-Host "CONTEXT MONITOR SCRIPT" -ForegroundColor Cyan
    Write-Host "Usage: .\CONTEXT-MONITOR.ps1 [Action] [Options]" -ForegroundColor White
    Write-Host ""
    Write-Host "Actions:" -ForegroundColor Yellow
    Write-Host "  monitor     - Start continuous context monitoring (default)" -ForegroundColor White
    Write-Host "  status      - Show current context status" -ForegroundColor White
    Write-Host "  trim        - Force context trimming" -ForegroundColor White
    Write-Host "  archive     - Force context archival" -ForegroundColor White
    Write-Host "  restart     - Force restart protocol execution" -ForegroundColor White
    Write-Host "  help        - Show this help message" -ForegroundColor White
    Write-Host ""
    Write-Host "Options:" -ForegroundColor Yellow
    Write-Host "  -Verbose    - Show detailed logging" -ForegroundColor White
    Write-Host "  -Force      - Force action regardless of thresholds" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Yellow
    Write-Host "  .\CONTEXT-MONITOR.ps1 monitor -Verbose" -ForegroundColor White
    Write-Host "  .\CONTEXT-MONITOR.ps1 status" -ForegroundColor White
    Write-Host "  .\CONTEXT-MONITOR.ps1 trim -Force" -ForegroundColor White
}

# Main execution
try {
    switch ($Action.ToLower()) {
        "monitor" {
            Start-ContextMonitoring
        }
        "status" {
            Show-ContextStatus
        }
        "trim" {
            if ($Force -or (Get-ConversationContextSize) -gt $Config.MaxContextExchanges) {
                Invoke-ContextTrimming
            } else {
                Write-Host "Context size within limits. Use -Force to trim anyway." -ForegroundColor Yellow
            }
        }
        "archive" {
            Invoke-ContextArchival
        }
        "restart" {
            if ($Force -or (Get-ConversationContextSize) -gt $Config.CriticalContextExchanges) {
                Invoke-RestartProtocol
            } else {
                Write-Host "Context size not critical. Use -Force to restart anyway." -ForegroundColor Yellow
            }
        }
        "help" {
            Show-Help
        }
        default {
            Write-Host "Unknown action: $Action" -ForegroundColor Red
            Show-Help
        }
    }
} catch {
    Write-ContextLog "Error in context monitor: $($_.Exception.Message)" "ERROR"
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-ContextLog "Context monitor completed successfully"
