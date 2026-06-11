namespace CommerceFlow;

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

        Payment = Payment.Create(TotalAmount);
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

    public void StartShipment()
    {
        if (Status != OrderStatus.PaymentApproved)
            throw new InvalidOperationException("Order must be confirmed to start shipment.");

        Shipment = Shipment.Create();
        AddDomainEvent(new ShipmentStarted(Id));
    }

    public void DispatchShipment(string trackingCode)
    {
        if (Shipment is null || Shipment.Status != ShipmentStatus.Started)
            throw new InvalidOperationException("Shipment has not been started.");

        Shipment.Dispatch(trackingCode);
        AddDomainEvent(new OrderShipped(Id, trackingCode));
    }

    public void CompleteShipment()
    {
        if (Shipment is null || Shipment.Status != ShipmentStatus.Dispatched)
            throw new InvalidOperationException("Shipment has not been dispatched.");

        Shipment.Complete();
        Status = OrderStatus.Delivered;
        AddDomainEvent(new OrderDelivered(Id));
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Delivered orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelled(Id, reason));
    }
}