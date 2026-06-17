
namespace CommerceFlow.Shipments;

public sealed record TrackingUpdated(Guid ShipmentId, string Description, string Location) : IDomainEvent;
