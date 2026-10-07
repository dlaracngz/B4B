namespace B4B.Application.DTOs.Cart
{
    public class CartItemResponseDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public int StockQuantity { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}