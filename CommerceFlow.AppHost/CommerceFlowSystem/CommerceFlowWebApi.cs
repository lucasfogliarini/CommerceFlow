using Aspire.C4;

public class CommerceFlowWebApi(IDistributedApplicationBuilder builder) : CommerceFlowApplicationRunner(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddWebApi();
        return system
                .WithChildRelationship(WebApi.ResourceBuilder);
    }  
    public Service<ProjectResource>? WebApi { get { return GetService<Service<ProjectResource>>(nameof(CommerceFlowWebApi)); } }
    private void AddWebApi()
    {
        var resource = Builder.AddProject<Projects.CommerceFlow_WebApi>(nameof(CommerceFlowWebApi))
                .WithReference(PostgresDatabase.ResourceBuilder)
                .WaitFor(PostgresDatabase.ResourceBuilder)
                .WithReference(KafkaServer.ResourceBuilder)
                .WaitFor(KafkaServer.ResourceBuilder)
                .WaitFor(ApplicationRunner.ResourceBuilder);

        AddService(nameof(CommerceFlowWebApi), resource);
    }
}