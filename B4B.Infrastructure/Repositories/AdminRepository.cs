using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using B4B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly B4BDbContext _context;

        public AdminRepository(B4BDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(x =>
                    x.Username == username &&
                    x.IsActive);
        }
        public async Task AddAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }
    }
}