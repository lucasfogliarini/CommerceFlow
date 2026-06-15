namespace CommerceFlow.Shipments;

public class TrackingEvent : Entity
{
    private TrackingEvent()
    {
    }

    public TrackingEvent(
        DateTime occurredAt,
        string description,
        string location)
    {
        OccurredAt = occurredAt;
        Description = description;
        Location = location;
    }

    public DateTime OccurredAt { get; private set; }

    public string Description { get; private set; }

    public string Location { get; private set; }
}
