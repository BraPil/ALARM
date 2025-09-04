# SIMPLE PERFORMANCE OPTIMIZER SCRIPT
# Basic performance optimization for ALARM system

param(
    [string]$Action = "optimize"
)

function Write-Log {
    param([string]$Message)
    $Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$Timestamp] $Message"
}

function Get-MemoryUsage {
    try {
        $Process = Get-Process -Name "powershell" -ErrorAction SilentlyContinue
        if ($Process) {
            $MemoryBytes = $Process.WorkingSet64
            # Convert bytes to MB using multiplication instead of division
            $MemoryMB = $MemoryBytes * 0.00000095367431640625
            return $MemoryMB
        }
    } catch {
        Write-Log "Error getting memory usage: $($_.Exception.Message)"
    }
    return 0
}

function Invoke-BasicCleanup {
    Write-Log "Starting basic cleanup"
    
    # Clear PowerShell variables
    try {
        Get-Variable | Where-Object { $_.Name -notlike "*PS*" -and $_.Name -notin @("Action", "Verbose", "Force") } | Remove-Variable -ErrorAction SilentlyContinue
        Write-Log "Cleared PowerShell variables"
    } catch {
        Write-Log "Error clearing variables: $($_.Exception.Message)"
    }
    
    # Clear PowerShell history if large
    try {
        $HistoryCount = (Get-History).Count
        if ($HistoryCount -gt 1000) {
            Clear-History
            Write-Log "Cleared PowerShell history ($HistoryCount entries)"
        }
    } catch {
        Write-Log "Error clearing history: $($_.Exception.Message)"
    }
    
    # Force garbage collection
    try {
        [System.GC]::Collect()
        [System.GC]::WaitForPendingFinalizers()
        [System.GC]::Collect()
        Write-Log "Forced garbage collection"
    } catch {
        Write-Log "Error during garbage collection: $($_.Exception.Message)"
    }
    
    Write-Log "Basic cleanup completed"
}

function Show-PerformanceStatus {
    $MemoryUsage = Get-MemoryUsage
    
    Write-Host "=== PERFORMANCE STATUS ===" -ForegroundColor Cyan
    Write-Host "Memory Usage: $MemoryUsage MB" -ForegroundColor White
    
    if ($MemoryUsage -gt 200) {
        Write-Host "WARNING: Memory usage exceeds 200MB threshold" -ForegroundColor Red
    } elseif ($MemoryUsage -gt 150) {
        Write-Host "WARNING: Memory usage approaching 200MB threshold" -ForegroundColor Yellow
    } else {
        Write-Host "Memory usage within acceptable limits" -ForegroundColor Green
    }
}

function Invoke-PerformanceOptimization {
    Write-Log "Starting performance optimization"
    
    $StartTime = Get-Date
    
    # Basic cleanup
    Invoke-BasicCleanup
    
    # Check performance
    Show-PerformanceStatus
    
    $EndTime = Get-Date
    $OptimizationTime = ($EndTime - $StartTime).TotalMilliseconds
    
    Write-Log "Performance optimization completed in $OptimizationTime ms"
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
            Invoke-BasicCleanup
        }
        default {
            Write-Host "Unknown action: $Action" -ForegroundColor Red
            Write-Host "Valid actions: optimize, status, cleanup" -ForegroundColor Yellow
        }
    }
} catch {
    Write-Log "Error in performance optimization: $($_.Exception.Message)"
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Log "Performance optimizer completed successfully"
