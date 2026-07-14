namespace CommerceFlow.Orders;

public record PaymentApproved(Guid OrderId, string OrderNumber, string PaymentReference) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: OrderId, $"O pagamento do pedido {OrderNumber} foi aprovado.");
}
