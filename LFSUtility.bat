@echo off
REM Get the current directory where the .bat file is located
set "CURRENT_DIR=%~dp0"
set "GIT_COMPRESSION_DIR=%CURRENT_DIR%GitCompression"

REM Check if the GitCompression folder exists
if not exist "%GIT_COMPRESSION_DIR%" (
    echo GitCompression folder not found!
    pause
    exit /b 1
)

REM Specify the full path to Python
set "PYTHON_PATH=python"

REM Action selection
echo Choose an action:
echo 0. Compress files
echo 1. Decompress files
set /p choice="Enter 0 or 1: "

REM Run the corresponding script
if "%choice%"=="0" (
    echo Running compression script...
    "%PYTHON_PATH%" "%GIT_COMPRESSION_DIR%\CompressLFS.py" "%CURRENT_DIR%\"
    if %errorlevel% equ 0 (
        echo Compression completed successfully.
    ) else (
        echo An error occurred during compression.
    )
) else if "%choice%"=="1" (
    echo Running decompression script...
    "%PYTHON_PATH%" "%GIT_COMPRESSION_DIR%\RestoreLFS.py" "%CURRENT_DIR%\"
    if %errorlevel% equ 0 (
        echo Decompression completed successfully.
    ) else (
        echo An error occurred during decompression.
    )
) else (
    echo Invalid choice. Please enter 0 or 1.
)

pause