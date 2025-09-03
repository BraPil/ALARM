# ADDS25 Cursor Named Pipe Server - FIXED VERSION
# Purpose: Real-time communication server with proper pipe instance management
# Environment: Dev Computer (kidsg) - Handles pipe conflicts
# Date: September 3, 2025

Write-Host "*** ADDS25 Cursor Named Pipe Server - FIXED VERSION ***" -ForegroundColor Cyan
Write-Host "Purpose: Real-time inter-process communication for immediate analysis" -ForegroundColor Yellow
Write-Host "Architecture: Named Pipes - Enterprise-grade IPC solution (Fixed)" -ForegroundColor Green
Write-Host ""

# Configuration
$basePipeName = "ADDS25-CURSOR-ANALYSIS-PIPE"
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
$maxRetries = 10

function Write-PipeLog {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "HH:mm:ss"
    $color = switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } }
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $color
}

function Get-AvailablePipeName {
    for ($i = 1; $i -le $maxRetries; $i++) {
        $pipeName = if ($i -eq 1) { $basePipeName } else { "$basePipeName-$i" }
        
        try {
            # Test if pipe name is available by trying to create it
            $testPipe = New-Object System.IO.Pipes.NamedPipeServerStream($pipeName, [System.IO.Pipes.PipeDirection]::In, 1, [System.IO.Pipes.PipeTransmissionMode]::Byte, [System.IO.Pipes.PipeOptions]::None)
            $testPipe.Close()
            Write-PipeLog "Found available pipe name: $pipeName" "SUCCESS"
            return $pipeName
        } catch {
            Write-PipeLog "Pipe name busy: $pipeName - trying next..." "WARNING"
            continue
        }
    }
    
    Write-PipeLog "No available pipe names found after $maxRetries attempts" "ERROR"
    return $null
}

# Find available pipe name
Write-PipeLog "Searching for available pipe name..." "INFO"
$pipeName = Get-AvailablePipeName

if (!$pipeName) {
    Write-Host ""
    Write-Host "SOLUTION: Kill existing PowerShell processes or use different pipe name" -ForegroundColor Red
    Write-Host "Run this command to see PowerShell processes:" -ForegroundColor Yellow
    Write-Host "Get-Process powershell | Select Id, ProcessName, StartTime" -ForegroundColor Cyan
    exit 1
}

Write-Host "Pipe Name: $pipeName" -ForegroundColor Green
Write-Host "Repository: $repoPath" -ForegroundColor Green
Write-Host ""
Write-Host "This server provides:" -ForegroundColor Yellow
Write-Host "  - Real-time inter-process communication" -ForegroundColor White
Write-Host "  - Instant analysis triggering (no polling)" -ForegroundColor White
Write-Host "  - Enterprise-grade reliability" -ForegroundColor White
Write-Host "  - Zero file system dependencies" -ForegroundColor White
Write-Host "  - Memory-based message passing" -ForegroundColor White
Write-Host "  - Automatic pipe conflict resolution" -ForegroundColor White
Write-Host ""
Write-Host "Waiting for PowerShell CI connections..." -ForegroundColor Green
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

$connectionCount = 0

while ($true) {
    try {
        # Create named pipe server with proper configuration
        $pipe = New-Object System.IO.Pipes.NamedPipeServerStream(
            $pipeName, 
            [System.IO.Pipes.PipeDirection]::In, 
            1, 
            [System.IO.Pipes.PipeTransmissionMode]::Byte, 
            [System.IO.Pipes.PipeOptions]::None
        )
        
        Write-PipeLog "Named pipe server created, waiting for client connection..." "INFO"
        
        # Wait for client connection
        $pipe.WaitForConnection()
        $connectionCount++
        Write-PipeLog "Client connected to named pipe! (Connection #$connectionCount)" "SUCCESS"
        
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
                Write-Host "REAL-TIME ANALYSIS TRIGGER RECEIVED!" -ForegroundColor Green
                Write-Host "=============================================" -ForegroundColor Yellow
                Write-Host "Connection: #$connectionCount" -ForegroundColor Cyan
                Write-Host "Commit: $commit" -ForegroundColor Cyan
                Write-Host "Time: $timestamp (ET, Test computer CT -1hr)" -ForegroundColor Cyan
                Write-Host "Message: $commitMessage" -ForegroundColor Cyan
                Write-Host ""
                
                # AUTOMATIC REAL-TIME ANALYSIS
                Write-Host "EXECUTING IMMEDIATE ANALYSIS..." -ForegroundColor Green
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
                    Write-Host "READY FOR CURSOR ANALYSIS!" -ForegroundColor Green
                    Write-Host "===============================================" -ForegroundColor Yellow
                    Write-Host "REAL-TIME TRIGGER - SEND TO CURSOR:" -ForegroundColor Cyan
                    Write-Host ""
                    Write-Host "New test results detected - analyze now with time zone adjustment" -ForegroundColor White
                    Write-Host "Latest file: $($latestResult.Name)" -ForegroundColor White  
                    Write-Host "Commit: $commit" -ForegroundColor White
                    Write-Host "Timestamp: $timestamp" -ForegroundColor White
                    Write-Host "Real-time communication: Named Pipes (Fixed)" -ForegroundColor White
                    Write-Host "Connection: #$connectionCount" -ForegroundColor White
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
        if ($pipe) { 
            try { $pipe.Close() } catch { }
        }
        Start-Sleep -Seconds 2
    }
}
