
using CommerceFlow.Orders;

namespace CommerceFlow.Shipments;

public sealed record TrackingUpdated(Guid ShipmentId, string OrderNumber, string Description, string Location) : DomainEvent
{
    public override INotification[] Notifications => [
        new ShipmentsNotification(ShipmentId: ShipmentId, $"Rastreamento do pedido {OrderNumber}: {Description}, Localização: {Location}."),
        new OrdersNotification(OrderId: ShipmentId, $"Rastreamento do pedido {OrderNumber}: {Description}, Localização: {Location}.")
];
}
