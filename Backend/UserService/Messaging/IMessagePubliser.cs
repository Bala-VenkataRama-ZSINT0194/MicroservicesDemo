namespace UserService.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string routingKey, T message);
    }
}
