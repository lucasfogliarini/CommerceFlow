using CommerceFlow.Customers;
using CommerceFlow.Orders;

namespace CommerceFlow.Application;

public class GetCustomerOrdersHandler(ICustomerRepository customerRepository)
{
    public async Task<IEnumerable<GetCustomerOrdersResponse>> HandleAsync(GetCustomerOrders getCustomerOrders, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(getCustomerOrders);

        var customer = await customerRepository.GetByEmailAsync(getCustomerOrders.Email, cancellationToken);
        var orders = customer.Orders
            .OrderByDescending(order => order.CreatedAt)
            .Select(o => new GetCustomerOrdersResponse
                            (
                                o.Id,
                                o.Number,
                                o.Status,
                                o.TotalAmount.GetValueOrDefault(),
                                o.CreatedAt,
                                o.Shipment?.TrackingCode,
                                o.Items.Select(i => new GetCustomerOrdersItemResponse(i.Product.Name, i.Product.UnitPrice, i.Quantity))
                            )
                    );

        return orders;
    }
}

public record GetCustomerOrders(string Email);
public record GetCustomerOrdersResponse(Guid Id, string Number, OrderStatus Status, decimal TotalAmount, DateTime CreatedAt, string? TrackingCode, IEnumerable<GetCustomerOrdersItemResponse> Items);
public record GetCustomerOrdersItemResponse(string ProductName, decimal UnitPrice, int Quantity);
