using CommerceFlow.Orders;
using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Orders;

public class ShipmentDispatchedHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(ShipmentDispatched shipmentDispatched, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentDispatched);

        var order = await orderRepository.GetByIdAsync(shipmentDispatched.OrderId, cancellationToken);
        if (order is null) return;

        order.DispatchShipment(shipmentDispatched.TrackingCode);

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
