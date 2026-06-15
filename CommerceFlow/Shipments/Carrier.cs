namespace CommerceFlow.Shipments;

public class Carrier : Entity
{
    private Carrier()
    {
    }

    public Carrier(
        Guid id,
        string name,
        string serviceLevel)
    {
        Id = id;
        Name = name;
        ServiceLevel = serviceLevel;
    }

    public string Name { get; private set; }

    public string ServiceLevel { get; private set; }
}
