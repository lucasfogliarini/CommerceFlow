using CommerceFlow.Accounts;
using CommerceFlow.Orders;

namespace CommerceFlow.Customers;

public class Customer : Account
{
    private Customer() { }

    public List<Order> Orders { get; private set; }
    
    public static Customer Create(Guid id, string email, string name)
    {
        return new Customer
        {
            Id = id,
            Email = email,
            Name = name
        };
    }
}
