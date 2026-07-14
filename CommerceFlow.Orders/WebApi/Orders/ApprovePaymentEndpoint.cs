using CommerceFlow.Application;
using Wolverine;

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
        await bus.PublishAsync(command);

        return Results.Ok();
    }

    public IEndpointConventionBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPut($"{Routes.Orders}/{{orderId}}/approve-payment", ApprovePaymentAsync)
           .WithTags(Routes.Orders)
           .Produces(StatusCodes.Status200OK)
           .WithSummary("Aprova um pagamento para o pedido.");
    }
}

internal sealed record ApprovePaymentRequest(string PaymentReference);
