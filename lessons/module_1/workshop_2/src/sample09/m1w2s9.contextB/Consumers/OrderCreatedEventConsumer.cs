using System.Text.Json;
using Confluent.Kafka;
using m1w2s9.contextB.Configurations;
using Microsoft.Extensions.Options;

namespace m1w2s9.contextB.Consumers;

public class OrderCreatedEventConsumer(
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<OrderCreatedEventConsumer> logger)
    : BackgroundService
{
    private readonly ConsumerConfig _config = new()
    {
        BootstrapServers = kafkaSettings.Value.BootstrapServers,
        GroupId = "context-b-group",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false
    };
    private readonly string _topic = "order-events";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetErrorHandler((_, e) => 
                logger.LogError("Erro no consumer: {Reason}", e.Reason))
            .Build();

        try
        {
            consumer.Subscribe(_topic);
            logger.LogInformation("Consumer iniciado para o tópico: {Topic}", _topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    
                    if (consumeResult?.Message == null) continue;

                    logger.LogInformation(
                        "Mensagem recebida: {Key} - Partition: {Partition} - Offset: {Offset}",
                        consumeResult.Message.Key,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value);

                    var order = JsonSerializer.Deserialize<OrderCreatedEvent>(
                        consumeResult.Message.Value);

                    if (order != null)
                    {
                        // Commit manual após processamento bem-sucedido
                        consumer.Commit(consumeResult);
                        
                        logger.LogInformation(
                            "Contexto B - Pedido criado evento: {OrderId}", 
                            order.OrderId);
                    }
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(
                        ex,
                        "Erro ao consumir mensagem: {Reason}",
                        ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Erro ao processar mensagem");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignorar exceção de cancelamento normal
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro fatal no consumer");
        }
        finally
        {
            consumer.Close();
        }
    }
} 