@echo off
setlocal enabledelayedexpansion

echo =====================================
echo ByteSplice Comprehensive Test Suite
echo =====================================
echo.

set PASSED=0
set FAILED=0
set BYTESPLICE=..\ByteSplice.exe

REM Test 1: Basic Exact Match
echo [TEST 1] Basic Exact Match...
copy /Y Suite\Fixtures\01_basic.bin Suite\Fixtures\01_basic_test.bin >nul 2>&1
%BYTESPLICE% Suite\Fixtures\01_basic_test.bin --config Suite\Configs\01_basic.txt --no-backup >nul 2>&1
if errorlevel 1 (
    echo  FAIL
    set /a FAILED+=1
) else (
    echo  PASS
    set /a PASSED+=1
)
del Suite\Fixtures\01_basic_test.bin >nul 2>&1

REM Test 2: Wildcard Matching
echo [TEST 2] Wildcard Matching...
copy /Y Suite\Fixtures\02_wildcard.bin Suite\Fixtures\02_wildcard_test.bin >nul 2>&1
%BYTESPLICE% Suite\Fixtures\02_wildcard_test.bin --config Suite\Configs\02_wildcard.txt --no-backup >nul 2>&1
if errorlevel 1 (
    echo  FAIL
    set /a FAILED+=1
) else (
    echo  PASS
    set /a PASSED+=1
)
del Suite\Fixtures\02_wildcard_test.bin >nul 2>&1

REM Test 3: Offset Patching
echo [TEST 3] Offset Patching...
copy /Y Suite\Fixtures\03_offset.bin Suite\Fixtures\03_offset_test.bin >nul 2>&1
%BYTESPLICE% Suite\Fixtures\03_offset_test.bin --config Suite\Configs\03_offset.txt --no-backup >nul 2>&1
if errorlevel 1 (
    echo  FAIL
    set /a FAILED+=1
) else (
    echo  PASS
    set /a PASSED+=1
)
del Suite\Fixtures\03_offset_test.bin >nul 2>&1

REM Test 4: Expected Matches (Fail Case)
echo [TEST 4] Expected Matches Validation (Should Abort)...
copy /Y Suite\Fixtures\04_expected.bin Suite\Fixtures\04_expected_test.bin >nul 2>&1
%BYTESPLICE% Suite\Fixtures\04_expected_test.bin --config Suite\Configs\04_expected_fail.txt --no-backup >temp_output.txt 2>&1
findstr /C:"ERROR" temp_output.txt >nul 2>&1
if errorlevel 1 (
    echo  FAIL
    set /a FAILED+=1
) else (
    echo  PASS
    set /a PASSED+=1
)
del Suite\Fixtures\04_expected_test.bin >nul 2>&1
del temp_output.txt >nul 2>&1

REM Test 5: Expected Matches (Pass Case)
echo [TEST 5] Expected Matches Validation (Should Pass)...
copy /Y Suite\Fixtures\04_expected.bin Suite\Fixtures\04_expected_test2.bin >nul 2>&1
%BYTESPLICE% Suite\Fixtures\04_expected_test2.bin --config Suite\Configs\04_expected_pass.txt --no-backup >nul 2>&1
if errorlevel 1 (
    echo  FAIL
    set /a FAILED+=1
) else (
    echo  PASS
    set /a PASSED+=1
)
del Suite\Fixtures\04_expected_test2.bin >nul 2>&1

REM Test 6: Standalone Mode
echo [TEST 6] Standalone Mode...
copy /Y Suite\Fixtures\01_basic.bin Suite\Fixtures\standalone_test.bin >nul 2>&1
%BYTESPLICE% Suite\Fixtures\standalone_test.bin --pattern "41 41 20 42 42" --replace "58 58 20 59 59" --no-backup >nul 2>&1
if errorlevel 1 (
    echo  FAIL
    set /a FAILED+=1
) else (
    echo  PASS
    set /a PASSED+=1
)
del Suite\Fixtures\standalone_test.bin >nul 2>&1

REM Test 7: Backup Collision Detection
echo [TEST 7] Backup Collision Detection...
copy /Y Suite\Fixtures\01_basic.bin Suite\Fixtures\backup_test.bin >nul 2>&1
echo DUMMY > Suite\Fixtures\backup_test.bin.bak
%BYTESPLICE% Suite\Fixtures\backup_test.bin --config Suite\Configs\01_basic.txt >temp_output.txt 2>&1
findstr /C:"Backup file already exists" temp_output.txt >nul 2>&1
if errorlevel 1 (
    echo  FAIL
    set /a FAILED+=1
) else (
    echo  PASS
    set /a PASSED+=1
)
del Suite\Fixtures\backup_test.bin >nul 2>&1
del Suite\Fixtures\backup_test.bin.bak >nul 2>&1
del temp_output.txt >nul 2>&1

REM Test 8: Dry Run Mode
echo [TEST 8] Dry Run Mode...
copy /Y Suite\Fixtures\01_basic.bin Suite\Fixtures\dryrun_test.bin >nul 2>&1
for %%A in (Suite\Fixtures\dryrun_test.bin) do set BEFORE_SIZE=%%~zA
%BYTESPLICE% Suite\Fixtures\dryrun_test.bin --config Suite\Configs\01_basic.txt --dry-run --no-backup >nul 2>&1
for %%A in (Suite\Fixtures\dryrun_test.bin) do set AFTER_SIZE=%%~zA
if "!BEFORE_SIZE!"=="!AFTER_SIZE!" (
    echo  PASS
    set /a PASSED+=1
) else (
    echo  FAIL
    set /a FAILED+=1
)
del Suite\Fixtures\dryrun_test.bin >nul 2>&1

echo.
echo =====================================
echo Test Results:
echo   Passed:  !PASSED!
echo   Failed:  !FAILED!
echo =====================================

if !FAILED! EQU 0 (
    echo All tests passed!
    exit /b 0
) else (
    echo Some tests failed!
    exit /b 1
)
