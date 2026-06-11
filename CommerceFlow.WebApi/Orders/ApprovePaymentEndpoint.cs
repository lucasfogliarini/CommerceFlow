using CommerceFlow.Application;
using Wolverine;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class ApprovePaymentEndpoint : IEndpoint
{
    public async Task<IResult> ApprovePaymentAsync(
        Guid orderId,
        ApprovePaymentRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var command = new ApprovePayment(orderId, request.PaymentReference);
        await bus.InvokeAsync(command, cancellationToken);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Orders}/orders/{{orderId}}/approve", ApprovePaymentAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Aprova um pagamento para o pedido.");
    }
}

internal sealed record ApprovePaymentRequest(string PaymentReference);
