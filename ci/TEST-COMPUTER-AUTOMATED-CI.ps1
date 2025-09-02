# ADDS25 Test Computer Automated CI System
# Purpose: Listen for fix commits, auto-pull, test, and push results
# Environment: Test Computer (wa-bdpilegg)
# Date: September 1, 2025

Write-Host "ADDS25 Test Computer Automated CI System Starting..." -ForegroundColor Cyan
Write-Host "This system will monitor GitHub for fixes and auto-test ADDS25" -ForegroundColor Yellow

# Configuration
$repoPath = "C:\Users\wa-bdpilegg\Downloads\ALARM"
$testResultsPath = "$repoPath\test-results"
$ciLogPath = "$repoPath\ci\logs"
$adds25Path = "$repoPath\tests\ADDS25\v0.1"

# Ensure directories exist
@($testResultsPath, $ciLogPath) | ForEach-Object {
    if (!(Test-Path $_)) { New-Item $_ -Type Directory -Force | Out-Null }
}

# Initialize CI logging
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$ciLog = "$ciLogPath\test-ci-session-$timestamp.md"

@"
# ADDS25 Test Computer CI Session

**Session Start**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Repository**: $repoPath
**ADDS25 Path**: $adds25Path

---

## TEST AUTOMATION LOG

"@ | Out-File $ciLog -Encoding UTF8

function Write-TestLog {
    param([string]$Message, [string]$Level = "INFO")
    $logEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $logEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } })
    Add-Content $ciLog $logEntry -Encoding UTF8
}

# Function: Execute comprehensive ADDS25 test
function Invoke-ADDS25Test {
    Write-TestLog "Starting comprehensive ADDS25 test execution..." "INFO"
    
    # Change to ADDS25 directory
    Set-Location $adds25Path
    
    # Initialize test result structure
    $testResult = @{
        Timestamp = Get-Date -Format 'yyyy-MM-dd_HH-mm-ss'
        BuildStatus = "UNKNOWN"
        LauncherStatus = "UNKNOWN"
        AutoCADStatus = "UNKNOWN"
        Errors = @()
        Warnings = @()
        LogFiles = @()
        NextActions = @()
    }
    
    Write-TestLog "Test session initialized: $($testResult.Timestamp)" "INFO"
    
    # Step 1: Build ADDS25 solution
    Write-TestLog "Step 1: Building ADDS25 solution..." "INFO"
    try {
        $buildOutput = dotnet build 2>&1 | Out-String
        
        if ($LASTEXITCODE -eq 0) {
            $testResult.BuildStatus = "SUCCESS"
            Write-TestLog "Build completed successfully" "SUCCESS"
        } else {
            $testResult.BuildStatus = "FAILED"
            $testResult.Errors += "Build failed with exit code: $LASTEXITCODE"
            Write-TestLog "Build failed with exit code: $LASTEXITCODE" "ERROR"
        }
        
        # Save build output
        $buildLogFile = "$testResultsPath\build-output-$($testResult.Timestamp).txt"
        $buildOutput | Out-File $buildLogFile -Encoding UTF8
        $testResult.LogFiles += $buildLogFile
        
    } catch {
        $testResult.BuildStatus = "ERROR"
        $testResult.Errors += "Build error: $($_.Exception.Message)"
        Write-TestLog "Build error: $($_.Exception.Message)" "ERROR"
    }
    
    # Step 2: Verify launcher files exist before attempting execution
    if ($testResult.BuildStatus -eq "SUCCESS") {
        Write-TestLog "Step 2: Verifying launcher files and executing..." "INFO"
        
        # First try emergency deployment batch file (Git-independent)
        $emergencyDeployBat = "$adds25Path\EMERGENCY-LAUNCHER-DEPLOY.bat"
        if (Test-Path $emergencyDeployBat) {
            Write-TestLog "Running emergency deployment batch file..." "INFO"
            try {
                $emergencyProcess = Start-Process -FilePath $emergencyDeployBat -PassThru -NoNewWindow -Wait
                if ($emergencyProcess.ExitCode -eq 0) {
                    Write-TestLog "Emergency deployment completed successfully" "SUCCESS"
                } else {
                    Write-TestLog "Emergency deployment failed with exit code: $($emergencyProcess.ExitCode)" "WARNING"
                }
            } catch {
                Write-TestLog "Emergency deployment error: $($_.Exception.Message)" "WARNING"
            }
        } else {
            Write-TestLog "Emergency deployment batch file not found, trying PowerShell script..." "WARNING"
            
            # Fallback to PowerShell force deployment
            $forceDeployScript = "$adds25Path\FORCE-DEPLOY-LAUNCHERS.ps1"
            if (Test-Path $forceDeployScript) {
                Write-TestLog "Running force deployment script..." "INFO"
                try {
                    & PowerShell.exe -ExecutionPolicy Bypass -File $forceDeployScript
                    Write-TestLog "Force deployment completed" "SUCCESS"
                } catch {
                    Write-TestLog "Force deployment error: $($_.Exception.Message)" "WARNING"
                }
            } else {
                Write-TestLog "Force deployment script not found, will attempt verification..." "WARNING"
            
            # Fallback to verification script
            $verificationScript = "$adds25Path\VERIFY-LAUNCHER-EXISTS.ps1"
            if (Test-Path $verificationScript) {
                Write-TestLog "Running launcher verification script..." "INFO"
                try {
                    & PowerShell.exe -ExecutionPolicy Bypass -File $verificationScript
                    Write-TestLog "Launcher verification completed" "SUCCESS"
                } catch {
                    Write-TestLog "Verification script error: $($_.Exception.Message)" "WARNING"
                }
            } else {
                Write-TestLog "Neither deployment nor verification script found" "ERROR"
            }
        }
        
        try {
            # First try simple launcher to verify basic execution works
            $simpleLauncherPath = "$adds25Path\ADDS25-Launcher-Simple.bat"
            $fullLauncherPath = "$adds25Path\ADDS25-Launcher.bat"
            
            # Check which launchers exist
            $simpleExists = Test-Path $simpleLauncherPath
            $fullExists = Test-Path $fullLauncherPath
            
            Write-TestLog "Simple launcher exists: $simpleExists at $simpleLauncherPath" "INFO"
            Write-TestLog "Full launcher exists: $fullExists at $fullLauncherPath" "INFO"
            
            # Choose launcher to execute
            if ($simpleExists) {
                $launcherToUse = $simpleLauncherPath
                Write-TestLog "Using simple launcher for testing" "INFO"
            } elseif ($fullExists) {
                $launcherToUse = $fullLauncherPath
                Write-TestLog "Using full launcher" "INFO"
            } else {
                throw "No launcher found. Simple: $simpleLauncherPath, Full: $fullLauncherPath"
            }
            
            # Execute the chosen launcher with proper working directory
            Write-TestLog "Current working directory: $(Get-Location)" "INFO"
            Write-TestLog "Launcher path: $launcherToUse" "INFO"
            Write-TestLog "Launcher exists: $(Test-Path $launcherToUse)" "INFO"
            Write-TestLog "ADDS25 directory: $adds25Path" "INFO"
            Write-TestLog "ADDS25 directory exists: $(Test-Path $adds25Path)" "INFO"
            
            # Change to ADDS25 directory before executing launcher
            $originalLocation = Get-Location
            Set-Location $adds25Path
            Write-TestLog "Changed working directory to: $(Get-Location)" "INFO"
            
            # Execute launcher from the ADDS25 directory
            $launcherFileName = Split-Path $launcherToUse -Leaf
            Write-TestLog "Executing launcher: $launcherFileName from directory: $(Get-Location)" "INFO"
            
            $launcherProcess = Start-Process -FilePath ".\$launcherFileName" -PassThru -NoNewWindow -RedirectStandardOutput "$testResultsPath\launcher-stdout-$($testResult.Timestamp).txt" -RedirectStandardError "$testResultsPath\launcher-stderr-$($testResult.Timestamp).txt"
            
            # Wait for launcher to complete (with timeout)
            $launcherCompleted = $launcherProcess.WaitForExit(60000) # 60 second timeout
            
            if ($launcherCompleted) {
                $testResult.LauncherStatus = "COMPLETED"
                Write-TestLog "Launcher completed with exit code: $($launcherProcess.ExitCode)" "INFO"
                
                # Check for AutoCAD process
                $autocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
                if ($autocadProcess) {
                    $testResult.AutoCADStatus = "RUNNING"
                    Write-TestLog "AutoCAD process detected (PID: $($autocadProcess.Id))" "SUCCESS"
                } else {
                    $testResult.AutoCADStatus = "NOT_RUNNING"
                    $testResult.Warnings += "AutoCAD process not detected after launcher execution"
                    Write-TestLog "AutoCAD process not detected" "WARNING"
                }
                
            } else {
                $testResult.LauncherStatus = "TIMEOUT"
                $testResult.Errors += "Launcher timed out after 60 seconds"
                Write-TestLog "Launcher timed out after 60 seconds" "ERROR"
                
                # Kill the process if still running
                if (!$launcherProcess.HasExited) {
                    $launcherProcess.Kill()
                }
            }
            
            # Restore original working directory
            Set-Location $originalLocation
            Write-TestLog "Restored working directory to: $(Get-Location)" "INFO"
            
            $testResult.LogFiles += "$testResultsPath\launcher-stdout-$($testResult.Timestamp).txt"
            $testResult.LogFiles += "$testResultsPath\launcher-stderr-$($testResult.Timestamp).txt"
            
        } catch {
            $testResult.LauncherStatus = "ERROR"
            $testResult.Errors += "Launcher execution error: $($_.Exception.Message)"
            Write-TestLog "Launcher execution error: $($_.Exception.Message)" "ERROR"
            
            # Restore original working directory in case of error
            if ($originalLocation) {
                Set-Location $originalLocation
                Write-TestLog "Restored working directory after error to: $(Get-Location)" "INFO"
            }
        }
    } else {
        Write-TestLog "Skipping launcher execution due to build failure" "WARNING"
        $testResult.LauncherStatus = "SKIPPED"
    }
    
    # Step 3: Collect additional system information
    Write-TestLog "Step 3: Collecting system information..." "INFO"
    
    # Check for launcher-generated logs
    $launcherLogs = Get-ChildItem "$testResultsPath\launcher-execution-*.md" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime | Select-Object -Last 1
    if ($launcherLogs) {
        $testResult.LogFiles += $launcherLogs.FullName
        Write-TestLog "Found launcher log: $($launcherLogs.Name)" "INFO"
    }
    
    # Generate summary and recommendations
    if ($testResult.BuildStatus -eq "FAILED") {
        $testResult.NextActions += "Fix build errors and dependency issues"
        $testResult.NextActions += "Verify AutoCAD DLL references"
        $testResult.NextActions += "Check .NET 8.0 SDK installation"
    }
    
    if ($testResult.LauncherStatus -ne "COMPLETED") {
        $testResult.NextActions += "Debug launcher script execution"
        $testResult.NextActions += "Check PowerShell execution policies"
        $testResult.NextActions += "Verify directory permissions"
    }
    
    if ($testResult.AutoCADStatus -ne "RUNNING") {
        $testResult.NextActions += "Investigate AutoCAD startup issues"
        $testResult.NextActions += "Check AutoCAD Map3D 2025 installation"
        $testResult.NextActions += "Verify LISP file loading"
    }
    
    Write-TestLog "Test execution completed" "SUCCESS"
    return $testResult
}

# Function: Generate comprehensive test report
function Generate-TestReport {
    param([object]$TestResult)
    
    Write-TestLog "Generating comprehensive test report..." "INFO"
    
    $reportFile = "$testResultsPath\test-report-$($TestResult.Timestamp).md"
    
    $report = @"
# ADDS25 Automated Test Report

**Test Execution Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Session ID**: $($TestResult.Timestamp)

---

## TEST RESULTS SUMMARY

| Component | Status | Details |
|-----------|--------|---------|
| **Build System** | $($TestResult.BuildStatus) | .NET 8.0 compilation |
| **Launcher** | $($TestResult.LauncherStatus) | ADDS25-Launcher.bat execution |
| **AutoCAD Integration** | $($TestResult.AutoCADStatus) | Map3D 2025 startup |

---

## DETAILED ANALYSIS

### Build Status: $($TestResult.BuildStatus)
"@

    if ($TestResult.BuildStatus -eq "SUCCESS") {
        $report += "`n[SUCCESS] **ADDS25 solution compiled successfully**`n"
    } else {
        $report += "`n[FAILED] **Build failed** - See build output for details`n"
    }

    $report += @"

### Launcher Status: $($TestResult.LauncherStatus)
"@

    switch ($TestResult.LauncherStatus) {
        "COMPLETED" { $report += "`n[SUCCESS] **Launcher executed successfully**`n" }
        "TIMEOUT" { $report += "`n[TIMEOUT] **Launcher timed out** - May indicate startup issues`n" }
        "ERROR" { $report += "`n[ERROR] **Launcher execution failed** - Check error logs`n" }
        "SKIPPED" { $report += "`n[SKIPPED] **Launcher skipped** - Build failure prevented execution`n" }
    }

    $report += @"

### AutoCAD Integration: $($TestResult.AutoCADStatus)
"@

    switch ($TestResult.AutoCADStatus) {
        "RUNNING" { $report += "`n[SUCCESS] **AutoCAD started successfully** - Integration working`n" }
        "NOT_RUNNING" { $report += "`n[FAILED] **AutoCAD not detected** - Integration failed`n" }
        default { $report += "`n[UNKNOWN] **AutoCAD status unknown** - Launcher did not complete`n" }
    }

    # Add errors section
    if ($TestResult.Errors.Count -gt 0) {
        $report += @"

---

## ERRORS DETECTED

"@
        foreach ($error in $TestResult.Errors) {
            $report += "- [ERROR] $error`n"
        }
    }

    # Add warnings section
    if ($TestResult.Warnings.Count -gt 0) {
        $report += @"

---

## WARNINGS

"@
        foreach ($warning in $TestResult.Warnings) {
            $report += "- [WARNING] $warning`n"
        }
    }

    # Add next actions
    if ($TestResult.NextActions.Count -gt 0) {
        $report += @"

---

## RECOMMENDED ACTIONS

"@
        foreach ($action in $TestResult.NextActions) {
            $report += "- [ACTION] $action`n"
        }
    }

    # Add log files section
    if ($TestResult.LogFiles.Count -gt 0) {
        $report += @"

---

## GENERATED LOG FILES

"@
        foreach ($logFile in $TestResult.LogFiles) {
            $report += "- [LOG] $(Split-Path $logFile -Leaf)`n"
        }
    }

    $report += @"

---

## CI INTEGRATION

**Next Steps**:
1. This report will be automatically pushed to GitHub
2. Dev computer will analyze results with Master Protocol
3. Automated fixes will be generated and pushed back
4. Test cycle will repeat until success

**Test Session Complete**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
"@

    $report | Out-File $reportFile -Encoding UTF8
    Write-TestLog "Test report generated: $(Split-Path $reportFile -Leaf)" "SUCCESS"
    
    return $reportFile
}

# Function: Push test results to GitHub
function Push-TestResults {
    param([string]$ReportFile)
    
    Write-TestLog "Pushing test results to GitHub..." "INFO"
    
    Set-Location $repoPath
    
    try {
        # Add all test results
        git add test-results\*
        
        # Create commit message
        $commitMessage = "ADDS25 Test Results: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') - Automated CI Test"
        
        # Commit results
        git commit -m $commitMessage
        
        # Push to GitHub
        git push origin main
        
        Write-TestLog "Test results successfully pushed to GitHub" "SUCCESS"
        
    } catch {
        Write-TestLog "Error pushing to GitHub: $($_.Exception.Message)" "ERROR"
    }
}

# Function: Monitor for dev computer fixes
function Start-FixMonitoring {
    Write-TestLog "Starting GitHub monitoring for dev computer fixes..." "INFO"
    
    $lastCommit = ""
    
    while ($true) {
        try {
            # Pull latest changes
            Set-Location $repoPath
            git pull origin main --quiet
            
            # Check for new commits
            $currentCommit = git rev-parse HEAD
            
            if ($currentCommit -ne $lastCommit) {
                Write-TestLog "New commit detected: $currentCommit" "INFO"
                
                # Check if this is a fix commit (not our own test results)
                $commitMessage = git log -1 --pretty=format:"%s"
                
                if ($commitMessage -notlike "*Test Results*" -and $commitMessage -notlike "*test results*") {
                    Write-TestLog "Fix commit detected: $commitMessage" "INFO"
                    
                    # Wait a moment for any file operations to complete
                    Start-Sleep -Seconds 5
                    
                    # Execute test cycle
                    $testResult = Invoke-ADDS25Test
                    $reportFile = Generate-TestReport -TestResult $testResult
                    Push-TestResults -ReportFile $reportFile
                    
                    Write-TestLog "Test cycle complete. Waiting for next fix..." "SUCCESS"
                }
                
                $lastCommit = $currentCommit
            }
            
            # Wait 30 seconds before next check
            Start-Sleep -Seconds 30
            
        } catch {
            Write-TestLog "Error in monitoring loop: $($_.Exception.Message)" "ERROR"
            Start-Sleep -Seconds 60
        }
    }
}

# Main execution
Write-TestLog "ADDS25 Test Computer Automated CI System Initialized" "SUCCESS"
Write-TestLog "Repository: $repoPath" "INFO"
Write-TestLog "ADDS25 Path: $adds25Path" "INFO"
Write-TestLog "Test Results Path: $testResultsPath" "INFO"
Write-TestLog "CI Log: $ciLog" "INFO"

Write-Host ""
Write-Host "Starting automated testing..." -ForegroundColor Green
Write-Host "This system will:" -ForegroundColor Yellow
Write-Host "  1. Monitor GitHub for fix commits from dev computer" -ForegroundColor White
Write-Host "  2. Automatically pull, build, and test ADDS25" -ForegroundColor White
Write-Host "  3. Generate comprehensive test reports" -ForegroundColor White
Write-Host "  4. Push results back to GitHub for analysis" -ForegroundColor White
Write-Host "  5. Wait for new fixes and repeat" -ForegroundColor White
Write-Host ""

# Execute initial test to establish baseline
Write-TestLog "Executing initial test to establish baseline..." "INFO"
$initialTest = Invoke-ADDS25Test
$initialReport = Generate-TestReport -TestResult $initialTest
Push-TestResults -ReportFile $initialReport

# Start the monitoring loop
Start-FixMonitoring
