using CommerceFlow.Application;
using CommerceFlow.Infrastructure.RabbitMQ;
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
            opts.Subscribe<OrderWaitingForPayment>();
            opts.Subscribe<ApprovePayment>();
            opts.Subscribe<RejectPayment>();
            opts.Subscribe<PaymentApproved>();
            opts.Subscribe<PaymentRejected>();
            opts.Subscribe<PaymentExpired>();
            opts.ConfigurePublisher<OrderReadyForShipment>();

            opts.Subscribe<OrderCancelled>();

            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
        });
    }
}