namespace CommerceFlow;

public class Address : Entity
{
    private Address() { }

    public Guid CustomerId { get; private set; }
    public string Street { get; private set; } = default!;
    public string Number { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string State { get; private set; } = default!;
    public string ZipCode { get; private set; } = default!;
    public string Country { get; private set; } = default!;

    public static Address Create(Guid customerId, string street, string number, string city, string state, string zipCode, string country)
    {
        return new Address
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Street = street,
            Number = number,
            City = city,
            State = state,
            ZipCode = zipCode,
            Country = country
        };
    }

    public void Update(string street, string number, string city, string state, string zipCode, string country)
    {
        Street = street;
        Number = number;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }
}
