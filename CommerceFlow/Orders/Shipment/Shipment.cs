using CommerceFlow;
public record Shipment
{
    public ShipmentStatus Status { get; private set; }
    public string? TrackingCode { get; private set; }

    public void Dispatch(string trackingCode)
    {
        TrackingCode = trackingCode;
        Status = ShipmentStatus.Dispatched;
    }

    public void Complete()
    {
        Status = ShipmentStatus.Delivered;
    }

    public static Shipment Create()
    {
        var shipment = new Shipment
        {
            Status = ShipmentStatus.Started
        };
        return shipment;
    }
}