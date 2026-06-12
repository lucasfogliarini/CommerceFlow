namespace Aspire.C4;

using Aspire.Hosting.ApplicationModel;

/// <summary>
/// Represents a system context as defined by the C4 Model.
/// https://c4model.com/abstractions/system-context
///
/// Architectural diagrams for this system should be maintained using a C4-compliant tool.
/// IcePanel is recommended for creating and maintaining these diagrams: https://icepanel.io/
/// </summary>
public abstract class SoftwareSystemContext(IDistributedApplicationBuilder builder, int Port = 2000)
{
    public const string LucasFogliariniAuthor = "lucasfogliarini";
    public const string GithubUniformResourceLocator = "https://github.com";
    public const string HostDefault = "127.0.0.1";//localhost

    /// <summary>
    /// The Author or Organization of the repositories for the domains and software systems (e.g. lucasfogliarini)
    /// </summary>
    protected abstract string RepositoriesAuthor { get; init; }
    /// <summary>
    /// The URL of the repositories for the domains and software systems (e.g. https://github.com, https://gitlab.com)
    /// </summary>
    protected abstract string RepositoriesUniformResourceLocator { get; init; }
    /// <summary>
    /// The repository URL for this domain or software system. (<see cref="RepositoryUniformResourceLocator"/> + <see cref="RepositoriesAuthor"/> + <see cref="Name"/>)
    /// </summary>
    protected string RepositoryUniformResourceLocator { get { return $"{RepositoriesUniformResourceLocator}/{RepositoriesAuthor}/{Name}"; } }
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

    /// <summary>
    /// Represents a list of containers as defined by the C4 Model.
    /// https://c4model.com/abstractions/container
    /// </summary>
    public IList<Service> Services { get; init; } = [];

    public virtual IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = new Service<ExternalServiceResource>
        {
            Name = Name,
            Port = Port,
            Host = HostDefault,
            Resource = Builder.AddExternalService(Name, SystemContextDiagramUrl)
        };
        if (RepositoryUniformResourceLocator is not null)
            system.Resource.WithUrl(RepositoryUniformResourceLocator);
        if (DomainUniformResourceLocator is not null)
            system.Resource.WithUrl(DomainUniformResourceLocator);
        Services.Add(system);

        var observabilityService = AddService("observabilityservice");

        Builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";
        Builder.Configuration["ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL"] = observabilityService!.Uri.ToString();
        Builder.Configuration["ASPNETCORE_URLS"] = system.Uri.ToString();

        return system.Resource;
    }

    public Service<Resource> AddService(string name, string host = HostDefault, int? port = null)
    {
        return AddService<Service<Resource>>(name, host, port);
    }
    public TService AddService<TService>(string name, string host = HostDefault, int? port = null) where TService : Service
    {
        port ??= Services.Any() ? Services.Max(e => e.Port) + 1 : Port + 1;
        var service = Activator.CreateInstance<TService>();
        service.Host = host;
        service.Port = port.Value;
        service.Name = name;
        Services.Add(service);
        return service;
    }
    public DatabaseServer<TDatabaseServerResource> AddDatabaseServer<TDatabaseServerResource>(string name, string username, string password, string provider, string host = HostDefault, int? servicePort = null) where TDatabaseServerResource : Resource
    {
        var databaseServer = AddService<DatabaseServer<TDatabaseServerResource>>(name, host, servicePort);
        databaseServer.Provider = provider;
        databaseServer.UsernameResource = Builder.AddParameter(DatabaseServers.DatabaseServerUsernameKey, username);
        databaseServer.PasswordResource = Builder.AddParameter(DatabaseServers.DatabaseServerPasswordKey, password, secret: true);

        return databaseServer;
    }
    public TService? GetService<TService>(string? name = null) where TService : Service
    {
        return Services.OfType<TService>().FirstOrDefault(e => name == null || e.Name == name);
    }

    public static TSystem CreateBuilder<TSystem>() where TSystem : SoftwareSystemContext
    {
        var builder = DistributedApplication.CreateBuilder();
        var system = (TSystem)Activator.CreateInstance(typeof(TSystem), builder)!;
        system.AddSystem();
        return system;
    }
}
