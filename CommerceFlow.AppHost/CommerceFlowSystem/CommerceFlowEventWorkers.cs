using Aspire.C4;

public class CommerceFlowEventWorkers(IDistributedApplicationBuilder builder) : CommerceFlowKafkaServer(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddApplicationRunner();
        return system
                .WithChildRelationship(ApplicationRunner.ResourceBuilder);
    }  
    public Service<ProjectResource>? ApplicationRunner { get { return GetService<Service<ProjectResource>>(nameof(CommerceFlowEventWorkers)); } }
    private void AddApplicationRunner()
    {
        var resourceBuilder = Builder.AddProject<Projects.CommerceFlow_EventWorkers>(nameof(CommerceFlowEventWorkers))
                .WithReference(PostgresDatabase.ResourceBuilder)
                .WaitFor(PostgresDatabase.ResourceBuilder)
                .WithReference(KafkaServer.ResourceBuilder)
                .WaitFor(KafkaServer.ResourceBuilder);

        AddService(nameof(CommerceFlowEventWorkers), resourceBuilder);
    }
}