# ADDS25 Named Pipes Analysis Trigger
# Purpose: Manually trigger Named Pipes analysis when GitHub push is detected
# Environment: Dev Computer (kidsg) - Triggers analysis for test results
# Date: September 3, 2025

Write-Host "*** ADDS25 Named Pipes Analysis Trigger ***" -ForegroundColor Cyan
Write-Host "Purpose: Manually trigger real-time analysis for test results" -ForegroundColor Yellow
Write-Host "Architecture: Named Pipes client for analysis triggering" -ForegroundColor Green
Write-Host ""

# Configuration
$pipeName = "ADDS25-CURSOR-ANALYSIS-PIPE"
$repoPath = "C:\Users\kidsg\Downloads\ALARM"
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmm"

function Write-TriggerLog {
    param([string]$Message, [string]$Level = "INFO")
    $logTimestamp = Get-Date -Format "HH:mm:ss"
    $color = switch($Level) { "ERROR" { "Red" } "SUCCESS" { "Green" } "WARNING" { "Yellow" } default { "White" } }
    Write-Host "[$logTimestamp] [$Level] $Message" -ForegroundColor $color
}

function Send-AnalysisTrigger {
    param([string]$commitHash, [string]$message)
    
    Write-TriggerLog "Attempting to connect to Named Pipes server..." "INFO"
    
    try {
        # Try different pipe names in case server is using alternate name
        $pipeNames = @(
            "ADDS25-CURSOR-ANALYSIS-PIPE",
            "ADDS25-CURSOR-ANALYSIS-PIPE-2",
            "ADDS25-CURSOR-ANALYSIS-PIPE-3"
        )
        
        $connected = $false
        foreach ($testPipeName in $pipeNames) {
            try {
                Write-TriggerLog "Trying pipe name: $testPipeName" "INFO"
                
                $pipe = New-Object System.IO.Pipes.NamedPipeClientStream(".", $testPipeName, [System.IO.Pipes.PipeDirection]::Out)
                $pipe.Connect(2000) # 2 second timeout
                
                $writer = New-Object System.IO.StreamWriter($pipe)
                $triggerMessage = "ANALYZE:$commitHash`:$timestamp`:$message"
                $writer.Write($triggerMessage)
                $writer.Flush()
                
                $writer.Close()
                $pipe.Close()
                
                Write-TriggerLog "Successfully sent trigger to pipe: $testPipeName" "SUCCESS"
                Write-TriggerLog "Message sent: $triggerMessage" "SUCCESS"
                $connected = $true
                break
            } catch {
                Write-TriggerLog "Failed to connect to $testPipeName`: $($_.Exception.Message)" "WARNING"
                continue
            }
        }
        
        if (-not $connected) {
            Write-TriggerLog "Failed to connect to any Named Pipes server" "ERROR"
            return $false
        }
        
        return $true
    } catch {
        Write-TriggerLog "Named Pipes trigger failed: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Get latest commit information
Write-TriggerLog "Retrieving latest commit information..." "INFO"
Set-Location $repoPath

$latestCommit = git log --oneline -1
if ($latestCommit) {
    $commitParts = $latestCommit -split ' ', 2
    $commitHash = $commitParts[0]
    $commitMessage = if ($commitParts.Length -gt 1) { $commitParts[1] } else { "No message" }
    
    Write-TriggerLog "Latest commit: $commitHash - $commitMessage" "SUCCESS"
    
    # Send analysis trigger
    Write-Host ""
    Write-Host "TRIGGERING REAL-TIME ANALYSIS..." -ForegroundColor Green
    Write-Host "=================================" -ForegroundColor Yellow
    
    $result = Send-AnalysisTrigger -commitHash $commitHash -message $commitMessage
    
    if ($result) {
        Write-Host ""
        Write-Host "✅ ANALYSIS TRIGGER SENT SUCCESSFULLY!" -ForegroundColor Green
        Write-Host "Named Pipes server should now process the analysis" -ForegroundColor White
        Write-Host "Check Terminal 1 (Named Pipes Server) for response" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "❌ ANALYSIS TRIGGER FAILED!" -ForegroundColor Red
        Write-Host "Ensure Named Pipes server is running in Terminal 1" -ForegroundColor White
        Write-Host "Run: .\ci\CURSOR-NAMED-PIPE-SERVER-FIXED.ps1" -ForegroundColor Cyan
    }
} else {
    Write-TriggerLog "No commits found in repository" "ERROR"
}

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
