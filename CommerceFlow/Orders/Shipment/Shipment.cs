using CommerceFlow;
public record Shipment
{
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

    public static Shipment Create()
    {
        var shipment = new Shipment
        {
            Status = ShipmentStatus.Created
        };
        return shipment;
    }
}