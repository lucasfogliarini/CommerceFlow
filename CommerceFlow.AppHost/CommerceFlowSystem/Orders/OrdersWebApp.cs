using Aspire.C4;

public class OrdersWebApp : Service
{
    public override string Name => nameof(OrdersWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        #pragma warning disable ASPIREJAVASCRIPT001
        var webApiResourceBuilder = system.Builder
                                           .AddNextJsApp("OrdersWebApp", "../CommerceFlow.Orders/WebApp")
                                           .WithEnvironment("COMMERCEFLOW_API_URL", "http://localhost:2008")
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}