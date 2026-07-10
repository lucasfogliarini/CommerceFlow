
namespace CommerceFlow.Notifications;

public class Notification : AggregateRoot
{
    public Guid AggregateId { get; set; }
    public required string Message { get; set; }
}
