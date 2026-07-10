namespace CommerceFlow;

public interface IProductRepository : IRepository
{
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    Task<IEnumerable<Product>> GetProductsByIds(Guid[] ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
}