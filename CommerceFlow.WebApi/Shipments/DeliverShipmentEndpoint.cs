using CommerceFlow.Application.Shipments;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CommerceFlow.WebApi.Shipments;

internal sealed class DeliverShipmentEndpoint : IEndpoint
{
    public async Task<IResult> DeliverShipmentAsync(
        Guid orderId,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new DeliverShipment(orderId);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Shipments}/{{shipmentId}}/deliver", DeliverShipmentAsync)
           .WithTags(Routes.Shipments)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Deliver a shipment for an order.");
    }
}
