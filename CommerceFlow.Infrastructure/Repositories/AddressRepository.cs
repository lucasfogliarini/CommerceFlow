using CommerceFlow.Customers;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class AddressRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), IAddressRepository
{
    public async Task AddAsync(Address address, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(address, cancellationToken);
    }

    public void Update(Address address)
    {
        dbContext.Update(address);
    }

    public async Task<Address?> GetByIdAsync(Guid addressId, Guid customerId, CancellationToken cancellationToken = default)
    {
        return await Set<Address>()
            .FirstOrDefaultAsync(a => a.Id == addressId && a.CustomerId == customerId, cancellationToken);
    }

    public Task RemoveAsync(Address address, CancellationToken cancellationToken = default)
    {
        dbContext.Remove(address);
        return Task.CompletedTask;
    }
}
