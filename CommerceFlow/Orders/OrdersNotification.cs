namespace CommerceFlow.Orders;

public record OrdersNotification(Guid OrderId, string Message) : INotification;
