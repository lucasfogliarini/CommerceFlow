using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CommerceFlow.Application.Notifications;

public sealed class SubUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
