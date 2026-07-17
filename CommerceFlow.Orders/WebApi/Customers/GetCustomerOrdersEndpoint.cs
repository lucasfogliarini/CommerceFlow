using CommerceFlow.Application;
using CommerceFlow.Orders;
using System.Security.Claims;
using Wolverine;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetCustomerOrdersEndpoint : IEndpoint
{
    public async Task<IResult> GetCustomerOrdersAsync(IMessageBus bus, ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email)) return Results.Unauthorized();

        var orders = await bus.InvokeAsync<IEnumerable<GetCustomerOrdersResponse>>(new GetCustomerOrders(email), cancellationToken);

        return Results.Ok(orders);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Customers}/me/orders", GetCustomerOrdersAsync)
            .WithTags(Routes.Customers)
            .Produces<IEnumerable<GetCustomerOrdersResponse>>(StatusCodes.Status200OK)
            .WithSummary("Recupera os pedidos do cliente autenticado.");
    }

   
}
