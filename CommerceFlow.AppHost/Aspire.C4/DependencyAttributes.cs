namespace Aspire.C4;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class WithReferenceAttribute<TDependencyResource> : Attribute where TDependencyResource : IResourceWithConnectionString;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class WaitForAttribute(params string[] dependencies) : Attribute
{
    public string[] Dependencies { get; } = dependencies;
}
