using B4B.Application.DTOs.Admin;

namespace B4B.Application.Interfaces
{
    public interface IAdminService
    {
        Task<string?> LoginAsync(AdminLoginDto request);

        Task<string?> CreateAsync(CreateAdminDto request);
    }
}