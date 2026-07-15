
namespace CommerceFlow.Shipments;

public sealed record ShipmentCreated(Guid ShipmentId, string OrderNumber) : DomainEvent
{
    public override INotification[] Notifications => [new ShipmentsNotification(ShipmentId: ShipmentId, $"O envio do pedido {OrderNumber} foi criado.")];
}
