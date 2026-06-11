using CommerceFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class DispatchShipmentEndpoint : IEndpoint
{
    public async Task<IResult> DispatchShipmentAsync(
        Guid orderId,
        DispatchShipmentRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new DispatchShipment(orderId, request.TrackingCode);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Orders}/orders/{{orderId}}/dispatch", DispatchShipmentAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Dispatch a shipment for an order.");
    }
}

internal sealed record DispatchShipmentRequest(string TrackingCode);
