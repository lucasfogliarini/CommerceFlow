namespace CommerceFlow.Orders;

public record PaymentApproved(Guid OrderId, string OrderNumber, string PaymentReference) : DomainEvent
{
    public override INotification[] Notifications => [ new OrdersNotification(OrderId: OrderId, $"O pagamento do pedido {OrderNumber} foi aprovado.") ];
}
