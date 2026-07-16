@echo off
setlocal

:menu
cls
echo ==========================================
echo CommerceFlow Docker Build
echo ==========================================
echo.
echo 1. Build EventWorkers
echo 2. Build WebApis
echo 3. Build WebApps
echo 4. Build All    
echo 5. Sair
echo.

set /p option=Escolha uma opcao:

if "%option%"=="1" call :BuildEventWorkers
if "%option%"=="2" call :BuildWebApis
if "%option%"=="3" call :BuildWebApps
if "%option%"=="4" call :BuildAll
if "%option%"=="5" goto end

pause
goto menu

:BuildEventWorkers
echo.
echo Building Order EventWorkers ...
docker build -f CommerceFlow.Orders/EventWorkers/Dockerfile -t lucasfogliarini/orders-eventworkers:latest .

echo Building Shipments EventWorkers ...
docker build -f CommerceFlow.Shipments/EventWorkers/Dockerfile -t lucasfogliarini/shipments-eventworkers:latest .

if errorlevel 1 (
    echo Falha ao buildar EventWorkers.
    exit /b 1
)

echo EventWorkers buildado com sucesso.
exit /b 0

:BuildWebApis
echo.
echo Building Order API...
docker build -f CommerceFlow.Orders/WebApi/Dockerfile -t lucasfogliarini/orders-webapi:latest .

echo Building Shipments API...
docker build -f CommerceFlow.Shipments/WebApi/Dockerfile -t lucasfogliarini/shipments-webapi:latest .

if errorlevel 1 (
    echo Falha ao buildar WebApis.
    exit /b 1
)

echo WebApis buildadas com sucesso.
exit /b 0

:BuildWebApps
echo.
echo Building Order App...
docker build -f CommerceFlow.Orders/WebApp/Dockerfile -t lucasfogliarini/orders-webapp:latest CommerceFlow.Orders/WebApp/

echo Building Shipments App...
docker build -f CommerceFlow.Shipments/WebApp/Dockerfile -t lucasfogliarini/shipments-webapp:latest CommerceFlow.Shipments/WebApp/

if errorlevel 1 (
    echo Falha ao buildar WebApps.
    exit /b 1
)

echo WebApps buildadas com sucesso.
exit /b 0

:BuildAll
call :BuildEventWorkers
if errorlevel 1 exit /b 1

call :BuildWebApis
if errorlevel 1 exit /b 1

call :BuildWebApps
if errorlevel 1 exit /b 1

echo.
echo Todos os builds foram concluídos com sucesso.
exit /b 0

:end
endlocal