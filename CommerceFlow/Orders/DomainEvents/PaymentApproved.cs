namespace CommerceFlow.Orders;

public record PaymentApproved(Guid OrderId, string PaymentReference) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, $"O pagamento do pedido foi aprovado. Referência: {PaymentReference}");
}
