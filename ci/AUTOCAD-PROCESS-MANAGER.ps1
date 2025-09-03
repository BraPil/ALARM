# ADDS25 AutoCAD Process Management System
# Purpose: Monitor, manage, and cleanup AutoCAD processes for CI testing
# Environment: Test Computer (wa-bdpilegg)
# Date: September 3, 2025

Write-Host "*** ADDS25 AutoCAD Process Manager ***" -ForegroundColor Cyan
Write-Host "Purpose: Monitor and manage AutoCAD processes for CI testing" -ForegroundColor Yellow
Write-Host ""

# Configuration
$logDir = "C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results"
$monitorInterval = 2 # seconds
$idleTimeout = 10 # seconds
$maxMonitorTime = 300 # 5 minutes maximum monitoring

function Write-AutoCADLog {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "HH:mm:ss"
    $logEntry = "[$timestamp] [$Level] $Message"
    Write-Host $logEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } })
    
    # Also log to file
    $logFile = "$logDir\autocad-monitor-$(Get-Date -Format 'yyyy-MM-dd').log"
    Add-Content $logFile $logEntry -Encoding UTF8
}

function Get-AutoCADProcesses {
    return Get-Process -Name "acad" -ErrorAction SilentlyContinue
}

function Close-AutoCADProcesses {
    param([switch]$Force)
    
    $processes = Get-AutoCADProcesses
    if (!$processes) {
        Write-AutoCADLog "No AutoCAD processes found to close" "INFO"
        return $true
    }
    
    Write-AutoCADLog "Found $($processes.Count) AutoCAD process(es) to close" "INFO"
    
    foreach ($process in $processes) {
        try {
            Write-AutoCADLog "Closing AutoCAD process PID: $($process.Id)" "INFO"
            
            if ($Force) {
                $process.Kill()
                Write-AutoCADLog "Force killed AutoCAD process PID: $($process.Id)" "WARNING"
            } else {
                $process.CloseMainWindow()
                Write-AutoCADLog "Sent close signal to AutoCAD process PID: $($process.Id)" "INFO"
            }
        } catch {
            Write-AutoCADLog "Error closing AutoCAD process PID $($process.Id): $($_.Exception.Message)" "ERROR"
        }
    }
    
    # Wait for processes to close
    Start-Sleep -Seconds 3
    
    # Verify closure
    $remainingProcesses = Get-AutoCADProcesses
    if ($remainingProcesses) {
        Write-AutoCADLog "$($remainingProcesses.Count) AutoCAD process(es) still running" "WARNING"
        return $false
    } else {
        Write-AutoCADLog "All AutoCAD processes closed successfully" "SUCCESS"
        return $true
    }
}

function Monitor-AutoCADActivity {
    param([int]$TimeoutSeconds = 10)
    
    Write-AutoCADLog "Starting AutoCAD activity monitoring..." "INFO"
    Write-AutoCADLog "Idle timeout: $TimeoutSeconds seconds" "INFO"
    
    $startTime = Get-Date
    $lastActivity = Get-Date
    $previousCpuTime = 0
    
    while ((Get-Date) -lt $startTime.AddSeconds($maxMonitorTime)) {
        $process = Get-AutoCADProcesses | Select-Object -First 1
        
        if (!$process) {
            Write-AutoCADLog "AutoCAD process no longer running" "INFO"
            break
        }
        
        try {
            # Check CPU activity
            $currentCpuTime = $process.TotalProcessorTime.TotalMilliseconds
            
            if ($currentCpuTime -ne $previousCpuTime) {
                $lastActivity = Get-Date
                Write-AutoCADLog "AutoCAD activity detected (PID: $($process.Id), CPU: $([math]::Round($currentCpuTime, 2))ms)" "INFO"
            }
            
            $previousCpuTime = $currentCpuTime
            
            # Check for idle timeout
            $idleTime = ((Get-Date) - $lastActivity).TotalSeconds
            if ($idleTime -ge $TimeoutSeconds) {
                Write-AutoCADLog "AutoCAD idle for $([math]::Round($idleTime, 1)) seconds - closing process" "WARNING"
                Close-AutoCADProcesses -Force
                break
            }
            
            # Log current status every 30 seconds
            if (((Get-Date) - $startTime).TotalSeconds % 30 -lt $monitorInterval) {
                $runTime = [math]::Round(((Get-Date) - $startTime).TotalSeconds, 1)
                $idle = [math]::Round($idleTime, 1)
                Write-AutoCADLog "AutoCAD running - Runtime: ${runTime}s, Idle: ${idle}s" "INFO"
            }
            
        } catch {
            Write-AutoCADLog "Error monitoring AutoCAD: $($_.Exception.Message)" "ERROR"
        }
        
        Start-Sleep -Seconds $monitorInterval
    }
    
    Write-AutoCADLog "AutoCAD monitoring completed" "SUCCESS"
}

function Ensure-AutoCADClosed {
    Write-AutoCADLog "Ensuring AutoCAD is completely closed before test..." "INFO"
    
    # First attempt: graceful close
    $closed = Close-AutoCADProcesses
    
    if (!$closed) {
        Write-AutoCADLog "Graceful close failed, forcing closure..." "WARNING"
        Start-Sleep -Seconds 2
        Close-AutoCADProcesses -Force
    }
    
    # Final verification
    Start-Sleep -Seconds 2
    $finalCheck = Get-AutoCADProcesses
    if ($finalCheck) {
        Write-AutoCADLog "WARNING: $($finalCheck.Count) AutoCAD process(es) still running after cleanup" "ERROR"
        return $false
    } else {
        Write-AutoCADLog "AutoCAD cleanup successful - ready for new test" "SUCCESS"
        return $true
    }
}

# Export functions for use in other scripts
Export-ModuleMember -Function Get-AutoCADProcesses, Close-AutoCADProcesses, Monitor-AutoCADActivity, Ensure-AutoCADClosed

# If script is run directly, show usage
if ($MyInvocation.InvocationName -ne '.') {
    Write-Host ""
    Write-Host "AutoCAD Process Manager Functions:" -ForegroundColor Green
    Write-Host "  Get-AutoCADProcesses     - List all AutoCAD processes" -ForegroundColor White
    Write-Host "  Close-AutoCADProcesses   - Close all AutoCAD processes" -ForegroundColor White
    Write-Host "  Monitor-AutoCADActivity  - Monitor AutoCAD and auto-close when idle" -ForegroundColor White
    Write-Host "  Ensure-AutoCADClosed     - Ensure AutoCAD is closed before testing" -ForegroundColor White
    Write-Host ""
    Write-Host "Example usage in CI scripts:" -ForegroundColor Yellow
    Write-Host "  Import-Module .\AUTOCAD-PROCESS-MANAGER.ps1" -ForegroundColor Cyan
    Write-Host "  Ensure-AutoCADClosed" -ForegroundColor Cyan
    Write-Host ""
}
