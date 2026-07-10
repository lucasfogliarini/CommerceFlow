namespace CommerceFlow.Orders;

public record OrderWaitingForPayment(Guid OrderId) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, "O pedido está aguardando pagamento.");
}
