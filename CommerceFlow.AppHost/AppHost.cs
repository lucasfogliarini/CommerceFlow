using Aspire.C4;

var systemBuilder = SoftwareSystemContextBuilder.CreateBuilder<CommerceFlowSystemBuilder>();

//systemBuilder.AddService<KafkaServer>();
systemBuilder.AddService<RabbitMQServer>();
//systemBuilder.AddService<ServiceBusServer>();
systemBuilder.AddService<PostgresServer>();
systemBuilder.AddService<KeycloakServer>();
systemBuilder.AddService<OrdersEventWorkers>();
systemBuilder.AddService<OrdersWebApi>();
systemBuilder.AddService<OrdersWebApp>();
systemBuilder.AddService<ShipmentsEventWorkers>();
systemBuilder.AddService<ShipmentsWebApi>();
systemBuilder.AddService<ShipmentsWebApp>();

var app = systemBuilder.Build();

await app.RunAsync();