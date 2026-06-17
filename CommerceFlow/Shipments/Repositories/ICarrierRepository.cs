namespace CommerceFlow.Shipments;

public interface ICarrierRepository : IRepository
{
    Task<Carrier?> GetByIdAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<IList<Carrier>> GetAllAsync(CancellationToken cancellationToken = default);
}