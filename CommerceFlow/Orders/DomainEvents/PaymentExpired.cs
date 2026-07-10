namespace CommerceFlow.Orders;

public record PaymentExpired(Guid OrderId) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, "O pagamento do pedido expirou.");
}
