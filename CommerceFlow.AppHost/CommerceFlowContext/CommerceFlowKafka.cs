using Aspire.C4;

public class CommerceFlowKafkaServer(IDistributedApplicationBuilder builder) : CommerceFlowSystem(builder)
{
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        AddKafkaServer();
        return system
                .WithChildRelationship(KafkaServer.Resource)
                .WithChildRelationship(KafkaUI?.Resource);
    }  
    public Service<KafkaServerResource>? KafkaServer { get { return GetService<Service<KafkaServerResource>>(); } }
    public Service<KafkaUIContainerResource>? KafkaUI { get { return GetService<Service<KafkaUIContainerResource>>(); } }
    private void AddKafkaServer()
    {
        var resource = builder.AddKafka(nameof(KafkaServer))
                               .WithKafkaUI((r) =>
                                {
                                    AddService(nameof(KafkaUI), r);
                                });
        AddService(nameof(KafkaServer), resource);
    }
}