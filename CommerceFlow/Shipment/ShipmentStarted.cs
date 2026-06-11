namespace CommerceFlow;

public record ShipmentStarted(Guid OrderId) : IDomainEvent;
