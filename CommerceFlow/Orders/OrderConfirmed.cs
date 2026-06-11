namespace CommerceFlow;

public record OrderConfirmed(Guid OrderId) : IDomainEvent;
