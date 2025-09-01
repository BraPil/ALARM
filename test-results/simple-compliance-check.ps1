# Simple Deployment Compliance Check
$deploymentPath = "C:\Users\kidsg\Downloads\ALARM\tests\ADDS25\v0.1"

Write-Host "Deployment Compliance Check" -ForegroundColor Cyan
Write-Host "Checking: $deploymentPath"

$violations = 0
$files = Get-ChildItem $deploymentPath -Recurse -File -Include "*.ps1","*.bat","*.csproj" | Where-Object { $_.Directory.Name -notin @("bin", "obj") }
$configFiles = Get-ChildItem "$deploymentPath\Config" -File -Include "*.json" -ErrorAction SilentlyContinue
$files = @($files) + @($configFiles)

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
    if ($content -match "\\kidsg\\") {
        Write-Host "VIOLATION: $($file.Name) contains kidsg reference" -ForegroundColor Red
        $violations++
    } else {
        Write-Host "OK: $($file.Name)" -ForegroundColor Green
    }
}

if ($violations -eq 0) {
    Write-Host "COMPLIANCE: PASSED" -ForegroundColor Green
} else {
    Write-Host "COMPLIANCE: FAILED - $violations violations" -ForegroundColor Red
}
