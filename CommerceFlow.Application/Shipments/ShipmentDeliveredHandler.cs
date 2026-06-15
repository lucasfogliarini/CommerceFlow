using CommerceFlow.Shipments;

namespace CommerceFlow.Application;

public class ShipmentDeliveredHandler : IDomainEventHandler<ShipmentDelivered>
{
    public Task HandleAsync(ShipmentDelivered shipmentDelivered, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentDelivered);

        // e.g., trigger post-delivery workflows

        return Task.CompletedTask;
    }
}
