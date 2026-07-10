using CommerceFlow.Application;
using CommerceFlow.Application.Shipments;
using CommerceFlow.Infrastructure.RabbitMQ;
using CommerceFlow.Orders;
using CommerceFlow.WebApi;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.AddInfrastructure();
        builder.Services.AddEndpoints();
        builder.Services.AddControllers().AddOData(opt =>
        {
            opt.EnableQueryFeatures(50);
            opt.AddRouteComponentsUsingODataControllers();
        }); ;
        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();

        builder.ConfigureMessageBus(opts =>
        {
            opts.ConfigurePublisher<OrderCreated>();
            opts.ConfigurePublisher<ApprovePayment>();
            opts.ConfigurePublisher<CompletePacking>();
            opts.ConfigurePublisher<DeliverShipment>();
            opts.ConfigurePublisher<RegisterTrackingEvent>();

            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
        });
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(
                new JsonStringEnumConverter());
        });
    }
    public static void UseApplication(this WebApplication app)
    {
        app.MapEndpoints();
        app.MapControllers();
        app.MapHealthChecks();
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