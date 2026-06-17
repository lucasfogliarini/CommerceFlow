namespace CommerceFlow.Orders;

public record OrderWaitingForPayment(Guid OrderId) : IDomainEvent;
