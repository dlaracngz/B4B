using B4B.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B4B.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var result = await _orderService.CreateOrderAsync();

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new
            {
                message = "Sipariş başarıyla oluşturuldu."
            });
        }

        // Kullanıcnın bütün siparişleri
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync();

            return Ok(orders);
        }
         
        // Tek bir siparişin detayı
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound(new { message = "Sipariş bulunamadı." });

            return Ok(order);
        }
    }
}