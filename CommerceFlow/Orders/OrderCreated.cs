namespace CommerceFlow;

public record OrderCreated(Guid OrderId, Guid CustomerId) : IDomainEvent;
