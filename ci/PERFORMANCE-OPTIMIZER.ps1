# PERFORMANCE OPTIMIZER SCRIPT
# Implements Performance Optimization Protocol for ALARM system

param(
    [string]$Action = "optimize",
    [switch]$Verbose,
    [switch]$Force
)

# Configuration
$Config = @{
    MaxContextExchanges = 15
    MaxMemoryMB = 200
    MaxCacheMB = 100
    MaxResponseTimeSeconds = 5
    CleanupFrequency = 5
    CacheRetentionHours = 24
}

# Performance tracking
$PerformanceMetrics = @{
    ResponseCount = 0
    LastCleanup = Get-Date
    CacheHits = 0
    CacheMisses = 0
    TotalResponseTime = 0
    AverageResponseTime = 0
}

function Write-PerformanceLog {
    param([string]$Message, [string]$Level = "INFO")
    $Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $LogMessage = "[$Timestamp] [$Level] $Message"
    
    if ($Verbose) {
        Write-Host $LogMessage
    }
    
    # Log to file
    $LogFile = "mcp_runs\performance_logs\performance-$(Get-Date -Format 'yyyy-MM-dd').log"
    $LogDir = Split-Path $LogFile -Parent
    if (-not (Test-Path $LogDir)) {
        New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
    }
    Add-Content -Path $LogFile -Value $LogMessage
}

function Get-MemoryUsage {
    $Process = Get-Process -Name "powershell" -ErrorAction SilentlyContinue
    if ($Process) {
        $MemoryMB = [math]::Round($Process.WorkingSet64 / 1048576, 2)
        return $MemoryMB
    }
    return 0
}

function Test-PerformanceThresholds {
    $MemoryUsage = Get-MemoryUsage
    $ResponseTime = $PerformanceMetrics.AverageResponseTime
    
    $Alerts = @()
    
    # Critical alerts
    if ($MemoryUsage -gt ($Config.MaxMemoryMB * 0.9)) {
        $Alerts += "CRITICAL: Memory usage $MemoryUsage MB exceeds 90% threshold"
    }
    
    if ($ResponseTime -gt 10) {
        $Alerts += "CRITICAL: Response time $ResponseTime seconds exceeds 10 second threshold"
    }
    
    # Warning alerts
    if ($MemoryUsage -gt ($Config.MaxMemoryMB * 0.8)) {
        $Alerts += "WARNING: Memory usage $MemoryUsage MB exceeds 80% threshold"
    }
    
    if ($ResponseTime -gt $Config.MaxResponseTimeSeconds) {
        $Alerts += "WARNING: Response time $ResponseTime seconds exceeds $($Config.MaxResponseTimeSeconds) second threshold"
    }
    
    return $Alerts
}

function Invoke-ContextTrimming {
    Write-PerformanceLog "Starting context trimming"
    
    # Check for conversation archives directory
    $ArchiveDir = "mcp_runs\conversation_archives"
    if (-not (Test-Path $ArchiveDir)) {
        New-Item -ItemType Directory -Path $ArchiveDir -Force | Out-Null
    }
    
    # Archive old conversation files if they exist
    $ConversationFiles = Get-ChildItem -Path "mcp_runs" -Filter "*conversation*" -Recurse -ErrorAction SilentlyContinue
    foreach ($File in $ConversationFiles) {
        if ($File.LastWriteTime -lt (Get-Date).AddDays(-1)) {
            $ArchivePath = Join-Path $ArchiveDir $File.Name
            Move-Item -Path $File.FullName -Destination $ArchivePath -Force
            Write-PerformanceLog "Archived conversation file: $($File.Name)"
        }
    }
    
    Write-PerformanceLog "Context trimming completed"
}

function Invoke-MemoryCleanup {
    Write-PerformanceLog "Starting memory cleanup"
    
    # Clear PowerShell variables (except essential ones)
    $EssentialVars = @("Config", "PerformanceMetrics", "Action", "Verbose", "Force")
    Get-Variable | Where-Object { $_.Name -notin $EssentialVars -and $_.Name -notlike "*PS*" } | Remove-Variable -ErrorAction SilentlyContinue
    
    # Clear PowerShell history if it's getting large
    $HistoryCount = (Get-History).Count
    if ($HistoryCount -gt 1000) {
        Clear-History
        Write-PerformanceLog "Cleared PowerShell history ($HistoryCount entries)"
    }
    
    # Force garbage collection
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
    [System.GC]::Collect()
    
    Write-PerformanceLog "Memory cleanup completed"
}

function Invoke-FileCacheCleanup {
    Write-PerformanceLog "Starting file cache cleanup"
    
    # Clean up temporary files
    $TempDirs = @("mcp_runs\temp", "mcp_runs\cache", "mcp_runs\logs")
    foreach ($TempDir in $TempDirs) {
        if (Test-Path $TempDir) {
            $OldFiles = Get-ChildItem -Path $TempDir -Recurse | Where-Object { 
                $_.LastWriteTime -lt (Get-Date).AddHours(-$Config.CacheRetentionHours) 
            }
            
            foreach ($File in $OldFiles) {
                Remove-Item -Path $File.FullName -Force -ErrorAction SilentlyContinue
                Write-PerformanceLog "Removed old file: $($File.Name)"
            }
        }
    }
    
    Write-PerformanceLog "File cache cleanup completed"
}

function Update-PerformanceMetrics {
    param(
        [double]$ResponseTime,
        [int]$CacheHits = 0,
        [int]$CacheMisses = 0
    )
    
    $PerformanceMetrics.ResponseCount++
    $PerformanceMetrics.TotalResponseTime += $ResponseTime
    if ($PerformanceMetrics.ResponseCount -gt 0) {
        $PerformanceMetrics.AverageResponseTime = $PerformanceMetrics.TotalResponseTime / $PerformanceMetrics.ResponseCount
    } else {
        $PerformanceMetrics.AverageResponseTime = 0
    }
    $PerformanceMetrics.CacheHits += $CacheHits
    $PerformanceMetrics.CacheMisses += $CacheMisses
    
    # Save metrics to file
    $MetricsFile = "mcp_runs\performance_logs\metrics.json"
    $MetricsDir = Split-Path $MetricsFile -Parent
    if (-not (Test-Path $MetricsDir)) {
        New-Item -ItemType Directory -Path $MetricsDir -Force | Out-Null
    }
    
    $PerformanceMetrics | ConvertTo-Json | Set-Content -Path $MetricsFile
}

function Invoke-PerformanceOptimization {
    Write-PerformanceLog "Starting performance optimization"
    
    $StartTime = Get-Date
    
    # Check if cleanup is needed
    $TimeSinceLastCleanup = (Get-Date) - $PerformanceMetrics.LastCleanup
    $NeedsCleanup = $TimeSinceLastCleanup.TotalMinutes -gt 5 -or $PerformanceMetrics.ResponseCount % $Config.CleanupFrequency -eq 0
    
    if ($NeedsCleanup -or $Force) {
        Invoke-ContextTrimming
        Invoke-MemoryCleanup
        Invoke-FileCacheCleanup
        $PerformanceMetrics.LastCleanup = Get-Date
    }
    
    # Check performance thresholds
    $Alerts = Test-PerformanceThresholds
    foreach ($Alert in $Alerts) {
        Write-PerformanceLog $Alert "WARNING"
    }
    
    $EndTime = Get-Date
    $OptimizationTime = ($EndTime - $StartTime).TotalMilliseconds
    
    Write-PerformanceLog "Performance optimization completed in $OptimizationTime ms"
    
    # Update metrics
    Update-PerformanceMetrics -ResponseTime $OptimizationTime
}

function Show-PerformanceStatus {
    $MemoryUsage = Get-MemoryUsage
    $TotalCacheOperations = [int]$PerformanceMetrics.CacheHits + [int]$PerformanceMetrics.CacheMisses
    $CacheHitRate = if ($TotalCacheOperations -gt 0) {
        $HitRate = [double]$PerformanceMetrics.CacheHits / [double]$TotalCacheOperations
        [math]::Round($HitRate * 100, 2)
    } else { 0 }
    
    Write-Host "=== PERFORMANCE STATUS ===" -ForegroundColor Cyan
    Write-Host "Memory Usage: $MemoryUsage MB" -ForegroundColor White
    Write-Host "Average Response Time: $([math]::Round($PerformanceMetrics.AverageResponseTime, 2)) seconds" -ForegroundColor White
    Write-Host "Total Responses: $($PerformanceMetrics.ResponseCount)" -ForegroundColor White
    Write-Host "Cache Hit Rate: $CacheHitRate%" -ForegroundColor White
    Write-Host "Last Cleanup: $($PerformanceMetrics.LastCleanup)" -ForegroundColor White
    
    $Alerts = Test-PerformanceThresholds
    if ($Alerts.Count -gt 0) {
        Write-Host "`n=== PERFORMANCE ALERTS ===" -ForegroundColor Red
        foreach ($Alert in $Alerts) {
            Write-Host $Alert -ForegroundColor Red
        }
    }
}

function Show-Help {
    Write-Host "PERFORMANCE OPTIMIZER SCRIPT" -ForegroundColor Cyan
    Write-Host "Usage: .\PERFORMANCE-OPTIMIZER.ps1 [Action] [Options]" -ForegroundColor White
    Write-Host ""
    Write-Host "Actions:" -ForegroundColor Yellow
    Write-Host "  optimize    - Run performance optimization (default)" -ForegroundColor White
    Write-Host "  status      - Show current performance status" -ForegroundColor White
    Write-Host "  cleanup     - Force immediate cleanup" -ForegroundColor White
    Write-Host "  help        - Show this help message" -ForegroundColor White
    Write-Host ""
    Write-Host "Options:" -ForegroundColor Yellow
    Write-Host "  -Verbose    - Show detailed logging" -ForegroundColor White
    Write-Host "  -Force      - Force cleanup regardless of timing" -ForegroundColor White
    Write-Host ""
    Write-Host "Examples:" -ForegroundColor Yellow
    Write-Host "  .\PERFORMANCE-OPTIMIZER.ps1 optimize -Verbose" -ForegroundColor White
    Write-Host "  .\PERFORMANCE-OPTIMIZER.ps1 status" -ForegroundColor White
    Write-Host "  .\PERFORMANCE-OPTIMIZER.ps1 cleanup -Force" -ForegroundColor White
}

# Main execution
try {
    switch ($Action.ToLower()) {
        "optimize" {
            Invoke-PerformanceOptimization
        }
        "status" {
            Show-PerformanceStatus
        }
        "cleanup" {
            Invoke-PerformanceOptimization -Force
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
    Write-PerformanceLog "Error in performance optimization: $($_.Exception.Message)" "ERROR"
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-PerformanceLog "Performance optimizer completed successfully"
