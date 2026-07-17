using Aspire.C4;

[WithReference<PostgresDatabaseResource>]
[WithReference<RabbitMQServerResource>]
public class ShipmentsWebApi : Service
{
    public override string Name => nameof(ShipmentsWebApi);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_Shipments_WebApi>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}