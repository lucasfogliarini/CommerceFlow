namespace CommerceFlow.Orders;

public record PaymentExpired(Guid OrderId, string OrderNumber) : DomainEvent
{
    public override INotification[] Notifications => [new OrdersNotification(OrderId: OrderId, $"O pagamento do pedido {OrderNumber} expirou.")];
}
