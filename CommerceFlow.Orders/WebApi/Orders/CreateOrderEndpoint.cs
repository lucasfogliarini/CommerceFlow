using CommerceFlow.Application;
using Wolverine;
using System.Security.Claims;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class CreateOrderEndpoint : IEndpoint
{
    public async Task<IResult> CreateOrderAsync(
        CreateOrderRequest request,
        ClaimsPrincipal User,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var customerId)) return Results.Unauthorized();    

        var createOrder = new CreateOrder(customerId, request.ShippingAddress, request.Items);
        var orderGuid = await bus.InvokeAsync<Guid>(createOrder, cancellationToken);

        return Results.Ok(orderGuid);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.Orders}", CreateOrderAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Cria um novo pedido.");
    }
}

internal sealed record CreateOrderRequest(ShippingAddress ShippingAddress, List<CreateOrderItem> Items);
