using Aspire.C4;

public class CommerceFlowSystemBuilder(IDistributedApplicationBuilder builder) : SoftwareSystemContextBuilder(builder)
{
    protected override string RepositoriesAuthor { get; init; } = "lucasfogliarini";
    protected override string RepositoriesDomainUrl { get; init; } = GithubDomainUrl;
    protected override string TopDomainLevel { get; init; } = "com";
    protected override string Domain { get; init; } = "commerceflow";
    protected override string SystemContextDiagramUrl { get; init; } = "https://s.icepanel.io/8ed1WNhopjo4xF/QQdp/";
}