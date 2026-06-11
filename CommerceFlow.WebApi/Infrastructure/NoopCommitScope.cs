using CommerceFlow;

namespace CommerceFlow.WebApi.Infrastructure;

internal sealed class NoopCommitScope : ICommitScope
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
    public int Commit() => 0;
}
