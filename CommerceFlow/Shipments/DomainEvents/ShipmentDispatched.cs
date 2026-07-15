
using CommerceFlow.Orders;

namespace CommerceFlow.Shipments;

public sealed record ShipmentDispatched(Guid ShipmentId, string OrderNumber, string TrackingCode) : DomainEvent
{
    public override INotification[] Notifications => [
            new ShipmentsNotification(ShipmentId: ShipmentId, $"O pedido {OrderNumber} foi despachado com o código de rastreamento {TrackingCode}."),
            new OrdersNotification(OrderNumber: OrderNumber, $"O pedido {OrderNumber} foi despachado com o código de rastreamento {TrackingCode}.")
        ];
}
