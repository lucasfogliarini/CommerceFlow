namespace CommerceFlow;

public record PaymentRejected(Guid OrderId, Guid PaymentId, string Reason) : IDomainEvent;
