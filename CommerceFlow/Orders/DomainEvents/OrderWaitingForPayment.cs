namespace CommerceFlow.Orders;

public record OrderWaitingForPayment(Guid OrderId, string OrderNumber) : DomainEvent
{
    public override INotification[] Notifications => [ new OrdersNotification(OrderId: OrderId, $"O pedido {OrderNumber} está aguardando pagamento.") ];
}
