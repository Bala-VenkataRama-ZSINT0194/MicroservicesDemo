#nullable enable
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events; // Required for AsyncEventingBasicConsumer
using System.Text;
using System.Text.Json;
using UserService.Events;

public class RabbitMQConsumer : IMessageConsumer, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ConnectionFactory _factory;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        // 1. Setup Factory (Do not connect yet)
        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };
    }

    // 2. Helper to Initialize Connection & Channel
    private async Task InitializeRabbitMQAsync()
    {
        if (_connection == null)
        {
            _connection = await _factory.CreateConnectionAsync();
        }

        if (_channel == null)
        {
            _channel = await _connection.CreateChannelAsync();

            // All declarations are now Async
            await _channel.ExchangeDeclareAsync("user_events", ExchangeType.Topic, durable: true);
            await _channel.QueueDeclareAsync("order_user_queue", durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync("order_user_queue", "user_events", "user.*");
        }
    }

    // 3. Renamed to Async (You must update your interface too)
    public async Task StartConsumingAsync()
    {
        await InitializeRabbitMQAsync();

        // Use AsyncEventingBasicConsumer for v7+
        var consumer = new AsyncEventingBasicConsumer(_channel!);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            // Body is ReadOnlyMemory<byte>, convert to array or use directly
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            if (routingKey == "user.deleted")
            {
                try
                {
                    var userDeletedEvent = JsonSerializer.Deserialize<UserDeletedEvent>(message);
                    if (userDeletedEvent != null)
                    {
                        await HandleUserDeleted(userDeletedEvent);
                    }
                }
                catch (Exception ex)
                {
                    // Log error here
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        };

        // BasicConsumeAsync
        await _channel!.BasicConsumeAsync(queue: "order_user_queue", autoAck: true, consumer: consumer);
    }

    private async Task HandleUserDeleted(UserDeletedEvent userDeletedEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        // Assuming AppDbContext is the name of your DB Context
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var orders = await context.Orders
            .Where(o => o.UserId == userDeletedEvent.UserId)
            .ToListAsync();

        foreach (var order in orders)
        {
            order.Status = "User Deleted";
        }

        await context.SaveChangesAsync();
    }

    // 4. Implement IAsyncDisposable for clean cleanup
    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
        }
        if (_connection != null)
        {
            await _connection.CloseAsync();
        }
    }
}