using CommerceFlow;
using CommerceFlow.Application;
using CommerceFlow.Customers;
using CommerceFlow.Infrastructure;
using CommerceFlow.Infrastructure.RabbitMQ;
using CommerceFlow.Infrastructure.Repositories;
using CommerceFlow.Infrastructure.Wolverine;
using CommerceFlow.Orders;
using CommerceFlow.Shipments;
using Humanizer.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        builder.AddOpenTelemetryExporter();
        builder.AddRateLimiter();
    }
    public static void ConfigureMessageBus(this IHostApplicationBuilder builder, Action<WolverineOptions>? configure)
    {
        var rabbitMqEndpoint = builder.Configuration.GetConnectionString("RabbitMQServer");

        builder.UseWolverine(opts =>
        {
            opts.UseRabbitMq(rabbitMqEndpoint).AutoProvision();

            opts.Subscribe<NotificationRequest>();

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

        var customer = Customer.Create("lucasfogliarini@gmail.com", "Lucas Fogliarini");
        await db.AddAsync(customer);

        var products = CreateProducts();

        await db.AddRangeAsync(products);

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
            Product.Create(new Guid("00000000-0000-0000-0000-000000000011"), "ipad-pro-13", "iPad Pro 13", "Tablet Apple iPad Pro 13.", 10999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000012"), "ipad-air", "iPad Air", "Tablet Apple iPad Air.", 5999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000013"), "galaxy-tab-s10", "Galaxy Tab S10", "Tablet Samsung Galaxy Tab S10.", 5299.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000014"), "kindle-paperwhite", "Kindle Paperwhite", "Leitor digital Kindle Paperwhite.", 799.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000015"), "apple-watch-series-10", "Apple Watch Series 10", "Smartwatch Apple Watch Series 10.", 4299.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000016"), "galaxy-watch-7", "Galaxy Watch 7", "Smartwatch Samsung Galaxy Watch 7.", 2499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000017"), "garmin-fenix-8", "Garmin Fenix 8", "Smartwatch Garmin Fenix 8.", 6999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000018"), "airpods-pro-2", "AirPods Pro 2", "Fones Apple AirPods Pro 2.", 1999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000019"), "sony-wh-1000xm6", "Sony WH-1000XM6", "Headphone Sony WH-1000XM6.", 2899.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000020"), "bose-quietcomfort-ultra", "Bose QuietComfort Ultra", "Headphone Bose QuietComfort Ultra.", 3199.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000021"), "jbl-flip-7", "JBL Flip 7", "Caixa de som JBL Flip 7.", 899.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000022"), "echo-dot-5", "Echo Dot 5", "Smart speaker Amazon Echo Dot 5.", 349.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000023"), "google-nest-audio", "Google Nest Audio", "Smart speaker Google Nest Audio.", 699.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000024"), "playstation-5-slim", "PlayStation 5 Slim", "Console Sony PlayStation 5 Slim.", 3799.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000025"), "xbox-series-x", "Xbox Series X", "Console Microsoft Xbox Series X.", 4299.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000026"), "nintendo-switch-2", "Nintendo Switch 2", "Console Nintendo Switch 2.", 4499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000027"), "dualsense", "Controle DualSense", "Controle sem fio DualSense.", 499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000028"), "xbox-wireless-controller", "Controle Xbox Wireless", "Controle sem fio Xbox.", 459.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000029"), "logitech-g502-x", "Mouse Logitech G502 X", "Mouse gamer Logitech G502 X.", 449.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000030"), "logitech-mx-master-3s", "Mouse Logitech MX Master 3S", "Mouse Logitech MX Master 3S.", 699.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000031"), "keychron-k8-pro", "Teclado Keychron K8 Pro", "Teclado mecânico Keychron K8 Pro.", 899.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000032"), "logitech-mx-keys-s", "Teclado Logitech MX Keys S", "Teclado Logitech MX Keys S.", 849.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000033"), "lg-oled-c5-55", "TV LG OLED C5 55", "Smart TV LG OLED C5 55 polegadas.", 6499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000034"), "samsung-qn90f-65", "TV Samsung QN90F 65", "Smart TV Samsung Neo QLED 65.", 8499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000035"), "philips-hue-starter-kit", "Philips Hue Starter Kit", "Kit de iluminação Philips Hue.", 1299.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000036"), "roborock-s8-maxv", "Roborock S8 MaxV", "Robô aspirador Roborock S8 MaxV.", 5999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000037"), "dyson-v15-detect", "Dyson V15 Detect", "Aspirador vertical Dyson V15.", 5299.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000038"), "nespresso-vertuo", "Nespresso Vertuo", "Cafeteira Nespresso Vertuo.", 999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000039"), "air-fryer-philips-xl", "Air Fryer Philips XL", "Fritadeira sem óleo Philips XL.", 899.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000040"), "instant-pot-duo", "Instant Pot Duo", "Panela elétrica Instant Pot Duo.", 799.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000041"), "cadeira-herman-miller-aeron", "Cadeira Herman Miller Aeron", "Cadeira ergonômica Herman Miller Aeron.", 12999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000042"), "mesa-eletrica-flexispot", "Mesa Elétrica FlexiSpot", "Mesa com ajuste de altura.", 2499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000043"), "monitor-lg-ultrafine-32", "Monitor LG UltraFine 32", "Monitor 4K LG UltraFine 32.", 3599.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000044"), "monitor-dell-u2725qe", "Monitor Dell U2725QE", "Monitor Dell UltraSharp 27 4K.", 4299.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000045"), "ssd-samsung-990-pro-2tb", "SSD Samsung 990 Pro 2TB", "SSD NVMe Samsung 990 Pro 2TB.", 1599.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000046"), "wd-black-sn850x-2tb", "WD Black SN850X 2TB", "SSD WD Black SN850X 2TB.", 1499.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000047"), "rtx-5090", "GeForce RTX 5090", "Placa de vídeo NVIDIA GeForce RTX 5090.", 18999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000048"), "ryzen-9-9950x3d", "Ryzen 9 9950X3D", "Processador AMD Ryzen 9 9950X3D.", 4999.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000049"), "intel-core-ultra-9-285k", "Intel Core Ultra 9 285K", "Processador Intel Core Ultra 9 285K.", 4599.90m),
            Product.Create(new Guid("00000000-0000-0000-0000-000000000050"), "steam-deck-oled", "Steam Deck OLED", "Console portátil Steam Deck OLED.", 4499.90m)
        };

        return products;
    }
}
