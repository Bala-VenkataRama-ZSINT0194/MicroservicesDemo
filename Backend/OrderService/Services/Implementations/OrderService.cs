using AutoMapper;
using OrderService.Data.Entities;
using OrderService.Dtos;
using OrderService.Repositories.Interfaces;
using OrderService.Services.Interfaces;

namespace OrderService.Services.Implementations
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OrderServiceImpl(
            IRepository<Order> orderRepository,
            IMapper mapper,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.FindAsync(o => o.UserId == userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            // Fetch user details from UserService
            var userServiceUrl = _configuration["UserService:Url"];
            var response = await _httpClient.GetAsync($"{userServiceUrl}/api/users/{createOrderDto.UserId}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"User with ID {createOrderDto.UserId} not found");

            var userData = await response.Content.ReadFromJsonAsync<UserDataDto>();

            var order = new Order
            {
                UserId = createOrderDto.UserId,
                UserName = userData.Name,
                UserEmail = userData.Email,
                ProductName = createOrderDto.ProductName,
                Amount = createOrderDto.Amount,
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };

            var createdOrder = await _orderRepository.AddAsync(order);
            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");

            order.ProductName = updateOrderDto.ProductName;
            order.Amount = updateOrderDto.Amount;
            order.Status = updateOrderDto.Status;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found");

            await _orderRepository.DeleteAsync(order);
        }

        // Helper DTO for deserializing user data from UserService
        private class UserDataDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
