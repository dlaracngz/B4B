using B4B.Domain.Entities;

namespace B4B.Application.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(Guid userId, Guid companyId);

        Task AddAsync(Cart cart);

        Task AddItemAsync(CartItem cartItem);

        Task UpdateAsync(Cart cart);

        Task DeleteItemAsync(Guid cartItemId);
    }
}