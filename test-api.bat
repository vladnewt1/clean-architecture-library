@echo off
chcp 65001 >nul
echo ============================================
echo   🧪 API Testing Script
echo   Testing Library Management System
echo ============================================
echo.

timeout /t 2 /nobreak >nul

echo 📡 Testing API Endpoints...
echo.

echo ✅ ПР1-3: SOLID + DI Lifecycle
powershell -Command "try { $result = Invoke-RestMethod -Uri 'http://localhost:5082/api/lifecycle/demo' -TimeoutSec 5; Write-Host '[OK]' -ForegroundColor Green } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ✅ ПР5: Generic Repository
powershell -Command "try { $result = Invoke-RestMethod -Uri 'http://localhost:5082/api/genericrepository/info' -TimeoutSec 5; Write-Host '[OK]' -ForegroundColor Green } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ✅ ЛБ5: Unit of Work
powershell -Command "try { $result = Invoke-RestMethod -Uri 'http://localhost:5082/api/unitofwork/info' -TimeoutSec 5; Write-Host '[OK]' -ForegroundColor Green } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ✅ ПР6/ЛБ6: AutoMapper + DTO
powershell -Command "try { $result = Invoke-RestMethod -Uri 'http://localhost:5082/api/AutoMapperDemo/info' -TimeoutSec 5; Write-Host '[OK]' -ForegroundColor Green } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ✅ ПР8: IMemoryCache
powershell -Command "try { $result = Invoke-RestMethod -Uri 'http://localhost:5082/api/CachingDemo/info' -TimeoutSec 5; Write-Host '[OK]' -ForegroundColor Green; Write-Host 'Cache Performance:' -ForegroundColor Cyan; $books1 = Invoke-RestMethod -Uri 'http://localhost:5082/api/CachingDemo/books' -TimeoutSec 5; $books2 = Invoke-RestMethod -Uri 'http://localhost:5082/api/CachingDemo/books/no-cache' -TimeoutSec 5; $speedup = [math]::Round($books2.loadTimeMs / $books1.loadTimeMs, 2); Write-Host \"Speedup: ${speedup}x\" -ForegroundColor Yellow } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ✅ CRUD Operations: Books
powershell -Command "try { $books = Invoke-RestMethod -Uri 'http://localhost:5082/api/Books' -TimeoutSec 5; Write-Host \"[OK] Found $($books.Count) books\" -ForegroundColor Green } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ✅ CRUD Operations: Members
powershell -Command "try { $members = Invoke-RestMethod -Uri 'http://localhost:5082/api/Members' -TimeoutSec 5; Write-Host \"[OK] Found $($members.Count) members\" -ForegroundColor Green } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ✅ CRUD Operations: Loans
powershell -Command "try { $loans = Invoke-RestMethod -Uri 'http://localhost:5082/api/Loans' -TimeoutSec 5; Write-Host \"[OK] Found $($loans.Count) loans\" -ForegroundColor Green } catch { Write-Host '[FAILED]' -ForegroundColor Red }"
echo.

echo ============================================
echo   ✅ All Tests Completed!
echo ============================================
echo.

pause
