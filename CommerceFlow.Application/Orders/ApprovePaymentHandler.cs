using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class ApprovePaymentHandler(IOrderRepository orderRepository)
{
    public async Task HandleAsync(ApprovePayment approvePayment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(approvePayment);

        var order = await orderRepository.GetByNumberAsync(approvePayment.OrderNumber, cancellationToken);
        if (order is null) return;

        order.ApprovePayment(approvePayment.PaymentReference);
        
        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}

public record ApprovePayment(string OrderNumber, string PaymentReference);
