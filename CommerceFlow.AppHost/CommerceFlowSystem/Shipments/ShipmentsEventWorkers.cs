using Aspire.C4;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<RabbitMQServerResource>]
public class ShipmentsEventWorkers : Service
{
    public override string Name => "ShipmentEventWorkers";

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_Shipments_EventWorkers>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}