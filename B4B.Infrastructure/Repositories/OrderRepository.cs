using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using B4B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly B4BDbContext _context;

        public OrderRepository(B4BDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .Include(x => x.Company)
                .Include(x => x.User)
                .ToListAsync();
        }
        public async Task<Order?> GetByIdAsync(
            Guid id,
            Guid userId,
            Guid companyId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId &&
                    x.CompanyId == companyId);
        }

        public async Task<List<Order>> GetByUserIdAsync(
            Guid userId,
            Guid companyId)
        {
            return await _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .Where(x =>
                    x.UserId == userId &&
                    x.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
    }
}