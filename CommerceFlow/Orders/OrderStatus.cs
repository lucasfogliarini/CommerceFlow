namespace CommerceFlow.Orders;

public enum OrderStatus
{
    Created,
    InventoryReserved,
    WaitingForPayment,
    PaymentApproved,
    ReadyForShipment,
    Dispatched,    
    Delivered,
    Cancelled,
}
