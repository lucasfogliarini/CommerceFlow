namespace CommerceFlow;

public record ShippingAddress(
    string Street,
    string Number,
    string City,
    string State,
    string ZipCode,
    string Country);
