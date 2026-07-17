using Aspire.C4;

public class PostgresServer : Service
{
    public override string Name => nameof(PostgresServer);
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
                                    .WithDataVolume(DataVolumeName);
        var postgresDatabaseResourceBuilder = postgresServerResourceBuilder.AddDatabase("CommerceFlowDb");

        system.AddResourceBuilder(postgresServerResourceBuilder);
        system.AddResourceBuilder(postgresDatabaseResourceBuilder);
    }
}