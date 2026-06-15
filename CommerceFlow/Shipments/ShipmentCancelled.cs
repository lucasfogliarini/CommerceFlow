
namespace CommerceFlow.Shipments;
public sealed record ShipmentCancelled(Guid ShipmentId) : IDomainEvent;
