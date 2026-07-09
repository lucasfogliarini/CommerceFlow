using CommerceFlow.Application;
using Wolverine;

namespace CommerceFlow.Infrastructure.Wolverine;

public sealed class WolverineMessageBus(IMessageBus bus) : IMessageDispatcher
{
    public async ValueTask SendAsync<T>(T message) => await bus.SendAsync(message);

    public async ValueTask PublishAsync<T>(T message) => await bus.PublishAsync(message);

    public async ValueTask ScheduleAsync<T>(T message, TimeSpan delay) => await bus.ScheduleAsync(message, delay);
}