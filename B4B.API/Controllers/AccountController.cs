using B4B.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace B4B.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITenantService _tenantService;

        public AccountController(
            IUserRepository userRepository,
            ITenantService tenantService)
        {
            _userRepository = userRepository;
            _tenantService = tenantService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // Domain + JWT üzerinden mevcut şirketi bul
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
            {
                return Forbid();
            }

            // JWT'den UserId al
            var userIdValue = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Kullanıcı bilgisi JWT içinde bulunamadı."
                });
            }

            // Kullanıcıyı database'den getir
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "Kullanıcı bulunamadı."
                });
            }

            // Kullanıcının şirketi ile mevcut tenant aynı mı?
            if (user.CompanyId != companyId)
            {
                return Forbid();
            }

            return Ok(new
            {
                userId = user.Id,
                username = user.Username,
                companyId = user.CompanyId,
                companyName = user.Company.Name
            });
        }
    }
}