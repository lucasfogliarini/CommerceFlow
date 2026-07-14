using Aspire.C4;

[DependsOn<PostgresDatabaseResource>]
[DependsOn<RabbitMQServerResource>]
public class OrdersWebApi : Service
{
    public override string Name => nameof(OrdersWebApi);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_Orders_WebApi>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}