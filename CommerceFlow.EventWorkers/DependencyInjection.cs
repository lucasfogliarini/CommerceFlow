using CommerceFlow;
using CommerceFlow.Application;
using CommerceFlow.Application.Shipments;
using CommerceFlow.Infrastructure;
using CommerceFlow.Shipments;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this HostApplicationBuilder builder)
    {
        builder.AddInfrastructure();
        builder.Services.AddTransient<ICarrierSelector, FakeCarrierSelector>();
        builder.ConfigureMessageBus(opts =>
        {
            // Orders
            opts.Subscribe<CreateOrder>();
            opts.Subscribe<OrderCreated>();
            opts.Subscribe<OrderInventoryReserved>();
            opts.ConfigurePublisher<OrderWaitingForPayment>();
            opts.Subscribe<ApprovePayment>();
            opts.Subscribe<PaymentApproved>();
            opts.ConfigurePublisher<CommerceFlow.Orders.ShipmentRequested>();

            // Shipments
            opts.Subscribe<ShipmentRequested>();
            opts.Subscribe<ShipmentCreated>();
            opts.Subscribe<CarrierAssigned>();
            opts.Subscribe<CompletePacking>();
            opts.Subscribe<PackingCompleted>();
            opts.ConfigurePublisher<ShipmentDispatched>();
            opts.Subscribe<RegisterTrackingEvent>();
            opts.Subscribe<DeliverShipment>();
            opts.ConfigurePublisher<ShipmentDelivered>();

            // Orders - continued
            opts.Subscribe<CommerceFlow.Orders.ShipmentDispatched>();            
            opts.Subscribe<CommerceFlow.Orders.ShipmentDelivered>();            

            opts.Subscribe<OrderCancelled>();

            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
        });
    }
}