namespace CommerceFlow.Shipments
{
    public class ShipmentItem
    {
        private ShipmentItem()
        {
        }

        public ShipmentItem(
            Guid productId,
            int quantity,
            decimal weight)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be greater than zero.");

            ProductId = productId;
            Quantity = quantity;
            Weight = weight;
        }

        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        public decimal Weight { get; private set; }

        public decimal TotalWeight => Weight * Quantity;
    }
}
