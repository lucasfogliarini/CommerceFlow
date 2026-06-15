using CommerceFlow.Shipments;

namespace CommerceFlow.Application;

public class ShipmentCreatedHandler : IDomainEventHandler<ShipmentCreated>
{
    public Task HandleAsync(ShipmentCreated shipmentCreated, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentCreated);

        // Intentionally left minimal: integration between shipment and other contexts
        // can be added here (e.g., notifications, read model updates).

        return Task.CompletedTask;
    }
}
