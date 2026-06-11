namespace CommerceFlow;

public record PaymentRejected(Guid OrderId, string Reason) : IDomainEvent;
