
namespace CommerceFlow.Shipments;

public sealed record ShipmentCreated(Guid ShipmentId, string OrderNumber) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"O envio do pedido {OrderNumber} foi criado.");
}
