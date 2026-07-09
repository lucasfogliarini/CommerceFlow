using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Wolverine;
using Wolverine.RabbitMQ;

namespace CommerceFlow.Infrastructure.RabbitMQ;

public static class RabbitMQWolverineExtensions
{
    public static void UseRabbitMQ(this WolverineOptions options, IConfiguration configuration)
    {
        var rabbitMqEndpoint = configuration.GetConnectionString("RabbitMQServer");

        options.UseRabbitMq(rabbitMqEndpoint).AutoProvision();
    }

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

    private static string ToQueue<TMessage>()
    {
        var type = typeof(TMessage);
        var topic = Regex
            .Replace(type.Name, "([a-z0-9])([A-Z])", "$1-$2")
            .ToLowerInvariant();
        return topic;
    }
}