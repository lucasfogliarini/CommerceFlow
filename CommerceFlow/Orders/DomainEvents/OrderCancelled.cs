namespace CommerceFlow.Orders;

public record OrderCancelled(string OrderNumber, string Reason) : DomainEvent
{
    public override INotification[] Notifications => [ new OrdersNotification(OrderNumber, $"O pedido {OrderNumber} foi cancelado. Motivo: {Reason}.") ];
}
