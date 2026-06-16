using Aspire.C4;

public class CommerceFlowApplicationRunner(IDistributedApplicationBuilder builder) : CommerceFlowKafkaServer(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddApplicationRunner();
        return system
                .WithChildRelationship(ApplicationRunner.Resource);
    }  
    public Service<ProjectResource>? ApplicationRunner { get { return GetService<Service<ProjectResource>>(nameof(CommerceFlowApplicationRunner)); } }
    private void AddApplicationRunner()
    {
        var resource = Builder.AddProject<Projects.CommerceFlow_ApplicationRunner>(nameof(CommerceFlowApplicationRunner))
                .WithReference(KafkaServer.Resource)
                .WaitFor(KafkaServer.Resource);

        AddService(nameof(CommerceFlowApplicationRunner), resource);
    }
}