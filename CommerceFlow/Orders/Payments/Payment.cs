namespace CommerceFlow;

public record Payment
{
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; set; }
    public string? PaymentReference { get; private set; }
    public string? RejectedReason { get; private set; }

    public void Approve(string paymentReference)
    {
        Status = PaymentStatus.Approved;
        PaymentReference = paymentReference;
    }

    public void Reject(string reason)
    {
        Status = PaymentStatus.Rejected;
        RejectedReason = reason;
    }

    public static Payment Create(decimal amount)
    {
        return new Payment
        {
            Amount = amount,
            Status = PaymentStatus.Processing
        };
    }
}
