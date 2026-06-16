using Aspire.C4;

public class CommerceFlowWebApi(IDistributedApplicationBuilder builder) : CommerceFlowApplicationRunner(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddWebApi();
        return system
                .WithChildRelationship(WebApi.Resource);
    }  
    public Service<ProjectResource>? WebApi { get { return GetService<Service<ProjectResource>>(nameof(CommerceFlowWebApi)); } }
    private void AddWebApi()
    {
        var resource = Builder.AddProject<Projects.CommerceFlow_WebApi>(nameof(CommerceFlowWebApi))
                .WithReference(KafkaServer.Resource)
                .WaitFor(KafkaServer.Resource);
                //.WithHttpEndpoint(name: WebApi.Name, port: WebApi.Port, isProxied: false);

        AddService(nameof(CommerceFlowWebApi), resource);
    }
}