using CommerceFlow.Application;
using Wolverine;

namespace CommerceFlow.WebApi.Endpoints;

internal sealed class RejectPaymentEndpoint : IEndpoint
{
    public async Task<IResult> RejectPaymentAsync(
        Guid orderId,
        RejectPaymentRequest request,
        IMessageBus bus,
        CancellationToken cancellationToken = default)
    {
        var message = new RejectPayment(orderId, request.PaymentReference, request.Reason);
        await bus.PublishAsync(message);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Orders}/{{orderId}}/reject-payment", RejectPaymentAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Rejeita um pagamento para o pedido.");
    }
}

internal sealed record RejectPaymentRequest(string PaymentReference, string Reason);
