using CommerceFlow.Application;
using CommerceFlow.Customers;
using System.Security.Claims;
using Wolverine;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetCustomerEndpoint : IEndpoint
{
    public async Task<IResult> GetCustomerAsync(IMessageBus bus, ClaimsPrincipal User, CancellationToken cancellationToken = default)
    {
        var getOrCreateCustomer = new GetOrCreateCustomer(User);
        var customer = await bus.InvokeAsync<Customer>(getOrCreateCustomer);

        return Results.Ok(customer);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Customers}/me", GetCustomerAsync)
            .WithTags(Routes.Customers)
            .Produces(StatusCodes.Status200OK)
            .WithSummary("Recupera os dados do cliente autenticado.");
    }
}
