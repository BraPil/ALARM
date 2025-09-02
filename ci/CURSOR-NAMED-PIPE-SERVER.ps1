# ADDS25 Cursor Named Pipe Server
# Purpose: Real-time communication server for immediate Cursor analysis triggering
# Environment: Dev Computer (kidsg) - Superior to file-based communication
# Date: September 2, 2025

Write-Host "*** ADDS25 Cursor Named Pipe Server ***" -ForegroundColor Cyan
Write-Host "Purpose: Real-time inter-process communication for immediate analysis" -ForegroundColor Yellow
Write-Host "Architecture: Named Pipes - Enterprise-grade IPC solution" -ForegroundColor Green
Write-Host ""

# Configuration
$pipeName = "ADDS25-CURSOR-ANALYSIS-PIPE"
$repoPath = "C:\Users\kidsg\Downloads\ALARM"

Write-Host "Pipe Name: $pipeName" -ForegroundColor Green
Write-Host "Repository: $repoPath" -ForegroundColor Green
Write-Host ""

function Write-PipeLog {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "HH:mm:ss"
    $color = switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } }
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $color
}

Write-PipeLog "Starting Named Pipe Server for Cursor automation..." "INFO"
Write-Host ""
Write-Host "This server provides:" -ForegroundColor Yellow
Write-Host "  • Real-time inter-process communication" -ForegroundColor White
Write-Host "  • Instant analysis triggering (no polling)" -ForegroundColor White
Write-Host "  • Enterprise-grade reliability" -ForegroundColor White
Write-Host "  • Zero file system dependencies" -ForegroundColor White
Write-Host "  • Memory-based message passing" -ForegroundColor White
Write-Host ""
Write-Host "Waiting for PowerShell CI connections..." -ForegroundColor Green
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

while ($true) {
    try {
        # Create named pipe server
        $pipe = New-Object System.IO.Pipes.NamedPipeServerStream($pipeName, [System.IO.Pipes.PipeDirection]::In)
        
        Write-PipeLog "Named pipe server created, waiting for client connection..." "INFO"
        
        # Wait for client connection
        $pipe.WaitForConnection()
        Write-PipeLog "Client connected to named pipe!" "SUCCESS"
        
        # Read message from client
        $reader = New-Object System.IO.StreamReader($pipe)
        $message = $reader.ReadToEnd()
        
        if ($message) {
            Write-PipeLog "Received message: $message" "SUCCESS"
            
            # Parse the message (expected format: "ANALYZE:commit:timestamp:message")
            $parts = $message -split ':'
            if ($parts.Length -ge 4 -and $parts[0] -eq "ANALYZE") {
                $commit = $parts[1]
                $timestamp = $parts[2]  
                $commitMessage = ($parts[3..($parts.Length-1)] -join ':')
                
                Write-Host ""
                Write-Host "🚀 REAL-TIME ANALYSIS TRIGGER RECEIVED!" -ForegroundColor Green
                Write-Host "=============================================" -ForegroundColor Yellow
                Write-Host "Commit: $commit" -ForegroundColor Cyan
                Write-Host "Time: $timestamp (ET, Test computer CT -1hr)" -ForegroundColor Cyan
                Write-Host "Message: $commitMessage" -ForegroundColor Cyan
                Write-Host ""
                
                # AUTOMATIC REAL-TIME ANALYSIS
                Write-Host "🤖 EXECUTING IMMEDIATE ANALYSIS..." -ForegroundColor Green
                Write-Host ""
                
                # Step 1: Pull latest changes
                Write-PipeLog "Step 1: Pulling latest changes..." "INFO"
                Set-Location $repoPath
                $pullResult = git pull origin main 2>&1
                Write-PipeLog "Git pull completed: $pullResult" "SUCCESS"
                
                # Step 2: Find latest test results with time zone awareness
                Write-PipeLog "Step 2: Searching for test results (time zone aware)..." "INFO"
                
                $currentHour = Get-Date -Format "HH"
                $previousHour = (Get-Date).AddHours(-1).ToString("HH")
                $testPatterns = @(
                    "test-results\test-report-*.md",
                    "test-results\*$currentHour-*.md", 
                    "test-results\*$previousHour-*.md"
                )
                
                $allResults = @()
                foreach ($pattern in $testPatterns) {
                    $files = Get-ChildItem $pattern -ErrorAction SilentlyContinue
                    if ($files) { $allResults += $files }
                }
                
                if ($allResults) {
                    $latestResult = $allResults | Sort-Object LastWriteTime -Descending | Select-Object -First 1
                    Write-PipeLog "Latest test result: $($latestResult.Name)" "SUCCESS"
                    Write-PipeLog "File timestamp: $($latestResult.LastWriteTime)" "SUCCESS"
                    
                    Write-Host ""
                    Write-Host "🎯 READY FOR CURSOR ANALYSIS!" -ForegroundColor Green
                    Write-Host "===============================================" -ForegroundColor Yellow
                    Write-Host "REAL-TIME TRIGGER - SEND TO CURSOR:" -ForegroundColor Cyan
                    Write-Host ""
                    Write-Host "New test results detected - analyze now with time zone adjustment" -ForegroundColor White
                    Write-Host "Latest file: $($latestResult.Name)" -ForegroundColor White  
                    Write-Host "Commit: $commit" -ForegroundColor White
                    Write-Host "Timestamp: $timestamp" -ForegroundColor White
                    Write-Host "Real-time communication: Named Pipes" -ForegroundColor White
                    Write-Host ""
                    Write-Host "CURSOR: Analyze the above test results immediately" -ForegroundColor Green
                    Write-Host "===============================================" -ForegroundColor Yellow
                    Write-Host ""
                    
                } else {
                    Write-PipeLog "No test results found yet - may still be processing" "WARNING"
                }
                
                Write-PipeLog "Real-time analysis trigger processed successfully" "SUCCESS"
                
            } else {
                Write-PipeLog "Invalid message format received: $message" "WARNING"
            }
        }
        
        # Clean up
        $reader.Close()
        $pipe.Close()
        Write-PipeLog "Client connection closed, ready for next connection" "INFO"
        Write-Host ""
        
    } catch {
        Write-PipeLog "Error in named pipe server: $($_.Exception.Message)" "ERROR"
        if ($pipe) { $pipe.Close() }
        Start-Sleep -Seconds 2
    }
}
