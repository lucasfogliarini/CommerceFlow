namespace CommerceFlow;

public interface IInventoryRepository : IRepository
{
    Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);
    void Update(Inventory inventory);
}
