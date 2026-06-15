using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class RequestShipmentHandler(IOrderRepository orderRepository) : IDomainEventHandler<PaymentApproved>
{
    public async Task HandleAsync(PaymentApproved paymentApproved, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(paymentApproved);

        var order = await orderRepository.GetByIdAsync(paymentApproved.OrderId, cancellationToken);
        if (order is null) return;

        order.RequestShipment();

        orderRepository.Update(order);
        await orderRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
