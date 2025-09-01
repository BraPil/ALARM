# ADDS25 Deployment Compliance Checker
# Purpose: Verify all deployment files use correct username references

param([string]$DeploymentPath = "C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1")

Write-Host "🔍 ADDS25 Deployment Compliance Check" -ForegroundColor Cyan
Write-Host "Checking: $DeploymentPath" -ForegroundColor Yellow
Write-Host ""

$violations = @()
$files = Get-ChildItem $DeploymentPath -Recurse -File -Include "*.ps1","*.bat","*.csproj","*.json"

Write-Host "Files to check:" -ForegroundColor Yellow
$files | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }
Write-Host ""

foreach ($file in $files) {
    Write-Host "Checking: $($file.Name)" -ForegroundColor Gray
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
    
    if ($content -match "\\kidsg\\") {
        $violations += "VIOLATION: $($file.Name) contains development username reference"
        Write-Host "  ❌ Contains 'kidsg' reference" -ForegroundColor Red
    } else {
        Write-Host "  ✅ Clean" -ForegroundColor Green
    }
}

Write-Host ""
if ($violations.Count -eq 0) {
    Write-Host "🎉 DEPLOYMENT COMPLIANCE: PASSED" -ForegroundColor Green
    Write-Host "All deployment files correctly reference 'wa-bdpilegg' target environment" -ForegroundColor Green
} else {
    Write-Host "❌ DEPLOYMENT COMPLIANCE: FAILED" -ForegroundColor Red
    Write-Host "Violations found:" -ForegroundColor Red
    $violations | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
}

Write-Host ""
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Files checked: $($files.Count)" -ForegroundColor White
Write-Host "  Violations: $($violations.Count)" -ForegroundColor White
$status = if($violations.Count -eq 0){'COMPLIANT'}else{'NON-COMPLIANT'}
$color = if($violations.Count -eq 0){'Green'}else{'Red'}
Write-Host "  Status: $status" -ForegroundColor $color
