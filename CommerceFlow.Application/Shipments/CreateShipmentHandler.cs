using CommerceFlow.Orders;
using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public class CreateShipmentHandler(IShipmentRepository shipmentRepository, IProductRepository productRepository)
{
    public async Task HandleAsync(OrderReadyForShipment orderReadyForShipment, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderReadyForShipment);

        var products = await productRepository.GetProductsByIds([.. orderReadyForShipment.Items.Select(i => i.ProductId)], cancellationToken);
        var shipmentItems = orderReadyForShipment.Items.Select(i =>
        {
            var product = products.FirstOrDefault(p => p.Id == i.ProductId);
            return product is null
                ? throw new InvalidOperationException($"Product with ID {i.ProductId} not found.")
                : new ShipmentItem(product.Id, i.Quantity, product.Weight);
        }).ToList();

        var shipment = Shipment.Create(orderReadyForShipment.OrderNumber, orderReadyForShipment.ShipmentAddress, shipmentItems);

        await shipmentRepository.AddAsync(shipment, cancellationToken);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
