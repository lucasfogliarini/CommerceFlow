namespace CommerceFlow.WebApi.Infrastructure;

internal sealed class OrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];

    public ICommitScope CommitScope { get; } = new NoopCommitScope();

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = _orders.FirstOrDefault(i => i.Id == id);
        return Task.FromResult(order);
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        var idx = _orders.FindIndex(i => i.Id == order.Id);
        if (idx >= 0) _orders[idx] = order;
        return Task.CompletedTask;
    }
}
