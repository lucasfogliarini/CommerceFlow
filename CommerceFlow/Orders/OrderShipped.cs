namespace CommerceFlow;

public record OrderShipped(Guid OrderId, string TrackingCode) : IDomainEvent;
