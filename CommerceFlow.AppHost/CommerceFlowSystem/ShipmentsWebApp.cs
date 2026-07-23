using Aspire.C4;

[WaitFor(nameof(WebApi), nameof(KeycloakServer))]
public class ShipmentsWebApp : Service
{
    public override string Name => nameof(ShipmentsWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var apiUrl = system.GetEndpoint<WebApi>();
        var keycloakUrl = system.GetEndpoint<KeycloakServer>();

#pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("ShipmentsWebApp", "../CommerceFlow.Shipments.WebApp")
                                           .WithEnvironment("SHIPMENTS_API_URL", apiUrl)
                                           .WithEnvironment("KEYCLOAK_URL", keycloakUrl)
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}