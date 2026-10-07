namespace B4B.Application.DTOs.Cart
{
    public class CartResponseDto
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }

        public List<CartItemResponseDto> Items { get; set; }

        public decimal TotalPrice =>
            Items.Sum(x => x.TotalPrice);
    }
}