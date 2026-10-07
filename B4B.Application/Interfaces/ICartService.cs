using B4B.Application.DTOs.Cart;

namespace B4B.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync();

        Task<string?> AddItemAsync(AddCartItemDto request);

        Task<string?> UpdateItemAsync(
            Guid cartItemId,
            int quantity);

        Task<string?> RemoveItemAsync(Guid cartItemId);
    }
}