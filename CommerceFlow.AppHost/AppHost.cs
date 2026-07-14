using Aspire.C4;

var systemBuilder = SoftwareSystemContextBuilder.CreateBuilder<CommerceFlowSystemBuilder>();

//systemBuilder.AddService<CommerceFlowKafkaServer>();
systemBuilder.AddService<CommerceFlowRabbitMQServer>();
//systemBuilder.AddService<CommerceFlowServiceBusServer>();
systemBuilder.AddService<CommerceFlowPostgresServer>();
systemBuilder.AddService<CommerceFlowKeycloakServer>();
systemBuilder.AddService<OrdersEventWorkers>();
systemBuilder.AddService<OrdersWebApi>();
systemBuilder.AddService<OrdersWebApp>();
systemBuilder.AddService<ShipmentsEventWorkers>();
systemBuilder.AddService<ShipmentsWebApi>();
systemBuilder.AddService<ShipmentsWebApp>();

var app = systemBuilder.Build();

await app.RunAsync();