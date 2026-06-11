namespace CommerceFlow;

public record PaymentApproved(Guid OrderId, string PaymentReference) : IDomainEvent;
