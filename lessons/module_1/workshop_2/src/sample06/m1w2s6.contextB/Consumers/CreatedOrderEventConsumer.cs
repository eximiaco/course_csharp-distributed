namespace m1w2s6.contextB.Consumers;

public class CreatedOrderEventConsumer(ILogger<CreatedOrderEventConsumer> logger)
{
    public async Task Consume(OrderCreatedEvent order)
    {
        logger.LogInformation("Contexto B- Pedido criado evento: {OrderId}", order.OrderId);
    }
}