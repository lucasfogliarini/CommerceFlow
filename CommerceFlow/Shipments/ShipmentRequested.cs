namespace CommerceFlow.Shipments;

public record ShipmentRequested(Guid OrderId, ShippingAddress ShippingAddress) : IDomainEvent;
