
namespace CommerceFlow.Shipments;

public sealed record ShipmentCreated(Guid ShipmentId, Guid OrderId) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"O envio do pedido {OrderId} foi criado.");
}
