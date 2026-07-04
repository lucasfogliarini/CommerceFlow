using Aspire.C4;

var systemBuilder = SoftwareSystemContextBuilder.CreateBuilder<CommerceFlowSystemBuilder>();

systemBuilder.AddService<CommerceFlowKafkaServer>();
//systemBuilder.AddService<CommerceFlowServiceBusServer>();
systemBuilder.AddService<CommerceFlowPostgresServer>();
systemBuilder.AddService<CommerceFlowOrderEventWorkers>();
systemBuilder.AddService<CommerceFlowShipmentEventWorkers>();
systemBuilder.AddService<CommerceFlowWebApi>();

var app = systemBuilder.Build();

await app.RunAsync();