using CommerceFlow.Orders;

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

public  record ShipmentDispatched(Guid ShipmentId, Guid OrderId, string TrackingCode);
