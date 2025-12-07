using AutoMapper;
using UserService.Data.Entities;
using UserService.Dtos;
using UserService.Events;
using UserService.Messaging;
using UserService.Repositories.Interfaces;
using UserService.Services.Interfaces;

namespace UserService.Services.Implementations
{
    public class UserServiceImpl : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IMessagePublisher _messagePublisher;

        public UserServiceImpl(
            IRepository<User> userRepository,
            IMapper mapper,
            IMessagePublisher messagePublisher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _messagePublisher = messagePublisher;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? throw new KeyNotFoundException($"User with ID {id} not found") : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            user.CreatedAt = DateTime.UtcNow;

            var createdUser = await _userRepository.AddAsync(user);

            // Publish event to RabbitMQ
            var userCreatedEvent = new UserCreatedEvent
            {
                UserId = createdUser.Id,
                Name = createdUser.Name,
                Email = createdUser.Email,
                CreatedAt = createdUser.CreatedAt
            };
            await _messagePublisher.PublishAsync("user.created", userCreatedEvent);

            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found");
            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found");
            await _userRepository.DeleteAsync(user);

            // Publish event to RabbitMQ
            var userDeletedEvent = new UserDeletedEvent
            {
                UserId = id,
                DeletedAt = DateTime.UtcNow
            };
            await _messagePublisher.PublishAsync("user.deleted", userDeletedEvent);
        }
    }
}
