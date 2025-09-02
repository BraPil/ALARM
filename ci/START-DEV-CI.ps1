# ADDS25 Dev Computer CI Startup Script
Write-Host "Starting ADDS25 Dev Computer CI System..." -ForegroundColor Cyan

$repoPath = "C:\Users\kidsg\Downloads\ALARM"
Set-Location $repoPath

git pull origin main

$ciPath = "$repoPath\ci"
$logsPath = "$ciPath\logs"

if (!(Test-Path $logsPath)) {
    New-Item $logsPath -Type Directory -Force | Out-Null
}

Write-Host "Launching automated CI system..." -ForegroundColor Green

$mainScript = "$ciPath\DEV-COMPUTER-AUTOMATED-CI.ps1"
& $mainScript
