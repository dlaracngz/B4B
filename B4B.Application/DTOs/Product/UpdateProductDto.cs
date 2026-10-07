namespace B4B.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        public string ProductCode { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int CriticalStockLevel { get; set; }

        public bool IsActive { get; set; }
    }
}