using CommerceFlow.Infrastructure;
using CommerceFlow.Orders;
using CommerceFlow.WebApi.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Security.Claims;

namespace CommerceFlow.WebApi;

public class OrdersController(CommerceFlowDbContext dbContext) : ODataController<Order>
{
    [EnableQuery]
    public IActionResult Get()
    {
        if (!User.Identity.IsAuthenticated) 
            return Unauthorized();
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var customerId = new Guid(nameIdentifier);

        var orders = dbContext.Set<Order>()
                    .Where(o => o.CustomerId == customerId);
        return Ok(orders);
    }
}