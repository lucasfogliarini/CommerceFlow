using Aspire.C4;
using Aspire.Hosting.Azure;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<KafkaServerResource>]
public class CommerceFlowShipmentEventWorkers : Service
{
    public override string Name => "ShipmentEventWorkers";

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_ShipmentEventWorkers>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}