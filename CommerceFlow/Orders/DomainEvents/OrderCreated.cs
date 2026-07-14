namespace CommerceFlow.Orders;

public record OrderCreated(string OrderNumber, Guid CustomerId) : IDomainEvent;
