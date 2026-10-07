using B4B.Application.DTOs.Auth;

namespace B4B.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterUserDto request);

        Task<string?> LoginAsync(LoginRequestDto request);
    }
}