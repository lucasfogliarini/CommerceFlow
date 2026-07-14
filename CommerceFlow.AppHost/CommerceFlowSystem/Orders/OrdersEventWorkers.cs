using Aspire.C4;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<RabbitMQServerResource>]
public class OrdersEventWorkers : Service
{
    public override string Name => "OrderEventWorkers";

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_Orders_EventWorkers>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}