using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class ScheduleExpirePaymentHandler(IMessageDispatcher bus) : IDomainEventHandler<OrderWaitingForPayment>
{
    public async Task HandleAsync(OrderWaitingForPayment orderWaitingForPayment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(orderWaitingForPayment);

        var expirePayment = new ExpirePayment(orderWaitingForPayment.OrderId);

        await bus.ScheduleAsync(expirePayment, TimeSpan.FromMinutes(1));
    }
}
