namespace CommerceFlow.Application.Orders;

public class CreateOrderHandler(IOrderRepository orders)
{
    public async Task HandleAsync(CreateOrder createOrder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createOrder);

        var order = Order.Create(createOrder.CustomerId, createOrder.Items);

        await orders.AddAsync(order, cancellationToken);

        await orders.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CreateOrder(Guid CustomerId, List<OrderItem> Items);
