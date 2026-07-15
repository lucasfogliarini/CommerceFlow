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
        if (customer is null) return Results.Ok(Array.Empty<CustomerOrderResponse>());

        var orders = customer.Orders
            .OrderByDescending(order => order.CreatedAt)
            .Select(o => new CustomerOrderResponse
                            (
                                o.Id,
                                o.Number,
                                o.Status,
                                o.TotalAmount.GetValueOrDefault(),
                                o.CreatedAt, 
                                o.Shipment?.TrackingCode,
                                o.Items.Select(i=> new CustomerOrderItemResponse(i.Product.Name, i.Product.UnitPrice, i.Quantity))
                            )
                    );

        return Results.Ok(orders);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Customers}/me/orders", GetCustomerOrdersAsync)
            .WithTags(Routes.Customers)
            .Produces<IEnumerable<CustomerOrderResponse>>(StatusCodes.Status200OK)
            .WithSummary("Recupera os pedidos do cliente autenticado.");
    }

    public record CustomerOrderResponse(Guid Id, string Number, OrderStatus Status, decimal TotalAmount, DateTime CreatedAt, string? TrackingCode, IEnumerable<CustomerOrderItemResponse> Items);
    public record CustomerOrderItemResponse(string ProductName, decimal UnitPrice, int Quantity);
}
