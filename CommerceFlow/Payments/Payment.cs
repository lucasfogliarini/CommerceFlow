namespace CommerceFlow;

public class Payment : AggregateRoot
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }

    public void Approve()
    {
        Status = PaymentStatus.Approved;
        AddDomainEvent(new PaymentApproved(OrderId, Id));
    }

    public void Reject(string reason)
    {
        Status = PaymentStatus.Rejected;
        AddDomainEvent(new PaymentRejected(OrderId, reason));
    }

    public static Payment Create(Guid orderId, Guid paymentId)
    {
        var payment = new Payment
        {
            OrderId = orderId,
            Id = paymentId,
            Status = PaymentStatus.Processing
        };
        return payment;
    }
}