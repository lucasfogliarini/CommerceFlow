namespace CommerceFlow;

public class Product : AggregateRoot
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Weight { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public string ImageUrl { get; set; }

    private Product() { }

    public Product(Guid id, string slug, string name, string description, decimal unitPrice, int availableQuantity = 0)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id must be a non-empty GUID.", nameof(id));
        if (string.IsNullOrWhiteSpace(slug)) throw new ArgumentException("Slug is required.", nameof(slug));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (unitPrice <= 0) throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price must be greater than zero.");
        if (availableQuantity < 0) throw new ArgumentOutOfRangeException(nameof(availableQuantity), "Available quantity cannot be negative.");

        Id = id;
        Slug = slug;
        ImageUrl = $"/images/{slug}.svg";
        Name = name;
        Description = description;
        UnitPrice = unitPrice;
        AvailableQuantity = availableQuantity;
        ReservedQuantity = 0;
    }

    public static Product Create(Guid id, string slug, string name, string description, decimal unitPrice, int availableQuantity = 99999999) => new(id, slug, name, description, unitPrice, availableQuantity);

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
    }

    public void Release(Guid orderId, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        AvailableQuantity += quantity;
        ReservedQuantity -= quantity;
    }
}
