using CommerceFlow.Application;
using System.Security.Claims;
using Wolverine;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetCustomerAddressesEndpoint : IEndpoint
{
    public async Task<IResult> GetCustomerAddressesAsync(
        ClaimsPrincipal User,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var customerId)) return Results.Unauthorized();

        var addresses = await bus.InvokeAsync<List<Address>>(
            new GetCustomerAddresses(customerId),
            cancellationToken);

        return Results.Ok(addresses);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Customers}/me/addresses", GetCustomerAddressesAsync)
            .WithTags(Routes.Customers)
            .Produces<List<Address>>(StatusCodes.Status200OK)
            .WithSummary("Recupera os endereços do cliente autenticado.");
    }
}
