namespace CommerceFlow.Application;

public class CreateOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository)
{
    public async Task<Guid> HandleAsync(CreateOrder createOrder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createOrder);

        var products = await productRepository.GetProductsByIds(createOrder.Items.Select(i=>i.ProductId).ToArray(), cancellationToken);
        var orderItems = createOrder.Items.Select(i => new OrderItem(products.FirstOrDefault(p => p.Id == i.ProductId), i.Quantity));
        var order = Order.Create(createOrder.CustomerId, orderItems);

        await orderRepository.AddAsync(order, cancellationToken);

        await orderRepository.CommitScope.CommitAsync(cancellationToken);

        return order.Id;
    }
}

public record CreateOrder(Guid CustomerId, List<CreateOrderItem> Items);
public record CreateOrderItem(int Quantity, Guid ProductId);
