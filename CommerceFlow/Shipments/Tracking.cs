namespace CommerceFlow.Shipments;

public class Tracking
{
    private readonly List<TrackingEvent> _events = [];

    private Tracking()
    {
    }

    public Tracking(string trackingCode)
    {
        TrackingCode = trackingCode;
    }

    public string TrackingCode { get; private set; }

    public IReadOnlyCollection<TrackingEvent> Events => _events;

    public void AddEvent(TrackingEvent trackingEvent)
    {
        _events.Add(trackingEvent);
    }
}
