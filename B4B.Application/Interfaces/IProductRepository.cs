using B4B.Domain.Entities;

namespace B4B.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, Guid companyId);
        Task<List<Product>> GetByCompanyIdAsync(Guid companyId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}