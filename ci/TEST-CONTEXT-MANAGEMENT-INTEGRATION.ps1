# TEST CONTEXT MANAGEMENT INTEGRATION SCRIPT
# Comprehensive testing of context monitoring and restart protocol integration

param(
    [switch]$Verbose
)

function Write-TestLog {
    param([string]$Message, [string]$Level = "INFO")
    $Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $LogMessage = "[$Timestamp] [$Level] TEST: $Message"
    
    if ($Verbose) {
        Write-Host $LogMessage
    }
    
    # Log to file
    $LogFile = "mcp_runs\test_logs\context-management-integration-test-$(Get-Date -Format 'yyyy-MM-dd').log"
    $LogDir = Split-Path $LogFile -Parent
    if (-not (Test-Path $LogDir)) {
        New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
    }
    Add-Content -Path $LogFile -Value $LogMessage
}

function Test-ContextMonitor {
    Write-TestLog "Testing Context Monitor functionality"
    
    $Tests = @()
    
    try {
        # Test 1: Context Monitor Status
        Write-TestLog "Test 1: Context Monitor Status"
        try {
            $StatusResult = & ".\ci\CONTEXT-MONITOR.ps1" status 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "Context Monitor Status"; Result = "PASS"; Details = "Status command executed successfully" }
                Write-TestLog "Context Monitor Status: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Context Monitor Status"; Result = "FAIL"; Details = "Status command failed with exit code $LASTEXITCODE" }
                Write-TestLog "Context Monitor Status: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "Context Monitor Status"; Result = "FAIL"; Details = "Status command exception: $($_.Exception.Message)" }
            Write-TestLog "Context Monitor Status: FAIL" "ERROR"
        }
        
        # Test 2: Context Monitor Help
        Write-TestLog "Test 2: Context Monitor Help"
        try {
            $HelpResult = & ".\ci\CONTEXT-MONITOR.ps1" help 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "Context Monitor Help"; Result = "PASS"; Details = "Help command executed successfully" }
                Write-TestLog "Context Monitor Help: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Context Monitor Help"; Result = "FAIL"; Details = "Help command failed with exit code $LASTEXITCODE" }
                Write-TestLog "Context Monitor Help: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "Context Monitor Help"; Result = "FAIL"; Details = "Help command exception: $($_.Exception.Message)" }
            Write-TestLog "Context Monitor Help: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Context Monitor Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Context Monitor Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-PerformanceOptimizerIntegration {
    Write-TestLog "Testing Performance Optimizer Integration"
    
    $Tests = @()
    
    try {
        # Test 1: Enhanced Performance Optimizer
        Write-TestLog "Test 1: Enhanced Performance Optimizer"
        try {
            $OptimizeResult = & ".\ci\SIMPLE-PERFORMANCE-OPTIMIZER.ps1" optimize 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "Enhanced Performance Optimizer"; Result = "PASS"; Details = "Optimization with context management executed successfully" }
                Write-TestLog "Enhanced Performance Optimizer: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Enhanced Performance Optimizer"; Result = "FAIL"; Details = "Optimization failed with exit code $LASTEXITCODE" }
                Write-TestLog "Enhanced Performance Optimizer: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "Enhanced Performance Optimizer"; Result = "FAIL"; Details = "Optimization exception: $($_.Exception.Message)" }
            Write-TestLog "Enhanced Performance Optimizer: FAIL" "ERROR"
        }
        
        # Test 2: Performance Optimizer Status
        Write-TestLog "Test 2: Performance Optimizer Status"
        try {
            $StatusResult = & ".\ci\SIMPLE-PERFORMANCE-OPTIMIZER.ps1" status 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "Performance Optimizer Status"; Result = "PASS"; Details = "Status command executed successfully" }
                Write-TestLog "Performance Optimizer Status: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Performance Optimizer Status"; Result = "FAIL"; Details = "Status command failed with exit code $LASTEXITCODE" }
                Write-TestLog "Performance Optimizer Status: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "Performance Optimizer Status"; Result = "FAIL"; Details = "Status command exception: $($_.Exception.Message)" }
            Write-TestLog "Performance Optimizer Status: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Performance Optimizer Integration Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Performance Optimizer Integration Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-ProtocolIntegration {
    Write-TestLog "Testing Protocol Integration"
    
    $Tests = @()
    
    try {
        # Test 1: Master Protocol File Exists
        Write-TestLog "Test 1: Master Protocol File Exists"
        $MasterProtocolPath = "mcp\protocols\master_protocol.md"
        if (Test-Path $MasterProtocolPath) {
            $Tests += @{ Test = "Master Protocol File Exists"; Result = "PASS"; Details = "Master Protocol file found" }
            Write-TestLog "Master Protocol File Exists: PASS" "SUCCESS"
        } else {
            $Tests += @{ Test = "Master Protocol File Exists"; Result = "FAIL"; Details = "Master Protocol file not found" }
            Write-TestLog "Master Protocol File Exists: FAIL" "ERROR"
        }
        
        # Test 2: Enhanced Restart Protocol File Exists
        Write-TestLog "Test 2: Enhanced Restart Protocol File Exists"
        $RestartProtocolPath = "mcp\protocols\enhanced-restart-protocol.md"
        if (Test-Path $RestartProtocolPath) {
            $Tests += @{ Test = "Enhanced Restart Protocol File Exists"; Result = "PASS"; Details = "Enhanced Restart Protocol file found" }
            Write-TestLog "Enhanced Restart Protocol File Exists: PASS" "SUCCESS"
        } else {
            $Tests += @{ Test = "Enhanced Restart Protocol File Exists"; Result = "FAIL"; Details = "Enhanced Restart Protocol file not found" }
            Write-TestLog "Enhanced Restart Protocol File Exists: FAIL" "ERROR"
        }
        
        # Test 3: Context Management Integration in Master Protocol
        Write-TestLog "Test 3: Context Management Integration in Master Protocol"
        $MasterProtocolContent = Get-Content -Path $MasterProtocolPath -Raw
        if ($MasterProtocolContent -match "Conversation Monitoring" -and $MasterProtocolContent -match "Context Archival") {
            $Tests += @{ Test = "Context Management Integration in Master Protocol"; Result = "PASS"; Details = "Context management integration found in Master Protocol" }
            Write-TestLog "Context Management Integration in Master Protocol: PASS" "SUCCESS"
        } else {
            $Tests += @{ Test = "Context Management Integration in Master Protocol"; Result = "FAIL"; Details = "Context management integration not found in Master Protocol" }
            Write-TestLog "Context Management Integration in Master Protocol: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Protocol Integration Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Protocol Integration Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Show-TestResults {
    param([array]$AllTests)
    
    $PassCount = ($AllTests | Where-Object { $_.Result -eq "PASS" }).Count
    $FailCount = ($AllTests | Where-Object { $_.Result -eq "FAIL" }).Count
    $ErrorCount = ($AllTests | Where-Object { $_.Result -eq "ERROR" }).Count
    $TotalTests = $AllTests.Count
    
    Write-Host "`n=== CONTEXT MANAGEMENT INTEGRATION TEST RESULTS ===" -ForegroundColor Cyan
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
        Write-Host "Context Management Integration: SUCCESSFUL" -ForegroundColor Green
    } else {
        Write-Host "`n=== SOME TESTS FAILED ===" -ForegroundColor Red
        Write-Host "Context Management Integration: NEEDS ATTENTION" -ForegroundColor Red
    }
}

# Main execution
try {
    Write-TestLog "Starting Context Management Integration Tests"
    
    $AllTests = @()
    
    # Run all test suites
    $AllTests += Test-ContextMonitor
    $AllTests += Test-PerformanceOptimizerIntegration
    $AllTests += Test-ProtocolIntegration
    
    # Show results
    Show-TestResults -AllTests $AllTests
    
    Write-TestLog "Context Management Integration Tests completed"
    
} catch {
    Write-TestLog "Error in integration tests: $($_.Exception.Message)" "ERROR"
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-TestLog "Integration test script completed successfully"
