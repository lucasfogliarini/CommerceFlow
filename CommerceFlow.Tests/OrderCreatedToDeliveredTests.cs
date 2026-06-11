using System;
using System.Linq;
using Xunit;
using CommerceFlow;

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
            var payment = Payment.Create(order.Id, Guid.NewGuid());

            // Act
            payment.Approve();

            // Assert
            Assert.Equal(PaymentStatus.Approved, payment.Status);
            Assert.Contains(payment.DomainEvents, e => e is PaymentApproved);
        }

        [Fact(DisplayName = "5. Confirmar Pedido")]
        public void WhenPaymentIsApproved_ThenOrderIsConfirmed()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.WaitForPayment();

            // Act
            order.Confirm();

            // Assert
            Assert.Equal(OrderStatus.Confirmed, order.Status);
            Assert.Contains(order.DomainEvents, e => e is OrderConfirmed);
        }

        [Fact(DisplayName = "6. Iniciar Entrega")]
        public void WhenOrderIsConfirmed_ThenStartShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            var shipment = Shipment.Create(order.Id);

            // Act
            shipment.Start();

            // Assert
            Assert.Equal(ShipmentStatus.Pending, shipment.Status);
            Assert.Contains(shipment.DomainEvents, e => e is ShipmentStarted);
        }

        [Fact(DisplayName = "7. Despachar Entrega")]
        public void DispatchShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            var shipment = Shipment.Create(order.Id);
            var trackingCode = "TRACK123";

            // Act
            shipment.Dispatch(trackingCode);

            // Assert
            Assert.Equal(ShipmentStatus.Shipped, shipment.Status);
            Assert.Equal(trackingCode, shipment.TrackingCode);
            Assert.Contains(shipment.DomainEvents, e => e is OrderShipped);
        }

        [Fact(DisplayName = "8. Entrega Concluída")]
        public void CompleteShipment()
        {
            // Arrange
            var order = CreateOrderHelper();
            var shipment = Shipment.Create(order.Id);
            shipment.Dispatch("TRACK123");

            // Act
            shipment.Complete();

            // Assert
            Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
            Assert.Contains(shipment.DomainEvents, e => e is OrderDelivered);
        }
    }
}
