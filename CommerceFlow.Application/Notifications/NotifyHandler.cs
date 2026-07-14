using Microsoft.AspNetCore.SignalR;

namespace CommerceFlow.Application.Notifications;

public sealed class NotifyHandler(IHubContext<NotificationHub> notificationHub)
{
    public async Task HandleAsync(NotificationRequest notificationRequest, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notificationRequest);
        await notificationHub.Clients.All.SendAsync("ReceiveNotification", notificationRequest, cancellationToken);
    }
}
