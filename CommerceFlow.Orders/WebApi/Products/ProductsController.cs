using CommerceFlow.Infrastructure;
using CommerceFlow.WebApi.OData;
using Microsoft.AspNetCore.OData.Query;

namespace CommerceFlow.WebApi;

public class ProductsController(CommerceFlowDbContext dbContext) : ODataController<Product>
{
    [EnableQuery]
    public IQueryable<ProductResponse> Get()
    {
        return dbContext.Set<Product>().Select(p => new ProductResponse(
            Id: p.Id,
            Slug: p.Slug,
            Name: p.Name,
            Description: p.Description,
            UnitPrice: p.UnitPrice,
            AvailableQuantity: p.AvailableQuantity,
            ImageUrl: p.ImageUrl
        ));
    }
}

public record ProductResponse(Guid Id, string Slug, string Name, string Description, decimal UnitPrice, int AvailableQuantity, string ImageUrl);