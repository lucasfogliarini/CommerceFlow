namespace CommerceFlow.Shipments;

public sealed record PackingCompleted(Guid ShipmentId) : IDomainEvent;
