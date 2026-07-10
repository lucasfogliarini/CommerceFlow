namespace CommerceFlow.Orders;

public record OrderCancelled(Guid OrderId, string Reason) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, $"O pedido {OrderId} foi cancelado. Motivo: {Reason}.");
}
