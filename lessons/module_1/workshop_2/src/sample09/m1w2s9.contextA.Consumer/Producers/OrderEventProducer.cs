using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace m1w2s9.contextA.Consumer.Producers;

public class OrderEventProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<OrderEventProducer> _logger;

    public OrderEventProducer(ProducerConfig config, ILogger<OrderEventProducer> logger)
    {
        _producer = new ProducerBuilder<string, string>(config).Build();
        _topic = "order-events";
        _logger = logger;
    }

    public async Task ProduceOrderCreatedEventAsync(OrderCreatedEvent orderEvent)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = orderEvent.OrderId.ToString(),
                Value = JsonSerializer.Serialize(orderEvent),
                Headers = new Headers
                {
                    { "event-type", Encoding.UTF8.GetBytes("OrderCreated") }
                }
            };

            var result = await _producer.ProduceAsync(_topic, message);

            if (result.Status == PersistenceStatus.NotPersisted)
            {
                throw new KafkaException(new Error(ErrorCode.Local_Transport, "Evento não foi persistido"));
            }

            _logger.LogInformation(
                "Evento publicado no tópico {Topic} - Partition: {Partition}, Offset: {Offset}, EventType: OrderCreated", 
                _topic, 
                result.Partition.Value, 
                result.Offset.Value
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao produzir evento OrderCreated");
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
} 