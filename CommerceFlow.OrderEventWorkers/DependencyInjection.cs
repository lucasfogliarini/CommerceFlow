using CommerceFlow.Application;
using CommerceFlow.Infrastructure;
using CommerceFlow.Orders;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.AddInfrastructure();
        builder.ConfigureMessageBus(opts =>
        {
            opts.Subscribe<OrderCreated>();
            opts.Subscribe<OrderInventoryReserved>();
            opts.ConfigurePublisher<OrderWaitingForPayment>();
            opts.Subscribe<ApprovePayment>();
            opts.Subscribe<PaymentApproved>();
            opts.ConfigurePublisher<OrderReadyForShipment>();

            opts.Subscribe<OrderCancelled>();

            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
        });
    }
}