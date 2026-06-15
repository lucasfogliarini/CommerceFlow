using CommerceFlow.Shipments;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class CarrierRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), ICarrierRepository
{
    public Task<Carrier?> GetByIdAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        return Set<Carrier>().FirstOrDefaultAsync(c => c.Id == carrierId, cancellationToken);
    }
}
