# ALARM High Priority Systems Test Runner
# This script builds and runs comprehensive tests for all high-priority components

param(
    [string]$TestType = "all",
    [switch]$Verbose = $false,
    [string]$OutputPath = "test-results\$(Get-Date -Format 'yyyyMMdd-HHmm')"
)

Write-Host "🚀 ALARM High Priority Systems Test Suite" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"

try {
    # 1. Build all projects
    Write-Host "🔨 Building ALARM components..." -ForegroundColor Yellow
    
    Write-Host "  - Building Protocol Engine..." -ForegroundColor Gray
    dotnet build tools/protocol-engine/ProtocolEngine.csproj -c Release --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw "Protocol Engine build failed" }
    
    Write-Host "  - Building Analyzers and ML Engine..." -ForegroundColor Gray
    dotnet build tools/analyzers/Analyzers.csproj -c Release --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw "Analyzers build failed" }
    
    Write-Host "  - Building Data Persistence..." -ForegroundColor Gray
    dotnet build tools/data-persistence/DataPersistence.csproj -c Release --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw "Data Persistence build failed" }
    
    Write-Host "  - Building Test Suite..." -ForegroundColor Gray
    dotnet build tests/system-tests/HighPrioritySystemTests.csproj -c Release --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw "Test Suite build failed" }
    
    Write-Host "✅ All components built successfully!" -ForegroundColor Green
    Write-Host ""

    # 2. Run tests
    Write-Host "🧪 Running system tests..." -ForegroundColor Yellow
    Write-Host "  Test Type: $TestType" -ForegroundColor Gray
    Write-Host "  Output Path: $OutputPath" -ForegroundColor Gray
    Write-Host ""

    $testArgs = @(
        "run"
        "--project", "tests/system-tests/HighPrioritySystemTests.csproj"
        "-c", "Release"
        "--"
        "--test", $TestType
        "--output-path", $OutputPath
    )
    
    if ($Verbose) {
        $testArgs += "--verbose"
    }

    & dotnet @testArgs
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Some tests failed. Check the test report for details." -ForegroundColor Red
        Write-Host "   Report location: $OutputPath" -ForegroundColor Gray
    } else {
        Write-Host "🎉 All tests passed successfully!" -ForegroundColor Green
    }

    # 3. Display test results summary
    Write-Host ""
    Write-Host "📊 Test Results Summary:" -ForegroundColor Cyan
    
    $reportPath = Join-Path $OutputPath "test_report.json"
    if (Test-Path $reportPath) {
        $report = Get-Content $reportPath | ConvertFrom-Json
        Write-Host "  Total Tests: $($report.totalTests)" -ForegroundColor White
        Write-Host "  Passed: $($report.passedTests) ✅" -ForegroundColor Green
        Write-Host "  Failed: $($report.failedTests) ❌" -ForegroundColor Red
        
        $successRate = [math]::Round(($report.passedTests / $report.totalTests) * 100, 1)
        Write-Host "  Success Rate: $successRate%" -ForegroundColor White
        
        $duration = [math]::Round($report.totalDuration.TotalSeconds, 2)
        Write-Host "  Total Duration: ${duration}s" -ForegroundColor White
        
        Write-Host ""
        Write-Host "📄 Detailed reports available at:" -ForegroundColor Cyan
        Write-Host "  - JSON: $reportPath" -ForegroundColor Gray
        Write-Host "  - Markdown: $(Join-Path $OutputPath 'test_report.md')" -ForegroundColor Gray
    }

} catch {
    Write-Host "❌ Test execution failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🏁 Test execution completed!" -ForegroundColor Cyan
