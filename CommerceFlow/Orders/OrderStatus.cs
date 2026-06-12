namespace CommerceFlow;

public enum OrderStatus
{
    Created,
    InventoryReserved,
    WaitingForPayment,
    PaymentApproved,
    Dispatched,    
    Delivered,
    Cancelled,
}
