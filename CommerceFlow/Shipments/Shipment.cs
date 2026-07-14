namespace CommerceFlow.Shipments;

public class Shipment : AggregateRoot
{
    private readonly List<ShipmentItem> _items = [];

    private Shipment()
    {
    }
    public DateTime CreatedAt { get; private set; }

    public string OrderNumber { get; private set; }

    public ShipmentStatus Status { get; private set; }

    public ShippingAddress Address { get; private set; }

    public Carrier? Carrier { get; private set; }

    public Tracking? Tracking { get; private set; }

    public IReadOnlyCollection<ShipmentItem> Items => _items;

    public static Shipment Create(
        string orderNumber,
        ShippingAddress shipmentAddress,
        IEnumerable<ShipmentItem> items)
    {
        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OrderNumber = orderNumber,
            Address = shipmentAddress,
            Status = ShipmentStatus.Created
        };

        shipment._items.AddRange(items);

        shipment.AddDomainEvent(
            new ShipmentCreated(
                shipment.Id,
                shipment.OrderNumber));

        return shipment;
    }

    public void AssignCarrier(Carrier carrier)
    {
        ArgumentNullException.ThrowIfNull(carrier);

        if (Status != ShipmentStatus.Created)
            throw new InvalidOperationException(
                "Carrier can only be assigned to a newly created shipment.");

        Carrier = carrier;

        Status = ShipmentStatus.CarrierAssigned;

        AddDomainEvent(new CarrierAssigned(Id, OrderNumber, Carrier.Name));
    }

    public void CompletePacking()
    {
        if (Status != ShipmentStatus.CarrierAssigned)
            throw new InvalidOperationException(
                "Carrier must be assigned before packing.");

        Status = ShipmentStatus.Packed;

        AddDomainEvent(new PackingCompleted(Id, OrderNumber));
    }

    public void Dispatch()
    {
        if (Status != ShipmentStatus.Packed)
            throw new InvalidOperationException(
                "Shipment must be packed before dispatch.");

        Tracking = Tracking.Create();

        Tracking.AddEvent(DateTime.UtcNow, "Shipment dispatched", Address.City);

        Status = ShipmentStatus.Dispatched;

        AddDomainEvent(
            new ShipmentDispatched(
                Id,
                OrderNumber,
                Tracking.TrackingCode));
    }

    public void RegisterTrackingEvent(
        string description,
        string location)
    {
        if (Status != ShipmentStatus.Dispatched)
            throw new InvalidOperationException(
                "Tracking updates are only allowed after dispatch.");

        Tracking!.AddEvent(
                DateTime.UtcNow,
                description,
                location);

        AddDomainEvent(
            new TrackingUpdated(
                Id,
                OrderNumber,
                description,
                location));
    }

    public void Deliver()
    {
        if (Status != ShipmentStatus.Dispatched)
            throw new InvalidOperationException(
                "Shipment must be dispatched before delivery.");

        Status = ShipmentStatus.Delivered;

        AddDomainEvent(new ShipmentDelivered(Id, OrderNumber));
    }

    public void Cancel()
    {
        if (Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException(
                "Delivered shipments cannot be cancelled.");

        Status = ShipmentStatus.Cancelled;

        AddDomainEvent(new ShipmentCancelled(Id, OrderNumber));
    }
}