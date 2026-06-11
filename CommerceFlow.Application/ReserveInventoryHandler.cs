namespace CommerceFlow.Application;

public class ReserveInventoryHandler(IInventoryRepository inventories, IOrderRepository orders) : IDomainEventHandler<OrderCreated>
{
    public async Task HandleAsync(OrderCreated orderCreated, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderCreated);

        var order = await orders.GetByIdAsync(orderCreated.OrderId, cancellationToken);
        if (order is null) return;

        foreach (var item in order.Items)
        {
            var inventory = await inventories.GetByProductIdAsync(item.Product.Id, cancellationToken);
            if (inventory is null)
            {
                inventory = Inventory.Create(item.Product.Id, 0);
                inventory.Reserve(order.Id, item.Quantity);
                await inventories.AddAsync(inventory, cancellationToken);
            }
            else
            {
                inventory.Reserve(order.Id, item.Quantity);
                await inventories.UpdateAsync(inventory, cancellationToken);
            }
        }

        await inventories.CommitScope.CommitAsync(cancellationToken);
    }
}
