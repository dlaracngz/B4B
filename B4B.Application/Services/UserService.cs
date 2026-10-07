using B4B.Application.DTOs;
using B4B.Application.DTOs.Admin;
using B4B.Application.Interfaces;

namespace B4B.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<AdminUserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(user => new AdminUserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                CompanyId = user.CompanyId,
                CompanyName = user.Company.Name,
                IsActive = user.IsActive
            }).ToList();
        }
        public async Task<UserCompanyResponseDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return null;

            return new UserCompanyResponseDto
            {
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone
                },

                Company = new CompanyResponseDto
                {
                    Id = user.Company.Id,
                    Name = user.Company.Name,
                    Phone = user.Company.Phone,
                    Email = user.Company.Email,
                    Address = user.Company.Address
                }
            };
        }
    }
}