using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mottu.Domain.Entities;
using Mottu.Domain.Events;
using Mottu.Infrastructure.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mottu.Infrastructure.Messaging
{
    public class MotoConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILogger<MotoConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _exchangeName = "mottu.exchange";
        private readonly string _queueName = "moto.2024.queue";

        public MotoConsumer(IConfiguration configuration, ILogger<MotoConsumer> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

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

                // Declarar queue
                _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false).Wait();

                // Bind queue ao exchange com routing key para motos 2024
                _channel.QueueBindAsync(queue: _queueName, exchange: _exchangeName, routingKey: "moto.cadastrada").Wait();

                _logger.LogInformation("MotoConsumer configurado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar MotoConsumer");
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var motoCadastrada = JsonSerializer.Deserialize<MotoCadastradaEvent>(message);

                    if (motoCadastrada != null && motoCadastrada.Ano == 2024)
                    {
                        await ProcessarMoto2024Async(motoCadastrada);
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                        _logger.LogInformation("Mensagem processada com sucesso: {MotoId}", motoCadastrada.Identificador);
                    }
                    else
                    {
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                        _logger.LogDebug("Moto não é do ano 2024, ignorando: {Ano}", motoCadastrada?.Ano);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem");
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

            _logger.LogInformation("MotoConsumer iniciado e aguardando mensagens...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessarMoto2024Async(MotoCadastradaEvent motoCadastrada)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MottuDbContext>();

            var notificacao = new NotificacaoMoto
            {
                MotoId = motoCadastrada.Identificador,
                Modelo = motoCadastrada.Modelo,
                Ano = motoCadastrada.Ano,
                Placa = motoCadastrada.Placa,
                DataNotificacao = DateTime.UtcNow,
                Mensagem = $"Nova moto do ano 2024 cadastrada: {motoCadastrada.Modelo} - {motoCadastrada.Placa}"
            };

            dbContext.NotificacaoMotos.Add(notificacao);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Notificação de moto 2024 salva no banco: {MotoId}", motoCadastrada.Identificador);
        }

        public override void Dispose()
        {
            _channel?.CloseAsync().Wait();
            _connection?.CloseAsync().Wait();
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}

