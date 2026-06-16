using CommerceFlow.Application.Shipments;
using CommerceFlow.Shipments;

namespace CommerceFlow.Application;

public class SelectAndAssignCarrierHandler(IShipmentRepository shipmentRepository, ICarrierSelector carrierSelector) : IDomainEventHandler<ShipmentCreated>
{
    public async Task HandleAsync(ShipmentCreated shipmentCreated, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentCreated);

        var shipment = await shipmentRepository.GetByIdAsync(shipmentCreated.ShipmentId, cancellationToken) ??
                            throw new InvalidOperationException($"Shipment '{shipmentCreated.ShipmentId}' not found.");

        var carrier = await carrierSelector.SelectAsync(shipment, cancellationToken);

        shipment.AssignCarrier(carrier);

        shipmentRepository.Update(shipment);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
