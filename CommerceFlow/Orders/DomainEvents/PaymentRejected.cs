namespace CommerceFlow.Orders;

public record PaymentRejected(string OrderNumber, string Reason) : DomainEvent
{    public override INotification[] Notifications => [new OrdersNotification(OrderNumber, $"O pagamento do pedido {OrderNumber} foi recusado. Motivo: {Reason}")];
}
