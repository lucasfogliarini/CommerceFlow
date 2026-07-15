
namespace CommerceFlow.Shipments;
public sealed record ShipmentCancelled(Guid ShipmentId, string OrderNumber) : DomainEvent
{
    public override INotification[] Notifications => [new ShipmentsNotification(ShipmentId: ShipmentId, $"O envio do pedido {OrderNumber} foi cancelado.")];
}
