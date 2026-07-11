using CommerceFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;
using System.Security.Claims;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class CreateOrderEndpoint : IEndpoint
{
    public async Task<IResult> CreateOrderAsync(
        CreateOrderRequest request,
        ClaimsPrincipal user,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return Results.Unauthorized();

        var command = new CreateOrder(email, request.ShippingAddress, request.Items);
        var guid = await bus.InvokeAsync<Guid>(command, cancellationToken);

        return Results.Ok(guid);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.Orders}", CreateOrderAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Cria um novo pedido.");
    }
}

internal sealed record CreateOrderRequest(Address ShippingAddress, List<CreateOrderItem> Items);
