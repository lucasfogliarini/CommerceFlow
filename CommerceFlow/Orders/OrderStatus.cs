namespace CommerceFlow;

public enum OrderStatus
{
    Created,
    InventoryReserved,
    WaitingForPayment,
    PaymentApproved,
    Cancelled,
    Delivered
}
