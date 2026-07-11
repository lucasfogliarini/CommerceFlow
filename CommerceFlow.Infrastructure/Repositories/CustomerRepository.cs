using CommerceFlow.Customers;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class CustomerRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), ICustomerRepository
{
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Set<Customer>().FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }
}
