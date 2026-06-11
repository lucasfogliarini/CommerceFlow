using CommerceFlow;
public class Shipment : AggregateRoot
{
    public Guid OrderId { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public string? TrackingCode { get; private set; }

    public void Start()
    {
        Status = ShipmentStatus.Picking;
        AddDomainEvent(new ShipmentStarted(OrderId));
    }

    public void Dispatch(string trackingCode)
    {
        TrackingCode = trackingCode;
        Status = ShipmentStatus.Shipped;
        AddDomainEvent(new OrderShipped(OrderId, trackingCode));
    }

    public void Complete()
    {
        Status = ShipmentStatus.Delivered;
        AddDomainEvent(new OrderDelivered(OrderId));
    }
}