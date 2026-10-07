namespace B4B.Infrastructure.Services
{
    public class ProductSearchDocument
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }
}