namespace Aspire.C4;

/// <summary>
/// Represents a container in the C4 Model. A container hosts a runnable component
/// such as a web application, database, or background worker and exposes a network endpoint.
/// See: https://c4model.com/abstractions/container
/// </summary>
public abstract class Service
{
    public abstract string Name { get; }
    public abstract void Configure(SoftwareSystemContextBuilder system);

    protected string DataVolumeName => $"{this.GetType().Name}_data";
}
