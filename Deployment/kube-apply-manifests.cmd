@echo off
setlocal

echo ===== Aplicando infraestrutura base do commerceflow =====

echo 1. Aplicando namespace e secrets...
kubectl apply -f "namespace.yaml"
kubectl apply -f "commerceflow-secrets.yaml"

echo.
call :apply_app "postgres"

echo.
call :apply_app "kafka"

echo.
echo ===== Aguardando infraestrutura ficar pronta =====
timeout /t 10 /nobreak >nul

echo.

call :apply_app "webapi"
call :apply_app "eventworkers"

echo.
echo ===== Services =====
kubectl get svc -A

echo.
echo ===== Nodes =====
kubectl get nodes -o wide

echo.
pause
exit /b

:apply_app

echo Aplicando servico: %~1

if exist "%~1\pvc.yaml" (
    echo   - Aplicando pvc.yaml
    kubectl apply -f "%~1\pvc.yaml"
) else (
    echo   - pvc.yaml NAO encontrado
)

if exist "%~1\deployment.yaml" (
    echo   - Aplicando deployment.yaml
    kubectl apply -f "%~1\deployment.yaml"
) else (
    echo   - deployment.yaml NAO encontrado
)

if exist "%~1\service.yaml" (
    echo   - Aplicando service.yaml
    kubectl apply -f "%~1\service.yaml"
) else (
    echo   - service.yaml NAO encontrado
)

if exist "%~1\hpa.yaml" (
    echo   - Aplicando hpa.yaml
    kubectl apply -f "%~1\hpa.yaml"
) else (
    echo   - hpa.yaml NAO encontrado
)
echo ----------
exit /b
