using B4B.Application.DTOs.Product;
using B4B.Application.Interfaces;
using B4B.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace B4B.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ITenantService _tenantService;

        private readonly IDistributedCache _cache;

        private readonly IProductSearchService _productSearchService;

        public ProductService(
            IProductRepository productRepository,
            ITenantService tenantService, IDistributedCache cache, IProductSearchService productSearchService)
        {
            _productRepository = productRepository;
            _tenantService = tenantService;
            _cache = cache;
            _productSearchService = productSearchService;
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return null;

            var product = await _productRepository.GetByIdAsync(id, companyId.Value);

            if (product == null || product.CompanyId != companyId)
                return null;

            return MapToResponse(product);
        }

        public async Task<List<ProductResponseDto>> GetAllAsync()
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return new List<ProductResponseDto>();

            // Her firmanın cache'i ayrı olacak
            var cacheKey = $"products:company:{companyId}";

            // Önce Redis'e bak
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<List<ProductResponseDto>>(cachedData)
                       ?? new List<ProductResponseDto>();
            }

            // Redis'te yoksa SQL Server'dan getir
            var products = await _productRepository
                .GetByCompanyIdAsync(companyId.Value);

            var result = products.Select(MapToResponse).ToList();

            // Redis'e kaydet
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return result;
        }

        public async Task<string?> CreateAsync(CreateProductDto request)
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return "Firma bilgisi doğrulanamadı.";

            var product = new Product
            {
                Id = Guid.NewGuid(),
                ProductCode = request.ProductCode,
                Name = request.Name,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                CriticalStockLevel = request.CriticalStockLevel,
                CompanyId = companyId.Value,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);

            // Eski redis cache'ini silme
            await _cache.RemoveAsync($"products:company:{companyId}");

            var productDto = MapToResponse(product);

            await _productSearchService.IndexProductAsync(
                productDto,
                companyId.Value);

            return null;
        }

        public async Task<string?> UpdateAsync(
            Guid id,
            UpdateProductDto request)
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return "Firma bilgisi doğrulanamadı.";

            var product = await _productRepository.GetByIdAsync(id, companyId.Value);

            if (product == null)
                return "Ürün bulunamadı.";

            if (product.CompanyId != companyId)
                return "Bu ürüne erişim yetkiniz yok.";

            product.ProductCode = request.ProductCode;
            product.Name = request.Name;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.CriticalStockLevel = request.CriticalStockLevel;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            // Ürünle ilgili bişey değiştiğinde redis temizlenir. Güncel sql verisini alıp Redis'e kaydeder. 
            await _cache.RemoveAsync($"products:company:{companyId}");

            var productDto = MapToResponse(product);

            await _productSearchService.IndexProductAsync(
                productDto,
                companyId.Value);

            return null;
        }

        private static ProductResponseDto MapToResponse(Product product)
        {
            return new ProductResponseDto
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
        }

        public async Task<string?> DeleteAsync(Guid id)
        {
            var companyId = await _tenantService.GetCurrentCompanyIdAsync();

            if (companyId == null)
                return "Firma bilgisi doğrulanamadı.";

            var product = await _productRepository.GetByIdAsync(id, companyId.Value);

            if (product == null)
                return "Ürün bulunamadı.";

            if (product.CompanyId != companyId)
                return "Bu ürüne erişim yetkiniz yok.";

            await _productRepository.DeleteAsync(product);

            // Redis cache temizlenir
            await _cache.RemoveAsync($"products:company:{companyId}");

            // Elasticsearch'ten ürünü sil
            await _productSearchService.DeleteProductAsync(product.Id);

            return null;
        }
    }
}