using Aspire.C4;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<RabbitMQServerResource>]
public class CommerceFlowWebApi : Service
{
    public override string Name => nameof(CommerceFlowWebApi);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_WebApi>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}