namespace CommerceFlow.Orders;

public record PaymentRejected(Guid OrderId, string OrderNumber, string Reason) : DomainEvent
{    public override INotification[] Notifications => [new OrdersNotification(OrderId: OrderId, $"O pagamento do pedido {OrderNumber} foi recusado. Motivo: {Reason}")];
}
