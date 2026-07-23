using CommerceFlow;
using CommerceFlow.Application;
using CommerceFlow.Application.Shipments;
using CommerceFlow.Customers;
using CommerceFlow.Infrastructure;
using CommerceFlow.Infrastructure.RabbitMQ;
using CommerceFlow.Infrastructure.Repositories;
using CommerceFlow.Infrastructure.Wolverine;
using CommerceFlow.Orders;
using CommerceFlow.Shipments;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddDbContext();        
        builder.Services.AddRepositories();
        builder.Services.AddTransient<ICarrierSelector, FakeCarrierSelector>();
        builder.AddOpenTelemetryExporter();
        builder.AddRateLimiter();
    }
    public static void ConfigureMessageBus(this IHostApplicationBuilder builder, Action<WolverineOptions>? configure)
    {
        var rabbitMqEndpoint = builder.Configuration.GetConnectionString("RabbitMQServer");

        builder.UseWolverine(opts =>
        {
            opts.UseRabbitMq(rabbitMqEndpoint).AutoProvision();

            opts.Subscribe<OrdersNotification>();
            opts.Subscribe<ShipmentsNotification>();

            opts.Subscribe<OrderCreated>();
            opts.Subscribe<OrderCancelled>();
            opts.Subscribe<OrderWaitingForPayment>();
            opts.Subscribe<ApprovePayment>();
            opts.Subscribe<RejectPayment>();
            opts.Subscribe<PaymentApproved>();
            opts.Subscribe<PaymentRejected>();
            opts.Subscribe<PaymentExpired>();
            opts.Subscribe<OrderReadyForShipment>();

            opts.Subscribe<ShipmentCreated>();
            opts.Subscribe<CarrierAssigned>();
            opts.Subscribe<CompletePacking>();
            opts.Subscribe<PackingCompleted>();
            opts.Subscribe<ShipmentDispatched>();
            opts.Subscribe<RegisterTrackingEvent>();
            opts.Subscribe<DeliverShipment>();
            opts.Subscribe<ShipmentDelivered>();

            configure?.Invoke(opts);

            opts.UseRuntimeCompilation();
            opts.CodeGeneration.AlwaysUseServiceLocationFor<CommerceFlowDbContext>();
        });
        builder.Services.AddScoped<IMessageDispatcher, WolverineMessageBus>();
        builder.Services
            .AddSingleton(sp =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(rabbitMqEndpoint),
                };
                var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                return connection;
            })
            .AddHealthChecks()
            .AddRabbitMQ();
    }
    public static void MapHealthChecks(this WebApplication app)
    {
        var serviceInfo = ServiceInfo.Get();
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("live")
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    serviceInfo.Name,
                    serviceInfo.Version,
                    app.Environment.EnvironmentName,
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                    })
                });

                await context.Response.WriteAsync(result);
            }
        });
    }
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IAddressRepository, AddressRepository>();
        services.AddTransient<IOrderRepository, OrderRepository>();
        services.AddTransient<IShipmentRepository, ShipmentRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<ICarrierRepository, CarrierRepository>();
    }
    public static async Task MigrateAndSeedAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CommerceFlowDbContext>();

        db.Database.Migrate();

        if (await db.Set<Product>().AnyAsync()) return;


        var products = CreateProducts();

        await db.AddRangeAsync(products);

        var c1 = new Carrier(new Guid("00000000-0000-0000-0000-000000000001"), "FedEx");
        var c2 = new Carrier(new Guid("00000000-0000-0000-0000-000000000002"), "UPS");

        await db.AddAsync(c1);
        await db.AddAsync(c2);
        await db.CommitAsync();
    }
    private static void AddDbContext(this IHostApplicationBuilder builder, string connectionStringKey = "CommerceFlowDb")
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringKey);
        void BuilderOptions(DbContextOptionsBuilder options)
        {
            if (connectionString is not null)
                options.UseNpgsql(connectionString);
            else
                options.UseInMemoryDatabase(connectionStringKey);

            // Use the following options only during development or troubleshooting
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }

        builder.Services.AddDbContext<CommerceFlowDbContext>(BuilderOptions);
        builder.Services.AddHealthChecks()
            .AddCheck<DbContextHealthCheck<CommerceFlowDbContext>>(nameof(CommerceFlowDbContext));
    }
    private static void AddOpenTelemetryExporter(this IHostApplicationBuilder builder)
    {
        var serviceInfo = ServiceInfo.Get();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(serviceInfo.Name, null, serviceInfo.Version))
            .WithTracing(tracerBuilder =>
            {
                tracerBuilder
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter();
            })
            .WithMetrics(meterBuilder =>
            {
                meterBuilder
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter();
            })
            .WithLogging(loggingBuilder =>
            {
                loggingBuilder
                    .AddOtlpExporter();
            });

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
        });
    }
    private static void AddRateLimiter(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy("per-user", context =>
            {
                var key = context.User.FindFirstValue("sid") ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
                return RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: key,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 100,
                        TokensPerPeriod = 50,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(30)
                    });
            });
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;
                await context.HttpContext.Response.WriteAsync("Limite atingido, tente novamente em breve.", token);
            };
        });
    }

    private static Product[] CreateProducts()
    {
        var products = new[]
        {
            Product.Create(new Guid("00000000-0000-0000-0000-000000000001"), "iphone-16-pro", "iPhone 16 Pro", "Smartphone Apple iPhone 16 Pro.", 8999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000002"), "iphone-16", "iPhone 16", "Smartphone Apple iPhone 16.", 6999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000003"), "galaxy-s25-ultra", "Galaxy S25 Ultra", "Smartphone Samsung Galaxy S25 Ultra.", 7999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000004"), "galaxy-s25", "Galaxy S25", "Smartphone Samsung Galaxy S25.", 5999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000005"), "pixel-9-pro", "Pixel 9 Pro", "Smartphone Google Pixel 9 Pro.", 6499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000006"), "macbook-air-m4", "MacBook Air M4", "Notebook Apple MacBook Air M4.", 9999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000007"), "macbook-pro-m4", "MacBook Pro M4", "Notebook Apple MacBook Pro M4.", 14999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000008"), "dell-xps-13", "Dell XPS 13", "Notebook Dell XPS 13.", 8999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000009"), "lenovo-legion-5", "Lenovo Legion 5", "Notebook gamer Lenovo Legion 5.", 7499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000010"), "asus-rog-zephyrus-g16", "ASUS ROG Zephyrus G16", "Notebook gamer ASUS ROG Zephyrus G16.", 12999.90m),
        };

        return products;
    }
}
