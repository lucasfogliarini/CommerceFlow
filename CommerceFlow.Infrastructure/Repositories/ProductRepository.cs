using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class ProductRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return Set<Product>().FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(product, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByIds(Guid[] ids, CancellationToken cancellationToken = default)
    {
        return await Set<Product>().Where(p => ids.Contains(p.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return await Set<Product>().ToListAsync(cancellationToken);
    }

    public void Update(Product product)
    {
        dbContext.Update(product);
    }    
}
