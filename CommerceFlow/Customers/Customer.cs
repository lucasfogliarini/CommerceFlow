using CommerceFlow.Orders;

namespace CommerceFlow.Customers;

public class Customer : Entity
{
    public string Email { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public List<Order> Orders { get; private set; }

    private Customer() { }

    public static Customer Create(string email, string name)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = name
        };
    }
}
