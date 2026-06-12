namespace CommerceFlow.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

public class OrderRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(order, cancellationToken);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Set<Order>()
            .Include(o=>o.Items)
            .ThenInclude(i=>i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public void Update(Order order)
    {
        dbContext.Update(order);
    }
}
