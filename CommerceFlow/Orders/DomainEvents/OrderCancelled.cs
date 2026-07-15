namespace CommerceFlow.Orders;

public record OrderCancelled(Guid OrderId, string Reason) : DomainEvent
{
    public override INotification[] Notifications => [ new OrdersNotification(OrderId: OrderId, $"O pedido {OrderId} foi cancelado. Motivo: {Reason}.") ];
}
