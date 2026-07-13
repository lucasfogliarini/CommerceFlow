using CommerceFlow.Customers;
using CommerceFlow.Orders;
using System.Security.Claims;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetCustomerOrdersEndpoint : IEndpoint
{
    public async Task<IResult> GetCustomerOrdersAsync(ClaimsPrincipal user, ICustomerRepository customerRepository, CancellationToken cancellationToken = default)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email)) return Results.Unauthorized();

        var customer = await customerRepository.GetByEmailAsync(email, cancellationToken);
        if (customer is null) return Results.Ok(Array.Empty<OrderSummary>());

        var orders = customer.Orders
            .OrderByDescending(order => order.CreatedAt)
            .Select(o => new OrderSummary(o.Id, o.CreatedAt, o.Number, o.Status, o.TotalAmount.GetValueOrDefault(), o.Items.Count));

        return Results.Ok(orders);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Customers}/me/orders", GetCustomerOrdersAsync)
            .WithTags(Routes.Customers)
            .Produces<IEnumerable<OrderSummary>>(StatusCodes.Status200OK)
            .WithSummary("Recupera os pedidos do cliente autenticado.");
    }
}

public record OrderSummary(Guid Id, DateTime CreatedAt, string Number, OrderStatus Status, decimal TotalAmount, int ItemsCount);
