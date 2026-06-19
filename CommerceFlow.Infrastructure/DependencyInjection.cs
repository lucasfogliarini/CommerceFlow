using CommerceFlow;
using CommerceFlow.Infrastructure;
using CommerceFlow.Infrastructure.Repositories;
using CommerceFlow.Orders;
using CommerceFlow.Shipments;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Kafka;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddDbContext();        
        builder.Services.AddRepositories();
        builder.AddOpenTelemetryExporter();
        builder.AddRateLimiter();
    }
    public static void ConfigureMessageBus(this IHostApplicationBuilder builder, Action<WolverineOptions>? configure)
    {
        builder.UseWolverine(opts =>
        {
            opts.Policies
                .OnException<Exception>()
                .RetryWithCooldown(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3));

            var kafkaEndpoint = builder.Configuration.GetConnectionString("KafkaServer");
            opts.UseKafka(kafkaEndpoint).AutoProvision();

            configure?.Invoke(opts);

            opts.UseRuntimeCompilation();
            opts.CodeGeneration.AlwaysUseServiceLocationFor<CommerceFlowDbContext>();
        });
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

        var p1 = Product.Create(new Guid("00000000-0000-0000-0000-000000000001"), "Keyboard", 50.0m, 99999999);
        var p2 = Product.Create(new Guid("00000000-0000-0000-0000-000000000002"), "Mouse", 25.0m, 99999999);
        var p3 = Product.Create(new Guid("00000000-0000-0000-0000-000000000003"), "Monitor", 3m, 99999999);

        await db.AddAsync(p1);
        await db.AddAsync(p2);
        await db.AddAsync(p3);

        var c1 = new Carrier(new Guid("00000000-0000-0000-0000-000000000001"), "FedEx");
        var c2 = new Carrier(new Guid("00000000-0000-0000-0000-000000000002"), "UPS");

        await db.AddAsync(c1);
        await db.AddAsync(c2);
        await db.CommitAsync();
    }
    private static void AddDbContext(this IHostApplicationBuilder builder, string connectionStringKey = "CommerceFlow")
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringKey);
        void BuilderOptions(DbContextOptionsBuilder options)
        {
            if (connectionString is not null)
                options.UseNpgsql(connectionString);
            else
            {
                var connection = new SqliteConnection("Data Source=:memory:");
                connection.Open();
                builder.Services.AddSingleton(connection);
                options.UseSqlite(connection);
            }

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
}
