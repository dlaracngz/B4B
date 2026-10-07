namespace B4B.Application.DTOs.Product
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }

        public string ProductCode { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int CriticalStockLevel { get; set; }

        public bool IsCriticalStock =>
            StockQuantity <= CriticalStockLevel;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}