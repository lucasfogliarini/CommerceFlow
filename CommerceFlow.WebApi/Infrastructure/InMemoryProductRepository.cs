namespace CommerceFlow.WebApi.Infrastructure;

internal sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = [];

    public ICommitScope CommitScope { get; } = new NoopCommitScope();

    public Task<IEnumerable<Product>> GetProductsByIds(Guid[] ids, CancellationToken cancellationToken)
    {
        var products = _products.Where(p => ids.Contains(p.Id));
        return Task.FromResult(products);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.Add(product);
        return Task.CompletedTask;
    }
}
