@echo off
setlocal EnableDelayedExpansion

REM ============================================
REM  ByteSplice - Portable Self-Extracting
REM ============================================

set TEMP_DIR=%~dp0.bytesplice_build_%RANDOM%

REM Extract embedded source files using PowerShell
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$batchPath = '%~f0';" ^
    "$tempDir = '%TEMP_DIR%';" ^
    "$lines = Get-Content $batchPath;" ^
    "$writing = $false;" ^
    "$outFile = '';" ^
    "$content = @();" ^
    "foreach ($rawLine in $lines) {" ^
    "    $line = $rawLine.Trim();" ^
    "    if ($line -match '=== BEGIN FILE (.+) ===') {" ^
    "        $outFile = Join-Path $tempDir $matches[1];" ^
    "        $parentDir = Split-Path $outFile -Parent;" ^
    "        if (-not (Test-Path $parentDir)) { New-Item -Path $parentDir -ItemType Directory -Force | Out-Null };" ^
    "        $writing = $true;" ^
    "        $content = @();" ^
    "    }" ^
    "    elseif ($line -match '=== END FILE ===') {" ^
    "        [IO.File]::WriteAllLines($outFile, $content);" ^
    "        $writing = $false;" ^
    "    }" ^
    "    elseif ($writing) {" ^
    "        $content += $line;" ^
    "    }" ^
    "}"

if errorlevel 1 (
    echo Error: Failed to extract source files
    exit /b 1
)

REM Find C# compiler
set CSC=""
if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if %CSC% == "" if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" set CSC="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"

if %CSC% == "" (
    echo Error: C# compiler not found
    rmdir /S /Q "%TEMP_DIR%" 2>nul
    exit /b 1
)

REM Compile
echo Compiling ByteSplice...
%CSC% /nologo /out:"%TEMP_DIR%\ByteSplice.exe" /recurse:"%TEMP_DIR%\*.cs" >nul 2>&1
if errorlevel 1 (
    echo Error: Compilation failed
    rmdir /S /Q "%TEMP_DIR%" 2>nul
    exit /b 1
)

REM Run the compiled executable with all arguments
"%TEMP_DIR%\ByteSplice.exe" %*
set EXIT_CODE=%ERRORLEVEL%

REM Cleanup
rmdir /S /Q "%TEMP_DIR%" 2>nul

exit /b %EXIT_CODE%

REM ============================================
REM  EMBEDDED SOURCE CODE (DO NOT MODIFY)
REM ============================================
