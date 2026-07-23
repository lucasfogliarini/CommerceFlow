var builder = WebApplication.CreateBuilder(args);

builder.AddApplication();

var app = builder.Build();

app.UseApplication();

await app.MigrateAndSeedAsync();

app.Run();
