
namespace CommerceFlow.Shipments;

public sealed record CarrierAssigned(Guid ShipmentId, Guid CarrierId) : IDomainEvent;