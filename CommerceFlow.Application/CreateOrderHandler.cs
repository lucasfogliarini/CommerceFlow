namespace CommerceFlow.Application;

public class CreateOrderHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(CreateOrder createOrder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createOrder);

        var order = Order.Create(createOrder.CustomerId, createOrder.Items);

        await orderRepository.AddAsync(order, cancellationToken);

        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CreateOrder(Guid CustomerId, List<OrderItem> Items);
