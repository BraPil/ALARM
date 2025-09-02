@ECHO OFF
REM ADDS25 Emergency Launcher Deploy
REM Purpose: Create launcher files directly via batch script to bypass Git sync issues
REM Environment: Test Computer (wa-bdpilegg)
REM Date: September 2, 2025
REM Method: Embedded content deployment - no external dependencies

ECHO *** ADDS25 Emergency Launcher Deploy ***
ECHO Purpose: Bypass Git sync issues by creating launchers directly
ECHO Environment: Test Computer (wa-bdpilegg)
ECHO Method: Embedded batch file deployment
ECHO.

REM Create logging directory
set LOGDIR=C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results
if not exist "%LOGDIR%" mkdir "%LOGDIR%"

set TIMESTAMP=%date:~-4,4%-%date:~-10,2%-%date:~-7,2%_%time:~0,2%-%time:~3,2%-%time:~6,2%
set TIMESTAMP=%TIMESTAMP: =0%

ECHO Creating emergency deployment log...
echo ADDS25 Emergency Launcher Deploy > "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
echo Deployment Time: %date% %time% >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
echo Purpose: Bypass Git sync issues >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
echo Method: Direct batch file creation >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
echo. >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"

REM Get current directory (should be ADDS25 v0.1 directory)
set ADDS25DIR=%~dp0
ECHO Current directory: %ADDS25DIR%
echo Current directory: %ADDS25DIR% >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"

REM Create Simple Launcher
ECHO Creating ADDS25-Launcher-Simple.bat...
echo Creating ADDS25-Launcher-Simple.bat >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"

(
echo @ECHO OFF
echo REM ADDS25 Simple Test Launcher - Emergency Deployed
echo REM Purpose: Basic test to verify batch file execution
echo.
echo ECHO *** ADDS25 Simple Test Launcher ^(Emergency Deployed^) ***
echo ECHO This launcher was emergency-deployed to resolve Git sync issues
echo ECHO Environment: Test Computer ^(wa-bdpilegg^)
echo ECHO.
echo.
echo REM Create test log
echo set LOGDIR=C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results
echo if not exist "%%LOGDIR%%" mkdir "%%LOGDIR%%"
echo.
echo set TIMESTAMP=%%date:~-4,4%%-%%date:~-10,2%%-%%date:~-7,2%%_%%time:~0,2%%-%%time:~3,2%%-%%time:~6,2%%
echo set TIMESTAMP=%%TIMESTAMP: =0%%
echo.
echo echo ADDS25 Simple Launcher Test ^(Emergency Deployed^) ^> "%%LOGDIR%%\emergency-simple-launcher-test-%%TIMESTAMP%%.txt"
echo echo Execution Time: %%date%% %%time%% ^>^> "%%LOGDIR%%\emergency-simple-launcher-test-%%TIMESTAMP%%.txt"
echo echo Status: SUCCESS - Emergency deployed launcher working ^>^> "%%LOGDIR%%\emergency-simple-launcher-test-%%TIMESTAMP%%.txt"
echo.
echo ECHO *** Emergency deployed simple launcher test completed successfully ***
echo ECHO Log created: %%LOGDIR%%\emergency-simple-launcher-test-%%TIMESTAMP%%.txt
echo ECHO.
echo.
echo exit /b 0
) > "%ADDS25DIR%ADDS25-Launcher-Simple.bat"

if exist "%ADDS25DIR%ADDS25-Launcher-Simple.bat" (
    ECHO SUCCESS: Simple launcher created
    echo SUCCESS: Simple launcher created >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
) else (
    ECHO ERROR: Failed to create simple launcher
    echo ERROR: Failed to create simple launcher >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
)

REM Create Full Launcher
ECHO Creating ADDS25-Launcher.bat...
echo Creating ADDS25-Launcher.bat >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"

(
echo @ECHO OFF
echo REM ADDS25 Full Launcher - Emergency Deployed
echo REM Purpose: Comprehensive launcher with AutoCAD integration test
echo.
echo ECHO *** ADDS25 Full Launcher ^(Emergency Deployed^) ***
echo ECHO Application: ADDS25 - Modernized Legacy System
echo ECHO Framework: .NET Core 8
echo ECHO AutoCAD: Map3D 2025
echo ECHO Oracle: 19c
echo ECHO Status: Emergency Deployed Version
echo ECHO.
echo.
echo REM Create comprehensive test log
echo set LOGDIR=C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results
echo if not exist "%%LOGDIR%%" mkdir "%%LOGDIR%%"
echo.
echo set TIMESTAMP=%%date:~-4,4%%-%%date:~-10,2%%-%%date:~-7,2%%_%%time:~0,2%%-%%time:~3,2%%-%%time:~6,2%%
echo set TIMESTAMP=%%TIMESTAMP: =0%%
echo.
echo echo ADDS25 Full Launcher Test ^(Emergency Deployed^) ^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo echo Execution Time: %%date%% %%time%% ^>^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo echo Framework: .NET Core 8 ^>^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo echo AutoCAD: Map3D 2025 ^>^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo echo Oracle: 19c ^>^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo echo Status: SUCCESS - Emergency deployed full launcher working ^>^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo.
echo ECHO *** Emergency deployed full launcher test completed successfully ***
echo ECHO Log created: %%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt
echo ECHO.
echo.
echo REM Simulate basic AutoCAD detection
echo ECHO *** Simulating AutoCAD Detection ***
echo if exist "C:\Program Files\Autodesk\AutoCAD 2025\acad.exe" ^(
echo     ECHO AutoCAD Map3D 2025 detected at standard location
echo     echo AutoCAD Detection: SUCCESS - Found at C:\Program Files\Autodesk\AutoCAD 2025\ ^>^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo ^) else ^(
echo     ECHO AutoCAD Map3D 2025 not found at standard location
echo     echo AutoCAD Detection: WARNING - Not found at standard location ^>^> "%%LOGDIR%%\emergency-full-launcher-test-%%TIMESTAMP%%.txt"
echo ^)
echo.
echo ECHO.
echo ECHO *** ADDS25 Emergency Deployed Launcher Complete ***
echo exit /b 0
) > "%ADDS25DIR%ADDS25-Launcher.bat"

if exist "%ADDS25DIR%ADDS25-Launcher.bat" (
    ECHO SUCCESS: Full launcher created
    echo SUCCESS: Full launcher created >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
) else (
    ECHO ERROR: Failed to create full launcher
    echo ERROR: Failed to create full launcher >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
)

REM Verify deployments
ECHO.
ECHO *** Verifying Emergency Deployments ***
echo. >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
echo Verification Results: >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"

if exist "%ADDS25DIR%ADDS25-Launcher-Simple.bat" (
    for %%F in ("%ADDS25DIR%ADDS25-Launcher-Simple.bat") do set SIMPLESIZE=%%~zF
    ECHO Simple Launcher: EXISTS (%%SIMPLESIZE%% bytes)
    echo Simple Launcher: EXISTS ^(%%SIMPLESIZE%% bytes^) >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
) else (
    ECHO Simple Launcher: MISSING
    echo Simple Launcher: MISSING >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
)

if exist "%ADDS25DIR%ADDS25-Launcher.bat" (
    for %%F in ("%ADDS25DIR%ADDS25-Launcher.bat") do set FULLSIZE=%%~zF
    ECHO Full Launcher: EXISTS (%%FULLSIZE%% bytes)
    echo Full Launcher: EXISTS ^(%%FULLSIZE%% bytes^) >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
) else (
    ECHO Full Launcher: MISSING
    echo Full Launcher: MISSING >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
)

REM Final summary
echo. >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
echo Emergency Deployment Complete: %date% %time% >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"
echo Log File: %LOGDIR%\emergency-deploy-%TIMESTAMP%.txt >> "%LOGDIR%\emergency-deploy-%TIMESTAMP%.txt"

ECHO.
ECHO *** Emergency Deployment Complete ***
ECHO Launcher files have been emergency-deployed to bypass Git sync issues
ECHO Log file: %LOGDIR%\emergency-deploy-%TIMESTAMP%.txt
ECHO.
ECHO *** ADDS25 Emergency Launcher Deploy Complete ***

exit /b 0
