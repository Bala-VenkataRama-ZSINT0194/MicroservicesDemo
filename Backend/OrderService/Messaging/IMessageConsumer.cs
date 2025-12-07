namespace OrderService.Messaging
{
    public interface IMessageConsumer
    {
        Task StartConsumingAsync();
    }
}
