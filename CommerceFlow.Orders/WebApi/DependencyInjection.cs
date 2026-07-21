using CommerceFlow.Application;
using CommerceFlow.Application.Notifications;
using CommerceFlow.Infrastructure.RabbitMQ;
using CommerceFlow.Orders;
using CommerceFlow.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
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
        builder.Services.AddSignalR();
        builder.AddCors();
        builder.ConfigureMessageBus(opts =>
        {
            opts.Subscribe<OrdersNotification>();
            opts.ConfigurePublisher<OrderCreated>();
            opts.ConfigurePublisher<ApprovePayment>();

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
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapEndpoints();
        app.MapControllers();
        app.MapHub<NotificationHub>("/hubs/notifications").RequireAuthorization();
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
         });
        builder.Services.AddAuthorization();
    }
    private static void AddCors(this IHostApplicationBuilder builder)
    {
        var corsSettings = builder.Configuration
            .GetSection(nameof(CorsConfiguration))
            .Get<CorsConfiguration>()
            ?? throw new InvalidOperationException(
                $"As configurações de CORS ({nameof(CorsConfiguration)}) não foram encontradas."
            );

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(corsSettings.AllowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });
    }
    public record CorsConfiguration(string[] AllowedOrigins);
    public record JwtConfiguration(string Authority, string Audience);

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