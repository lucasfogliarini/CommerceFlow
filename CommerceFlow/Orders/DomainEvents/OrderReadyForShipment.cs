namespace CommerceFlow.Orders;

public record OrderReadyForShipment(Guid OrderId, string OrderNumber, ShippingAddress ShipmentAddress, IEnumerable<OrderReadyForShipmentItem> Items) : DomainEvent
{
    public override INotification[] Notifications => [new OrdersNotification(OrderId: OrderId, $"O pedido {OrderNumber} está pronto para envio.")];
}
public record OrderReadyForShipmentItem(Guid ProductId, int Quantity);
