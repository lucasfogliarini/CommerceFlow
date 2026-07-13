namespace CommerceFlow.Customers;

public interface IAddressRepository : IRepository
{
    Task AddAsync(Address address, CancellationToken cancellationToken = default);
    void Update(Address address);
    Task RemoveAsync(Guid Id, CancellationToken cancellationToken = default);
}
