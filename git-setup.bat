@echo off
REM ============================================
REM Git Setup Script for FallingSand Unity v1.0
REM ============================================

echo.
echo ========================================
echo  FallingSand Unity - Git Setup v1.0
echo ========================================
echo.

REM Clean up legacy Java/Gradle folders
echo [1/5] Cleaning up legacy folders...
if exist "core" (
    rmdir /s /q "core"
    echo    - Deleted: core/
)
if exist ".gradle" (
    rmdir /s /q ".gradle"
    echo    - Deleted: .gradle/
)
echo    ✓ Cleanup complete
echo.

REM Initialize Git repository
echo [2/5] Initializing Git repository...
if exist ".git" (
    echo    - Git already initialized
) else (
    git init
    echo    ✓ Git initialized
)
echo.

REM Configure Git (optional - adjust these)
echo [3/5] Configuring Git...
git config user.name "Johnny-Z13"
git config user.email "your-email@example.com"
echo    ✓ Git configured (update email if needed)
echo.

REM Stage all files
echo [4/5] Staging files...
git add .
echo    ✓ Files staged
echo.

REM Show status
echo [5/5] Repository status:
echo.
git status -sb
echo.

echo ========================================
echo  Ready to commit!
echo ========================================
echo.
echo Next steps:
echo   1. Review staged files above
echo   2. Run: git commit -m "Initial commit: Unity 6 Falling Sand v1.0"
echo   3. Add remote: git remote add origin https://github.com/Johnny-Z13/FallingSand.git
echo   4. Push: git push -u origin main
echo.
echo Press any key to exit...
pause >nul
