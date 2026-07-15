using CommerceFlow.Orders;
using CommerceFlow.Shipments;
using Microsoft.AspNetCore.SignalR;

namespace CommerceFlow.Application.Notifications;

public sealed class NotifyHandler(IHubContext<NotificationHub> notificationHub)
{
    public async Task HandleAsync(OrdersNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        await notificationHub.Clients.All.SendAsync("ReceiveNotification", notification, cancellationToken);
    }

    public async Task HandleAsync(ShipmentsNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        await notificationHub.Clients.All.SendAsync("ReceiveNotification", notification, cancellationToken);
    }
}
