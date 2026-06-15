using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public interface ICarrierSelector
{
    Task<Carrier> SelectAsync(Shipment shipment, CancellationToken cancellationToken);
}
public sealed class FakeCarrierSelector(ICarrierRepository carrierRepository) : ICarrierSelector
{
    public async Task<Carrier> SelectAsync(Shipment shipment, CancellationToken cancellationToken)
    {
        var carriers = await carrierRepository.GetAllAsync(cancellationToken);
        return carriers[Random.Shared.Next(carriers.Count)];
    }
}
