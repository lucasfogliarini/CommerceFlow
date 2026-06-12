namespace CommerceFlow.WebApi.Endpoints;

internal sealed class GetOrderEndpoint : IEndpoint
{
    public async Task<IResult> GetOrderAsync(
        Guid orderId,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order is null)
            return Results.NotFound();

        var response = new OrderResponse(
            order.Id,
            order.Number,
            order.CustomerId,
            order.Status,
            order.Items.Select(i => new OrderItemResponse(
                i.Product.Id,
                i.Product.Name,
                i.Quantity,
                i.Product.UnitPrice,
                i.TotalAmount)).ToList(),
            order.TotalAmount,
            order.Shipment?.TrackingCode,
            order.Payment.Status,
            order.Payment.PaymentReference);

        return Results.Ok(response);
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet($"{Routes.Orders}/orders/{{orderId}}", GetOrderAsync)
           .WithTags(Routes.Orders)
           .Produces<OrderResponse>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status404NotFound)
           .WithSummary("Recupera um pedido pelo id.");
    }
}

internal sealed record OrderResponse(
    Guid Id,
    string Number,
    Guid CustomerId,
    OrderStatus Status,
    List<OrderItemResponse> Items,
    decimal TotalAmount,
    string? TrackingCode,
    PaymentStatus PaymentStatus,
    string? PaymentReference);

internal sealed record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount);
