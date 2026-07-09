using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class ReleaseInventoryHandler(IOrderRepository orderRepository) : IDomainEventHandler<PaymentRejected>
{
    public async Task HandleAsync(PaymentRejected orderCreated, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderCreated);

        var order = await orderRepository.GetByIdAsync(orderCreated.OrderId, cancellationToken);
        if (order is null) return;

        order.ReleaseInventory();

        orderRepository.Update(order);

        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
