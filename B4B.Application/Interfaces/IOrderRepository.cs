using B4B.Domain.Entities;

namespace B4B.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id, Guid userId, Guid companyId);

        Task<List<Order>> GetByUserIdAsync(
            Guid userId,
            Guid companyId);

        Task AddAsync(Order order);
    }
}