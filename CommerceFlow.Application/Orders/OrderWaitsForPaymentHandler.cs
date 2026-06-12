namespace CommerceFlow.Application;

public class OrderWaitsForPaymentHandler(IOrderRepository orderRepository) : IDomainEventHandler<OrderInventoryReserved>
{
    public async Task HandleAsync(OrderInventoryReserved orderInventoryReserved, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderInventoryReserved);

        var order = await orderRepository.GetByIdAsync(orderInventoryReserved.OrderId, cancellationToken);
        if (order is null) return;

        order.WaitForPayment();

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
