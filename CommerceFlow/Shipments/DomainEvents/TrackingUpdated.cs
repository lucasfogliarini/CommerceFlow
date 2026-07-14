
namespace CommerceFlow.Shipments;

public sealed record TrackingUpdated(Guid ShipmentId, string OrderNumber, string Description, string Location) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"Rastreamento do pedido {OrderNumber}: {Description}, Localização: {Location}.");
}
