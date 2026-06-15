using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class ReserveInventoryHandler(IOrderRepository orderRepository) : IDomainEventHandler<OrderCreated>
{
    public async Task HandleAsync(OrderCreated orderCreated, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderCreated);

        var order = await orderRepository.GetByIdAsync(orderCreated.OrderId, cancellationToken);
        if (order is null) return;

        order.ReserveInventory();

        orderRepository.Update(order);

        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
