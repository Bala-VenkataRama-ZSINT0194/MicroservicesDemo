using OrderService.Dtos;

namespace OrderService.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task DeleteOrderAsync(int id);
    }
}
