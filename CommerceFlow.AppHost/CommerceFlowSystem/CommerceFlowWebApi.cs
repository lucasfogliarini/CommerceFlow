using Aspire.C4;
using Aspire.Hosting.Azure;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<KafkaServerResource>]
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