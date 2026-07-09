namespace CommerceFlow.Orders;

public record PaymentExpired(Guid OrderId) : IDomainEvent;
