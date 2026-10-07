namespace B4B.Application.DTOs.Order
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
        public decimal TotalPrice =>
            Items.Sum(x => x.TotalPrice);
        public DateTime CreatedAt { get; set; }
    }
}