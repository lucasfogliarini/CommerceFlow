namespace CommerceFlow;

public record OrderCancelled(Guid OrderId, string Reason) : IDomainEvent;
