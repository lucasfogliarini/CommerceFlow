namespace CommerceFlow.Orders;

public record ShipmentRequested(Guid OrderId, ShippingAddress ShippingAddress, IEnumerable<ShipmentRequestedItem> Items) : IDomainEvent;
public record ShipmentRequestedItem(Guid ProductId, int Quantity) : IDomainEvent;
