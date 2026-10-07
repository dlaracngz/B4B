using B4B.Application.DTOs.Product;

namespace B4B.Application.Interfaces
{
    public interface IProductSearchService
    {
        // Ürünü Elasticsearch'e kaydeder
        Task IndexProductAsync(ProductResponseDto product, Guid companyId);

        // Elasticsearch'te ürün arar.
        Task<List<ProductResponseDto>> SearchAsync(Guid companyId, string search);

        // Tüm ürünleri Elasticsearch'e aktarır.
        Task IndexAllProductsAsync();

        // Ürünü Elasticsearch'ten siler.
        Task DeleteProductAsync(Guid productId);
    }
}