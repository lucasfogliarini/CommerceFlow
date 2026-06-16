using Aspire.C4;

public class CommerceFlowKafkaServer(IDistributedApplicationBuilder builder) : CommerceFlowPostgresServer(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddKafkaServer();
        return system
                .WithChildRelationship(KafkaServer.ResourceBuilder)
                .WithChildRelationship(KafkaUI?.ResourceBuilder);
    }  
    public Service<KafkaServerResource>? KafkaServer { get { return GetService<Service<KafkaServerResource>>(); } }
    public Service<KafkaUIContainerResource>? KafkaUI { get { return GetService<Service<KafkaUIContainerResource>>(); } }
    private void AddKafkaServer()
    {
        var resource = builder.AddKafka(nameof(KafkaServer), 9092)
                               .WithKafkaUI((r) =>
                                {
                                    AddService(nameof(KafkaUI), r);
                                })
                               .WithLifetime(ContainerLifetime.Persistent)
                               .WithDataVolume("KafkaServer_data");
        AddService(nameof(KafkaServer), resource);
    }
}