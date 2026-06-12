using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class ProductRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), IProductRepository
{
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(product, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByIds(Guid[] ids, CancellationToken cancellationToken = default)
    {
        return await Set<Product>().Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
    }
}
