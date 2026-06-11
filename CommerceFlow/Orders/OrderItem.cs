namespace CommerceFlow;

public class OrderItem
{
    public Product Product { get; private set; } = default!;
    public int Quantity { get; private set; }

    public decimal TotalAmount => Quantity * Product.UnitPrice;

    public OrderItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        Product = product;
        Quantity = quantity;
    }
}
