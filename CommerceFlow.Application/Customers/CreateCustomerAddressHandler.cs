using CommerceFlow.Customers;

namespace CommerceFlow.Application;

public class CreateCustomerAddressHandler(IAddressRepository addressRepository)
{
    public async Task<Address?> HandleAsync(CreateCustomerAddress createCustomerAddress, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createCustomerAddress);

        var address = Address.Create(createCustomerAddress.CustomerId, createCustomerAddress.Street, createCustomerAddress.Number, createCustomerAddress.City, createCustomerAddress.State, createCustomerAddress.ZipCode, createCustomerAddress.Country);
        await addressRepository.AddAsync(address, cancellationToken);
        await addressRepository.CommitScope.CommitAsync(cancellationToken);
        return address;
    }
}

public record CreateCustomerAddress(Guid CustomerId, string Street, string Number, string City, string State, string ZipCode, string Country);
