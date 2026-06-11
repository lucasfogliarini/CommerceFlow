namespace CommerceFlow;

public class Payment : AggregateRoot
{
    public Guid Id { get; private set; }
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
}