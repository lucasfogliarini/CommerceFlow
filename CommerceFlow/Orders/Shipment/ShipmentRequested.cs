namespace CommerceFlow.Orders;

public record ShipmentRequested(Guid OrderId, ShippingAddress ShippingAddress) : IDomainEvent;
