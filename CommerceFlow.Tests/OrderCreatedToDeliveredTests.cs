using CommerceFlow.Orders;
using CommerceFlow.Orders;
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
            var product = Product.Create(productId.Value, "produto-teste", "Produto Teste", "Description", 5.00m, 10);
            var item = new OrderItem(product, quantity);
            var address = new Address("Rua Teste","123", "Cidade Teste", "Estado Teste", "12345-678", "Pais Teste");
            var order = Order.Create(customerId.Value, address, [item]);
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

            // Act
            order.ReserveInventory();

            // Assert
            Assert.Equal(9, order.Items.First().Product.AvailableQuantity);
            Assert.Equal(1, order.Items.First().Product.ReservedQuantity);
            Assert.Contains(order.DomainEvents, e => e is OrderInventoryReserved);
        }

        [Fact(DisplayName = "3. Aguardar Pagamento")]
        public void WhenInventoryIsReserved_ThenOrderWaitsForPayment()
        {
            // Arrange & Act
            var order = CreateOrderHelper();
            order.ReserveInventory();

            // Assert
            Assert.Equal(OrderStatus.WaitingForPayment, order.Status);
            Assert.Contains(order.DomainEvents, e => e is OrderWaitingForPayment);
        }

        [Fact(DisplayName = "4. Aprovar Pagamento")]
        public void ApprovePayment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.ReserveInventory();

            // Act
            order.ApprovePayment("PAYMENT123");

            // Assert
            Assert.Equal(OrderStatus.PaymentApproved, order.Status);
            Assert.Equal(PaymentStatus.Approved, order.Payment.Status);
            Assert.Contains(order.DomainEvents, e => e is PaymentApproved);
        }

        [Fact(DisplayName = "5. Iniciar Entrega")]
        public void WhenOrderIsPaid_ThenReadyForShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.ReserveInventory();
            order.ApprovePayment("PAYMENT123");

            // Act
            order.ReadyForShipment();

            // Assert
            Assert.NotNull(order.Shipment);
            Assert.Equal(ShipmentStatus.Requested, order.Shipment!.Status);
            Assert.Contains(order.DomainEvents, e => e is OrderReadyForShipment);
        }

        [Fact(DisplayName = "6. Despachar Entrega")]
        public void DispatchShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.ReserveInventory();
            order.ApprovePayment("PAYMENT123");
            order.ReadyForShipment();
            var trackingCode = "TRACK123";

            // Act
            order.DispatchShipment(trackingCode);

            // Assert
            Assert.NotNull(order.Shipment);
            Assert.Equal(ShipmentStatus.Dispatched, order.Shipment!.Status);
            Assert.Equal(trackingCode, order.Shipment!.TrackingCode);
        }

        [Fact(DisplayName = "7. Entrega Concluída")]
        public void CompleteShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.ReserveInventory();
            order.ApprovePayment("PAYMENT123");
            order.ReadyForShipment();
            order.DispatchShipment("TRACK123");

            // Act
            order.DeliverShipment();

            // Assert
            Assert.NotNull(order.Shipment);
            Assert.Equal(ShipmentStatus.Delivered, order.Shipment!.Status);
            Assert.Equal(OrderStatus.Delivered, order.Status);
        }
    }
}
