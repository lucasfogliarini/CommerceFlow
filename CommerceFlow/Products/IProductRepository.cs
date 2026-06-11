namespace CommerceFlow;

public interface IProductRepository : IRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
