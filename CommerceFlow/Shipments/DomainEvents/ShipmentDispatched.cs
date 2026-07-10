
namespace CommerceFlow.Shipments;

public sealed record ShipmentDispatched(Guid ShipmentId, Guid OrderId, string TrackingCode) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"O envio do pedido {OrderId} foi despachado com o código de rastreamento {TrackingCode}.");
}
