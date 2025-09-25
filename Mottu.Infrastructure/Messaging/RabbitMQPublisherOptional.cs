using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mottu.Domain.Interfaces;

namespace Mottu.Infrastructure.Messaging
{
    public class RabbitMQPublisherOptional : IMessagePublisher
    {
        private readonly ILogger<RabbitMQPublisherOptional> _logger;

        public RabbitMQPublisherOptional(ILogger<RabbitMQPublisherOptional> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(T message, string routingKey) where T : class
        {
            _logger.LogWarning("RabbitMQ não está disponível. Mensagem seria publicada: {RoutingKey} - {Message}", 
                routingKey, System.Text.Json.JsonSerializer.Serialize(message));
            
            await Task.CompletedTask;
        }
    }
}

