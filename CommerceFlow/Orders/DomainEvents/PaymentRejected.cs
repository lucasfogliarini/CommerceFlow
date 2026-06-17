namespace CommerceFlow.Orders;

public record PaymentRejected(Guid OrderId, string Reason) : IDomainEvent;
