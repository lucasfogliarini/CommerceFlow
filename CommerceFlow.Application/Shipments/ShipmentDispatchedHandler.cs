using CommerceFlow.Shipments;

namespace CommerceFlow.Application;

public class ShipmentDispatchedHandler : IDomainEventHandler<ShipmentDispatched>
{
    public Task HandleAsync(ShipmentDispatched shipmentDispatched, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentDispatched);

        // Place integration logic here (e.g., notify carriers, update read models)

        return Task.CompletedTask;
    }
}
