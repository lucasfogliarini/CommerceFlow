namespace CommerceFlow.Orders;

public record OrderShipment
{
    public ShipmentStatus Status { get; private set; }
    public string? TrackingCode { get; private set; }
    public Address? Address { get; set; }

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

    public static OrderShipment Create(Address shippingAddress)
    {
        var shipment = new OrderShipment
        {
            Status = ShipmentStatus.Created,
            Address = shippingAddress
        };
        return shipment;
    }
}