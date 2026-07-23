using CommerceFlow.Infrastructure;
using CommerceFlow.WebApi.OData;
using Microsoft.AspNetCore.OData.Query;

namespace CommerceFlow.WebApi;

public class ProductsController(CommerceFlowDbContext dbContext) : ODataController<Product>
{
    [EnableQuery]
    public IQueryable<Product> Get()
    {
        return dbContext.Set<Product>();
    }
}