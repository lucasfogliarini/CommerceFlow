using CommerceFlow;
public class Shipment
{
    public Guid OrderId { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public string? TrackingCode { get; private set; }

    public void Start()
    {
        Status = ShipmentStatus.Pending;
    }

    public void Dispatch(string trackingCode)
    {
        TrackingCode = trackingCode;
        Status = ShipmentStatus.Shipped;
    }

    public void Complete()
    {
        Status = ShipmentStatus.Delivered;
    }

    public static Shipment Create(Guid orderId)
    {
        var shipment = new Shipment
        {
            OrderId = orderId,
            Status = ShipmentStatus.Pending
        };
        return shipment;
    }
}