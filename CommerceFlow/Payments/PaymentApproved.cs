namespace CommerceFlow;

public record PaymentApproved(Guid OrderId, Guid PaymentId) : IDomainEvent;
