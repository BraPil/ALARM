@ECHO OFF
REM ADDS25 Modernized Launcher
REM Original: ADDS19TransTest.bat
REM Migration: Updated for ADDS25 with .NET Core 8, AutoCAD Map3D 2025, Oracle 19c
REM Date: September 1, 2025

ECHO *** ADDS25 Launcher Starting ***
ECHO Application: ADDS25 - Modernized Legacy System
ECHO Framework: .NET Core 8
ECHO AutoCAD: Map3D 2025  
ECHO Oracle: 19c
ECHO.

REM ADDS25: Phase 1 - Directory Setup with Administrator Elevation
ECHO *** Phase 1: Directory Setup ***
PowerShell.exe -Command "& {Start-Process PowerShell.exe -ArgumentList '-ExecutionPolicy Bypass -File ""%~dp0ADDS25-DirSetup.ps1""' -Verb RunAs}"

REM ADDS25: Wait for directory setup completion
timeout /t 3 /nobreak >nul

REM ADDS25: Phase 2 - Application Setup and AutoCAD Launch
ECHO *** Phase 2: Application Setup ***
PowerShell.exe -Command "& '%~dp0ADDS25-AppSetup.ps1'"

ECHO *** ADDS25 Launcher Complete ***
PAUSE
