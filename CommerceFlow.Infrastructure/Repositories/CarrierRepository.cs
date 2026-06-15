using CommerceFlow.Shipments;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class CarrierRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), ICarrierRepository
{
    public async Task<IList<Carrier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Set<Carrier>().ToListAsync(cancellationToken);
    }

    public async Task<Carrier?> GetByIdAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        return await Set<Carrier>().FirstOrDefaultAsync(c => c.Id == carrierId, cancellationToken);
    }
}
