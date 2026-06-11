namespace CommerceFlow.Application;

public class DispatchShipmentHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(DispatchShipment dispatchShipment, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dispatchShipment);

        var order = await orderRepository.GetByIdAsync(dispatchShipment.OrderId, cancellationToken);
        if (order is null) return;

        order.DispatchShipment(dispatchShipment.TrackingCode);

        await orderRepository.UpdateAsync(order, cancellationToken);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record DispatchShipment(Guid OrderId, string TrackingCode);
