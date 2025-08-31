@echo off
echo ========================================
echo ALARM High Priority Systems Build Test
echo ========================================
echo.

echo Building Protocol Engine...
dotnet build tools/protocol-engine/ProtocolEngine.csproj -c Release --verbosity minimal
if %ERRORLEVEL% neq 0 (
    echo ERROR: Protocol Engine build failed
    exit /b 1
)
echo ✅ Protocol Engine build successful

echo.
echo Building Analyzers with ML Engine...
dotnet build tools/analyzers/Analyzers.csproj -c Release --verbosity minimal
if %ERRORLEVEL% neq 0 (
    echo ERROR: Analyzers build failed
    exit /b 1
)
echo ✅ Analyzers build successful

echo.
echo Building Data Persistence Layer...
dotnet build tools/data-persistence/DataPersistence.csproj -c Release --verbosity minimal
if %ERRORLEVEL% neq 0 (
    echo ERROR: Data Persistence build failed
    exit /b 1
)
echo ✅ Data Persistence build successful

echo.
echo Building Test Suite...
dotnet build tests/system-tests/HighPrioritySystemTests.csproj -c Release --verbosity minimal
if %ERRORLEVEL% neq 0 (
    echo ERROR: Test Suite build failed
    exit /b 1
)
echo ✅ Test Suite build successful

echo.
echo ========================================
echo 🎉 ALL HIGH PRIORITY SYSTEMS BUILT SUCCESSFULLY!
echo ========================================
echo.

echo Running basic smoke test...
dotnet run --project tests/system-tests/HighPrioritySystemTests.csproj -c Release -- --test protocol-engine --output-path test-results\smoke-test

echo.
echo Test completed! Check test-results\smoke-test for detailed results.
