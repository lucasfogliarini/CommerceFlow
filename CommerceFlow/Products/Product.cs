namespace CommerceFlow;

public class Product : Entity
{
    public string Name { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }

    private Product() { }

    public Product(Guid id, string name, decimal unitPrice)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must be a non-empty GUID.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (unitPrice <= 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must be greater than zero.");

        Id = id;
        Name = name;
        UnitPrice = unitPrice;
    }

    public static Product Create(Guid id, string name, decimal unitPrice) => new(id, name, unitPrice);
}
