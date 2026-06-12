namespace CommerceFlow;

public record InventoryUnavailable(Guid OrderId, Guid ProductId) : IDomainEvent;
