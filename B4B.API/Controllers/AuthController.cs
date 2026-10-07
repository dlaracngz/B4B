using B4B.Application.DTOs.Auth;
using B4B.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace B4B.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (result != null)
                return BadRequest(new
                {
                    message = result
                });

            return Ok(new
            {
                message = "Kullanıcı başarıyla oluşturuldu."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new
                {
                    message = "Kullanıcı adı veya şifre hatalı."
                });

            Response.Cookies.Append("B4B.Auth", result, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60)
            });

            return Ok(new
            {
                message = "Giriş başarılı."
            });
        }
    }
}