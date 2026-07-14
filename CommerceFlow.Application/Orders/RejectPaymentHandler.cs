using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class RejectPaymentHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(RejectPayment rejectPayment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rejectPayment);

        var order = await orderRepository.GetByNumberAsync(rejectPayment.OrderNumber, cancellationToken);
        if (order is null) return;

        order.RejectPayment(rejectPayment.PaymentReference, rejectPayment.Reason);

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record RejectPayment(string OrderNumber, string PaymentReference, string Reason);
