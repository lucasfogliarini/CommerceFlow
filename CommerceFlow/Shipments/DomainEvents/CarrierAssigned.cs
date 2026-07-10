
namespace CommerceFlow.Shipments;

public sealed record CarrierAssigned(Guid ShipmentId, Guid OrderId, string Carrier) : DomainEvent
{
    public override NotificationRequest Notification => new(AggregateId: ShipmentId, $"O transportador {Carrier} foi atribuído ao envio do pedido {OrderId}.");
}