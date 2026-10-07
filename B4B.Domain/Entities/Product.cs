namespace B4B.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string ProductCode { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int CriticalStockLevel { get; set; }

        public Guid CompanyId { get; set; }

        public Company Company { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}