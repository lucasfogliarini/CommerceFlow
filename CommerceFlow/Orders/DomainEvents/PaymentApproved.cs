namespace CommerceFlow.Orders;

public record PaymentApproved(Guid OrderId, string PaymentReference) : IDomainEvent;
