using CommerceFlow.Shipments;

namespace CommerceFlow.WebApi.Shipments;

internal sealed class GetShipmentEndpoint : IEndpoint
{
    public async Task<IResult> GetShipmentAsync(
        Guid orderId,
        IShipmentRepository shipmentRepository,
        CancellationToken cancellationToken = default)
    {
        var shipment = await shipmentRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (shipment is null)
            return Results.NotFound();

        return Results.Ok(shipment);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Shipments}", GetShipmentAsync)
           .WithTags(Routes.Shipments)
           .Produces<Shipment>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound)
           .WithSummary("Get shipment by order ID.");
    }
}
