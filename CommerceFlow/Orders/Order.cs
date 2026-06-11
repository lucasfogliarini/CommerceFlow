namespace CommerceFlow;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public string Number { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items;
    public decimal TotalAmount => _items.Sum(x => x.TotalAmount);

    private Order() { }

    public static Order Create(Guid customerId, IEnumerable<OrderItem> items)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));

        if (!items.Any())
            throw new ArgumentException("Order must contain at least one item.", nameof(items));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Number = DateTime.UtcNow.Ticks.ToString(),
            CustomerId = customerId,
            Status = OrderStatus.Created
        };

        order._items.AddRange(items);
        order.AddDomainEvent(new OrderCreated(order.Id, customerId));

        return order;
    }

    public void WaitForPayment()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Order must be in Created status to wait for payment.");

        Status = OrderStatus.WaitingForPayment;
        AddDomainEvent(new OrderWaitingForPayment(Id));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.WaitingForPayment)
            throw new InvalidOperationException("Order must be waiting for payment.");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmed(Id));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelled(Id, reason));
    }
}