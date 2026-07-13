namespace CommerceFlow.Orders;

public class Order : AggregateRoot
{
    public string Number { get; private set; } = default!;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public decimal? TotalAmount => Items?.Sum(x => x.TotalAmount);
    public OrderShipment? Shipment { get; private set; }
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
            Items = [.. items],
            Shipment = OrderShipment.Create(shippingAddress)
        };
        order.Payment = Payment.Create(order.TotalAmount.GetValueOrDefault());
        order.AddDomainEvent(new OrderCreated(order.Id, customerId));

        return order;
    }

    public bool CanExpirePayment()
    {
        if (Status != OrderStatus.WaitingForPayment)
            return false;

        return true;
    }

    public void ExpirePayment()
    {
        if (Status != OrderStatus.WaitingForPayment)
            return;

        Status = OrderStatus.PaymentExpired;

        AddDomainEvent(new PaymentExpired(Id));
    }
    public void ReleaseInventory()
    {
        if (Status is not (OrderStatus.PaymentRejected or OrderStatus.PaymentExpired))
            throw new InvalidOperationException("Order must be in a cancelled or expired state to release inventory.");

        foreach (var item in Items)
        {
            item.Product.Release(Id, item.Quantity);
        }
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
        Status = OrderStatus.WaitingForPayment;

        AddDomainEvent(new OrderInventoryReserved(Id));
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
    public void RejectPayment(string paymentReference, string reason)
    {
        if (Status != OrderStatus.WaitingForPayment)
            throw new InvalidOperationException("Order must be waiting for payment.");

        Payment.Reject(paymentReference, reason);
        Status = OrderStatus.PaymentRejected;

        AddDomainEvent(new PaymentRejected(Id, reason));
    }
    public void ReadyForShipment()
    {
        if (Status != OrderStatus.PaymentApproved)
            throw new InvalidOperationException("Order must be payed to be ready for shipment.");

        var items = Items.Select(x => new OrderReadyForShipmentItem(x.ProductId, x.Quantity)).ToList();

        Shipment!.Request();
        Status = OrderStatus.ReadyForShipment;
        AddDomainEvent(new OrderReadyForShipment(Id, Shipment.Address, items));
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