#nullable enable
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace UserService.Messaging
{
    // Note: You must update IMessagePublisher to return Task instead of void
    public class RabbitMQPublisher : IMessagePublisher
    {
        private IConnection? _connection;
        private IChannel? _channel; // CHANGED: IModel is now IChannel
        private readonly ConnectionFactory _factory;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            // 1. Only configure the factory here. Do not connect yet.
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"]!,
                Port = int.Parse(configuration["RabbitMQ:Port"]!),
                UserName = configuration["RabbitMQ:Username"]!,
                Password = configuration["RabbitMQ:Password"]!
            };
        }

        // 2. Helper method to initialize connection asynchronously
        private async Task InitializeRabbitMQAsync()
        {
            if (_connection == null)
            {
                _connection = await _factory.CreateConnectionAsync();
            }

            if (_channel == null)
            {
                _channel = await _connection.CreateChannelAsync();

                // ExchangeDeclare is now Async
                await _channel.ExchangeDeclareAsync("user_events", ExchangeType.Topic, durable: true);
            }
        }

        // 3. Method must be Async and return Task
        public async Task PublishAsync<T>(string routingKey, T message)
        {
            await InitializeRabbitMQAsync();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // FIX: Create the properties object explicitly
            var properties = new BasicProperties();
            // Optional: properties.Persistent = true; (if you want messages to survive restart)

            await _channel!.BasicPublishAsync(
                exchange: "user_events",
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties, // Pass the object here, not null
                body: body);
        }
    }
}