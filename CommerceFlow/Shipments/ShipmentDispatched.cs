
namespace CommerceFlow.Shipments;

public sealed record ShipmentDispatched(Guid ShipmentId, string TrackingCode) : IDomainEvent;
