using System.Text.RegularExpressions;
using Wolverine;
using Wolverine.Kafka;

namespace CommerceFlow.Infrastructure;
public static class WolverineOptionsExtensions
{
    public static WolverineOptions Subscribe<TMessage>(this WolverineOptions options, string groupId = "commerceflow")
    {
        var topic = ToTopic<TMessage>();

        options.ConfigurePublisher<TMessage>();//Must have it for AutoProvision the topic
        options.ListenToKafkaTopic(topic)            
            .DefaultIncomingMessage<TMessage>()
            .WithGroupId(groupId);
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
