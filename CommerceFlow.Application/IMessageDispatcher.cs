namespace CommerceFlow.Application;

public interface IMessageDispatcher
{
    ValueTask SendAsync<T>(T message);
    ValueTask PublishAsync<T>(T message);
    ValueTask ScheduleAsync<T>(T message, TimeSpan delay);
}
