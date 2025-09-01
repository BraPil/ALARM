# ALARM Process Monitoring Script
# Enhanced monitoring for ADDS Training Data Generation with comprehensive logging

param(
    [int]$IntervalSeconds = 10,
    [string]$LogFile = "process_monitor.log",
    [int]$MaxIterations = 60  # 10 minutes max
)

$StartTime = Get-Date
$ProcessName = "dotnet"
$DatabaseFile = "suggestion_validation.db"
$Iteration = 0

Write-Host "🚀 ALARM Process Monitor Started" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Start Time: $StartTime"
Write-Host "Monitoring: $ProcessName processes"
Write-Host "Database File: $DatabaseFile"
Write-Host "Log File: $LogFile"
Write-Host "Check Interval: $IntervalSeconds seconds"
Write-Host "Max Runtime: $($MaxIterations * $IntervalSeconds) seconds"
Write-Host ""

# Initialize log file
$LogHeader = @"
ALARM Process Monitor Log
========================
Start Time: $StartTime
Process: $ProcessName
Database: $DatabaseFile
Interval: $IntervalSeconds seconds

Timestamp,Iteration,ProcessFound,ProcessId,CPU_Seconds,Memory_MB,Handles,DatabaseExists,DatabaseSize_KB,Status,Notes
"@

$LogHeader | Out-File -FilePath $LogFile -Encoding UTF8

while ($Iteration -lt $MaxIterations) {
    $Iteration++
    $CurrentTime = Get-Date
    $ElapsedMinutes = [math]::Round(($CurrentTime - $StartTime).TotalMinutes, 2)
    
    Write-Host "[$($CurrentTime.ToString('HH:mm:ss'))] Iteration $Iteration (Elapsed: $ElapsedMinutes min)" -ForegroundColor Yellow
    
    # Check for dotnet processes
    $DotnetProcesses = Get-Process | Where-Object {$_.ProcessName -like "*$ProcessName*"}
    
    $ProcessFound = $false
    $ProcessId = "N/A"
    $CpuSeconds = 0
    $MemoryMB = 0
    $Handles = 0
    $Status = "No Process"
    $Notes = ""
    
    if ($DotnetProcesses) {
        $Process = $DotnetProcesses[0]  # Take first if multiple
        $ProcessFound = $true
        $ProcessId = $Process.Id
        $CpuSeconds = [math]::Round($Process.CPU, 2)
        $MemoryMB = [math]::Round($Process.WorkingSet / 1MB, 2)
        $Handles = $Process.Handles
        
        # Determine process status
        if ($Iteration -eq 1) {
            $Script:LastCpuSeconds = $CpuSeconds
            $Status = "Initial"
        } else {
            $CpuDelta = $CpuSeconds - $Script:LastCpuSeconds
            if ($CpuDelta -gt 0.1) {
                $Status = "Active"
                $Notes = "CPU +$([math]::Round($CpuDelta, 2))s"
            } elseif ($CpuDelta -eq 0) {
                $Status = "Idle/Stuck"
                $Notes = "No CPU change"
            } else {
                $Status = "Active"
                $Notes = "CPU activity detected"
            }
            $Script:LastCpuSeconds = $CpuSeconds
        }
        
        Write-Host "  Process: PID $ProcessId, CPU: ${CpuSeconds}s, Memory: ${MemoryMB}MB, Status: $Status" -ForegroundColor Green
        if ($Notes) { Write-Host "  Notes: $Notes" -ForegroundColor Cyan }
    } else {
        Write-Host "  No $ProcessName processes found" -ForegroundColor Red
    }
    
    # Check database file
    $DatabaseExists = Test-Path $DatabaseFile
    $DatabaseSizeKB = 0
    
    if ($DatabaseExists) {
        $DbFile = Get-Item $DatabaseFile
        $DatabaseSizeKB = [math]::Round($DbFile.Length / 1KB, 2)
        Write-Host "  Database: EXISTS, Size: ${DatabaseSizeKB}KB, Modified: $($DbFile.LastWriteTime.ToString('HH:mm:ss'))" -ForegroundColor Green
        
        if ($Status -eq "Idle/Stuck" -and $DatabaseSizeKB -gt 0) {
            $Status = "Processing"
            $Notes += " | DB activity"
        }
    } else {
        Write-Host "  Database: NOT FOUND" -ForegroundColor Red
    }
    
    # Check for console output files
    $ConsoleOutput = Get-ChildItem . -Filter "*.log" -ErrorAction SilentlyContinue | Where-Object {$_.LastWriteTime -gt $StartTime}
    if ($ConsoleOutput) {
        $Notes += " | Log files updated"
        Write-Host "  Log Activity: $($ConsoleOutput.Count) files updated" -ForegroundColor Magenta
    }
    
    # Log to file
    $LogEntry = "$($CurrentTime.ToString('yyyy-MM-dd HH:mm:ss')),$Iteration,$ProcessFound,$ProcessId,$CpuSeconds,$MemoryMB,$Handles,$DatabaseExists,$DatabaseSizeKB,$Status,`"$Notes`""
    $LogEntry | Out-File -FilePath $LogFile -Append -Encoding UTF8
    
    # Status assessment
    $StuckThresholdMinutes = 3
    if ($ProcessFound -and $Status -eq "Idle/Stuck" -and $ElapsedMinutes -gt $StuckThresholdMinutes -and -not $DatabaseExists) {
        Write-Host ""
        Write-Host "⚠️  PROCESS APPEARS STUCK!" -ForegroundColor Red
        Write-Host "   - No CPU activity for multiple checks" -ForegroundColor Red
        Write-Host "   - No database file created after $ElapsedMinutes minutes" -ForegroundColor Red
        Write-Host "   - Recommend interrupting process PID $ProcessId" -ForegroundColor Red
        Write-Host ""
        
        $LogEntry = "$($CurrentTime.ToString('yyyy-MM-dd HH:mm:ss')),ALERT,STUCK_PROCESS,$ProcessId,$CpuSeconds,$MemoryMB,$Handles,$DatabaseExists,$DatabaseSizeKB,STUCK,Process appears hung - recommend interrupt"
        $LogEntry | Out-File -FilePath $LogFile -Append -Encoding UTF8
        
        break
    }
    
    # Success check
    if ($DatabaseExists -and $DatabaseSizeKB -gt 10) {
        Write-Host ""
        Write-Host "✅ PROGRESS DETECTED!" -ForegroundColor Green
        Write-Host "   - Database file created and growing" -ForegroundColor Green
        Write-Host "   - Process appears to be working normally" -ForegroundColor Green
        Write-Host ""
        
        # Continue monitoring but with longer intervals for successful processes
        $IntervalSeconds = 30
    }
    
    Write-Host ""
    
    if ($Iteration -lt $MaxIterations) {
        Start-Sleep -Seconds $IntervalSeconds
    }
}

$EndTime = Get-Date
$TotalMinutes = [math]::Round(($EndTime - $StartTime).TotalMinutes, 2)

Write-Host "🏁 ALARM Process Monitor Completed" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host "End Time: $EndTime"
Write-Host "Total Runtime: $TotalMinutes minutes"
Write-Host "Log File: $LogFile"

# Final log entry
$FinalLogEntry = "$($EndTime.ToString('yyyy-MM-dd HH:mm:ss')),FINAL,MONITOR_END,N/A,N/A,N/A,N/A,N/A,N/A,COMPLETED,Monitor completed after $TotalMinutes minutes"
$FinalLogEntry | Out-File -FilePath $LogFile -Append -Encoding UTF8

