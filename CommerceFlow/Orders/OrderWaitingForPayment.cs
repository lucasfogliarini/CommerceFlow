namespace CommerceFlow;

public record OrderWaitingForPayment(Guid OrderId) : IDomainEvent;
