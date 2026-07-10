using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public sealed class DispatchShipmentHandler(IShipmentRepository shipmentRepository) : IDomainEventHandler<PackingCompleted>
{
    public async Task HandleAsync(PackingCompleted packingCompleted, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packingCompleted);

        var shipment = await shipmentRepository.GetByIdAsync(
            packingCompleted.ShipmentId,
            cancellationToken) ?? throw new InvalidOperationException(
                $"Shipment '{packingCompleted.ShipmentId}' not found.");

        shipment.Dispatch();

        shipmentRepository.Update(shipment);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}