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
        if (user.Identity?.IsAuthenticated != true) return Results.Unauthorized();

        var createCustomer = new CreateCustomerIfNotExist(user);
        var customerGuid = await bus.InvokeAsync<Guid>(createCustomer, cancellationToken);        

        var createOrder = new CreateOrder(customerGuid, request.ShippingAddress, request.Items);
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
