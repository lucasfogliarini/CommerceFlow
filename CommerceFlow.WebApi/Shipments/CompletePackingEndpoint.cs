using CommerceFlow.Application.Shipments;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CommerceFlow.WebApi.Shipments;

internal sealed class CompletePackingEndpoint : IEndpoint
{
    public async Task<IResult> CompletePackingAsync(
        Guid shipmentId,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new CompletePacking(shipmentId);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Shipments}/{{shipmentId}}/complete-packing", CompletePackingAsync)
           .WithTags(Routes.Shipments)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Complete packing for a shipment.");
    }
}
