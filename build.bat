@echo off
setlocal

set CSC=""
if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if %CSC% == "" if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe" set CSC="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"

if %CSC% == "" (
    echo Error: csc.exe not found
    exit /b 1
)

echo Using compiler: %CSC%

if not exist bin mkdir bin

%CSC% /nologo /out:bin\ByteSplice.exe /recurse:src\*.cs
if errorlevel 1 goto failed

echo Build successful!

REM Build portable version if available
if exist "Tools\Portable\build_portable.bat" (
    echo.
    echo Building portable version...
    call "Tools\Portable\build_portable.bat"
)

exit /b 0

:failed
echo Build failed!
exit /b 1
