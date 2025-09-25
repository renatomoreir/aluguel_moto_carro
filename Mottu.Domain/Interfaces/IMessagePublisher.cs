namespace Mottu.Domain.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string routingKey) where T : class;
    }
}

