using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using B4B.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B4B.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly B4BDbContext _context;

        public UserRepository(B4BDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(x => x.Company)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.Username == username);
        }
    }
}