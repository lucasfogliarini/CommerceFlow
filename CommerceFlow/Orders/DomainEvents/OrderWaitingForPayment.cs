namespace CommerceFlow.Orders;

public record OrderWaitingForPayment(Guid OrderId, string OrderNumber) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, $"O pedido {OrderNumber} está aguardando pagamento.");
}
