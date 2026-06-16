var builder = Host.CreateApplicationBuilder(args);
builder.AddApplication();
var host = builder.Build();

await host.MigrateAndSeedAsync();

await host.RunAsync();
