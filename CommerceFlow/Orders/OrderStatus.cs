namespace CommerceFlow.Orders;

public enum OrderStatus
{
    Created,
    WaitingForPayment,
    PaymentApproved,
    PaymentExpired,    
    PaymentRejected,
    ReadyForShipment,
    Dispatched,    
    Delivered,
    Cancelled,
}
