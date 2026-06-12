namespace CommerceFlow;

public record OrderInventoryReserved(Guid OrderId) : IDomainEvent;