namespace CommerceFlow.Orders;

public record OrderCreated(Guid OrderId, Guid CustomerId) : IDomainEvent;
