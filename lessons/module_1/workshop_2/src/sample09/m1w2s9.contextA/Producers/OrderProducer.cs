using Confluent.Kafka;
using System.Text.Json;
using m1w2s9.contextA;

namespace M1W2S9.ContextA.Producers;

public class OrderProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<OrderProducer> _logger;

    public OrderProducer(ProducerConfig config, ILogger<OrderProducer> logger)
    {
        _producer = new ProducerBuilder<string, string>(config).Build();
        _topic = "orders";
        _logger = logger;
    }

    public async Task ProduceOrderAsync(OrderRequest order)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = order.OrderId.ToString(),
                Value = JsonSerializer.Serialize(order)
            };

            var result = await _producer.ProduceAsync(_topic, message);

            if (result.Status == PersistenceStatus.NotPersisted)
            {
                throw new KafkaException(new Error(ErrorCode.Local_Transport, "Mensagem não foi persistida"));
            }

            _logger.LogInformation(
                "Mensagem enviada para o tópico {Topic} - Partition: {Partition}, Offset: {Offset}", 
                _topic, 
                result.Partition.Value, 
                result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, 
                "Erro ao produzir mensagem para o tópico {Topic}. Error Code: {ErrorCode}", 
                _topic, 
                ex.Error.Code);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao produzir mensagem para o tópico {Topic}", _topic);
            throw;
        }
    }

    public void Dispose()
    {
        try
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer dispose do producer");
        }
    }
} 