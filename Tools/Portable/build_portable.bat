@echo off
setlocal EnableDelayedExpansion

set OUTPUT=bin\ByteSplice_Portable.bat
set BASE=%~dp0portable_base.bat
set SRC_DIR=src

echo Generating portable batch file: %OUTPUT%

REM Start with the base template
copy /Y %BASE% %OUTPUT% >nul

REM Recursively find all .cs files
for /R "%SRC_DIR%" %%F in (*.cs) do (
    set "FULL_PATH=%%F"
    
    REM Handle relative path calculation safely
    REM We need to get the path relative to the current directory
    REM This simple approach assumes run from project root
    set "REL_PATH=!FULL_PATH:%CD%\=!"
    
    echo Embedding: !REL_PATH!
    
    REM Append Start Marker
    (
        echo(
        echo === BEGIN FILE !REL_PATH! ===
    ) >> %OUTPUT%
    
    REM Append File Content
    type "%%F" >> %OUTPUT%
    
    REM Append End Marker
    (
        echo(
        echo === END FILE ===
    ) >> %OUTPUT%
)

echo.
echo Portable batch file created successfully.
echo Usage: %OUTPUT% ^<target_file^> [options]
