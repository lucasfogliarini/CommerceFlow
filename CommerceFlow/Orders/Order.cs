namespace CommerceFlow.Orders;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = [];

    public string Number { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items;
    public decimal TotalAmount => _items.Sum(x => x.TotalAmount);
    public Shipment? Shipment { get; private set; }
    public Payment Payment { get; private set; }
    

    private Order() { }

    public static Order Create(Guid customerId, ShippingAddress shippingAddress, IEnumerable<OrderItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(shippingAddress);

        if (!items.Any())
            throw new ArgumentException("Order must contain at least one item.", nameof(items));

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Number = DateTime.UtcNow.Ticks.ToString(),
            CustomerId = customerId,
            Status = OrderStatus.Created,
            Shipment = Shipment.Create(shippingAddress)
        };
        order.Payment = Payment.Create(order.TotalAmount);

        order._items.AddRange(items);
        order.AddDomainEvent(new OrderCreated(order.Id, customerId));

        return order;
    }

    public void ReserveInventory()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Order must be in Created status to reserve inventory.");
        foreach (var item in Items)
        {
            var reserved = item.Product.Reserve(Id, item.Quantity);
            if (!reserved)
                throw new InvalidOperationException("Failed to reserve inventory for one or more items.");
        }
        Status = OrderStatus.InventoryReserved;
        AddDomainEvent(new OrderInventoryReserved(Id));
    }
    public void WaitForPayment()
    {
        if (Status != OrderStatus.InventoryReserved)
            throw new InvalidOperationException("Order must be in InventoryReserved status to wait for payment.");
        
        Status = OrderStatus.WaitingForPayment;
        AddDomainEvent(new OrderWaitingForPayment(Id));
    }
    public void ApprovePayment(string paymentReference)
    {
        if (Status != OrderStatus.WaitingForPayment)
            throw new InvalidOperationException("Order must be waiting for payment.");

        Payment.Approve(paymentReference);

        Status = OrderStatus.PaymentApproved;
        AddDomainEvent(new PaymentApproved(Id, paymentReference));
    }
    public void RejectPayment(string reason)
    {
        if (Status != OrderStatus.WaitingForPayment)
            throw new InvalidOperationException("Order must be waiting for payment.");

        Payment.Reject(reason);

        AddDomainEvent(new PaymentRejected(Id, reason));

        Cancel(reason);
    }
    public void RequestShipment()
    {
        if (Status != OrderStatus.PaymentApproved)
            throw new InvalidOperationException("Order must be confirmed to start shipment.");

        var items = Items.Select(x => new ShipmentRequestedItem(x.ProductId, x.Quantity)).ToList();

        Shipment!.Request();
        AddDomainEvent(new ShipmentRequested(Id, Shipment.Address, items));
    }
    public void DispatchShipment(string trackingCode)
    {
        if (Shipment is null || Shipment.Status != ShipmentStatus.Requested)
            throw new InvalidOperationException("Shipment has not been started.");

        Status = OrderStatus.Dispatched;

        Shipment.Dispatch(trackingCode);
    }
    public void DeliverShipment()
    {
        if (Shipment is null || Shipment.Status != ShipmentStatus.Dispatched)
            throw new InvalidOperationException("Shipment has not been dispatched.");

        Shipment.Deliver();
        Status = OrderStatus.Delivered;
    }
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelled(Id, reason));
    }
}