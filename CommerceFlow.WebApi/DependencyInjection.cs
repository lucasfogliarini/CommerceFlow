using CommerceFlow;
using CommerceFlow.Application;
using CommerceFlow.Infrastructure;
using CommerceFlow.WebApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine;
using Wolverine.Kafka;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebApi(this WebApplicationBuilder builder)
    {
        builder.AddInfrastructure();
        builder.Services.AddEndpoints();
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();
        builder.Host.UseWolverine(opts =>
        {
            var kafkaEndpoint = builder.Configuration.GetConnectionString("KafkaServer");
            opts.UseKafka(kafkaEndpoint);

            ConfigurePubSub<OrderCreated>(opts, "order-created");
            ConfigurePubSub<OrderInventoryReserved>(opts, "order-inventory-reserved");
            opts.PublishMessage<OrderWaitingForPayment>().ToKafkaTopic("order-waiting-for-payment");            
            ConfigurePubSub<PaymentApproved>(opts, "order-payment-approved");
            opts.PublishMessage<OrderShipped>().ToKafkaTopic("order-shipped");
            opts.PublishMessage<OrderDelivered>().ToKafkaTopic("order-delivered");
            opts.PublishMessage<OrderCancelled>().ToKafkaTopic("order-cancelled");

            opts.UseRuntimeCompilation();
            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
            opts.CodeGeneration.AlwaysUseServiceLocationFor<CommerceFlowDbContext>();
        });
    }

    public static void ConfigurePubSub<TMessage>(WolverineOptions options, string topic)
    {
        options.PublishMessage<TMessage>()
                .ToKafkaTopic(topic);

        options.ListenToKafkaTopic(topic)
            .DefaultIncomingMessage<TMessage>();
    }

    public static void UseWebApi(this WebApplication app)
    {
        app.MapEndpoints();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointTypes = typeof(Program).Assembly
            .DefinedTypes
            .Where(type => !type.IsAbstract
                           && !type.IsInterface
                           && typeof(IEndpoint).IsAssignableFrom(type))
            .Select(type => ServiceDescriptor.Scoped(typeof(IEndpoint), type));

        services.TryAddEnumerable(endpointTypes);

        return services;
    }
    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        IEnumerable<IEndpoint> endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}