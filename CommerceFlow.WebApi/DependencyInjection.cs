using CommerceFlow.Application;
using CommerceFlow.WebApi;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine;

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
            opts.UseRuntimeCompilation();
            opts.Discovery.IncludeAssembly(typeof(CreateOrderHandler).Assembly);
        });
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