namespace Aspire.C4;

using Aspire.Hosting.ApplicationModel;

/// <summary>
/// Represents a database server service which can host databases and expose credentials
/// as parameter resources. The generic parameter indicates the type of resource exposed by the server.
/// </summary>
public class DatabaseServer<TServerResource> : Service<TServerResource> where TServerResource : Resource
{
    public string? Provider { get; set; }
    public Database? Database { get; set; }
    public IResourceBuilder<ParameterResource>? UsernameResource { get; set; }
    public IResourceBuilder<ParameterResource>? PasswordResource { get; set; }
}

/// <summary>
/// Constants for well-known database provider identifiers and configuration keys.
/// </summary>
public static class DatabaseServers
{
    public const string postgres = "postgres";
    public const string mssql = "mssql";
    public const string oracle = "oracle";
    public const string mysql = "mysql";
    public const string azure_tables = "azure_tables";
    public const string DatabaseServerUsernameKey = "database-server-username";
    public const string DatabaseServerPasswordKey = "database-server-password";
}
