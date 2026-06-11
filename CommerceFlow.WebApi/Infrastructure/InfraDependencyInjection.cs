using CommerceFlow;
using CommerceFlow.WebApi.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfraDependencyInjection
{
    public static void AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
        builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
    }

    public static async Task SeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var inventoryRepo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();
        var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();

        var p1 = Product.Create(Guid.NewGuid(), "Keyboard", 50.0m);
        var p2 = Product.Create(Guid.NewGuid(), "Mouse", 25.0m);
        var p3 = Product.Create(Guid.NewGuid(), "Monitor", 300.0m);

        await productRepo.AddAsync(p1);
        await productRepo.AddAsync(p2);
        await productRepo.AddAsync(p3);

        var i1 = Inventory.Create(p1.Id, 10);
        var i2 = Inventory.Create(p2.Id, 20);
        var i3 = Inventory.Create(p3.Id, 5);

        await inventoryRepo.AddAsync(i1);
        await inventoryRepo.AddAsync(i2);
        await inventoryRepo.AddAsync(i3);
    }
}