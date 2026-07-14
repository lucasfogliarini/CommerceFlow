using Aspire.C4;

var systemBuilder = SoftwareSystemContextBuilder.CreateBuilder<CommerceFlowSystemBuilder>();

//systemBuilder.AddService<CommerceFlowKafkaServer>();
systemBuilder.AddService<CommerceFlowRabbitMQServer>();
//systemBuilder.AddService<CommerceFlowServiceBusServer>();
systemBuilder.AddService<CommerceFlowPostgresServer>();
systemBuilder.AddService<CommerceFlowKeycloakServer>();
systemBuilder.AddService<CommerceFlowOrderEventWorkers>();
systemBuilder.AddService<CommerceFlowShipmentEventWorkers>();
systemBuilder.AddService<CommerceFlowWebApi>();
systemBuilder.AddService<CommerceFlowWebApp>();
systemBuilder.AddService<CommerceFlowShipmentsWebApp>();

var app = systemBuilder.Build();

await app.RunAsync();