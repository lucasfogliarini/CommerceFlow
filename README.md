# CommerceFlow

CommerceFlow is an event-driven commerce platform for browsing products, managing customer orders, and coordinating shipment fulfillment. The solution is orchestrated locally with .NET Aspire, which starts the APIs, workers, web applications, PostgreSQL, RabbitMQ, and Keycloak dependencies together.

## Running with Aspire

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) running
- Node.js and npm (used by the Next.js web applications)

From the repository root, restore dependencies and run the AppHost:

```powershell
dotnet restore CommerceFlow.slnx
dotnet run --project CommerceFlow.AppHost
```

Alternatively, open `CommerceFlow.slnx` in Visual Studio, set **CommerceFlow.AppHost** as the startup project, and press **F5**.

Wait until the Aspire resources are running, then open the applications:

| Application | URL | Description |
| --- | --- | --- |
| CommerceFlow | [http://localhost:2009/](http://localhost:2009/) | Customer-facing commerce application for browsing products, managing the cart, placing orders, and reviewing account orders. |
| RoutePulse | [http://localhost:2012/](http://localhost:2012/) | Shipment operations workspace for managing and monitoring shipment fulfillment. |

## Development login

Both applications use Keycloak for authentication. Use the development account below:

- **Username:** `admin`
- **Password:** `admin`

These credentials are intended only for local development.

## Configure Google sign-in

Google can be configured as an identity provider in the local Keycloak realm.

1. Open the [Google identity provider settings](http://localhost:2006/admin/commerceflow/console/#/commerceflow/identity-providers/google/google/settings) after Keycloak has started.
2. Enable the provider.
3. In [Google Cloud Console](https://console.cloud.google.com/), create an OAuth 2.0 client credential for a web application.
4. Add this authorized redirect URI to the Google credential:

   ```text
   http://localhost:2006/realms/commerceflow/broker/google/endpoint
   ```

5. Copy the Google OAuth **Client ID** and **Client Secret** into the corresponding Keycloak Google provider fields, then save the configuration.

Do not commit OAuth credentials or use the local callback URL in production. Configure the production Keycloak hostname and redirect URI in the Google credential instead.

## Running with Kubernetes (kind)

Use the steps below to run a local Kubernetes cluster with kind.

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) running

### 1. Install kind

```powershell
winget install --id Kubernetes.kind --exact
```

### 2. Create the cluster

```powershell
kind create cluster
```

### 3. Install cloud-provider-kind

1. Download the binary from [cloud-provider-kind releases](https://github.com/kubernetes-sigs/cloud-provider-kind/releases).
2. Extract `cloud-provider-kind.exe`.
3. Add the executable folder to your `PATH` (for example, `C:\Tools`).

### 4. Start cloud-provider-kind

```powershell
cloud-provider-kind
```

Keep this process running while you are using the cluster.

### 5. Install the NGINX Ingress Controller

```powershell
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml
```

### 6. Validate

```powershell
kubectl get nodes
kubectl get ingressclass
kubectl get svc -n ingress-nginx
```
