@echo off
chcp 65001 >nul
echo ============================================
echo   📚 Library Management System
echo   Starting API Server...
echo ============================================
echo.

cd /d "%~dp0src\LibraryManagement.API"

echo 🔨 Building project...
dotnet build --verbosity quiet

if %ERRORLEVEL% NEQ 0 (
    echo ❌ Build failed!
    pause
    exit /b 1
)

echo ✅ Build successful!
echo.
echo 🚀 Starting server on http://localhost:5082
echo 📖 Swagger UI: http://localhost:5082/swagger
echo.
echo Press Ctrl+C to stop the server
echo ============================================
echo.

dotnet run

pause
