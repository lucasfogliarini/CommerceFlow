namespace CommerceFlow.Shipments;

public record ShippingAddress(
                string Street,
                string Number,
                string Neighborhood,
                string City,
                string State,
                string ZipCode,
                string Country);
