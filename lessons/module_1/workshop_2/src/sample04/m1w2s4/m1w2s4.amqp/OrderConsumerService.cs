using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace m1w2s4.amqp;

public class OrderConsumerService(
    RabbitMqConnection rabbitConnection,
    IConfiguration configuration,
    ILogger<OrderConsumerService> logger)
    : BackgroundService
{
    private IChannel _channel = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _channel = await rabbitConnection.CreateChannelAsync();
            
            // Configuração para fila de pedidos direta
            await SetupDirectQueueConsumerAsync(stoppingToken);
            
            // Configuração para eventos de pedidos
            await SetupOrderEventsConsumerAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro no serviço de consumo");
            throw;
        }
    }

    private async Task SetupDirectQueueConsumerAsync(CancellationToken stoppingToken)
    {
        var queueName = configuration.GetSection("RabbitMQ")["QueueName"];

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<OrderRequest>(message);

                logger.LogInformation("Pedido recebido via fila direta - OrderId: {OrderId}", order?.OrderId);
                await ProcessOrderAsync(order);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar mensagem da fila direta");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);
    }

    private async Task SetupOrderEventsConsumerAsync(CancellationToken stoppingToken)
    {
        var exchangeName = "order.events";
        
        // Declara o exchange fanout
        await _channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: "fanout",
            durable: true,
            autoDelete: false,
            arguments: null);

        // Cria uma fila exclusiva e temporária para este consumidor
        var queueResult = await _channel.QueueDeclareAsync(
            queue: "",
            durable: false,
            exclusive: true,
            autoDelete: true,
            arguments: null);

        // Vincula a fila ao exchange
        await _channel.QueueBindAsync(
            queue: queueResult.QueueName,
            exchange: exchangeName,
            routingKey: "",
            arguments: null);

        var eventConsumer = new AsyncEventingBasicConsumer(_channel);
        eventConsumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                logger.LogInformation("Evento recebido - Tipo: {EventType}, OrderId: {OrderId}", 
                    orderEvent?.EventType, orderEvent?.Data?.OrderId);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar evento");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueResult.QueueName,
            autoAck: false,
            consumer: eventConsumer,
            cancellationToken: stoppingToken);
    }

    // Classe para deserialização do evento
    private class OrderCreatedEvent
    {
        public string? EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public OrderRequest? Data { get; set; }
    }

    private async Task ProcessOrderAsync(OrderRequest order)
    {
        // Simula processamento
        await Task.Delay(1000);
        logger.LogInformation("Pedido processado com sucesso - OrderId: {OrderId}", order?.OrderId);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel.CloseAsync(cancellationToken: cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}