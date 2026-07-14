namespace CommerceFlow.Orders;

public record PaymentExpired(Guid OrderId, string OrderNumber) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, $"O pagamento do pedido {OrderNumber} expirou.");
}
