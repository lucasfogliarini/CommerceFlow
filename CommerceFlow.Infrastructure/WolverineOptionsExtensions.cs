using System.Text.RegularExpressions;
using Wolverine;
using Wolverine.Kafka;

namespace CommerceFlow.Infrastructure;
public static class WolverineOptionsExtensions
{
    public static WolverineOptions Subscribe<TMessage>(this WolverineOptions options)
    {
        var topic = ToTopic<TMessage>();

        options.ListenToKafkaTopic(topic)
            .DefaultIncomingMessage<TMessage>();
        return options;
    }
    public static WolverineOptions ConfigurePublisher<TMessage>(this WolverineOptions options)
    {
        var topic = ToTopic<TMessage>();
        options.PublishMessage<TMessage>()
            .ToKafkaTopic(topic);
        return options;
    }
    
    private static string ToTopic<TMessage>()
    {
        var type = typeof(TMessage);
        var topic = Regex
            .Replace(type.Name, "([a-z0-9])([A-Z])", "$1-$2")
            .ToLowerInvariant();
        return topic;
    }
}
