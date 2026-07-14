using CommerceFlow.Orders;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetOrdersEndpoint : IEndpoint
{
    public async Task<IResult> GetOrdersAsync(
        IOrderRepository orderRepository,
        CancellationToken cancellationToken = default)
    {
        var orders = await orderRepository.GetOrdersAsync(cancellationToken);

        var orderResponses = orders.Select(o => new OrderResponse(o.Id, o.Status));

        return Results.Ok(orderResponses);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Orders}", GetOrdersAsync)
           .WithTags(Routes.Orders)
           .WithSummary("Recupera todos os pedidos.");
    }

    public record OrderResponse(Guid Id, OrderStatus Status);
}
