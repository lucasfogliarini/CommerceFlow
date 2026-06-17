namespace Aspire.C4;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class DependsOnAttribute<TDependencyResource> : Attribute where TDependencyResource : IResourceWithConnectionString;