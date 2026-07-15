namespace CommerceFlow.Orders;

public record PaymentApproved(string OrderNumber, string PaymentReference) : DomainEvent
{
    public override INotification[] Notifications => [ new OrdersNotification(OrderNumber, $"O pagamento do pedido {OrderNumber} foi aprovado.") ];
}
