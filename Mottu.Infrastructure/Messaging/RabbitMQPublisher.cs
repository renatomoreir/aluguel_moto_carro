using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mottu.Domain.Interfaces;
using RabbitMQ.Client;

namespace Mottu.Infrastructure.Messaging
{
    public class RabbitMQPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly string _exchangeName = "mottu.exchange";

        public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
        {
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = configuration.GetConnectionString("RabbitMQ") ?? "localhost",
                UserName = "guest",
                Password = "guest"
            };

            try
            {
                _connection = factory.CreateConnectionAsync().Result;
                _channel = _connection.CreateChannelAsync().Result;

                // Declarar exchange
                _channel.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Topic, durable: true).Wait();

                _logger.LogInformation("RabbitMQ Publisher conectado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao conectar com RabbitMQ");
                throw;
            }
        }

        public async Task PublishAsync<T>(T message, string routingKey) where T : class
        {
            try
            {
                var messageBody = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(messageBody);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                };

                await _channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Mensagem publicada: {RoutingKey} - {Message}", routingKey, messageBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem: {RoutingKey}", routingKey);
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.CloseAsync().Wait();
            _connection?.CloseAsync().Wait();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

