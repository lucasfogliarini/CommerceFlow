@echo off
setlocal

echo ===== Aplicando infraestrutura do commerceflow =====

echo 1. Aplicando namespace e secrets...
kubectl apply -f "namespace.yaml"
kubectl apply -f "commerceflow-secrets.yaml"
kubectl create configmap keycloak-realm --namespace commerceflow --from-file=commerceflow-realm.json="..\commerce-flow-realm-export.json" --dry-run=client -o yaml | kubectl apply -f -

echo.
call :apply_app "postgres"

echo.
call :apply_app "keycloak"

echo.
REM call :apply_app "kafka"

echo.
call :apply_app "rabbitmq"

echo.
call :apply_app "orders/eventworkers"
call :apply_app "orders/webapi"
call :apply_app "orders/webapp"
call :apply_app "shipments/eventworkers"
call :apply_app "shipments/webapi"
REM call :apply_app "shipments/webapp"

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
    kubectl apply -f "%~1\pvc.yaml"
) else (
    echo   - pvc.yaml NAO encontrado
)

if exist "%~1\deployment.yaml" (
    kubectl apply -f "%~1\deployment.yaml"
) else (
    echo   - deployment.yaml NAO encontrado
)

if exist "%~1\service.yaml" (
    kubectl apply -f "%~1\service.yaml"
) else (
    echo   - service.yaml NAO encontrado
)

if exist "%~1\ingress.yaml" (
    kubectl apply -f "%~1\ingress.yaml"
) else (
    echo   - ingress.yaml NAO encontrado
)

if exist "%~1\hpa.yaml" (
    kubectl apply -f "%~1\hpa.yaml"
) else (
    echo   - hpa.yaml NAO encontrado
)
echo ----------
exit /b

