namespace CommerceFlow.Infrastructure.Repositories;

using CommerceFlow.Orders;
using Microsoft.EntityFrameworkCore;

public class OrderRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), IOrderRepository
{
    public async Task<Guid?> GetCustomerIdAsync(string number, CancellationToken cancellationToken = default)
    {
        var order = await Set<Order>()
            .FirstOrDefaultAsync(o => o.Number == number, cancellationToken);
        return order?.CustomerId;
    }
    public async Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await Set<Order>()
            .OrderBy(o=> o.Status)
            .ToListAsync(cancellationToken);
    }   

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(order, cancellationToken);
    }

    public Task<Order?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return Set<Order>()
            .Include(o=>o.Items)
            .ThenInclude(i=>i.Product)
            .FirstOrDefaultAsync(o => o.Number == number, cancellationToken);
    }

    public void Update(Order order)
    {
        dbContext.Update(order);
    }
}
