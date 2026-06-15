using CommerceFlow.Shipments;

namespace CommerceFlow.Application.Shipments;

public class ShipmentRequestedHandler(IShipmentRepository shipmentRepository, IProductRepository productRepository)
{
    public async Task HandleAsync(ShipmentRequested shipmentRequested, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(shipmentRequested);

        var products = await productRepository.GetProductsByIds([.. shipmentRequested.Items.Select(i => i.ProductId)], cancellationToken);
        var shipmentItems = shipmentRequested.Items.Select(i =>
        {
            var product = products.FirstOrDefault(p => p.Id == i.ProductId);
            return product is null
                ? throw new InvalidOperationException($"Product with ID {i.ProductId} not found.")
                : new ShipmentItem(product.Id, i.Quantity, product.Weight);
        }).ToList();

        var shipment = Shipment.Create(shipmentRequested.OrderId, shipmentRequested.ShippingAddress, shipmentItems);

        await shipmentRepository.AddAsync(shipment, cancellationToken);
        await shipmentRepository.CommitScope.CommitAsync(cancellationToken);
    }
}
