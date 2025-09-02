@ECHO OFF
REM ADDS25 Simple Test Launcher
REM Purpose: Basic test to verify batch file execution on test computer
REM Date: September 2, 2025

ECHO *** ADDS25 Simple Test Launcher ***
ECHO This is a basic test to verify the launcher can execute
ECHO Environment: Test Computer (wa-bdpilegg)
ECHO.

REM Create a simple test log
set LOGDIR=C:\Users\wa-bdpilegg\Downloads\ADDS25-Test-Results
if not exist "%LOGDIR%" mkdir "%LOGDIR%"

set TIMESTAMP=%date:~-4,4%-%date:~-10,2%-%date:~-7,2%_%time:~0,2%-%time:~3,2%-%time:~6,2%
set TIMESTAMP=%TIMESTAMP: =0%

echo ADDS25 Simple Launcher Test > "%LOGDIR%\simple-launcher-test-%TIMESTAMP%.txt"
echo Execution Time: %date% %time% >> "%LOGDIR%\simple-launcher-test-%TIMESTAMP%.txt"
echo Status: SUCCESS - Basic launcher execution working >> "%LOGDIR%\simple-launcher-test-%TIMESTAMP%.txt"

ECHO *** Simple launcher test completed successfully ***
ECHO Log created: %LOGDIR%\simple-launcher-test-%TIMESTAMP%.txt
ECHO.

REM Exit with success code
exit /b 0
