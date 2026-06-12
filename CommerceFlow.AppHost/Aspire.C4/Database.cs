namespace Aspire.C4;

using Aspire.Hosting.ApplicationModel;

/// <summary>
/// Represents a database resource and its associated IResourceBuilder that exposes a connection string.
/// Provides a typed accessor to the underlying resource implementation.
/// </summary>
public class Database(string name, IResourceBuilder<IResourceWithConnectionString> resource)
{
    public string Name { get; private set; } = name;
    public IResourceBuilder<IResourceWithConnectionString> Resource { get; private set; } = resource;
    public TDatabaseResource? GetResource<TDatabaseResource>()
                where TDatabaseResource : Resource, IResourceWithConnectionString
    {
        return Resource.Resource as TDatabaseResource;
    }
}
