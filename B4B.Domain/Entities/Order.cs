namespace B4B.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}