using Aspire.C4;

[WaitFor(nameof(ShipmentsWebApi), nameof(KeycloakServer))]
public class ShipmentsWebApp : Service
{
    public override string Name => nameof(ShipmentsWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var shipmentsApiUrl = "http://localhost:2011";
        var keycloakUrl = "http://localhost:2006";

#pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("ShipmentsWebApp", "../CommerceFlow.Shipments/WebApp")
                                           .WithEnvironment("SHIPMENTS_API_URL", shipmentsApiUrl)
                                           .WithEnvironment("NEXT_PUBLIC_KEYCLOAK_URL", keycloakUrl)
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}