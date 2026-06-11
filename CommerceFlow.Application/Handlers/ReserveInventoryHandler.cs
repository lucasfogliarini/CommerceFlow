namespace CommerceFlow.Application;

public class ReserveInventoryHandler(IInventoryRepository inventoryRepository, IOrderRepository orderRepository) : IDomainEventHandler<OrderCreated>
{
    public async Task HandleAsync(OrderCreated orderCreated, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderCreated);

        var order = await orderRepository.GetByIdAsync(orderCreated.OrderId, cancellationToken);
        if (order is null) return;

        foreach (var item in order.Items)
        {
            var inventory = await inventoryRepository.GetByProductIdAsync(item.Product.Id, cancellationToken);
            if (inventory is null)
            {
                inventory = Inventory.Create(item.Product.Id, 0);
                inventory.Reserve(order.Id, item.Quantity);
                await inventoryRepository.AddAsync(inventory, cancellationToken);
            }
            else
            {
                inventory.Reserve(order.Id, item.Quantity);
                await inventoryRepository.UpdateAsync(inventory, cancellationToken);
            }
        }

        await inventoryRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
