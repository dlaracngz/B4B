using B4B.Application.DTOs.Admin;
using B4B.Application.DTOs.Order;

namespace B4B.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<AdminOrderResponseDto>> GetAllOrdersAsync();
        Task<string?> CreateOrderAsync();

        Task<List<OrderResponseDto>> GetMyOrdersAsync();

        Task<OrderResponseDto?> GetByIdAsync(Guid id);
    }
}