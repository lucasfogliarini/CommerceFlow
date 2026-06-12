namespace Aspire.C4;

using Aspire.Hosting.ApplicationModel;

/// <summary>
/// Represents a container in the C4 Model. A container hosts a runnable component
/// such as a web application, database, or background worker and exposes a network endpoint.
/// See: https://c4model.com/abstractions/container
/// </summary>
public class Service
{
    public string Name { get; set; } = "Bora";
    public string Scheme { get; set; } = "http";
    public required string Host { get; set; }
    public int Port { get; set; } = 2000;
    public Uri Uri => new($"{Scheme}://{Host}:{Port}");

}
/// <summary>
/// Generic service that carries an associated resource description of type TResource.
/// The Resource property allows attaching infrastructure metadata (external service, database resource, etc.).
/// </summary>
public class Service<TResource> : Service where TResource : Resource
{
    public IResourceBuilder<TResource>? Resource { get; set; }
}
