using System;
using System.Linq;
using Xunit;

namespace CommerceFlow.Tests
{
    public class OrderCreatedToDeliveredTests
    {

        private Order CreateOrderHelper(Guid? customerId = null, int quantity = 1, Guid? productId = null)
        {
            customerId ??= Guid.NewGuid();
            productId ??= Guid.NewGuid();
            var product = Product.Create(productId.Value, "Produto Teste", 5.00m);
            var item = new OrderItem(product, quantity);
            var order = Order.Create(customerId.Value, [item]);
            return order;
        }

        [Fact(DisplayName = "1. Criar Pedido")]
        public void CreateOrder()
        {
            // Arrange & Act
            var order = CreateOrderHelper();

            // Assert
            Assert.Equal(OrderStatus.Created, order.Status);
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Contains(order.DomainEvents, e => e is OrderCreated);
        }

        [Fact(DisplayName = "2. Reservar Estoque")]
        public void WhenOrderIsCreated_ThenReserveInventory()
        {
            // Arrange
            var order = CreateOrderHelper();
            var productId = order.Items.First().Product.Id;
            var inventory = Inventory.Create(productId, 5);

            // Act
            inventory.Reserve(order.Id, 2);

            // Assert
            Assert.Equal(3, inventory.AvailableQuantity);
            Assert.Equal(2, inventory.ReservedQuantity);
            Assert.Contains(inventory.DomainEvents, e => e is InventoryReserved);
        }

        [Fact(DisplayName = "3. Aguardar Pagamento")]
        public void WhenInventoryIsReserved_ThenOrderWaitsForPayment()
        {
            // Arrange & Act
            var order = CreateOrderHelper();
            order.WaitForPayment();

            // Assert
            Assert.Equal(OrderStatus.WaitingForPayment, order.Status);
            Assert.Contains(order.DomainEvents, e => e is OrderWaitingForPayment);
        }

        [Fact(DisplayName = "4. Aprovar Pagamento")]
        public void ApprovePayment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.WaitForPayment();

            // Act
            order.ApprovePayment("PAYMENT123");

            // Assert
            Assert.Equal(OrderStatus.PaymentApproved, order.Status);
            Assert.Equal(PaymentStatus.Approved, order.Payment.Status);
            Assert.Contains(order.DomainEvents, e => e is PaymentApproved);
        }

        [Fact(DisplayName = "5. Iniciar Entrega")]
        public void WhenOrderIsConfirmed_ThenStartShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.WaitForPayment();
            order.ApprovePayment("PAYMENT123");

            // Act
            order.StartShipment();

            // Assert
            Assert.NotNull(order.Shipment);
            Assert.Equal(ShipmentStatus.Started, order.Shipment!.Status);
            Assert.Contains(order.DomainEvents, e => e is ShipmentStarted);
        }

        [Fact(DisplayName = "6. Despachar Entrega")]
        public void DispatchShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.WaitForPayment();
            order.ApprovePayment("PAYMENT123");
            order.StartShipment();
            var trackingCode = "TRACK123";

            // Act
            order.DispatchShipment(trackingCode);

            // Assert
            Assert.NotNull(order.Shipment);
            Assert.Equal(ShipmentStatus.Dispatched, order.Shipment!.Status);
            Assert.Equal(trackingCode, order.Shipment!.TrackingCode);
            Assert.Contains(order.DomainEvents, e => e is OrderShipped);
        }

        [Fact(DisplayName = "7. Entrega Concluída")]
        public void CompleteShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.WaitForPayment();
            order.ApprovePayment("PAYMENT123");
            order.StartShipment();
            order.DispatchShipment("TRACK123");

            // Act
            order.CompleteShipment();

            // Assert
            Assert.NotNull(order.Shipment);
            Assert.Equal(ShipmentStatus.Delivered, order.Shipment!.Status);
            Assert.Equal(OrderStatus.Delivered, order.Status);
            Assert.Contains(order.DomainEvents, e => e is OrderDelivered);
        }
    }
}
