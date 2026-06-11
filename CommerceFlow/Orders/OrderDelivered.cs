namespace CommerceFlow;

public record OrderDelivered(Guid OrderId) : IDomainEvent;
