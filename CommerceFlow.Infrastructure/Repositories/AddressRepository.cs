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
    public async Task RemoveAsync(Guid Id, CancellationToken cancellationToken = default)
    {
        var address = await Set<Address>().FirstOrDefaultAsync(a => a.Id == Id, cancellationToken);
        if (address != null)
            dbContext.Remove(address);
    }
}
