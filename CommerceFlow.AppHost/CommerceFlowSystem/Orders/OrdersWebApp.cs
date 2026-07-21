using Aspire.C4;

[WaitFor(nameof(OrdersWebApi), nameof(KeycloakServer))]
public class OrdersWebApp : Service
{
    public override string Name => nameof(OrdersWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var ordersApiUrl = "http://localhost:2008";
        var keycloakUrl = "http://localhost:2006";

#pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("OrdersWebApp", "../CommerceFlow.Orders/WebApp")
                                           .WithEnvironment("ORDERS_API_URL", ordersApiUrl)
                                           .WithEnvironment("NEXT_PUBLIC_KEYCLOAK_URL", keycloakUrl)
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}