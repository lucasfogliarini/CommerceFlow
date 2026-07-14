using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class ExpirePaymentHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(ExpirePayment expirePayment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expirePayment);

        var order = await orderRepository.GetByNumberAsync(expirePayment.OrderNumber, cancellationToken);
        if (order is null || !order.CanExpirePayment()) return;

        order.ExpirePayment();

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record ExpirePayment(string OrderNumber);
