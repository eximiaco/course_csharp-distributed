using m1w2s6;
using m1w2s6.contextA;

namespace M1W2S6.ContextA.Consumers;

public class CreatedOrderEventConsumer(ILogger<CreatedOrderEventConsumer> logger)
{
    public async Task Consume(OrderCreatedEvent order)
    {
        logger.LogInformation("Contexto A - Pedido criado evento: {OrderId}", order.OrderId);
    }
}