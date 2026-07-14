using CommerceFlow.Application;
using CommerceFlow.Application.Shipments;
using CommerceFlow.Infrastructure.RabbitMQ;
using CommerceFlow.Orders;
using CommerceFlow.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json.Serialization;
using Wolverine;

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
        });
        builder.AddJwtBearerAuthentication();
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
        app.UseAuthentication();
        app.UseAuthorization();
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
            endpoint
                .MapEndpoint(app)
                .RequireAuthorization();
        }

        return app;
    }
    private static void AddJwtBearerAuthentication(this IHostApplicationBuilder builder)
    {
        var jwtConfiguration = GetConfiguration<JwtConfiguration>(builder.Configuration);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
         .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
         {
             options.Authority = jwtConfiguration.Authority;
             options.Audience = jwtConfiguration.Audience;
             options.RequireHttpsMetadata = false;
             options.SaveToken = true;
             options.Events.OnTokenValidated = async (tokenValidatedContext) =>
             {
                 var bus = tokenValidatedContext.HttpContext.RequestServices.GetRequiredService<IMessageBus>();
                 var createCustomer = new GetOrCreateCustomer(tokenValidatedContext.Principal);
                 await bus.InvokeAsync<Guid>(createCustomer);
             };
         });
        builder.Services.AddAuthorization();
    }
    record JwtConfiguration(string Authority, string Audience);

    static TConfiguration GetConfiguration<TConfiguration>(IConfiguration configuration)
    {
        var sectionName = typeof(TConfiguration).Name;
        var section = configuration.GetRequiredSection(sectionName);
        var config = section.Get<TConfiguration>();
        return config is null
            ? throw new InvalidOperationException($"Configuration section '{sectionName}' is missing or invalid.")
            : config;
    }
}