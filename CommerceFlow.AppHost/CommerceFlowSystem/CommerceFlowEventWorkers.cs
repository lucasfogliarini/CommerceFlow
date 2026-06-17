using Aspire.C4;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<KafkaServerResource>]
public class CommerceFlowEventWorkers : Service
{
    public override string Name => "EventWorkers";

    public override void Configure(SoftwareSystemContext system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_EventWorkers>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddService(webApiResourceBuilder);
    }
}