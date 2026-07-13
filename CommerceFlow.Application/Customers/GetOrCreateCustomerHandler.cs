using CommerceFlow.Customers;
using System.Security.Claims;

namespace CommerceFlow.Application;

public class GetOrCreateCustomerHandler(ICustomerRepository customerRepository)
{
    public async Task<Customer> HandleAsync(GetOrCreateCustomer createCustomer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createCustomer);

        var nameIdentifier = createCustomer.User.FindFirst(ClaimTypes.NameIdentifier)!;
        
        var id = new Guid(nameIdentifier.Value);

        var customer = await customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            var email = createCustomer.User.FindFirst(ClaimTypes.Email)!;
            var name = createCustomer.User.FindFirst("name")!;

            customer = Customer.Create(id, email.Value, name.Value);
            await customerRepository.AddAsync(customer, cancellationToken);

            await customerRepository.CommitScope.CommitAsync(cancellationToken);
        }

        return customer;
    }
}

public record GetOrCreateCustomer(ClaimsPrincipal User);
