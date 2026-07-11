namespace CommerceFlow.Orders;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; } = default!;
    public Guid ProductId { get; private set; } = default!;
    public Product Product { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal TotalAmount => Quantity * Product.UnitPrice;

    private OrderItem() { }

    public OrderItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        Product = product;
        Quantity = quantity;
    }
}
