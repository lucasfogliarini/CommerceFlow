using CommerceFlow.Customers;
using System.Security.Claims;

namespace CommerceFlow.Application;

public class CreateCustomerIfNotExistHandler(ICustomerRepository customerRepository)
{
    public async Task<Guid> HandleAsync(CreateCustomerIfNotExist createCustomer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createCustomer);

        var nameIdentifier = createCustomer.User.FindFirst(ClaimTypes.NameIdentifier)!;
        var email = createCustomer.User.FindFirst(ClaimTypes.Email)!;
        var name = createCustomer.User.FindFirst("name")!;
        var id = new Guid(nameIdentifier.Value);

        if (await customerRepository.AnyAsync(id, cancellationToken))
            return id;

        var customer = Customer.Create(id, email.Value, name.Value);
        await customerRepository.AddAsync(customer, cancellationToken);

        await customerRepository.CommitScope.CommitAsync(cancellationToken);

        return customer.Id;
    }
}

public record CreateCustomerIfNotExist(ClaimsPrincipal User);
