namespace CommerceFlow.Shipments;

public enum ShipmentStatus
{
    Created = 1,
    CarrierAssigned = 2,
    Packed = 3,
    Dispatched = 4,
    Delivered = 5,
    Cancelled = 6
}