namespace CommerceFlow.Orders;

public record OrdersNotification(string OrderNumber, string Message) : INotification;
