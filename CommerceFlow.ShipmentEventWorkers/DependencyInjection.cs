using CommerceFlow.Application;
using CommerceFlow.Application.Shipments;
using CommerceFlow.Infrastructure;
using CommerceFlow.Orders;
using CommerceFlow.Shipments;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.AddInfrastructure();
        builder.Services.AddTransient<ICarrierSelector, FakeCarrierSelector>();
        builder.ConfigureMessageBus(opts =>
        {
            opts.Subscribe<OrderReadyForShipment>();
            opts.Subscribe<ShipmentCreated>();
            opts.Subscribe<CarrierAssigned>();
            opts.Subscribe<CompletePacking>();
            opts.Subscribe<PackingCompleted>();
            opts.Subscribe<ShipmentDispatched>();
            opts.Subscribe<RegisterTrackingEvent>();
            opts.Subscribe<DeliverShipment>();
            opts.Subscribe<ShipmentDelivered>();

            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
        });
    }
}