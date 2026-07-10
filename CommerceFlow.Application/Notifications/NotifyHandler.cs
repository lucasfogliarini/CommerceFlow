namespace CommerceFlow.Application.Notifications;

public sealed class NotifyHandler()
{
    public async Task HandleAsync(NotificationRequest notificationRequest, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notificationRequest);
    }
}
