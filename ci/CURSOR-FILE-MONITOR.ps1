# ADDS25 Cursor File Monitor
# Purpose: Monitor for trigger files and automatically execute analysis
# Environment: Dev Computer (kidsg) - Run alongside Cursor
# Date: September 2, 2025

Write-Host "*** ADDS25 Cursor File Monitor Starting ***" -ForegroundColor Cyan
Write-Host "Purpose: Monitor for analysis trigger files and auto-execute" -ForegroundColor Yellow
Write-Host ""

# Configuration
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
$triggerPattern = "$repoPath\CURSOR-ANALYZE-NOW.trigger"
$monitorInterval = 2 # seconds

Write-Host "Monitoring: $triggerPattern" -ForegroundColor Green
Write-Host "Interval: $monitorInterval seconds" -ForegroundColor Green
Write-Host ""
Write-Host "This monitor will:" -ForegroundColor Yellow
Write-Host "  1. Watch for trigger files created by PowerShell CI" -ForegroundColor White
Write-Host "  2. Automatically execute Cursor analysis commands" -ForegroundColor White
Write-Host "  3. Clean up trigger files after processing" -ForegroundColor White
Write-Host "  4. Log all automatic analysis activities" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop monitoring" -ForegroundColor Yellow
Write-Host ""

$lastProcessed = ""

while ($true) {
    try {
        if (Test-Path $triggerPattern) {
            $triggerFile = Get-Item $triggerPattern
            $currentHash = $triggerFile.LastWriteTime.ToString()
            
            # Only process if this is a new/updated trigger file
            if ($currentHash -ne $lastProcessed) {
                Write-Host "[$(Get-Date -Format 'HH:mm:ss')] TRIGGER DETECTED: $($triggerFile.Name)" -ForegroundColor Green
                
                # Read trigger data
                $triggerData = Get-Content $triggerPattern -Raw | ConvertFrom-Json
                
                Write-Host "  Commit: $($triggerData.commit)" -ForegroundColor Cyan
                Write-Host "  Message: $($triggerData.message)" -ForegroundColor Cyan
                Write-Host "  Timestamp: $($triggerData.timestamp)" -ForegroundColor Cyan
                Write-Host "  Action: $($triggerData.action)" -ForegroundColor Yellow
                
                # AUTOMATIC ANALYSIS EXECUTION
                Write-Host ""
                Write-Host "🤖 EXECUTING AUTOMATIC ANALYSIS..." -ForegroundColor Green
                Write-Host ""
                
                # Step 1: Pull latest changes
                Write-Host "Step 1: Pulling latest changes..." -ForegroundColor Yellow
                Set-Location $repoPath
                $pullResult = git pull origin main 2>&1
                Write-Host "Git pull result: $pullResult" -ForegroundColor Gray
                
                # Step 2: Find latest test results (with time zone consideration)
                Write-Host "Step 2: Finding latest test results (time zone aware)..." -ForegroundColor Yellow
                $testResults = @()
                foreach ($pattern in $triggerData.files_to_check) {
                    $files = Get-ChildItem $pattern -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending
                    if ($files) {
                        $testResults += $files
                        Write-Host "  Found: $($files.Count) files matching $pattern" -ForegroundColor Gray
                    }
                }
                
                if ($testResults) {
                    $latestResult = $testResults | Sort-Object LastWriteTime -Descending | Select-Object -First 1
                    Write-Host "  Latest result: $($latestResult.Name)" -ForegroundColor Green
                    Write-Host "  File time: $($latestResult.LastWriteTime)" -ForegroundColor Green
                    
                    # Step 3: Display analysis command for user
                    Write-Host ""
                    Write-Host "🎯 READY FOR CURSOR ANALYSIS!" -ForegroundColor Green
                    Write-Host "===============================================" -ForegroundColor Yellow
                    Write-Host "AUTOMATIC ANALYSIS READY - SEND TO CURSOR:" -ForegroundColor Cyan
                    Write-Host ""
                    Write-Host "New test results detected - analyze now with time zone adjustment" -ForegroundColor White
                    Write-Host "Latest file: $($latestResult.Name)" -ForegroundColor White
                    Write-Host "Commit: $($triggerData.commit)" -ForegroundColor White
                    Write-Host "Time: $($triggerData.timestamp) ($($triggerData.timezone))" -ForegroundColor White
                    Write-Host ""
                    Write-Host "CURSOR: Pull latest changes and analyze the above test results" -ForegroundColor Green
                    Write-Host "===============================================" -ForegroundColor Yellow
                    
                } else {
                    Write-Host "  No test result files found - may still be processing" -ForegroundColor Yellow
                    Write-Host "  Will continue monitoring for results..." -ForegroundColor Yellow
                }
                
                # Step 4: Clean up trigger file
                Remove-Item $triggerPattern -Force
                Write-Host ""
                Write-Host "✅ Trigger file processed and cleaned up" -ForegroundColor Green
                Write-Host "Continuing to monitor for next trigger..." -ForegroundColor Cyan
                Write-Host ""
                
                $lastProcessed = $currentHash
            }
        }
        
        Start-Sleep -Seconds $monitorInterval
        
    } catch {
        Write-Host "Error in monitoring loop: $($_.Exception.Message)" -ForegroundColor Red
        Start-Sleep -Seconds 5
    }
}
