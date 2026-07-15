
using CommerceFlow.Orders;

namespace CommerceFlow.Shipments;

public sealed record ShipmentDelivered(Guid ShipmentId, string OrderNumber) : DomainEvent
{
    public override INotification[] Notifications => [
            new ShipmentsNotification(ShipmentId: ShipmentId, $"O envio do pedido {OrderNumber} foi entregue com sucesso."),
            new OrdersNotification(OrderId: ShipmentId, $"O envio do pedido {OrderNumber} foi entregue com sucesso.")
        ];
}
