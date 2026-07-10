using Aspire.C4;

public class CommerceFlowWebApp : Service
{
    public override string Name => nameof(CommerceFlowWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        #pragma warning disable ASPIREJAVASCRIPT001
        var webApiResourceBuilder = system.Builder
                                           .AddNextJsApp("webapp", "../CommerceFlow.WebApp")
                                           .WithEnvironment("COMMERCEFLOW_API_URL", "http://localhost:2008")
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}