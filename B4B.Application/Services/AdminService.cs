using B4B.Application.DTOs.Admin;
using B4B.Application.Interfaces;
using B4B.Application.Security;

namespace B4B.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly PasswordService _passwordService;
        private readonly JwtService _jwtService;
        private readonly PasswordValidator _passwordValidator;

        public AdminService(
            IAdminRepository adminRepository,
            PasswordService passwordService,
            PasswordValidator passwordValidator,
            JwtService jwtService)
        {
            _adminRepository = adminRepository;
            _passwordService = passwordService;
            _passwordValidator = passwordValidator;
            _jwtService = jwtService;
        }

        public async Task<string?> LoginAsync(AdminLoginDto request)
        {
            var admin = await _adminRepository
                .GetByUsernameAsync(request.Username);

            if (admin == null)
                return "Kullanıcı adı veya şifre hatalı.";

            var isValid = _passwordService.VerifyPassword(
                admin.PasswordHash,
                request.Password);

            if (!isValid)
                return "Kullanıcı adı veya şifre hatalı.";

            var token = _jwtService.GenerateToken(admin);

            return token;
        }
        public async Task<string?> CreateAsync(CreateAdminDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return "Kullanıcı adı zorunludur.";

            if (string.IsNullOrWhiteSpace(request.Email))
                return "Email zorunludur.";

            if (!_passwordValidator.IsValid(request.Password))
                return "Şifre en az 8 karakter olmalı ve büyük harf, küçük harf, rakam ve özel karakter içermelidir.";

            var existingAdmin = await _adminRepository
                .GetByUsernameAsync(request.Username);

            if (existingAdmin != null)
                return "Bu kullanıcı adı zaten kullanılıyor.";

            var admin = new Domain.Entities.Admin
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _adminRepository.AddAsync(admin);

            return null;
        }

    }
}