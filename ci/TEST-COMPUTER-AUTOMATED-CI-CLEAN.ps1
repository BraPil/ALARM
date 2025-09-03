# ADDS25 Test Computer Automated CI System - CLEAN VERSION
# Purpose: Listen for fix commits, auto-pull, test, and push results
# Environment: Test Computer (wa-bdpilegg)
# Date: September 2, 2025

Write-Host "ADDS25 Test Computer Automated CI System - CLEAN VERSION" -ForegroundColor Cyan
Write-Host "This system will monitor GitHub for fixes and auto-test ADDS25" -ForegroundColor Yellow
Write-Host ""

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
# ADDS25 Test Computer CI Session - CLEAN VERSION

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
    
    $testResult = @{
        Timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
        BuildStatus = "UNKNOWN"
        LauncherStatus = "UNKNOWN"
        AutoCADStatus = "UNKNOWN"
        Errors = @()
        Warnings = @()
        LogFiles = @()
        TestDuration = 0
    }
    
    $testStart = Get-Date
    
    try {
        # Step 1: Build ADDS25
        Write-TestLog "Building ADDS25 solution..." "INFO"
        Set-Location $adds25Path
        
        $buildOutput = & dotnet build --configuration Debug 2>&1
        $buildLogFile = "$testResultsPath\build-output-$($testResult.Timestamp).txt"
        $buildOutput | Out-File $buildLogFile -Encoding UTF8
        $testResult.LogFiles += $buildLogFile
        
        if ($LASTEXITCODE -eq 0) {
            $testResult.BuildStatus = "SUCCESS"
            Write-TestLog "Build completed successfully" "SUCCESS"
        } else {
            $testResult.BuildStatus = "FAILED"
            $testResult.Errors += "Build failed with exit code: $LASTEXITCODE"
            Write-TestLog "Build failed with exit code: $LASTEXITCODE" "ERROR"
        }
        
        # Step 2: Execute Pure PowerShell Launcher
        if ($testResult.BuildStatus -eq "SUCCESS") {
            Write-TestLog "Executing ADDS25 Pure PowerShell Launcher..." "INFO"
            
            $launcherPath = "$adds25Path\ADDS25-Launcher.ps1"
            if (Test-Path $launcherPath) {
                try {
                    # Execute launcher in separate process to capture all output
                    $launcherJob = Start-Job -ScriptBlock {
                        param($LauncherPath, $WorkingDir)
                        Set-Location $WorkingDir
                        & $LauncherPath
                    } -ArgumentList $launcherPath, $adds25Path
                    
                    # Wait for launcher completion (5 minute timeout)
                    $launcherCompleted = Wait-Job $launcherJob -Timeout 300
                    
                    if ($launcherCompleted) {
                        $launcherOutput = Receive-Job $launcherJob
                        $testResult.LauncherStatus = "COMPLETED"
                        Write-TestLog "Launcher completed successfully" "SUCCESS"
                        
                        # Save launcher output
                        $launcherOutputFile = "$testResultsPath\launcher-output-$($testResult.Timestamp).txt"
                        $launcherOutput | Out-File $launcherOutputFile -Encoding UTF8
                        $testResult.LogFiles += $launcherOutputFile
                        
                    } else {
                        $testResult.LauncherStatus = "TIMEOUT"
                        $testResult.Errors += "Launcher timed out after 5 minutes"
                        Write-TestLog "Launcher timed out after 5 minutes" "ERROR"
                        Stop-Job $launcherJob -Force
                    }
                    
                    Remove-Job $launcherJob -Force
                    
                } catch {
                    $testResult.LauncherStatus = "ERROR"
                    $testResult.Errors += "Launcher execution error: $($_.Exception.Message)"
                    Write-TestLog "Launcher execution error: $($_.Exception.Message)" "ERROR"
                }
            } else {
                $testResult.LauncherStatus = "NOT_FOUND"
                $testResult.Errors += "Launcher not found: $launcherPath"
                Write-TestLog "Launcher not found: $launcherPath" "ERROR"
            }
        }
        
        # Step 3: Check AutoCAD status
        Write-TestLog "Checking AutoCAD process status..." "INFO"
        $autocadProcess = Get-Process -Name "acad" -ErrorAction SilentlyContinue
        if ($autocadProcess) {
            $testResult.AutoCADStatus = "RUNNING"
            Write-TestLog "AutoCAD process detected (PID: $($autocadProcess.Id))" "SUCCESS"
        } else {
            $testResult.AutoCADStatus = "NOT_RUNNING"
            $testResult.Warnings += "AutoCAD process not detected"
            Write-TestLog "AutoCAD process not detected" "WARNING"
        }
        
    } catch {
        $testResult.Errors += "Test execution error: $($_.Exception.Message)"
        Write-TestLog "Test execution error: $($_.Exception.Message)" "ERROR"
    }
    
    $testEnd = Get-Date
    $testResult.TestDuration = [math]::Round(($testEnd - $testStart).TotalSeconds, 2)
    
    Write-TestLog "Test execution completed in $($testResult.TestDuration) seconds" "INFO"
    return $testResult
}

# Function: Generate comprehensive test report
function Generate-TestReport {
    param($TestResult)
    
    $reportFile = "$testResultsPath\test-report-$($TestResult.Timestamp).md"
    
    $report = @"
# ADDS25 Automated Test Report - CLEAN VERSION

**Test Execution Time**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Test Computer (wa-bdpilegg)
**Session ID**: $($TestResult.Timestamp)
**Test Duration**: $($TestResult.TestDuration) seconds

---

## TEST RESULTS SUMMARY

| Component | Status | Details |
|-----------|--------|---------|
| **Build System** | $($TestResult.BuildStatus) | .NET Core 8 compilation |
| **Launcher** | $($TestResult.LauncherStatus) | Pure PowerShell launcher |
| **AutoCAD Integration** | $($TestResult.AutoCADStatus) | Map3D 2025 startup |

---

## DETAILED ANALYSIS

### Build Status: $($TestResult.BuildStatus)
$(if ($TestResult.BuildStatus -eq "SUCCESS") { "[SUCCESS] ADDS25 solution compiled successfully" } else { "[ERROR] Build failed - Check build logs" })

### Launcher Status: $($TestResult.LauncherStatus)
$(if ($TestResult.LauncherStatus -eq "COMPLETED") { "[SUCCESS] Pure PowerShell launcher executed successfully" } else { "[ERROR] Launcher execution failed - Check launcher logs" })

### AutoCAD Integration: $($TestResult.AutoCADStatus)
$(if ($TestResult.AutoCADStatus -eq "RUNNING") { "[SUCCESS] AutoCAD Map3D 2025 is running" } else { "[WARNING] AutoCAD process not detected" })

---

## ERRORS DETECTED
$(if ($TestResult.Errors.Count -gt 0) { ($TestResult.Errors | ForEach-Object { "- [ERROR] $_" }) -join "`n" } else { "- No errors detected" })

---

## WARNINGS
$(if ($TestResult.Warnings.Count -gt 0) { ($TestResult.Warnings | ForEach-Object { "- [WARNING] $_" }) -join "`n" } else { "- No warnings" })

---

## GENERATED LOG FILES
$(($TestResult.LogFiles | ForEach-Object { "- [LOG] $(Split-Path $_ -Leaf)" }) -join "`n")

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
    Write-TestLog "Test report generated: $reportFile" "SUCCESS"
    return $reportFile
}

# Function: Push test results to GitHub
function Push-TestResults {
    param([string]$ReportFile)
    
    Write-TestLog "Pushing test results to GitHub..." "INFO"
    
    try {
        Set-Location $repoPath
        
        # Add test results
        git add test-results\*
        
        # Commit with descriptive message
        $commitMessage = "ADDS25 Test Results: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') - Automated CI test execution"
        git commit -m $commitMessage
        
        # Push to GitHub
        git push origin main
        
        Write-TestLog "Test results pushed to GitHub successfully" "SUCCESS"
        
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
Write-TestLog "ADDS25 Test Computer Automated CI System Initialized - CLEAN VERSION" "SUCCESS"
Write-TestLog "Repository: $repoPath" "INFO"
Write-TestLog "ADDS25 Path: $adds25Path" "INFO"
Write-TestLog "Test Results Path: $testResultsPath" "INFO"
Write-TestLog "CI Log: $ciLog" "INFO"

Write-Host ""
Write-Host "Starting automated monitoring..." -ForegroundColor Green
Write-Host "This system will:" -ForegroundColor Yellow
Write-Host "  1. Monitor GitHub for dev computer fixes" -ForegroundColor White
Write-Host "  2. Automatically pull latest changes" -ForegroundColor White
Write-Host "  3. Build and test ADDS25 with Pure PowerShell launcher" -ForegroundColor White
Write-Host "  4. Generate comprehensive test reports" -ForegroundColor White
Write-Host "  5. Push results back to GitHub" -ForegroundColor White
Write-Host "  6. Trigger next CI cycle" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the CI system" -ForegroundColor Yellow
Write-Host ""

# Start the monitoring loop
Start-FixMonitoring
