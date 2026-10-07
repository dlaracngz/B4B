using B4B.Application.DTOs.Cart;
using B4B.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B4B.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync();

            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(AddCartItemDto request)
        {
            var result = await _cartService.AddItemAsync(request);

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new
            {
                message = "Ürün sepete eklendi."
            });
        }

        [HttpPut("{cartItemId:guid}")]
        public async Task<IActionResult> UpdateItem(Guid cartItemId, int quantity)
        {
            var result = await _cartService.UpdateItemAsync(
                cartItemId,
                quantity);

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new
            {
                message = "Sepet ürünü güncellendi."
            });
        }

        [HttpDelete("{cartItemId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            var result = await _cartService.RemoveItemAsync(cartItemId);

            if (result != null)
                return BadRequest(new { message = result });

            return Ok(new
            {
                message = "Ürün sepetten kaldırıldı."
            });
        }
    }
}