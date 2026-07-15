using System.Text.RegularExpressions;
using Wolverine;
using Wolverine.RabbitMQ;

namespace CommerceFlow.Infrastructure.RabbitMQ;

public static class RabbitMQWolverineExtensions
{
    public static WolverineOptions Subscribe<TMessage>(this WolverineOptions options)
    {
        var queue = ToQueue<TMessage>();

        options.ConfigurePublisher<TMessage>();//Must have it for AutoProvision the topic
        options.ListenToRabbitQueue(queue);
        return options;
    }
    public static WolverineOptions ConfigurePublisher<TMessage>(this WolverineOptions options)
    {
        var queue = ToQueue<TMessage>();
        options.PublishMessage<TMessage>()
               .ToRabbitQueue(queue);
        return options;
    }
    public static WolverineOptions ConfigureExchangePublisher<TMessage>(this WolverineOptions options, params string[] queues)
    {
        var exchange = ToQueue<TMessage>();
        options.ConfigureRabbitMq()
         .DeclareExchange(exchange, ex =>
         {
             ex.ExchangeType = ExchangeType.Fanout;

             foreach (var queue in queues)
                 ex.BindQueue(queue);
         });

        options.PublishMessage<TMessage>()
            .ToRabbitExchange(exchange);

        return options;
    }

    private static string ToQueue<TMessage>()
    {
        var type = typeof(TMessage);
        var topic = Regex
            .Replace(type.Name, "([a-z0-9])([A-Z])", "$1-$2")
            .ToLowerInvariant();
        return topic;
    }
}