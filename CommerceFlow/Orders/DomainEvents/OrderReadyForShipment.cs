namespace CommerceFlow.Orders;

public record OrderReadyForShipment(Guid OrderId, Address ShipmentAddress, IEnumerable<OrderReadyForShipmentItem> Items) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, "O pedido está pronto para envio.");
}
public record OrderReadyForShipmentItem(Guid ProductId, int Quantity);
