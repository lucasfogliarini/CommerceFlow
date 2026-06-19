using Aspire.C4;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<KafkaServerResource>]
public class CommerceFlowShipmentEventWorkers : Service
{
    public override string Name => "ShipmentEventWorkers";

    public override void Configure(SoftwareSystemContext system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_ShipmentEventWorkers>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddService(webApiResourceBuilder);
    }
}