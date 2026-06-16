using Aspire.C4;

public class CommerceFlowApplicationRunner(IDistributedApplicationBuilder builder) : CommerceFlowKafkaServer(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddApplicationRunner();
        return system
                .WithChildRelationship(ApplicationRunner.ResourceBuilder);
    }  
    public Service<ProjectResource>? ApplicationRunner { get { return GetService<Service<ProjectResource>>(nameof(CommerceFlowApplicationRunner)); } }
    private void AddApplicationRunner()
    {
        var resourceBuilder = Builder.AddProject<Projects.CommerceFlow_ApplicationRunner>(nameof(CommerceFlowApplicationRunner))
                .WithReference(PostgresDatabase.ResourceBuilder)
                .WaitFor(PostgresDatabase.ResourceBuilder)
                .WithReference(KafkaServer.ResourceBuilder)
                .WaitFor(KafkaServer.ResourceBuilder);

        AddService(nameof(CommerceFlowApplicationRunner), resourceBuilder);
    }
}