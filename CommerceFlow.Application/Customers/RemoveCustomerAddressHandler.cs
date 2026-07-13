using CommerceFlow.Customers;

namespace CommerceFlow.Application;

public class RemoveCustomerAddressHandler(IAddressRepository addressRepository)
{
    public async Task<bool> HandleAsync(RemoveCustomerAddress command, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.GetByIdAsync(command.AddressId, command.CustomerId, cancellationToken);
        if (address is null) return false;

        await addressRepository.RemoveAsync(address, cancellationToken);
        await addressRepository.CommitScope.CommitAsync(cancellationToken);
        return true;
    }
}

public record RemoveCustomerAddress(Guid CustomerId, Guid AddressId);
