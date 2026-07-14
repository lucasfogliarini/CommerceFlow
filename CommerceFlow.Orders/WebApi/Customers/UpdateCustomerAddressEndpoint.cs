using CommerceFlow.Application;
using System.Security.Claims;
using Wolverine;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class UpdateCustomerAddressEndpoint : IEndpoint
{
    public async Task<IResult> UpdateAsync(Guid addressId, UpdateCustomerAddressRequest request, ClaimsPrincipal user, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId)) return Results.Unauthorized();

        var updated = await bus.InvokeAsync<bool>(new UpdateCustomerAddress(customerId, addressId, request.Street, request.Number, request.City, request.State, request.ZipCode, request.Country), cancellationToken);
        return updated ? Results.NoContent() : Results.NotFound();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Customers}/me/addresses/{{addressId:guid}}", UpdateAsync)
            .WithTags(Routes.Customers)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}

internal sealed record UpdateCustomerAddressRequest(string Street, string Number, string City, string State, string ZipCode, string Country);
