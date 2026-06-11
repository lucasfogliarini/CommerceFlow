namespace CommerceFlow;

public class Inventory : AggregateRoot
{
    public Guid ProductId { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }

    public void Reserve(Guid orderId, int quantity)
    {
        if (AvailableQuantity < quantity)
        {
            AddDomainEvent(new InventoryUnavailable(orderId, ProductId));
            return;
        }

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;

        AddDomainEvent(new InventoryReserved(orderId, ProductId, quantity));
    }

    public void Release(Guid orderId, int quantity)
    {
        AvailableQuantity += quantity;
        ReservedQuantity -= quantity;

        AddDomainEvent(new InventoryReleased(orderId, ProductId, quantity));
    }

    public static Inventory Create(Guid productId, int availableQuantity)
    {
        var inventory = new Inventory
        {
            ProductId = productId,
            AvailableQuantity = availableQuantity,
            ReservedQuantity = 0
        };
        return inventory;
    }
}