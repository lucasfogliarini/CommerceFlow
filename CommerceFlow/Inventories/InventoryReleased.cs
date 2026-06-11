namespace CommerceFlow;

public record InventoryReleased(Guid OrderId, Guid ProductId, int Quantity) : IDomainEvent;
