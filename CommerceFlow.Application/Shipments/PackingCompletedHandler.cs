using CommerceFlow.Shipments;

namespace CommerceFlow.Application;

public class PackingCompletedHandler : IDomainEventHandler<PackingCompleted>
{
    public Task HandleAsync(PackingCompleted packingCompleted, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packingCompleted);

        // e.g., notify fulfillment or update tracking

        return Task.CompletedTask;
    }
}
