namespace CommerceFlow.Customers;

public interface ICustomerRepository : IRepository
{
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
}
