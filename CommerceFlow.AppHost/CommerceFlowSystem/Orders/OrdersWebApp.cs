using Aspire.C4;

[WaitFor(nameof(OrdersWebApi), nameof(KeycloakServer))]
public class OrdersWebApp : Service
{
    public override string Name => nameof(OrdersWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var ordersApiEndpoint = system.GetEndpoint<OrdersWebApi>();
        var keycloakEndpoint = system.GetEndpoint<KeycloakServer>();

#pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("OrdersWebApp", "../CommerceFlow.Orders/WebApp")
                                           .WithEnvironment("ORDERS_API_URL", ordersApiEndpoint)
                                           .WithEnvironment("KEYCLOAK_URL", keycloakEndpoint)
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}