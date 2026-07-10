namespace CommerceFlow.Orders;

public record OrderCreated(Guid OrderId, Guid CustomerId) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, "O pagamento do pedido expirou.");
}
