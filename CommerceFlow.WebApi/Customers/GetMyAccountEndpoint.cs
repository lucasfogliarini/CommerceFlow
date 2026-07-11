using CommerceFlow.Customers;
using CommerceFlow.Orders;
using System.Security.Claims;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetMyAccountEndpoint : IEndpoint
{
    public async Task<IResult> GetMyAccountAsync(
        ClaimsPrincipal User,
        ICustomerRepository customerRepository,
        CancellationToken cancellationToken = default)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return Results.Unauthorized();

        var customer = await customerRepository.GetByEmailAsync(email, cancellationToken);

        var response = new AccountResponse(
            Email: email,
            Customer: customer,
            Orders: customer.Orders.Select(o => new OrderSummary(o.Id, o.Number, o.Status, o.TotalAmount.GetValueOrDefault(), o.Items.Count)).ToList()
        );

        return Results.Ok(response);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Customers}/me", GetMyAccountAsync)
           .WithTags(Routes.Customers)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Recupera os dados da conta do usuário autenticado.");
    }
}

public record AccountResponse(string Email, Customer? Customer, List<OrderSummary> Orders);
public record OrderSummary(Guid Id, string Number, OrderStatus Status, decimal TotalAmount, int ItemsCount);
