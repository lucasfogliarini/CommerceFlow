using CommerceFlow.Application;
using System.Security.Claims;
using Wolverine;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class RemoveCustomerAddressEndpoint : IEndpoint
{
    public async Task<IResult> RemoveAsync(Guid addressId, ClaimsPrincipal user, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId)) return Results.Unauthorized();

        var removed = await bus.InvokeAsync<bool>(new RemoveCustomerAddress(customerId, addressId), cancellationToken);
        return removed ? Results.NoContent() : Results.NotFound();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapDelete($"{Routes.Customers}/me/addresses/{{addressId:guid}}", RemoveAsync)
            .WithTags(Routes.Customers)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}