
namespace CommerceFlow.Shipments;

public sealed record ShipmentDispatched(Guid ShipmentId, string OrderNumber, string TrackingCode) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"O pedido {OrderNumber} foi despachado com o código de rastreamento {TrackingCode}.");
}
