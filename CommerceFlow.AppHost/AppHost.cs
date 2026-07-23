using Aspire.C4;

var systemBuilder = SoftwareSystemContextBuilder.CreateBuilder<CommerceFlowSystemBuilder>();

systemBuilder.AddService<PostgresServer>();
systemBuilder.AddService<KeycloakServer>();
systemBuilder.AddService<WebApi>();
systemBuilder.AddService<OrdersWebApp>();
systemBuilder.AddService<ShipmentsWebApp>();


var app = systemBuilder.Build();

await app.RunAsync();