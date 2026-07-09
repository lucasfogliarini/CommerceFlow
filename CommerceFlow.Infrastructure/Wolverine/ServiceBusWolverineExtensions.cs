using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Wolverine;
using Wolverine.AzureServiceBus;

namespace CommerceFlow.Infrastructure.ServiceBus;

public static class ServiceBusWolverineExtensions
{
    public static void UseServiceBus(this WolverineOptions options, IConfiguration configuration)
    {
        var serviceBusEndpoint = configuration.GetConnectionString("ServiceBusServer");

        options.UseAzureServiceBus(serviceBusEndpoint).AutoProvision();

        var transport = options.Transports.GetOrCreate<AzureServiceBusTransport>();

        transport.ManagementConnectionString = "Endpoint=sb://localhost:55469;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
    }

    public static WolverineOptions Subscribe<TMessage>(this WolverineOptions options, string subscription = "commerceflow")
    {
        var topic = ToTopic<TMessage>();

        options.ConfigurePublisher<TMessage>();//Must have it for AutoProvision the topic
        options.ListenToAzureServiceBusSubscription(subscription)
               .FromTopic(topic);
        return options;
    }
    public static WolverineOptions ConfigurePublisher<TMessage>(this WolverineOptions options)
    {
        var topic = ToTopic<TMessage>();
        options.PublishMessage<TMessage>()
            .ToAzureServiceBusTopic(topic);
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