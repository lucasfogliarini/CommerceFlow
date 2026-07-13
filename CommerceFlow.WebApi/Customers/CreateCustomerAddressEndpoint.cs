using CommerceFlow.Application;
using System.Security.Claims;
using Wolverine;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class CreateCustomerAddressEndpoint : IEndpoint
{
    public async Task<IResult> CreateCustomerAddressAsync(CreateCustomerAddressRequest request, ClaimsPrincipal user, IMessageBus bus, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var customerId)) return Results.Unauthorized();
        var address = await bus.InvokeAsync<Address?>(new CreateCustomerAddress(customerId, request.Street, request.Number, request.City, request.State, request.ZipCode, request.Country), cancellationToken);
        return address is null ? Results.NotFound() : Results.Created($"{Routes.Customers}/me/addresses/{address.Id}", address);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost($"{Routes.Customers}/me/addresses", CreateCustomerAddressAsync)
            .WithTags(Routes.Customers).Produces<Address>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Cria um endereço para o cliente autenticado.");
    }
}

internal sealed record CreateCustomerAddressRequest(string Street, string Number, string City, string State, string ZipCode, string Country);
