using B4B.Application.DTOs;
using B4B.Application.DTOs.Admin;

namespace B4B.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<AdminUserResponseDto>> GetAllUsersAsync();
        Task<UserCompanyResponseDto?> GetByIdAsync(Guid id);
    }
}