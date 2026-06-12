using Aspire.C4;

public class CommerceFlowWebApi(IDistributedApplicationBuilder builder) : CommerceFlowSystem(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddWebApi();
        return system
                .WithChildRelationship(WebApi.Resource);
    }  
    public Service<ProjectResource> WebApi { get { return GetService<Service<ProjectResource>>(); } }
    const string WebApiFolder = $"../CommerceFlow.WebApi";
    private void AddWebApi()
    {
        var webApiService = AddService<Service<ProjectResource>>(nameof(WebApi));
        webApiService.Resource = Builder.AddProject(WebApi.Name, WebApiFolder)
                //.WithReference(BoraDatabaseServer.Database!.Resource)
                //.WaitFor(BoraDatabaseServer.Database!.Resource)
                .WithHttpEndpoint(name: WebApi.Name, port: WebApi.Port, isProxied: false);
    }
}