# ADDS25 Enhanced Dev Computer Automated CI
# Purpose: Advanced CI system with file-based communication integration
# Environment: Dev Computer (kidsg) - Production-ready CI automation
# Date: September 3, 2025
# Version: 2.0 - Enhanced with file-based triggers, monitoring, and reliability

param(
    [string]$ConfigPath = ".\ci\config\ci-config.json",
    [int]$MonitorInterval = 5,
    [switch]$EnableFileBasedTriggers = $true,
    [switch]$EnablePerformanceMonitoring = $true,
    [switch]$EnableHealthChecks = $true,
    [switch]$Verbose = $false
)

# Configuration and Constants
$script:Config = @{
    MonitorInterval = $MonitorInterval
    EnableFileBasedTriggers = $EnableFileBasedTriggers
    EnablePerformanceMonitoring = $EnablePerformanceMonitoring
    EnableHealthChecks = $EnableHealthChecks
    Verbose = $Verbose
    MaxRetries = 3
    RetryDelay = 5
    LogRetentionDays = 30
    TriggerDirectory = "C:\Users\kidsg\Downloads\ALARM\triggers"
}

# File Paths
$script:RepoPath = "C:\Users\kidsg\Downloads\ALARM"
$script:LogPath = "$RepoPath\ci\logs\ci-system"
$script:PerformanceLogPath = "$LogPath\performance"
$script:HealthLogPath = "$LogPath\health"
$script:TriggerGeneratorPath = "$RepoPath\ci\GENERATE-TRIGGER.ps1"

# Performance Metrics
$script:PerformanceMetrics = @{
    TotalCommits = 0
    SuccessfulAnalysis = 0
    FailedAnalysis = 0
    AverageResponseTime = 0
    StartTime = Get-Date
    LastCommitTime = $null
    ProcessingTimes = @()
    TriggerFilesCreated = 0
}

# Health Status
$script:HealthStatus = @{
    IsHealthy = $true
    LastHealthCheck = Get-Date
    ErrorCount = 0
    WarningCount = 0
    SystemResources = @{
        DiskSpace = 0
        MemoryUsage = 0
        CPUUsage = 0
    }
}

# CI State
$script:CIState = @{
    LastCommit = ""
    LastCommitMessage = ""
    IsProcessing = $false
    CurrentAction = ""
    RetryCount = 0
}

# Initialize logging and directories
function Initialize-CISystem {
    Write-Host "*** ADDS25 Enhanced Dev Computer CI Starting ***" -ForegroundColor Cyan
    Write-Host "Version: 2.0 - Enhanced with file-based triggers, monitoring, and reliability" -ForegroundColor Yellow
    Write-Host ""
    
    # Create necessary directories
    $directories = @($LogPath, $PerformanceLogPath, $HealthLogPath, $script:Config.TriggerDirectory)
    foreach ($dir in $directories) {
        if (!(Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
            Write-Host "Created directory: $dir" -ForegroundColor Green
        }
    }
    
    # Load configuration if exists
    if (Test-Path $ConfigPath) {
        try {
            $script:Config = Get-Content $ConfigPath | ConvertFrom-Json | ConvertTo-Hashtable
            Write-Host "Configuration loaded from: $ConfigPath" -ForegroundColor Green
        } catch {
            Write-Host "Warning: Failed to load configuration, using defaults" -ForegroundColor Yellow
        }
    }
    
    # Initialize performance monitoring
    if ($script:Config.EnablePerformanceMonitoring) {
        Initialize-CIPerformanceMonitoring
    }
    
    # Initialize health monitoring
    if ($script:Config.EnableHealthChecks) {
        Initialize-CIHealthMonitoring
    }
    
    Write-Host "CI system initialized successfully" -ForegroundColor Green
    Write-Host "Repository: $RepoPath" -ForegroundColor Green
    Write-Host "Monitor Interval: $($script:Config.MonitorInterval) seconds" -ForegroundColor Green
    Write-Host "File-based Triggers: $($script:Config.EnableFileBasedTriggers)" -ForegroundColor Green
    Write-Host ""
}

# Enhanced logging function
function Write-CILog {
    param(
        [string]$Message,
        [string]$Level = "INFO",
        [string]$Component = "CISystem",
        [hashtable]$AdditionalData = @{}
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logEntry = @{
        Timestamp = $timestamp
        Level = $Level
        Component = $Component
        Message = $Message
        Data = $AdditionalData
    }
    
    # Console output with colors
    $color = switch($Level) {
        "ERROR" { "Red" }
        "SUCCESS" { "Green" }
        "WARNING" { "Yellow" }
        "DEBUG" { "Gray" }
        default { "White" }
    }
    
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $color
    
    # File logging
    $logFile = Join-Path $LogPath "ci-system-$(Get-Date -Format 'yyyy-MM-dd').log"
    $logJson = $logEntry | ConvertTo-Json -Depth 10
    Add-Content -Path $logFile -Value $logJson
    
    # Error logging to separate file
    if ($Level -eq "ERROR") {
        $errorLogFile = Join-Path $LogPath "ci-errors-$(Get-Date -Format 'yyyy-MM-dd').log"
        Add-Content -Path $errorLogFile -Value $logJson
    }
}

# Performance monitoring initialization
function Initialize-CIPerformanceMonitoring {
    $script:PerformanceMetrics.StartTime = Get-Date
    Write-CILog "CI Performance monitoring initialized" "INFO" "PerformanceMonitor"
    
    # Start background performance logging
    Start-Job -ScriptBlock {
        param($LogPath)
        while ($true) {
            $metrics = @{
                Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
                TotalCommits = $using:script:PerformanceMetrics.TotalCommits
                SuccessfulAnalysis = $using:script:PerformanceMetrics.SuccessfulAnalysis
                FailedAnalysis = $using:script:PerformanceMetrics.FailedAnalysis
                AverageResponseTime = $using:script:PerformanceMetrics.AverageResponseTime
                TriggerFilesCreated = $using:script:PerformanceMetrics.TriggerFilesCreated
                Uptime = (Get-Date) - $using:script:PerformanceMetrics.StartTime
            }
            
            $logFile = Join-Path $LogPath "ci-performance-$(Get-Date -Format 'yyyy-MM-dd').log"
            $metricsJson = $metrics | ConvertTo-Json
            Add-Content -Path $logFile -Value $metricsJson
            
            Start-Sleep -Seconds 60
        }
    } -ArgumentList $PerformanceLogPath | Out-Null
}

# Health monitoring initialization
function Initialize-CIHealthMonitoring {
    Write-CILog "CI Health monitoring initialized" "INFO" "HealthMonitor"
    
    # Start background health checking
    Start-Job -ScriptBlock {
        param($LogPath, $RepoPath)
        while ($true) {
            try {
                # Check disk space
                $disk = Get-WmiObject -Class Win32_LogicalDisk -Filter "DeviceID='C:'"
                $diskSpace = [math]::Round(($disk.FreeSpace / $disk.Size) * 100, 2)
                
                # Check memory usage
                $memory = Get-WmiObject -Class Win32_OperatingSystem
                $memoryUsage = [math]::Round((($memory.TotalVisibleMemorySize - $memory.FreePhysicalMemory) / $memory.TotalVisibleMemorySize) * 100, 2)
                
                # Check CPU usage (simplified)
                $cpuUsage = (Get-Counter "\Processor(_Total)\% Processor Time").CounterSamples.CookedValue
                
                $health = @{
                    Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
                    DiskSpace = $diskSpace
                    MemoryUsage = $memoryUsage
                    CPUUsage = [math]::Round($cpuUsage, 2)
                    IsHealthy = ($diskSpace -gt 10 -and $memoryUsage -lt 90 -and $cpuUsage -lt 90)
                }
                
                $logFile = Join-Path $LogPath "ci-health-$(Get-Date -Format 'yyyy-MM-dd').log"
                $healthJson = $health | ConvertTo-Json
                Add-Content -Path $logFile -Value $healthJson
                
                Start-Sleep -Seconds 30
                
            } catch {
                Start-Sleep -Seconds 30
            }
        }
    } -ArgumentList $HealthLogPath, $RepoPath | Out-Null
}

# Create file-based trigger
function Create-FileBasedTrigger {
    param(
        [string]$CommitHash,
        [string]$CommitMessage,
        [string]$Action = "analyze_test_results",
        [string]$Priority = "normal",
        [string]$Source = "dev_computer"
    )
    
    try {
        Write-CILog "Creating file-based trigger for commit: $CommitHash" "INFO" "TriggerGenerator"
        
        # Build trigger generator command
        $triggerArgs = @(
            "-Action", $Action,
            "-CommitHash", $CommitHash,
            "-CommitMessage", $CommitMessage,
            "-Priority", $Priority,
            "-Source", $Source
        )
        
        if ($Verbose) {
            $triggerArgs += "-Verbose"
        }
        
        # Execute trigger generator
        $result = & $TriggerGeneratorPath @triggerArgs
        
        if ($LASTEXITCODE -eq 0) {
            $script:PerformanceMetrics.TriggerFilesCreated++
            Write-CILog "File-based trigger created successfully" "SUCCESS" "TriggerGenerator" @{
                Commit = $CommitHash
                Action = $Action
                Priority = $Priority
                Source = $Source
            }
            return $true
        } else {
            Write-CILog "Failed to create file-based trigger" "ERROR" "TriggerGenerator" @{
                ExitCode = $LASTEXITCODE
                Commit = $CommitHash
            }
            return $false
        }
        
    } catch {
        Write-CILog "Exception creating file-based trigger: $($_.Exception.Message)" "ERROR" "TriggerGenerator"
        return $false
    }
}

# Process new commit with file-based triggers
function Process-NewCommit {
    param(
        [string]$CommitHash,
        [string]$CommitMessage
    )
    
    $startTime = Get-Date
    $script:PerformanceMetrics.TotalCommits++
    $script:CIState.IsProcessing = $true
    $script:CIState.CurrentAction = "processing_commit"
    
    try {
        Write-CILog "Processing new commit: $CommitHash" "INFO" "CommitProcessor" @{
            Commit = $CommitHash
            Message = $CommitMessage
        }
        
        # Determine action based on commit message
        $action = Determine-CommitAction -CommitMessage $CommitMessage
        $priority = Determine-CommitPriority -CommitMessage $CommitMessage
        
        Write-CILog "Determined action: $action, priority: $priority" "INFO" "CommitProcessor"
        
        # Create file-based trigger
        if ($script:Config.EnableFileBasedTriggers) {
            $triggerResult = Create-FileBasedTrigger -CommitHash $CommitHash -CommitMessage $CommitMessage -Action $action -Priority $priority
            
            if ($triggerResult) {
                $script:PerformanceMetrics.SuccessfulAnalysis++
                $script:CIState.LastCommit = $CommitHash
                $script:CIState.LastCommitMessage = $CommitMessage
                
                # Update performance metrics
                $processingTime = (Get-Date) - $startTime
                $script:PerformanceMetrics.ProcessingTimes += $processingTime.TotalMilliseconds
                $script:PerformanceMetrics.AverageResponseTime = $script:PerformanceMetrics.ProcessingTimes | Measure-Object -Average | Select-Object -ExpandProperty Average
                $script:PerformanceMetrics.LastCommitTime = Get-Date
                
                Write-CILog "Commit processed successfully in $([math]::Round($processingTime.TotalMilliseconds, 2))ms" "SUCCESS" "CommitProcessor" @{
                    ProcessingTime = $processingTime.TotalMilliseconds
                    Action = $action
                    Priority = $priority
                }
                
                return $true
            } else {
                throw "Failed to create file-based trigger"
            }
        } else {
            Write-CILog "File-based triggers disabled, skipping trigger creation" "WARNING" "CommitProcessor"
            return $false
        }
        
    } catch {
        $script:PerformanceMetrics.FailedAnalysis++
        Write-CILog "Failed to process commit: $($_.Exception.Message)" "ERROR" "CommitProcessor" @{
            Error = $_.Exception.Message
            StackTrace = $_.ScriptStackTrace
            Commit = $CommitHash
        }
        return $false
    } finally {
        $script:CIState.IsProcessing = $false
        $script:CIState.CurrentAction = ""
    }
}

# Determine commit action based on message
function Determine-CommitAction {
    param([string]$CommitMessage)
    
    $message = $CommitMessage.ToLower()
    
    if ($message -match "test|result|analysis") {
        return "analyze_test_results"
    } elseif ($message -match "fix|deploy|update") {
        return "deploy_fixes"
    } elseif ($message -match "run.*test|execute.*test") {
        return "run_tests"
    } else {
        return "analyze_test_results" # Default action
    }
}

# Determine commit priority based on message
function Determine-CommitPriority {
    param([string]$CommitMessage)
    
    $message = $CommitMessage.ToLower()
    
    if ($message -match "critical|urgent|emergency|hotfix") {
        return "high"
    } elseif ($message -match "important|priority") {
        return "high"
    } else {
        return "normal" # Default priority
    }
}

# Monitor GitHub for new commits
function Start-GitHubMonitoring {
    Write-CILog "Starting GitHub monitoring for new commits..." "INFO" "GitHubMonitor"
    
    $lastCommit = ""
    
    while ($true) {
        try {
            # Pull latest changes
            Set-Location $script:RepoPath
            $pullResult = git pull origin main --quiet 2>&1
            
            if ($LASTEXITCODE -ne 0) {
                Write-CILog "Git pull failed: $pullResult" "ERROR" "GitHubMonitor"
                Start-Sleep -Seconds $script:Config.MonitorInterval
                continue
            }
            
            # Check for new commits
            $currentCommit = git rev-parse HEAD
            
            if ($currentCommit -ne $lastCommit) {
                Write-CILog "New commit detected: $currentCommit" "INFO" "GitHubMonitor"
                
                # Get commit message
                $commitMessage = git log -1 --pretty=format:"%s"
                
                # Process the new commit
                $result = Process-NewCommit -CommitHash $currentCommit -CommitMessage $commitMessage
                
                if ($result) {
                    $lastCommit = $currentCommit
                    Write-CILog "Commit processing completed successfully" "SUCCESS" "GitHubMonitor"
                } else {
                    Write-CILog "Commit processing failed" "ERROR" "GitHubMonitor"
                }
            }
            
            # Health check
            if ($script:Config.EnableHealthChecks) {
                $script:HealthStatus.LastHealthCheck = Get-Date
            }
            
            Start-Sleep -Seconds $script:Config.MonitorInterval
            
        } catch {
            Write-CILog "Error in GitHub monitoring: $($_.Exception.Message)" "ERROR" "GitHubMonitor"
            Start-Sleep -Seconds $script:Config.MonitorInterval
        }
    }
}

# Cleanup function
function Stop-CISystem {
    Write-CILog "Stopping CI system..." "INFO" "CISystem"
    
    # Stop background jobs
    Get-Job | Stop-Job
    Get-Job | Remove-Job
    
    # Final performance report
    if ($script:Config.EnablePerformanceMonitoring) {
        $uptime = (Get-Date) - $script:PerformanceMetrics.StartTime
        Write-Host ""
        Write-Host "📊 CI PERFORMANCE SUMMARY:" -ForegroundColor Cyan
        Write-Host "  Total Commits: $($script:PerformanceMetrics.TotalCommits)" -ForegroundColor White
        Write-Host "  Successful Analysis: $($script:PerformanceMetrics.SuccessfulAnalysis)" -ForegroundColor Green
        Write-Host "  Failed Analysis: $($script:PerformanceMetrics.FailedAnalysis)" -ForegroundColor Red
        Write-Host "  Trigger Files Created: $($script:PerformanceMetrics.TriggerFilesCreated)" -ForegroundColor White
        Write-Host "  Average Response Time: $([math]::Round($script:PerformanceMetrics.AverageResponseTime, 2))ms" -ForegroundColor White
        Write-Host "  Uptime: $($uptime.ToString('dd\.hh\:mm\:ss'))" -ForegroundColor White
        Write-Host ""
    }
    
    Write-CILog "CI system stopped successfully" "INFO" "CISystem"
}

# Main execution
try {
    Initialize-CISystem
    Start-GitHubMonitoring
} catch {
    Write-CILog "Critical error during startup: $($_.Exception.Message)" "ERROR" "Main"
} finally {
    Stop-CISystem
}
