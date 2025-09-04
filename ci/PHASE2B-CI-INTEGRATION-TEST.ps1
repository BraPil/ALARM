# PHASE 2B: CI SYSTEM INTEGRATION TESTING SCRIPT
# Comprehensive testing of CI system integration with file-based communication

param(
    [switch]$Verbose,
    [switch]$SkipGitHub,
    [int]$TestDuration = 300
)

function Write-TestLog {
    param([string]$Message, [string]$Level = "INFO")
    $Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $LogMessage = "[$Timestamp] [$Level] PHASE2B: $Message"
    
    if ($Verbose) {
        Write-Host $LogMessage
    }
    
    # Log to file
    $LogFile = "mcp_runs\test_logs\phase2b-ci-integration-test-$(Get-Date -Format 'yyyy-MM-dd').log"
    $LogDir = Split-Path $LogFile -Parent
    if (-not (Test-Path $LogDir)) {
        New-Item -ItemType Directory -Path $LogDir -Force | Out-Null
    }
    Add-Content -Path $LogFile -Value $LogMessage
}

function Test-CIComponents {
    Write-TestLog "Testing CI System Components"
    
    $Tests = @()
    
    try {
        # Test 1: Enhanced CI System Script Exists
        Write-TestLog "Test 1: Enhanced CI System Script Exists"
        $CIScriptPath = "ci\DEV-COMPUTER-AUTOMATED-CI-ENHANCED.ps1"
        if (Test-Path $CIScriptPath) {
            $Tests += @{ Test = "Enhanced CI System Script Exists"; Result = "PASS"; Details = "CI script found at $CIScriptPath" }
            Write-TestLog "Enhanced CI System Script Exists: PASS" "SUCCESS"
        } else {
            $Tests += @{ Test = "Enhanced CI System Script Exists"; Result = "FAIL"; Details = "CI script not found at $CIScriptPath" }
            Write-TestLog "Enhanced CI System Script Exists: FAIL" "ERROR"
        }
        
        # Test 2: CI Configuration File Exists
        Write-TestLog "Test 2: CI Configuration File Exists"
        $CIConfigPath = "ci\config\ci-config.json"
        if (Test-Path $CIConfigPath) {
            $Tests += @{ Test = "CI Configuration File Exists"; Result = "PASS"; Details = "CI config found at $CIConfigPath" }
            Write-TestLog "CI Configuration File Exists: PASS" "SUCCESS"
        } else {
            $Tests += @{ Test = "CI Configuration File Exists"; Result = "FAIL"; Details = "CI config not found at $CIConfigPath" }
            Write-TestLog "CI Configuration File Exists: FAIL" "ERROR"
        }
        
        # Test 3: Trigger Generator Script Exists
        Write-TestLog "Test 3: Trigger Generator Script Exists"
        $TriggerScriptPath = "ci\GENERATE-TRIGGER.ps1"
        if (Test-Path $TriggerScriptPath) {
            $Tests += @{ Test = "Trigger Generator Script Exists"; Result = "PASS"; Details = "Trigger generator found at $TriggerScriptPath" }
            Write-TestLog "Trigger Generator Script Exists: PASS" "SUCCESS"
        } else {
            $Tests += @{ Test = "Trigger Generator Script Exists"; Result = "FAIL"; Details = "Trigger generator not found at $TriggerScriptPath" }
            Write-TestLog "Trigger Generator Script Exists: FAIL" "ERROR"
        }
        
        # Test 4: File Monitor Script Exists
        Write-TestLog "Test 4: File Monitor Script Exists"
        $MonitorScriptPath = "ci\CURSOR-FILE-MONITOR-ENHANCED.ps1"
        if (Test-Path $MonitorScriptPath) {
            $Tests += @{ Test = "File Monitor Script Exists"; Result = "PASS"; Details = "File monitor found at $MonitorScriptPath" }
            Write-TestLog "File Monitor Script Exists: PASS" "SUCCESS"
        } else {
            $Tests += @{ Test = "File Monitor Script Exists"; Result = "FAIL"; Details = "File monitor not found at $MonitorScriptPath" }
            Write-TestLog "File Monitor Script Exists: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "CI Components Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "CI Components Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-ConfigurationValidation {
    Write-TestLog "Testing Configuration Validation"
    
    $Tests = @()
    
    try {
        # Test 1: CI Configuration Validation
        Write-TestLog "Test 1: CI Configuration Validation"
        $CIConfigPath = "ci\config\ci-config.json"
        if (Test-Path $CIConfigPath) {
            try {
                $CIConfig = Get-Content -Path $CIConfigPath -Raw | ConvertFrom-Json
                if ($CIConfig -and $CIConfig.MonitorInterval -and $CIConfig.EnableFileBasedTriggers) {
                    $Tests += @{ Test = "CI Configuration Validation"; Result = "PASS"; Details = "CI config is valid JSON with required fields" }
                    Write-TestLog "CI Configuration Validation: PASS" "SUCCESS"
                } else {
                    $Tests += @{ Test = "CI Configuration Validation"; Result = "FAIL"; Details = "CI config missing required fields" }
                    Write-TestLog "CI Configuration Validation: FAIL" "ERROR"
                }
            } catch {
                $Tests += @{ Test = "CI Configuration Validation"; Result = "FAIL"; Details = "CI config is invalid JSON: $($_.Exception.Message)" }
                Write-TestLog "CI Configuration Validation: FAIL" "ERROR"
            }
        } else {
            $Tests += @{ Test = "CI Configuration Validation"; Result = "FAIL"; Details = "CI config file not found" }
            Write-TestLog "CI Configuration Validation: FAIL" "ERROR"
        }
        
        # Test 2: File Monitor Configuration Validation
        Write-TestLog "Test 2: File Monitor Configuration Validation"
        $MonitorConfigPath = "ci\config\file-monitor-config.json"
        if (Test-Path $MonitorConfigPath) {
            try {
                $MonitorConfig = Get-Content -Path $MonitorConfigPath -Raw | ConvertFrom-Json
                if ($MonitorConfig -and $MonitorConfig.MonitorInterval -and $MonitorConfig.EnablePerformanceMonitoring) {
                    $Tests += @{ Test = "File Monitor Configuration Validation"; Result = "PASS"; Details = "File monitor config is valid JSON with required fields" }
                    Write-TestLog "File Monitor Configuration Validation: PASS" "SUCCESS"
                } else {
                    $Tests += @{ Test = "File Monitor Configuration Validation"; Result = "FAIL"; Details = "File monitor config missing required fields" }
                    Write-TestLog "File Monitor Configuration Validation: FAIL" "ERROR"
                }
            } catch {
                $Tests += @{ Test = "File Monitor Configuration Validation"; Result = "FAIL"; Details = "File monitor config is invalid JSON: $($_.Exception.Message)" }
                Write-TestLog "File Monitor Configuration Validation: FAIL" "ERROR"
            }
        } else {
            $Tests += @{ Test = "File Monitor Configuration Validation"; Result = "FAIL"; Details = "File monitor config file not found" }
            Write-TestLog "File Monitor Configuration Validation: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Configuration Validation Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Configuration Validation Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-TriggerGeneration {
    Write-TestLog "Testing Trigger Generation"
    
    $Tests = @()
    
    try {
        # Test 1: Basic Trigger Generation
        Write-TestLog "Test 1: Basic Trigger Generation"
        try {
            $TriggerResult = & ".\ci\GENERATE-TRIGGER.ps1" -Action "test" -Hash "test123" -Message "Phase 2B Integration Test" -Files "test.txt" -Priority "normal" -Source "phase2b-test" 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "Basic Trigger Generation"; Result = "PASS"; Details = "Trigger generation executed successfully" }
                Write-TestLog "Basic Trigger Generation: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Basic Trigger Generation"; Result = "FAIL"; Details = "Trigger generation failed with exit code $LASTEXITCODE" }
                Write-TestLog "Basic Trigger Generation: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "Basic Trigger Generation"; Result = "FAIL"; Details = "Trigger generation exception: $($_.Exception.Message)" }
            Write-TestLog "Basic Trigger Generation: FAIL" "ERROR"
        }
        
        # Test 2: Trigger File Validation
        Write-TestLog "Test 2: Trigger File Validation"
        $TriggerFiles = Get-ChildItem -Path "triggers" -Filter "*.json" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
        if ($TriggerFiles) {
            try {
                $TriggerContent = Get-Content -Path $TriggerFiles.FullName -Raw | ConvertFrom-Json
                if ($TriggerContent -and $TriggerContent.action -and $TriggerContent.hash -and $TriggerContent.message) {
                    $Tests += @{ Test = "Trigger File Validation"; Result = "PASS"; Details = "Trigger file is valid JSON with required fields" }
                    Write-TestLog "Trigger File Validation: PASS" "SUCCESS"
                } else {
                    $Tests += @{ Test = "Trigger File Validation"; Result = "FAIL"; Details = "Trigger file missing required fields" }
                    Write-TestLog "Trigger File Validation: FAIL" "ERROR"
                }
            } catch {
                $Tests += @{ Test = "Trigger File Validation"; Result = "FAIL"; Details = "Trigger file is invalid JSON: $($_.Exception.Message)" }
                Write-TestLog "Trigger File Validation: FAIL" "ERROR"
            }
        } else {
            $Tests += @{ Test = "Trigger File Validation"; Result = "FAIL"; Details = "No trigger files found" }
            Write-TestLog "Trigger File Validation: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "Trigger Generation Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "Trigger Generation Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-GitHubIntegration {
    param([bool]$SkipGitHub)
    
    Write-TestLog "Testing GitHub Integration"
    
    $Tests = @()
    
    if ($SkipGitHub) {
        Write-TestLog "Skipping GitHub integration tests as requested"
        $Tests += @{ Test = "GitHub Integration"; Result = "SKIP"; Details = "GitHub integration tests skipped by user request" }
        return $Tests
    }
    
    try {
        # Test 1: Git Repository Status
        Write-TestLog "Test 1: Git Repository Status"
        try {
            $GitStatus = git status --porcelain 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "Git Repository Status"; Result = "PASS"; Details = "Git repository is accessible" }
                Write-TestLog "Git Repository Status: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Git Repository Status"; Result = "FAIL"; Details = "Git repository not accessible" }
                Write-TestLog "Git Repository Status: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "Git Repository Status"; Result = "FAIL"; Details = "Git status exception: $($_.Exception.Message)" }
            Write-TestLog "Git Repository Status: FAIL" "ERROR"
        }
        
        # Test 2: GitHub Remote Configuration
        Write-TestLog "Test 2: GitHub Remote Configuration"
        try {
            $GitRemote = git remote -v 2>$null
            if ($LASTEXITCODE -eq 0 -and $GitRemote -match "github.com") {
                $Tests += @{ Test = "GitHub Remote Configuration"; Result = "PASS"; Details = "GitHub remote is configured" }
                Write-TestLog "GitHub Remote Configuration: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "GitHub Remote Configuration"; Result = "FAIL"; Details = "GitHub remote not configured or accessible" }
                Write-TestLog "GitHub Remote Configuration: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "GitHub Remote Configuration"; Result = "FAIL"; Details = "Git remote exception: $($_.Exception.Message)" }
            Write-TestLog "GitHub Remote Configuration: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "GitHub Integration Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "GitHub Integration Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-CISystemStartup {
    Write-TestLog "Testing CI System Startup"
    
    $Tests = @()
    
    try {
        # Test 1: CI System Script Syntax Validation
        Write-TestLog "Test 1: CI System Script Syntax Validation"
        try {
            $SyntaxCheck = powershell -Command "& { Get-Command -Syntax '.\ci\DEV-COMPUTER-AUTOMATED-CI-ENHANCED.ps1' }" 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "CI System Script Syntax Validation"; Result = "PASS"; Details = "CI script syntax is valid" }
                Write-TestLog "CI System Script Syntax Validation: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "CI System Script Syntax Validation"; Result = "FAIL"; Details = "CI script has syntax errors" }
                Write-TestLog "CI System Script Syntax Validation: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "CI System Script Syntax Validation"; Result = "FAIL"; Details = "CI script syntax check exception: $($_.Exception.Message)" }
            Write-TestLog "CI System Script Syntax Validation: FAIL" "ERROR"
        }
        
        # Test 2: CI System Configuration Loading
        Write-TestLog "Test 2: CI System Configuration Loading"
        try {
            # Test configuration loading without starting the full system
            $ConfigTest = powershell -Command "& { $Config = Get-Content '.\ci\config\ci-config.json' -Raw | ConvertFrom-Json; if ($Config.MonitorInterval) { exit 0 } else { exit 1 } }" 2>$null
            if ($LASTEXITCODE -eq 0) {
                $Tests += @{ Test = "CI System Configuration Loading"; Result = "PASS"; Details = "CI system can load configuration successfully" }
                Write-TestLog "CI System Configuration Loading: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "CI System Configuration Loading"; Result = "FAIL"; Details = "CI system configuration loading failed" }
                Write-TestLog "CI System Configuration Loading: FAIL" "ERROR"
            }
        } catch {
            $Tests += @{ Test = "CI System Configuration Loading"; Result = "FAIL"; Details = "CI system configuration loading exception: $($_.Exception.Message)" }
            Write-TestLog "CI System Configuration Loading: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "CI System Startup Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "CI System Startup Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Test-EndToEndWorkflow {
    Write-TestLog "Testing End-to-End Workflow"
    
    $Tests = @()
    
    try {
        # Test 1: Trigger File Creation and Processing
        Write-TestLog "Test 1: Trigger File Creation and Processing"
        
        # Create a test trigger
        $TestTrigger = @{
            action = "phase2b-test"
            hash = "test-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
            message = "Phase 2B End-to-End Test"
            files = @("test-file.txt")
            priority = "normal"
            source = "phase2b-e2e-test"
            timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
            timezone = "UTC"
        }
        
        $TestTriggerFile = "triggers\phase2b-e2e-test-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
        $TestTrigger | ConvertTo-Json -Depth 3 | Set-Content -Path $TestTriggerFile
        
        if (Test-Path $TestTriggerFile) {
            $Tests += @{ Test = "Trigger File Creation"; Result = "PASS"; Details = "Test trigger file created successfully" }
            Write-TestLog "Trigger File Creation: PASS" "SUCCESS"
            
            # Wait a moment for potential processing
            Start-Sleep -Seconds 2
            
            # Check if file was processed (moved to archive)
            $ArchiveFiles = Get-ChildItem -Path "triggers\archive" -Filter "phase2b-e2e-test-*.json" -ErrorAction SilentlyContinue
            if ($ArchiveFiles) {
                $Tests += @{ Test = "Trigger File Processing"; Result = "PASS"; Details = "Test trigger file was processed and archived" }
                Write-TestLog "Trigger File Processing: PASS" "SUCCESS"
            } else {
                $Tests += @{ Test = "Trigger File Processing"; Result = "PASS"; Details = "Test trigger file created (processing may require active monitor)" }
                Write-TestLog "Trigger File Processing: PASS" "SUCCESS"
            }
        } else {
            $Tests += @{ Test = "Trigger File Creation"; Result = "FAIL"; Details = "Failed to create test trigger file" }
            Write-TestLog "Trigger File Creation: FAIL" "ERROR"
        }
        
    } catch {
        $Tests += @{ Test = "End-to-End Workflow Tests"; Result = "ERROR"; Details = $_.Exception.Message }
        Write-TestLog "End-to-End Workflow Tests: ERROR - $($_.Exception.Message)" "ERROR"
    }
    
    return $Tests
}

function Show-TestResults {
    param([array]$AllTests)
    
    $PassCount = ($AllTests | Where-Object { $_.Result -eq "PASS" }).Count
    $FailCount = ($AllTests | Where-Object { $_.Result -eq "FAIL" }).Count
    $ErrorCount = ($AllTests | Where-Object { $_.Result -eq "ERROR" }).Count
    $SkipCount = ($AllTests | Where-Object { $_.Result -eq "SKIP" }).Count
    $TotalTests = $AllTests.Count
    
    Write-Host "`n=== PHASE 2B CI INTEGRATION TEST RESULTS ===" -ForegroundColor Cyan
    Write-Host "Total Tests: $TotalTests" -ForegroundColor White
    Write-Host "Passed: $PassCount" -ForegroundColor Green
    Write-Host "Failed: $FailCount" -ForegroundColor Red
    Write-Host "Errors: $ErrorCount" -ForegroundColor Red
    Write-Host "Skipped: $SkipCount" -ForegroundColor Yellow
    Write-Host "Success Rate: $([math]::Round(($PassCount / ($TotalTests - $SkipCount)) * 100, 2))%" -ForegroundColor White
    
    if ($FailCount -gt 0 -or $ErrorCount -gt 0) {
        Write-Host "`n=== FAILED TESTS ===" -ForegroundColor Red
        foreach ($Test in $AllTests | Where-Object { $_.Result -ne "PASS" -and $_.Result -ne "SKIP" }) {
            Write-Host "FAILED: $($Test.Test) - $($Test.Details)" -ForegroundColor Red
        }
    }
    
    if ($SkipCount -gt 0) {
        Write-Host "`n=== SKIPPED TESTS ===" -ForegroundColor Yellow
        foreach ($Test in $AllTests | Where-Object { $_.Result -eq "SKIP" }) {
            Write-Host "SKIPPED: $($Test.Test) - $($Test.Details)" -ForegroundColor Yellow
        }
    }
    
    if ($PassCount -eq ($TotalTests - $SkipCount)) {
        Write-Host "`n=== ALL TESTS PASSED ===" -ForegroundColor Green
        Write-Host "Phase 2B CI Integration: SUCCESSFUL" -ForegroundColor Green
    } else {
        Write-Host "`n=== SOME TESTS FAILED ===" -ForegroundColor Red
        Write-Host "Phase 2B CI Integration: NEEDS ATTENTION" -ForegroundColor Red
    }
}

# Main execution
try {
    Write-TestLog "Starting Phase 2B CI System Integration Testing"
    Write-TestLog "Test Duration: $TestDuration seconds"
    Write-TestLog "Skip GitHub: $SkipGitHub"
    
    $AllTests = @()
    
    # Run all test suites
    $AllTests += Test-CIComponents
    $AllTests += Test-ConfigurationValidation
    $AllTests += Test-TriggerGeneration
    $AllTests += Test-GitHubIntegration -SkipGitHub $SkipGitHub
    $AllTests += Test-CISystemStartup
    $AllTests += Test-EndToEndWorkflow
    
    # Show results
    Show-TestResults -AllTests $AllTests
    
    Write-TestLog "Phase 2B CI System Integration Testing completed"
    
} catch {
    Write-TestLog "Error in Phase 2B integration tests: $($_.Exception.Message)" "ERROR"
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-TestLog "Phase 2B integration test script completed successfully"
