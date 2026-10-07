using B4B.Application.DTOs.Product;

namespace B4B.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
        Task<List<ProductResponseDto>> GetAllAsync();
        Task<string?> CreateAsync(CreateProductDto request);
        Task<string?> UpdateAsync(Guid id, UpdateProductDto request);
        Task<string?> DeleteAsync(Guid id);
    }
}