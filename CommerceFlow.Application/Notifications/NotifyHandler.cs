using CommerceFlow.Orders;
using CommerceFlow.Shipments;
using Microsoft.AspNetCore.SignalR;

namespace CommerceFlow.Application.Notifications;

public sealed class NotifyHandler(IOrderRepository orderRepository, IHubContext<NotificationHub> notificationHub)
{
    public async Task HandleAsync(OrdersNotification notification, CancellationToken cancellationToken = default)
    {
        var customerId = await orderRepository.GetCustomerIdAsync(notification.OrderNumber, cancellationToken);
        ArgumentNullException.ThrowIfNull(customerId);
        await notificationHub.Clients.User(customerId.ToString()).SendAsync("ReceiveNotification", notification, cancellationToken);
    }

    public async Task HandleAsync(ShipmentsNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        await notificationHub.Clients.All.SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}
