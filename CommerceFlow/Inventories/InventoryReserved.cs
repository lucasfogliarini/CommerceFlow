namespace CommerceFlow;

public record InventoryReserved(Guid OrderId, Guid ProductId, int Quantity) : IDomainEvent;
