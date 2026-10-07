using B4B.Application.DTOs.Admin;
using B4B.Application.Interfaces;
using B4B.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B4B.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IUserService _userService;

        private readonly IOrderService _orderService;

        private readonly IProductSearchService _productSearchService;

        public AdminController(
            IAdminService adminService,
            IUserService userService,
            IOrderService orderService,
            IProductSearchService productSearchService)
        {
            _adminService = adminService;
            _userService = userService;
            _orderService = orderService;
            _productSearchService = productSearchService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AdminLoginDto request)
        {
            var token = await _adminService.LoginAsync(request);

            if (token == null)
                return Unauthorized(new
                {
                    message = "Kullanıcı adı veya şifre hatalı."
                });

            return Ok(new
            {
                token
            });
        }

        // Burda güvenli hale getirmek için daha sonra [Authorize(Roles = "Admin")] bunu eklemeliyiz [AllowAnonymous] bunun yerine . 
        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateAdminDto request)
        {
            var result = await _adminService.CreateAsync(request);

            if (result != null)
                return BadRequest(new
                {
                    message = result
                });

            return Ok(new
            {
                message = "Admin başarıyla oluşturuldu."
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("index-products")]
        public async Task<IActionResult> IndexProducts()
        {
            await _productSearchService.IndexAllProductsAsync();

            return Ok(new
            {
                message = "Ürünler Elasticsearch'e aktarıldı."
            });
        }
    }
}