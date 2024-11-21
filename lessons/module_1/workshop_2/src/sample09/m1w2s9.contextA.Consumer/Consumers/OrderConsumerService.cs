using System.Text.Json;
using Confluent.Kafka;
using m1w2s9.contextA.Consumer.Producers;
using Microsoft.Extensions.Options;

namespace m1w2s9.contextA.Consumer.Consumers;

public class OrderConsumerService(
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<OrderConsumerService> logger,
    OrderEventProducer eventProducer)
    : BackgroundService
{
    private readonly ConsumerConfig _orderConfig = new()
    {
        BootstrapServers = kafkaSettings.Value.BootstrapServers,
        GroupId = "context-a-group",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false
    };
    private readonly ConsumerConfig _eventConfig = new()
    {
        BootstrapServers = kafkaSettings.Value.BootstrapServers,
        GroupId = "context-a-group",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false
    };

    private const string OrdersTopic = "orders";
    private const string EventsTopic = "order-events";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Inicia as tasks de consumo em paralelo
        await Task.WhenAll(
            ConsumeOrdersAsync(stoppingToken),
            ConsumeCreatedOrderEventsAsync(stoppingToken)
        );
    }

    private async Task ConsumeOrdersAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_orderConfig)
            .SetErrorHandler((_, e) => logger.LogError("Erro no consumer de pedidos: {Reason}", e.Reason))
            .Build();

        try
        {
            consumer.Subscribe(OrdersTopic);
            logger.LogInformation("Consumer de pedidos iniciado: {Topic}", OrdersTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message == null) continue;

                    var order = JsonSerializer.Deserialize<OrderRequest>(consumeResult.Message.Value);
                    if (order == null) continue;

                    logger.LogInformation("Processando pedido: {OrderId}", order.OrderId);
                    
                    // Simula processamento
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    
                    // Publica evento
                    await eventProducer.ProduceOrderCreatedEventAsync(new OrderCreatedEvent 
                    { 
                        OrderId = order.OrderId 
                    });

                    consumer.Commit(consumeResult);
                    logger.LogInformation("Pedido processado: {OrderId}", order.OrderId);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Erro ao consumir pedido");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao processar pedido");
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ConsumeCreatedOrderEventsAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_eventConfig)
            .SetErrorHandler((_, e) => logger.LogError("Erro no consumer de eventos: {Reason}", e.Reason))
            .Build();

        try
        {
            consumer.Subscribe(EventsTopic);
            logger.LogInformation("Consumer de eventos iniciado: {Topic}", EventsTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message == null) continue;

                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(consumeResult.Message.Value);
                    if (orderEvent == null) continue;

                    logger.LogInformation("Contexto A - Processando pedido criado: {OrderId}", orderEvent.OrderId);
                    
                    // Simula processamento do evento
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                    consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Erro ao consumir evento");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao processar evento");
                }
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}