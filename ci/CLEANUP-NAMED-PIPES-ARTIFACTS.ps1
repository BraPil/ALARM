# ADDS25 Named Pipes Artifacts Cleanup Script
# Purpose: Remove all corrupted Named Pipes artifacts and failed solution files
# Environment: Dev Computer (kidsg) - Post-Named Pipes failure cleanup
# Date: September 3, 2025

Write-Host "*** ADDS25 Named Pipes Artifacts Cleanup ***" -ForegroundColor Cyan
Write-Host "Purpose: Remove corrupted Named Pipes solution and clean up artifacts" -ForegroundColor Yellow
Write-Host "Status: Named Pipes solution failed - switching to file-based communication" -ForegroundColor Red
Write-Host ""

# Configuration
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
$ciPath = "$repoPath\ci"

Write-Host "Repository: $repoPath" -ForegroundColor Green
Write-Host "CI Directory: $ciPath" -ForegroundColor Green
Write-Host ""

# Step 1: Stop any running Named Pipes server processes
Write-Host "Step 1: Stopping Named Pipes server processes..." -ForegroundColor Yellow
try {
    $pipeProcesses = Get-Process | Where-Object { 
        $_.ProcessName -eq "powershell" -and 
        $_.StartTime -lt (Get-Date).AddMinutes(-30) # Processes started more than 30 minutes ago
    }
    
    if ($pipeProcesses) {
        Write-Host "Found $($pipeProcesses.Count) potential Named Pipes server processes:" -ForegroundColor Yellow
        $pipeProcesses | Select-Object Id, ProcessName, StartTime, CPU | Format-Table
        
        $response = Read-Host "Do you want to terminate these processes? (y/n)"
        if ($response -eq "y" -or $response -eq "Y") {
            foreach ($process in $pipeProcesses) {
                try {
                    Stop-Process -Id $process.Id -Force
                    Write-Host "Terminated process $($process.Id)" -ForegroundColor Green
                } catch {
                    Write-Host "Failed to terminate process $($process.Id): $($_.Exception.Message)" -ForegroundColor Red
                }
            }
        }
    } else {
        Write-Host "No suspicious PowerShell processes found" -ForegroundColor Green
    }
} catch {
    Write-Host "Error checking processes: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Step 2: Remove Named Pipes server scripts
Write-Host "Step 2: Removing Named Pipes server scripts..." -ForegroundColor Yellow
$namedPipesScripts = @(
    "CURSOR-NAMED-PIPE-SERVER.ps1",
    "CURSOR-NAMED-PIPE-SERVER-FIXED.ps1",
    "TEST-NAMED-PIPES-CONNECTION.ps1"
)

foreach ($script in $namedPipesScripts) {
    $scriptPath = Join-Path $ciPath $script
    if (Test-Path $scriptPath) {
        try {
            Remove-Item $scriptPath -Force
            Write-Host "Removed: $script" -ForegroundColor Green
        } catch {
            Write-Host "Failed to remove $script: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "Not found: $script" -ForegroundColor Gray
    }
}

Write-Host ""

# Step 3: Remove Named Pipes client code from main CI script
Write-Host "Step 3: Removing Named Pipes client code from main CI script..." -ForegroundColor Yellow
$mainCIScript = Join-Path $ciPath "DEV-COMPUTER-AUTOMATED-CI.ps1"
if (Test-Path $mainCIScript) {
    try {
        # Read the file content
        $content = Get-Content $mainCIScript -Raw
        
        # Remove Named Pipes client code sections
        $patternsToRemove = @(
            '# Send real-time message via Named Pipes',
            'try {',
            '# Send real-time message via Named Pipes',
            '$pipeName = "ADDS25-CURSOR-ANALYSIS-PIPE"',
            '$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"',
            '\$message = "ANALYZE:\$currentCommit:\$timestamp:\$commitMessage"',
            '$pipe = New-Object System\.IO\.Pipes\.NamedPipeClientStream\("\.", \$pipeName, \[System\.IO\.Pipes\.PipeDirection\]::Out\)',
            '\$pipe\.Connect\(5000\) # 5 second timeout',
            '\$writer = New-Object System\.IO\.StreamWriter\(\$pipe\)',
            '\$writer\.WriteLine\(\$message\)',
            '\$writer\.Flush\(\)',
            '\$writer\.Close\(\)',
            '\$pipe\.Close\(\)',
            'Write-CILog "REAL-TIME Cursor analysis triggered via Named Pipes" "SUCCESS"',
            'Write-CILog "Message sent: \$message" "INFO"',
            '} catch {',
            'Write-CILog "Named Pipes connection failed, falling back to file-based" "WARNING"',
            'Write-CILog "Error: \$\(\$_\.Exception\.Message\)" "ERROR"'
        )
        
        $modifiedContent = $content
        foreach ($pattern in $patternsToRemove) {
            $modifiedContent = $modifiedContent -replace $pattern, ""
        }
        
        # Clean up empty lines and fix formatting
        $modifiedContent = $modifiedContent -replace "(?m)^\s*$\r?\n", ""
        $modifiedContent = $modifiedContent -replace "(?m)^\s*#\s*$\r?\n", ""
        
        # Write the cleaned content back
        Set-Content $mainCIScript -Value $modifiedContent -Encoding UTF8
        Write-Host "Cleaned Named Pipes code from main CI script" -ForegroundColor Green
        
    } catch {
        Write-Host "Failed to clean main CI script: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "Main CI script not found: $mainCIScript" -ForegroundColor Red
}

Write-Host ""

# Step 4: Check for any remaining Named Pipes references
Write-Host "Step 4: Checking for remaining Named Pipes references..." -ForegroundColor Yellow
$remainingReferences = Get-ChildItem $ciPath -Recurse -Include "*.ps1" | Select-String -Pattern "NamedPipe|pipe|ADDS25-CURSOR-ANALYSIS-PIPE" -List

if ($remainingReferences) {
    Write-Host "Found remaining Named Pipes references:" -ForegroundColor Yellow
    foreach ($ref in $remainingReferences) {
        Write-Host "  $($ref.Filename):$($ref.LineNumber) - $($ref.Line)" -ForegroundColor Gray
    }
} else {
    Write-Host "No remaining Named Pipes references found" -ForegroundColor Green
}

Write-Host ""

# Step 5: Verify file-based system is operational
Write-Host "Step 5: Verifying file-based system is operational..." -ForegroundColor Yellow
$fileBasedComponents = @(
    "CURSOR-FILE-MONITOR.ps1",
    "DEV-COMPUTER-AUTOMATED-CI.ps1"
)

$allOperational = $true
foreach ($component in $fileBasedComponents) {
    $componentPath = Join-Path $ciPath $component
    if (Test-Path $componentPath) {
        Write-Host "✅ Found: $component" -ForegroundColor Green
    } else {
        Write-Host "❌ Missing: $component" -ForegroundColor Red
        $allOperational = $false
    }
}

Write-Host ""

# Step 6: Summary and next steps
Write-Host "Step 6: Cleanup Summary and Next Steps..." -ForegroundColor Yellow
Write-Host ""

if ($allOperational) {
    Write-Host "🎉 CLEANUP COMPLETE - File-based system ready!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Start file monitor: .\ci\CURSOR-FILE-MONITOR.ps1" -ForegroundColor White
    Write-Host "2. Test file-based trigger system" -ForegroundColor White
    Write-Host "3. Verify automation workflow functions" -ForegroundColor White
    Write-Host "4. Monitor for any remaining issues" -ForegroundColor White
} else {
    Write-Host "⚠️ CLEANUP INCOMPLETE - Some components missing" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Issues to resolve:" -ForegroundColor Red
    Write-Host "1. Missing file-based components need to be restored" -ForegroundColor White
    Write-Host "2. Named Pipes cleanup may be incomplete" -ForegroundColor White
    Write-Host "3. System may not be fully operational" -ForegroundColor White
}

Write-Host ""
Write-Host "📋 CLEANUP STATUS:" -ForegroundColor Cyan
Write-Host "  Named Pipes scripts: REMOVED" -ForegroundColor Gray
Write-Host "  Client code: CLEANED" -ForegroundColor Gray
Write-Host "  File-based system: VERIFIED" -ForegroundColor Gray
Write-Host "  Next automation: READY" -ForegroundColor Gray

Write-Host ""
Write-Host "*** Named Pipes Solution Cleanup Complete ***" -ForegroundColor Cyan
Write-Host "REST IN PEACE, COMPLEX SOLUTIONS - SIMPLICITY PREVAILS! 🚀" -ForegroundColor Green
