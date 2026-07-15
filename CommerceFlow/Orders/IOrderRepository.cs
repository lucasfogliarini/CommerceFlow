namespace CommerceFlow.Orders;

public interface IOrderRepository : IRepository
{
    Task<Guid?> GetCustomerIdAsync(string number, CancellationToken cancellationToken = default);
    Task<Order?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void Update(Order order);
}
