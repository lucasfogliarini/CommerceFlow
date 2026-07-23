using Aspire.C4;

[WaitFor(nameof(ShipmentsWebApi), nameof(KeycloakServer))]
public class ShipmentsWebApp : Service
{
    public override string Name => nameof(ShipmentsWebApp);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var shipmentsApiEndpoint = system.GetEndpoint<ShipmentsWebApi>();
        var keycloakEndpoint = system.GetEndpoint<KeycloakServer>();

#pragma warning disable ASPIREJAVASCRIPT001
        var webAppResourceBuilder = system.Builder
                                           .AddNextJsApp("ShipmentsWebApp", "../CommerceFlow.Shipments/WebApp")
                                           .WithEnvironment("SHIPMENTS_API_URL", shipmentsApiEndpoint)
                                           .WithEnvironment("KEYCLOAK_URL", keycloakEndpoint)
                                           .WithHttpEndpoint(system.GetNextPort());
        #pragma warning restore ASPIREJAVASCRIPT001
        system.AddResourceBuilder(webAppResourceBuilder);
    }
}