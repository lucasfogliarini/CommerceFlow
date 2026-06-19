@echo off
setlocal

:menu
cls
echo ==========================================
echo CommerceFlow Docker Build
echo ==========================================
echo.
echo 1. Build EventWorkers
echo 2. Build WebApi
echo 3. Build Ambos
echo 4. Sair
echo.

set /p option=Escolha uma opcao:

if "%option%"=="1" call :BuildEventWorkers
if "%option%"=="2" call :BuildWebApi
if "%option%"=="3" call :BuildAll
if "%option%"=="4" goto end

pause
goto menu

:BuildEventWorkers
echo.
echo Building EventWorkers...
docker build -f CommerceFlow.OrderEventWorkers/Dockerfile -t lucasfogliarini/commerceflow-ordereventworkers:latest .
docker build -f CommerceFlow.ShipmentEventWorkers/Dockerfile -t lucasfogliarini/commerceflow-shipmenteventworkers:latest .

if errorlevel 1 (
    echo Falha ao buildar EventWorkers.
    exit /b 1
)

echo EventWorkers buildado com sucesso.
exit /b 0

:BuildWebApi
echo.
echo Building WebApi...
docker build -f CommerceFlow.WebApi/Dockerfile -t lucasfogliarini/commerceflow-webapi:latest .

if errorlevel 1 (
    echo Falha ao buildar WebApi.
    exit /b 1
)

echo WebApi buildada com sucesso.
exit /b 0

:BuildAll
call :BuildEventWorkers
if errorlevel 1 exit /b 1

call :BuildWebApi
if errorlevel 1 exit /b 1

echo.
echo Todos os builds foram concluídos com sucesso.
exit /b 0

:end
endlocal