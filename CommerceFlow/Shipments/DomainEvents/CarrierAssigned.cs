
namespace CommerceFlow.Shipments;

public sealed record CarrierAssigned(Guid ShipmentId, string OrderNumber, string Carrier) : DomainEvent
{
    public override INotification[] Notifications => [new ShipmentsNotification(ShipmentId: ShipmentId, $"A transportadora {Carrier} foi atribuída ao envio do pedido {OrderNumber}.")];
}