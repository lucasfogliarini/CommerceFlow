namespace Aspire.C4;

using Aspire.Hosting.ApplicationModel;
using System.Reflection;


/// <summary>
/// Represents a system context as defined by the C4 Model.
/// https://c4model.com/abstractions/system-context
///
/// Architectural diagrams for this system should be maintained using a C4-compliant tool.
/// IcePanel is recommended for creating and maintaining these diagrams: https://icepanel.io/
/// </summary>
public abstract class SoftwareSystemContextBuilder(IDistributedApplicationBuilder builder)
{
    public const string GithubDomainUrl = "https://github.com";
    public const string HostDefault = "127.0.0.1";//localhost

    /// <summary>
    /// The Author or Organization of the repositories for the domains and software systems (e.g. lucasfogliarini)
    /// </summary>
    protected abstract string RepositoriesAuthor { get; init; }
    /// <summary>
    /// The URL of the repositories for the domains and software systems (e.g. https://github.com, https://gitlab.com)
    /// </summary>
    protected abstract string RepositoriesDomainUrl { get; init; }
    /// <summary>
    /// The repository URL for this domain or software system. (<see cref="RepositoryUniformResourceLocator"/> + <see cref="RepositoriesAuthor"/> + <see cref="Name"/>)
    /// </summary>
    protected string RepositoryUniformResourceLocator { get { return $"{RepositoriesDomainUrl}/{RepositoriesAuthor}/{Name}"; } }
    /// <summary>
    /// A Top-Level Domain (TLD) is the final part of a domain name, appearing after the last dot (e.g., .earth, .org, .br, .com)
    /// </summary>
    protected abstract string TopDomainLevel { get; init; }
    /// <summary>
    /// The registered name under a TLD that identifies an organization or entity. (e.g., bora.earth, bora.org, bora.br, bora.com)
    /// </summary>
    protected abstract string Domain { get; init; }
    /// <summary>
    /// A subdivision of a domain, used to organize or separate services. (e.g., app.bora.earth, api.bora.earth, morar.bora.earth, bank.bora.earth)
    /// </summary>
    protected virtual string? Subdomain { get; init; }
    /// <summary>
    /// FQDN – Fully Qualified Domain Name
    /// The complete, absolute domain name that specifies an exact location in the DNS hierarchy, including all subdomains, the domain, and the TLD.
    /// </summary>
    protected string FullyQualifiedDomainName { get { return $"{Subdomain}.{Domain}.{TopDomainLevel}"; }  }

    /// <summary>
    /// The Domain Url
    /// A standardized address that specifies how to access a resource (scheme/protocol) and where it is located on the Internet (domain), optionally including path, query parameters, and fragment identifiers.
    /// 
    /// Must be https!
    /// </summary>
    protected string DomainUniformResourceLocator { get { return $"https://{Subdomain}.{Domain}.{TopDomainLevel}"; } }
    /// <summary>
    /// The name of the domain or software system.
    /// </summary>
    protected virtual string Name { get { return FullyQualifiedDomainName.Replace(".","-").TrimStart('-'); } }

    /// <summary>
    /// Represents a system context as defined by the C4 Model.
    /// https://c4model.com/diagrams/system-context
    /// </summary>
    protected abstract string SystemContextDiagramUrl { get; init; }    
    public IDistributedApplicationBuilder Builder { get; init; } = builder;
    public int CurrentPort { get; set; } = 2000;
    public int GetNextPort() => CurrentPort++;

    /// <summary>
    /// Represents a collection of container resource builders as defined by the C4 Model.
    /// https://c4model.com/abstractions/container
    /// </summary>

    public IList<Service> Services { get; init; } = [];
    public IList<IResourceBuilder<IResource>> ResourceBuilders { get; init; } = [];
    public IResourceBuilder<ExternalServiceResource>? SystemResourceBuilder { get; private set; }
    public DistributedApplication Build()
    {
        SystemResourceBuilder = Builder.AddExternalService(Name, SystemContextDiagramUrl);
        if (RepositoryUniformResourceLocator is not null)
            SystemResourceBuilder.WithUrl(RepositoryUniformResourceLocator);
        if (DomainUniformResourceLocator is not null)
            SystemResourceBuilder.WithUrl(DomainUniformResourceLocator);

        Builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";
        Builder.Configuration["ASPNETCORE_URLS"] = $"http://+:{GetNextPort()}";
        Builder.Configuration["ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL"] = $"http://+:{GetNextPort()}";

        ConfigureServices();

        ConfigureDependencies();
         
        return Builder.Build();
    }
    public void AddService<TService>() where TService : Service
    {
        var service = Activator.CreateInstance<TService>()!;
        Services.Add(service);
    }
    public void AddResourceBuilder(IResourceBuilder<IResource> resourceBuilder)
    {
        SystemResourceBuilder.WithChildRelationship(resourceBuilder);
        ResourceBuilders.Add(resourceBuilder);
    }
    public EndpointReference? GetEndpoint<TService>() where TService : Service
    {
        var resourceBuilder = ResourceBuilders.FirstOrDefault(e => e.Resource.Name == typeof(TService).Name) as IResourceBuilder<IResourceWithEndpoints>;
        ArgumentNullException.ThrowIfNull(resourceBuilder, $"Resource builder for service '{typeof(TService).Name}' not found or does not implement IResourceWithEndpoints.");
        return resourceBuilder?.GetEndpoint("http");
    }
    public IResourceBuilder<TResource>? GetResourceBuilder<TResource>(string name) where TResource : IResource
    {
        return ResourceBuilders.FirstOrDefault(e => e.Resource.Name == name) as IResourceBuilder<TResource>;
    }
    public IResourceBuilder<TResource> GetResourceBuilder<TResource>() where TResource : IResource
    {
        var type = typeof(TResource);
        return GetResourceBuilder<TResource>(type)!;
    }
    public IResourceBuilder<TResource>? GetResourceBuilder<TResource>(Type type) where TResource : IResource
    {
        return ResourceBuilders.FirstOrDefault(e => type.IsAssignableFrom(e.Resource.GetType())) as IResourceBuilder<TResource>;
    }
    public static TSystemContextBuilder CreateBuilder<TSystemContextBuilder>() where TSystemContextBuilder : SoftwareSystemContextBuilder
    {
        var builder = DistributedApplication.CreateBuilder();
        var systemContextBuilder = (TSystemContextBuilder)Activator.CreateInstance(typeof(TSystemContextBuilder), builder)!;
        return systemContextBuilder;
    }
    private void ConfigureServices()
    {
        foreach (var service in Services)
        {
            service.Configure(this);
        }
    }
    private void ConfigureDependencies()
    {
        foreach (var service in Services)
        {
            var serviceResourceBuilderWithWaitSupport = GetResourceBuilder<IResourceWithWaitSupport>(service.Name);
            var serviceResourceBuilderWithEnvironment = GetResourceBuilder<IResourceWithEnvironment>(service.Name);

            var references = service.GetType()
                                .GetCustomAttributes()
                                .Where(a =>
                                    a.GetType().IsGenericType &&
                                    a.GetType().GetGenericTypeDefinition() == typeof(WithReferenceAttribute<>))
                                .Select(a => a.GetType().GetGenericArguments()[0]);

            foreach (var referenceType in references)
            {
                var referenceResource = GetResourceBuilder<IResourceWithConnectionString>(referenceType);

                serviceResourceBuilderWithWaitSupport.WaitFor(referenceResource);
                serviceResourceBuilderWithEnvironment.WithReference(referenceResource);
            }

            var waitForAttribute = service.GetType().GetCustomAttribute<WaitForAttribute>()!;
            if(waitForAttribute is null)
                continue;

            foreach (var dependency in waitForAttribute.Dependencies)
            {
                var dependencyResource = GetResourceBuilder<IResource>(dependency);

                serviceResourceBuilderWithWaitSupport.WaitFor(dependencyResource);
            }
        }
    }
}
