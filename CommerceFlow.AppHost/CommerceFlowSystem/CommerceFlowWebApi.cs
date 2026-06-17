using Aspire.C4;

[DependsOn<PostgresDatabaseResource>]
public class CommerceFlowWebApi : Service
{
    public override string Name => nameof(CommerceFlowWebApi);

    public override void Configure(SoftwareSystemContext system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_WebApi>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddService(webApiResourceBuilder);
    }
}