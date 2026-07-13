namespace CommerceFlow.Customers;

public interface IAddressRepository : IRepository
{
    Task AddAsync(Address address, CancellationToken cancellationToken = default);
    Task<Address?> GetByIdAsync(Guid addressId, Guid customerId, CancellationToken cancellationToken = default);
    void Update(Address address);
    Task RemoveAsync(Address address, CancellationToken cancellationToken = default);
}
