namespace CommerceFlow.Application;

public class ApprovePaymentHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(ApprovePayment approvePayment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(approvePayment);

        var order = await orderRepository.GetByIdAsync(approvePayment.OrderId, cancellationToken);
        if (order is null) return;

        order.ApprovePayment(approvePayment.PaymentReference);

        await orderRepository.UpdateAsync(order, cancellationToken);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record ApprovePayment(Guid OrderId, string PaymentReference);
