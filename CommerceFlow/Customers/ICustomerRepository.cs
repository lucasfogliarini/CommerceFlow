namespace CommerceFlow.Customers;

public interface ICustomerRepository : IRepository
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<List<Address>> GetAddressesAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    void Update(Customer customer);
}
