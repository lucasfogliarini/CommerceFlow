using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class ShipmentDeliveredHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(ShipmentDelivered shipmentDelivered, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentDelivered);

        var order = await orderRepository.GetByIdAsync(shipmentDelivered.OrderId, cancellationToken);
        if (order is null) return;

        order.DeliverShipment();

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record ShipmentDelivered(Guid OrderId);
