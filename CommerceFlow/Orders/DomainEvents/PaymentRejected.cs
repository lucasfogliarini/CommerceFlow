namespace CommerceFlow.Orders;

public record PaymentRejected(Guid OrderId, string Reason) : DomainEvent
{    public override NotificationRequest Notification => new(AggregateId: OrderId, $"O pagamento do pedido foi recusado. Motivo: {Reason}");
}
