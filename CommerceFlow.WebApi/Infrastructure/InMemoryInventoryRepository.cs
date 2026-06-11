
namespace CommerceFlow.WebApi.Infrastructure;

internal sealed class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly List<Inventory> _items = [];

    public ICommitScope CommitScope { get; } = new NoopCommitScope();

    public Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        return Task.FromResult(item);
    }

    public Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        _items.Add(inventory);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        var idx = _items.FindIndex(i => i.ProductId == inventory.ProductId);
        if (idx >= 0) _items[idx] = inventory;
        return Task.CompletedTask;
    }
}
