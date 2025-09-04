# PHASE 2C: PERFORMANCE AND STRESS TESTING SCRIPT
# Comprehensive performance and stress testing for ADDS25 Enhanced File-Based Communication System

param(
    [switch]$Verbose,
    [int]$LoadTestCount = 50,
    [int]$StressTestDuration = 60,
    [int]$StabilityTestDuration = 300
)

function Write-TestLog {
    param([string]$Message, [string]$Level = "INFO")
    $Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $LogMessage = "[$Timestamp] [$Level] PHASE2C: $Message"
    
    if ($Verbose) {
        Write-Host $LogMessage
    }
    
    # Log to file
    $LogFile = "mcp_runs\test_logs\phase2c-performance-stress-test-$(Get-Date -Format 'yyyy-MM-dd').log"
    $LogDir = Split-Path $LogFile -Parent
    if (-not (Test-Path $LogDir)) {
        New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
    }
    Add-Content -Path $LogFile -Value $LogMessage
}

function Test-LoadPerformance {
    Write-TestLog "Testing Load Performance"
    
    $Tests = @()
    $StartTime = Get-Date
    
    try {
        # Test 1: High-Volume Trigger Generation
        Write-TestLog "Test 1: High-Volume Trigger Generation"
        $TriggerStartTime = Get-Date
        
        for ($i = 1; $i -le $LoadTestCount; $i++) {
            $TriggerResult = & ".\ci\SIMPLE-TRIGGER-GENERATOR.ps1" -Action "custom" -Hash "load-$i" -Message "Load test trigger $i" -Files "test-$i.txt" -Priority "normal" -Source "dev_computer" 2>$null
            if ($LASTEXITCODE -ne 0) {
                $Tests += @{ Test = "High-Volume Trigger Generation"; Result = "FAIL"; Details = "Trigger generation failed at iteration $i" }
                Write-TestLog "High-Volume Trigger Generation: FAIL at iteration $i" "ERROR"
                break
            }
        }
        
        $TriggerEndTime = Get-Date
        $TriggerDuration = ($TriggerEndTime - $TriggerStartTime).TotalMilliseconds
        $TriggersPerSecond = [math]::Round($LoadTestCount / ($TriggerDuration / 1000), 2)
        
        if ($Tests.Count -eq 0) {
            $Tests += @{ Test = "High-Volume Trigger Generation"; Result = "PASS"; Details = "Generated $LoadTestCount triggers in $TriggerDuration ms ($TriggersPerSecond triggers/sec)" }
            Write-TestLog "High-Volume Trigger Generation: PASS - $TriggersPerSecond triggers/sec" "SUCCESS"
        }
        
        # Test 2: File System Performance
        Write-TestLog "Test 2: File System Performance"
        $FileStartTime = Get-Date
        
        $TriggerFiles = Get-ChildItem -Path "triggers" -Filter "simple-trigger-*.json" | Sort-Object LastWriteTime -Descending | Select-Object -First $LoadTestCount
        
        if ($TriggerFiles.Count -eq $LoadTestCount) {
            $FileEndTime = Get-Date
            $FileDuration = ($FileEndTime - $FileStartTime).TotalMilliseconds
            $FilesPerSecond = [math]::Round($LoadTestCount / ($FileDuration / 1000), 2)
            
            $Tests += @{ Test = "File System Performance"; Result = "PASS"; Details = "Processed $($TriggerFiles.Count) files in $FileDuration ms ($FilesPerSecond files/sec)" }
            Write-TestLog "File System Performance: PASS - $FilesPerSecond files/sec" "SUCCESS"
        } else {
            $Tests += @{ Test = "File System Performance"; Result = "FAIL"; Details = "Expected $LoadTestCount files, found $($TriggerFiles.Count)" }
            Write-TestLog "File System Performance: FAIL - Expected $LoadTestCount files, found $($TriggerFiles.Count)" "ERROR"
        }
        
        # Test 3: Memory Usage Under Load
        Write-TestLog "Test 3: Memory Usage Under Load"
        $MemoryBefore = [System.GC]::GetTotalMemory($false)
        
        # Perform memory-intensive operations
        for ($i = 1; $i -le 100; $i++) {
            $TestData = @{
                iteration = $i
                timestamp = Get-Date
                data = "x" * 1000
            }
            $TestData | ConvertTo-Json | Out-Null
        }
        
        $MemoryAfter = [System.GC]::GetTotalMemory($false)
        $MemoryUsed = $MemoryAfter - $MemoryBefore
        $MemoryMB = [math]::Round($MemoryUsed / 1MB, 2)
        
        if ($MemoryMB -lt 50) {
            $Tests += @{ Test = "Memory Usage Under Load"; Result = "PASS"; Details = "Memory usage: $MemoryMB MB (under 50MB limit)" }
            Write-TestLog "Memory Usage Under Load: PASS - $MemoryMB MB" "SUCCESS"
        } else {
            $Tests += @{ Test = "Memory Usage Under Load"; Result = "FAIL"; Details = "Memory usage: $MemoryMB MB (exceeds 50MB limit)" }
            Write-TestLog "Memory Usage Under Load: FAIL - $MemoryMB MB" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Load Performance Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Load Performance Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    $EndTime = Get-Date
    $TotalDuration = ($EndTime - $StartTime).TotalMilliseconds
    Write-TestLog "Load Performance Testing completed in $TotalDuration ms"
    
    return $Tests
}

function Test-StressConditions {
    Write-TestLog "Testing Stress Conditions"
    
    $Tests = @()
    $StartTime = Get-Date
    
    try {
        # Test 1: Rapid Sequential Operations
        Write-TestLog "Test 1: Rapid Sequential Operations"
        $RapidStartTime = Get-Date
        $SuccessCount = 0
        
        for ($i = 1; $i -le 20; $i++) {
            try {
                $TriggerResult = & ".\ci\SIMPLE-TRIGGER-GENERATOR.ps1" -Action "custom" -Hash "stress-$i" -Message "Stress test $i" -Files "stress-$i.txt" -Priority "high" -Source "dev_computer" 2>$null
                if ($LASTEXITCODE -eq 0) {
                    $SuccessCount++
                }
                Start-Sleep -Milliseconds 50
            } catch {
                Write-TestLog "Rapid operation $i failed: $($_.Exception.Message)" "WARNING"
            }
        }
        
        $RapidEndTime = Get-Date
        $RapidDuration = ($RapidEndTime - $RapidStartTime).TotalMilliseconds
        
        if ($SuccessCount -ge 18) {
            $Tests += @{ Test = "Rapid Sequential Operations"; Result = "PASS"; Details = "Completed $SuccessCount/20 operations in $RapidDuration ms" }
            Write-TestLog "Rapid Sequential Operations: PASS - $SuccessCount/20 operations" "SUCCESS"
        } else {
            $Tests += @{ Test = "Rapid Sequential Operations"; Result = "FAIL"; Details = "Completed only $SuccessCount/20 operations" }
            Write-TestLog "Rapid Sequential Operations: FAIL - $SuccessCount/20 operations" "ERROR"
        }
        
        # Test 2: Error Handling Under Stress
        Write-TestLog "Test 2: Error Handling Under Stress"
        $ErrorHandlingStartTime = Get-Date
        $ErrorCount = 0
        
        # Test with invalid parameters
        for ($i = 1; $i -le 10; $i++) {
            try {
                $InvalidResult = & ".\ci\SIMPLE-TRIGGER-GENERATOR.ps1" -Action "" -Hash "" -Message "" -Files @() -Priority "invalid" -Source "" 2>$null
                if ($LASTEXITCODE -ne 0) {
                    $ErrorCount++
                }
            } catch {
                $ErrorCount++
            }
        }
        
        $ErrorHandlingEndTime = Get-Date
        $ErrorHandlingDuration = ($ErrorHandlingEndTime - $ErrorHandlingStartTime).TotalMilliseconds
        
        if ($ErrorCount -ge 8) {
            $Tests += @{ Test = "Error Handling Under Stress"; Result = "PASS"; Details = "Handled $ErrorCount/10 error conditions correctly" }
            Write-TestLog "Error Handling Under Stress: PASS - $ErrorCount/10 errors handled" "SUCCESS"
        } else {
            $Tests += @{ Test = "Error Handling Under Stress"; Result = "FAIL"; Details = "Only handled $ErrorCount/10 error conditions" }
            Write-TestLog "Error Handling Under Stress: FAIL - $ErrorCount/10 errors handled" "ERROR"
        }
        
        # Test 3: Resource Exhaustion Handling
        Write-TestLog "Test 3: Resource Exhaustion Handling"
        $ResourceStartTime = Get-Date
        
        # Test with very large data
        try {
            $LargeData = "x" * 10000
            $LargeTrigger = @{
                action = "resource-test"
                hash = "resource-test"
                message = $LargeData
                files = @($LargeData)
                priority = "normal"
                source = "resource-test"
                timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
                timezone = "UTC"
            }
            
            $LargeTriggerFile = "triggers\resource-test-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
            $LargeTrigger | ConvertTo-Json -Depth 3 | Set-Content -Path $LargeTriggerFile
            
            if (Test-Path $LargeTriggerFile) {
                $Tests += @{ Test = "Resource Exhaustion Handling"; Result = "PASS"; Details = "Successfully handled large data processing" }
                Write-TestLog "Resource Exhaustion Handling: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Resource Exhaustion Handling"; Result = "FAIL"; Details = "Failed to handle large data processing" }
                Write-TestLog "Resource Exhaustion Handling: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "Resource Exhaustion Handling"; Result = "FAIL"; Details = "Exception during large data processing: $($_.Exception.Message)" }
            Write-TestLog "Resource Exhaustion Handling: FAIL - $($_.Exception.Message)" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Stress Condition Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Stress Condition Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    $EndTime = Get-Date
    $TotalDuration = ($EndTime - $StartTime).TotalMilliseconds
    Write-TestLog "Stress Condition Testing completed in $TotalDuration ms"
    
    return $Tests
}

function Test-LongRunningStability {
    param([int]$Duration)
    
    Write-TestLog "Testing Long-Running Stability for $Duration seconds"
    
    $Tests = @()
    $StartTime = Get-Date
    $EndTime = $StartTime.AddSeconds($Duration)
    $OperationCount = 0
    $ErrorCount = 0
    
    try {
        while ((Get-Date) -lt $EndTime) {
            try {
                $OperationCount++
                $TriggerResult = & ".\ci\SIMPLE-TRIGGER-GENERATOR.ps1" -Action "custom" -Hash "stability-$OperationCount" -Message "Stability test operation $OperationCount" -Files "stability-$OperationCount.txt" -Priority "normal" -Source "dev_computer" 2>$null
                
                if ($LASTEXITCODE -ne 0) {
                    $ErrorCount++
                }
                
                # Check memory usage periodically
                if ($OperationCount % 10 -eq 0) {
                    $MemoryUsage = [System.GC]::GetTotalMemory($false)
                    $MemoryMB = [math]::Round($MemoryUsage / 1MB, 2)
                    Write-TestLog "Stability test: $OperationCount operations, $ErrorCount errors, $MemoryMB MB memory" "INFO"
                }
                
                Start-Sleep -Seconds 2
                
            } catch {
                $ErrorCount++
                Write-TestLog "Stability test operation $OperationCount failed: $($_.Exception.Message)" "WARNING"
            }
        }
        
        $ActualDuration = ((Get-Date) - $StartTime).TotalSeconds
        $ErrorRate = [math]::Round(($ErrorCount / $OperationCount) * 100, 2)
        
        if ($ErrorRate -lt 10) {
            $Tests += @{ Test = "Long-Running Stability"; Result = "PASS"; Details = "Completed $OperationCount operations in $ActualDuration seconds with $ErrorRate% error rate" }
            Write-TestLog "Long-Running Stability: PASS - $OperationCount operations, $ErrorRate% error rate" "SUCCESS"
        } else {
            $Tests += @{ Test = "Long-Running Stability"; Result = "FAIL"; Details = "Completed $OperationCount operations with $ErrorRate% error rate (exceeds 10% limit)" }
            Write-TestLog "Long-Running Stability: FAIL - $ErrorRate% error rate" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Long-Running Stability"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Long-Running Stability: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-PerformanceOptimization {
    Write-TestLog "Testing Performance Optimization"
    
    $Tests = @()
    
    try {
        # Test 1: Performance Optimizer Functionality
        Write-TestLog "Test 1: Performance Optimizer Functionality"
        $OptimizerStartTime = Get-Date
        
        $OptimizerResult = & ".\ci\SIMPLE-PERFORMANCE-OPTIMIZER.ps1" optimize 2>$null
        
        $OptimizerEndTime = Get-Date
        $OptimizerDuration = ($OptimizerEndTime - $OptimizerStartTime).TotalMilliseconds
        
        if ($LASTEXITCODE -eq 0) {
            $Tests += @{ Test = "Performance Optimizer Functionality"; Result = "PASS"; Details = "Performance optimization completed in $OptimizerDuration ms" }
            Write-TestLog "Performance Optimizer Functionality: PASS - $OptimizerDuration ms" "SUCCESS"
        } else {
            $Tests += @{ Test = "Performance Optimizer Functionality"; Result = "FAIL"; Details = "Performance optimization failed" }
            Write-TestLog "Performance Optimizer Functionality: FAIL" "ERROR"
        }
        
        # Test 2: Context Monitor Performance
        Write-TestLog "Test 2: Context Monitor Performance"
        $ContextStartTime = Get-Date
        
        $ContextResult = & ".\ci\CONTEXT-MONITOR.ps1" status 2>$null
        
        $ContextEndTime = Get-Date
        $ContextDuration = ($ContextEndTime - $ContextStartTime).TotalMilliseconds
        
        if ($LASTEXITCODE -eq 0) {
            $Tests += @{ Test = "Context Monitor Performance"; Result = "PASS"; Details = "Context monitoring completed in $ContextDuration ms" }
            Write-TestLog "Context Monitor Performance: PASS - $ContextDuration ms" "SUCCESS"
        } else {
            $Tests += @{ Test = "Context Monitor Performance"; Result = "FAIL"; Details = "Context monitoring failed" }
            Write-TestLog "Context Monitor Performance: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Performance Optimization Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Performance Optimization Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Show-TestResults {
    param([array]$AllTests)
    
    $PassCount = ($AllTests | Where-Object { $_.Result -eq "PASS" }).Count
    $FailCount = ($AllTests | Where-Object { $_.Result -eq "FAIL" }).Count
    $ErrorCount = ($AllTests | Where-Object { $_.Result -eq "ERROR" }).Count
    $TotalTests = $AllTests.Count
    
    Write-Host "`n=== PHASE 2C PERFORMANCE AND STRESS TEST RESULTS ===" -ForegroundColor Cyan
    Write-Host "Total Tests: $TotalTests" -ForegroundColor White
    Write-Host "Passed: $PassCount" -ForegroundColor Green
    Write-Host "Failed: $FailCount" -ForegroundColor Red
    Write-Host "Errors: $ErrorCount" -ForegroundColor Red
    Write-Host "Success Rate: $([math]::Round(($PassCount / $TotalTests) * 100, 2))%" -ForegroundColor White
    
    if ($FailCount -gt 0 -or $ErrorCount -gt 0) {
        Write-Host "`n=== FAILED TESTS ===" -ForegroundColor Red
        foreach ($Test in $AllTests | Where-Object { $_.Result -ne "PASS" }) {
            Write-Host "FAILED: $($Test.Test) - $($Test.Details)" -ForegroundColor Red
        }
    }
    
    if ($PassCount -eq $TotalTests) {
        Write-Host "`n=== ALL TESTS PASSED ===" -ForegroundColor Green
        Write-Host "Phase 2C Performance and Stress Testing: SUCCESSFUL" -ForegroundColor Green
    } else {
        Write-Host "`n=== SOME TESTS FAILED ===" -ForegroundColor Red
        Write-Host "Phase 2C Performance and Stress Testing: NEEDS ATTENTION" -ForegroundColor Red
    }
}

# Main execution
try {
    Write-TestLog "Starting Phase 2C Performance and Stress Testing"
    Write-TestLog "Load Test Count: $LoadTestCount"
    Write-TestLog "Stress Test Duration: $StressTestDuration seconds"
    Write-TestLog "Stability Test Duration: $StabilityTestDuration seconds"
    
    $AllTests = @()
    
    # Run all test suites
    $AllTests += Test-LoadPerformance
    $AllTests += Test-StressConditions
    $AllTests += Test-LongRunningStability -Duration $StabilityTestDuration
    $AllTests += Test-PerformanceOptimization
    
    # Show results
    Show-TestResults -AllTests $AllTests
    
    Write-TestLog "Phase 2C Performance and Stress Testing completed"
    
} catch {
    Write-TestLog "Error in Phase 2C testing: $($_.Exception.Message)" "ERROR"
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-TestLog "Phase 2C performance and stress test script completed successfully"
