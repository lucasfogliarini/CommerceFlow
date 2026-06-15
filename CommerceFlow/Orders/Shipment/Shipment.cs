namespace CommerceFlow.Orders;
public record Shipment
{
    public ShipmentStatus Status { get; private set; }
    public string? TrackingCode { get; private set; }
    public ShippingAddress? Address { get; set; }

    public void Request()
    {
        Status = ShipmentStatus.Requested;
    }

    public void Dispatch(string trackingCode)
    {
        TrackingCode = trackingCode;
        Status = ShipmentStatus.Dispatched;
    }

    public void Deliver()
    {
        Status = ShipmentStatus.Delivered;
    }

    public static Shipment Create(ShippingAddress shippingAddress)
    {
        var shipment = new Shipment
        {
            Status = ShipmentStatus.Created,
            Address = shippingAddress
        };
        return shipment;
    }
}