using CommerceFlow.Customers;
using Microsoft.EntityFrameworkCore;

namespace CommerceFlow.Infrastructure.Repositories;

public class CustomerRepository(CommerceFlowDbContext dbContext) : Repository(dbContext), ICustomerRepository
{
    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Set<Customer>()
                     .Include(c=>c.Orders)
                     .ThenInclude(o=>o.Items)
                     .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> AnyAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await Set<Customer>()
                     .AnyAsync(c => c.Id == customerId, cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await Set<Customer>()
            .Include(customer => customer.Addresses)
            .FirstOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);
    }

    public async Task<List<Address>> GetAddressesAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await Set<Address>()
            .Where(address => address.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(customer, cancellationToken);
    }

    public void Update(Customer customer)
    {
        dbContext.Update(customer);
    }
}
