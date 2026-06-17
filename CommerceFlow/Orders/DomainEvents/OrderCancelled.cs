namespace CommerceFlow.Orders;

public record OrderCancelled(Guid OrderId, string Reason) : IDomainEvent;
