
namespace CommerceFlow.Shipments;

public sealed record TrackingUpdated(Guid ShipmentId, Guid OrderId, string Description, string Location) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"O rastreamento do envio do pedido {OrderId} foi atualizado. Descrição: {Description}, Localização: {Location}.");
}
