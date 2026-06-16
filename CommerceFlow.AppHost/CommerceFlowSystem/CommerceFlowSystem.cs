using Aspire.C4;

public abstract class CommerceFlowSystem(IDistributedApplicationBuilder builder) : SoftwareSystemContext(builder)
{
    protected override string RepositoriesAuthor { get; init; } = "lucasfogliarini";
    protected override string RepositoriesDomainUrl { get; init; } = GithubDomainUrl;
    protected override string TopDomainLevel { get; init; } = "com";
    protected override string Domain { get; init; } = "commerceflow";
    protected override string SystemContextDiagramUrl { get; init; } = "https://app.icepanel.io/landscapes/v3MTVXKMPjCfg4LaPENe/";
}