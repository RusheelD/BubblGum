@echo off
REM Build BubblGum Compiler as standalone executable

echo ============================================
echo Building BubblGum Compiler
echo ============================================

cd /d "%~dp0"

REM Clean previous builds
echo Cleaning previous builds...
dotnet clean BubblGum.sln >nul 2>&1

REM Build as standalone executable
echo Building standalone executable...
dotnet publish Main\Main.csproj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o bin\Release\

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ❌ Build failed!
    exit /b 1
)

echo.
echo ✅ Build successful!
echo.
echo Executable location: bin\Release\bbgm.exe
echo.
echo To install system-wide:
echo   1. Add bin\Release to your PATH
echo   2. Or copy bbgm.exe to a directory in your PATH
echo.
echo Try it:
echo   bin\Release\bbgm.exe --help
echo   bin\Release\bbgm.exe Tests\Lexer_Parser\HelloWorld.bbgm
echo.

pause
