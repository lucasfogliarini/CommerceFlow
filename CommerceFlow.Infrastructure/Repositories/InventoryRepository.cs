using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class InventoryRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), IInventoryRepository
{

    public Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return Set<Inventory>().FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(inventory, cancellationToken);
    }

    public void Update(Inventory inventory)
    {
        dbContext.Update(inventory);
    }
}
