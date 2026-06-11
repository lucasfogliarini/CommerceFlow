using CommerceFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class CompleteShipmentEndpoint : IEndpoint
{
    public async Task<IResult> CompleteShipmentAsync(
        Guid orderId,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new CompleteShipment(orderId);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Orders}/orders/{{orderId}}/complete", CompleteShipmentAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Complete a shipment for an order.");
    }
}
