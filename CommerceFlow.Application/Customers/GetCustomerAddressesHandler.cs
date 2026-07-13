using CommerceFlow.Customers;

namespace CommerceFlow.Application;

public class GetCustomerAddressesHandler(ICustomerRepository customerRepository)
{
    public Task<List<Address>> HandleAsync(GetCustomerAddresses query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        return customerRepository.GetAddressesAsync(query.CustomerId, cancellationToken);
    }
}

public record GetCustomerAddresses(Guid CustomerId);
