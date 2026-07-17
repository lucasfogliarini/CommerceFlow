using Aspire.C4;

public class ShipmentsWebApp : Service
{
    public override string Name => nameof(ShipmentsWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        #pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("ShipmentsWebApp", "../CommerceFlow.Shipments/WebApp")
                                           .WithEnvironment("COMMERCEFLOW_API_URL", "http://localhost:2011")
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}