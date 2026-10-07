using B4B.Domain.Entities;

namespace B4B.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);

        Task AddAsync(User user);

        Task<User?> GetByUsernameAsync(string username);
    }
}