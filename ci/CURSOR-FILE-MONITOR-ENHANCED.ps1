# ADDS25 Enhanced Cursor File Monitor
# Purpose: Advanced file-based communication system for Cursor automation
# Environment: Dev Computer (kidsg) - Production-ready file monitoring
# Date: September 3, 2025
# Version: 2.0 - Enhanced with validation, monitoring, and reliability

param(
    [string]$ConfigPath = ".\ci\config\file-monitor-config.json",
    [int]$MonitorInterval = 2,
    [switch]$EnablePerformanceMonitoring = $true,
    [switch]$EnableHealthChecks = $true,
    [switch]$Verbose = $false
)

# Configuration and Constants
$script:Config = @{
    MonitorInterval = $MonitorInterval
    EnablePerformanceMonitoring = $EnablePerformanceMonitoring
    EnableHealthChecks = $EnableHealthChecks
    Verbose = $Verbose
    MaxRetries = 3
    RetryDelay = 5
    LogRetentionDays = 30
}

# File Paths
$script:RepoPath = "C:\Users\kidsg\Downloads\ALARM"
$script:TriggerPattern = "$RepoPath\triggers\CURSOR-ANALYZE-NOW.trigger"
$script:LogPath = "$RepoPath\ci\logs\file-monitor"
$script:ArchivePath = "$RepoPath\triggers\archive"
$script:PerformanceLogPath = "$LogPath\performance"
$script:HealthLogPath = "$LogPath\health"

# Performance Metrics
$script:PerformanceMetrics = @{
    TotalTriggers = 0
    SuccessfulProcessing = 0
    FailedProcessing = 0
    AverageResponseTime = 0
    StartTime = Get-Date
    LastTriggerTime = $null
    ProcessingTimes = @()
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

# JSON Schema for Trigger Validation
$script:TriggerSchema = @{
    type = "object"
    required = @("commit", "message", "timestamp", "action", "files_to_check")
    properties = @{
        commit = @{ type = "string"; minLength = 7; maxLength = 40 }
        message = @{ type = "string"; minLength = 1; maxLength = 500 }
        timestamp = @{ type = "string"; pattern = "^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$" }
        timezone = @{ type = "string"; enum = @("ET", "CT", "MT", "PT", "UTC") }
        action = @{ type = "string"; enum = @("analyze_test_results", "deploy_fixes", "run_tests", "custom") }
        files_to_check = @{ type = "array"; items = @{ type = "string" }; minItems = 1 }
        priority = @{ type = "string"; enum = @("high", "normal", "low") }
        source = @{ type = "string"; enum = @("test_computer", "dev_computer", "automated") }
    }
}

# Initialize logging and directories
function Initialize-System {
    Write-Host "*** ADDS25 Enhanced File Monitor Starting ***" -ForegroundColor Cyan
    Write-Host "Version: 2.0 - Enhanced with validation, monitoring, and reliability" -ForegroundColor Yellow
    Write-Host ""
    
    # Create necessary directories
    $directories = @($LogPath, $PerformanceLogPath, $HealthLogPath, $ArchivePath, "triggers")
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
        Initialize-PerformanceMonitoring
    }
    
    # Initialize health monitoring
    if ($script:Config.EnableHealthChecks) {
        Initialize-HealthMonitoring
    }
    
    Write-Host "System initialized successfully" -ForegroundColor Green
    Write-Host "Monitoring: $TriggerPattern" -ForegroundColor Green
    Write-Host "Interval: $($script:Config.MonitorInterval) seconds" -ForegroundColor Green
    Write-Host ""
}

# Enhanced logging function
function Write-EnhancedLog {
    param(
        [string]$Message,
        [string]$Level = "INFO",
        [string]$Component = "FileMonitor",
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
    $logFile = Join-Path $LogPath "file-monitor-$(Get-Date -Format 'yyyy-MM-dd').log"
    $logJson = $logEntry | ConvertTo-Json -Depth 10
    Add-Content -Path $logFile -Value $logJson
    
    # Error logging to separate file
    if ($Level -eq "ERROR") {
        $errorLogFile = Join-Path $LogPath "errors-$(Get-Date -Format 'yyyy-MM-dd').log"
        Add-Content -Path $errorLogFile -Value $logJson
    }
}

# JSON Schema Validation
function Test-JsonSchema {
    param(
        [string]$JsonContent,
        [hashtable]$Schema
    )
    
    try {
        $data = $JsonContent | ConvertFrom-Json
        
        # Basic validation
        foreach ($requiredField in $Schema.required) {
            if (-not $data.PSObject.Properties.Name.Contains($requiredField)) {
                return @{ IsValid = $false; Error = "Missing required field: $requiredField" }
            }
        }
        
        # Type validation
        foreach ($property in $Schema.properties.Keys) {
            if ($data.PSObject.Properties.Name.Contains($property)) {
                $expectedType = $Schema.properties[$property].type
                $actualValue = $data.$property
                
                switch ($expectedType) {
                    "string" {
                        if ($actualValue -isnot [string]) {
                            return @{ IsValid = $false; Error = "Field '$property' must be a string" }
                        }
                        
                        # Length validation
                        if ($Schema.properties[$property].minLength -and $actualValue.Length -lt $Schema.properties[$property].minLength) {
                            return @{ IsValid = $false; Error = "Field '$property' is too short (min: $($Schema.properties[$property].minLength))" }
                        }
                        
                        if ($Schema.properties[$property].maxLength -and $actualValue.Length -gt $Schema.properties[$property].maxLength) {
                            return @{ IsValid = $false; Error = "Field '$property' is too long (max: $($Schema.properties[$property].maxLength))" }
                        }
                        
                        # Pattern validation
                        if ($Schema.properties[$property].pattern) {
                            if ($actualValue -notmatch $Schema.properties[$property].pattern) {
                                return @{ IsValid = $false; Error = "Field '$property' does not match required pattern" }
                            }
                        }
                        
                        # Enum validation
                        if ($Schema.properties[$property].enum) {
                            if ($actualValue -notin $Schema.properties[$property].enum) {
                                return @{ IsValid = $false; Error = "Field '$property' must be one of: $($Schema.properties[$property].enum -join ', ')" }
                            }
                        }
                    }
                    "array" {
                        if ($actualValue -isnot [array]) {
                            return @{ IsValid = $false; Error = "Field '$property' must be an array" }
                        }
                        
                        # Array length validation
                        if ($Schema.properties[$property].minItems -and $actualValue.Count -lt $Schema.properties[$property].minItems) {
                            return @{ IsValid = $false; Error = "Field '$property' array is too short (min: $($Schema.properties[$property].minItems))" }
                        }
                    }
                }
            }
        }
        
        return @{ IsValid = $true; Error = $null }
        
    } catch {
        return @{ IsValid = $false; Error = "JSON parsing error: $($_.Exception.Message)" }
    }
}

# Performance monitoring initialization
function Initialize-PerformanceMonitoring {
    $script:PerformanceMetrics.StartTime = Get-Date
    Write-EnhancedLog "Performance monitoring initialized" "INFO" "PerformanceMonitor"
    
    # Start background performance logging
    Start-Job -ScriptBlock {
        param($LogPath)
        while ($true) {
            $metrics = @{
                Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
                TotalTriggers = $using:script:PerformanceMetrics.TotalTriggers
                SuccessfulProcessing = $using:script:PerformanceMetrics.SuccessfulProcessing
                FailedProcessing = $using:script:PerformanceMetrics.FailedProcessing
                AverageResponseTime = $using:script:PerformanceMetrics.AverageResponseTime
                Uptime = (Get-Date) - $using:script:PerformanceMetrics.StartTime
            }
            
            $logFile = Join-Path $LogPath "performance-$(Get-Date -Format 'yyyy-MM-dd').log"
            $metricsJson = $metrics | ConvertTo-Json
            Add-Content -Path $logFile -Value $metricsJson
            
            Start-Sleep -Seconds 60
        }
    } -ArgumentList $PerformanceLogPath | Out-Null
}

# Health monitoring initialization
function Initialize-HealthMonitoring {
    Write-EnhancedLog "Health monitoring initialized" "INFO" "HealthMonitor"
    
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
                
                $logFile = Join-Path $LogPath "health-$(Get-Date -Format 'yyyy-MM-dd').log"
                $healthJson = $health | ConvertTo-Json
                Add-Content -Path $logFile -Value $healthJson
                
                Start-Sleep -Seconds 30
                
            } catch {
                Start-Sleep -Seconds 30
            }
        }
    } -ArgumentList $HealthLogPath, $RepoPath | Out-Null
}

# Process trigger file with enhanced validation
function Process-TriggerFile {
    param([string]$TriggerFilePath)
    
    $startTime = Get-Date
    $script:PerformanceMetrics.TotalTriggers++
    
    try {
        Write-EnhancedLog "Processing trigger file: $TriggerFilePath" "INFO" "TriggerProcessor"
        
        # Read and validate trigger file
        $triggerContent = Get-Content $TriggerFilePath -Raw -ErrorAction Stop
        
        # JSON Schema validation
        $validation = Test-JsonSchema -JsonContent $triggerContent -Schema $script:TriggerSchema
        if (-not $validation.IsValid) {
            throw "JSON validation failed: $($validation.Error)"
        }
        
        # Parse trigger data
        $triggerData = $triggerContent | ConvertFrom-Json
        
        Write-EnhancedLog "Trigger validated successfully" "SUCCESS" "TriggerProcessor" @{
            Commit = $triggerData.commit
            Action = $triggerData.action
            Priority = $triggerData.priority
            Source = $triggerData.source
        }
        
        # Execute analysis based on action
        $result = Execute-AnalysisAction -TriggerData $triggerData
        
        # Archive processed trigger
        $archiveName = "trigger-$(Get-Date -Format 'yyyyMMdd-HHmmss')-$(Get-Random).json"
        $archivePath = Join-Path $script:ArchivePath $archiveName
        Move-Item $TriggerFilePath $archivePath -Force
        
        # Update performance metrics
        $processingTime = (Get-Date) - $startTime
        $script:PerformanceMetrics.ProcessingTimes += $processingTime.TotalMilliseconds
        $script:PerformanceMetrics.AverageResponseTime = $script:PerformanceMetrics.ProcessingTimes | Measure-Object -Average | Select-Object -ExpandProperty Average
        $script:PerformanceMetrics.SuccessfulProcessing++
        $script:PerformanceMetrics.LastTriggerTime = Get-Date
        
        Write-EnhancedLog "Trigger processed successfully in $([math]::Round($processingTime.TotalMilliseconds, 2))ms" "SUCCESS" "TriggerProcessor" @{
            ProcessingTime = $processingTime.TotalMilliseconds
            ArchivePath = $archivePath
        }
        
        return $result
        
    } catch {
        $script:PerformanceMetrics.FailedProcessing++
        Write-EnhancedLog "Failed to process trigger: $($_.Exception.Message)" "ERROR" "TriggerProcessor" @{
            Error = $_.Exception.Message
            StackTrace = $_.ScriptStackTrace
        }
        
        # Move failed trigger to error archive
        $errorArchiveName = "error-$(Get-Date -Format 'yyyyMMdd-HHmmss')-$(Get-Random).json"
        $errorArchivePath = Join-Path $script:ArchivePath $errorArchiveName
        Move-Item $TriggerFilePath $errorArchivePath -Force
        
        return $false
    }
}

# Execute analysis action based on trigger data
function Execute-AnalysisAction {
    param([object]$TriggerData)
    
    try {
        Write-EnhancedLog "Executing analysis action: $($TriggerData.action)" "INFO" "AnalysisExecutor"
        
        switch ($TriggerData.action) {
            "analyze_test_results" {
                return Execute-TestResultsAnalysis -TriggerData $TriggerData
            }
            "deploy_fixes" {
                return Execute-FixDeployment -TriggerData $TriggerData
            }
            "run_tests" {
                return Execute-TestExecution -TriggerData $TriggerData
            }
            "custom" {
                return Execute-CustomAction -TriggerData $TriggerData
            }
            default {
                throw "Unknown action: $($TriggerData.action)"
            }
        }
        
    } catch {
        Write-EnhancedLog "Failed to execute analysis action: $($_.Exception.Message)" "ERROR" "AnalysisExecutor"
        return $false
    }
}

# Execute test results analysis
function Execute-TestResultsAnalysis {
    param([object]$TriggerData)
    
    Write-EnhancedLog "Starting test results analysis" "INFO" "TestAnalysis"
    
    # Pull latest changes
    Set-Location $script:RepoPath
    $pullResult = git pull origin main 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-EnhancedLog "Git pull failed: $pullResult" "ERROR" "TestAnalysis"
        return $false
    }
    
    Write-EnhancedLog "Git pull successful" "SUCCESS" "TestAnalysis"
    
    # Find and analyze test results
    $testResults = @()
    foreach ($pattern in $TriggerData.files_to_check) {
        $files = Get-ChildItem $pattern -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
        if ($files) {
            $testResults += $files
            Write-EnhancedLog "Found $($files.Count) files matching pattern: $pattern" "INFO" "TestAnalysis"
        }
    }
    
    if ($testResults) {
        $latestResult = $testResults | Sort-Object LastWriteTime -Descending | Select-Object -First 1
        
        Write-EnhancedLog "Test results analysis ready for Cursor" "SUCCESS" "TestAnalysis" @{
            LatestFile = $latestResult.Name
            FileTime = $latestResult.LastWriteTime
            TotalFiles = $testResults.Count
            Commit = $TriggerData.commit
        }
        
        # Display analysis command for user
        Write-Host ""
        Write-Host "🎯 READY FOR CURSOR ANALYSIS!" -ForegroundColor Green
        Write-Host "===============================================" -ForegroundColor Yellow
        Write-Host "AUTOMATIC ANALYSIS READY - SEND TO CURSOR:" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "New test results detected - analyze now with time zone adjustment" -ForegroundColor White
        Write-Host "Latest file: $($latestResult.Name)" -ForegroundColor White
        Write-Host "Commit: $($TriggerData.commit)" -ForegroundColor White
        Write-Host "Time: $($TriggerData.timestamp) ($($TriggerData.timezone))" -ForegroundColor White
        Write-Host ""
        Write-Host "CURSOR: Pull latest changes and analyze the above test results" -ForegroundColor Green
        Write-Host "===============================================" -ForegroundColor Yellow
        
        return $true
    } else {
        Write-EnhancedLog "No test result files found" "WARNING" "TestAnalysis"
        return $false
    }
}

# Execute fix deployment
function Execute-FixDeployment {
    param([object]$TriggerData)
    
    Write-EnhancedLog "Starting fix deployment" "INFO" "FixDeployment"
    
    # Implementation for fix deployment
    Write-EnhancedLog "Fix deployment not yet implemented" "WARNING" "FixDeployment"
    return $false
}

# Execute test execution
function Execute-TestExecution {
    param([object]$TriggerData)
    
    Write-EnhancedLog "Starting test execution" "INFO" "TestExecution"
    
    # Implementation for test execution
    Write-EnhancedLog "Test execution not yet implemented" "WARNING" "TestExecution"
    return $false
}

# Execute custom action
function Execute-CustomAction {
    param([object]$TriggerData)
    
    Write-EnhancedLog "Starting custom action execution" "INFO" "CustomAction"
    
    # Implementation for custom actions
    Write-EnhancedLog "Custom action execution not yet implemented" "WARNING" "CustomAction"
    return $false
}

# Main monitoring loop
function Start-FileMonitoring {
    Write-Host "Starting enhanced file monitoring..." -ForegroundColor Green
    Write-Host "Press Ctrl+C to stop monitoring" -ForegroundColor Yellow
    Write-Host ""
    
    $lastProcessed = ""
    
    try {
        while ($true) {
            try {
                if (Test-Path $script:TriggerPattern) {
                    $triggerFile = Get-Item $script:TriggerPattern
                    $currentHash = $triggerFile.LastWriteTime.ToString()
                    
                    # Only process if this is a new/updated trigger file
                    if ($currentHash -ne $lastProcessed) {
                        Write-EnhancedLog "New trigger file detected" "INFO" "FileMonitor" @{
                            FileName = $triggerFile.Name
                            FileSize = $triggerFile.Length
                            LastWriteTime = $triggerFile.LastWriteTime
                        }
                        
                        # Process the trigger file
                        $result = Process-TriggerFile -TriggerFilePath $triggerFile.FullName
                        
                        if ($result) {
                            $lastProcessed = $currentHash
                        }
                    }
                }
                
                # Health check
                if ($script:Config.EnableHealthChecks) {
                    $script:HealthStatus.LastHealthCheck = Get-Date
                }
                
                Start-Sleep -Seconds $script:Config.MonitorInterval
                
            } catch {
                Write-EnhancedLog "Error in monitoring loop: $($_.Exception.Message)" "ERROR" "FileMonitor"
                Start-Sleep -Seconds $script:Config.MonitorInterval
            }
        }
    } catch {
        Write-EnhancedLog "Critical error in file monitoring: $($_.Exception.Message)" "ERROR" "FileMonitor"
    } finally {
        Write-EnhancedLog "File monitoring stopped" "INFO" "FileMonitor"
    }
}

# Cleanup function
function Stop-FileMonitoring {
    Write-EnhancedLog "Stopping file monitoring..." "INFO" "FileMonitor"
    
    # Stop background jobs
    Get-Job | Stop-Job
    Get-Job | Remove-Job
    
    # Final performance report
    if ($script:Config.EnablePerformanceMonitoring) {
        $uptime = (Get-Date) - $script:PerformanceMetrics.StartTime
        Write-Host ""
        Write-Host "📊 PERFORMANCE SUMMARY:" -ForegroundColor Cyan
        Write-Host "  Total Triggers: $($script:PerformanceMetrics.TotalTriggers)" -ForegroundColor White
        Write-Host "  Successful: $($script:PerformanceMetrics.SuccessfulProcessing)" -ForegroundColor Green
        Write-Host "  Failed: $($script:PerformanceMetrics.FailedProcessing)" -ForegroundColor Red
        Write-Host "  Average Response Time: $([math]::Round($script:PerformanceMetrics.AverageResponseTime, 2))ms" -ForegroundColor White
        Write-Host "  Uptime: $($uptime.ToString('dd\.hh\:mm\:ss'))" -ForegroundColor White
        Write-Host ""
    }
    
    Write-EnhancedLog "File monitoring stopped successfully" "INFO" "FileMonitor"
}

# Main execution
try {
    Initialize-System
    Start-FileMonitoring
} catch {
    Write-EnhancedLog "Critical error during startup: $($_.Exception.Message)" "ERROR" "Main"
} finally {
    Stop-FileMonitoring
}
