namespace CommerceFlow.Orders;

public record OrderInventoryReserved(Guid OrderId) : IDomainEvent;