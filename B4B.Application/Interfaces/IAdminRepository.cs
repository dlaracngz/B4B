using B4B.Domain.Entities;

namespace B4B.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);

        Task AddAsync(Admin admin);
    }
}