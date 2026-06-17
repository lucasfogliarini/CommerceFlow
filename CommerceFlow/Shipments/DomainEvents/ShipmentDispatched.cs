
namespace CommerceFlow.Shipments;

public sealed record ShipmentDispatched(Guid ShipmentId, Guid OrderId, string TrackingCode) : IDomainEvent;
