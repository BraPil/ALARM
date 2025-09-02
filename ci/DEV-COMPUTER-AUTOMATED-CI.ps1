# ADDS25 Dev Computer Automated CI System
# Purpose: Listen for test result commits, auto-analyze with Master Protocol, generate fixes
# Environment: Dev Computer (kidsg)
# Date: September 1, 2025

Write-Host "🚀 ADDS25 Dev Computer Automated CI System Starting..." -ForegroundColor Cyan
Write-Host "This system will monitor GitHub for test results and auto-generate fixes" -ForegroundColor Yellow

# Configuration
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
$testResultsPath = "$repoPath\test-results"
$ciLogPath = "$repoPath\ci\logs"
$webhookPort = 8080

# Ensure directories exist
if (!(Test-Path $ciLogPath)) { New-Item $ciLogPath -Type Directory -Force | Out-Null }

# Initialize CI logging
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$ciLog = "$ciLogPath\dev-ci-session-$timestamp.md"

@"
# ADDS25 Dev Computer CI Session

**Session Start**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: Dev Computer (kidsg)
**Repository**: C:\Users\kidsg\Downloads\ALARM
**Webhook Port**: $webhookPort

---

## 🎯 **CI AUTOMATION LOG**

"@ | Out-File $ciLog -Encoding UTF8

function Write-CILog {
    param([string]$Message, [string]$Level = "INFO")
    $logEntry = "[$(Get-Date -Format 'HH:mm:ss')] [$Level] $Message"
    Write-Host $logEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } })
    Add-Content $ciLog $logEntry -Encoding UTF8
}

# Function: Analyze test results with Master Protocol
function Invoke-MasterProtocolAnalysis {
    param([string]$TestResultFile)
    
    Write-CILog "🎯 MASTER PROTOCOL ENGAGEMENT - Analyzing: $TestResultFile" "INFO"
    
    # Read test results completely (Anti-Sampling Directive)
    $testContent = Get-Content $TestResultFile -Raw
    Write-CILog "📖 Read complete test results file: $(($testContent -split "`n").Count) lines" "INFO"
    
    # Master Protocol Analysis
    $analysisResult = @{
        Errors = @()
        Warnings = @()
        Fixes = @()
        BuildStatus = "UNKNOWN"
        AutoCADStatus = "UNKNOWN"
        NextActions = @()
    }
    
    # Analyze build status
    if ($testContent -match "Build FAILED|❌.*Build|ERROR.*build") {
        $analysisResult.BuildStatus = "FAILED"
        $analysisResult.Errors += "Build failure detected"
        
        # Check for specific error patterns
        if ($testContent -match "Could not resolve.*AcCoreMgd|Could not resolve.*AcDbMgd") {
            $analysisResult.Fixes += "AutoCAD DLL path correction required"
        }
        if ($testContent -match "NETSDK1045|does not support targeting \.NET 8\.0") {
            $analysisResult.Fixes += ".NET 8.0 SDK installation required"
        }
        if ($testContent -match "CS0246.*Autodesk") {
            $analysisResult.Fixes += "AutoCAD namespace resolution required"
        }
    } elseif ($testContent -match "Build succeeded|✅.*Build|BUILD SUCCESSFUL") {
        $analysisResult.BuildStatus = "SUCCESS"
    }
    
    # Analyze AutoCAD status
    if ($testContent -match "AutoCAD.*RUNNING|acad.*PID") {
        $analysisResult.AutoCADStatus = "RUNNING"
    } elseif ($testContent -match "AutoCAD.*NOT.*RUNNING|AutoCAD.*NOT DETECTED") {
        $analysisResult.AutoCADStatus = "NOT_RUNNING"
        $analysisResult.Warnings += "AutoCAD did not start properly"
    }
    
    # Generate specific fixes based on analysis
    if ($analysisResult.BuildStatus -eq "FAILED") {
        $analysisResult.NextActions += "Update project references"
        $analysisResult.NextActions += "Verify AutoCAD installation paths"
        $analysisResult.NextActions += "Test build system"
    }
    
    if ($analysisResult.AutoCADStatus -eq "NOT_RUNNING") {
        $analysisResult.NextActions += "Check AutoCAD launcher scripts"
        $analysisResult.NextActions += "Verify AutoCAD Map3D 2025 installation"
        $analysisResult.NextActions += "Update launcher timeout settings"
    }
    
    Write-CILog "📊 Analysis Complete: Build=$($analysisResult.BuildStatus), AutoCAD=$($analysisResult.AutoCADStatus)" "INFO"
    return $analysisResult
}

# Function: Generate automated fixes
function Generate-AutomatedFixes {
    param([object]$AnalysisResult, [string]$TestResultFile)
    
    Write-CILog "🔧 Generating automated fixes based on analysis..." "INFO"
    
    $fixesApplied = @()
    
    # Fix 1: AutoCAD DLL Path Issues
    if ($AnalysisResult.Fixes -contains "AutoCAD DLL path correction required") {
        Write-CILog "🔧 Applying AutoCAD DLL path fix..." "INFO"
        
        $autocadProject = "$repoPath\tests\ADDS25\v0.1\ADDS25.AutoCAD\ADDS25.AutoCAD.csproj"
        if (Test-Path $autocadProject) {
            $content = Get-Content $autocadProject -Raw
            $originalContent = $content
            
            # Update DLL paths to standard AutoCAD installation
            $content = $content -replace 'C:\\Users\\wa-bdpilegg\\Downloads\\ADDS\\ADDS25v1\.1\\Documentation\\objectarx-for-autocad-2025-win-64bit-dlm\\inc\\', 'C:\Program Files\Autodesk\AutoCAD 2025\'
            
            if ($content -ne $originalContent) {
                Set-Content $autocadProject $content -Encoding UTF8
                $fixesApplied += "Updated AutoCAD DLL paths to standard installation location"
                Write-CILog "✅ AutoCAD DLL paths updated" "SUCCESS"
            }
        }
    }
    
    # Fix 2: .NET 8.0 SDK Issues
    if ($AnalysisResult.Fixes -contains ".NET 8.0 SDK installation required") {
        Write-CILog "🔧 Creating .NET 8.0 SDK installation script..." "INFO"
        
        $sdkInstallScript = @"
# Auto-generated .NET 8.0 SDK installation script
Write-Host "🔧 Installing .NET 8.0 SDK..." -ForegroundColor Cyan
winget install Microsoft.DotNet.SDK.8 --accept-package-agreements --accept-source-agreements
Write-Host "✅ .NET 8.0 SDK installation complete" -ForegroundColor Green
"@
        
        Set-Content "$repoPath\test-results\auto-install-net8.ps1" $sdkInstallScript -Encoding UTF8
        $fixesApplied += "Created .NET 8.0 SDK auto-installation script"
        Write-CILog "✅ .NET 8.0 SDK installation script created" "SUCCESS"
    }
    
    # Fix 3: Launcher timeout adjustments
    if ($AnalysisResult.AutoCADStatus -eq "NOT_RUNNING") {
        Write-CILog "🔧 Adjusting launcher timeout settings..." "INFO"
        
        $launcher = "$repoPath\tests\ADDS25\v0.1\ADDS25-Launcher.bat"
        if (Test-Path $launcher) {
            $content = Get-Content $launcher -Raw
            $originalContent = $content
            
            # Increase timeout for AutoCAD startup
            $content = $content -replace 'timeout /t 3 /nobreak', 'timeout /t 10 /nobreak'
            
            if ($content -ne $originalContent) {
                Set-Content $launcher $content -Encoding UTF8
                $fixesApplied += "Increased AutoCAD startup timeout to 10 seconds"
                Write-CILog "✅ Launcher timeout increased" "SUCCESS"
            }
        }
    }
    
    Write-CILog "🎯 Applied $($fixesApplied.Count) automated fixes" "SUCCESS"
    return $fixesApplied
}

# Function: Commit and push fixes
function Commit-AutomatedFixes {
    param([array]$FixesApplied)
    
    if ($FixesApplied.Count -eq 0) {
        Write-CILog "ℹ️ No fixes to commit" "INFO"
        return
    }
    
    Write-CILog "📤 Committing and pushing automated fixes..." "INFO"
    
    Set-Location $repoPath
    
    # Stage all changes
    git add -A
    
    # Create commit message
    $commitMessage = "ADDS25 Automated CI Fixes:`n`n" + ($FixesApplied -join "`n- ")
    
    # Commit changes
    git commit -m $commitMessage
    
    # Push to GitHub
    git push origin main
    
    Write-CILog "✅ Automated fixes committed and pushed to GitHub" "SUCCESS"
}

# Function: Monitor for test result changes
function Start-GitHubMonitoring {
    Write-CILog "👁️ Starting GitHub monitoring for test result changes..." "INFO"
    
    $lastCommit = ""
    
    while ($true) {
        try {
            # Pull latest changes
            Set-Location $repoPath
            git pull origin main --quiet
            
            # Check for new test results
            $currentCommit = git rev-parse HEAD
            
            if ($currentCommit -ne $lastCommit) {
                Write-CILog "🔔 New commit detected: $currentCommit" "INFO"
                
                # Check if test results were updated
                $testResultFiles = Get-ChildItem "$testResultsPath\launcher-execution-*.md" | Sort-Object LastWriteTime | Select-Object -Last 1
                
                if ($testResultFiles) {
                    Write-CILog "📊 New test results found: $($testResultFiles.Name)" "INFO"
                    
                    # Analyze with Master Protocol
                    $analysis = Invoke-MasterProtocolAnalysis -TestResultFile $testResultFiles.FullName
                    
                    # Generate and apply fixes
                    $fixes = Generate-AutomatedFixes -AnalysisResult $analysis -TestResultFile $testResultFiles.FullName
                    
                    # Commit fixes if any were applied
                    Commit-AutomatedFixes -FixesApplied $fixes
                    
                    Write-CILog "🔄 CI cycle complete. Waiting for next test results..." "SUCCESS"
                }
                
                $lastCommit = $currentCommit
            }
            
            # Wait 30 seconds before next check
            Start-Sleep -Seconds 30
            
        } catch {
            Write-CILog "❌ Error in monitoring loop: $($_.Exception.Message)" "ERROR"
            Start-Sleep -Seconds 60
        }
    }
}

# Main execution
Write-CILog "🎯 ADDS25 Dev Computer Automated CI System Initialized" "SUCCESS"
Write-CILog "📁 Repository: $repoPath" "INFO"
Write-CILog "📊 Test Results Path: $testResultsPath" "INFO"
Write-CILog "📝 CI Log: $ciLog" "INFO"

Write-Host ""
Write-Host "🚀 Starting automated monitoring..." -ForegroundColor Green
Write-Host "This system will:" -ForegroundColor Yellow
Write-Host "  1. Monitor GitHub for test result commits" -ForegroundColor White
Write-Host "  2. Automatically analyze results with Master Protocol" -ForegroundColor White
Write-Host "  3. Generate and apply fixes" -ForegroundColor White
Write-Host "  4. Push fixes back to GitHub" -ForegroundColor White
Write-Host "  5. Wait for test computer to test and repeat" -ForegroundColor White
Write-Host ""

# Start the monitoring loop
Start-GitHubMonitoring
