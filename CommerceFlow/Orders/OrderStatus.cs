namespace CommerceFlow.Orders;

public enum OrderStatus
{
    Created,
    WaitingForPayment,
    PaymentApproved,
    PaymentRejected,
    ReadyForShipment,
    Dispatched,    
    Delivered,
    Cancelled,
}
