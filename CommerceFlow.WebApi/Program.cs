var builder = WebApplication.CreateBuilder(args);

builder.AddWebApi();

var app = builder.Build();

await app.SeedAsync();

app.UseWebApi();

app.Run();
