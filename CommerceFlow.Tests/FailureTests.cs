using System;
using System.Linq;
using Xunit;

namespace CommerceFlow.Tests
{
    public class FailureTests
    {

        private Order CreateOrderHelper(Guid? customerId = null, int quantity = 1, Guid? productId = null)
        {
            customerId ??= Guid.NewGuid();
            productId ??= Guid.NewGuid();
            var product = Product.Create(productId.Value, "Produto Teste", 5.00m, 10);
            var item = new OrderItem(product, quantity);
            var order = Order.Create(customerId.Value, [item]);
            return order;
        }

        [Fact(DisplayName = "Reservar Estoque deve retornar erro quando não houver estoque disponível")]
        public void WhenOrderIsCreated_ThenReserveInventory()
        {
            // Arrange
            var order = CreateOrderHelper();
            var productId = order.Items.First().Product.Id;
            var product = Product.Create(productId, "Product1", 5m, 2);

            // Act
            product.Reserve(order.Id, 3);

            // Assert
            Assert.Equal(2, product.AvailableQuantity);
            Assert.Equal(0, product.ReservedQuantity);
            Assert.Contains(product.DomainEvents, e => e is InventoryUnavailable);
        }

        [Fact(DisplayName = "Rejeitar Pagamento deve cancelar o pedido e disparar eventos")]
        public void WhenPaymentIsRejected_OrderIsCancelled()
        {
            // Arrange
            var order = CreateOrderHelper();
            order.ReserveInventory();
            order.WaitForPayment();

            // Act
            order.RejectPayment("Card declined");

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Contains(order.DomainEvents, e => e is PaymentRejected);
            Assert.Contains(order.DomainEvents, e => e is OrderCancelled);
        }
    }
}
