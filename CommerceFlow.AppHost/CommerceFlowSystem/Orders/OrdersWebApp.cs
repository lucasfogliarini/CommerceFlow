using Aspire.C4;

[WaitFor(nameof(OrdersWebApi), nameof(KeycloakServer))]
public class OrdersWebApp : Service
{
    public override string Name => nameof(OrdersWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        #pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("OrdersWebApp", "../CommerceFlow.Orders/WebApp")
                                           .WithEnvironment("COMMERCEFLOW_API_URL", "http://localhost:2008")
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}