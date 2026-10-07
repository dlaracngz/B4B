using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using B4B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly B4BDbContext _context;

        public CartRepository(B4BDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(
            Guid userId,
            Guid companyId)
        {
            return await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.CompanyId == companyId);
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task AddItemAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(Guid cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == cartItemId);

            if (cartItem == null)
                return;

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();
        }
    }
}