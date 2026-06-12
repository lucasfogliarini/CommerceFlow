namespace CommerceFlow;

public class Product : AggregateRoot
{
    public string Name { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }

    private Product() { }

    public Product(Guid id, string name, decimal unitPrice, int availableQuantity = 0)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must be a non-empty GUID.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (unitPrice <= 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must be greater than zero.");
        if (availableQuantity < 0) throw new ArgumentOutOfRangeException(nameof(availableQuantity), "Available quantity cannot be negative.");

        Id = id;
        Name = name;
        UnitPrice = unitPrice;
        AvailableQuantity = availableQuantity;
        ReservedQuantity = 0;
    }

    public static Product Create(Guid id, string name, decimal unitPrice, int availableQuantity) => new(id, name, unitPrice, availableQuantity);

    public bool Reserve(Guid orderId, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        if (AvailableQuantity < quantity)
        {
            AddDomainEvent(new InventoryUnavailable(orderId, Id));
            return false;
        }

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
        return true;

        AddDomainEvent(new InventoryReserved(orderId, Id, quantity));
    }

    public void Release(Guid orderId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        AvailableQuantity += quantity;
        ReservedQuantity -= quantity;
    }
}
