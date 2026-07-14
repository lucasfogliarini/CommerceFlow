using CommerceFlow.Orders;
using CommerceFlow.Orders;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetOrderEndpoint : IEndpoint
{
    public async Task<IResult> GetOrderAsync(
        Guid orderId,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Results.NotFound();

        return Results.Ok(order);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Orders}/{{orderId}}", GetOrderAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status404NotFound)
           .WithSummary("Recupera um pedido pelo id.");
    }
}
