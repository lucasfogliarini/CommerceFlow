namespace CommerceFlow.Orders;

public record PaymentExpired(string OrderNumber) : DomainEvent
{
    public override INotification[] Notifications => [new OrdersNotification(OrderNumber, $"O pagamento do pedido {OrderNumber} expirou.")];
}
