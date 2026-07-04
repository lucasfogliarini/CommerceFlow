using Aspire.C4;

public class CommerceFlowPostgresServer : Service
{
    public override string Name => "PostgresServer";
    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var postgresServerResourceBuilder = system.Builder.AddPostgres(Name, port: system.GetNextPort())
                                    .WithLifetime(ContainerLifetime.Persistent)
                                    .WithPgAdmin((pgAdminResourceBuilder) =>
                                    {
                                        pgAdminResourceBuilder.WithHostPort(system.GetNextPort());
                                        pgAdminResourceBuilder.WithLifetime(ContainerLifetime.Persistent);
                                        system.AddResourceBuilder(pgAdminResourceBuilder);
                                    })
                                    .WithDataVolume($"{nameof(CommerceFlowPostgresServer)}_data");
        var postgresDatabaseResourceBuilder = postgresServerResourceBuilder.AddDatabase("CommerceFlow");

        system.AddResourceBuilder(postgresServerResourceBuilder);
        system.AddResourceBuilder(postgresDatabaseResourceBuilder);
    }
}