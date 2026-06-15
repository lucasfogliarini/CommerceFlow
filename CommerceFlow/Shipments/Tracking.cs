namespace CommerceFlow.Shipments;

public class Tracking : Entity
{
    private readonly List<TrackingEvent> _events = [];

    private Tracking()
    {
    }

    public string TrackingCode { get; private set; }

    public IReadOnlyCollection<TrackingEvent> Events => _events;

    public void AddEvent(
        DateTime occurredAt,
        string description,
        string location)
    {
        var trackingEvent = new TrackingEvent(occurredAt, description, location);
        _events.Add(trackingEvent);
    }

    public static Tracking Create()
    {
        var tracking = new Tracking
        {
            TrackingCode = $"TRK-{Guid.NewGuid():N}"[..16]
        };

        return tracking;
    }
}
