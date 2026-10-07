using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using B4B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly B4BDbContext _context;

        public ProductRepository(B4BDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid id, Guid companyId)
        {
            return await _context.Products
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId);
        }

        public async Task<List<Product>> GetByCompanyIdAsync(Guid companyId)
        {
            return await _context.Products
                .Where(x => x.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}