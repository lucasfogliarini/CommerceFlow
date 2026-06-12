using Aspire.C4;

public abstract class CommerceFlowSystem(IDistributedApplicationBuilder builder) : SoftwareSystemContext(builder)
{
    protected override string RepositoriesAuthor { get; init; } = LucasFogliariniAuthor;
    protected override string RepositoriesUniformResourceLocator { get; init; } = GithubUniformResourceLocator;
    protected override string TopDomainLevel { get; init; } = "com";
    protected override string Domain { get; init; } = "commerceflow";
    protected override string SystemContextDiagramUrl { get; init; } = "https://app.icepanel.io/landscapes/v3MTVXKMPjCfg4LaPENe/";
    protected const string BoraDatabaseNameAndConnectionStringName = "BoraDatabase";
    protected const string BoraDatabaseServerName = "bora-database-server";
    protected const string BoraDefaultPassword = "Bora+" + LucasFogliariniAuthor;
}