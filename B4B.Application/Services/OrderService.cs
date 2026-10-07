using B4B.Application.DTOs.Admin;
using B4B.Application.DTOs.Order;
using B4B.Application.DTOs.Product;
using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace B4B.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITenantService _tenantService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _cache;
        private readonly IProductSearchService _productSearchService;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            ITenantService tenantService,
            IHttpContextAccessor httpContextAccessor,
            IDistributedCache cache,
            IProductSearchService productSearchService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _tenantService = tenantService;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _productSearchService = productSearchService;
        }

        public async Task<List<AdminOrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(order => new AdminOrderResponseDto
            {
                Id = order.Id,
                CompanyId = order.CompanyId,
                CompanyName = order.Company.Name,
                UserId = order.UserId,
                Username = order.User.Username,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            }).ToList();
        }

        public async Task<string?> CreateOrderAsync()
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

            if (cart == null || !cart.CartItems.Any())
                return "Sepet boş.";

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId.Value,
                UserId = userId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepository.GetByIdAsync(
                    cartItem.ProductId,
                    companyId.Value);

                if (product == null)
                    return $"Ürün bulunamadı: {cartItem.ProductId}";

                if (!product.IsActive)
                    return $"Ürün aktif değil: {product.Name}";

                if (cartItem.Quantity > product.StockQuantity)
                    return $"Yeterli stok yok. Ürün: {product.Name}, Mevcut stok: {product.StockQuantity}";

                var itemTotal = product.Price * cartItem.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,

                    // Sipariş anındaki fiyat
                    UnitPrice = product.Price,
                    TotalPrice = itemTotal
                });

                totalPrice += itemTotal;

                // Stoktan düş
                product.StockQuantity -= cartItem.Quantity;
                product.UpdatedAt = DateTime.UtcNow;

                // SQL Server güncelle
                await _productRepository.UpdateAsync(product);

                // Elasticsearch stok bilgisini güncelle
                var productDto = new ProductResponseDto
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    Name = product.Name,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CriticalStockLevel = product.CriticalStockLevel,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                await _productSearchService.IndexProductAsync(
                    productDto,
                    companyId.Value);
            }

            order.TotalPrice = totalPrice;

            await _orderRepository.AddAsync(order);

            // Redis'teki eski ürün listesini temizle
            await _cache.RemoveAsync(
                $"products:company:{companyId}");

            // Sepeti temizle
            cart.CartItems.Clear();
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(cart);

            return null;
        }

        // Sadece giriş yapan kullanıcının siparişleri
        public async Task<List<OrderResponseDto>> GetMyOrdersAsync()
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return new List<OrderResponseDto>();

            var userIdValue = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!Guid.TryParse(userIdValue, out var userId))
                return new List<OrderResponseDto>();

            var orders = await _orderRepository.GetByUserIdAsync(
                userId,
                companyId.Value);

            return orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CompanyId = order.CompanyId,
                CreatedAt = order.CreatedAt,

                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductCode = item.Product.ProductCode,
                    ProductName = item.Product.Name,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            }).ToList();
        }

        // Sipariş Detay
        public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return null;

            var userIdValue = _httpContextAccessor.HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!Guid.TryParse(userIdValue, out var userId))
                return null;

            var order = await _orderRepository.GetByIdAsync(
                id,
                userId,
                companyId.Value);

            if (order == null)
                return null;

            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CompanyId = order.CompanyId,
                CreatedAt = order.CreatedAt,

                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductCode = item.Product.ProductCode,
                    ProductName = item.Product.Name,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            };
        }
    }
}