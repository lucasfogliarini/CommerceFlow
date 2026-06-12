namespace CommerceFlow.Application;

public class CompleteShipmentHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(CompleteShipment completeShipment, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(completeShipment);

        var order = await orderRepository.GetByIdAsync(completeShipment.OrderId, cancellationToken);
        if (order is null) return;

        order.CompleteShipment();

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CompleteShipment(Guid OrderId);
