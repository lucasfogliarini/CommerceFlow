using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class ReleaseInventoryHandler(IOrderRepository orderRepository) : IDomainEventHandler<PaymentRejected>
{
    public async Task HandleAsync(PaymentRejected paymentRejected, CancellationToken cancellationToken)
    {
        await ReleaseInventoryAsync(paymentRejected.OrderNumber, cancellationToken);
    }

    public async Task HandleAsync(PaymentExpired paymentExpired, CancellationToken cancellationToken)
    {
        await ReleaseInventoryAsync(paymentExpired.OrderNumber, cancellationToken);
    }

    public async Task ReleaseInventoryAsync(string orderNumber, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByNumberAsync(orderNumber, cancellationToken);
        if (order is null) return;

        order.ReleaseInventory();

        orderRepository.Update(order);

        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
