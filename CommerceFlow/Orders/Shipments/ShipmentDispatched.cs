namespace CommerceFlow.Orders;

public record ShipmentDispatched(Guid ShipmentId, Guid OrderId, string TrackingCode);
