using Aspire.C4;

public class CommerceFlowPostgresServer(IDistributedApplicationBuilder builder) : CommerceFlowSystem(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddPostgresServer();
        return system
                .WithChildRelationship(PostgresServer.ResourceBuilder);
    }  
    public Service<PostgresServerResource>? PostgresServer { get { return GetService<Service<PostgresServerResource>>(nameof(PostgresServer)); } }
    public Service<PostgresDatabaseResource>? PostgresDatabase { get { return GetService<Service<PostgresDatabaseResource>>(nameof(PostgresDatabase)); } }
    void AddPostgresServer()
    {
        var postgresServerResourceBuilder = Builder.AddPostgres(nameof(PostgresServer), port: 5432)
                                    .WithLifetime(ContainerLifetime.Persistent)
                                    .WithPgAdmin((pgAdminResourceBuilder) =>
                                    {
                                        AddService("pgadmin", pgAdminResourceBuilder);
                                    })
                                    .WithDataVolume($"{nameof(PostgresServer)}_data");
        var postgresDatabaseResourceBuilder = postgresServerResourceBuilder.AddDatabase("CommerceFlow");
        AddService(nameof(PostgresDatabase), postgresDatabaseResourceBuilder);
        AddService(nameof(PostgresServer), postgresServerResourceBuilder);
    }
}