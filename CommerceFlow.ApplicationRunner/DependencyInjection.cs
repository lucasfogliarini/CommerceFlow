using CommerceFlow;
using CommerceFlow.Application;
using CommerceFlow.Application.Shipments;
using CommerceFlow.Infrastructure;
using CommerceFlow.Shipments;
using Wolverine;
using Wolverine.Kafka;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this HostApplicationBuilder builder)
    {
        builder.AddInfrastructure();
        builder.Services.AddTransient<ICarrierSelector, FakeCarrierSelector>();
        builder.UseWolverine(opts =>
        {
            var kafkaEndpoint = builder.Configuration.GetConnectionString("KafkaServer");
            opts.UseKafka(kafkaEndpoint);

            opts.Subscribe<CreateOrder>();
            opts.Subscribe<OrderCreated>();
            opts.ConfigurePublisher<OrderInventoryReserved>().Subscribe<OrderInventoryReserved>();
            opts.ConfigurePublisher<OrderWaitingForPayment>();
            opts.Subscribe<ApprovePayment>();
            opts.ConfigurePublisher<PaymentApproved>().Subscribe<PaymentApproved>();
            opts.ConfigurePublisher<CommerceFlow.Orders.ShipmentRequested>().Subscribe<CommerceFlow.Shipments.ShipmentRequested>();
            opts.ConfigurePublisher<ShipmentCreated>().Subscribe<CarrierAssigned>();
            opts.ConfigurePublisher<CarrierAssigned>().Subscribe<CarrierAssigned>();
            opts.Subscribe<CompletePacking>();
            opts.ConfigurePublisher<PackingCompleted>().Subscribe<PackingCompleted>();
            opts.ConfigurePublisher<CommerceFlow.Shipments.ShipmentDispatched>().Subscribe<CommerceFlow.Orders.ShipmentDispatched>();
            opts.Subscribe<RegisterTrackingEvent>();
            opts.ConfigurePublisher<CommerceFlow.Shipments.ShipmentDelivered>().Subscribe<CommerceFlow.Orders.ShipmentDelivered>();
            opts.Subscribe<DeliverShipment>();

            opts.ConfigurePublisher<OrderCancelled>();

            opts.UseRuntimeCompilation();
            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
            opts.CodeGeneration.AlwaysUseServiceLocationFor<CommerceFlowDbContext>();
        });
    }
}