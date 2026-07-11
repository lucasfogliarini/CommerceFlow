using Aspire.C4;

public class CommerceFlowRabbitMQServer : Service
{
    public override string Name => "RabbitMQServer";

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var user = system.Builder.AddParameter("user", "admin");
        var password = system.Builder.AddParameter("password", "admin", secret: true);
        var serviceBusResourceBuilder = system.Builder.AddRabbitMQ(Name, user, password, port: system.GetNextPort())
                                        .WithManagementPlugin(system.GetNextPort())
                                        .WithLifetime(ContainerLifetime.Persistent)
                                        .WithDataVolume(DataVolumeName);

        system.AddResourceBuilder(serviceBusResourceBuilder);
    }
}