using Aspire.C4;

[WaitFor(nameof(WebApi), nameof(KeycloakServer))]
public class OrdersWebApp : Service
{
    public override string Name => nameof(OrdersWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var apiUrl = system.GetEndpoint<WebApi>();
        var keycloakUrl = system.GetEndpoint<KeycloakServer>();

#pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("OrdersWebApp", "../CommerceFlow.Orders.WebApp")
                                           .WithEnvironment("ORDERS_API_URL", apiUrl)
                                           .WithEnvironment("KEYCLOAK_URL", keycloakUrl)
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}