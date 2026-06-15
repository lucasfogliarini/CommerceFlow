namespace CommerceFlow.Shipments;

public class Carrier : Entity
{
    private Carrier()
    {
    }

    public Carrier(
        Guid id,
        string name)
    {
        Id = id;
        Name = name;
    }

    public string Name { get; private set; }
}
