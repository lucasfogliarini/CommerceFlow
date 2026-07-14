namespace CommerceFlow.Infrastructure.Repositories;

using CommerceFlow.Shipments;
using Microsoft.EntityFrameworkCore;

public class ShipmentRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), IShipmentRepository
{
    public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(shipment, cancellationToken);
    }

    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Set<Shipment>()
                .Include(s=>s.Carrier)
                .Include(s=>s.Tracking)
                .ThenInclude(t=>t.Events)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
    public async Task<IEnumerable<Shipment>> GetShipmentsAsync(CancellationToken cancellationToken = default)
    {
        return await Set<Shipment>()
                .Include(s => s.Carrier)
                .Include(s => s.Tracking)
                .ThenInclude(t => t.Events)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
    }

    public Task<Shipment?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return Set<Shipment>()
                .Include(s => s.Carrier)
                .Include(s => s.Tracking)
                .ThenInclude(t => t.Events)
                .FirstOrDefaultAsync(s => s.OrderNumber == orderNumber, cancellationToken);
    }

    public void Update(Shipment shipment)
    {
        dbContext.Update(shipment);
    }
}
