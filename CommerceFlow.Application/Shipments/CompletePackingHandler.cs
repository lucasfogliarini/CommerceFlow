using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public sealed class CompletePackingHandler(IShipmentRepository shipmentRepository)
{
    public async Task HandleAsync(
        CompletePacking completePacking,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(completePacking);

        var shipment = await shipmentRepository.GetByIdAsync(
            completePacking.ShipmentId,
            cancellationToken) ?? throw new InvalidOperationException(
                $"Shipment '{completePacking.ShipmentId}' not found.");

        shipment.CompletePacking();

        shipmentRepository.Update(shipment);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record CompletePacking(Guid ShipmentId);