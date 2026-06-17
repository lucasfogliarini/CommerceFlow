namespace CommerceFlow.Orders;

public record OrderReadyForShipment(Guid OrderId, Address ShipmentAddress, IEnumerable<OrderReadyForShipmentItem> Items) : IDomainEvent;
public record OrderReadyForShipmentItem(Guid ProductId, int Quantity);
