namespace CommerceFlow.Orders;

public record OrderWaitingForPayment(string OrderNumber) : DomainEvent
{
    public override INotification[] Notifications => [ new OrdersNotification(OrderNumber, $"O pedido {OrderNumber} está aguardando pagamento.") ];
}
