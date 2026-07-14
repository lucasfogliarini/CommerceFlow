using Aspire.C4;

public class CommerceFlowShipmentsWebApp : Service
{
    public override string Name => nameof(CommerceFlowShipmentsWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        #pragma warning disable ASPIREJAVASCRIPT001
        var webApiResourceBuilder = system.Builder
                                           .AddNextJsApp("CommerceFlowShipments", "../CommerceFlow.ShipmentsWebApp")
                                           .WithEnvironment("COMMERCEFLOW_API_URL", "http://localhost:2009")
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}