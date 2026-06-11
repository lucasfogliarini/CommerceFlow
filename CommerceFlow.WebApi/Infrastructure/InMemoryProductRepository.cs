using CommerceFlow;

namespace CommerceFlow.WebApi.Infrastructure;

internal sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public ICommitScope CommitScope { get; } = new NoopCommitScope();

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.Add(product);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var idx = _products.FindIndex(p => p.Id == product.Id);
        if (idx >= 0) _products[idx] = product;
        return Task.CompletedTask;
    }
}
