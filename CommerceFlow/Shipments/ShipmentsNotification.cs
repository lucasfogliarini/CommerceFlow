namespace CommerceFlow.Shipments;
public record ShipmentsNotification(Guid ShipmentId, string Message) : INotification;
