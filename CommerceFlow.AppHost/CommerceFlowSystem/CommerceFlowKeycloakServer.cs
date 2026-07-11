using Aspire.C4;

[DependsOn<PostgresServerResource>]
public class CommerceFlowKeycloakServer : Service
{
    public override string Name => "KeycloakServer";

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var postgresServerResource = system.GetResourceBuilder<PostgresServerResource>();
        var keycloakDatabaseResource = postgresServerResource.AddDatabase("keycloak");
        var keycloakResourceBuilder = system.Builder
                                           .AddKeycloakContainer(Name, port: system.GetNextPort())
                                           .WithLifetime(ContainerLifetime.Persistent)
                                           .WithDataVolume(DataVolumeName)
                                           .WithPostgresDatabase(keycloakDatabaseResource)
                                           .WithImport("../commerce-flow-realm-export.json");
        system.AddResourceBuilder(keycloakResourceBuilder);
    }
}