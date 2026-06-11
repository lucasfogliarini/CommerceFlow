namespace CommerceFlow;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsByIds(Guid[] ids, CancellationToken cancellationToken = default);
}