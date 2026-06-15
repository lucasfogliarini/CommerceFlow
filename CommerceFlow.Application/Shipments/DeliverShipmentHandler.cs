using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public sealed class DeliverShipmentHandler(
    IShipmentRepository shipmentRepository)
{
    public async Task HandleAsync(
        DeliverShipment deliverShipment,
        CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(
            deliverShipment.ShipmentId,
            cancellationToken) ?? throw new InvalidOperationException(
                $"Shipment '{deliverShipment.ShipmentId}' not found.");

        shipment.Deliver();

        shipmentRepository.Update(shipment);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record DeliverShipment(Guid ShipmentId);