using Aspire.C4;

public class CommerceFlowKafkaServer : Service
{
    public override string Name => "KafkaServer";

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var kafkaResourceBuilder = system.Builder.AddKafka(Name, system.GetNextPort())
                               .WithKafkaUI((r) =>
                               {
                                   r.WithHostPort(system.GetNextPort());
                                   r.WithLifetime(ContainerLifetime.Persistent);
                                   system.AddResourceBuilder(r);
                               })
                               .WithLifetime(ContainerLifetime.Persistent)
                               .WithDataVolume(DataVolumeName);

        system.AddResourceBuilder(kafkaResourceBuilder);
    }
}