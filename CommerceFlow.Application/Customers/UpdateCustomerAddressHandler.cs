using CommerceFlow.Customers;

namespace CommerceFlow.Application;

public class UpdateCustomerAddressHandler(IAddressRepository addressRepository)
{
    public async Task<bool> HandleAsync(UpdateCustomerAddress command, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.GetByIdAsync(command.AddressId, command.CustomerId, cancellationToken);
        if (address is null) return false;

        address.Update(command.Street, command.Number, command.City, command.State, command.ZipCode, command.Country);
        addressRepository.Update(address);
        await addressRepository.CommitScope.CommitAsync(cancellationToken);
        return true;
    }
}

public record UpdateCustomerAddress(Guid CustomerId, Guid AddressId, string Street, string Number, string City, string State, string ZipCode, string Country);
