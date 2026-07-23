using Aspire.C4;

[WithReference<PostgresDatabaseResource>]
public class WebApi : Service
{
    public override string Name => nameof(WebApi);

    public override void Configure(SoftwareSystemContextBuilder system)
    {
        var webApiResourceBuilder = system.Builder
                                           .AddProject<Projects.CommerceFlow_WebApi>(Name)
                                           .WithHttpEndpoint(system.GetNextPort());
        system.AddResourceBuilder(webApiResourceBuilder);
    }
}