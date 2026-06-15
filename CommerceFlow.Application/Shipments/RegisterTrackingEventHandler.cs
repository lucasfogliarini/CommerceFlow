using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public sealed class RegisterTrackingEventHandler(
    IShipmentRepository shipmentRepository)
{
    public async Task HandleAsync(
        RegisterTrackingEvent registerTrackingEvent,
        CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(
            registerTrackingEvent.ShipmentId,
            cancellationToken) ?? throw new InvalidOperationException(
                $"Shipment '{registerTrackingEvent.ShipmentId}' not found.");

        shipment.RegisterTrackingEvent(registerTrackingEvent.Description, registerTrackingEvent.Location);

        shipmentRepository.Update(shipment);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record RegisterTrackingEvent(
    Guid ShipmentId,
    string Description,
    string Location);