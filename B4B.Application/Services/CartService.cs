using B4B.Application.DTOs.Cart;
using B4B.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace B4B.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITenantService _tenantService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ITenantService tenantService,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _tenantService = tenantService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CartResponseDto> GetCartAsync()
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
            {
                return new CartResponseDto
                {
                    Id = null,
                    UserId = Guid.Empty,
                    Items = new List<CartItemResponseDto>()
                };
            }

            var userIdValue = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (string.IsNullOrEmpty(userIdValue))
            {
                userIdValue = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst("sub")?
                    .Value;
            }

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return new CartResponseDto
                {
                    Id = null,
                    UserId = Guid.Empty,
                    Items = new List<CartItemResponseDto>()
                };
            }

            var cart = await _cartRepository.GetByUserIdAsync(
                userId,
                companyId.Value);

            if (cart == null)
            {
                return new CartResponseDto
                {
                    Id = null,
                    UserId = userId,
                    Items = new List<CartItemResponseDto>()
                };
            }

            return new CartResponseDto
            {
                Id = cart.Id,
                UserId = cart.UserId,

                Items = cart.CartItems.Select(x => new CartItemResponseDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductCode = x.Product.ProductCode,
                    ProductName = x.Product.Name,

                    // Güncel ürün fiyatı
                    UnitPrice = x.Product.Price,

                    Quantity = x.Quantity,

                    // Güncel ürün stoğu
                    StockQuantity = x.Product.StockQuantity
                }).ToList()
            };
        }

        public async Task<string?> AddItemAsync(AddCartItemDto request)
        {
            if (request.Quantity <= 0)
                return "Ürün adedi 0'dan büyük olmalıdır.";

            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return "Firma bilgisi doğrulanamadı.";

            var userIdValue = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (string.IsNullOrEmpty(userIdValue))
            {
                userIdValue = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst("sub")?
                    .Value;
            }

            if (!Guid.TryParse(userIdValue, out var userId))
                return "Kullanıcı bilgisi doğrulanamadı.";

            var product = await _productRepository.GetByIdAsync(
                request.ProductId,
                companyId.Value);

            if (product == null)
                return "Ürün bulunamadı.";

            if (!product.IsActive)
                return "Ürün aktif değil.";

            var cart = await _cartRepository.GetByUserIdAsync(
                userId,
                companyId.Value);

            if (cart == null)
            {
                if (request.Quantity > product.StockQuantity)
                    return $"Yeterli stok yok. Mevcut stok: {product.StockQuantity}";

                cart = new Domain.Entities.Cart
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId.Value,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CartItems = new List<Domain.Entities.CartItem>()
                };

                cart.CartItems.Add(new Domain.Entities.CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = request.Quantity
                });

                await _cartRepository.AddAsync(cart);

                return null;
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(x => x.ProductId == product.Id);

            var newQuantity = existingItem == null
                ? request.Quantity
                : existingItem.Quantity + request.Quantity;

            if (newQuantity > product.StockQuantity)
                return $"Yeterli stok yok. Mevcut stok: {product.StockQuantity}";

            if (existingItem == null)
            {
                var cartItem = new Domain.Entities.CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = request.Quantity
                };

                await _cartRepository.AddItemAsync(cartItem);
            }
            else
            {
                existingItem.Quantity = newQuantity;

                cart.UpdatedAt = DateTime.UtcNow;

                await _cartRepository.UpdateAsync(cart);
            }

            return null;
        }

        public async Task<string?> UpdateItemAsync(Guid cartItemId, int quantity)
        {
            if (quantity <= 0)
                return "Ürün adedi 0'dan büyük olmalıdır.";

            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return "Firma bilgisi doğrulanamadı.";

            var userIdValue = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!Guid.TryParse(userIdValue, out var userId))
                return "Kullanıcı bilgisi doğrulanamadı.";

            var cart = await _cartRepository.GetByUserIdAsync(
                userId,
                companyId.Value);

            if (cart == null)
                return "Sepet bulunamadı.";

            var cartItem = cart.CartItems
                .FirstOrDefault(x => x.Id == cartItemId);

            if (cartItem == null)
                return "Sepet ürünü bulunamadı.";

            var product = await _productRepository.GetByIdAsync(
                cartItem.ProductId,
                companyId.Value);

            if (product == null)
                return "Ürün bulunamadı.";

            if (!product.IsActive)
                return "Ürün aktif değil.";

            if (quantity > product.StockQuantity)
                return $"Yeterli stok yok. Mevcut stok: {product.StockQuantity}";

            cartItem.Quantity = quantity;
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(cart);

            return null;
        }

        public async Task<string?> RemoveItemAsync(Guid cartItemId)
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return "Firma bilgisi doğrulanamadı.";

            var userIdValue = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!Guid.TryParse(userIdValue, out var userId))
                return "Kullanıcı bilgisi doğrulanamadı.";

            var cart = await _cartRepository.GetByUserIdAsync(
                userId,
                companyId.Value);

            if (cart == null)
                return "Sepet bulunamadı.";

            var cartItem = cart.CartItems
                .FirstOrDefault(x => x.Id == cartItemId);

            if (cartItem == null)
                return "Sepet ürünü bulunamadı.";

            await _cartRepository.DeleteItemAsync(cartItemId);

            return null;
        }
    }
}