namespace CommerceFlow.Shipments
{
    public class ShipmentItem
    {
        private ShipmentItem()
        {
        }

        public ShipmentItem(
            Guid productId,
            string name,
            int quantity,
            decimal weight,
            Dimensions dimensions)
        {
            if (quantity <= 0)
                throw new ArgumentException(
                    "Quantity must be greater than zero.");

            ProductId = productId;
            Name = name;
            Quantity = quantity;
            Weight = weight;
            Dimensions = dimensions;
        }

        public Guid ProductId { get; private set; }

        public string Name { get; private set; }

        public int Quantity { get; private set; }

        public decimal Weight { get; private set; }

        public Dimensions Dimensions { get; private set; }

        public decimal TotalWeight =>
            Weight * Quantity;
    }
}
