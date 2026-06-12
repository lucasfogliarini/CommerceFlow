namespace CommerceFlow.Application;

public class ReserveInventoryHandler(IProductRepository productRepository, IOrderRepository orderRepository) : IDomainEventHandler<OrderCreated>
{
    public async Task HandleAsync(OrderCreated orderCreated, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderCreated);

        var order = await orderRepository.GetByIdAsync(orderCreated.OrderId, cancellationToken);
        if (order is null) return;

        foreach (var item in order.Items)
        {
            var product = await productRepository.GetByIdAsync(item.Product.Id, cancellationToken);
            product.Reserve(order.Id, item.Quantity);
            productRepository.Update(product);
        }

        await productRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
