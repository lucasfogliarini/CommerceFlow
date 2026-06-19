namespace CommerceFlow.Orders;

public enum OrderStatus
{
    Created,
    WaitingForPayment,
    PaymentApproved,
    ReadyForShipment,
    Dispatched,    
    Delivered,
    Cancelled,
}
