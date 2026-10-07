namespace B4B.Application.DTOs.Admin
{
    public class AdminOrderResponseDto
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }

        public string CompanyName { get; set; }

        public Guid UserId { get; set; }

        public string Username { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}