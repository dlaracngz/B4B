using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using B4B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly B4BDbContext _context;

        public CompanyRepository(B4BDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByDomainAsync(string domain)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(x => x.Domain == domain);
        }

        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies
                .ToListAsync();
        }
    }
}