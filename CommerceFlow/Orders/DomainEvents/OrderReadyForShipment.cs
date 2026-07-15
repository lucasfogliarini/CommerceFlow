namespace CommerceFlow.Orders;

public record OrderReadyForShipment( string OrderNumber, ShippingAddress ShipmentAddress, IEnumerable<OrderReadyForShipmentItem> Items) : DomainEvent
{
    public override INotification[] Notifications => [new OrdersNotification(OrderNumber, $"O pedido {OrderNumber} está pronto para envio.")];
}
public record OrderReadyForShipmentItem(Guid ProductId, int Quantity);
