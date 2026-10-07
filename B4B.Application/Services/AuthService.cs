using B4B.Application.DTOs.Auth;
using B4B.Application.Interfaces;
using B4B.Application.Security;
using B4B.Domain.Entities;

namespace B4B.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordService _passwordService;
        private readonly PasswordValidator _passwordValidator;
        private readonly JwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            PasswordService passwordService,
            PasswordValidator passwordValidator,
            JwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _passwordValidator = passwordValidator;
            _jwtService = jwtService;
        }

        public async Task<string?> RegisterAsync(RegisterUserDto request)
        {
            if (!_passwordValidator.IsValid(request.Password))
            {
                return "Şifre en az 8 karakter olmalı, en az 1 büyük harf, 1 küçük harf, 1 rakam ve 1 özel karakter içermelidir.";
            }

            var hashedPassword = _passwordService.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                Password = hashedPassword,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                CompanyId = request.CompanyId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return null;
        }
        public async Task<string?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null)
                return null;

            var isPasswordValid = _passwordService.VerifyPassword(
                user.Password,
                request.Password);

            if (!isPasswordValid)
                return null;

            var token = _jwtService.GenerateToken(user);

            return token;
        }
    }
}