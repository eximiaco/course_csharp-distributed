namespace m1w2s10.contextA.Consumers;

public class CreatedOrderEventConsumer(ILogger<CreatedOrderEventConsumer> logger)
{
    public async Task Consume(OrderCreatedEvent order)
    {
        logger.LogInformation("Contexto A - Pedido criado evento: {OrderId}", order.OrderId);
    }
}