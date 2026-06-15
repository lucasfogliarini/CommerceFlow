namespace CommerceFlow.Shipments;

public record ShipmentRequested(Guid OrderId, ShippingAddress ShippingAddress, IEnumerable<ShipmentRequestedItem> Items);
public record ShipmentRequestedItem(Guid ProductId, int Quantity);
