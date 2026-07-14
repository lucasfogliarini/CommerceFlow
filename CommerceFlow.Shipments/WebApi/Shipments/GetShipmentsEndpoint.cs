using CommerceFlow.Shipments;

namespace CommerceFlow.WebApi.Shipments;

internal sealed class GetShipmentsEndpoint : IEndpoint
{
    public async Task<IResult> GetShipmentsAsync(
        IShipmentRepository shipmentRepository,
        CancellationToken cancellationToken = default)
    {
        var shipments = await shipmentRepository.GetShipmentsAsync(cancellationToken);

        if (shipments is null || !shipments.Any())
            return Results.NotFound();

        return Results.Ok(shipments);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Shipments}", GetShipmentsAsync)
           .WithTags(Routes.Shipments)
           .Produces<IEnumerable<Shipment>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound)
           .WithSummary("Get all shipments.");
    }
}
