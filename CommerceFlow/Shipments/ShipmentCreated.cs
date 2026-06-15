
namespace CommerceFlow.Shipments;

public sealed record ShipmentCreated(Guid ShipmentId, Guid OrderId) : IDomainEvent;
