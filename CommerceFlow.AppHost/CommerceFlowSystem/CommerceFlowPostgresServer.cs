using Aspire.C4;

public class CommerceFlowPostgresServer : Service
{
    public override string Name => nameof(CommerceFlowPostgresServer);
    public override void Configure(SoftwareSystemContext system)
    {
        var postgresServerResourceBuilder = system.Builder.AddPostgres(Name, port: system.GetNextPort())
                                    .WithLifetime(ContainerLifetime.Persistent)
                                    .WithPgAdmin((pgAdminResourceBuilder) =>
                                    {
                                        pgAdminResourceBuilder.WithHostPort(system.GetNextPort());
                                        pgAdminResourceBuilder.WithLifetime(ContainerLifetime.Persistent);
                                        system.AddService(pgAdminResourceBuilder);
                                    })
                                    .WithDataVolume($"{nameof(CommerceFlowPostgresServer)}_data");
        var postgresDatabaseResourceBuilder = postgresServerResourceBuilder.AddDatabase("CommerceFlow");

        system.AddService(postgresServerResourceBuilder);
        system.AddService(postgresDatabaseResourceBuilder);
    }
}