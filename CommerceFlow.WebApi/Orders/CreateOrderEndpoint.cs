using CommerceFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class CreateOrderEndpoint : IEndpoint
{
    public async Task<IResult> CreateOrderAsync(
        CreateOrderRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateOrder(request.CustomerId, request.ShippingAddress, request.Items);
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

internal sealed record CreateOrderRequest(Guid CustomerId, Address ShippingAddress, List<CreateOrderItem> Items);
