
namespace CommerceFlow.Shipments;
public sealed record ShipmentCancelled(Guid ShipmentId, string OrderNumber) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"O envio do pedido {OrderNumber} foi cancelado.");
}
