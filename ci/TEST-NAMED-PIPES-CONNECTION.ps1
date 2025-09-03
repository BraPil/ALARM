# ADDS25 Named Pipes Connection Test
# Purpose: Diagnose Named Pipes connection issues
# Date: September 3, 2025

Write-Host "*** ADDS25 Named Pipes Connection Test ***" -ForegroundColor Cyan
Write-Host ""

$pipeName = "ADDS25-CURSOR-ANALYSIS-PIPE"

try {
    Write-Host "Testing connection to pipe: $pipeName" -ForegroundColor Yellow
    
    $pipe = New-Object System.IO.Pipes.NamedPipeClientStream(".", $pipeName, [System.IO.Pipes.PipeDirection]::Out)
    
    Write-Host "Attempting connection with 5 second timeout..." -ForegroundColor White
    $pipe.Connect(5000)
    
    Write-Host "✅ CONNECTION SUCCESSFUL!" -ForegroundColor Green
    Write-Host "Named Pipes server is responding correctly" -ForegroundColor White
    
    # Send a simple test message
    $writer = New-Object System.IO.StreamWriter($pipe)
    $testMessage = "TEST:connection-test:$(Get-Date -Format 'yyyy-MM-dd-HHmm'):Named Pipes Connection Test"
    $writer.Write($testMessage)
    $writer.Flush()
    
    Write-Host "Test message sent: $testMessage" -ForegroundColor Cyan
    
    $writer.Close()
    $pipe.Close()
    
    Write-Host "✅ Test completed successfully!" -ForegroundColor Green
    
} catch {
    Write-Host "❌ CONNECTION FAILED!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Possible causes:" -ForegroundColor Yellow
    Write-Host "1. Named Pipes server not running" -ForegroundColor White
    Write-Host "2. Permissions issue" -ForegroundColor White
    Write-Host "3. Server busy or not responding" -ForegroundColor White
    Write-Host "4. Pipe name mismatch" -ForegroundColor White
}

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

