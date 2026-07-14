namespace CommerceFlow.Orders;

public record PaymentRejected(Guid OrderId, string OrderNumber, string Reason) : DomainEvent
{    public override NotificationRequest Notification => new(AggregateId: OrderId, $"O pagamento do pedido {OrderNumber} foi recusado. Motivo: {Reason}");
}
