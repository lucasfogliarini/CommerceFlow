
namespace CommerceFlow.Shipments;

public sealed record ShipmentDelivered(Guid ShipmentId) : IDomainEvent;
