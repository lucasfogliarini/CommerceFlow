using CommerceFlow.Application.Shipments;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CommerceFlow.WebApi.Shipments;

internal sealed class RegisterTrackingEventEndpoint : IEndpoint
{
    public async Task<IResult> RegisterTrackingEventAsync(
        Guid shipmentId,
        RegisterTrackingEventRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterTrackingEvent(shipmentId, request.Description, request.Location);
        await bus.PublishAsync(command);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.Shipments}/{{shipmentId}}/tracking", RegisterTrackingEventAsync)
           .WithTags(Routes.Shipments)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Register a tracking event for a shipment.");
    }
}

internal sealed record RegisterTrackingEventRequest(string Description, string Location);
