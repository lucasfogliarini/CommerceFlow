namespace CommerceFlow.Application;

public class OrderWaitsForPaymentHandler(IOrderRepository orderRepository) : IDomainEventHandler<InventoryReserved>
{
    public async Task HandleAsync(InventoryReserved inventoryReserved, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(inventoryReserved);

        var order = await orderRepository.GetByIdAsync(inventoryReserved.OrderId, cancellationToken);
        if (order is null) return;

        order.WaitForPayment();

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
