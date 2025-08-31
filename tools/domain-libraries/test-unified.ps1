#!/usr/bin/env pwsh

# Simple test script for the Unified Domain Libraries
Write-Host "🔧 Testing Unified Domain Libraries" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "Building Unified Library..." -ForegroundColor Yellow
$buildResult = dotnet build tools/domain-libraries/Unified/ALARM.DomainLibraries.csproj --verbosity minimal
$buildExitCode = $LASTEXITCODE

if ($buildExitCode -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "Checking assembly..." -ForegroundColor Yellow
    $assemblyPath = "tools\domain-libraries\Unified\bin\Debug\net8.0\ALARM.DomainLibraries.dll"
    
    if (Test-Path $assemblyPath) {
        $assembly = [System.Reflection.Assembly]::LoadFrom((Resolve-Path $assemblyPath).Path)
        $types = $assembly.GetTypes() | Where-Object { $_.IsPublic }
        
        Write-Host "✅ Assembly loaded successfully!" -ForegroundColor Green
        Write-Host "📊 Found $($types.Count) public types:" -ForegroundColor Blue
        
        $domainLibraries = $types | Where-Object { $_.Name.EndsWith("Patterns") }
        foreach ($type in $domainLibraries) {
            Write-Host "  - $($type.Name)" -ForegroundColor White
        }
        
        Write-Host ""
        Write-Host "🎉 Unified Domain Libraries test completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "❌ Assembly not found at $assemblyPath" -ForegroundColor Red
    }
} else {
    Write-Host "❌ Build failed with exit code $buildExitCode" -ForegroundColor Red
}
