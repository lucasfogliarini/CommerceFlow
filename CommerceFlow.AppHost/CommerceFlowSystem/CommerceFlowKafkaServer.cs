using Aspire.C4;

public class CommerceFlowKafkaServer : Service
{
    public override string Name => nameof(CommerceFlowKafkaServer);

    public override void Configure(SoftwareSystemContext system)
    {
        var kafkaResourceBuilder = system.Builder.AddKafka(Name, system.GetNextPort())
                               .WithKafkaUI((r) =>
                               {
                                   r.WithHostPort(system.GetNextPort());
                                   r.WithLifetime(ContainerLifetime.Persistent);
                                   system.AddService(r);
                               })
                               .WithLifetime(ContainerLifetime.Persistent)
                               .WithDataVolume("KafkaServer_data");

        system.AddService(kafkaResourceBuilder);
    }
}