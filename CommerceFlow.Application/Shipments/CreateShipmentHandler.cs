using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public class RequestShipmentHandler(IShipmentRepository shipmentRepository) : IDomainEventHandler<ShipmentRequested>
{
    public async Task HandleAsync(ShipmentRequested shipmentRequested, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentRequested);

        var shipment = Shipment.Create(shipmentRequested.OrderId, shipmentRequested.ShippingAddress, null);

        await shipmentRepository.AddAsync(shipment, cancellationToken);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
